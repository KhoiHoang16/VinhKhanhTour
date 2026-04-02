namespace VinhKhanhTour.Shared.Models
{
    // DTO dùng cho POST/PUT — Frontend gửi lên
    public class TourCreateUpdateDto
    {
        public string TourName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public double TotalDistance { get; set; }
        public List<TourStopDto> Stops { get; set; } = new();
    }

    public class TourStopDto
    {
        public int PoiId { get; set; }
        public string PoiName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OrderIndex { get; set; }
    }

    // DTO dùng cho GET — API trả về cho Frontend
    public class TourDetailDto
    {
        public int Id { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public double TotalDistance { get; set; }
        public List<TourStopDto> Stops { get; set; } = new();
    }
}
