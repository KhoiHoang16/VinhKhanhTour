using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.Shared.Models
{
    public partial class Poi : ObservableObject
    {
        public static ILocalizationService? LocalizationService { get; set; }

        private string GetLocalizedString(string key)
        {
            return LocalizationService?.GetString(key) ?? key;
        }

        private string GetCurrentLanguageCode()
        {
            return LocalizationService?.CurrentLanguageCode ?? "en";
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameEs { get; set; } = string.Empty;
        public string NameFr { get; set; } = string.Empty;
        public string NameDe { get; set; } = string.Empty;
        public string NameZh { get; set; } = string.Empty;
        public string NameJa { get; set; } = string.Empty;
        public string NameKo { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public string NameIt { get; set; } = string.Empty;
        public string NamePt { get; set; } = string.Empty;
        

        [Ignore]
        public string DisplayImage => string.IsNullOrWhiteSpace(ImageUrl) ? "poi_placeholder.png" : ImageUrl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayDistanceText))]
        [NotifyPropertyChangedFor(nameof(ListDisplayDistanceText))]
        private double? _distanceToUser;

        [Ignore]
        public string DisplayDistanceText => DistanceToUser.HasValue 
            ? $"📍 {GetLocalizedString("Cách bạn")}: {DistanceToUser.Value:F0}m" 
            : $"📍 {GetLocalizedString("Đang định vị...")}";

        [Ignore]
        public string ListDisplayDistanceText => DistanceToUser.HasValue ? $"📍 {DistanceToUser.Value:F0}m" : "📍 ---";

        [Ignore]
        public string DisplayRadiusText => $"📍 {GetLocalizedString("Bán kính")}: {Radius}m";

        [Ignore]
        public string ListDisplayRadiusText => $"📍 {Radius}m";

        public string PrimaryCategory { get; set; } = string.Empty;

        [Ignore]
        public string DynamicPrimaryCategory
        {
            get 
            {
                if (!string.IsNullOrEmpty(PrimaryCategory)) return PrimaryCategory;
                
                string content = (Name + " " + Description).ToLowerInvariant();
                if (content.Contains("sushi")) return "Sushi";
                if (content.Contains("bún") || content.Contains("lẩu") || content.Contains("noodle") || content.Contains("hotpot")) return "Noodle";
                if (content.Contains("nướng") || content.Contains("bbq")) return "BBQ";
                return "Seafood";
            }
        }

        [Ignore]
        public string DisplayCategory 
        {
            get
            {
                string key = DynamicPrimaryCategory switch
                {
                    "BBQ" => "🍖 Đồ nướng",
                    "Noodle" => "🍜 Món nước",
                    "Sushi" => "🍣 Sushi",
                    _ => "🐚 Ốc & Hải sản"
                };
                return GetLocalizedString(key);
            }
        }
        
        [Ignore]
        public string ShortDisplayCategory 
        {
            get
            {
                string key = DynamicPrimaryCategory switch
                {
                    "BBQ" => "🍖 Đồ nướng",
                    "Noodle" => "🍜 Món nước",
                    "Sushi" => "🍣 Sushi",
                    _ => "🐚 Ốc"
                };
                return GetLocalizedString(key);
            }
        }

        [Ignore]
        public string DisplayName => GetCurrentLanguageCode() switch
        {
            "en" => !string.IsNullOrWhiteSpace(NameEn) ? NameEn : Name,
            "es" => !string.IsNullOrWhiteSpace(NameEs) ? NameEs : Name,
            "fr" => !string.IsNullOrWhiteSpace(NameFr) ? NameFr : Name,
            "de" => !string.IsNullOrWhiteSpace(NameDe) ? NameDe : Name,
            "zh" => !string.IsNullOrWhiteSpace(NameZh) ? NameZh : Name,
            "ja" => !string.IsNullOrWhiteSpace(NameJa) ? NameJa : Name,
            "ko" => !string.IsNullOrWhiteSpace(NameKo) ? NameKo : Name,
            "ru" => !string.IsNullOrWhiteSpace(NameRu) ? NameRu : Name,
            "it" => !string.IsNullOrWhiteSpace(NameIt) ? NameIt : Name,
            "pt" => !string.IsNullOrWhiteSpace(NamePt) ? NamePt : Name,
            _ => Name
        };
        
        [Ignore]
        public string DisplayDescription => GetCurrentLanguageCode() switch
        {
            "en" => !string.IsNullOrWhiteSpace(DescriptionEn) ? DescriptionEn : Description,
            "es" => !string.IsNullOrWhiteSpace(DescriptionEs) ? DescriptionEs : Description,
            "fr" => !string.IsNullOrWhiteSpace(DescriptionFr) ? DescriptionFr : Description,
            "de" => !string.IsNullOrWhiteSpace(DescriptionDe) ? DescriptionDe : Description,
            "zh" => !string.IsNullOrWhiteSpace(DescriptionZh) ? DescriptionZh : Description,
            "ja" => !string.IsNullOrWhiteSpace(DescriptionJa) ? DescriptionJa : Description,
            "ko" => !string.IsNullOrWhiteSpace(DescriptionKo) ? DescriptionKo : Description,
            "ru" => !string.IsNullOrWhiteSpace(DescriptionRu) ? DescriptionRu : Description,
            "it" => !string.IsNullOrWhiteSpace(DescriptionIt) ? DescriptionIt : Description,
            "pt" => !string.IsNullOrWhiteSpace(DescriptionPt) ? DescriptionPt : Description,
            _ => Description
        };

        // Added missing description properties used by DisplayDescription and sample data
        public string Description { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionEs { get; set; } = string.Empty;
        public string DescriptionFr { get; set; } = string.Empty;
        public string DescriptionDe { get; set; } = string.Empty;
        public string DescriptionZh { get; set; } = string.Empty;
        public string DescriptionJa { get; set; } = string.Empty;
        public string DescriptionKo { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionIt { get; set; } = string.Empty;
        public string DescriptionPt { get; set; } = string.Empty;

        public string TtsScript { get; set; } = string.Empty;
        public string TtsScriptEn { get; set; } = string.Empty;
        public string TtsScriptEs { get; set; } = string.Empty;
        public string TtsScriptFr { get; set; } = string.Empty;
        public string TtsScriptDe { get; set; } = string.Empty;
        public string TtsScriptZh { get; set; } = string.Empty;
        public string TtsScriptJa { get; set; } = string.Empty;
        public string TtsScriptKo { get; set; } = string.Empty;
        public string TtsScriptRu { get; set; } = string.Empty;
        public string TtsScriptIt { get; set; } = string.Empty;
        public string TtsScriptPt { get; set; } = string.Empty;

        [Ignore]
        public string DisplayTtsScript => GetCurrentLanguageCode() switch
        {
            "en" => !string.IsNullOrWhiteSpace(TtsScriptEn) ? TtsScriptEn : TtsScript,
            "es" => !string.IsNullOrWhiteSpace(TtsScriptEs) ? TtsScriptEs : TtsScript,
            "fr" => !string.IsNullOrWhiteSpace(TtsScriptFr) ? TtsScriptFr : TtsScript,
            "de" => !string.IsNullOrWhiteSpace(TtsScriptDe) ? TtsScriptDe : TtsScript,
            "zh" => !string.IsNullOrWhiteSpace(TtsScriptZh) ? TtsScriptZh : TtsScript,
            "ja" => !string.IsNullOrWhiteSpace(TtsScriptJa) ? TtsScriptJa : TtsScript,
            "ko" => !string.IsNullOrWhiteSpace(TtsScriptKo) ? TtsScriptKo : TtsScript,
            "ru" => !string.IsNullOrWhiteSpace(TtsScriptRu) ? TtsScriptRu : TtsScript,
            "it" => !string.IsNullOrWhiteSpace(TtsScriptIt) ? TtsScriptIt : TtsScript,
            "pt" => !string.IsNullOrWhiteSpace(TtsScriptPt) ? TtsScriptPt : TtsScript,
            _ => TtsScript
        };

        public string ImageUrl { get; set; } = string.Empty;
        public string AudioUrlVi { get; set; } = string.Empty;
        public string AudioUrlEn { get; set; } = string.Empty;

        // Added Radius property used by geofence checks
        public double Radius { get; set; } = 30;

        // Added Latitude, Longitude and Priority used by sample data
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Priority { get; set; }

        public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                new Poi
                {
                    Id = 1,
                    Name = "Ốc Vũ - 37 Vĩnh Khánh",
                    NameEn = "Vu Snail - 37 Vinh Khanh",
                    Description = "Quán ốc bình dân nổi tiếng với các món xào bơ tỏi thơm lừng. Rất được lòng giới trẻ Sài Gòn.",
                    DescriptionEn = "Popular snail stall famous for garlic butter stir-fry. Loved by young Saigonese.",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = "Ốc Vũ là điểm đến yêu thích của giới trẻ. Đừng bỏ qua món ốc móng tay xào bơ tỏi nhé.",
                    TtsScriptEn = "Vu Snail is a favorite spot for young people. Don't miss the bamboo snails stir-fried with garlic butter.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 2,
                    Name = "Ốc Phát Quán - 46 Vĩnh Khánh",
                    NameEn = "Phat Snail - 46 Vinh Khanh",
                    Description = "Ốc len xào dừa và các món sa tế cay nồng là niềm tự hào của quán. Giá cả phải chăng.",
                    DescriptionEn = "Coconut milk mud snails and spicy satay dishes are their pride. Affordable prices.",
                    Latitude = 10.76210,
                    Longitude = 106.70210,
                    Radius = 30,
                    TtsScript = "Ốc Phát nổi bật với ốc len xào dừa béo ngậy. Món nướng mỡ hành cũng rất tuyệt.",
                    TtsScriptEn = "Phat Snail stands out with creamy coconut mud snails. Their grilled dishes with scallion oil are also great.",
                    Priority = 3,
                    ImageUrl = "oc_loan_vinh_khanh_1773306611397.png"
                },
                new Poi
                {
                    Id = 3,
                    Name = "Lẩu Bò Khu Ba - 180 Vĩnh Khánh",
                    NameEn = "Zone 3 Beef Hotpot - 180 Vinh Khanh",
                    Description = "Quán lẩu bò, bò nướng trứ danh bình dân đông đúc. Thịt bò tươi ngon mềm mại.",
                    DescriptionEn = "Famous affordable beef hotpot and grilled beef stall. Fresh and tender beef.",
                    Latitude = 10.76061,
                    Longitude = 106.70425,
                    Radius = 30,
                    TtsScript = "Hãy ghé thử Lẩu Bò Khu Ba cực kỳ đông đúc ở số 180 Vĩnh Khánh. Thịt sườn bò nướng tuyệt cú mèo.",
                    TtsScriptEn = "Try the incredibly crowded Zone 3 Beef Hotpot at 180 Vinh Khanh. The grilled beef ribs are fantastic.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 4,
                    Name = "Quán Ốc Thảo - 383 Vĩnh Khánh",
                    NameEn = "Thao Snail - 383 Vinh Khanh",
                    Description = "Một trong những quán ăn đông khách nhất phố. Giá cả phải chăng, ốc tươi ngon.",
                    DescriptionEn = "One of the busiest restaurants on the street. Affordable prices, fresh and delicious snails.",
                    Latitude = 10.76185,
                    Longitude = 106.70335,
                    Radius = 30,
                    TtsScript = "Quán Ốc Thảo nằm ở số 383 Vĩnh Khánh, nổi tiếng với ốc hương nướng mỡ hành.",
                    TtsScriptEn = "Thao Snail at 383 Vinh Khanh is famous for grilled whelks with scallion oil.",
                    Priority = 4,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                }
            };
        }
}

}
