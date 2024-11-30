using INDUENDUM_API.Models; // Namespace për modelet
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Për konfigurimin e identitetit
using Microsoft.EntityFrameworkCore;

namespace INDUENDUM_API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> // Trashëgo nga IdentityDbContext për ApplicationUser
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet për modelet ekzistuese
        public DbSet<Product> Products { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<AdRequest> AdRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Thirrni konfigurimet bazë për identitetin
            base.OnModelCreating(modelBuilder);

            // Relacioni many-to-many midis Collection dhe Product
            modelBuilder.Entity<Collection>()
                .HasMany(c => c.Products)
                .WithMany(p => p.Collections)
                .UsingEntity(j => j.ToTable("CollectionProducts"));

            // Relacioni One-to-Many midis Product dhe ApplicationUser (Company)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict për të parandaluar kaskadimin e fshirjes

            // Relacioni One-to-Many midis AdRequest dhe ApplicationUser (Company)
            modelBuilder.Entity<AdRequest>()
                .HasOne(ar => ar.Company)
                .WithMany()
                .HasForeignKey(ar => ar.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict për të parandaluar fshirjen e lidhjeve

            // Precizimi për fushën monetare 'Price' në Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            // Shto role të paracaktuara
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );

            // Shto përdorues të paracaktuar
            var hasher = new PasswordHasher<ApplicationUser>();
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "1", // ID e përdoruesit
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "AdminPassword123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FullName = "Administrator"
                }
            );

            // Lidh përdoruesin e paracaktuar me rolin Admin
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "1", // ID e rolit Admin
                    UserId = "1"  // ID e përdoruesit admin
                }
            );
        }
    }
}
