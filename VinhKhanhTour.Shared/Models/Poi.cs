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
                },
                new Poi
                {
                    Id = 5,
                    Name = "Bún Thái 68 - Ngã Tư Vĩnh Khánh",
                    NameEn = "Thai Noodle 68 - Vinh Khanh Intersection",
                    NameEs = "Fideos Tailandeses 68 - Intersección de Vinh Khanh",
                    NameFr = "Nouilles Thaïes 68 - Intersection de Vinh Khanh",
                    NameDe = "Thai Nudeln 68 - Vinh Khanh Kreuzung",
                    NameZh = "68泰式面条 - 永庆街十字路口",
                    NameJa = "タイ風ヌードル68 - ヴィンカン交差点",
                    NameKo = "태국 쌀국수 68 - 빈칸 교차로",
                    NameRu = "Тайская лапша 68 - Перекресток Винь Кхань",
                    NameIt = "Tagliatelle Tailandesi 68 - Incrocio di Vinh Khanh",
                    NamePt = "Macarrão Tailandês 68 - Cruzamento de Vinh Khanh",
                    Description = "Tô bún Thái chua cay đậm vị hải sản, tôm mực tươi rói rất bắt miệng.",
                    DescriptionEn = "Spicy and sour Thai noodles rich in seafood flavor with extremely fresh shrimp and squid.",
                    DescriptionEs = "Fideos tailandeses picantes y agrios ricos en sabor a marisco, con camarones y calamares.",
                    DescriptionFr = "Nouilles thaïes aigres-douces riches en fruits de mer avec des crevettes fraîches.",
                    DescriptionDe = "Scharf-saure thailändische Nudeln mit Meeresfrüchtegeschmack, frischen Garnelen und Tintenfisch.",
                    DescriptionZh = "酸辣泰式面条，海鲜味浓郁，鲜虾鱿鱼非常诱人。",
                    DescriptionJa = "新鮮なエビとイカが入ったシーフードの風味豊かなスパイシーで酸っぱいタイの麺。",
                    DescriptionKo = "신선한 새우와 오징어가 들어있는 해산물 풍미의 새콤매콤한 태국 쌀국수.",
                    DescriptionRu = "Остро-кислая тайская лапша с морепродуктами, свежими креветками и кальмарами.",
                    DescriptionIt = "Noodles tailandesi agrodolci con sapore di frutti di mare, gamberi ed erbe.",
                    DescriptionPt = "Macarrão tailandês picante e azedo com frutos do mar frescos e pimenta.",
                    PrimaryCategory = "Noodle",
                    Latitude = 10.76020,
                    Longitude = 106.70110,
                    Radius = 30,
                    TtsScript = "Bún Thái 68 chua cay khó cưỡng sẽ kích thích vị giác của bạn.",
                    TtsScriptEn = "Irresistible spicy Thai noodles 68 will stimulate your taste buds.",
                    TtsScriptEs = "Los irresistibles fideos tailandeses 68 picantes estimularán sus papilas gustativas.",
                    TtsScriptFr = "Les irrésistibles nouilles thaïes 68 épicées stimuleront vos papilles.",
                    TtsScriptDe = "Unwiderstehlich würzige Thai-Nudeln 68 regen Ihre Geschmacksknospen an.",
                    TtsScriptZh = "难挡诱惑的酸辣68泰式面条会刺激您的味蕾。",
                    TtsScriptJa = "魅力的なスパイシータイヌードル68があなたの味覚を刺激します。",
                    TtsScriptKo = "거부할 수 없는 매운 태국 쌀국수 68이 당신의 미각을 자극합니다.",
                    TtsScriptRu = "Непреодолимая острая тайская лапша 68 пробудит ваши вкусовые рецепторы.",
                    TtsScriptIt = "Gli irresistibili spaghetti tailandesi 68 piccanti stimoleranno le tue papille gustative.",
                    TtsScriptPt = "O irresistível macarrão tailandês picante 68 vai estimular as suas papilas gustativas.",
                    Priority = 3,
                    ImageUrl = "bun_thai_68_1774970185387.jpg"
                },
                new Poi
                {
                    Id = 6,
                    Name = "Lẩu Dê 414 - 414 Vĩnh Khánh",
                    NameEn = "Goat Hotpot 414 - 414 Vinh Khanh",
                    NameEs = "Sopa Caliente de Cabra 414 - 414 Vinh Khanh",
                    NameFr = "Fondue de Chèvre 414 - 414 Vinh Khanh",
                    NameDe = "Ziegen-Hotpot 414 - 414 Vinh Khanh",
                    NameZh = "羊肉火锅 414 - 永庆街414号",
                    NameJa = "ヤギ鍋 414 - ヴィンカン414番地",
                    NameKo = "염소 고기 전골 414 - 빈칸 414번지",
                    NameRu = "Козий Хот-пот 414 - 414 Винь Кхань",
                    NameIt = "Fonduta di Capra 414 - 414 Vinh Khanh",
                    NamePt = "Fondue de Cabra 414 - 414 Vinh Khanh",
                    Description = "Thịt dê tươi ngon, lẩu dê nước trong ngọt thanh, không bị hôi.",
                    DescriptionEn = "Fresh and delicious goat meat, clear and sweet hotpot broth, no strong odor.",
                    DescriptionEs = "Carne de cabra fresca y deliciosa, caldo de sopa claro y dulce, sin olor fuerte.",
                    DescriptionFr = "Viande de chèvre fraîche, bouillon de fondue clair et doux, sans forte odeur.",
                    DescriptionDe = "Frisches Ziegenfleisch, klare und süße Brühe, kein strenger Geruch.",
                    DescriptionZh = "新鲜美味的羊肉，汤底清澈甘甜，没有膻味。",
                    DescriptionJa = "新鮮で美味しいヤギ肉、透き通った甘い鍋スープで臭みはありません。",
                    DescriptionKo = "신선하고 맛있는 염소 고기, 맑고 단맛이 나는 달콤한 전골 육수, 특유의 향이 없습니다.",
                    DescriptionRu = "Свежее мясо козы, прозрачный и сладкий бульон для хот-пота без запаха.",
                    DescriptionIt = "Carne di capra fresca, brodo per fonduta chiaro e dolce senza odore.",
                    DescriptionPt = "Carne de cabra fresca e deliciosa, caldo claro e doce, sem cheio forte.",
                    PrimaryCategory = "Noodle",
                    Latitude = 10.76510,
                    Longitude = 106.70610,
                    Radius = 30,
                    TtsScript = "Lẩu dê 414 sẽ làm ấm bụng bạn vào những buổi tối Sài Gòn.",
                    TtsScriptEn = "Goat hotpot 414 will warm your stomach on Sai Gon evenings.",
                    TtsScriptEs = "La sopa de cabra 414 calentará tu estómago en las noches de Saigón.",
                    TtsScriptFr = "La fondue de chèvre 414 réchauffera votre estomac lors des soirées en ville.",
                    TtsScriptDe = "Der Ziegen-Hotpot 414 wärmt Ihren Magen an den Abenden in Saigon.",
                    TtsScriptZh = "414羊肉火锅会在西贡的夜晚温暖您的胃。",
                    TtsScriptJa = "ヤギ鍋414は、サイゴンの夜にあなたのお腹を温めてくれます。",
                    TtsScriptKo = "염소 고기 전골 414는 사이공의 저녁에 배를 따뜻하게 해줍니다.",
                    TtsScriptRu = "Козий хот-пот 414 согреет ваш желудок вечерами в Сайгоне.",
                    TtsScriptIt = "La fonduta di capra 414 riscalderà il tuo stomaco nelle serate di Saigon.",
                    TtsScriptPt = "O fondue de cabra 414 aquecerá o seu estômago nas noites de Saigão.",
                    Priority = 4,
                    ImageUrl = "lau_de_414_1774970205367.jpg"
                },
                new Poi
                {
                    Id = 7,
                    Name = "Khói BBQ - 232 Vĩnh Khánh",
                    NameEn = "Smoke BBQ - 232 Vinh Khanh",
                    NameEs = "Barbacoa Humo - 232 Vinh Khanh",
                    NameFr = "Fumée BBQ - 232 Vinh Khanh",
                    NameDe = "Rauch BBQ - 232 Vinh Khanh",
                    NameZh = "烟熏烧烤 - 永庆街232号",
                    NameJa = "スモーク BBQ - ヴィンカン232番地",
                    NameKo = "스모크 바베큐 - 빈칸 232번지",
                    NameRu = "Дымное барбекю - 232 Винь Кхань",
                    NameIt = "Fumo Barbecue - 232 Vinh Khanh",
                    NamePt = "Fumo Churrasco - 232 Vinh Khanh",
                    Description = "Quán nướng phong cách đường phố. Vú heo nướng chao là món ăn làm nên tên tuổi.",
                    DescriptionEn = "Street style BBQ stall. Grilled pork udder with fermented tofu is their signature dish.",
                    DescriptionEs = "Puesto de barbacoa de estilo callejero. Las ubres de cerdo a la parrilla es su plato fuerte.",
                    DescriptionFr = "Stand de BBQ au style rue. Leur spécialité est le mamelle de porc grillé.",
                    DescriptionDe = "Straßen-BBQ-Stand. Gegrillter Schweineeuter mit fermentiertem Tofu ist ihr Markenzeichen.",
                    DescriptionZh = "街头派烧烤摊。腐乳烤猪乳房是他们的招牌菜。",
                    DescriptionJa = "屋台スタイルのBBQ。発酵豆腐付きの豚のおっぱい肉焼きは看板メニューです。",
                    DescriptionKo = "길거리 스타일의 바베큐 매장. 콩 발효 두부를 곁들인 구운 돼지 유방 고기가 시그니처입니다.",
                    DescriptionRu = "Уличное барбекю. Жареное свиное вымя с ферментированным тофу - их фирменное блюдо.",
                    DescriptionIt = "Bancarella barbecue di strada. La carne di maiale grigliata con tofu è la loro firma.",
                    DescriptionPt = "Churrasco de estilo de rua. Carne de porco grelhada com tofu fermentado é o prato de assinatura.",
                    PrimaryCategory = "BBQ",
                    Latitude = 10.76610,
                    Longitude = 106.70710,
                    Radius = 30,
                    TtsScript = "Mùi thịt nướng thơm nức mũi từ Khói BBQ sẽ mời gọi bạn tấp vào.",
                    TtsScriptEn = "The delicious smell of grilled meat from Smoke BBQ will invite you to pop in.",
                    TtsScriptEs = "El olor delicioso a carne asada de Barbacoa Humo te invitará a entrar.",
                    TtsScriptFr = "L'odeur délicieuse de viande du Fumée BBQ vous invitera à vous arrêter.",
                    TtsScriptDe = "Der leckere Geruch von gegrilltem Fleisch bei Rauch BBQ wird Sie hereinlocken.",
                    TtsScriptZh = "烟熏烧烤诱人的烤肉味会吸引你进去品尝。",
                    TtsScriptJa = "スモークBBQの食欲をそそる焼肉の匂いが、あなたをお店に引き寄せます。",
                    TtsScriptKo = "스모크 바베큐의 고기 굽는 맛있는 냄새가 당신의 발길을 이끌 것입니다.",
                    TtsScriptRu = "Вкусный запах жареного мяса из Дымного барбекю пригласит вас зайти.",
                    TtsScriptIt = "Il delizioso profumo di carne grigliata da Fumo Barbecue ti invoglierà ad entrare.",
                    TtsScriptPt = "O cheiro maravilhoso e delicioso de carne grelhada convidará você a entrar.",
                    Priority = 5,
                    ImageUrl = "khoi_bbq_1774970225661.jpg"
                },
                new Poi
                {
                    Id = 8,
                    Name = "Nướng Ngói Sài Gòn - 240 Vĩnh Khánh",
                    NameEn = "Saigon Tile BBQ - 240 Vinh Khanh",
                    NameEs = "Barbacoa de Teja Saigón - 240 Vinh Khanh",
                    NameFr = "BBQ sur Tuile Saigon - 240 Vinh Khanh",
                    NameDe = "Saigon Dachziegel BBQ - 240 Vinh Khanh",
                    NameZh = "西贡瓦片烧烤 - 永庆街240号",
                    NameJa = "サイゴン瓦焼き - ヴィンカン240番地",
                    NameKo = "사이공 기와 구이 - 빈칸 240번지",
                    NameRu = "Сайгон Барбекю на черепице - 240 Винь Кхань",
                    NameIt = "Barbecue su Tegola Saigon - 240 Vinh Khanh",
                    NamePt = "Churrasco de Telha Saigon - 240 Vinh Khanh",
                    Description = "Nướng thịt trên ngói nóng giúp thịt mềm, giữ nước và không bị khét.",
                    DescriptionEn = "Grilling meat on hot roof tiles keeps it tender, juicy, and avoids burning.",
                    DescriptionEs = "Asar carne en tejas calientes la mantiene tierna, jugosa y evita que se queme.",
                    DescriptionFr = "Griller la viande sur des tuiles chaudes la garde tendre, juteuse, et sans brûler.",
                    DescriptionDe = "Das Grillen auf heißen Dachziegeln hält das Fleisch zart und saftig.",
                    DescriptionZh = "在热瓦片上烤肉可以保持肉质鲜嫩多汁，而且不易烤糊。",
                    DescriptionJa = "熱い屋根瓦で肉を焼くと、柔らかくジューシーに保たれ、焦げるのを防ぎます。",
                    DescriptionKo = "뜨거운 기와에서 고기를 구우면 타지 않고 육즙이 가득하고 부드럽습니다.",
                    DescriptionRu = "Жарка мяса на горячей черепице сохраняет его нежным, сочным и не дает сгореть.",
                    DescriptionIt = "Grigliare la carne sulle tegole calde la mantiene tenera, succosa ed evita bruciature.",
                    DescriptionPt = "Grelhar carne em telhas quentes ajuda a mantê-la tenra, suculenta e não queima.",
                    PrimaryCategory = "BBQ",
                    Latitude = 10.76710,
                    Longitude = 106.70810,
                    Radius = 30,
                    TtsScript = "Nướng ngói là một trải nghiệm ẩm thực độc đáo mà bạn nên thử qua.",
                    TtsScriptEn = "Grilling on tiles is a unique culinary experience you must try.",
                    TtsScriptEs = "Asar en tejas es una experiencia culinaria única que debes probar.",
                    TtsScriptFr = "Griller sur des tuiles est une expérience culinaire unique à essayer.",
                    TtsScriptDe = "Das Grillen auf Dachziegeln ist ein einzigartiges kulinarisches Erlebnis.",
                    TtsScriptZh = "瓦片烧烤是一种独特的烹饪体验，您一定不能错过。",
                    TtsScriptJa = "瓦焼きは、ぜひ試していただきたいユニークな料理体験です。",
                    TtsScriptKo = "기와 구이는 꼭 즐겨봐야 할 독특한 요리 경험입니다.",
                    TtsScriptRu = "Жарка на черепице — это уникальный кулинарный опыт, который стоит попробовать.",
                    TtsScriptIt = "Grigliare sulle tegole è un'esperienza culinaria unica che dovete provare.",
                    TtsScriptPt = "Grelhar em telhas é uma experiência de culinária única que você deve testar.",
                    Priority = 4,
                    ImageUrl = "nuong_ngoi_saigon_1774970246067.jpg"
                },
                new Poi
                {
                    Id = 9,
                    Name = "Sườn Nướng Chú Tèo - 250 Vĩnh Khánh",
                    NameEn = "Uncle Teo Ribs - 250 Vinh Khanh",
                    NameEs = "Costillas del Tío Teo - 250 Vinh Khanh",
                    NameFr = "Côtes de l'Oncle Teo - 250 Vinh Khanh",
                    NameDe = "Onkel Teos Rippchen - 250 Vinh Khanh",
                    NameZh = "阿刁叔烤排骨 - 永庆街250号",
                    NameJa = "テオおじさんのスペアリブ - ヴィンカン250番地",
                    NameKo = "테오 삼촌의 갈비구이 - 빈칸 250번지",
                    NameRu = "Ребрышки дядюшки Тео - 250 Винь Кхань",
                    NameIt = "Costolette dello Zio Teo - 250 Vinh Khanh",
                    NamePt = "Costelas do Tio Teo - 250 Vinh Khanh",
                    Description = "Chuyên sườn tảng nướng mật ong và các món nhậu lai rai.",
                    DescriptionEn = "Specializing in honey grilled rib racks and a variety of drinking snacks.",
                    DescriptionEs = "Especializado en costillas de miel a la parrilla y aperitivos para beber.",
                    DescriptionFr = "Spécialisé dans les côtes grillées au miel et divers snacks.",
                    DescriptionDe = "Spezialisiert auf honigglasierte Schweinerippchen und Snacks zum Bier.",
                    DescriptionZh = "主打蜂蜜烤肋排以及各种下酒小菜。",
                    DescriptionJa = "ハチミツ焼きのリブラックや様々なお酒のおつまみを専門としています。",
                    DescriptionKo = "꿀에 구운 갈비와 다양한 술안주거리를 전문으로 합니다.",
                    DescriptionRu = "Специализируется на запеченных в меду ребрышках и разнообразных закусках.",
                    DescriptionIt = "Specializzato in costolette grigliate al miele e vari stuzzichini per bevande.",
                    DescriptionPt = "Especializando em costelas grelhadas com mel e petiscos variados.",
                    PrimaryCategory = "BBQ",
                    Latitude = 10.76810,
                    Longitude = 106.70910,
                    Radius = 30,
                    TtsScript = "Sườn nướng Chú Tèo với lớp mật ong bóng bẩy hứa hẹn một bữa tối no nê.",
                    TtsScriptEn = "Uncle Teo's glossy honey glazed ribs promise a satisfying dinner.",
                    TtsScriptEs = "Las costillas con glaseado de miel brillante del Tío Teo prometen una cena satisfactoria.",
                    TtsScriptFr = "Les côtes de porc au miel glacées de l'oncle Teo promettent un dîner complet.",
                    TtsScriptDe = "Die glasierten Honigrippchen von Onkel Teo versprechen ein sättigendes Abendessen.",
                    TtsScriptZh = "阿刁叔色泽亮丽的蜂蜜烤排骨一定会让您吃得心满意足。",
                    TtsScriptJa = "ツヤのあるハチミツを塗ったテオおじさんのカルビは、満足のいくディナーをお約束します。",
                    TtsScriptKo = "꿀을 발라 구운 테오 삼촌의 윤기 나는 갈비가 만족스러운 저녁 식사를 약속합니다.",
                    TtsScriptRu = "Свиные ребрышки дядюшки Тео в медовой глазури обещают сытный ужин.",
                    TtsScriptIt = "Le laccate costolette al miele dello zio Teo promettono una cena soddisfacente.",
                    TtsScriptPt = "As costelas com cobertura de mel brilhante do Tio Teo garantem um jantar ótimo.",
                    Priority = 3,
                    ImageUrl = "suon_nuong_chu_teo_1774970264977.jpg"
                },
                new Poi
                {
                    Id = 10,
                    Name = "Sushi Mr Tôm - 215 Vĩnh Khánh",
                    NameEn = "Mr Tom Sushi - 215 Vinh Khanh",
                    NameEs = "Sushi Sr. Tom - 215 Vinh Khanh",
                    NameFr = "Sushi M. Tom - 215 Vinh Khanh",
                    NameDe = "Mr Tom Sushi - 215 Vinh Khanh",
                    NameZh = "汤姆先生寿司 - 永庆街215号",
                    NameJa = "Mr. トム寿司 - ヴィンカン215番地",
                    NameKo = "스시 미스터 톰 - 빈칸 215번지",
                    NameRu = "Суши Мистера Тома - 215 Винь Кхань",
                    NameIt = "Sushi Sig. Tom - 215 Vinh Khanh",
                    NamePt = "Sushi Sr. Tom - 215 Vinh Khanh",
                    Description = "Quán sushi vỉa hè độc lạ, chuyên phục vụ các set sushi bình dân mà vô cùng tươi ngon.",
                    DescriptionEn = "Unique street-side sushi stall offering affordable yet incredibly fresh sushi sets.",
                    DescriptionEs = "Puesto callejero único de sushi que ofrece bandejas asequibles y extraordinariamente frescas.",
                    DescriptionFr = "Stand de sushis de rue unique proposant des plateaux abordables mais incroyablement frais.",
                    DescriptionDe = "Einzigartiger Straßen-Sushi-Stand, der preiswerte und dennoch unglaublich frische Sushi-Sets anbietet.",
                    DescriptionZh = "独特的街边寿司摊，提供物美价廉而且极其新鲜的寿司套餐。",
                    DescriptionJa = "手頃な価格でありながら信じられないほど新鮮な寿司セットを提供する、ユニークな路上の寿司屋台。",
                    DescriptionKo = "저렴하면서도 믿을 수 없을 만큼 신선한 스시 세트를 제공하는 독특한 길거리 스시 가판대.",
                    DescriptionRu = "Уникальная уличная суши-лавка, предлагающая недорогие, но невероятно свежие сеты суши.",
                    DescriptionIt = "Bancarella di sushi in strada unica, offre vassoi convenienti ma incredibilmente freschi.",
                    DescriptionPt = "Banca de sushi de beira de rua única que oferece conjuntos acessíveis e ainda assim incrivelmente frescos.",
                    PrimaryCategory = "Sushi",
                    Latitude = 10.76315,
                    Longitude = 106.70588,
                    Radius = 30,
                    TtsScript = "Tìm kiếm làn gió mới? Sushi Mr Tôm mang ẩm thực Nhật Bản ra vỉa hè, bạn nhất định phải thử set thập cẩm.",
                    TtsScriptEn = "Looking for something different? Mr Tom Sushi brings Japanese cuisine to the curbside, you must try their mixed platter.",
                    TtsScriptEs = "¿Buscas algo distinto? Sushi Sr. Tom trae la gastronomía japonesa al nivel de la acera; tienes que probar su plato variado.",
                    TtsScriptFr = "Envie de nouveauté ? Sushi Mr. Tom amène la cuisine japonaise sur le trottoir, vous devez essayer leur plateau assorti.",
                    TtsScriptDe = "Suchen Sie etwas anderes? Sushi Mr Tom bringt japanische Küche auf den Bürgersteig, Sie müssen ihre gemischte Platte probieren.",
                    TtsScriptZh = "想换换口味吗？汤姆先生寿司把日本料理带到了街头，您一定要试试他们的各种拼盘。",
                    TtsScriptJa = "新しい風を求めていますか？ ミスター・トム・スシは日本料理を路上に持ち出しました。盛り合わせは必食です。",
                    TtsScriptKo = "새로운 것을 원하십니까? 스시 미스터 톰은 일본 요리를 길거리로 가져왔습니다. 모둠 접시를 꼭 시도해 보세요.",
                    TtsScriptRu = "Ищете новизну? Суши Мистера Тома приносит японскую кухню на тротуар, вы обязательно должны попробовать их ассорти.",
                    TtsScriptIt = "Cerchi qualcosa di diverso? Sushi Mr. Tom porta la cucina giapponese in strada; devi assolutamente provare il tagliere misto.",
                    TtsScriptPt = "À procura de uma brisa fresca? Sushi Mr Tôm traz a culinária japonesa para a calçada; tens mesmo de provar a variedade mista.",
                    Priority = 4,
                    ImageUrl = "sushi_mr_tom_1774970284030.jpg"
                },
                new Poi
                {
                    Id = 11,
                    Name = "Chè Ngon 81 - 81 Vĩnh Khánh",
                    NameEn = "Delicious Sweet Soup 81 - 81 Vinh Khanh",
                    NameEs = "Sopa Dulce Deliciosa 81 - 81 Vinh Khanh",
                    NameFr = "Soupe Sucrée Délicieuse 81 - 81 Vinh Khanh",
                    NameDe = "Leckere Süße Suppe 81 - 81 Vinh Khanh",
                    NameZh = "美味甜汤 81 - 永庆街81号",
                    NameJa = "美味しいチェー 81 - ヴィンカン81番地",
                    NameKo = "맛있는 디저트 수프 81 - 빈칸 81번지",
                    NameRu = "Вкусный сладкий суп 81 - 81 Винь Кхань",
                    NameIt = "Zuppa Dolce Deliziosa 81 - 81 Vinh Khanh",
                    NamePt = "Sopa Doce Deliciosa 81 - 81 Vinh Khanh",
                    Description = "Quán chè bình dân với đủ loại chè truyền thống: chè Thái, chè bưởi, chè khúc bạch.",
                    DescriptionEn = "Affordable sweet soup shop with traditional varieties: Thai sweet soup, pomelo sweet soup, cheese jelly.",
                    DescriptionEs = "Tienda asequible de sopas dulces tradicionales: sopa dulce tailandesa, sopa de pomelo, gelatina de queso.",
                    DescriptionFr = "Boutique abordable de soupes sucrées traditionnelles : thaïlandaise, au pamplemousse, gelée de fromage.",
                    DescriptionDe = "Erschwinglicher Süßsuppenladen mit traditionellen Sorten: Thai-Süßsuppe, Pomelo-Süßsuppe, Käsegelee.",
                    DescriptionZh = "价格实惠的甜汤店，提供泰式甜汤、柚子甜汤、奶酪果冻等传统品种。",
                    DescriptionJa = "手頃な価格の伝統的なスイーツショップ。タイ風チェー、ザボンチェー、チーズゼリーなど。",
                    DescriptionKo = "태국식 디저트, 포멜로 디저트, 치즈 젤리 등 다양한 전통 디저트를 판매하는 저렴한 상점.",
                    DescriptionRu = "Доступный магазин традиционных сладких супов: тайский суп, суп с помело, сырное желе.",
                    DescriptionIt = "Negozio economico di zuppe dolci tradizionali: tailandese, al pomelo, gelatina di formaggio.",
                    DescriptionPt = "Lugar acessível de sopas doces tradicionais: tailandesa, de pomelo, e gelatina de queijo.",
                    PrimaryCategory = "Dessert",
                    Latitude = 10.76156,
                    Longitude = 106.70295,
                    Radius = 30,
                    TtsScript = "Sau khi no nê đồ mặn, một ly chè mát lạnh tại Chè Ngon 81 là lựa chọn tuyệt vời.",
                    TtsScriptEn = "After a hearty savory meal, a cold glass of sweet soup at Delicious Sweet Soup 81 is a great choice.",
                    TtsScriptEs = "Después de una copiosa comida salada, un vaso frío de sopa dulce en Sopa Dulce Deliciosa 81 es una gran opción.",
                    TtsScriptFr = "Après un copieux repas salé, un grand verre de soupe sucrée froide chez Délicieuse Soupe Sucrée 81 est un excellent choix.",
                    TtsScriptDe = "Nach einer herzhaften und salzigen Mahlzeit ist ein kaltes Glas süße Suppe bei Delicious Sweet Soup 81 eine gute Wahl.",
                    TtsScriptZh = "吃完丰盛的咸味大餐后，来美味甜汤81喝一杯冰凉的甜汤是个不错的选择。",
                    TtsScriptJa = "ボリュームたっぷりの食事の後は、デリシャス・スウィート・スープ81の冷たいチェーが最高です。",
                    TtsScriptKo = "푸짐한 식사 후에는 맛있는 디저트 수프 81에서 차가운 디저트 한 잔을 즐기는 것이 좋습니다.",
                    TtsScriptRu = "После сытной соленой еды холодный стакан сладкого супа в Delicious Sweet Soup 81 — отличный выбор.",
                    TtsScriptIt = "Dopo un pasto abbondante, un bicchiere freddo di zuppa dolce da Delicious Sweet Soup 81 è un'ottima scelta.",
                    TtsScriptPt = "Depois de uma refeição salgada e farta, um copo frio de deliciosa sopa doce 81 é uma ótima escolha.",
                    Priority = 3,
                    ImageUrl = "che_ngon_81_1774970306024.jpg"
                },
                new Poi
                {
                    Id = 12,
                    Name = "Gà Nướng Ò Ó O - 100 Vĩnh Khánh",
                    NameEn = "O O O Grilled Chicken - 100 Vinh Khanh",
                    NameEs = "Pollo a la parrilla O O O - 100 Vinh Khanh",
                    NameFr = "Poulet grillé O O O - 100 Vinh Khanh",
                    NameDe = "O O O Gegrilltes Hähnchen - 100 Vinh Khanh",
                    NameZh = "打鸣烤鸡 - 永庆街100号",
                    NameJa = "O O Oグリルドチキン - ヴィンカン100番地",
                    NameKo = "꼬끼오 그릴드 치킨 - 빈칸 100번지",
                    NameRu = "Курица на гриле О-о-о - 100 Винь Кхань",
                    NameIt = "Pollo alla griglia O O O - 100 Vinh Khanh",
                    NamePt = "Frango Assado O O O - 100 Vinh Khanh",
                    Description = "Quán thịt gà bình dân, ướp gà ngũ vị hương thấm vị. Đóng cửa muộn phù hợp ăn đêm.",
                    DescriptionEn = "Affordable chicken spot, marinated with five-spice. Opens late, great for night snacks.",
                    DescriptionEs = "Lugar económico de pollo, marinado con cinco especias. Abre hasta tarde, ideal para picar de noche.",
                    DescriptionFr = "Où l'on trouve du poulet abordable, mariné aux cinq épices. Ouvre tard, parfait pour les en-cas nocturnes.",
                    DescriptionDe = "Günstiger Hähnchenstand, mariniert mit Fünf-Gewürze-Pulver. Spät geöffnet, toll für Snacks am späten Abend.",
                    DescriptionZh = "平价烤鸡店，用五香腌制。营业到很晚，适合吃夜宵。",
                    DescriptionJa = "五香粉でマリネした手頃な価格のチキン屋台。遅くまで開いており、夜食に最適。",
                    DescriptionKo = "오향이 스며든 저렴한 치킨집. 늦게까지 영업하여 야식으로 제격입니다.",
                    DescriptionRu = "Недорогая курица, маринованная в пяти специях. Открыто до поздна, отлично подходит для ночного перекуса.",
                    DescriptionIt = "Posto di pollo economico, marinato con cinque spezie. Aperto fino a tardi, ottimo per gli spuntini notturni.",
                    DescriptionPt = "Lugar económico de frango, marinado com cinco especias. Aberto até tarde, ótimo para lanches noturnos.",
                    PrimaryCategory = "BBQ",
                    Latitude = 10.76251,
                    Longitude = 106.70462,
                    Radius = 30,
                    TtsScript = "Hãy ghé qua Gà nướng Ò ó o số 100 để thưởng thức món gà mềm thơm nức mũi ngũ vị hương.",
                    TtsScriptEn = "Drop by O O O Grilled Chicken at number 100 to savor the tender chicken fragrant with five spices.",
                    TtsScriptEs = "Pásate por Pollo a la parrilla O O O en el número 100 para probar un pollo tierno aromatizado con cinco especias.",
                    TtsScriptFr = "Arrêtez-vous au Poulet Grillé O O O au numéro 100 pour savourer un poulet tendre parfumé aux cinq épices.",
                    TtsScriptDe = "Kommen Sie bei O O O Gegrilltes Hähnchen in der Nummer 100 vorbei, um das zarte Hähnchen mit Fünf-Gewürze-Pulver zu genießen.",
                    TtsScriptZh = "不妨去100号的打鸣烤鸡尝尝充满五香味的鲜嫩烤鸡。",
                    TtsScriptJa = "100番地にあるオオオ・グリルド・チキンに立ち寄って、五香粉の香りがする柔らかいチキンを味わってください。",
                    TtsScriptKo = "100번지 꼬끼오 구운 닭에 들러 오향 향이 도는 부드러운 닭고기를 맛보세요.",
                    TtsScriptRu = "Загляните в жареную курицу «О-о-о» по номеру 100, чтобы насладиться нежной курицей, ароматизированной пятью специями.",
                    TtsScriptIt = "Ferma a Pollo alla Griglia O O O al numero 100 per gustare il pollo tenero profumato alle cinque spezie.",
                    TtsScriptPt = "Dá as tuas paragens na Gà Nướng Ò ó o no número 100 para saborear o frango tenro e aromático confecionado com cinco especiarias.",
                    Priority = 3,
                    ImageUrl = "ga_nuong_o_o_o_1774970325066.jpg"
                }
            };
        }
}

}
