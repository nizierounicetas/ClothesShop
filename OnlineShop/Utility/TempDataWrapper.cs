using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace OnlineShop.Utility
{
    public class TempDataWrapper
    {
        private readonly ITempDataDictionary TempData;
        private readonly ViewDataDictionary ViewData;

        public TempDataWrapper(ITempDataDictionary TempData, ViewDataDictionary ViewData)
        {
            this.TempData = TempData;
            this.ViewData = ViewData;
        }

        public void PutMessage(string message)
        {
            TempData["Message"] = message;
        }

        public void PutErrorMessage(string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        public void SetViewBagMessage()
        {
            if (TempData["Message"] != null)
            {
                ViewData["Message"] = TempData["Message"];
                TempData.Remove("Message");
            }
        }

        public void SetViewBagErrorMessage()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                TempData.Remove("ErrorMessage");
            }
        }
    }
}
