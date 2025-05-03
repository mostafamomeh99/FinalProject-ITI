using finalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.StoredprocMapping;

namespace DatabaseConnection
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripSites> TripSites { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Government> Governments { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Transportation> Transportations { get; set; }
        public DbSet<SiteImage> SiteImages { get; set; }
        public DbSet<TripImage> TripImages { get; set; }
        public DbSet<TripExcluded> TripExcludeds { get; set; }
        public DbSet<TripIncluded> TripIncludeds { get; set; }
        public DbSet<TripSiteDetailDto> TripSiteDetails { get; set; }
        //blog dbset
        public DbSet<Author> Authors { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // stored-procuders
            modelBuilder.Entity<TripSiteDetailDto>().HasNoKey();



            // Rating

            modelBuilder.Entity<Rating>().HasKey(u => new {u.TripId , u.ApplicationUserId});

            modelBuilder.Entity<Rating>()
                .HasOne(b => b.Trip)
                .WithMany(t => t.Ratings)
                .HasForeignKey(b => b.TripId);

            modelBuilder.Entity<Rating>()
                          .HasOne(b => b.ApplicationUser)
                          .WithMany(t => t.Ratings)
                          .HasForeignKey(b => b.ApplicationUserId);

            modelBuilder.Entity<Rating>()
                            .Property(r => r.RateNumber)
                             .HasDefaultValue(5);

            // trip
            modelBuilder.Entity<Trip>()
    .Property(p => p.Money)
    .HasColumnType("decimal(10,2)");

            // book
            modelBuilder.Entity<Book>()
.Property(p => p.AmountMoney)
.HasColumnType("decimal(10,2)");

            //tripsite
            modelBuilder.Entity<TripSites>().HasKey(u => new { u.TripId, u.SiteId });

            modelBuilder.Entity<TripSites>()
                .HasOne(b => b.Trip)
                .WithMany(t => t.TripSites)
                .HasForeignKey(b => b.TripId);

            modelBuilder.Entity<TripSites>()
                          .HasOne(b => b.Site)
                          .WithMany(t => t.TripSites)
                          .HasForeignKey(b => b.SiteId);

            // roles

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "A1B2C3D4-E5F6-7890-1234-56789ABCDEF0",
                    Name = "Admin",
                    ConcurrencyStamp = "admin-role-stamp",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id ="B1C2D3E4-F5G6-7890-2345-67890ABCDEF1",
                    Name = "User",
                    ConcurrencyStamp = "user-role-stamp",
                    NormalizedName = "User"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // for admin

            var adminUser = new ApplicationUser
            {
                Id = "admin-1234-5678-9101-abcdef",
                UserName = "admin_number1",
                NormalizedUserName = "ADMIN_NUMBER1",
                Email = "admin.shop1@gmail.com",
                NormalizedEmail = "ADMIN.SHOP1@GMAIL.COM",
                PasswordHash = "AQAAAAEAACcQAAAAEK0fKeXaq1cZ+W5mYeQ==",
                EmailConfirmed = true,
                SecurityStamp = "admin-user-security-stamp",
                ConcurrencyStamp = "admin-1234-5678-9101-abcdef"
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            /// admin - role
            var adminUserRole = new IdentityUserRole<string>
            {
                UserId = "admin-1234-5678-9101-abcdef", 
                RoleId = "A1B2C3D4-E5F6-7890-1234-56789ABCDEF0" 
            };

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);

        }


    }
}
