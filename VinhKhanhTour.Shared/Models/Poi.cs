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

        private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;

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
        public string DisplayName
        {
            get
            {
                var lang = GetCurrentLanguageCode();
                string? localized = lang switch
                {
                    "en" => NullIfEmpty(NameEn),
                    "es" => NullIfEmpty(NameEs),
                    "fr" => NullIfEmpty(NameFr),
                    "de" => NullIfEmpty(NameDe),
                    "zh" => NullIfEmpty(NameZh),
                    "ja" => NullIfEmpty(NameJa),
                    "ko" => NullIfEmpty(NameKo),
                    "ru" => NullIfEmpty(NameRu),
                    "it" => NullIfEmpty(NameIt),
                    "pt" => NullIfEmpty(NamePt),
                    _ => null
                };
                // If specific translation missing, try English, then Vietnamese
                return localized
                    ?? (lang != "vi" && lang != "en" ? NullIfEmpty(NameEn) : null)
                    ?? Name;
            }
        }
        
        [Ignore]
        public string DisplayDescription
        {
            get
            {
                var lang = GetCurrentLanguageCode();
                string? localized = lang switch
                {
                    "en" => NullIfEmpty(DescriptionEn),
                    "es" => NullIfEmpty(DescriptionEs),
                    "fr" => NullIfEmpty(DescriptionFr),
                    "de" => NullIfEmpty(DescriptionDe),
                    "zh" => NullIfEmpty(DescriptionZh),
                    "ja" => NullIfEmpty(DescriptionJa),
                    "ko" => NullIfEmpty(DescriptionKo),
                    "ru" => NullIfEmpty(DescriptionRu),
                    "it" => NullIfEmpty(DescriptionIt),
                    "pt" => NullIfEmpty(DescriptionPt),
                    _ => null
                };
                return localized
                    ?? (lang != "vi" && lang != "en" ? NullIfEmpty(DescriptionEn) : null)
                    ?? Description;
            }
        }

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
        public string DisplayTtsScript
        {
            get
            {
                var lang = GetCurrentLanguageCode();
                string? localized = lang switch
                {
                    "en" => NullIfEmpty(TtsScriptEn),
                    "es" => NullIfEmpty(TtsScriptEs),
                    "fr" => NullIfEmpty(TtsScriptFr),
                    "de" => NullIfEmpty(TtsScriptDe),
                    "zh" => NullIfEmpty(TtsScriptZh),
                    "ja" => NullIfEmpty(TtsScriptJa),
                    "ko" => NullIfEmpty(TtsScriptKo),
                    "ru" => NullIfEmpty(TtsScriptRu),
                    "it" => NullIfEmpty(TtsScriptIt),
                    "pt" => NullIfEmpty(TtsScriptPt),
                    _ => null
                };
                return localized
                    ?? (lang != "vi" && lang != "en" ? NullIfEmpty(TtsScriptEn) : null)
                    ?? TtsScript;
            }
        }

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
                    NameEs = "Caracoles Vu - 37 Vinh Khanh",
                    NameFr = "Escargots Vu - 37 Vinh Khanh",
                    NameDe = "Vu Schnecken - 37 Vinh Khanh",
                    NameZh = "阿武蜗牛 - 永庆街37号",
                    NameJa = "ヴー・スネイル - ヴィンカン37番地",
                    NameKo = "부 달팽이 - 빈칸 37번지",
                    NameRu = "Улитки Ву - 37 Винь Кхань",
                    NameIt = "Lumache Vu - 37 Vinh Khanh",
                    NamePt = "Caracóis Vu - 37 Vinh Khanh",
                    Description = "Quán ốc bình dân nổi tiếng với các món xào bơ tỏi thơm lừng. Rất được lòng giới trẻ Sài Gòn.",
                    DescriptionEn = "Popular snail stall famous for garlic butter stir-fry. Loved by young Saigonese.",
                    DescriptionEs = "Puesto de caracoles famoso por su salteado con ajo y mantequilla. Muy popular entre los jóvenes de Saigón.",
                    DescriptionFr = "Stand d'escargots célèbre pour ses sautés au beurre d'ail. Très apprécié des jeunes Saïgonnais.",
                    DescriptionDe = "Beliebter Schneckenstand, berühmt für Knoblauchbutter-Pfannengerichte. Bei jungen Saigonern sehr beliebt.",
                    DescriptionZh = "以蒜香黄油炒闻名的人气蜗牛摊。深受西贡年轻人喜爱。",
                    DescriptionJa = "ガーリックバター炒めで有名な人気カタツムリ屋台。サイゴンの若者に大人気。",
                    DescriptionKo = "마늘 버터 볶음으로 유명한 인기 달팽이 가게. 사이공 젊은이들에게 사랑받는 곳.",
                    DescriptionRu = "Популярный лоток с улитками, знаменитый жареными блюдами с чесночным маслом. Любимец молодёжи Сайгона.",
                    DescriptionIt = "Bancarella di lumache famosa per il saltato al burro e aglio. Amata dai giovani di Saigon.",
                    DescriptionPt = "Barraca de caracóis famosa pelo refogado com manteiga e alho. Adorada pelos jovens de Saigão.",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = "Ốc Vũ là điểm đến yêu thích của giới trẻ. Đừng bỏ qua món ốc móng tay xào bơ tỏi nhé.",
                    TtsScriptEn = "Vu Snail is a favorite spot for young people. Don't miss the bamboo snails stir-fried with garlic butter.",
                    TtsScriptEs = "Caracoles Vu es un lugar favorito de los jóvenes. No te pierdas los caracoles salteados con mantequilla de ajo.",
                    TtsScriptFr = "Vu Snail est un endroit préféré des jeunes. Ne manquez pas les escargots sautés au beurre d'ail.",
                    TtsScriptDe = "Vu Schnecken ist ein beliebter Ort für junge Leute. Verpassen Sie nicht die in Knoblauchbutter gebratenen Schnecken.",
                    TtsScriptZh = "阿武蜗牛是年轻人最喜欢的地方。千万不要错过蒜香黄油炒蜗牛。",
                    TtsScriptJa = "ヴー・スネイルは若者に人気のスポットです。ガーリックバターで炒めたカタツムリをお見逃しなく。",
                    TtsScriptKo = "부 달팽이는 젊은이들이 좋아하는 곳입니다. 마늘 버터로 볶은 달팽이를 놓치지 마세요.",
                    TtsScriptRu = "Улитки Ву — любимое место молодёжи. Не пропустите улиток, жареных с чесночным маслом.",
                    TtsScriptIt = "Lumache Vu è un posto preferito dai giovani. Non perdete le lumache saltate al burro e aglio.",
                    TtsScriptPt = "Caracóis Vu é um local favorito dos jovens. Não perca os caracóis salteados com manteiga de alho.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 2,
                    Name = "Ốc Phát Quán - 46 Vĩnh Khánh",
                    NameEn = "Phat Snail - 46 Vinh Khanh",
                    NameEs = "Caracoles Phat - 46 Vinh Khanh",
                    NameFr = "Escargots Phat - 46 Vinh Khanh",
                    NameDe = "Phat Schnecken - 46 Vinh Khanh",
                    NameZh = "发记蜗牛 - 永庆街46号",
                    NameJa = "ファット・スネイル - ヴィンカン46番地",
                    NameKo = "팟 달팽이 - 빈칸 46번지",
                    NameRu = "Улитки Фат - 46 Винь Кхань",
                    NameIt = "Lumache Phat - 46 Vinh Khanh",
                    NamePt = "Caracóis Phat - 46 Vinh Khanh",
                    Description = "Ốc len xào dừa và các món sa tế cay nồng là niềm tự hào của quán. Giá cả phải chăng.",
                    DescriptionEn = "Coconut milk mud snails and spicy satay dishes are their pride. Affordable prices.",
                    DescriptionEs = "Los caracoles de barro con leche de coco y los platos picantes de satay son su orgullo. Precios asequibles.",
                    DescriptionFr = "Les escargots de boue au lait de coco et les plats satay épicés sont leur fierté. Prix abordables.",
                    DescriptionDe = "Kokosmilch-Schlammschnecken und scharfe Satay-Gerichte sind ihr Stolz. Erschwingliche Preise.",
                    DescriptionZh = "椰奶炒泥蜗牛和辣味沙嗲菜品是他们的骄傲。价格实惠。",
                    DescriptionJa = "ココナッツミルクの泥カタツムリとスパイシーなサテ料理が自慢です。手頃な価格。",
                    DescriptionKo = "코코넛 밀크 진흙 달팽이와 매운 사테 요리가 자랑입니다. 합리적인 가격.",
                    DescriptionRu = "Улитки в кокосовом молоке и острые блюда сатай — их гордость. Доступные цены.",
                    DescriptionIt = "Le lumache al latte di cocco e i piatti piccanti satay sono il loro orgoglio. Prezzi accessibili.",
                    DescriptionPt = "Caracóis no leite de coco e pratos picantes de satay são seu orgulho. Preços acessíveis.",
                    Latitude = 10.76210,
                    Longitude = 106.70210,
                    Radius = 30,
                    TtsScript = "Ốc Phát nổi bật với ốc len xào dừa béo ngậy. Món nướng mỡ hành cũng rất tuyệt.",
                    TtsScriptEn = "Phat Snail stands out with creamy coconut mud snails. Their grilled dishes with scallion oil are also great.",
                    TtsScriptEs = "Caracoles Phat destaca por sus caracoles de barro con coco cremoso. Sus platos a la parrilla con aceite de cebolleta también son geniales.",
                    TtsScriptFr = "Escargots Phat se distingue par ses escargots crémeux au coco. Leurs grillades à l'huile de ciboule sont aussi excellentes.",
                    TtsScriptDe = "Phat Schnecken zeichnet sich durch cremige Kokosnuss-Schnecken aus. Ihre gegrillten Gerichte mit Frühlingszwiebelöl sind auch großartig.",
                    TtsScriptZh = "发记以奶油椰汁泥蜗牛闻名。他们的葱油烤菜也非常棒。",
                    TtsScriptJa = "ファット・スネイルはクリーミーなココナッツ泥カタツムリが自慢です。ネギ油のグリル料理も素晴らしいです。",
                    TtsScriptKo = "팟 달팽이는 크리미한 코코넛 진흙 달팽이로 유명합니다. 파기름 구이 요리도 훌륭합니다.",
                    TtsScriptRu = "Улитки Фат выделяются сливочными кокосовыми улитками. Их блюда на гриле с маслом зелёного лука тоже великолепны.",
                    TtsScriptIt = "Lumache Phat si distingue per le lumache al cocco cremose. I loro piatti grigliati con olio di cipollotto sono ottimi.",
                    TtsScriptPt = "Caracóis Phat destaca-se pelos caracóis cremosos de coco. Seus pratos grelhados com óleo de cebolinha também são ótimos.",
                    Priority = 3,
                    ImageUrl = "oc_loan_vinh_khanh_1773306611397.png"
                },
                new Poi
                {
                    Id = 3,
                    Name = "Lẩu Bò Khu Ba - 180 Vĩnh Khánh",
                    NameEn = "Zone 3 Beef Hotpot - 180 Vinh Khanh",
                    NameEs = "Fondue de Res Zona 3 - 180 Vinh Khanh",
                    NameFr = "Fondue de Bœuf Zone 3 - 180 Vinh Khanh",
                    NameDe = "Zone 3 Rindfleisch-Hotpot - 180 Vinh Khanh",
                    NameZh = "三区牛肉火锅 - 永庆街180号",
                    NameJa = "ゾーン3ビーフ鍋 - ヴィンカン180番地",
                    NameKo = "3구역 소고기 전골 - 빈칸 180번지",
                    NameRu = "Говяжий хот-пот Зона 3 - 180 Винь Кхань",
                    NameIt = "Fonduta di Manzo Zona 3 - 180 Vinh Khanh",
                    NamePt = "Fondue de Carne Zona 3 - 180 Vinh Khanh",
                    Description = "Quán lẩu bò, bò nướng trứ danh bình dân đông đúc. Thịt bò tươi ngon mềm mại.",
                    DescriptionEn = "Famous affordable beef hotpot and grilled beef stall. Fresh and tender beef.",
                    DescriptionEs = "Famoso puesto de fondue de res y carne a la parrilla. Carne fresca y tierna.",
                    DescriptionFr = "Célèbre stand de fondue et bœuf grillé à prix abordable. Viande fraîche et tendre.",
                    DescriptionDe = "Berühmter erschwinglicher Rindfleisch-Hotpot und Grillstand. Frisches und zartes Rindfleisch.",
                    DescriptionZh = "著名的平价牛肉火锅和烤牛肉摊。牛肉新鲜嫩滑。",
                    DescriptionJa = "有名なお手頃ビーフ鍋と焼肉の屋台。新鮮で柔らかい牛肉。",
                    DescriptionKo = "유명한 저렴한 소고기 전골과 구이 가게. 신선하고 부드러운 소고기.",
                    DescriptionRu = "Знаменитый доступный говяжий хот-пот и гриль. Свежая и нежная говядина.",
                    DescriptionIt = "Famosa bancarella di fonduta e manzo grigliato a prezzi accessibili. Carne fresca e tenera.",
                    DescriptionPt = "Famosa barraca de fondue e carne grelhada acessível. Carne fresca e macia.",
                    Latitude = 10.76061,
                    Longitude = 106.70425,
                    Radius = 30,
                    TtsScript = "Hãy ghé thử Lẩu Bò Khu Ba cực kỳ đông đúc ở số 180 Vĩnh Khánh. Thịt sườn bò nướng tuyệt cú mèo.",
                    TtsScriptEn = "Try the incredibly crowded Zone 3 Beef Hotpot at 180 Vinh Khanh. The grilled beef ribs are fantastic.",
                    TtsScriptEs = "Prueba la fondue de res Zona 3 increíblemente concurrida en el 180 de Vinh Khanh. Las costillas de res a la parrilla son fantásticas.",
                    TtsScriptFr = "Essayez la fondue de bœuf Zone 3 incroyablement bondée au 180 Vinh Khanh. Les côtes de bœuf grillées sont fantastiques.",
                    TtsScriptDe = "Probieren Sie den unglaublich beliebten Zone 3 Rindfleisch-Hotpot in der Vinh Khanh 180. Die gegrillten Rinderrippen sind fantastisch.",
                    TtsScriptZh = "来试试永庆街180号超级火爆的三区牛肉火锅吧。烤牛排骨太棒了。",
                    TtsScriptJa = "ヴィンカン180番地の大人気ゾーン3ビーフ鍋をぜひお試しください。グリルビーフリブは最高です。",
                    TtsScriptKo = "빈칸 180번지의 엄청나게 붐비는 3구역 소고기 전골을 꼭 드셔보세요. 소갈비 구이가 환상적입니다.",
                    TtsScriptRu = "Попробуйте невероятно популярный говяжий хот-пот Зона 3 по адресу 180 Винь Кхань. Жареные рёбрышки — фантастика.",
                    TtsScriptIt = "Provate la fonduta di manzo Zona 3 incredibilmente affollata al 180 di Vinh Khanh. Le costine grigliate sono fantastiche.",
                    TtsScriptPt = "Experimente o fondue de carne Zona 3 incrivelmente lotado no 180 Vinh Khanh. As costelas grelhadas são fantásticas.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 4,
                    Name = "Quán Ốc Thảo - 383 Vĩnh Khánh",
                    NameEn = "Thao Snail - 383 Vinh Khanh",
                    NameEs = "Caracoles Thao - 383 Vinh Khanh",
                    NameFr = "Escargots Thao - 383 Vinh Khanh",
                    NameDe = "Thao Schnecken - 383 Vinh Khanh",
                    NameZh = "草记蜗牛 - 永庆街383号",
                    NameJa = "タオ・スネイル - ヴィンカン383番地",
                    NameKo = "타오 달팽이 - 빈칸 383번지",
                    NameRu = "Улитки Тхао - 383 Винь Кхань",
                    NameIt = "Lumache Thao - 383 Vinh Khanh",
                    NamePt = "Caracóis Thao - 383 Vinh Khanh",
                    Description = "Một trong những quán ăn đông khách nhất phố. Giá cả phải chăng, ốc tươi ngon.",
                    DescriptionEn = "One of the busiest restaurants on the street. Affordable prices, fresh and delicious snails.",
                    DescriptionEs = "Uno de los restaurantes más concurridos de la calle. Precios accesibles, caracoles frescos y deliciosos.",
                    DescriptionFr = "L'un des restaurants les plus fréquentés de la rue. Prix abordables, escargots frais et délicieux.",
                    DescriptionDe = "Eines der meistbesuchten Restaurants der Straße. Erschwingliche Preise, frische und köstliche Schnecken.",
                    DescriptionZh = "街上最繁忙的餐厅之一。价格实惠，蜗牛新鲜美味。",
                    DescriptionJa = "通りで最も賑わうレストランの一つ。手頃な価格で新鮮なカタツムリ。",
                    DescriptionKo = "거리에서 가장 붐비는 식당 중 하나. 합리적인 가격, 신선하고 맛있는 달팽이.",
                    DescriptionRu = "Один из самых оживлённых ресторанов на улице. Доступные цены, свежие и вкусные улитки.",
                    DescriptionIt = "Uno dei ristoranti più affollati della via. Prezzi accessibili, lumache fresche e deliziose.",
                    DescriptionPt = "Um dos restaurantes mais movimentados da rua. Preços acessíveis, caracóis frescos e deliciosos.",
                    Latitude = 10.76185,
                    Longitude = 106.70335,
                    Radius = 30,
                    TtsScript = "Quán Ốc Thảo nằm ở số 383 Vĩnh Khánh, nổi tiếng với ốc hương nướng mỡ hành.",
                    TtsScriptEn = "Thao Snail at 383 Vinh Khanh is famous for grilled whelks with scallion oil.",
                    TtsScriptEs = "Caracoles Thao en el 383 de Vinh Khanh es famoso por los buccinos a la parrilla con aceite de cebolleta.",
                    TtsScriptFr = "Escargots Thao au 383 Vinh Khanh est célèbre pour ses bulots grillés à l'huile de ciboule.",
                    TtsScriptDe = "Thao Schnecken in der 383 Vinh Khanh ist berühmt für gegrillte Wellhornschnecken mit Frühlingszwiebelöl.",
                    TtsScriptZh = "草记蜗牛位于永庆街383号，以葱油烤海螺闻名。",
                    TtsScriptJa = "タオ・スネイルはヴィンカン383番地にあり、ネギ油で焼いたバイ貝で有名です。",
                    TtsScriptKo = "타오 달팽이는 빈칸 383번지에 있으며 파기름 소라구이로 유명합니다.",
                    TtsScriptRu = "Улитки Тхао на Винь Кхань 383 знамениты жареными трубачами с маслом зелёного лука.",
                    TtsScriptIt = "Lumache Thao al 383 di Vinh Khanh è famoso per i buccini grigliati con olio di cipollotto.",
                    TtsScriptPt = "Caracóis Thao no 383 Vinh Khanh é famoso por búzios grelhados com óleo de cebolinha.",
                    Priority = 4,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                }
            };
        }
}

}
