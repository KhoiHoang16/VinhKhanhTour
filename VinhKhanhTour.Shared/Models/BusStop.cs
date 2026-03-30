namespace VinhKhanhTour.Shared.Models
{
    public class BusStop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<int> AssociatedPoiIds { get; set; } = new();

        /// <summary>
        /// Returns the 3 predefined bus stops in the Vĩnh Khánh - Khánh Hội area
        /// </summary>
        public static List<BusStop> GetBusStops()
        {
            return new List<BusStop>
            {
                new BusStop
                {
                    Id = 1,
                    Name = "Trạm xe buýt Khánh Hội",
                    Description = "Trạm dừng xe buýt trên đường Khánh Hội, Quận 4 - gần các quán ốc nổi tiếng",
                    Latitude = 10.7610,
                    Longitude = 106.7025,
                    AssociatedPoiIds = new List<int> { 1, 2, 4 }
                },
                new BusStop
                {
                    Id = 2,
                    Name = "Trạm xe buýt Vĩnh Hội",
                    Description = "Trạm dừng xe buýt trên đường Vĩnh Hội, Quận 4 - khu vực lẩu bò và ẩm thực",
                    Latitude = 10.7605,
                    Longitude = 106.7040,
                    AssociatedPoiIds = new List<int> { 3, 4 }
                },
                new BusStop
                {
                    Id = 3,
                    Name = "Trạm xe buýt Xóm Chiếu",
                    Description = "Trạm dừng xe buýt khu vực Xóm Chiếu, Quận 4 - chợ truyền thống và ẩm thực địa phương",
                    Latitude = 10.7595,
                    Longitude = 106.7055,
                    AssociatedPoiIds = new List<int> { 1, 2, 3, 4 }
                }
            };
        }
    }
}
