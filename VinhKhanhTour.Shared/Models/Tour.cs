using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VinhKhanhTour.Shared.Models
{
    public class Tour
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class TourStop
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int TourId { get; set; }
        public int PoiId { get; set; }
        public int OrderIndex { get; set; }
    }
}
