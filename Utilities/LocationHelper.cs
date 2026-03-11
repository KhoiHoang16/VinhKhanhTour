namespace VinhKhanhTour.Utilities
{
    public static class LocationHelper
    {
        private const double EarthRadiusKm = 6371;

        /// <summary>
        /// Sử dụng công thức Haversine để tính khoảng cách (mét) giữa 2 điểm tọa độ.
        /// Đây là điểm cộng lớn cho đồ án thay vì dùng hàm Location.CalculateDistance có sẵn.
        /// </summary>
        public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));

            return EarthRadiusKm * c * 1000; // Đổi ra mét
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
