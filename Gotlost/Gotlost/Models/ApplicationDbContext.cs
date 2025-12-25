using Gotlost.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<FoundItem> FoundItems { get; set; }
    public DbSet<ContactUs> ContactUsMessages { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       

        modelBuilder.Entity<Claim>()
            .HasOne(c => c.FoundItem)
            .WithMany(f => f.Claims)
            .HasForeignKey(c => c.FoundItemId)
            .OnDelete(DeleteBehavior.Restrict);  // Disable cascade delete

        modelBuilder.Entity<Claim>()
            .HasOne(c => c.User)
            .WithMany(u => u.Claims)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data if necessary (e.g., Users)
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, FullName = "Adil Masood", Email = "aadilbhatti623@gamil.com", PhoneNumber = "03339571670", ProfilePicturePath = "~/content/images/users/alice.jpg", Password = "adil123" },
            new User { UserId = 2, FullName = "Abdullah", Email = "abdullahsyed6370@gmail.com", PhoneNumber = "03123221493", ProfilePicturePath = "~/content/images/users/bob.jpg", Password = "abdullah123" }
        );
    }

}
