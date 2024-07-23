using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence
{
    public class ShoppingAppDbContext : IdentityDbContext<AppUser>
    {
        public ShoppingAppDbContext(DbContextOptions<ShoppingAppDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Product> Products { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

			builder.Entity<Image>()
				.Property(e => e.Id)
				.ValueGeneratedOnAdd();

			builder.Entity<Product>()
				.Property(e => e.Id)
				.ValueGeneratedOnAdd();

			builder.Entity<Cart>()
				.Property(e => e.Id)
				.ValueGeneratedOnAdd();

			builder.Entity<Item>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();


            builder.Entity<Cart>()
                .HasOne(x => x.User)
                .WithOne(x => x.Cart)
                .HasForeignKey<Cart>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Item>()
                .HasOne(x => x.Cart)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

			builder.Entity<Item>()
	            .HasOne(x => x.Product)
	            .WithMany(x => x.CartItems)
	            .HasForeignKey(x => x.ProductId)
	            .OnDelete(DeleteBehavior.Cascade)
	            .IsRequired();

            builder.Entity<Image>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
		}
    }
}
