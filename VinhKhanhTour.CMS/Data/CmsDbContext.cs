using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }

        public DbSet<Poi> Pois { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourStop> TourStops { get; set; }
        public DbSet<UsageHistory> UsageHistories { get; set; }
        public DbSet<UserRoute> UserRoutes { get; set; }
        public DbSet<AppAuditLog> AppAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map POI
            var poi = modelBuilder.Entity<Poi>();
            poi.HasKey(p => p.Id);
            poi.Property(p => p.Id).ValueGeneratedOnAdd();
            // Ignore properties decorated with [Ignore] from SQLite or derived properties
            poi.Ignore(p => p.DisplayImage);
            poi.Ignore(p => p.DisplayDistanceText);
            poi.Ignore(p => p.ListDisplayDistanceText);
            poi.Ignore(p => p.DisplayRadiusText);
            poi.Ignore(p => p.ListDisplayRadiusText);
            poi.Ignore(p => p.DynamicPrimaryCategory);
            poi.Ignore(p => p.DisplayCategory);
            poi.Ignore(p => p.ShortDisplayCategory);
            poi.Ignore(p => p.DisplayName);
            poi.Ignore(p => p.DisplayDescription);
            poi.Ignore(p => p.DisplayTtsScript);
            // Ignore observable properties from CommunityToolkit.Mvvm
            poi.Ignore(p => p.DistanceToUser);
            
            modelBuilder.Entity<Tour>().HasKey(t => t.Id);
            modelBuilder.Entity<Tour>().Property(t => t.Id).ValueGeneratedOnAdd();
            // Cấu hình quan hệ 1-N: Tour -> TourStops
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Stops)
                .WithOne(ts => ts.Tour)
                .HasForeignKey(ts => ts.TourId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Tour -> xóa luôn các Stops

            modelBuilder.Entity<TourStop>().HasKey(ts => ts.Id);
            modelBuilder.Entity<TourStop>().Property(ts => ts.Id).ValueGeneratedOnAdd();
            
            modelBuilder.Entity<UsageHistory>().HasKey(h => h.Id);
            modelBuilder.Entity<UsageHistory>().Property(h => h.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRoute>().HasKey(ur => ur.Id);
            modelBuilder.Entity<UserRoute>().Property(ur => ur.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AppAuditLog>().HasKey(al => al.Id);
            modelBuilder.Entity<AppAuditLog>().Property(al => al.Id).ValueGeneratedOnAdd();
        }
    }
}
