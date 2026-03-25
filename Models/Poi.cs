using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VinhKhanhTour.Models
{
    public partial class Poi : ObservableObject
    {
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
        public string NameHi { get; set; } = string.Empty;
        

        [Ignore]
        public string DisplayImage => string.IsNullOrWhiteSpace(ImageUrl) ? "poi_placeholder.png" : ImageUrl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayDistanceText))]
        [NotifyPropertyChangedFor(nameof(ListDisplayDistanceText))]
        private double? _distanceToUser;

        [Ignore]
        public string DisplayDistanceText => DistanceToUser.HasValue 
            ? $"📍 {Services.LocalizationResourceManager.Instance["Cách bạn"]}: {DistanceToUser.Value:F0}m" 
            : $"📍 {Services.LocalizationResourceManager.Instance["Đang định vị..."]}";

        [Ignore]
        public string ListDisplayDistanceText => DistanceToUser.HasValue ? $"📍 {DistanceToUser.Value:F0}m" : "📍 ---";

        [Ignore]
        public string DisplayRadiusText => $"📏 {Services.LocalizationResourceManager.Instance["Bán kính"]}: {Radius}m";

        [Ignore]
        public string ListDisplayRadiusText => $"📏 {Radius}m";

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
                return Services.LocalizationResourceManager.Instance[key]?.ToString() ?? key;
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
                return Services.LocalizationResourceManager.Instance[key]?.ToString() ?? key;
            }
        }

        [Ignore]
        public string DisplayName => Services.LocalizationResourceManager.Instance.CurrentLanguageCode switch
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
            "hi" => !string.IsNullOrWhiteSpace(NameHi) ? NameHi : Name,
            _ => Name
        };
        
        [Ignore]
        public string DisplayDescription => Services.LocalizationResourceManager.Instance.CurrentLanguageCode switch
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
            "hi" => !string.IsNullOrWhiteSpace(DescriptionHi) ? DescriptionHi : Description,
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
        public string DescriptionHi { get; set; } = string.Empty;

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
        public string TtsScriptHi { get; set; } = string.Empty;

        [Ignore]
        public string DisplayTtsScript => Services.LocalizationResourceManager.Instance.CurrentLanguageCode switch
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
            "hi" => !string.IsNullOrWhiteSpace(TtsScriptHi) ? TtsScriptHi : TtsScript,
            _ => TtsScript
        };

        public string ImageUrl { get; set; } = string.Empty;

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
                    Name = "Quán Ốc Vũ - 37 Vĩnh Khánh",
                    NameEn = "Vu Snail - 37 Vinh Khanh",
                    NameEs = "Vu Snail - 37 Vinh Khanh", NameFr = "Escargot Vu - 37 Vinh Khanh", NameDe = "Vu Schnecke - 37 Vinh Khanh", NameZh = "武蜗牛 - 37 永庆", NameJa = "ヴーカタツムリ - 37 ヴィンカン", NameKo = "부 달팽이 - 37 빈칸", NameRu = "Улитка Ву - 37 Винь Кхань", NameIt = "Lumaca Vu - 37 Vinh Khanh", NamePt = "Caracol Vu - 37 Vinh Khanh", NameHi = "حلزون فو - 37 فينه خان",
                    Description = "Quán ốc bình dân nổi tiếng với các món xào bơ tỏi thơm lừng. Rất được lòng giới trẻ Sài Gòn.",
                    DescriptionEn = "Popular snail stall famous for garlic butter stir-fry. Loved by young Saigonese.",
                    DescriptionEs = "Puesto popular famoso por el salteado con mantequilla de ajo. Amado por los jóvenes de Saigón.", DescriptionFr = "Stand populaire célèbre pour le sauté au beurre à l'ail. Aimé par les jeunes Saigonnais.", DescriptionDe = "Beliebter Stand, berühmt für Knoblauchbutter-Pfannengerichte. Geliebt von jungen Saigonesen.", DescriptionZh = "受欢迎的摊位，以大蒜奶油炒菜闻名。受到西贡年轻人的喜爱。", DescriptionJa = "ガーリックバター炒めで有名な人気の屋台。サイゴンの若者に愛されています。", DescriptionKo = "마늘 버터 볶음으로 유명한 인기 노점. 젊은 사이공인들에게 사랑받습니다.", DescriptionRu = "Популярный киоск, известный жареным в чесночном масле. Любим молодыми сайгонцами.", DescriptionIt = "Bancarella popolare famosa per il saltato nel burro all'aglio. Amata dai giovani saigonesi.", DescriptionPt = "Banca popular famosa pelo refogado na manteiga de alho. Amada pelos jovens saigoneses.", DescriptionHi = "كشك شعبي مشهور بالقلي السريع بزبدة الثوم. محبوب من قبل شباب سايغون.",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = "Ốc Vũ là điểm đến yêu thích của giới trẻ. Đừng bỏ qua món ốc móng tay xào bơ tỏi nhé.",
                    TtsScriptEn = "Vu Snail is a favorite spot for young people. Don't miss the bamboo snails stir-fried with garlic butter.",
                    TtsScriptEs = "Vu Snail es un lugar favorito para los jóvenes. No te pierdas los caracoles con mantequilla de ajo.", TtsScriptFr = "L'escargot Vu est un endroit préféré des jeunes. Ne manquez pas les escargots au beurre à l'ail.", TtsScriptDe = "Vu Schnecke ist ein beliebter Ort für junge Leute. Verpassen Sie nicht die Schnecken mit Knoblauchbutter.", TtsScriptZh = "武蜗牛是年轻人喜爱的地方。不要错过大蒜黄油竹蛏。", TtsScriptJa = "ヴーカタツムリは若者のお気に入りのスポットです。ガーリックバターカタツムリをお見逃しなく。", TtsScriptKo = "부 달팽이는 젊은이들이 즐겨 찾는 곳입니다. 마늘 버터 달팽이를 놓치지 마세요.", TtsScriptRu = "Улитка Ву - любимое место молодежи. Не пропустите улиток с чесночным маслом.", TtsScriptIt = "La lumaca Vu è il posto preferito dai giovani. Non perdetevi le lumache al burro all'aglio.", TtsScriptPt = "O caracol Vu é um local favorito para os jovens. Não perca os caracóis com manteiga de alho.", TtsScriptHi = "يعد حلزون فو بقعة مفضلة للشباب. لا تفوت حلزون زبدة الثوم.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 2,
                    Name = "Ốc Phát Quán - 46 Vĩnh Khánh",
                    NameEn = "Phat Snail - 46 Vinh Khanh",
                    NameEs = "Phat Snail - 46 Vinh Khanh", NameFr = "Escargot Phat - 46 Vinh Khanh", NameDe = "Phat Schnecke - 46 Vinh Khanh", NameZh = "法蜗牛 - 46 永庆", NameJa = "ファットカタツムリ - 46 ヴィンカン", NameKo = "팟 달팽이 - 46 빈칸", NameRu = "Улитка Фат - 46 Винь Кхань", NameIt = "Lumaca Phat - 46 Vinh Khanh", NamePt = "Caracol Phat - 46 Vinh Khanh", NameHi = "الزون فات - 46 فينه خان",
                    Description = "Ốc len xào dừa và các món sa tế cay nồng là niềm tự hào của quán. Giá cả phải chăng.",
                    DescriptionEn = "Coconut milk mud snails and spicy satay dishes are their pride. Affordable prices.",
                    DescriptionEs = "Caracoles de barro con leche de coco y platos picantes de satay son su orgullo. Precios asequibles.", DescriptionFr = "Les escargots de boue au lait de coco et les plats satay épicés font leur fierté. Prix abordables.", DescriptionDe = "Kokosmilch-Schlammschnecken und würzige Satay-Gerichte sind ihr Stolz. Erschwingliche Preise.", DescriptionZh = "椰奶泥螺和香辣沙爹菜肴是他们的骄傲。价格实惠。", DescriptionJa = "ココナッツミルクの泥カタツムリとスパイシーなサテ料理が彼らの誇りです。手頃な価格。", DescriptionKo = "코코넛 밀크 진흙 달팽이와 매운 사테 요리가 그들의 자랑입니다. 저렴한 가격.", DescriptionRu = "Грязевые улитки в кокосовом молоке и пряные блюда сатай - их гордость. Доступные цены.", DescriptionIt = "Le lumache di fango al latte di cocco e i piatti piccanti di satay sono il loro orgoglio. Prezzi convenienti.", DescriptionPt = "Caracóis de lama de leite de coco e pratos de satay picantes são o orgulho deles. Preços acessíveis.", DescriptionHi = "حلزون الطين بحليب جوز الهند وأطباق الساتيه الحارة هي فخرهم. أسعار معقولة.",
                    Latitude = 10.76027,
                    Longitude = 106.70301,
                    Radius = 35,
                    TtsScript = "Ốc Phát Quán số 46. Thử món ốc len xào dừa với vị béo ngậy khó cưỡng nha.",
                    TtsScriptEn = "Phat Snail at number 46. Try the coconut mud snails with irresistible creamy flavor.",
                    TtsScriptEs = "Phat Snail en el número 46. Prueba los caracoles de barro de coco con un irresistible sabor cremoso.", TtsScriptFr = "Escargot Phat au numéro 46. Essayez les escargots de boue de noix de coco avec une saveur crémeuse irrésistible.", TtsScriptDe = "Phat Schnecke bei Nummer 46. Probieren Sie die Kokos-Schlammschnecken mit unwiderstehlich cremigem Geschmack.", TtsScriptZh = "法蜗牛在46号。试试椰子泥螺，有不可抗拒的奶油味。", TtsScriptJa = "46番のファットカタツムリ。抗えないクリーミーな風味のココナッツ泥カタツムリをお試しください。", TtsScriptKo = "46번의 팟 달팽이. 참을 수 없는 크리미한 맛의 코코넛 진흙 달팽이를 맛보세요.", TtsScriptRu = "Улитка Фат на номере 46. Попробуйте грязевых улиток с кокосом с непреодолимым сливочным вкусом.", TtsScriptIt = "Lumaca Phat al numero 46. Prova le lumache di fango al cocco con sapore cremoso irresistibile.", TtsScriptPt = "Caracol Phat no número 46. Experimente os caracóis de lama de coco com sabor cremoso irresistível.", TtsScriptHi = "حلزون فات في الرقم 46. جرب حلزون الطين بجوز الهند بنكهة كريمية لا تقاوم.",
                    Priority = 2,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 3,
                    Name = "Quán Bé Ốc - 58 Vĩnh Khánh",
                    NameEn = "Be Ooc Snail - 58 Vinh Khanh",
                    NameEs = "Be Ooc Snail - 58 Vinh Khanh", NameFr = "Escargot Be Ooc - 58 Vinh Khanh", NameDe = "Be Ooc Schnecke - 58 Vinh Khanh", NameZh = "贝蜗牛 - 58 永庆", NameJa = "ベ・オック カタツムリ - 58 ヴィンカン", NameKo = "베옥 달팽이 - 58 빈칸", NameRu = "Улитка Бе Оок - 58 Винь Кхань", NameIt = "Lumaca Be Ooc - 58 Vinh Khanh", NamePt = "Caracol Be Ooc - 58 Vinh Khanh", NameHi = "حلزون بي أوك - 58 فينه خان",
                    Description = "Quán ốc nhỏ xinh, chuyên các loại ốc hấp sả và nướng mỡ hành. Phù hợp nhóm bạn.",
                    DescriptionEn = "Small cozy stall specializing in lemongrass steamed and scallion oil grilled snails.",
                    DescriptionEs = "Pequeño puesto acogedor especializado en caracoles cocidos al vapor con hierba de limón y carne a la parrilla.", DescriptionFr = "Petit stand chaleureux spécialisé dans les escargots à la vapeur à la citronnelle et grillés.", DescriptionDe = "Kleiner gemütlicher Stand, der auf gedünstete und gegrillte Schnecken spezialisiert ist.", DescriptionZh = "小型舒适摊位，专营香茅清蒸和葱油烤蜗牛。适合一群朋友。", DescriptionJa = "レモングラス蒸しとネギ油焼きカタツムリを専門とする小さくて居心地の良い屋台。友人のグループに適しています。", DescriptionKo = "레몬 그라스 무침과 파기름 구이 달팽이를 전문으로 하는 작고 아늑한 노점. 친구 그룹에 적합합니다.", DescriptionRu = "Маленький уютный киоск, специализирующийся на улитках на пару и на гриле. Подходит для группы друзей.", DescriptionIt = "Piccola bancarella accogliente specializzata in lumache al vapore e alla griglia. Adatto a un gruppo di amici.", DescriptionPt = "Pequena banca aconchegante especializada em caracóis cozidos e grelhados. Adequado para um grupo de amigos.", DescriptionHi = "كشك صغير ومريح متخصص في القواقع المطهية على البخار والمشوية.",
                    Latitude = 10.76339,
                    Longitude = 106.70206,
                    Radius = 30,
                    TtsScript = "Quán Bé Ốc số 58, nhỏ nhưng mà hương vị cực kỳ đậm đà đang chờ bạn.",
                    TtsScriptEn = "Be Ooc Snail at 58, small but offering incredibly rich flavors awaiting you.",
                    TtsScriptEs = "Be Ooc Snail en el 58, pequeño pero ofrece sabores increíblemente ricos esperándote.", TtsScriptFr = "Escargot Be Ooc à 58, petit mais offrant des saveurs incroyablement riches qui vous attendent.", TtsScriptDe = "Be Ooc Schnecke bei 58, klein, bietet aber unglaublich reiche Aromen, die auf Sie warten.", TtsScriptZh = "58号的贝蜗牛，很小但提供令人难以置信的丰富口味等你来品尝。", TtsScriptJa = "58番のベ・オック・カタツムリ。小さいですが、信じられないほど豊かな風味があなたを待っています。", TtsScriptKo = "58번의 베옥 달팽이. 작지만 놀랍도록 풍부한 맛이 여러분을 기다리고 있습니다.", TtsScriptRu = "Улитка Бе Оок в 58, маленькая, но предлагает невероятно богатые вкусы, ожидающие вас.", TtsScriptIt = "Lumaca Be Ooc a 58, piccola ma offre sapori incredibilmente ricchi che ti aspettano.", TtsScriptPt = "Caracol Be Ooc em 58, pequeno mas oferecendo sabores incrivelmente ricos esperando por você.", TtsScriptHi = "حلزون بي أوك في 58 ، صغير ولكنه يقدم نكهات غنية بشكل لا يصدق في انتظارك.",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 4,
                    Name = "Sushi Ko - 122 Vĩnh Khánh",
                    NameEn = "Sushi Ko - 122 Vinh Khanh",
                    NameEs = "Sushi Ko - 122 Vinh Khanh", NameFr = "Sushi Ko - 122 Vinh Khanh", NameDe = "Sushi Ko - 122 Vinh Khanh", NameZh = "Sushi Ko - 122 永庆", NameJa = "寿司Ko - 122 ヴィンカン", NameKo = "스시코 - 122 빈칸", NameRu = "Суши Ко - 122 Винь Кхань", NameIt = "Sushi Ko - 122 Vinh Khanh", NamePt = "Sushi Ko - 122 Vinh Khanh", NameHi = "سوشي كو - 122 فينه خان",
                    Description = "Sushi vỉa hè đẳng cấp nhà hàng. Giá cả bình dân, cá tươi ngon mỗi ngày.",
                    DescriptionEn = "Restaurant quality street sushi. Casual prices, fresh fish everyday.",
                    DescriptionEs = "Sushi callejero con calidad de restaurante. Precios casuales, pescado fresco todos los días.", DescriptionFr = "Sushi de rue de qualité restaurant. Prix ​​décontractés, poisson frais tous les jours.", DescriptionDe = "Street-Sushi in Restaurantqualität. Legere Preise, jeden Tag frischer Fisch.", DescriptionZh = "餐厅品质的街头寿司。价格休闲，每天都有新鲜的鱼。", DescriptionJa = "レストラン品質のストリート寿司。カジュアルな価格、毎日新鮮な魚。", DescriptionKo = "레스토랑 품질의 길거리 초밥. 캐주얼 한 가격, 매일 신선한 생선.", DescriptionRu = "Уличные суши ресторанного качества. Доступные цены, свежая рыба каждый день.", DescriptionIt = "Sushi di strada di qualità da ristorante. Prezzi casual, pesce fresco tutti i giorni.", DescriptionPt = "Sushi de rua com qualidade de restaurante. Preços casuais, peixe fresco todos os dias.", DescriptionHi = "سوشي الشارع بجودة المطاعم. أسعار عادية، أسماك طازجة كل يوم.",
                    Latitude = 10.76073,
                    Longitude = 106.70468,
                    Radius = 25,
                    TtsScript = "Sushi Ko mang đến trải nghiệm ẩm thực Nhật Bản ngay trên vỉa hè. Số 122 Vĩnh Khánh đây rồi.",
                    TtsScriptEn = "Sushi Ko brings Japanese dining experience right to the sidewalk. Number 122 Vinh Khanh is here.",
                    TtsScriptEs = "Sushi Ko lleva la experiencia gastronómica japonesa directamente a la acera. El número 122 de Vinh Khanh está aquí.", TtsScriptFr = "Sushi Ko apporte l'expérience culinaire japonaise jusque sur le trottoir. Le numéro 122 Vinh Khanh est ici.", TtsScriptDe = "Sushi Ko bringt das japanische Restauranterlebnis direkt auf den Bürgersteig. Nummer 122 Vinh Khanh ist hier.", TtsScriptZh = "Sushi Ko将日本的用餐体验直接带到了人行道上。 122永庆就在这里。", TtsScriptJa = "Sushi Koは日本のダイニング体験をそのまま歩道に持ち込みます。122番のヴィンカンがここにあります。", TtsScriptKo = "Sushi Ko는 일본 식당 경험을 보도로 바로 가져옵니다. 122번 빈칸이 여기 있습니다.", TtsScriptRu = "Суши Ко приносит японский обеденный опыт прямо на тротуар. Номер 122 Винь Кхань здесь.", TtsScriptIt = "Sushi Ko porta l'esperienza culinaria giapponese direttamente sul marciapiede. Il numero 122 di Vinh Khanh è qui.", TtsScriptPt = "A Sushi Ko traz a experiência gastronômica japonesa direto para a calçada. O número 122 de Vinh Khanh está aqui.", TtsScriptHi = "يجلب سوشي كو تجربة تناول الطعام اليابانية مباشرة إلى الرصيف. الرقم 122 فينه خان هنا.",
                    Priority = 4,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },
                new Poi
                {
                    Id = 5,
                    Name = "Ốc Đào 2 - 123 Vĩnh Khánh",
                    NameEn = "Dao Snail 2 - 123 Vinh Khanh",
                    NameEs = "Dao Snail 2 - 123 Vinh Khanh", NameFr = "Escargot Dao 2 - 123 Vinh Khanh", NameDe = "Dao Schnecke 2 - 123 Vinh Khanh", NameZh = "桃蜗牛 2 - 123 永庆", NameJa = "ダオカタツムリ 2 - 123 ヴィンカン", NameKo = "다오 달팽이 2 - 123 빈칸", NameRu = "Улитка Дао 2 - 123 Винь Кхань", NameIt = "Lumaca Dao 2 - 123 Vinh Khanh", NamePt = "Caracol Dao 2 - 123 Vinh Khanh", NameHi = "حلزون داو 2 - 123 فينه خان",
                    Description = "Chi nhánh Ốc Đào Nguyễn Trãi danh tiếng. Nước xốt bơ tỏi gây nghiện, hải sản tươi sống.",
                    DescriptionEn = "Branch of famous Dao Snail Nguyen Trai. Addictive garlic butter sauce, fresh seafood.",
                    DescriptionEs = "Sucursal de la famosa Dao Snail Nguyen Trai. Salsa de mantequilla de ajo adictiva, mariscos frescos.", DescriptionFr = "Succursale du célèbre escargot Dao Nguyen Trai. Sauce addictive au beurre à l'ail, fruits de mer frais.", DescriptionDe = "Zweig der berühmten Dao Schnecke Nguyen Trai. Süchtig machende Knoblauchbuttersauce, frische Meeresfrüchte.", DescriptionZh = "著名的道蜗牛阮斋分店。令人上瘾的大蒜黄油酱，新鲜的海鲜。", DescriptionJa = "有名なダオカタツムリ・グエントライの支店。やみつきになるガーリックバターソース、新鮮なシーフード。", DescriptionKo = "유명한 다오 달팽이 응우옌 트라이 지점. 중독성 있는 마늘 버터 소스, 신선한 해산물.", DescriptionRu = "Филиал знаменитой Улитки Дао Нгуен Трай. Вызывает привыкание чесночно-масляный соус, свежие морепродукты.", DescriptionIt = "Ramo della famosa lumaca Dao Nguyen Trai. Addictive salsa al burro all'aglio, pesce fresco.", DescriptionPt = "Filial da famosa Dao Snail Nguyen Trai. Molho de manteiga de alho viciante, frutos do mar frescos.", DescriptionHi = "فرع من حلزون داو نغوين تراي الشهير. صلصة زبدة الثوم التي تسبب الإدمان والماكولات البحرية الطازجة.",
                    Latitude = 10.76114,
                    Longitude = 106.70498,
                    Radius = 35,
                    TtsScript = "Ốc Đào 2 số 123 Vĩnh Khánh. Nước xốt bơ tỏi ăn kèm bánh mì ở đây cực kỳ gây nghiện.",
                    TtsScriptEn = "Dao Snail 2 at 123 Vinh Khanh. The garlic butter sauce with bread here is highly addictive.",
                    TtsScriptEs = "Dao Snail 2 en el 123 de Vinh Khanh. La salsa de mantequilla de ajo con pan aquí es muy adictiva.", TtsScriptFr = "Escargot Dao 2 au 123 Vinh Khanh. La sauce au beurre à l'ail avec du pain ici est très addictive.", TtsScriptDe = "Dao Schnecke 2 im 123 Vinh Khanh. Die Knoblauchbuttersauce mit Brot hier macht sehr süchtig.", TtsScriptZh = "123永庆的道蜗牛2号。这里的大蒜黄油酱配面包非常容易上瘾。", TtsScriptJa = "123番ヴィンカンのダオカタツムリ2。ここのガーリックバターソースとパンは非常にやみつきになります。", TtsScriptKo = "123번 빈칸의 다오 달팽이 2. 여기의 마늘 버터 소스와 빵은 중독성이 매우 강합니다.", TtsScriptRu = "Улитка Дао 2 на 123 Винь Кхань. Чесночно-масляный соус с хлебом здесь вызывает сильное привыкание.", TtsScriptIt = "Lumaca Dao 2 a 123 Vinh Khanh. La salsa al burro all'aglio con il pane qui è molto avvincente.", TtsScriptPt = "Dao Snail 2 em 123 Vinh Khanh. O molho de manteiga de alho com pão aqui é muito viciante.", TtsScriptHi = "حلزون داو 2 في 123 فينه خان. صلصة زبدة الثوم مع الخبز هنا تسبب الإدمان للغاية.",
                    Priority = 5,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 6,
                    Name = "Ốc Nhớ Sài Gòn - 159 Vĩnh Khánh",
                    NameEn = "Remember Saigon Snail - 159 Vinh Khanh",
                    NameEs = "Remember Saigon Snail - 159 Vinh Khanh", NameFr = "Souviens-toi de Saigon - 159 Vinh Khanh", NameDe = "Erinnern Sie sich an Saigon - 159 Vinh Khanh", NameZh = "记住西贡蜗牛 - 159 永庆", NameJa = "サイゴンを思い出すカタツムリ - 159 ヴィンカン", NameKo = "사이공을 기억하라 달팽이 - 159 빈칸", NameRu = "Вспомните Сайгон - 159 Винь Кхань", NameIt = "Ricorda Saigon Lumaca - 159 Vinh Khanh", NamePt = "Lembre-se de Saigon Snail - 159 Vinh Khanh", NameHi = "تذكر سايغون - 159 فينه خان",
                    Description = "Menu nhiều món sáng tạo, phục vụ nhanh nhẹn. Không gian mở thoáng mát, rất đông vào buổi tối.",
                    DescriptionEn = "Creative menu, fast service. Open airy space, very crowded in the evening.",
                    DescriptionEs = "Menú creativo, servicio rápido. Espacio abierto y aireado, muy concurrido por la noche.", DescriptionFr = "Menu créatif, service rapide. Espace aéré ouvert, très bondé en soirée.", DescriptionDe = "Kreatives Menü, schneller Service. Offener luftiger Raum, abends sehr voll.", DescriptionZh = "创意菜单，提供快速的服务。开放通风的空间，晚上非常拥挤。", DescriptionJa = "クリエイティブなメニュー、迅速なサービス。オープンで風通しの良いスペース、夕方は非常に混雑しています。", DescriptionKo = "창의적인 메뉴, 빠른 서비스. 개방적이고 통풍이 잘되는 공간, 저녁에 매우 붐빁니다.", DescriptionRu = "Креативное меню, быстрое обслуживание. Открытое проветриваемое пространство, очень многолюдно вечером.", DescriptionIt = "Menu creativo, servizio veloce. Spazio aperto e arioso, molto affollato la sera.", DescriptionPt = "Menu criativo, serviço rápido. Espaço arejado aberto, muito lotado à noite.", DescriptionHi = "قائمة ابداعية، خدمة سريعة. مساحة مفتوحة ومتجددة الهواء، مزدحمة للغاية في المساء.",
                    Latitude = 10.76120,
                    Longitude = 106.70540,
                    Radius = 30,
                    TtsScript = "Ốc Nhớ Sài Gòn - cái tên nói lên tất cả. Bạn sẽ nhớ mãi hương vị tại đây.",
                    TtsScriptEn = "Remember Saigon Snail - the name says it all. You will remember the taste here forever.",
                    TtsScriptEs = "Recuerde Saigon Snail: el nombre lo dice todo. Recordarás el sabor de aquí para siempre.", TtsScriptFr = "Se souvenir de l'escargot de Saigon - le nom dit tout. Vous vous souviendrez de la saveur ici pour toujours.", TtsScriptDe = "Erinnere dich an Saigon Snail - der Name sagt alles. Sie werden den Geschmack hier für immer in Erinnerung behalten.", TtsScriptZh = "记住西贡蜗牛——这个名字说明了一切。 你会永远记住这里的味道。", TtsScriptJa = "サイゴンカタツムリを思い出す-その名前がすべてを物語っています。ここでの味を永遠に思い出すでしょう。", TtsScriptKo = "사이공 달팽이 기억 - 이름이 모든 것을 말해줍니다. 이곳의 맛을 영원히 기억할 것입니다.", TtsScriptRu = "Вспомните Сайгонскую улитку - название говорит само за себя. Вы запомните вкус здесь навсегда.", TtsScriptIt = "Ricorda Saigon Snail - il nome dice tutto. Ricorderai per sempre il sapore qui.", TtsScriptPt = "Lembre-se do Saigon Snail - o nome diz tudo. Você vai se lembrar do sabor aqui para sempre.", TtsScriptHi = "تذكر سايغون - الاسم يقول كل شيء. سوف تتذكر الطعم هنا إلى الأبد.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 7,
                    Name = "Bánh Flan Ngọc Nga - 167 Vĩnh Khánh",
                    NameEn = "Ngoc Nga Flan - 167 Vinh Khanh",
                    NameEs = "Ngoc Nga Flan - 167 Vinh Khanh", NameFr = "Flan Ngoc Nga - 167 Vinh Khanh", NameDe = "Ngoc Nga Flan - 167 Vinh Khanh", NameZh = "玉娥布丁 - 167 永庆", NameJa = "ゴック・ンガ フラン - 167 ヴィンカン", NameKo = "응옥응아 플란 - 167 빈칸", NameRu = "Флан Нгок Нга - 167 Винь Кхань", NameIt = "Ngoc Nga Flan - 167 Vinh Khanh", NamePt = "Flan Ngoc Nga - 167 Vinh Khanh", NameHi = "فطيرة نجوك نجا - 167 فينه خان",
                    Description = "Bánh flan béo mịn tan ngay đầu lưỡi, nước caramel đậm đà. Món tráng miệng phải thử sau ốc.",
                    DescriptionEn = "Creamy melt-in-mouth flan, rich caramel. A must-try dessert after snails.",
                    DescriptionEs = "Flan cremoso que se derrite en la boca, caramelo rico. Un postre que debes probar después de los caracoles.", DescriptionFr = "Flan crémeux fondant en bouche, caramel riche. Un dessert à ne pas manquer après les escargots.", DescriptionDe = "Cremiger zartschmelzender Flan, reichhaltiges Karamell. Ein Muss nach Schnecken.", DescriptionZh = "入口即化的奶油布丁，浓郁的焦 caramel。吃完蜗牛后必试的甜点。", DescriptionJa = "口の中でとろけるクリーミーなフラン、リッチなキャラメル。カタツムリの後にぜひ試していただきたいデザートです。", DescriptionKo = "입안에서 사르르 녹는 크리미한 플란, 풍부한 캐러멜. 달팽이를 먹은 후 꼭 먹어봐야 할 디저트입니다.", DescriptionRu = "Сливочный, тающий во рту флан, густая карамель. Обязательный десерт после улиток.", DescriptionIt = "Flan cremoso che si scioglie in bocca, caramello ricco. Un dessert da provare assolutamente dopo le lumache.", DescriptionPt = "Pudim cremoso que derrete na boca, caramelo rico. Uma sobremesa imperdível após os caracóis.", DescriptionHi = "فطيرة كريمية تذوب في الفم، والكراميل الغني. حلوى لا بد من تجربتها بعد القواقع.",
                    Latitude = 10.76232,
                    Longitude = 106.70316,
                    Radius = 30,
                    TtsScript = "Nghỉ chân ăn bánh flan Ngọc Nga nhé. Béo mịn, ngọt vừa, tuyệt vời sau một bữa ốc.",
                    TtsScriptEn = "Take a break with Ngoc Nga flan. Creamy, perfectly sweet, wonderful after a snail feast.",
                    TtsScriptEs = "Tome un descanso con el flan de Ngoc Nga. Cremoso, perfectamente dulce, maravilloso después de un festín de caracoles.", TtsScriptFr = "Faites une pause avec le flan de Ngoc Nga. Crémeux, parfaitement sucré, merveilleux après un festin d'escargots.", TtsScriptDe = "Machen Sie eine Pause mit Ngoc Nga Flan. Cremig, perfekt süß, wunderbar nach einem Schneckenfest.", TtsScriptZh = "吃一口玉娥布丁休息一下。奶油味，恰到好处的甜度，在蜗牛盛宴后很棒。", TtsScriptJa = "ゴック・ンガのフランで休憩しましょう。クリーミーで完璧な甘さ、カタツムリの饗宴の後に素晴らしい。", TtsScriptKo = "응옥응아 플란과 함께 휴식을 취하세요. 크리미하고 완벽하게 달콤하며 달팽이 축제 후에 훌륭합니다.", TtsScriptRu = "Сделайте перерыв с фланом Нгок Нга. Кремовый, идеально сладкий, прекрасный после пиршества с улитками.", TtsScriptIt = "Fai una pausa con il flan di Ngoc Nga. Cremoso, perfettamente dolce, meraviglioso dopo un banchetto di lumache.", TtsScriptPt = "Faça uma pausa com o pudim de Ngoc Nga. Cremoso, perfeitamente doce, maravilhoso depois de um banquete de caracóis.", TtsScriptHi = "خذ استراحة مع فطيرة نجوك نجا. كريمي، حلو تماما، رائع بعد وليمة الحلزون.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 8,
                    Name = "Ốc Su 20k - 225 Vĩnh Khánh",
                    NameEn = "Su Snail 20k - 225 Vinh Khanh",
                    NameEs = "Su Snail 20k - 225 Vinh Khanh", NameFr = "Escargot Su 20k - 225 Vinh Khanh", NameDe = "Su Schnecke 20k - 225 Vinh Khanh", NameZh = "苏蜗牛 20k - 225 永庆", NameJa = "スー カタツムリ 20k - 225 ヴィンカン", NameKo = "수 달팽이 20k - 225 빈칸", NameRu = "Улитка Су 20k - 225 Винь Кхань", NameIt = "Lumaca Su 20k - 225 Vinh Khanh", NamePt = "Caracol Su 20k - 225 Vinh Khanh", NameHi = "حلزون سو 20 ك - 225 فينه خان",
                    Description = "Ốc đồng giá 20k cực rẻ, phù hợp cho sinh viên muốn ăn nhiều món với chi phí thấp.",
                    DescriptionEn = "Super cheap 20k flat-rate snails, perfect for students wanting variety on a budget.",
                    DescriptionEs = "Caracoles súper baratos con tarifa plana de 20k, perfectos para estudiantes que desean variedad con un presupuesto ajustado.", DescriptionFr = "Des escargots très bon marché avec un tarif forfaitaire de 20k, parfaits pour les étudiants souhaitant de la variété avec un budget limité.", DescriptionDe = "Super günstige 20k Flatrate-Schnecken, perfekt für Studenten, die Abwechslung zu einem bestimmten Budget wünschen.", DescriptionZh = "超级便宜的20k固定价蜗牛，非常适合预算有限、希望口味多样的学生。", DescriptionJa = "超格安の20k均一カタツムリ。予算に限りがあり、バラエティを楽しみたい学生に最適です。", DescriptionKo = "매우 저렴한 20k 플랫 요금 달팽이, 예산으로 다양성을 원하는 학생들에게 적합합니다.", DescriptionRu = "Супер дешевые улитки по фиксированной ставке 20k, идеально подходящие для студентов, желающих разнообразия при ограниченном бюджете.", DescriptionIt = "Lumache super economiche a tariffa fissa da 20k, perfette per studenti che desiderano varietà a basso costo.", DescriptionPt = "Caracóis super baratos com taxa fixa de 20k, perfeitos para estudantes que desejam variedade em um orçamento.", DescriptionHi = "حلزونات رخيصة للغاية بسعر ثابت 20 ألف، مثالية للطلاب الذين يريدون التنوع في الميزانية.",
                    Latitude = 10.76056,
                    Longitude = 106.70396,
                    Radius = 30,
                    TtsScript = "Ốc Su 20k, đồng giá cực kỳ hấp dẫn. Ăn bao nhiêu cũng không sợ cháy túi!",
                    TtsScriptEn = "Su Snail 20k, an incredibly attractive flat rate. Eat as much as you want without going broke!",
                    TtsScriptEs = "Su Snail 20k, una tarifa plana increíblemente atractiva. ¡Come todo lo que quieras sin arruinarte!", TtsScriptFr = "Escargot Su 20k, un tarif forfaitaire incroyablement attractif. Mangez autant que vous le souhaitez sans vous ruiner !", TtsScriptDe = "Su Schnecke 20k, eine unglaublich attraktive Flatrate. Essen Sie so viel Sie wollen, ohne pleite zu gehen!", TtsScriptZh = "苏蜗牛20k，极具吸引力的统一费率。 尽情享用，无需担心破产！", TtsScriptJa = "驚くほど魅力的な均一料金のスーカタツムリ20k。破産することなく好きなだけ食べてください！", TtsScriptKo = "엄청나게 매력적인 정액제의 수 달팽이 20k. 파산 걱정 없이 마음껏 드세요!", TtsScriptRu = "Улитка Су 20k, невероятно привлекательная фиксированная ставка. Ешьте сколько хотите, не разоряясь!", TtsScriptIt = "Lumaca Su 20k, una tariffa fissa incredibilmente attraente. Mangia quanto vuoi senza andare in rovina!", TtsScriptPt = "Caracol Su 20k, uma taxa fixa incrivelmente atraente. Coma o quanto quiser sem falir!", TtsScriptHi = "حلزون سو 20 ك، سعر ثابت جذاب بشكل لا يصدق. كل ما تريد دون أن تفلس!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 9,
                    Name = "Ốc Nhi 20k - 262 Vĩnh Khánh",
                    NameEn = "Nhi Snail 20k - 262 Vinh Khanh",
                    NameEs = "Nhi Snail 20k - 262 Vinh Khanh", NameFr = "Escargot Nhi 20k - 262 Vinh Khanh", NameDe = "Nhi Schnecke 20k - 262 Vinh Khanh", NameZh = "儿蜗牛 20k - 262 永庆", NameJa = "ニー カタツムリ 20k - 262 ヴィンカン", NameKo = "니 달팽이 20k - 262 빈칸", NameRu = "Улитка Нхи 20k - 262 Винь Кхань", NameIt = "Lumaca Nhi 20k - 262 Vinh Khanh", NamePt = "Caracol Nhi 20k - 262 Vinh Khanh", NameHi = "حلزون ني 20 ك - 262 فينه خان",
                    Description = "Thêm một lựa chọn đồng giá 20k. Thực đơn đa dạng, phục vụ nhanh nhẹn.",
                    DescriptionEn = "Another 20k flat-rate choice. Diverse menu, fast service.",
                    DescriptionEs = "Otra opción con tarifa plana de 20k. Menú variado, servicio rápido.", DescriptionFr = "Un autre choix de tarif forfaitaire de 20 000. Menu varié, service rapide.", DescriptionDe = "Eine weitere Wahl mit einer Flatrate von 20.000. Vielfältige Speisekarte, schneller Service.", DescriptionZh = "另一个20k固定价的选择。菜色多样，服务快捷。", DescriptionJa = "もう一つの20k定額の選択肢。多様なメニュー、迅速なサービス。", DescriptionKo = "또 다른 20k 정액제 선택. 다양한 메뉴, 빠른 서비스.", DescriptionRu = "Еще один выбор с фиксированной ставкой 20k. Разнообразное меню, быстрое обслуживание.", DescriptionIt = "Un'altra scelta a tariffa fissa di 20k. Menu vario, servizio veloce.", DescriptionPt = "Outra escolha de taxa fixa de 20k. Menu diversificado, atendimento rápido.", DescriptionHi = "خيار آخر بسعر ثابت قدره 20 ألف. قائمة متنوعة ، خدمة سريعة.",
                    Latitude = 10.76128,
                    Longitude = 106.70597,
                    Radius = 30,
                    TtsScript = "Ốc Nhi 20k tại số 262. Đồng giá cực mềm mà hương vị thì không hề tệ.",
                    TtsScriptEn = "Nhi Snail 20k at number 262. Very cheap flat rate, but the flavor is not bad at all.",
                    TtsScriptEs = "Nhi Snail 20k en el número 262. Tarifa plana muy barata, pero el sabor no está nada mal.", TtsScriptFr = "Escargot Nhi 20k au numéro 262. Forfait très bon marché, mais la saveur n'est pas mauvaise du tout.", TtsScriptDe = "Nhi Schnecke 20k an der Nummer 262. Sehr günstige Flatrate, aber der Geschmack ist überhaupt nicht schlecht.", TtsScriptZh = "在262号的儿蜗牛20k。很便宜的统一定价，但味道一点都不差。", TtsScriptJa = "262番のニーカタツムリ20k。非常に安い均一料金ですが、味はまったく悪くありません。", TtsScriptKo = "262번의 니 달팽이 20k. 매우 저렴한 정액 요원이지만 맛은 전혀 나쁘지 않습니다.", TtsScriptRu = "Улитка Нхи 20k под номером 262. Очень дешевая фиксированная ставка, но вкус совсем не плохой.", TtsScriptIt = "Lumaca Nhi 20k al numero 262. Tariffa fissa molto economica, ma il sapore non è affatto male.", TtsScriptPt = "Nhi Snail 20k no número 262. Taxa fixa muito barata, mas o sabor não é nada mau.", TtsScriptHi = "حلزون ني 20 ألف في الرقم 262. سعر ثابت رخيص جدًا ، لكن النكهة ليست سيئة على الإطلاق.",
                    Priority = 4,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 10,
                    Name = "Quán Ốc Thảo - 383 Vĩnh Khánh",
                    NameEn = "Thao Snail - 383 Vinh Khanh",
                    NameEs = "Thao Snail - 383 Vinh Khanh", NameFr = "Escargot Thao - 383 Vinh Khanh", NameDe = "Thao Schnecke - 383 Vinh Khanh", NameZh = "草蜗牛 - 383 永庆", NameJa = "タオ カタツムリ - 383 ヴィンカン", NameKo = "타오 달팽이 - 383 빈칸", NameRu = "Улитка Тао - 383 Винь Кхань", NameIt = "Lumaca Thao - 383 Vinh Khanh", NamePt = "Caracol Thao - 383 Vinh Khanh", NameHi = "حلزون ثاو - 383 فينه خان",
                    Description = "Một trong những quán đông khách nhất phố. Giá cả phải chăng, ốc tươi ngon.",
                    DescriptionEn = "One of the most crowded stalls on the street. Affordable prices, fresh snails.",
                    DescriptionEs = "Uno de los puestos de mayor afluencia de la calle. Precios asequibles, caracoles frescos.", DescriptionFr = "Un des stands les plus fréquentés de la rue. Prix ​​abordables, escargots frais.", DescriptionDe = "Einer der am meisten besuchten Stände auf der Straße. Erschwingliche Preise, frische Schnecken.", DescriptionZh = "这条街上最拥挤的摊位之一。价格实惠，蜗牛新鲜。", DescriptionJa = "通りで最も混雑している屋台の一つ。手頃な価格、新鮮なカタツムリ。", DescriptionKo = "거리에서 가장 붐비는 노점 중 하나. 저렴한 가격, 신선한 달팽이.", DescriptionRu = "Один из самых оживленных киосков на улице. Доступные цены, свежие улитки.", DescriptionIt = "Una delle bancarelle più affollate della strada. Prezzi abbordabili, lumache fresche.", DescriptionPt = "Uma das bancas de maior afluência da rua. Preços acessíveis, caracóis frescos.", DescriptionHi = "أحد أكثر الأكشاك ازدحامًا في الشارع. أسعار معقولة، حلزون طازج.",
                    Latitude = 10.76168,
                    Longitude = 106.70236,
                    Radius = 35,
                    TtsScript = "Gần đến Ốc Thảo rồi. Số 383, quán rất đông khách, bạn phải xếp hàng đấy.",
                    TtsScriptEn = "Almost at Thao Snail. Number 383, very crowded, you might have to queue.",
                    TtsScriptEs = "Casi en Thao Snail. Número 383, muy concurrido, puede que tengas que hacer cola.", TtsScriptFr = "Presque à Escargot Thao. Numéro 383, très bondé, vous devrez peut-être faire la queue.", TtsScriptDe = "Fast bei der Thao Schnecke. Nummer 383, sehr voll, vielleicht müssen Sie anstehen.", TtsScriptZh = "快到草蜗牛了。383号，非常挤，你可能必须排队。", TtsScriptJa = "もうすぐタオカタツムリです。383番、非常に混雑しているので並ぶ必要があるかもしれません。", TtsScriptKo = "타오 달팽이에 거의 다 왔습니다. 383번, 매우 붐비므로 줄을 서야 할 수도 있습니다.", TtsScriptRu = "Почти у улитки Тао. Номер 383, очень многолюдно, возможно, придется отстоять в очереди.", TtsScriptIt = "Quasi da Lumaca Thao. Numero 383, molto affollato, potresti dover fare la fila.", TtsScriptPt = "Quase na Thao Snail. Número 383, muito lotado, pode ser necessário fazer fila.", TtsScriptHi = "بالقرب من حلزون تاو. رقم 383، مزدحم للغاية، قد تحتاج إلى الانتظار.",
                    Priority = 5,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 11,
                    Name = "Sushi Cô Bông - 390 Vĩnh Khánh",
                    NameEn = "Co Bong Sushi - 390 Vinh Khanh",
                    NameEs = "Co Bong Sushi - 390 Vinh Khanh", NameFr = "Sushi Co Bong - 390 Vinh Khanh", NameDe = "Co Bong Sushi - 390 Vinh Khanh", NameZh = "Bong姑寿司 - 390 永庆", NameJa = "コーボン寿司 - 390 ヴィンカン", NameKo = "코봉 스시 - 390 빈칸", NameRu = "Суши Ко Бонг - 390 Винь Кхань", NameIt = "Sushi Co Bong - 390 Vinh Khanh", NamePt = "Sushi Co Bong - 390 Vinh Khanh", NameHi = "سوشي كو بونج - 390 فينه خان",
                    Description = "Sushi Nhật Bản chất lượng trên vỉa hè Sài Gòn. Cá hồi tươi, cơm dẻo, giá mềm.",
                    DescriptionEn = "Quality Japanese sushi on Saigon sidewalk. Fresh salmon, sticky rice, soft prices.",
                    DescriptionEs = "Sushi japonés de calidad en la acera de Saigón. Salmón fresco, arroz pegajoso, precios suaves.", DescriptionFr = "Sushi japonais de qualité sur le trottoir de Saigon. Saumon frais, riz gluant, prix doux.", DescriptionDe = "Qualitativ hochwertiges japanisches Sushi auf dem Bürgersteig von Saigon. Frischer Lachs, klebriger Reis, sanfte Preise.", DescriptionZh = "西贡街头优质的日本寿司。新鲜鲑鱼，糯米饭，价格温柔。", DescriptionJa = "サイゴンの歩道の高品質な日本の寿司。新鮮な鮭、もち米、ソフトな価格。", DescriptionKo = "사이공 거리의 고품질 일본 초밥. 신선한 연어, 찹쌀, 부드러운 가격.", DescriptionRu = "Качественные японские суши на тротуаре Сайгона. Свежий лосось, клейкий рис, низкие цены.", DescriptionIt = "Sushi giapponese di qualità sul marciapiede di Saigon. Salmone fresco, riso appiccicoso, prezzi bassi.", DescriptionPt = "Sushi japonês de qualidade na calçada de Saigon. Salmão fresco, arroz pegajoso, preços baixos.", DescriptionHi = "سوشي ياباني عالي الجودة على رصيف سيغون. سلمون طازج وأرز لزج وأسعار معتدلة.",
                    Latitude = 10.76166,
                    Longitude = 106.70240,
                    Radius = 30,
                    TtsScript = "Sushi Cô Bông ở ngay số 390. Thêm một lựa chọn Nhật Bản tuyệt vời trên phố Vĩnh Khánh.",
                    TtsScriptEn = "Co Bong Sushi right at 390. Another great Japanese choice on Vinh Khanh street.",
                    TtsScriptEs = "Co Bong Sushi en 390. Otra gran opción japonesa en la calle Vinh Khanh.", TtsScriptFr = "Le Sushi Cô Bong juste au 390. Un autre excellent choix japonais dans la rue Vinh Khanh.", TtsScriptDe = "Co Bong Sushi direkt am 390. Eine weitere großartige japanische Wahl in der Vinh Khanh Straße.", TtsScriptZh = "Bong姑寿司就在390号。永庆街上的又一大日本选择。", TtsScriptJa = "390にあるコーボン寿司。ヴィン・カイン通りにあるもう一つの素晴らしい日本の選択肢。", TtsScriptKo = "390에 있는 코봉 스시. 빈 칸 거리의 또 다른 훌륭한 일본 선택.", TtsScriptRu = "Суши Ко Бонг прямо на 390. Еще один отличный японский выбор на улице Винь Кхань.", TtsScriptIt = "Sushi Co Bong proprio al 390. Un'altra ottima scelta giapponese sulla strada Vinh Khanh.", TtsScriptPt = "Sushi Co Bong no 390. Outra excelente escolha japonesa na rua Vinh Khanh.", TtsScriptHi = "سوشي كو بونج مباشرة في 390. خيار ياباني رائع آخر في شارع فينه خان.",
                    Priority = 1,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },
                new Poi
                {
                    Id = 12,
                    Name = "Ốc Đêm - 474 Vĩnh Khánh",
                    NameEn = "Night Snail - 474 Vinh Khanh",
                    NameEs = "Night Snail - 474 Vinh Khanh", NameFr = "Escargot de Nuit - 474 Vinh Khanh", NameDe = "Nachtschnecke - 474 Vinh Khanh", NameZh = "夜蜗牛 - 474 永庆", NameJa = "夜のカタツムリ - 474 ヴィンカン", NameKo = "밤 달팽이 - 474 빈칸", NameRu = "Ночная Улитка - 474 Винь Кхань", NameIt = "Lumaca Notturna - 474 Vinh Khanh", NamePt = "Caracol Noturno - 474 Vinh Khanh", NameHi = "حلزون الليل - 474 فينه خان",
                    Description = "Chuyên phục vụ buổi tối và đêm khuya. Ốc nướng mỡ hành và nghêu hấp sả là đặc sản.",
                    DescriptionEn = "Specializes in late night service. Scallion grilled snails and steamed clams are signatures.",
                    DescriptionEs = "Se especializa en servicio nocturno. Los caracoles a la parrilla y las almejas al vapor son los platos principales.", DescriptionFr = "Spécialisé dans le service de fin de nuit. Les escargots grillés à la ciboule et les palourdes cuites à la vapeur en sont la marque.", DescriptionDe = "Spezialisiert auf Late-Night-Service. Gegrillte Schnecken mit Frühlingszwiebeln und gedämpfte Muscheln sind die Signaturen.", DescriptionZh = "专营深夜服务。烤蜗牛和清蒸蛤蜊是特色。", DescriptionJa = "深夜営業を専門としています。ネギ焼きのカタツムリと蒸しアサリが名物です。", DescriptionKo = "심야 서비스 전문. 파 구이 달팽이와 찐 조개가 주력 메뉴입니다.", DescriptionRu = "Специализируется на ночном обслуживании. Приготовленные на гриле улитки с луком и приготовленные на пару моллюски являются визитной карточкой.", DescriptionIt = "Specializzato nel servizio a tarda notte. Le lumache alla griglia alla cipolla e le vongole al vapore sono le specialità.", DescriptionPt = "É especialista no serviço nocturno. Os caracóis grelhados na cebola e as amêijoas no vapor são as assinaturas.", DescriptionHi = "متخصص في خدمة الليل في وقت متأخر. الحلزونات المشوية بالبصل والبطلينوس على البخار هي العناصر المتخصصة.",
                    Latitude = 10.76050,
                    Longitude = 106.70413,
                    Radius = 30,
                    TtsScript = "Ốc Đêm Vĩnh Khánh số 474. Trời tối rồi mới là lúc quán này đông nhất.",
                    TtsScriptEn = "Night Snail Vinh Khanh number 474. It is only when it gets dark that this place is most crowded.",
                    TtsScriptEs = "Night Snail Vinh Khanh número 474. Solo cuando oscurece, este lugar está más lleno.", TtsScriptFr = "Escargot de nuit Vinh Khanh numéro 474. Ce n'est qu'à la tombée de la nuit que cet endroit est le plus bondé.", TtsScriptDe = "Nachtschnecke Vinh Khanh Nummer 474. Erst wenn es dunkel wird, ist es hier am vollsten.", TtsScriptZh = "夜蜗牛永庆474号。 只有天黑时，这里才最拥挤。", TtsScriptJa = "夜のカタツムリヴィン・カイン474号。 暗くなると、ここが最も混雑します。", TtsScriptKo = "밤 달팽이 빈 칸 474번. 어두워 져야 이곳이 가장 붐빕니다.", TtsScriptRu = "Ночная Улитка Винь Кхань 474. Только когда становится темно, здесь больше всего людей.", TtsScriptIt = "Lumaca notturna Vinh Khanh numero 474. È solo quando fa buio che questo posto è più affollato.", TtsScriptPt = "Night Snail Vinh Khanh número 474. É apenas quando escurece que este lugar está mais cheio.", TtsScriptHi = "حلزون ليل فينه خان رقم 474. فقط عندما يحل الظلام يصبح هذا المكان مزدحمًا.",
                    Priority = 2,
                    ImageUrl = "nuong_vinh_khanh_1773306623915.png"
                },
                new Poi
                {
                    Id = 13,
                    Name = "Ốc Oanh - 534 Vĩnh Khánh",
                    NameEn = "Oanh Snail - 534 Vinh Khanh",
                    NameEs = "Oanh Snail - 534 Vinh Khanh", NameFr = "Escargot Oanh - 534 Vinh Khanh", NameDe = "Oanh Schnecke - 534 Vinh Khanh", NameZh = "莺蜗牛 - 534 永庆", NameJa = "オアン カタツムリ - 534 ヴィンカン", NameKo = "오안 달팽이 - 534 빈칸", NameRu = "Улитка Оань - 534 Винь Кхань", NameIt = "Lumaca Oanh - 534 Vinh Khanh", NamePt = "Caracol Oanh - 534 Vinh Khanh", NameHi = "حلزون أوانه - 534 فينه خان",
                    Description = "Quán ốc huyền thoại, sầm uất nhất phố. Được Michelin Bib Gourmand giới thiệu. Ốc hương rang muối ớt là món trứ danh.",
                    DescriptionEn = "Legendary stall, busiest on the street. Michelin Bib Gourmand recommended. Famous chili salt snails.",
                    DescriptionEs = "Puesto legendario, el más concurrido de la calle. Recomendado por el premio Michelin Bib Gourmand. Famosos caracoles con salsa picante.", DescriptionFr = "Stand légendaire, le plus animé de la rue. Recommandé par le label Michelin Bib Gourmand. Famoses escargots à la sauce épicée.", DescriptionDe = "Legendärer Stand, der belebteste auf der Straße. Von Michelin Bib Gourmand empfohlen. Berühmte Schnecken mit Chili-Salz.", DescriptionZh = "传说中的摊位，这是这条街上最繁忙的。 被米其林围脖美食推荐。 著名的辣椒盐螺。", DescriptionJa = "通りの伝説的な屋台。 ミシュランのビブグルマン賞に推薦されました。 有名なチリ・ソルトカタツムリ。", DescriptionKo = "길거리에서 가장 분주한 전설적인 포장 마차. 미쉐린 빕 구르망 수상자가 추천했습니다. 유명한 칠리 소금 달팽이.", DescriptionRu = "Легендарный киоск, самый оживленный на улице. Рекомендовано Michelin Bib Gourmand. Знаменитые улитки с чили и солью.", DescriptionIt = "Bancarella leggendaria, la più affollata della strada. Consigliato Michelin Bib Gourmand. Famose lumache al sale e peperoncino.", DescriptionPt = "Mítico bar, o mais movimentado da rua. Recomendado pelo Michelin Bib Gourmand. Famosos caracóis com sal de pimenta.", DescriptionHi = "كشك أسطوري، الأكثر ازدحامًا في الشارع. موصى به من قبل ميشلان بيب جورماند. حلزون بالملح والفلفل الحار المشهور.",
                    Latitude = 10.76072,
                    Longitude = 106.70330,
                    Radius = 40,
                    TtsScript = "Chào mừng bạn đến với Ốc Oanh - ông trùm ốc phố Vĩnh Khánh. Đừng quên thử ốc hương rang muối ớt trứ danh nhé!",
                    TtsScriptEn = "Welcome to Oanh Snail - the boss of Vinh Khanh street. Don't forget to try the famous chili salt roasted sweet snails!",
                    TtsScriptEs = "Bienvenido a Oanh Snail: el jefe de la calle Vinh Khanh. ¡No olvides probar los famosos caracoles dulces asados ​​con sal y ají!", TtsScriptFr = "Bienvenue à Escargot Oanh, le chef de la rue Vinh Khanh. N'oubliez pas d'essayer les célèbres escargots doux rôtis au sel et au chili !", TtsScriptDe = "Willkommen bei der Oanh-Schnecke – dem Chefkoch der Vinh-Khanh-Straße. Vergessen Sie nicht, die berühmten, mit Chili-Salz gerösteten süßen Schnecken zu probieren!", TtsScriptZh = "欢迎来到莺蜗牛-永庆街老板。不要忘了尝试著名的辣椒盐烤蜗牛！", TtsScriptJa = "ヴィンカン通りのボスであるオアンカタツムリへようこそ。 有名なチリ・ソルトローストカタツムリを試すのを忘れないでください！", TtsScriptKo = "빈칸 거리의 보스인 오안 달팽이에 오신 것을 환영합니다. 유명한 칠리 소금 구이 달팽이를 잊지 마세요!", TtsScriptRu = "Добро пожаловать в компанию Улитка Оань - босса улицы Винь Кхань. Не забудьте попробовать знаменитых улиток, запеченных с чили и солью!", TtsScriptIt = "Benvenuti a Oanh Snail, il capo della via Vinh Khanh. Non dimenticate di provare le famose lumache dolci arrosto con sale e peperoncino!", TtsScriptPt = "Bem-vindo ao Oanh Snail - o chefe da rua Vinh Khanh. Não se esqueça de experimentar os famosos caracóis doces assados ​​​​com sal e pimenta!", TtsScriptHi = "مرحبًا بك في حلزون أوانه - زعيم شارع فينه خان. لا تنس تجربة حلزون حلو محمص بالملح والفلفل الحار الشهير!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 14,
                    Name = "Bún Nước Cô Có - 39 Vĩnh Khánh",
                    NameEn = "Co Co Noodle - 39 Vinh Khanh",
                    NameEs = "Co Co Noodle - 39 Vinh Khanh", NameFr = "Nouilles Co Co - 39 Vinh Khanh", NameDe = "Co Co Nudel - 39 Vinh Khanh", NameZh = "Co Co 汤粉 - 39 永庆", NameJa = "Co Co ヌードル - 39 ヴィンカン", NameKo = "Co Co 누들 - 39 빈칸", NameRu = "Лапша Co Co - 39 Винь Кхань", NameIt = "Spaghetti Co Co - 39 Vinh Khanh", NamePt = "Macarrão Co Co - 39 Vinh Khanh", NameHi = "نودلز كو كو - 39 فينه خان",
                    Description = "Bún nước lèo đậm đà đặc sản Sài Gòn ngọt thanh, topping hải sản siêu ngon.",
                    DescriptionEn = "Special Saigon sweet broth noodles, super delicious seafood toppings.",
                    DescriptionEs = "Fideos especiales en caldo dulce de Saigón, ingredientes súper deliciosos con mariscos.", DescriptionFr = "Nouilles spéciales au bouillon sucré de Saigon, garnitures de fruits de mer super délicieuses.", DescriptionDe = "Spezielle Saigon-Süßbrühe-Nudeln, super leckere Meeresfrüchte-Toppings.", DescriptionZh = "西贡特色甜汤粉，非常美味的海鲜浇头。", DescriptionJa = "特製のサイゴン甘酒ヌードル、とても美味しいシーフードのトッピング。", DescriptionKo = "특별한 사이공 단 국물 누들, 아주 맛있는 해산물 토핑.", DescriptionRu = "Специальная сайгонская лапша на сладком бульоне, очень вкусные топпинги из морепродуктов.", DescriptionIt = "Speciali spaghetti al brodo dolce di Saigon, condimenti per frutti di mare deliziosi.", DescriptionPt = "Macarrão especial com caldo doce de Saigon, coberturas de frutos do mar super deliciosas.", DescriptionHi = "سايغون الحلو الخاص مرق المعكرونة ، طبقة المأكولات البحرية اللذيذة.",
                    Latitude = 10.76210,
                    Longitude = 106.70280,
                    Radius = 30,
                    TtsScript = "Hãy thử qua Bún Nước Cô Có nhé. Nước lèo đậm đà, ăn một lần là nhớ mãi.",
                    TtsScriptEn = "Try Co Co Noodle. The broth is rich, once you eat it you will remember it forever.",
                    TtsScriptEs = "Prueba Co Co Noodle. El caldo es rico, una vez que lo comas lo recordarás para siempre.", TtsScriptFr = "Essayez les nouilles Co Co. Le bouillon est riche, une fois que vous l'aurez mangé, vous vous en souviendrez pour toujours.", TtsScriptDe = "Probieren Sie Co Co Noodle. Die Brühe ist reichhaltig, wenn Sie sie einmal gegessen haben, werden Sie sich ewig daran erinnern.", TtsScriptZh = "试试 Co Co 汤粉。 肉汤浓郁，吃过一次就会永远记住。", TtsScriptJa = "Co Coヌードルをお試しください。 スープが濃厚で、一度食べたら忘れられない味です。", TtsScriptKo = "Co Co 누들을 먹어보세요. 국물이 진해서 한 번 먹으면 영원히 기억될 것입니다.", TtsScriptRu = "Попробуйте лапшу Co Co. Бульон наваристый, один раз попробуешь - запомнишь навсегда.", TtsScriptIt = "Prova Co Co Noodle. Il brodo è ricco, una volta mangiato lo ricorderai per sempre.", TtsScriptPt = "Experimente o Co Co Noodle. O caldo é rico, depois de comê-lo você vai se lembrar dele para sempre.", TtsScriptHi = "Co Co Noodle جرب. المرق غني ، بمجرد أن تأكله سوف تتذكره إلى الأبد.",
                    Priority = 1,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 15,
                    Name = "Lẩu Bò Khu Ba - 180 Vĩnh Khánh",
                    NameEn = "Zone 3 Beef Hotpot - 180 Vinh Khanh",
                    NameEs = "Zone 3 Beef Hotpot - 180 Vinh Khanh", NameFr = "Fondue de boeuf Zone 3 - 180 Vinh Khanh", NameDe = "Rindfleisch Hotpot Zone 3 - 180 Vinh Khanh", NameZh = "三区牛肉火锅 - 180 永庆", NameJa = "ゾーン3ビーフホットポット - 180 ヴィンカン", NameKo = "구역 3 소고기 전골 - 180 빈칸", NameRu = "Говяжий Хот-пот Зона 3 - 180 Винь Кхань", NameIt = "Hotpot di manzo Zona 3 - 180 Vinh Khanh", NamePt = "Hotpot de carne Zona 3 - 180 Vinh Khanh", NameHi = "طاجن لحم البقر المنطقة 3 - 180 فينه خان",
                    Description = "Quán lẩu bò, bò nướng trứ danh bình dân đông đúc. Thịt bò tươi ngon mềm mại.",
                    DescriptionEn = "Famous affordable beef hotpot and grilled beef stall. Fresh and tender beef.",
                    DescriptionEs = "Famoso puesto asequible de estofado de ternera y ternera a la parrilla. Carne fresca y tierna.", DescriptionFr = "Célèbre stand abordable de fondue de bœuf et de bœuf grillé. Bœuf frais et tendre.", DescriptionDe = "Berühmter erschwinglicher Rindfleisch-Hotpot und gegrillter Rindfleisch-Stand. Frisches und zartes Rindfleisch.", DescriptionZh = "著名的价格实惠的牛肉火锅和烤牛肉摊。牛肉新鲜嫩滑。", DescriptionJa = "有名な手頃な価格のビーフホットポットと焼き肉屋台。 新鮮で柔らかい牛肉。", DescriptionKo = "유명하고 저렴한 소고기 전골 및 구운 소고기 노점. 신선하고 부드러운 소고기.", DescriptionRu = "Знаменитый недорогой горячий котелок с говядиной и киоск с говядиной на гриле. Свежая и нежная говядина.", DescriptionIt = "Famosa bancarella di manzo caldo e manzo alla griglia a prezzi accessibili. Manzo fresco e tenero.", DescriptionPt = "Famosa e acessível churrasqueira e fondue de carne. Carne fresca e macia.", DescriptionHi = "وعاء ساخن من اللحم البقري الشهير وبأسعار معقولة وكشك لحم بقري مشوي. لحم بقر طازج وطري.",
                    Latitude = 10.76061,
                    Longitude = 106.70425,
                    Radius = 30,
                    TtsScript = "Hãy ghé thử Lẩu Bò Khu Ba cực kỳ đông đúc ở số 180 Vĩnh Khánh. Thịt sườn bò nướng tuyệt cú mèo.",
                    TtsScriptEn = "Try the incredibly crowded Zone 3 Beef Hotpot at 180 Vinh Khanh. The grilled beef ribs are fantastic.",
                    TtsScriptEs = "Pruebe el estofado de ternera de la Zona 3 increíblemente concurrido en 180 Vinh Khanh. Las costillas de ternera asadas son fantásticas.", TtsScriptFr = "Essayez la fondue au boeuf de la zone 3 incroyablement bondée au 180 Vinh Khanh. Les côtes de bœuf grillées sont fantastiques.", TtsScriptDe = "Probieren Sie den unglaublich überfüllten Zone 3 Beef Hotpot bei 180 Vinh Khanh. Die gegrillten Rinderrippen sind fantastisch.", TtsScriptZh = "在180永庆尝试拥挤的三区牛肉火锅。 烤牛排棒极了。", TtsScriptJa = "180ヴィンカンにある信じられないほど混雑しているゾーン3ビーフホットポットをお試しください。 焼き牛カルビは素晴らしいです。", TtsScriptKo = "180 빈칸에서 엄청나게 붐비는 구역 3 소고기 전골을 사용해보십시오. 구운 소갈비는 환상적입니다.", TtsScriptRu = "Попробуйте невероятно многолюдный Хот-пот из говядины Зоны 3, на улице 180 Винь Кхань. Говяжьи ребрышки на гриле просто фантастические.", TtsScriptIt = "Prova l'incredibilmente affollato Hotpot di manzo della Zona 3 in 180 Vinh Khanh. Le costolette di manzo alla griglia sono fantastiche.", TtsScriptPt = "Experimente o Hotpot de carne incrivelmente lotado da Zona 3 em 180 Vinh Khanh. As costelas de vaca grelhadas são fantásticas.", TtsScriptHi = "180 فينه خان في طاجن لحم البقر في المنطقة 3 المزدحمة بشكل لا يصدق جرب. ضلوع اللحم البقري المشوي رائعة.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                }
            };
        }
    }
}
