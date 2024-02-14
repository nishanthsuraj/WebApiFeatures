using Microsoft.EntityFrameworkCore;
using WebApiFeatures.Models;
using WebApiFeatures.Utilities;

namespace WebApiFeatures.Db
{
    public class ShopContext : DbContext
    {
        #region Public Properties
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        #endregion

        #region Constructor
        public ShopContext(DbContextOptions<ShopContext> options)
            : base(options)
        {

        }
        #endregion

        #region Overridden Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Seed();
        }
        #endregion
    }
}
