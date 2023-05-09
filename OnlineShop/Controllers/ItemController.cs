using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(_dbContext.Items.Include(i => i.Category));
        }


        public IActionResult Create()
        {
            ItemVM itemVM = new ItemVM() {
                Item = new Item(),
                SexSelectList = this.SexSelectList,
                CategorySelectList = this.CategorySelectList,
                SizeVMList = this.SizeVMList
            };

            return View(itemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemVM itemVM)
        {
            if (!ModelState.IsValid)
            {
                itemVM.SexSelectList = this.SexSelectList;
                itemVM.CategorySelectList = this.CategorySelectList;
                return View(itemVM);
            }

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;

                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                itemVM.Item.Image = fileName + extension;
            }

            List<SizedItem> sizedItems = new List<SizedItem>();
            foreach (var sizeVM in itemVM.SizeVMList)
            {
                if (sizeVM.Checked)
                    sizedItems.Add(new SizedItem { SizeId = sizeVM.Size.Code, Amount = sizeVM.Amount });
            }
            itemVM.Item.SizedItems = sizedItems;

            await _dbContext.AddAsync(itemVM.Item);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Item {itemVM.Item.Name} added succesfully!";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var itemVM = await GetItemVM(id ?? 0);

            if (itemVM == null)
            {
                return NotFound();
            }

            return View(itemVM);
        }


        public async Task<EditItemVM> GetItemVM(int id)
        {
            var item = await _dbContext.Items.FindAsync(id);
            var sizedItems = _dbContext.SizedItems.Where(si => (si.ItemId == id)).Include(si => si.Size);

            if (item == null)
            {
                return null;
            }

            EditItemVM itemVM = new EditItemVM()
            {
                Item = item,
                SexSelectList = this.SexSelectList,
                CategorySelectList = this.CategorySelectList,
                SizeVMList = this.SizeVMList,
                RemoveImage = false
            };

            foreach (var sizeVM in itemVM.SizeVMList)
            {
                var sizeditem = await sizedItems.FirstOrDefaultAsync(si => si.SizeId == sizeVM.Size.Code);
                if (sizeditem != null)
                {
                    sizeVM.Amount = sizeditem.Amount;
                    sizeVM.Checked = true;
                }
            }

            return itemVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditItemVM itemVM)
        {
            if (!ModelState.IsValid)
            {
                itemVM.SexSelectList = this.SexSelectList;
                itemVM.CategorySelectList = this.CategorySelectList;
                return View(itemVM);
            }

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0 || itemVM.RemoveImage)
            {
                if (itemVM.Item.Image != null)
                {
                    var path = $"{_webHostEnvironment.WebRootPath}{WC.ImagePath}{itemVM.Item.Image}";
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            // TODO: syncronize
                            System.IO.File.Delete(path);
                        }
                        catch (Exception ex)
                        {
                            System.Console.Error.WriteLine(ex.Message.ToString());
                        }
                    }
                }

                itemVM.Item.Image = null;

            }

            if (files.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;

                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                itemVM.Item.Image = fileName + extension;
            }

            //_dbContext.SizedItems.RemoveRange(_dbContext.SizedItems.Where(si => si.ItemId == itemVM.Item.Id));
            List<SizedItem> sizedItems = new List<SizedItem>();
            foreach (var sizeVM in itemVM.SizeVMList)
            {
                var sizedItem = await _dbContext.SizedItems.FirstOrDefaultAsync(si => si.ItemId == itemVM.Item.Id && si.SizeId == sizeVM.Size.Code);

                if (sizeVM.Checked)
                {
                    if (sizedItem != null)
                    {
                        sizedItem.Amount = sizeVM.Amount;
                        _dbContext.SizedItems.Update(sizedItem);
                    }
                    else
                        _dbContext.SizedItems.Add(new SizedItem { SizeId = sizeVM.Size.Code, Amount = sizeVM.Amount, ItemId = itemVM.Item.Id });
                }
                else
                {
                    if (sizedItem != null)
                        _dbContext.SizedItems.Remove(sizedItem);
                }
            }

            _dbContext.Items.Update(itemVM.Item);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Item {itemVM.Item.Name} edited succesfully!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var item = await _dbContext.Items.Include(i => i.Category).FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var sizedItems = _dbContext.SizedItems.Include(si => si.Size).Where(si => si.ItemId == id);
            item.SizedItems = await sizedItems.ToListAsync();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var item = await _dbContext.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }


            if (item.Image != null)
            {
                var path = $"{_webHostEnvironment.WebRootPath}{WC.ImagePath}{item.Image}";
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        // TODO: syncronize
                        System.IO.File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        System.Console.Error.WriteLine(ex.Message.ToString());
                    }
                }
            }

            _dbContext.Items.Remove(item);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Item {item.Name} deleted succesfully!";

            return RedirectToAction(nameof(Index));
        }

        public IEnumerable<SelectListItem> SexSelectList
        {
            get
            {
                return new List<Sex>() { Sex.Male, Sex.Female, Sex.Unisex }.Select(s => new SelectListItem
                {
                    Text = s.ToString(),
                    Value = ((byte) s).ToString()
                });
            }
        }

        public IEnumerable<SelectListItem> CategorySelectList 
        {
            get
            {
                return _dbContext.Categories.Select(
               c => new SelectListItem
               {
                   Text = c.Name,
                   Value = c.Code.ToString()
               });
            } 
        }

        public List<SizeVM> SizeVMList
        {
            get
            {
                var sizes = new List<SizeVM>();
                foreach(var size in _dbContext.Sizes)
                {
                    sizes.Add(new SizeVM()
                    {
                        Size = size,
                        Checked = false,
                        Amount = 0
                    }) ;
                }

                return sizes;
            }
        }

    }
}
