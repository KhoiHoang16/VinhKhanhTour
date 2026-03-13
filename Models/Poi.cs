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

        /// <summary>
        /// Đường dẫn tới hình ảnh minh họa cho POI
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        // ============================================================
        // 13 quán ăn THẬT trên đường Vĩnh Khánh, Quận 4
        // Tọa độ xác minh trực tiếp từ Google Maps (03/2026)
        // Đường VK chạy hình chữ S từ ĐB (Hoàng Diệu) → TN (Kênh Tẻ)
        // ============================================================
        public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                // ─── ĐẦU PHỐ (phía Hoàng Diệu, số nhà nhỏ) ───
                new Poi
                {
                    Id = 1,
                    Name = "Quán Ốc Vũ - 37 Vĩnh Khánh",
                    Description = "Quán ốc bình dân nổi tiếng với các món xào bơ tỏi thơm lừng. Rất được lòng giới trẻ Sài Gòn.",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = "Ốc Vũ là điểm đến yêu thích của giới trẻ. Đừng bỏ qua món ốc móng tay xào bơ tỏi nhé.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 2,
                    Name = "Ốc Phát Quán - 46 Vĩnh Khánh",
                    Description = "Ốc len xào dừa và các món sa tế cay nồng là niềm tự hào của quán. Giá cả phải chăng.",
                    Latitude = 10.76027,
                    Longitude = 106.70301,
                    Radius = 35,
                    TtsScript = "Ốc Phát Quán số 46. Thử món ốc len xào dừa với vị béo ngậy khó cưỡng nha.",
                    Priority = 2,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 3,
                    Name = "Quán Bé Ốc - 58 Vĩnh Khánh",
                    Description = "Quán ốc nhỏ xinh, chuyên các loại ốc hấp sả và nướng mỡ hành. Phù hợp nhóm bạn.",
                    Latitude = 10.76339,
                    Longitude = 106.70206,
                    Radius = 30,
                    TtsScript = "Quán Bé Ốc số 58, nhỏ nhưng mà hương vị cực kỳ đậm đà đang chờ bạn.",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },

                // ─── KHU VỰC GIỮA (đoạn cong: số nhà 100-200) ───
                new Poi
                {
                    Id = 4,
                    Name = "Sushi Ko - 122 Vĩnh Khánh",
                    Description = "Sushi vỉa hè đẳng cấp nhà hàng. Giá cả bình dân, cá tươi ngon mỗi ngày.",
                    Latitude = 10.76073,
                    Longitude = 106.70468,
                    Radius = 25,
                    TtsScript = "Sushi Ko mang đến trải nghiệm ẩm thực Nhật Bản ngay trên vỉa hè. Số 122 Vĩnh Khánh đây rồi.",
                    Priority = 4,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },
                new Poi
                {
                    Id = 5,
                    Name = "Ốc Đào 2 - 123 Vĩnh Khánh",
                    Description = "Chi nhánh Ốc Đào Nguyễn Trãi danh tiếng. Nước xốt bơ tỏi gây nghiện, hải sản tươi sống.",
                    Latitude = 10.76114,
                    Longitude = 106.70498,
                    Radius = 35,
                    TtsScript = "Ốc Đào 2 số 123 Vĩnh Khánh. Nước xốt bơ tỏi ăn kèm bánh mì ở đây cực kỳ gây nghiện.",
                    Priority = 5,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 6,
                    Name = "Ốc Nhớ Sài Gòn - 159 Vĩnh Khánh",
                    Description = "Menu nhiều món sáng tạo, phục vụ nhanh nhẹn. Không gian mở thoáng mát, rất đông vào buổi tối.",
                    Latitude = 10.76120,
                    Longitude = 106.70540,
                    Radius = 30,
                    TtsScript = "Ốc Nhớ Sài Gòn - cái tên nói lên tất cả. Bạn sẽ nhớ mãi hương vị tại đây.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 7,
                    Name = "Bánh Flan Ngọc Nga - 167 Vĩnh Khánh",
                    Description = "Bánh flan béo mịn tan ngay đầu lưỡi, nước caramel đậm đà. Món tráng miệng phải thử sau ốc.",
                    Latitude = 10.76232,
                    Longitude = 106.70316,
                    Radius = 30,
                    TtsScript = "Nghỉ chân ăn bánh flan Ngọc Nga nhé. Béo mịn, ngọt vừa, tuyệt vời sau một bữa ốc.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },

                // ─── KHU VỰC 200-300 ───
                new Poi
                {
                    Id = 8,
                    Name = "Ốc Su 20k - 225 Vĩnh Khánh",
                    Description = "Ốc đồng giá 20k cực rẻ, phù hợp cho sinh viên muốn ăn nhiều món với chi phí thấp.",
                    Latitude = 10.76056,
                    Longitude = 106.70396,
                    Radius = 30,
                    TtsScript = "Ốc Su 20k, đồng giá cực kỳ hấp dẫn. Ăn bao nhiêu cũng không sợ cháy túi!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 9,
                    Name = "Ốc Nhi 20k - 262 Vĩnh Khánh",
                    Description = "Thêm một lựa chọn đồng giá 20k. Thực đơn đa dạng, phục vụ nhanh nhẹn.",
                    Latitude = 10.76128,
                    Longitude = 106.70597,
                    Radius = 30,
                    TtsScript = "Ốc Nhi 20k tại số 262. Đồng giá cực mềm mà hương vị thì không hề tệ.",
                    Priority = 4,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },

                // ─── KHU VỰC 300-400 ───
                new Poi
                {
                    Id = 10,
                    Name = "Quán Ốc Thảo - 383 Vĩnh Khánh",
                    Description = "Một trong những quán đông khách nhất phố. Giá cả phải chăng, ốc tươi ngon.",
                    Latitude = 10.76168,
                    Longitude = 106.70236,
                    Radius = 35,
                    TtsScript = "Gần đến Ốc Thảo rồi. Số 383, quán rất đông khách, bạn phải xếp hàng đấy.",
                    Priority = 5,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 11,
                    Name = "Sushi Cô Bông - 390 Vĩnh Khánh",
                    Description = "Sushi Nhật Bản chất lượng trên vỉa hè Sài Gòn. Cá hồi tươi, cơm dẻo, giá mềm.",
                    Latitude = 10.75529,
                    Longitude = 106.70122,
                    Radius = 30,
                    TtsScript = "Sushi Cô Bông ở ngay số 390. Thêm một lựa chọn Nhật Bản tuyệt vời trên phố Vĩnh Khánh.",
                    Priority = 1,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },

                // ─── KHU VỰC 400+ (cuối phố, gần Kênh Tẻ) ───
                new Poi
                {
                    Id = 12,
                    Name = "Ốc Đêm - 474 Vĩnh Khánh",
                    Description = "Chuyên phục vụ buổi tối và đêm khuya. Ốc nướng mỡ hành và nghêu hấp sả là đặc sản.",
                    Latitude = 10.76050,
                    Longitude = 106.70413,
                    Radius = 30,
                    TtsScript = "Ốc Đêm Vĩnh Khánh số 474. Trời tối rồi mới là lúc quán này đông nhất.",
                    Priority = 2,
                    ImageUrl = "nuong_vinh_khanh_1773306623915.png"
                },
                new Poi
                {
                    Id = 13,
                    Name = "Ốc Oanh - 534 Vĩnh Khánh",
                    Description = "Quán ốc huyền thoại, sầm uất nhất phố. Được Michelin Bib Gourmand giới thiệu. Ốc hương rang muối ớt là món trứ danh.",
                    Latitude = 10.76072,
                    Longitude = 106.70330,
                    Radius = 40,
                    TtsScript = "Chào mừng bạn đến với Ốc Oanh - ông trùm ốc phố Vĩnh Khánh. Đừng quên thử ốc hương rang muối ớt trứ danh nhé!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                }
            };
        }
    }
}

