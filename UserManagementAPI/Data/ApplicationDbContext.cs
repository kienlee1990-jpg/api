using FastFoodAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ================= DbSet =================
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Combo> Combos { get; set; }
    public DbSet<ComboFood> ComboFoods { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =====================================================
        // RefreshToken
        // =====================================================
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Token)
                  .IsUnique();

            entity.HasOne(x => x.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Category
        // =====================================================
        builder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();
        });

        // =====================================================
        // Food
        // =====================================================
        builder.Entity<Food>(entity =>
        {
            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.HasOne(x => x.Category)
                  .WithMany(c => c.Foods)
                  .HasForeignKey(x => x.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // ComboFood (N-N giữa Combo và Food) ⭐ QUAN TRỌNG
        // =====================================================
        builder.Entity<ComboFood>(entity =>
        {
            entity.HasKey(x => new { x.ComboId, x.FoodId });

            entity.HasOne(x => x.Combo)
                  .WithMany(c => c.ComboFoods)
                  .HasForeignKey(x => x.ComboId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Food)
                  .WithMany(f => f.ComboFoods)
                  .HasForeignKey(x => x.FoodId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // OrderItem (nếu bạn dùng FK)
        // =====================================================
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(x => x.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}