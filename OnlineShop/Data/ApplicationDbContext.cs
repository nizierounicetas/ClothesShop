using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SizedItem> SizedItems { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OrderedSizedItem> OrderedSizedItems { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
