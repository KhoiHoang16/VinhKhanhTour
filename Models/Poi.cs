using SQLite;

namespace VinhKhanhTour.Models
{
    public class Poi
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        /// <summary>
        /// Bán kính kích hoạt tính bằng mét (m)
        /// </summary>
        public double Radius { get; set; } 
        
        /// <summary>
        /// Mức độ ưu tiên để xử lý khi đứng giữa nhiều POI
        /// </summary>
        public int Priority { get; set; }

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung văn bản sẽ được dùng để Auto-play Thuyết minh (Text to Speech)
        /// </summary>
        public string TtsScript { get; set; } = string.Empty;

        // Phương thức cung cấp danh sách 5 quán ốc mẫu ở phố Vĩnh Khánh
        public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                new Poi 
                { 
                    Id = 1, 
                    Name = "Ốc Oanh", 
                    Latitude = 10.760300, 
                    Longitude = 106.703300, 
                    Radius = 50, 
                    Description = "Quán ốc lớn bậc nhất phố Vĩnh Khánh, nổi tiếng với món ốc hương rang muối ớt đậm đà, cua rang me nguyên con béo ngậy. Luôn nhộn nhịp khách khứa vào mỗi chiều tối.",
                    TtsScript = "Chào mừng bạn đến với Ốc Oanh. Đây là quán ốc lớn và lâu đời bậc nhất con phố Vĩnh Khánh. Thử ngay ốc hương rang muối ớt nhé!",
                    Priority = 1
                },
                new Poi 
                { 
                    Id = 2, 
                    Name = "Ốc Vũ", 
                    Latitude = 10.761800, 
                    Longitude = 106.702000, 
                    Radius = 40, 
                    Description = "Ốc Vũ với không gian thoáng mát, menu đa dạng. Món đặc trưng ở đây là nghêu hấp sả và sò điệp nướng phô mai béo ngậy. Rất được lòng giới trẻ Sài Gòn.",
                    TtsScript = "Bạn đang ở gần Ốc Vũ. Không gian thoáng mát, nghêu hấp sả cực đỉnh đang đợi bạn.",
                    Priority = 2
                },
                new Poi 
                { 
                    Id = 3, 
                    Name = "Ốc Nho", 
                    Latitude = 10.761300, 
                    Longitude = 106.702500, 
                    Radius = 45, 
                    Description = "Với mức giá bình dân và cách nêm nếm đậm đà kiểu Nam Bộ, Ốc Nho thu hút sự chú ý với món càng ghẹ rang muối và ốc móng tay xào rau muống đầy hấp dẫn.",
                    TtsScript = "Ốc Nho với mức giá bình dân và càng ghẹ rang muối siêu ngon ở ngay bên cạnh bạn.",
                    Priority = 3
                },
                new Poi 
                { 
                    Id = 4, 
                    Name = "Ốc Thảo", 
                    Latitude = 10.759700, 
                    Longitude = 106.703800, 
                    Radius = 40, 
                    Description = "Quán ốc lâu năm với công thức xào bơ tỏi gia truyền vô cùng cuốn hút ăn kèm với bánh mì nướng. Nghêu hấp thái chua cay ở đây cũng là một trải nghiệm tuyệt vời.",
                    TtsScript = "Hương thơm bơ tỏi gia truyền tỏa ra kìa! Ốc Thảo nằm ngay góc đây thôi.",
                    Priority = 4
                },
                new Poi 
                { 
                    Id = 5, 
                    Name = "Ốc Sáu Nở", 
                    Latitude = 10.760800, 
                    Longitude = 106.702800, 
                    Radius = 35, 
                    Description = "Quán có không gian vỉa hè đúng chuẩn Sài Gòn. Các món ốc xào sa tế hay nướng mỡ hành ở Ốc Sáu Nở luôn được giữ nóng hổi, thịt ốc dai giòn sần sật.",
                    TtsScript = "Làm chút ốc nướng mỡ hành vỉa hè đúng điệu Sài Gòn tại Ốc Sáu Nở nào.",
                    Priority = 5
                }
            };
        }
    }
}
