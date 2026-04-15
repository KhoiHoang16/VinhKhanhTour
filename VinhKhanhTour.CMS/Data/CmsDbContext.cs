using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Interfaces;
using VinhKhanhTour.CMS.Services;

using Microsoft.Extensions.DependencyInjection;

namespace VinhKhanhTour.CMS.Data
{
    public class CmsDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        [ActivatorUtilitiesConstructor]
        public CmsDbContext(DbContextOptions<CmsDbContext> options, ICurrentUserService currentUserService) : base(options) 
        { 
            _currentUserService = currentUserService;
        }

        // For testing/mocking where service is not injected
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }

        public DbSet<Poi> Pois { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourStop> TourStops { get; set; }
        public DbSet<UsageHistory> UsageHistories { get; set; }
        public DbSet<UserRoute> UserRoutes { get; set; }
        public DbSet<AppAuditLog> AppAuditLogs { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppNotification> AppNotifications { get; set; }
        public DbSet<DevicePurchase> DevicePurchases { get; set; }
        public DbSet<Tourist> Tourists { get; set; }

        // EXTREMELY IMPORTANT: Properties must be evaluated dynamically per DbContext instance query, NOT baked into OnModelCreating
        private int? CurrentAgencyId => _currentUserService?.AgencyId ?? -1;
        private bool IsSystemAdmin => _currentUserService?.IsSystemAdmin ?? false;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map POI
            var poi = modelBuilder.Entity<Poi>();
            poi.HasKey(p => p.Id);
            poi.Property(p => p.Id).ValueGeneratedOnAdd();
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
            poi.Ignore(p => p.DistanceToUser);
            poi.Ignore(p => p.DisplayApprovalBadge);
            
            modelBuilder.Entity<Tour>().HasKey(t => t.Id);
            modelBuilder.Entity<Tour>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Stops)
                .WithOne(ts => ts.Tour)
                .HasForeignKey(ts => ts.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TourStop>().HasKey(ts => ts.Id);
            modelBuilder.Entity<TourStop>().Property(ts => ts.Id).ValueGeneratedOnAdd();
            
            modelBuilder.Entity<UsageHistory>().HasKey(h => h.Id);
            modelBuilder.Entity<UsageHistory>().Property(h => h.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRoute>().HasKey(ur => ur.Id);
            modelBuilder.Entity<UserRoute>().Property(ur => ur.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AppAuditLog>().HasKey(al => al.Id);
            modelBuilder.Entity<AppAuditLog>().Property(al => al.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AppUser>().HasKey(u => u.Id);
            modelBuilder.Entity<AppUser>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<AppNotification>().HasKey(n => n.Id);
            modelBuilder.Entity<AppNotification>().Property(n => n.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<AppNotification>().HasIndex(n => n.RecipientAgencyId);

            // Tourist
            modelBuilder.Entity<Tourist>().HasKey(t => t.Id);
            modelBuilder.Entity<Tourist>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Tourist>().HasIndex(t => t.SocialId).IsUnique();
            modelBuilder.Entity<Tourist>().HasIndex(t => t.Email);

            // DevicePurchase
            modelBuilder.Entity<DevicePurchase>().HasKey(d => d.Id);
            modelBuilder.Entity<DevicePurchase>().Property(d => d.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<DevicePurchase>().HasIndex(d => new { d.DeviceId, d.PoiId });
            modelBuilder.Entity<DevicePurchase>().HasIndex(d => d.RecoveryContact);
            modelBuilder.Entity<DevicePurchase>().HasIndex(d => d.TouristId);

            // Relationship: Tourist → nhiều DevicePurchase
            modelBuilder.Entity<DevicePurchase>()
                .HasOne<Tourist>()
                .WithMany()
                .HasForeignKey(d => d.TouristId)
                .OnDelete(DeleteBehavior.SetNull);

            // -------------------------------------------------------------
            // GLOBAL QUERY FILTERS & INDEXES FOR AGENCY ISOLATION
            // -------------------------------------------------------------
            var tourEntity = modelBuilder.Entity<Tour>();
            var userHistoryEntity = modelBuilder.Entity<UsageHistory>();
            var poiEntity = modelBuilder.Entity<Poi>();

            poiEntity.HasIndex(p => p.AgencyId);
            tourEntity.HasIndex(t => t.AgencyId);
            userHistoryEntity.HasIndex(h => h.AgencyId);

            // Bỏ mệnh đề OR để Index chạy tối đa chỉ khi chắc chắn dùng .IgnoreQueryFilters()
            // Nhưng để hệ thống vững vàng & an toàn ngay lập tức cho Admin, ta tích hợp this.IsSystemAdmin
            // EF Core dịch this.IsSystemAdmin thành Parameter, không làm mất hiệu năng của Postgres.
            poiEntity.HasQueryFilter(p => (this.IsSystemAdmin || p.AgencyId == this.CurrentAgencyId) && !p.IsDeleted);
            tourEntity.HasQueryFilter(t => (this.IsSystemAdmin || t.AgencyId == this.CurrentAgencyId) && !t.IsDeleted);
            userHistoryEntity.HasQueryFilter(h => this.IsSystemAdmin || h.AgencyId == this.CurrentAgencyId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_currentUserService != null)
            {
                foreach (var entry in ChangeTracker.Entries<IMustHaveAgency>().Where(e => e.State == EntityState.Added))
                {
                    if (!_currentUserService.IsSystemAdmin && _currentUserService.AgencyId.HasValue)
                    {
                        entry.Entity.AgencyId = _currentUserService.AgencyId.Value;
                    }
                }
            }

            // Tự động set ApprovalStatus = Pending khi Agency (non-Admin) sửa POI
            // Theo góp ý: không áp dụng khi Admin sửa
            if (_currentUserService != null && !_currentUserService.IsSystemAdmin)
            {
                foreach (var entry in ChangeTracker.Entries<Poi>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    entry.Entity.ApprovalStatus = ApprovalStatus.Pending;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDelete>().Where(e => e.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
