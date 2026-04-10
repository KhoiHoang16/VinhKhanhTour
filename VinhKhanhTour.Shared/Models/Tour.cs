using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using VinhKhanhTour.Shared.Interfaces;

namespace VinhKhanhTour.Shared.Models
{
    public class Tour : IMustHaveAgency, ISoftDelete
    {
        public int? AgencyId { get; set; }
        public bool IsDeleted { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // Thông tin lộ trình
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public double TotalDistance { get; set; } // Quãng đường (km)

        // Navigation Property — EF Core sử dụng, SQLite bỏ qua
        [SQLite.Ignore]
        public List<TourStop> Stops { get; set; } = new();
    }

    public class TourStop
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int TourId { get; set; }
        public int PoiId { get; set; }
        public string PoiName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OrderIndex { get; set; }

        // Navigation Property
        [SQLite.Ignore]
        public Tour? Tour { get; set; }
    }
}
