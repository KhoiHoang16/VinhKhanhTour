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
        

        [Ignore]
        public string DisplayImage => string.IsNullOrWhiteSpace(ImageUrl) ? "poi_placeholder.png" : ImageUrl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayDistanceText))]
        [NotifyPropertyChangedFor(nameof(ListDisplayDistanceText))]
        private double? _distanceToUser;

        [Ignore]
        public string DisplayDistanceText => DistanceToUser.HasValue 
            ? $"ðŸ“ {Services.LocalizationResourceManager.Instance["CÃ¡ch báº¡n"]}: {DistanceToUser.Value:F0}m" 
            : $"ðŸ“ {Services.LocalizationResourceManager.Instance["Äang Ä‘á»‹nh vá»‹..."]}";

        [Ignore]
        public string ListDisplayDistanceText => DistanceToUser.HasValue ? $"ðŸ“ {DistanceToUser.Value:F0}m" : "ðŸ“ ---";

        [Ignore]
        public string DisplayRadiusText => $"ðŸ“ {Services.LocalizationResourceManager.Instance["BÃ¡n kÃ­nh"]}: {Radius}m";

        [Ignore]
        public string ListDisplayRadiusText => $"ðŸ“ {Radius}m";

        public string PrimaryCategory { get; set; } = string.Empty;

        [Ignore]
        public string DynamicPrimaryCategory
        {
            get 
            {
                if (!string.IsNullOrEmpty(PrimaryCategory)) return PrimaryCategory;
                
                string content = (Name + " " + Description).ToLowerInvariant();
                if (content.Contains("sushi")) return "Sushi";
                if (content.Contains("bÃºn") || content.Contains("láº©u") || content.Contains("noodle") || content.Contains("hotpot")) return "Noodle";
                if (content.Contains("nÆ°á»›ng") || content.Contains("bbq")) return "BBQ";
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
                    "BBQ" => "ðŸ– Äá»“ nÆ°á»›ng",
                    "Noodle" => "ðŸœ MÃ³n nÆ°á»›c",
                    "Sushi" => "ðŸ£ Sushi",
                    _ => "ðŸš á»c & Háº£i sáº£n"
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
                    "BBQ" => "ðŸ– Äá»“ nÆ°á»›ng",
                    "Noodle" => "ðŸœ MÃ³n nÆ°á»›c",
                    "Sushi" => "ðŸ£ Sushi",
                    _ => "ðŸš á»c"
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
                    Name = "QuÃ¡n á»c VÅ© - 37 VÄ©nh KhÃ¡nh",
                    NameEn = "Vu Snail - 37 Vinh Khanh",
                    NameEs = "Vu Snail - 37 Vinh Khanh", NameFr = "Escargot Vu - 37 Vinh Khanh", NameDe = "Vu Schnecke - 37 Vinh Khanh", NameZh = "æ­¦èœ—ç‰› - 37 æ°¸åº†", NameJa = "ãƒ´ãƒ¼ã‚«ã‚¿ãƒ„ãƒ ãƒª - 37 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ë¶€ ë‹¬íŒ½ì´ - 37 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð’Ñƒ - 37 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Vu - 37 Vinh Khanh", NamePt = "Caracol Vu - 37 Vinh Khanh",
                    Description = "QuÃ¡n á»‘c bÃ¬nh dÃ¢n ná»•i tiáº¿ng vá»›i cÃ¡c mÃ³n xÃ o bÆ¡ tá»i thÆ¡m lá»«ng. Ráº¥t Ä‘Æ°á»£c lÃ²ng giá»›i tráº» SÃ i GÃ²n.",
                    DescriptionEn = "Popular snail stall famous for garlic butter stir-fry. Loved by young Saigonese.",
                    DescriptionEs = "Puesto popular famoso por el salteado con mantequilla de ajo. Amado por los jÃ³venes de SaigÃ³n.", DescriptionFr = "Stand populaire cÃ©lÃ¨bre pour le sautÃ© au beurre Ã  l'ail. AimÃ© par les jeunes Saigonnais.", DescriptionDe = "Beliebter Stand, berÃ¼hmt fÃ¼r Knoblauchbutter-Pfannengerichte. Geliebt von jungen Saigonesen.", DescriptionZh = "å—æ¬¢è¿Žçš„æ‘Šä½ï¼Œä»¥å¤§è’œå¥¶æ²¹ç‚’èœé—»åã€‚å—åˆ°è¥¿è´¡å¹´è½»äººçš„å–œçˆ±ã€‚", DescriptionJa = "ã‚¬ãƒ¼ãƒªãƒƒã‚¯ãƒã‚¿ãƒ¼ç‚’ã‚ã§æœ‰åãªäººæ°—ã®å±‹å°ã€‚ã‚µã‚¤ã‚´ãƒ³ã®è‹¥è€…ã«æ„›ã•ã‚Œã¦ã„ã¾ã™ã€‚", DescriptionKo = "ë§ˆëŠ˜ ë²„í„° ë³¶ìŒìœ¼ë¡œ ìœ ëª…í•œ ì¸ê¸° ë…¸ì . ì Šì€ ì‚¬ì´ê³µì¸ë“¤ì—ê²Œ ì‚¬ëž‘ë°›ìŠµë‹ˆë‹¤.", DescriptionRu = "ÐŸÐ¾Ð¿ÑƒÐ»ÑÑ€Ð½Ñ‹Ð¹ ÐºÐ¸Ð¾ÑÐº, Ð¸Ð·Ð²ÐµÑÑ‚Ð½Ñ‹Ð¹ Ð¶Ð°Ñ€ÐµÐ½Ñ‹Ð¼ Ð² Ñ‡ÐµÑÐ½Ð¾Ñ‡Ð½Ð¾Ð¼ Ð¼Ð°ÑÐ»Ðµ. Ð›ÑŽÐ±Ð¸Ð¼ Ð¼Ð¾Ð»Ð¾Ð´Ñ‹Ð¼Ð¸ ÑÐ°Ð¹Ð³Ð¾Ð½Ñ†Ð°Ð¼Ð¸.", DescriptionIt = "Bancarella popolare famosa per il saltato nel burro all'aglio. Amata dai giovani saigonesi.", DescriptionPt = "Banca popular famosa pelo refogado na manteiga de alho. Amada pelos jovens saigoneses.",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = "á»c VÅ© lÃ  Ä‘iá»ƒm Ä‘áº¿n yÃªu thÃ­ch cá»§a giá»›i tráº». Äá»«ng bá» qua mÃ³n á»‘c mÃ³ng tay xÃ o bÆ¡ tá»i nhÃ©.",
                    TtsScriptEn = "Vu Snail is a favorite spot for young people. Don't miss the bamboo snails stir-fried with garlic butter.",
                    TtsScriptEs = "Vu Snail es un lugar favorito para los jÃ³venes. No te pierdas los caracoles con mantequilla de ajo.", TtsScriptFr = "L'escargot Vu est un endroit prÃ©fÃ©rÃ© des jeunes. Ne manquez pas les escargots au beurre Ã  l'ail.", TtsScriptDe = "Vu Schnecke ist ein beliebter Ort fÃ¼r junge Leute. Verpassen Sie nicht die Schnecken mit Knoblauchbutter.", TtsScriptZh = "æ­¦èœ—ç‰›æ˜¯å¹´è½»äººå–œçˆ±çš„åœ°æ–¹ã€‚ä¸è¦é”™è¿‡å¤§è’œé»„æ²¹ç«¹è›ã€‚", TtsScriptJa = "ãƒ´ãƒ¼ã‚«ã‚¿ãƒ„ãƒ ãƒªã¯è‹¥è€…ã®ãŠæ°—ã«å…¥ã‚Šã®ã‚¹ãƒãƒƒãƒˆã§ã™ã€‚ã‚¬ãƒ¼ãƒªãƒƒã‚¯ãƒã‚¿ãƒ¼ã‚«ã‚¿ãƒ„ãƒ ãƒªã‚’ãŠè¦‹é€ƒã—ãªãã€‚", TtsScriptKo = "ë¶€ ë‹¬íŒ½ì´ëŠ” ì Šì€ì´ë“¤ì´ ì¦ê²¨ ì°¾ëŠ” ê³³ìž…ë‹ˆë‹¤. ë§ˆëŠ˜ ë²„í„° ë‹¬íŒ½ì´ë¥¼ ë†“ì¹˜ì§€ ë§ˆì„¸ìš”.", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð’Ñƒ - Ð»ÑŽÐ±Ð¸Ð¼Ð¾Ðµ Ð¼ÐµÑÑ‚Ð¾ Ð¼Ð¾Ð»Ð¾Ð´ÐµÐ¶Ð¸. ÐÐµ Ð¿Ñ€Ð¾Ð¿ÑƒÑÑ‚Ð¸Ñ‚Ðµ ÑƒÐ»Ð¸Ñ‚Ð¾Ðº Ñ Ñ‡ÐµÑÐ½Ð¾Ñ‡Ð½Ñ‹Ð¼ Ð¼Ð°ÑÐ»Ð¾Ð¼.", TtsScriptIt = "La lumaca Vu Ã¨ il posto preferito dai giovani. Non perdetevi le lumache al burro all'aglio.", TtsScriptPt = "O caracol Vu Ã© um local favorito para os jovens. NÃ£o perca os caracÃ³is com manteiga de alho.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 2,
                    Name = "á»c PhÃ¡t QuÃ¡n - 46 VÄ©nh KhÃ¡nh",
                    NameEn = "Phat Snail - 46 Vinh Khanh",
                    NameEs = "Phat Snail - 46 Vinh Khanh", NameFr = "Escargot Phat - 46 Vinh Khanh", NameDe = "Phat Schnecke - 46 Vinh Khanh", NameZh = "æ³•èœ—ç‰› - 46 æ°¸åº†", NameJa = "ãƒ•ã‚¡ãƒƒãƒˆã‚«ã‚¿ãƒ„ãƒ ãƒª - 46 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "íŒŸ ë‹¬íŒ½ì´ - 46 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð¤Ð°Ñ‚ - 46 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Phat - 46 Vinh Khanh", NamePt = "Caracol Phat - 46 Vinh Khanh",
                    Description = "á»c len xÃ o dá»«a vÃ  cÃ¡c mÃ³n sa táº¿ cay ná»“ng lÃ  niá»m tá»± hÃ o cá»§a quÃ¡n. GiÃ¡ cáº£ pháº£i chÄƒng.",
                    DescriptionEn = "Coconut milk mud snails and spicy satay dishes are their pride. Affordable prices.",
                    DescriptionEs = "Caracoles de barro con leche de coco y platos picantes de satay son su orgullo. Precios asequibles.", DescriptionFr = "Les escargots de boue au lait de coco et les plats satay Ã©picÃ©s font leur fiertÃ©. Prix abordables.", DescriptionDe = "Kokosmilch-Schlammschnecken und wÃ¼rzige Satay-Gerichte sind ihr Stolz. Erschwingliche Preise.", DescriptionZh = "æ¤°å¥¶æ³¥èžºå’Œé¦™è¾£æ²™çˆ¹èœè‚´æ˜¯ä»–ä»¬çš„éª„å‚²ã€‚ä»·æ ¼å®žæƒ ã€‚", DescriptionJa = "ã‚³ã‚³ãƒŠãƒƒãƒ„ãƒŸãƒ«ã‚¯ã®æ³¥ã‚«ã‚¿ãƒ„ãƒ ãƒªã¨ã‚¹ãƒ‘ã‚¤ã‚·ãƒ¼ãªã‚µãƒ†æ–™ç†ãŒå½¼ã‚‰ã®èª‡ã‚Šã§ã™ã€‚æ‰‹é ƒãªä¾¡æ ¼ã€‚", DescriptionKo = "ì½”ì½”ë„› ë°€í¬ ì§„í™ ë‹¬íŒ½ì´ì™€ ë§¤ìš´ ì‚¬í…Œ ìš”ë¦¬ê°€ ê·¸ë“¤ì˜ ìžëž‘ìž…ë‹ˆë‹¤. ì €ë ´í•œ ê°€ê²©.", DescriptionRu = "Ð“Ñ€ÑÐ·ÐµÐ²Ñ‹Ðµ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸ Ð² ÐºÐ¾ÐºÐ¾ÑÐ¾Ð²Ð¾Ð¼ Ð¼Ð¾Ð»Ð¾ÐºÐµ Ð¸ Ð¿Ñ€ÑÐ½Ñ‹Ðµ Ð±Ð»ÑŽÐ´Ð° ÑÐ°Ñ‚Ð°Ð¹ - Ð¸Ñ… Ð³Ð¾Ñ€Ð´Ð¾ÑÑ‚ÑŒ. Ð”Ð¾ÑÑ‚ÑƒÐ¿Ð½Ñ‹Ðµ Ñ†ÐµÐ½Ñ‹.", DescriptionIt = "Le lumache di fango al latte di cocco e i piatti piccanti di satay sono il loro orgoglio. Prezzi convenienti.", DescriptionPt = "CaracÃ³is de lama de leite de coco e pratos de satay picantes sÃ£o o orgulho deles. PreÃ§os acessÃ­veis.",
                    Latitude = 10.76027,
                    Longitude = 106.70301,
                    Radius = 35,
                    TtsScript = "á»c PhÃ¡t QuÃ¡n sá»‘ 46. Thá»­ mÃ³n á»‘c len xÃ o dá»«a vá»›i vá»‹ bÃ©o ngáº­y khÃ³ cÆ°á»¡ng nha.",
                    TtsScriptEn = "Phat Snail at number 46. Try the coconut mud snails with irresistible creamy flavor.",
                    TtsScriptEs = "Phat Snail en el nÃºmero 46. Prueba los caracoles de barro de coco con un irresistible sabor cremoso.", TtsScriptFr = "Escargot Phat au numÃ©ro 46. Essayez les escargots de boue de noix de coco avec une saveur crÃ©meuse irrÃ©sistible.", TtsScriptDe = "Phat Schnecke bei Nummer 46. Probieren Sie die Kokos-Schlammschnecken mit unwiderstehlich cremigem Geschmack.", TtsScriptZh = "æ³•èœ—ç‰›åœ¨46å·ã€‚è¯•è¯•æ¤°å­æ³¥èžºï¼Œæœ‰ä¸å¯æŠ—æ‹’çš„å¥¶æ²¹å‘³ã€‚", TtsScriptJa = "46ç•ªã®ãƒ•ã‚¡ãƒƒãƒˆã‚«ã‚¿ãƒ„ãƒ ãƒªã€‚æŠ—ãˆãªã„ã‚¯ãƒªãƒ¼ãƒŸãƒ¼ãªé¢¨å‘³ã®ã‚³ã‚³ãƒŠãƒƒãƒ„æ³¥ã‚«ã‚¿ãƒ„ãƒ ãƒªã‚’ãŠè©¦ã—ãã ã•ã„ã€‚", TtsScriptKo = "46ë²ˆì˜ íŒŸ ë‹¬íŒ½ì´. ì°¸ì„ ìˆ˜ ì—†ëŠ” í¬ë¦¬ë¯¸í•œ ë§›ì˜ ì½”ì½”ë„› ì§„í™ ë‹¬íŒ½ì´ë¥¼ ë§›ë³´ì„¸ìš”.", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð¤Ð°Ñ‚ Ð½Ð° Ð½Ð¾Ð¼ÐµÑ€Ðµ 46. ÐŸÐ¾Ð¿Ñ€Ð¾Ð±ÑƒÐ¹Ñ‚Ðµ Ð³Ñ€ÑÐ·ÐµÐ²Ñ‹Ñ… ÑƒÐ»Ð¸Ñ‚Ð¾Ðº Ñ ÐºÐ¾ÐºÐ¾ÑÐ¾Ð¼ Ñ Ð½ÐµÐ¿Ñ€ÐµÐ¾Ð´Ð¾Ð»Ð¸Ð¼Ñ‹Ð¼ ÑÐ»Ð¸Ð²Ð¾Ñ‡Ð½Ñ‹Ð¼ Ð²ÐºÑƒÑÐ¾Ð¼.", TtsScriptIt = "Lumaca Phat al numero 46. Prova le lumache di fango al cocco con sapore cremoso irresistibile.", TtsScriptPt = "Caracol Phat no nÃºmero 46. Experimente os caracÃ³is de lama de coco com sabor cremoso irresistÃ­vel.",
                    Priority = 2,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 3,
                    Name = "QuÃ¡n BÃ© á»c - 58 VÄ©nh KhÃ¡nh",
                    NameEn = "Be Ooc Snail - 58 Vinh Khanh",
                    NameEs = "Be Ooc Snail - 58 Vinh Khanh", NameFr = "Escargot Be Ooc - 58 Vinh Khanh", NameDe = "Be Ooc Schnecke - 58 Vinh Khanh", NameZh = "è´èœ—ç‰› - 58 æ°¸åº†", NameJa = "ãƒ™ãƒ»ã‚ªãƒƒã‚¯ ã‚«ã‚¿ãƒ„ãƒ ãƒª - 58 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ë² ì˜¥ ë‹¬íŒ½ì´ - 58 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð‘Ðµ ÐžÐ¾Ðº - 58 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Be Ooc - 58 Vinh Khanh", NamePt = "Caracol Be Ooc - 58 Vinh Khanh",
                    Description = "QuÃ¡n á»‘c nhá» xinh, chuyÃªn cÃ¡c loáº¡i á»‘c háº¥p sáº£ vÃ  nÆ°á»›ng má»¡ hÃ nh. PhÃ¹ há»£p nhÃ³m báº¡n.",
                    DescriptionEn = "Small cozy stall specializing in lemongrass steamed and scallion oil grilled snails.",
                    DescriptionEs = "PequeÃ±o puesto acogedor especializado en caracoles cocidos al vapor con hierba de limÃ³n y carne a la parrilla.", DescriptionFr = "Petit stand chaleureux spÃ©cialisÃ© dans les escargots Ã  la vapeur Ã  la citronnelle et grillÃ©s.", DescriptionDe = "Kleiner gemÃ¼tlicher Stand, der auf gedÃ¼nstete und gegrillte Schnecken spezialisiert ist.", DescriptionZh = "å°åž‹èˆ’é€‚æ‘Šä½ï¼Œä¸“è¥é¦™èŒ…æ¸…è’¸å’Œè‘±æ²¹çƒ¤èœ—ç‰›ã€‚é€‚åˆä¸€ç¾¤æœ‹å‹ã€‚", DescriptionJa = "ãƒ¬ãƒ¢ãƒ³ã‚°ãƒ©ã‚¹è’¸ã—ã¨ãƒã‚®æ²¹ç„¼ãã‚«ã‚¿ãƒ„ãƒ ãƒªã‚’å°‚é–€ã¨ã™ã‚‹å°ã•ãã¦å±…å¿ƒåœ°ã®è‰¯ã„å±‹å°ã€‚å‹äººã®ã‚°ãƒ«ãƒ¼ãƒ—ã«é©ã—ã¦ã„ã¾ã™ã€‚", DescriptionKo = "ë ˆëª¬ ê·¸ë¼ìŠ¤ ë¬´ì¹¨ê³¼ íŒŒê¸°ë¦„ êµ¬ì´ ë‹¬íŒ½ì´ë¥¼ ì „ë¬¸ìœ¼ë¡œ í•˜ëŠ” ìž‘ê³  ì•„ëŠ‘í•œ ë…¸ì . ì¹œêµ¬ ê·¸ë£¹ì— ì í•©í•©ë‹ˆë‹¤.", DescriptionRu = "ÐœÐ°Ð»ÐµÐ½ÑŒÐºÐ¸Ð¹ ÑƒÑŽÑ‚Ð½Ñ‹Ð¹ ÐºÐ¸Ð¾ÑÐº, ÑÐ¿ÐµÑ†Ð¸Ð°Ð»Ð¸Ð·Ð¸Ñ€ÑƒÑŽÑ‰Ð¸Ð¹ÑÑ Ð½Ð° ÑƒÐ»Ð¸Ñ‚ÐºÐ°Ñ… Ð½Ð° Ð¿Ð°Ñ€Ñƒ Ð¸ Ð½Ð° Ð³Ñ€Ð¸Ð»Ðµ. ÐŸÐ¾Ð´Ñ…Ð¾Ð´Ð¸Ñ‚ Ð´Ð»Ñ Ð³Ñ€ÑƒÐ¿Ð¿Ñ‹ Ð´Ñ€ÑƒÐ·ÐµÐ¹.", DescriptionIt = "Piccola bancarella accogliente specializzata in lumache al vapore e alla griglia. Adatto a un gruppo di amici.", DescriptionPt = "Pequena banca aconchegante especializada em caracÃ³is cozidos e grelhados. Adequado para um grupo de amigos.",
                    Latitude = 10.76339,
                    Longitude = 106.70206,
                    Radius = 30,
                    TtsScript = "QuÃ¡n BÃ© á»c sá»‘ 58, nhá» nhÆ°ng mÃ  hÆ°Æ¡ng vá»‹ cá»±c ká»³ Ä‘áº­m Ä‘Ã  Ä‘ang chá» báº¡n.",
                    TtsScriptEn = "Be Ooc Snail at 58, small but offering incredibly rich flavors awaiting you.",
                    TtsScriptEs = "Be Ooc Snail en el 58, pequeÃ±o pero ofrece sabores increÃ­blemente ricos esperÃ¡ndote.", TtsScriptFr = "Escargot Be Ooc Ã  58, petit mais offrant des saveurs incroyablement riches qui vous attendent.", TtsScriptDe = "Be Ooc Schnecke bei 58, klein, bietet aber unglaublich reiche Aromen, die auf Sie warten.", TtsScriptZh = "58å·çš„è´èœ—ç‰›ï¼Œå¾ˆå°ä½†æä¾›ä»¤äººéš¾ä»¥ç½®ä¿¡çš„ä¸°å¯Œå£å‘³ç­‰ä½ æ¥å“å°ã€‚", TtsScriptJa = "58ç•ªã®ãƒ™ãƒ»ã‚ªãƒƒã‚¯ãƒ»ã‚«ã‚¿ãƒ„ãƒ ãƒªã€‚å°ã•ã„ã§ã™ãŒã€ä¿¡ã˜ã‚‰ã‚Œãªã„ã»ã©è±Šã‹ãªé¢¨å‘³ãŒã‚ãªãŸã‚’å¾…ã£ã¦ã„ã¾ã™ã€‚", TtsScriptKo = "58ë²ˆì˜ ë² ì˜¥ ë‹¬íŒ½ì´. ìž‘ì§€ë§Œ ë†€ëžë„ë¡ í’ë¶€í•œ ë§›ì´ ì—¬ëŸ¬ë¶„ì„ ê¸°ë‹¤ë¦¬ê³  ìžˆìŠµë‹ˆë‹¤.", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð‘Ðµ ÐžÐ¾Ðº Ð² 58, Ð¼Ð°Ð»ÐµÐ½ÑŒÐºÐ°Ñ, Ð½Ð¾ Ð¿Ñ€ÐµÐ´Ð»Ð°Ð³Ð°ÐµÑ‚ Ð½ÐµÐ²ÐµÑ€Ð¾ÑÑ‚Ð½Ð¾ Ð±Ð¾Ð³Ð°Ñ‚Ñ‹Ðµ Ð²ÐºÑƒÑÑ‹, Ð¾Ð¶Ð¸Ð´Ð°ÑŽÑ‰Ð¸Ðµ Ð²Ð°Ñ.", TtsScriptIt = "Lumaca Be Ooc a 58, piccola ma offre sapori incredibilmente ricchi che ti aspettano.", TtsScriptPt = "Caracol Be Ooc em 58, pequeno mas oferecendo sabores incrivelmente ricos esperando por vocÃª.",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 4,
                    Name = "Sushi Ko - 122 VÄ©nh KhÃ¡nh",
                    NameEn = "Sushi Ko - 122 Vinh Khanh",
                    NameEs = "Sushi Ko - 122 Vinh Khanh", NameFr = "Sushi Ko - 122 Vinh Khanh", NameDe = "Sushi Ko - 122 Vinh Khanh", NameZh = "Sushi Ko - 122 æ°¸åº†", NameJa = "å¯¿å¸Ko - 122 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ìŠ¤ì‹œì½” - 122 ë¹ˆì¹¸", NameRu = "Ð¡ÑƒÑˆÐ¸ ÐšÐ¾ - 122 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Sushi Ko - 122 Vinh Khanh", NamePt = "Sushi Ko - 122 Vinh Khanh",
                    Description = "Sushi vá»‰a hÃ¨ Ä‘áº³ng cáº¥p nhÃ  hÃ ng. GiÃ¡ cáº£ bÃ¬nh dÃ¢n, cÃ¡ tÆ°Æ¡i ngon má»—i ngÃ y.",
                    DescriptionEn = "Restaurant quality street sushi. Casual prices, fresh fish everyday.",
                    DescriptionEs = "Sushi callejero con calidad de restaurante. Precios casuales, pescado fresco todos los dÃ­as.", DescriptionFr = "Sushi de rue de qualitÃ© restaurant. Prix â€‹â€‹dÃ©contractÃ©s, poisson frais tous les jours.", DescriptionDe = "Street-Sushi in RestaurantqualitÃ¤t. Legere Preise, jeden Tag frischer Fisch.", DescriptionZh = "é¤åŽ…å“è´¨çš„è¡—å¤´å¯¿å¸ã€‚ä»·æ ¼ä¼‘é—²ï¼Œæ¯å¤©éƒ½æœ‰æ–°é²œçš„é±¼ã€‚", DescriptionJa = "ãƒ¬ã‚¹ãƒˆãƒ©ãƒ³å“è³ªã®ã‚¹ãƒˆãƒªãƒ¼ãƒˆå¯¿å¸ã€‚ã‚«ã‚¸ãƒ¥ã‚¢ãƒ«ãªä¾¡æ ¼ã€æ¯Žæ—¥æ–°é®®ãªé­šã€‚", DescriptionKo = "ë ˆìŠ¤í† ëž‘ í’ˆì§ˆì˜ ê¸¸ê±°ë¦¬ ì´ˆë°¥. ìºì£¼ì–¼ í•œ ê°€ê²©, ë§¤ì¼ ì‹ ì„ í•œ ìƒì„ .", DescriptionRu = "Ð£Ð»Ð¸Ñ‡Ð½Ñ‹Ðµ ÑÑƒÑˆÐ¸ Ñ€ÐµÑÑ‚Ð¾Ñ€Ð°Ð½Ð½Ð¾Ð³Ð¾ ÐºÐ°Ñ‡ÐµÑÑ‚Ð²Ð°. Ð”Ð¾ÑÑ‚ÑƒÐ¿Ð½Ñ‹Ðµ Ñ†ÐµÐ½Ñ‹, ÑÐ²ÐµÐ¶Ð°Ñ Ñ€Ñ‹Ð±Ð° ÐºÐ°Ð¶Ð´Ñ‹Ð¹ Ð´ÐµÐ½ÑŒ.", DescriptionIt = "Sushi di strada di qualitÃ  da ristorante. Prezzi casual, pesce fresco tutti i giorni.", DescriptionPt = "Sushi de rua com qualidade de restaurante. PreÃ§os casuais, peixe fresco todos os dias.",
                    Latitude = 10.76073,
                    Longitude = 106.70468,
                    Radius = 25,
                    TtsScript = "Sushi Ko mang Ä‘áº¿n tráº£i nghiá»‡m áº©m thá»±c Nháº­t Báº£n ngay trÃªn vá»‰a hÃ¨. Sá»‘ 122 VÄ©nh KhÃ¡nh Ä‘Ã¢y rá»“i.",
                    TtsScriptEn = "Sushi Ko brings Japanese dining experience right to the sidewalk. Number 122 Vinh Khanh is here.",
                    TtsScriptEs = "Sushi Ko lleva la experiencia gastronÃ³mica japonesa directamente a la acera. El nÃºmero 122 de Vinh Khanh estÃ¡ aquÃ­.", TtsScriptFr = "Sushi Ko apporte l'expÃ©rience culinaire japonaise jusque sur le trottoir. Le numÃ©ro 122 Vinh Khanh est ici.", TtsScriptDe = "Sushi Ko bringt das japanische Restauranterlebnis direkt auf den BÃ¼rgersteig. Nummer 122 Vinh Khanh ist hier.", TtsScriptZh = "Sushi Koå°†æ—¥æœ¬çš„ç”¨é¤ä½“éªŒç›´æŽ¥å¸¦åˆ°äº†äººè¡Œé“ä¸Šã€‚ 122æ°¸åº†å°±åœ¨è¿™é‡Œã€‚", TtsScriptJa = "Sushi Koã¯æ—¥æœ¬ã®ãƒ€ã‚¤ãƒ‹ãƒ³ã‚°ä½“é¨“ã‚’ãã®ã¾ã¾æ­©é“ã«æŒã¡è¾¼ã¿ã¾ã™ã€‚122ç•ªã®ãƒ´ã‚£ãƒ³ã‚«ãƒ³ãŒã“ã“ã«ã‚ã‚Šã¾ã™ã€‚", TtsScriptKo = "Sushi KoëŠ” ì¼ë³¸ ì‹ë‹¹ ê²½í—˜ì„ ë³´ë„ë¡œ ë°”ë¡œ ê°€ì ¸ì˜µë‹ˆë‹¤. 122ë²ˆ ë¹ˆì¹¸ì´ ì—¬ê¸° ìžˆìŠµë‹ˆë‹¤.", TtsScriptRu = "Ð¡ÑƒÑˆÐ¸ ÐšÐ¾ Ð¿Ñ€Ð¸Ð½Ð¾ÑÐ¸Ñ‚ ÑÐ¿Ð¾Ð½ÑÐºÐ¸Ð¹ Ð¾Ð±ÐµÐ´ÐµÐ½Ð½Ñ‹Ð¹ Ð¾Ð¿Ñ‹Ñ‚ Ð¿Ñ€ÑÐ¼Ð¾ Ð½Ð° Ñ‚Ñ€Ð¾Ñ‚ÑƒÐ°Ñ€. ÐÐ¾Ð¼ÐµÑ€ 122 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ Ð·Ð´ÐµÑÑŒ.", TtsScriptIt = "Sushi Ko porta l'esperienza culinaria giapponese direttamente sul marciapiede. Il numero 122 di Vinh Khanh Ã¨ qui.", TtsScriptPt = "A Sushi Ko traz a experiÃªncia gastronÃ´mica japonesa direto para a calÃ§ada. O nÃºmero 122 de Vinh Khanh estÃ¡ aqui.",
                    Priority = 4,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },
                new Poi
                {
                    Id = 5,
                    Name = "á»c ÄÃ o 2 - 123 VÄ©nh KhÃ¡nh",
                    NameEn = "Dao Snail 2 - 123 Vinh Khanh",
                    NameEs = "Dao Snail 2 - 123 Vinh Khanh", NameFr = "Escargot Dao 2 - 123 Vinh Khanh", NameDe = "Dao Schnecke 2 - 123 Vinh Khanh", NameZh = "æ¡ƒèœ—ç‰› 2 - 123 æ°¸åº†", NameJa = "ãƒ€ã‚ªã‚«ã‚¿ãƒ„ãƒ ãƒª 2 - 123 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ë‹¤ì˜¤ ë‹¬íŒ½ì´ 2 - 123 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð”Ð°Ð¾ 2 - 123 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Dao 2 - 123 Vinh Khanh", NamePt = "Caracol Dao 2 - 123 Vinh Khanh",
                    Description = "Chi nhÃ¡nh á»c ÄÃ o Nguyá»…n TrÃ£i danh tiáº¿ng. NÆ°á»›c xá»‘t bÆ¡ tá»i gÃ¢y nghiá»‡n, háº£i sáº£n tÆ°Æ¡i sá»‘ng.",
                    DescriptionEn = "Branch of famous Dao Snail Nguyen Trai. Addictive garlic butter sauce, fresh seafood.",
                    DescriptionEs = "Sucursal de la famosa Dao Snail Nguyen Trai. Salsa de mantequilla de ajo adictiva, mariscos frescos.", DescriptionFr = "Succursale du cÃ©lÃ¨bre escargot Dao Nguyen Trai. Sauce addictive au beurre Ã  l'ail, fruits de mer frais.", DescriptionDe = "Zweig der berÃ¼hmten Dao Schnecke Nguyen Trai. SÃ¼chtig machende Knoblauchbuttersauce, frische MeeresfrÃ¼chte.", DescriptionZh = "è‘—åçš„é“èœ—ç‰›é˜®æ–‹åˆ†åº—ã€‚ä»¤äººä¸Šç˜¾çš„å¤§è’œé»„æ²¹é…±ï¼Œæ–°é²œçš„æµ·é²œã€‚", DescriptionJa = "æœ‰åãªãƒ€ã‚ªã‚«ã‚¿ãƒ„ãƒ ãƒªãƒ»ã‚°ã‚¨ãƒ³ãƒˆãƒ©ã‚¤ã®æ”¯åº—ã€‚ã‚„ã¿ã¤ãã«ãªã‚‹ã‚¬ãƒ¼ãƒªãƒƒã‚¯ãƒã‚¿ãƒ¼ã‚½ãƒ¼ã‚¹ã€æ–°é®®ãªã‚·ãƒ¼ãƒ•ãƒ¼ãƒ‰ã€‚", DescriptionKo = "ìœ ëª…í•œ ë‹¤ì˜¤ ë‹¬íŒ½ì´ ì‘ìš°ì˜Œ íŠ¸ë¼ì´ ì§€ì . ì¤‘ë…ì„± ìžˆëŠ” ë§ˆëŠ˜ ë²„í„° ì†ŒìŠ¤, ì‹ ì„ í•œ í•´ì‚°ë¬¼.", DescriptionRu = "Ð¤Ð¸Ð»Ð¸Ð°Ð» Ð·Ð½Ð°Ð¼ÐµÐ½Ð¸Ñ‚Ð¾Ð¹ Ð£Ð»Ð¸Ñ‚ÐºÐ¸ Ð”Ð°Ð¾ ÐÐ³ÑƒÐµÐ½ Ð¢Ñ€Ð°Ð¹. Ð’Ñ‹Ð·Ñ‹Ð²Ð°ÐµÑ‚ Ð¿Ñ€Ð¸Ð²Ñ‹ÐºÐ°Ð½Ð¸Ðµ Ñ‡ÐµÑÐ½Ð¾Ñ‡Ð½Ð¾-Ð¼Ð°ÑÐ»ÑÐ½Ñ‹Ð¹ ÑÐ¾ÑƒÑ, ÑÐ²ÐµÐ¶Ð¸Ðµ Ð¼Ð¾Ñ€ÐµÐ¿Ñ€Ð¾Ð´ÑƒÐºÑ‚Ñ‹.", DescriptionIt = "Ramo della famosa lumaca Dao Nguyen Trai. Addictive salsa al burro all'aglio, pesce fresco.", DescriptionPt = "Filial da famosa Dao Snail Nguyen Trai. Molho de manteiga de alho viciante, frutos do mar frescos.",
                    Latitude = 10.76114,
                    Longitude = 106.70498,
                    Radius = 35,
                    TtsScript = "á»c ÄÃ o 2 sá»‘ 123 VÄ©nh KhÃ¡nh. NÆ°á»›c xá»‘t bÆ¡ tá»i Äƒn kÃ¨m bÃ¡nh mÃ¬ á»Ÿ Ä‘Ã¢y cá»±c ká»³ gÃ¢y nghiá»‡n.",
                    TtsScriptEn = "Dao Snail 2 at 123 Vinh Khanh. The garlic butter sauce with bread here is highly addictive.",
                    TtsScriptEs = "Dao Snail 2 en el 123 de Vinh Khanh. La salsa de mantequilla de ajo con pan aquÃ­ es muy adictiva.", TtsScriptFr = "Escargot Dao 2 au 123 Vinh Khanh. La sauce au beurre Ã  l'ail avec du pain ici est trÃ¨s addictive.", TtsScriptDe = "Dao Schnecke 2 im 123 Vinh Khanh. Die Knoblauchbuttersauce mit Brot hier macht sehr sÃ¼chtig.", TtsScriptZh = "123æ°¸åº†çš„é“èœ—ç‰›2å·ã€‚è¿™é‡Œçš„å¤§è’œé»„æ²¹é…±é…é¢åŒ…éžå¸¸å®¹æ˜“ä¸Šç˜¾ã€‚", TtsScriptJa = "123ç•ªãƒ´ã‚£ãƒ³ã‚«ãƒ³ã®ãƒ€ã‚ªã‚«ã‚¿ãƒ„ãƒ ãƒª2ã€‚ã“ã“ã®ã‚¬ãƒ¼ãƒªãƒƒã‚¯ãƒã‚¿ãƒ¼ã‚½ãƒ¼ã‚¹ã¨ãƒ‘ãƒ³ã¯éžå¸¸ã«ã‚„ã¿ã¤ãã«ãªã‚Šã¾ã™ã€‚", TtsScriptKo = "123ë²ˆ ë¹ˆì¹¸ì˜ ë‹¤ì˜¤ ë‹¬íŒ½ì´ 2. ì—¬ê¸°ì˜ ë§ˆëŠ˜ ë²„í„° ì†ŒìŠ¤ì™€ ë¹µì€ ì¤‘ë…ì„±ì´ ë§¤ìš° ê°•í•©ë‹ˆë‹¤.", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð”Ð°Ð¾ 2 Ð½Ð° 123 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ. Ð§ÐµÑÐ½Ð¾Ñ‡Ð½Ð¾-Ð¼Ð°ÑÐ»ÑÐ½Ñ‹Ð¹ ÑÐ¾ÑƒÑ Ñ Ñ…Ð»ÐµÐ±Ð¾Ð¼ Ð·Ð´ÐµÑÑŒ Ð²Ñ‹Ð·Ñ‹Ð²Ð°ÐµÑ‚ ÑÐ¸Ð»ÑŒÐ½Ð¾Ðµ Ð¿Ñ€Ð¸Ð²Ñ‹ÐºÐ°Ð½Ð¸Ðµ.", TtsScriptIt = "Lumaca Dao 2 a 123 Vinh Khanh. La salsa al burro all'aglio con il pane qui Ã¨ molto avvincente.", TtsScriptPt = "Dao Snail 2 em 123 Vinh Khanh. O molho de manteiga de alho com pÃ£o aqui Ã© muito viciante.",
                    Priority = 5,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 6,
                    Name = "á»c Nhá»› SÃ i GÃ²n - 159 VÄ©nh KhÃ¡nh",
                    NameEn = "Remember Saigon Snail - 159 Vinh Khanh",
                    NameEs = "Remember Saigon Snail - 159 Vinh Khanh", NameFr = "Souviens-toi de Saigon - 159 Vinh Khanh", NameDe = "Erinnern Sie sich an Saigon - 159 Vinh Khanh", NameZh = "è®°ä½è¥¿è´¡èœ—ç‰› - 159 æ°¸åº†", NameJa = "ã‚µã‚¤ã‚´ãƒ³ã‚’æ€ã„å‡ºã™ã‚«ã‚¿ãƒ„ãƒ ãƒª - 159 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ì‚¬ì´ê³µì„ ê¸°ì–µí•˜ë¼ ë‹¬íŒ½ì´ - 159 ë¹ˆì¹¸", NameRu = "Ð’ÑÐ¿Ð¾Ð¼Ð½Ð¸Ñ‚Ðµ Ð¡Ð°Ð¹Ð³Ð¾Ð½ - 159 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Ricorda Saigon Lumaca - 159 Vinh Khanh", NamePt = "Lembre-se de Saigon Snail - 159 Vinh Khanh",
                    Description = "Menu nhiá»u mÃ³n sÃ¡ng táº¡o, phá»¥c vá»¥ nhanh nháº¹n. KhÃ´ng gian má»Ÿ thoÃ¡ng mÃ¡t, ráº¥t Ä‘Ã´ng vÃ o buá»•i tá»‘i.",
                    DescriptionEn = "Creative menu, fast service. Open airy space, very crowded in the evening.",
                    DescriptionEs = "MenÃº creativo, servicio rÃ¡pido. Espacio abierto y aireado, muy concurrido por la noche.", DescriptionFr = "Menu crÃ©atif, service rapide. Espace aÃ©rÃ© ouvert, trÃ¨s bondÃ© en soirÃ©e.", DescriptionDe = "Kreatives MenÃ¼, schneller Service. Offener luftiger Raum, abends sehr voll.", DescriptionZh = "åˆ›æ„èœå•ï¼Œæä¾›å¿«é€Ÿçš„æœåŠ¡ã€‚å¼€æ”¾é€šé£Žçš„ç©ºé—´ï¼Œæ™šä¸Šéžå¸¸æ‹¥æŒ¤ã€‚", DescriptionJa = "ã‚¯ãƒªã‚¨ã‚¤ãƒ†ã‚£ãƒ–ãªãƒ¡ãƒ‹ãƒ¥ãƒ¼ã€è¿…é€Ÿãªã‚µãƒ¼ãƒ“ã‚¹ã€‚ã‚ªãƒ¼ãƒ—ãƒ³ã§é¢¨é€šã—ã®è‰¯ã„ã‚¹ãƒšãƒ¼ã‚¹ã€å¤•æ–¹ã¯éžå¸¸ã«æ··é›‘ã—ã¦ã„ã¾ã™ã€‚", DescriptionKo = "ì°½ì˜ì ì¸ ë©”ë‰´, ë¹ ë¥¸ ì„œë¹„ìŠ¤. ê°œë°©ì ì´ê³  í†µí’ì´ ìž˜ë˜ëŠ” ê³µê°„, ì €ë…ì— ë§¤ìš° ë¶ë¹ë‹ˆë‹¤.", DescriptionRu = "ÐšÑ€ÐµÐ°Ñ‚Ð¸Ð²Ð½Ð¾Ðµ Ð¼ÐµÐ½ÑŽ, Ð±Ñ‹ÑÑ‚Ñ€Ð¾Ðµ Ð¾Ð±ÑÐ»ÑƒÐ¶Ð¸Ð²Ð°Ð½Ð¸Ðµ. ÐžÑ‚ÐºÑ€Ñ‹Ñ‚Ð¾Ðµ Ð¿Ñ€Ð¾Ð²ÐµÑ‚Ñ€Ð¸Ð²Ð°ÐµÐ¼Ð¾Ðµ Ð¿Ñ€Ð¾ÑÑ‚Ñ€Ð°Ð½ÑÑ‚Ð²Ð¾, Ð¾Ñ‡ÐµÐ½ÑŒ Ð¼Ð½Ð¾Ð³Ð¾Ð»ÑŽÐ´Ð½Ð¾ Ð²ÐµÑ‡ÐµÑ€Ð¾Ð¼.", DescriptionIt = "Menu creativo, servizio veloce. Spazio aperto e arioso, molto affollato la sera.", DescriptionPt = "Menu criativo, serviÃ§o rÃ¡pido. EspaÃ§o arejado aberto, muito lotado Ã  noite.",
                    Latitude = 10.76120,
                    Longitude = 106.70540,
                    Radius = 30,
                    TtsScript = "á»c Nhá»› SÃ i GÃ²n - cÃ¡i tÃªn nÃ³i lÃªn táº¥t cáº£. Báº¡n sáº½ nhá»› mÃ£i hÆ°Æ¡ng vá»‹ táº¡i Ä‘Ã¢y.",
                    TtsScriptEn = "Remember Saigon Snail - the name says it all. You will remember the taste here forever.",
                    TtsScriptEs = "Recuerde Saigon Snail: el nombre lo dice todo. RecordarÃ¡s el sabor de aquÃ­ para siempre.", TtsScriptFr = "Se souvenir de l'escargot de Saigon - le nom dit tout. Vous vous souviendrez de la saveur ici pour toujours.", TtsScriptDe = "Erinnere dich an Saigon Snail - der Name sagt alles. Sie werden den Geschmack hier fÃ¼r immer in Erinnerung behalten.", TtsScriptZh = "è®°ä½è¥¿è´¡èœ—ç‰›â€”â€”è¿™ä¸ªåå­—è¯´æ˜Žäº†ä¸€åˆ‡ã€‚ ä½ ä¼šæ°¸è¿œè®°ä½è¿™é‡Œçš„å‘³é“ã€‚", TtsScriptJa = "ã‚µã‚¤ã‚´ãƒ³ã‚«ã‚¿ãƒ„ãƒ ãƒªã‚’æ€ã„å‡ºã™-ãã®åå‰ãŒã™ã¹ã¦ã‚’ç‰©èªžã£ã¦ã„ã¾ã™ã€‚ã“ã“ã§ã®å‘³ã‚’æ°¸é ã«æ€ã„å‡ºã™ã§ã—ã‚‡ã†ã€‚", TtsScriptKo = "ì‚¬ì´ê³µ ë‹¬íŒ½ì´ ê¸°ì–µ - ì´ë¦„ì´ ëª¨ë“  ê²ƒì„ ë§í•´ì¤ë‹ˆë‹¤. ì´ê³³ì˜ ë§›ì„ ì˜ì›ížˆ ê¸°ì–µí•  ê²ƒìž…ë‹ˆë‹¤.", TtsScriptRu = "Ð’ÑÐ¿Ð¾Ð¼Ð½Ð¸Ñ‚Ðµ Ð¡Ð°Ð¹Ð³Ð¾Ð½ÑÐºÑƒÑŽ ÑƒÐ»Ð¸Ñ‚ÐºÑƒ - Ð½Ð°Ð·Ð²Ð°Ð½Ð¸Ðµ Ð³Ð¾Ð²Ð¾Ñ€Ð¸Ñ‚ ÑÐ°Ð¼Ð¾ Ð·Ð° ÑÐµÐ±Ñ. Ð’Ñ‹ Ð·Ð°Ð¿Ð¾Ð¼Ð½Ð¸Ñ‚Ðµ Ð²ÐºÑƒÑ Ð·Ð´ÐµÑÑŒ Ð½Ð°Ð²ÑÐµÐ³Ð´Ð°.", TtsScriptIt = "Ricorda Saigon Snail - il nome dice tutto. Ricorderai per sempre il sapore qui.", TtsScriptPt = "Lembre-se do Saigon Snail - o nome diz tudo. VocÃª vai se lembrar do sabor aqui para sempre.",
                    Priority = 1,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 7,
                    Name = "BÃ¡nh Flan Ngá»c Nga - 167 VÄ©nh KhÃ¡nh",
                    NameEn = "Ngoc Nga Flan - 167 Vinh Khanh",
                    NameEs = "Ngoc Nga Flan - 167 Vinh Khanh", NameFr = "Flan Ngoc Nga - 167 Vinh Khanh", NameDe = "Ngoc Nga Flan - 167 Vinh Khanh", NameZh = "çŽ‰å¨¥å¸ƒä¸ - 167 æ°¸åº†", NameJa = "ã‚´ãƒƒã‚¯ãƒ»ãƒ³ã‚¬ ãƒ•ãƒ©ãƒ³ - 167 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ì‘ì˜¥ì‘ì•„ í”Œëž€ - 167 ë¹ˆì¹¸", NameRu = "Ð¤Ð»Ð°Ð½ ÐÐ³Ð¾Ðº ÐÐ³Ð° - 167 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Ngoc Nga Flan - 167 Vinh Khanh", NamePt = "Flan Ngoc Nga - 167 Vinh Khanh",
                    Description = "BÃ¡nh flan bÃ©o má»‹n tan ngay Ä‘áº§u lÆ°á»¡i, nÆ°á»›c caramel Ä‘áº­m Ä‘Ã . MÃ³n trÃ¡ng miá»‡ng pháº£i thá»­ sau á»‘c.",
                    DescriptionEn = "Creamy melt-in-mouth flan, rich caramel. A must-try dessert after snails.",
                    DescriptionEs = "Flan cremoso que se derrite en la boca, caramelo rico. Un postre que debes probar despuÃ©s de los caracoles.", DescriptionFr = "Flan crÃ©meux fondant en bouche, caramel riche. Un dessert Ã  ne pas manquer aprÃ¨s les escargots.", DescriptionDe = "Cremiger zartschmelzender Flan, reichhaltiges Karamell. Ein Muss nach Schnecken.", DescriptionZh = "å…¥å£å³åŒ–çš„å¥¶æ²¹å¸ƒä¸ï¼Œæµ“éƒçš„ç„¦ caramelã€‚åƒå®Œèœ—ç‰›åŽå¿…è¯•çš„ç”œç‚¹ã€‚", DescriptionJa = "å£ã®ä¸­ã§ã¨ã‚ã‘ã‚‹ã‚¯ãƒªãƒ¼ãƒŸãƒ¼ãªãƒ•ãƒ©ãƒ³ã€ãƒªãƒƒãƒãªã‚­ãƒ£ãƒ©ãƒ¡ãƒ«ã€‚ã‚«ã‚¿ãƒ„ãƒ ãƒªã®å¾Œã«ãœã²è©¦ã—ã¦ã„ãŸã ããŸã„ãƒ‡ã‚¶ãƒ¼ãƒˆã§ã™ã€‚", DescriptionKo = "ìž…ì•ˆì—ì„œ ì‚¬ë¥´ë¥´ ë…¹ëŠ” í¬ë¦¬ë¯¸í•œ í”Œëž€, í’ë¶€í•œ ìºëŸ¬ë©œ. ë‹¬íŒ½ì´ë¥¼ ë¨¹ì€ í›„ ê¼­ ë¨¹ì–´ë´ì•¼ í•  ë””ì €íŠ¸ìž…ë‹ˆë‹¤.", DescriptionRu = "Ð¡Ð»Ð¸Ð²Ð¾Ñ‡Ð½Ñ‹Ð¹, Ñ‚Ð°ÑŽÑ‰Ð¸Ð¹ Ð²Ð¾ Ñ€Ñ‚Ñƒ Ñ„Ð»Ð°Ð½, Ð³ÑƒÑÑ‚Ð°Ñ ÐºÐ°Ñ€Ð°Ð¼ÐµÐ»ÑŒ. ÐžÐ±ÑÐ·Ð°Ñ‚ÐµÐ»ÑŒÐ½Ñ‹Ð¹ Ð´ÐµÑÐµÑ€Ñ‚ Ð¿Ð¾ÑÐ»Ðµ ÑƒÐ»Ð¸Ñ‚Ð¾Ðº.", DescriptionIt = "Flan cremoso che si scioglie in bocca, caramello ricco. Un dessert da provare assolutamente dopo le lumache.", DescriptionPt = "Pudim cremoso que derrete na boca, caramelo rico. Uma sobremesa imperdÃ­vel apÃ³s os caracÃ³is.",
                    Latitude = 10.76232,
                    Longitude = 106.70316,
                    Radius = 30,
                    TtsScript = "Nghá»‰ chÃ¢n Äƒn bÃ¡nh flan Ngá»c Nga nhÃ©. BÃ©o má»‹n, ngá»t vá»«a, tuyá»‡t vá»i sau má»™t bá»¯a á»‘c.",
                    TtsScriptEn = "Take a break with Ngoc Nga flan. Creamy, perfectly sweet, wonderful after a snail feast.",
                    TtsScriptEs = "Tome un descanso con el flan de Ngoc Nga. Cremoso, perfectamente dulce, maravilloso despuÃ©s de un festÃ­n de caracoles.", TtsScriptFr = "Faites une pause avec le flan de Ngoc Nga. CrÃ©meux, parfaitement sucrÃ©, merveilleux aprÃ¨s un festin d'escargots.", TtsScriptDe = "Machen Sie eine Pause mit Ngoc Nga Flan. Cremig, perfekt sÃ¼ÃŸ, wunderbar nach einem Schneckenfest.", TtsScriptZh = "åƒä¸€å£çŽ‰å¨¥å¸ƒä¸ä¼‘æ¯ä¸€ä¸‹ã€‚å¥¶æ²¹å‘³ï¼Œæ°åˆ°å¥½å¤„çš„ç”œåº¦ï¼Œåœ¨èœ—ç‰›ç››å®´åŽå¾ˆæ£’ã€‚", TtsScriptJa = "ã‚´ãƒƒã‚¯ãƒ»ãƒ³ã‚¬ã®ãƒ•ãƒ©ãƒ³ã§ä¼‘æ†©ã—ã¾ã—ã‚‡ã†ã€‚ã‚¯ãƒªãƒ¼ãƒŸãƒ¼ã§å®Œç’§ãªç”˜ã•ã€ã‚«ã‚¿ãƒ„ãƒ ãƒªã®é¥—å®´ã®å¾Œã«ç´ æ™´ã‚‰ã—ã„ã€‚", TtsScriptKo = "ì‘ì˜¥ì‘ì•„ í”Œëž€ê³¼ í•¨ê»˜ íœ´ì‹ì„ ì·¨í•˜ì„¸ìš”. í¬ë¦¬ë¯¸í•˜ê³  ì™„ë²½í•˜ê²Œ ë‹¬ì½¤í•˜ë©° ë‹¬íŒ½ì´ ì¶•ì œ í›„ì— í›Œë¥­í•©ë‹ˆë‹¤.", TtsScriptRu = "Ð¡Ð´ÐµÐ»Ð°Ð¹Ñ‚Ðµ Ð¿ÐµÑ€ÐµÑ€Ñ‹Ð² Ñ Ñ„Ð»Ð°Ð½Ð¾Ð¼ ÐÐ³Ð¾Ðº ÐÐ³Ð°. ÐšÑ€ÐµÐ¼Ð¾Ð²Ñ‹Ð¹, Ð¸Ð´ÐµÐ°Ð»ÑŒÐ½Ð¾ ÑÐ»Ð°Ð´ÐºÐ¸Ð¹, Ð¿Ñ€ÐµÐºÑ€Ð°ÑÐ½Ñ‹Ð¹ Ð¿Ð¾ÑÐ»Ðµ Ð¿Ð¸Ñ€ÑˆÐµÑÑ‚Ð²Ð° Ñ ÑƒÐ»Ð¸Ñ‚ÐºÐ°Ð¼Ð¸.", TtsScriptIt = "Fai una pausa con il flan di Ngoc Nga. Cremoso, perfettamente dolce, meraviglioso dopo un banchetto di lumache.", TtsScriptPt = "FaÃ§a uma pausa com o pudim de Ngoc Nga. Cremoso, perfeitamente doce, maravilhoso depois de um banquete de caracÃ³is.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 8,
                    Name = "á»c Su 20k - 225 VÄ©nh KhÃ¡nh",
                    NameEn = "Su Snail 20k - 225 Vinh Khanh",
                    NameEs = "Su Snail 20k - 225 Vinh Khanh", NameFr = "Escargot Su 20k - 225 Vinh Khanh", NameDe = "Su Schnecke 20k - 225 Vinh Khanh", NameZh = "è‹èœ—ç‰› 20k - 225 æ°¸åº†", NameJa = "ã‚¹ãƒ¼ ã‚«ã‚¿ãƒ„ãƒ ãƒª 20k - 225 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ìˆ˜ ë‹¬íŒ½ì´ 20k - 225 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð¡Ñƒ 20k - 225 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Su 20k - 225 Vinh Khanh", NamePt = "Caracol Su 20k - 225 Vinh Khanh",
                    Description = "á»c Ä‘á»“ng giÃ¡ 20k cá»±c ráº», phÃ¹ há»£p cho sinh viÃªn muá»‘n Äƒn nhiá»u mÃ³n vá»›i chi phÃ­ tháº¥p.",
                    DescriptionEn = "Super cheap 20k flat-rate snails, perfect for students wanting variety on a budget.",
                    DescriptionEs = "Caracoles sÃºper baratos con tarifa plana de 20k, perfectos para estudiantes que desean variedad con un presupuesto ajustado.", DescriptionFr = "Des escargots trÃ¨s bon marchÃ© avec un tarif forfaitaire de 20k, parfaits pour les Ã©tudiants souhaitant de la variÃ©tÃ© avec un budget limitÃ©.", DescriptionDe = "Super gÃ¼nstige 20k Flatrate-Schnecken, perfekt fÃ¼r Studenten, die Abwechslung zu einem bestimmten Budget wÃ¼nschen.", DescriptionZh = "è¶…çº§ä¾¿å®œçš„20kå›ºå®šä»·èœ—ç‰›ï¼Œéžå¸¸é€‚åˆé¢„ç®—æœ‰é™ã€å¸Œæœ›å£å‘³å¤šæ ·çš„å­¦ç”Ÿã€‚", DescriptionJa = "è¶…æ ¼å®‰ã®20kå‡ä¸€ã‚«ã‚¿ãƒ„ãƒ ãƒªã€‚äºˆç®—ã«é™ã‚ŠãŒã‚ã‚Šã€ãƒãƒ©ã‚¨ãƒ†ã‚£ã‚’æ¥½ã—ã¿ãŸã„å­¦ç”Ÿã«æœ€é©ã§ã™ã€‚", DescriptionKo = "ë§¤ìš° ì €ë ´í•œ 20k í”Œëž« ìš”ê¸ˆ ë‹¬íŒ½ì´, ì˜ˆì‚°ìœ¼ë¡œ ë‹¤ì–‘ì„±ì„ ì›í•˜ëŠ” í•™ìƒë“¤ì—ê²Œ ì í•©í•©ë‹ˆë‹¤.", DescriptionRu = "Ð¡ÑƒÐ¿ÐµÑ€ Ð´ÐµÑˆÐµÐ²Ñ‹Ðµ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸ Ð¿Ð¾ Ñ„Ð¸ÐºÑÐ¸Ñ€Ð¾Ð²Ð°Ð½Ð½Ð¾Ð¹ ÑÑ‚Ð°Ð²ÐºÐµ 20k, Ð¸Ð´ÐµÐ°Ð»ÑŒÐ½Ð¾ Ð¿Ð¾Ð´Ñ…Ð¾Ð´ÑÑ‰Ð¸Ðµ Ð´Ð»Ñ ÑÑ‚ÑƒÐ´ÐµÐ½Ñ‚Ð¾Ð², Ð¶ÐµÐ»Ð°ÑŽÑ‰Ð¸Ñ… Ñ€Ð°Ð·Ð½Ð¾Ð¾Ð±Ñ€Ð°Ð·Ð¸Ñ Ð¿Ñ€Ð¸ Ð¾Ð³Ñ€Ð°Ð½Ð¸Ñ‡ÐµÐ½Ð½Ð¾Ð¼ Ð±ÑŽÐ´Ð¶ÐµÑ‚Ðµ.", DescriptionIt = "Lumache super economiche a tariffa fissa da 20k, perfette per studenti che desiderano varietÃ  a basso costo.", DescriptionPt = "CaracÃ³is super baratos com taxa fixa de 20k, perfeitos para estudantes que desejam variedade em um orÃ§amento.",
                    Latitude = 10.76056,
                    Longitude = 106.70396,
                    Radius = 30,
                    TtsScript = "á»c Su 20k, Ä‘á»“ng giÃ¡ cá»±c ká»³ háº¥p dáº«n. Ä‚n bao nhiÃªu cÅ©ng khÃ´ng sá»£ chÃ¡y tÃºi!",
                    TtsScriptEn = "Su Snail 20k, an incredibly attractive flat rate. Eat as much as you want without going broke!",
                    TtsScriptEs = "Su Snail 20k, una tarifa plana increÃ­blemente atractiva. Â¡Come todo lo que quieras sin arruinarte!", TtsScriptFr = "Escargot Su 20k, un tarif forfaitaire incroyablement attractif. Mangez autant que vous le souhaitez sans vous ruiner !", TtsScriptDe = "Su Schnecke 20k, eine unglaublich attraktive Flatrate. Essen Sie so viel Sie wollen, ohne pleite zu gehen!", TtsScriptZh = "è‹èœ—ç‰›20kï¼Œæžå…·å¸å¼•åŠ›çš„ç»Ÿä¸€è´¹çŽ‡ã€‚ å°½æƒ…äº«ç”¨ï¼Œæ— éœ€æ‹…å¿ƒç ´äº§ï¼", TtsScriptJa = "é©šãã»ã©é­…åŠ›çš„ãªå‡ä¸€æ–™é‡‘ã®ã‚¹ãƒ¼ã‚«ã‚¿ãƒ„ãƒ ãƒª20kã€‚ç ´ç”£ã™ã‚‹ã“ã¨ãªãå¥½ããªã ã‘é£Ÿã¹ã¦ãã ã•ã„ï¼", TtsScriptKo = "ì—„ì²­ë‚˜ê²Œ ë§¤ë ¥ì ì¸ ì •ì•¡ì œì˜ ìˆ˜ ë‹¬íŒ½ì´ 20k. íŒŒì‚° ê±±ì • ì—†ì´ ë§ˆìŒê» ë“œì„¸ìš”!", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð¡Ñƒ 20k, Ð½ÐµÐ²ÐµÑ€Ð¾ÑÑ‚Ð½Ð¾ Ð¿Ñ€Ð¸Ð²Ð»ÐµÐºÐ°Ñ‚ÐµÐ»ÑŒÐ½Ð°Ñ Ñ„Ð¸ÐºÑÐ¸Ñ€Ð¾Ð²Ð°Ð½Ð½Ð°Ñ ÑÑ‚Ð°Ð²ÐºÐ°. Ð•ÑˆÑŒÑ‚Ðµ ÑÐºÐ¾Ð»ÑŒÐºÐ¾ Ñ…Ð¾Ñ‚Ð¸Ñ‚Ðµ, Ð½Ðµ Ñ€Ð°Ð·Ð¾Ñ€ÑÑÑÑŒ!", TtsScriptIt = "Lumaca Su 20k, una tariffa fissa incredibilmente attraente. Mangia quanto vuoi senza andare in rovina!", TtsScriptPt = "Caracol Su 20k, uma taxa fixa incrivelmente atraente. Coma o quanto quiser sem falir!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 9,
                    Name = "á»c Nhi 20k - 262 VÄ©nh KhÃ¡nh",
                    NameEn = "Nhi Snail 20k - 262 Vinh Khanh",
                    NameEs = "Nhi Snail 20k - 262 Vinh Khanh", NameFr = "Escargot Nhi 20k - 262 Vinh Khanh", NameDe = "Nhi Schnecke 20k - 262 Vinh Khanh", NameZh = "å„¿èœ—ç‰› 20k - 262 æ°¸åº†", NameJa = "ãƒ‹ãƒ¼ ã‚«ã‚¿ãƒ„ãƒ ãƒª 20k - 262 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ë‹ˆ ë‹¬íŒ½ì´ 20k - 262 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° ÐÑ…Ð¸ 20k - 262 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Nhi 20k - 262 Vinh Khanh", NamePt = "Caracol Nhi 20k - 262 Vinh Khanh",
                    Description = "ThÃªm má»™t lá»±a chá»n Ä‘á»“ng giÃ¡ 20k. Thá»±c Ä‘Æ¡n Ä‘a dáº¡ng, phá»¥c vá»¥ nhanh nháº¹n.",
                    DescriptionEn = "Another 20k flat-rate choice. Diverse menu, fast service.",
                    DescriptionEs = "Otra opciÃ³n con tarifa plana de 20k. MenÃº variado, servicio rÃ¡pido.", DescriptionFr = "Un autre choix de tarif forfaitaire de 20 000. Menu variÃ©, service rapide.", DescriptionDe = "Eine weitere Wahl mit einer Flatrate von 20.000. VielfÃ¤ltige Speisekarte, schneller Service.", DescriptionZh = "å¦ä¸€ä¸ª20kå›ºå®šä»·çš„é€‰æ‹©ã€‚èœè‰²å¤šæ ·ï¼ŒæœåŠ¡å¿«æ·ã€‚", DescriptionJa = "ã‚‚ã†ä¸€ã¤ã®20kå®šé¡ã®é¸æŠžè‚¢ã€‚å¤šæ§˜ãªãƒ¡ãƒ‹ãƒ¥ãƒ¼ã€è¿…é€Ÿãªã‚µãƒ¼ãƒ“ã‚¹ã€‚", DescriptionKo = "ë˜ ë‹¤ë¥¸ 20k ì •ì•¡ì œ ì„ íƒ. ë‹¤ì–‘í•œ ë©”ë‰´, ë¹ ë¥¸ ì„œë¹„ìŠ¤.", DescriptionRu = "Ð•Ñ‰Ðµ Ð¾Ð´Ð¸Ð½ Ð²Ñ‹Ð±Ð¾Ñ€ Ñ Ñ„Ð¸ÐºÑÐ¸Ñ€Ð¾Ð²Ð°Ð½Ð½Ð¾Ð¹ ÑÑ‚Ð°Ð²ÐºÐ¾Ð¹ 20k. Ð Ð°Ð·Ð½Ð¾Ð¾Ð±Ñ€Ð°Ð·Ð½Ð¾Ðµ Ð¼ÐµÐ½ÑŽ, Ð±Ñ‹ÑÑ‚Ñ€Ð¾Ðµ Ð¾Ð±ÑÐ»ÑƒÐ¶Ð¸Ð²Ð°Ð½Ð¸Ðµ.", DescriptionIt = "Un'altra scelta a tariffa fissa di 20k. Menu vario, servizio veloce.", DescriptionPt = "Outra escolha de taxa fixa de 20k. Menu diversificado, atendimento rÃ¡pido.",
                    Latitude = 10.76128,
                    Longitude = 106.70597,
                    Radius = 30,
                    TtsScript = "á»c Nhi 20k táº¡i sá»‘ 262. Äá»“ng giÃ¡ cá»±c má»m mÃ  hÆ°Æ¡ng vá»‹ thÃ¬ khÃ´ng há» tá»‡.",
                    TtsScriptEn = "Nhi Snail 20k at number 262. Very cheap flat rate, but the flavor is not bad at all.",
                    TtsScriptEs = "Nhi Snail 20k en el nÃºmero 262. Tarifa plana muy barata, pero el sabor no estÃ¡ nada mal.", TtsScriptFr = "Escargot Nhi 20k au numÃ©ro 262. Forfait trÃ¨s bon marchÃ©, mais la saveur n'est pas mauvaise du tout.", TtsScriptDe = "Nhi Schnecke 20k an der Nummer 262. Sehr gÃ¼nstige Flatrate, aber der Geschmack ist Ã¼berhaupt nicht schlecht.", TtsScriptZh = "åœ¨262å·çš„å„¿èœ—ç‰›20kã€‚å¾ˆä¾¿å®œçš„ç»Ÿä¸€å®šä»·ï¼Œä½†å‘³é“ä¸€ç‚¹éƒ½ä¸å·®ã€‚", TtsScriptJa = "262ç•ªã®ãƒ‹ãƒ¼ã‚«ã‚¿ãƒ„ãƒ ãƒª20kã€‚éžå¸¸ã«å®‰ã„å‡ä¸€æ–™é‡‘ã§ã™ãŒã€å‘³ã¯ã¾ã£ãŸãæ‚ªãã‚ã‚Šã¾ã›ã‚“ã€‚", TtsScriptKo = "262ë²ˆì˜ ë‹ˆ ë‹¬íŒ½ì´ 20k. ë§¤ìš° ì €ë ´í•œ ì •ì•¡ ìš”ì›ì´ì§€ë§Œ ë§›ì€ ì „í˜€ ë‚˜ì˜ì§€ ì•ŠìŠµë‹ˆë‹¤.", TtsScriptRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° ÐÑ…Ð¸ 20k Ð¿Ð¾Ð´ Ð½Ð¾Ð¼ÐµÑ€Ð¾Ð¼ 262. ÐžÑ‡ÐµÐ½ÑŒ Ð´ÐµÑˆÐµÐ²Ð°Ñ Ñ„Ð¸ÐºÑÐ¸Ñ€Ð¾Ð²Ð°Ð½Ð½Ð°Ñ ÑÑ‚Ð°Ð²ÐºÐ°, Ð½Ð¾ Ð²ÐºÑƒÑ ÑÐ¾Ð²ÑÐµÐ¼ Ð½Ðµ Ð¿Ð»Ð¾Ñ…Ð¾Ð¹.", TtsScriptIt = "Lumaca Nhi 20k al numero 262. Tariffa fissa molto economica, ma il sapore non Ã¨ affatto male.", TtsScriptPt = "Nhi Snail 20k no nÃºmero 262. Taxa fixa muito barata, mas o sabor nÃ£o Ã© nada mau.",
                    Priority = 4,
                    ImageUrl = "oc_dao_vinh_khanh_1773306598631.png"
                },
                new Poi
                {
                    Id = 10,
                    Name = "QuÃ¡n á»c Tháº£o - 383 VÄ©nh KhÃ¡nh",
                    NameEn = "Thao Snail - 383 Vinh Khanh",
                    NameEs = "Thao Snail - 383 Vinh Khanh", NameFr = "Escargot Thao - 383 Vinh Khanh", NameDe = "Thao Schnecke - 383 Vinh Khanh", NameZh = "è‰èœ—ç‰› - 383 æ°¸åº†", NameJa = "ã‚¿ã‚ª ã‚«ã‚¿ãƒ„ãƒ ãƒª - 383 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "íƒ€ì˜¤ ë‹¬íŒ½ì´ - 383 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð¢Ð°Ð¾ - 383 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Thao - 383 Vinh Khanh", NamePt = "Caracol Thao - 383 Vinh Khanh",
                    Description = "Má»™t trong nhá»¯ng quÃ¡n Ä‘Ã´ng khÃ¡ch nháº¥t phá»‘. GiÃ¡ cáº£ pháº£i chÄƒng, á»‘c tÆ°Æ¡i ngon.",
                    DescriptionEn = "One of the most crowded stalls on the street. Affordable prices, fresh snails.",
                    DescriptionEs = "Uno de los puestos de mayor afluencia de la calle. Precios asequibles, caracoles frescos.", DescriptionFr = "Un des stands les plus frÃ©quentÃ©s de la rue. Prix â€‹â€‹abordables, escargots frais.", DescriptionDe = "Einer der am meisten besuchten StÃ¤nde auf der StraÃŸe. Erschwingliche Preise, frische Schnecken.", DescriptionZh = "è¿™æ¡è¡—ä¸Šæœ€æ‹¥æŒ¤çš„æ‘Šä½ä¹‹ä¸€ã€‚ä»·æ ¼å®žæƒ ï¼Œèœ—ç‰›æ–°é²œã€‚", DescriptionJa = "é€šã‚Šã§æœ€ã‚‚æ··é›‘ã—ã¦ã„ã‚‹å±‹å°ã®ä¸€ã¤ã€‚æ‰‹é ƒãªä¾¡æ ¼ã€æ–°é®®ãªã‚«ã‚¿ãƒ„ãƒ ãƒªã€‚", DescriptionKo = "ê±°ë¦¬ì—ì„œ ê°€ìž¥ ë¶ë¹„ëŠ” ë…¸ì  ì¤‘ í•˜ë‚˜. ì €ë ´í•œ ê°€ê²©, ì‹ ì„ í•œ ë‹¬íŒ½ì´.", DescriptionRu = "ÐžÐ´Ð¸Ð½ Ð¸Ð· ÑÐ°Ð¼Ñ‹Ñ… Ð¾Ð¶Ð¸Ð²Ð»ÐµÐ½Ð½Ñ‹Ñ… ÐºÐ¸Ð¾ÑÐºÐ¾Ð² Ð½Ð° ÑƒÐ»Ð¸Ñ†Ðµ. Ð”Ð¾ÑÑ‚ÑƒÐ¿Ð½Ñ‹Ðµ Ñ†ÐµÐ½Ñ‹, ÑÐ²ÐµÐ¶Ð¸Ðµ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸.", DescriptionIt = "Una delle bancarelle piÃ¹ affollate della strada. Prezzi abbordabili, lumache fresche.", DescriptionPt = "Uma das bancas de maior afluÃªncia da rua. PreÃ§os acessÃ­veis, caracÃ³is frescos.",
                    Latitude = 10.76168,
                    Longitude = 106.70236,
                    Radius = 35,
                    TtsScript = "Gáº§n Ä‘áº¿n á»c Tháº£o rá»“i. Sá»‘ 383, quÃ¡n ráº¥t Ä‘Ã´ng khÃ¡ch, báº¡n pháº£i xáº¿p hÃ ng Ä‘áº¥y.",
                    TtsScriptEn = "Almost at Thao Snail. Number 383, very crowded, you might have to queue.",
                    TtsScriptEs = "Casi en Thao Snail. NÃºmero 383, muy concurrido, puede que tengas que hacer cola.", TtsScriptFr = "Presque Ã  Escargot Thao. NumÃ©ro 383, trÃ¨s bondÃ©, vous devrez peut-Ãªtre faire la queue.", TtsScriptDe = "Fast bei der Thao Schnecke. Nummer 383, sehr voll, vielleicht mÃ¼ssen Sie anstehen.", TtsScriptZh = "å¿«åˆ°è‰èœ—ç‰›äº†ã€‚383å·ï¼Œéžå¸¸æŒ¤ï¼Œä½ å¯èƒ½å¿…é¡»æŽ’é˜Ÿã€‚", TtsScriptJa = "ã‚‚ã†ã™ãã‚¿ã‚ªã‚«ã‚¿ãƒ„ãƒ ãƒªã§ã™ã€‚383ç•ªã€éžå¸¸ã«æ··é›‘ã—ã¦ã„ã‚‹ã®ã§ä¸¦ã¶å¿…è¦ãŒã‚ã‚‹ã‹ã‚‚ã—ã‚Œã¾ã›ã‚“ã€‚", TtsScriptKo = "íƒ€ì˜¤ ë‹¬íŒ½ì´ì— ê±°ì˜ ë‹¤ ì™”ìŠµë‹ˆë‹¤. 383ë²ˆ, ë§¤ìš° ë¶ë¹„ë¯€ë¡œ ì¤„ì„ ì„œì•¼ í•  ìˆ˜ë„ ìžˆìŠµë‹ˆë‹¤.", TtsScriptRu = "ÐŸÐ¾Ñ‡Ñ‚Ð¸ Ñƒ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸ Ð¢Ð°Ð¾. ÐÐ¾Ð¼ÐµÑ€ 383, Ð¾Ñ‡ÐµÐ½ÑŒ Ð¼Ð½Ð¾Ð³Ð¾Ð»ÑŽÐ´Ð½Ð¾, Ð²Ð¾Ð·Ð¼Ð¾Ð¶Ð½Ð¾, Ð¿Ñ€Ð¸Ð´ÐµÑ‚ÑÑ Ð¾Ñ‚ÑÑ‚Ð¾ÑÑ‚ÑŒ Ð² Ð¾Ñ‡ÐµÑ€ÐµÐ´Ð¸.", TtsScriptIt = "Quasi da Lumaca Thao. Numero 383, molto affollato, potresti dover fare la fila.", TtsScriptPt = "Quase na Thao Snail. NÃºmero 383, muito lotado, pode ser necessÃ¡rio fazer fila.",
                    Priority = 5,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 11,
                    Name = "Sushi CÃ´ BÃ´ng - 390 VÄ©nh KhÃ¡nh",
                    NameEn = "Co Bong Sushi - 390 Vinh Khanh",
                    NameEs = "Co Bong Sushi - 390 Vinh Khanh", NameFr = "Sushi Co Bong - 390 Vinh Khanh", NameDe = "Co Bong Sushi - 390 Vinh Khanh", NameZh = "Bongå§‘å¯¿å¸ - 390 æ°¸åº†", NameJa = "ã‚³ãƒ¼ãƒœãƒ³å¯¿å¸ - 390 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ì½”ë´‰ ìŠ¤ì‹œ - 390 ë¹ˆì¹¸", NameRu = "Ð¡ÑƒÑˆÐ¸ ÐšÐ¾ Ð‘Ð¾Ð½Ð³ - 390 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Sushi Co Bong - 390 Vinh Khanh", NamePt = "Sushi Co Bong - 390 Vinh Khanh",
                    Description = "Sushi Nháº­t Báº£n cháº¥t lÆ°á»£ng trÃªn vá»‰a hÃ¨ SÃ i GÃ²n. CÃ¡ há»“i tÆ°Æ¡i, cÆ¡m dáº»o, giÃ¡ má»m.",
                    DescriptionEn = "Quality Japanese sushi on Saigon sidewalk. Fresh salmon, sticky rice, soft prices.",
                    DescriptionEs = "Sushi japonÃ©s de calidad en la acera de SaigÃ³n. SalmÃ³n fresco, arroz pegajoso, precios suaves.", DescriptionFr = "Sushi japonais de qualitÃ© sur le trottoir de Saigon. Saumon frais, riz gluant, prix doux.", DescriptionDe = "Qualitativ hochwertiges japanisches Sushi auf dem BÃ¼rgersteig von Saigon. Frischer Lachs, klebriger Reis, sanfte Preise.", DescriptionZh = "è¥¿è´¡è¡—å¤´ä¼˜è´¨çš„æ—¥æœ¬å¯¿å¸ã€‚æ–°é²œé²‘é±¼ï¼Œç³¯ç±³é¥­ï¼Œä»·æ ¼æ¸©æŸ”ã€‚", DescriptionJa = "ã‚µã‚¤ã‚´ãƒ³ã®æ­©é“ã®é«˜å“è³ªãªæ—¥æœ¬ã®å¯¿å¸ã€‚æ–°é®®ãªé®­ã€ã‚‚ã¡ç±³ã€ã‚½ãƒ•ãƒˆãªä¾¡æ ¼ã€‚", DescriptionKo = "ì‚¬ì´ê³µ ê±°ë¦¬ì˜ ê³ í’ˆì§ˆ ì¼ë³¸ ì´ˆë°¥. ì‹ ì„ í•œ ì—°ì–´, ì°¹ìŒ€, ë¶€ë“œëŸ¬ìš´ ê°€ê²©.", DescriptionRu = "ÐšÐ°Ñ‡ÐµÑÑ‚Ð²ÐµÐ½Ð½Ñ‹Ðµ ÑÐ¿Ð¾Ð½ÑÐºÐ¸Ðµ ÑÑƒÑˆÐ¸ Ð½Ð° Ñ‚Ñ€Ð¾Ñ‚ÑƒÐ°Ñ€Ðµ Ð¡Ð°Ð¹Ð³Ð¾Ð½Ð°. Ð¡Ð²ÐµÐ¶Ð¸Ð¹ Ð»Ð¾ÑÐ¾ÑÑŒ, ÐºÐ»ÐµÐ¹ÐºÐ¸Ð¹ Ñ€Ð¸Ñ, Ð½Ð¸Ð·ÐºÐ¸Ðµ Ñ†ÐµÐ½Ñ‹.", DescriptionIt = "Sushi giapponese di qualitÃ  sul marciapiede di Saigon. Salmone fresco, riso appiccicoso, prezzi bassi.", DescriptionPt = "Sushi japonÃªs de qualidade na calÃ§ada de Saigon. SalmÃ£o fresco, arroz pegajoso, preÃ§os baixos.",
                    Latitude = 10.76166,
                    Longitude = 106.70240,
                    Radius = 30,
                    TtsScript = "Sushi CÃ´ BÃ´ng á»Ÿ ngay sá»‘ 390. ThÃªm má»™t lá»±a chá»n Nháº­t Báº£n tuyá»‡t vá»i trÃªn phá»‘ VÄ©nh KhÃ¡nh.",
                    TtsScriptEn = "Co Bong Sushi right at 390. Another great Japanese choice on Vinh Khanh street.",
                    TtsScriptEs = "Co Bong Sushi en 390. Otra gran opciÃ³n japonesa en la calle Vinh Khanh.", TtsScriptFr = "Le Sushi CÃ´ Bong juste au 390. Un autre excellent choix japonais dans la rue Vinh Khanh.", TtsScriptDe = "Co Bong Sushi direkt am 390. Eine weitere groÃŸartige japanische Wahl in der Vinh Khanh StraÃŸe.", TtsScriptZh = "Bongå§‘å¯¿å¸å°±åœ¨390å·ã€‚æ°¸åº†è¡—ä¸Šçš„åˆä¸€å¤§æ—¥æœ¬é€‰æ‹©ã€‚", TtsScriptJa = "390ã«ã‚ã‚‹ã‚³ãƒ¼ãƒœãƒ³å¯¿å¸ã€‚ãƒ´ã‚£ãƒ³ãƒ»ã‚«ã‚¤ãƒ³é€šã‚Šã«ã‚ã‚‹ã‚‚ã†ä¸€ã¤ã®ç´ æ™´ã‚‰ã—ã„æ—¥æœ¬ã®é¸æŠžè‚¢ã€‚", TtsScriptKo = "390ì— ìžˆëŠ” ì½”ë´‰ ìŠ¤ì‹œ. ë¹ˆ ì¹¸ ê±°ë¦¬ì˜ ë˜ ë‹¤ë¥¸ í›Œë¥­í•œ ì¼ë³¸ ì„ íƒ.", TtsScriptRu = "Ð¡ÑƒÑˆÐ¸ ÐšÐ¾ Ð‘Ð¾Ð½Ð³ Ð¿Ñ€ÑÐ¼Ð¾ Ð½Ð° 390. Ð•Ñ‰Ðµ Ð¾Ð´Ð¸Ð½ Ð¾Ñ‚Ð»Ð¸Ñ‡Ð½Ñ‹Ð¹ ÑÐ¿Ð¾Ð½ÑÐºÐ¸Ð¹ Ð²Ñ‹Ð±Ð¾Ñ€ Ð½Ð° ÑƒÐ»Ð¸Ñ†Ðµ Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ.", TtsScriptIt = "Sushi Co Bong proprio al 390. Un'altra ottima scelta giapponese sulla strada Vinh Khanh.", TtsScriptPt = "Sushi Co Bong no 390. Outra excelente escolha japonesa na rua Vinh Khanh.",
                    Priority = 1,
                    ImageUrl = "sushi_vinh_khanh_street_1773306642405.png"
                },
                new Poi
                {
                    Id = 12,
                    Name = "á»c ÄÃªm - 474 VÄ©nh KhÃ¡nh",
                    NameEn = "Night Snail - 474 Vinh Khanh",
                    NameEs = "Night Snail - 474 Vinh Khanh", NameFr = "Escargot de Nuit - 474 Vinh Khanh", NameDe = "Nachtschnecke - 474 Vinh Khanh", NameZh = "å¤œèœ—ç‰› - 474 æ°¸åº†", NameJa = "å¤œã®ã‚«ã‚¿ãƒ„ãƒ ãƒª - 474 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ë°¤ ë‹¬íŒ½ì´ - 474 ë¹ˆì¹¸", NameRu = "ÐÐ¾Ñ‡Ð½Ð°Ñ Ð£Ð»Ð¸Ñ‚ÐºÐ° - 474 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Notturna - 474 Vinh Khanh", NamePt = "Caracol Noturno - 474 Vinh Khanh",
                    Description = "ChuyÃªn phá»¥c vá»¥ buá»•i tá»‘i vÃ  Ä‘Ãªm khuya. á»c nÆ°á»›ng má»¡ hÃ nh vÃ  nghÃªu háº¥p sáº£ lÃ  Ä‘áº·c sáº£n.",
                    DescriptionEn = "Specializes in late night service. Scallion grilled snails and steamed clams are signatures.",
                    DescriptionEs = "Se especializa en servicio nocturno. Los caracoles a la parrilla y las almejas al vapor son los platos principales.", DescriptionFr = "SpÃ©cialisÃ© dans le service de fin de nuit. Les escargots grillÃ©s Ã  la ciboule et les palourdes cuites Ã  la vapeur en sont la marque.", DescriptionDe = "Spezialisiert auf Late-Night-Service. Gegrillte Schnecken mit FrÃ¼hlingszwiebeln und gedÃ¤mpfte Muscheln sind die Signaturen.", DescriptionZh = "ä¸“è¥æ·±å¤œæœåŠ¡ã€‚çƒ¤èœ—ç‰›å’Œæ¸…è’¸è›¤èœŠæ˜¯ç‰¹è‰²ã€‚", DescriptionJa = "æ·±å¤œå–¶æ¥­ã‚’å°‚é–€ã¨ã—ã¦ã„ã¾ã™ã€‚ãƒã‚®ç„¼ãã®ã‚«ã‚¿ãƒ„ãƒ ãƒªã¨è’¸ã—ã‚¢ã‚µãƒªãŒåç‰©ã§ã™ã€‚", DescriptionKo = "ì‹¬ì•¼ ì„œë¹„ìŠ¤ ì „ë¬¸. íŒŒ êµ¬ì´ ë‹¬íŒ½ì´ì™€ ì° ì¡°ê°œê°€ ì£¼ë ¥ ë©”ë‰´ìž…ë‹ˆë‹¤.", DescriptionRu = "Ð¡Ð¿ÐµÑ†Ð¸Ð°Ð»Ð¸Ð·Ð¸Ñ€ÑƒÐµÑ‚ÑÑ Ð½Ð° Ð½Ð¾Ñ‡Ð½Ð¾Ð¼ Ð¾Ð±ÑÐ»ÑƒÐ¶Ð¸Ð²Ð°Ð½Ð¸Ð¸. ÐŸÑ€Ð¸Ð³Ð¾Ñ‚Ð¾Ð²Ð»ÐµÐ½Ð½Ñ‹Ðµ Ð½Ð° Ð³Ñ€Ð¸Ð»Ðµ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸ Ñ Ð»ÑƒÐºÐ¾Ð¼ Ð¸ Ð¿Ñ€Ð¸Ð³Ð¾Ñ‚Ð¾Ð²Ð»ÐµÐ½Ð½Ñ‹Ðµ Ð½Ð° Ð¿Ð°Ñ€Ñƒ Ð¼Ð¾Ð»Ð»ÑŽÑÐºÐ¸ ÑÐ²Ð»ÑÑŽÑ‚ÑÑ Ð²Ð¸Ð·Ð¸Ñ‚Ð½Ð¾Ð¹ ÐºÐ°Ñ€Ñ‚Ð¾Ñ‡ÐºÐ¾Ð¹.", DescriptionIt = "Specializzato nel servizio a tarda notte. Le lumache alla griglia alla cipolla e le vongole al vapore sono le specialitÃ .", DescriptionPt = "Ã‰ especialista no serviÃ§o nocturno. Os caracÃ³is grelhados na cebola e as amÃªijoas no vapor sÃ£o as assinaturas.",
                    Latitude = 10.76050,
                    Longitude = 106.70413,
                    Radius = 30,
                    TtsScript = "á»c ÄÃªm VÄ©nh KhÃ¡nh sá»‘ 474. Trá»i tá»‘i rá»“i má»›i lÃ  lÃºc quÃ¡n nÃ y Ä‘Ã´ng nháº¥t.",
                    TtsScriptEn = "Night Snail Vinh Khanh number 474. It is only when it gets dark that this place is most crowded.",
                    TtsScriptEs = "Night Snail Vinh Khanh nÃºmero 474. Solo cuando oscurece, este lugar estÃ¡ mÃ¡s lleno.", TtsScriptFr = "Escargot de nuit Vinh Khanh numÃ©ro 474. Ce n'est qu'Ã  la tombÃ©e de la nuit que cet endroit est le plus bondÃ©.", TtsScriptDe = "Nachtschnecke Vinh Khanh Nummer 474. Erst wenn es dunkel wird, ist es hier am vollsten.", TtsScriptZh = "å¤œèœ—ç‰›æ°¸åº†474å·ã€‚ åªæœ‰å¤©é»‘æ—¶ï¼Œè¿™é‡Œæ‰æœ€æ‹¥æŒ¤ã€‚", TtsScriptJa = "å¤œã®ã‚«ã‚¿ãƒ„ãƒ ãƒªãƒ´ã‚£ãƒ³ãƒ»ã‚«ã‚¤ãƒ³474å·ã€‚ æš—ããªã‚‹ã¨ã€ã“ã“ãŒæœ€ã‚‚æ··é›‘ã—ã¾ã™ã€‚", TtsScriptKo = "ë°¤ ë‹¬íŒ½ì´ ë¹ˆ ì¹¸ 474ë²ˆ. ì–´ë‘ì›Œ ì ¸ì•¼ ì´ê³³ì´ ê°€ìž¥ ë¶ë¹•ë‹ˆë‹¤.", TtsScriptRu = "ÐÐ¾Ñ‡Ð½Ð°Ñ Ð£Ð»Ð¸Ñ‚ÐºÐ° Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ 474. Ð¢Ð¾Ð»ÑŒÐºÐ¾ ÐºÐ¾Ð³Ð´Ð° ÑÑ‚Ð°Ð½Ð¾Ð²Ð¸Ñ‚ÑÑ Ñ‚ÐµÐ¼Ð½Ð¾, Ð·Ð´ÐµÑÑŒ Ð±Ð¾Ð»ÑŒÑˆÐµ Ð²ÑÐµÐ³Ð¾ Ð»ÑŽÐ´ÐµÐ¹.", TtsScriptIt = "Lumaca notturna Vinh Khanh numero 474. Ãˆ solo quando fa buio che questo posto Ã¨ piÃ¹ affollato.", TtsScriptPt = "Night Snail Vinh Khanh nÃºmero 474. Ã‰ apenas quando escurece que este lugar estÃ¡ mais cheio.",
                    Priority = 2,
                    ImageUrl = "nuong_vinh_khanh_1773306623915.png"
                },
                new Poi
                {
                    Id = 13,
                    Name = "á»c Oanh - 534 VÄ©nh KhÃ¡nh",
                    NameEn = "Oanh Snail - 534 Vinh Khanh",
                    NameEs = "Oanh Snail - 534 Vinh Khanh", NameFr = "Escargot Oanh - 534 Vinh Khanh", NameDe = "Oanh Schnecke - 534 Vinh Khanh", NameZh = "èŽºèœ—ç‰› - 534 æ°¸åº†", NameJa = "ã‚ªã‚¢ãƒ³ ã‚«ã‚¿ãƒ„ãƒ ãƒª - 534 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "ì˜¤ì•ˆ ë‹¬íŒ½ì´ - 534 ë¹ˆì¹¸", NameRu = "Ð£Ð»Ð¸Ñ‚ÐºÐ° ÐžÐ°Ð½ÑŒ - 534 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Lumaca Oanh - 534 Vinh Khanh", NamePt = "Caracol Oanh - 534 Vinh Khanh",
                    Description = "QuÃ¡n á»‘c huyá»n thoáº¡i, sáº§m uáº¥t nháº¥t phá»‘. ÄÆ°á»£c Michelin Bib Gourmand giá»›i thiá»‡u. á»c hÆ°Æ¡ng rang muá»‘i á»›t lÃ  mÃ³n trá»© danh.",
                    DescriptionEn = "Legendary stall, busiest on the street. Michelin Bib Gourmand recommended. Famous chili salt snails.",
                    DescriptionEs = "Puesto legendario, el mÃ¡s concurrido de la calle. Recomendado por el premio Michelin Bib Gourmand. Famosos caracoles con salsa picante.", DescriptionFr = "Stand lÃ©gendaire, le plus animÃ© de la rue. RecommandÃ© par le label Michelin Bib Gourmand. Famoses escargots Ã  la sauce Ã©picÃ©e.", DescriptionDe = "LegendÃ¤rer Stand, der belebteste auf der StraÃŸe. Von Michelin Bib Gourmand empfohlen. BerÃ¼hmte Schnecken mit Chili-Salz.", DescriptionZh = "ä¼ è¯´ä¸­çš„æ‘Šä½ï¼Œè¿™æ˜¯è¿™æ¡è¡—ä¸Šæœ€ç¹å¿™çš„ã€‚ è¢«ç±³å…¶æž—å›´è„–ç¾Žé£ŸæŽ¨èã€‚ è‘—åçš„è¾£æ¤’ç›èžºã€‚", DescriptionJa = "é€šã‚Šã®ä¼èª¬çš„ãªå±‹å°ã€‚ ãƒŸã‚·ãƒ¥ãƒ©ãƒ³ã®ãƒ“ãƒ–ã‚°ãƒ«ãƒžãƒ³è³žã«æŽ¨è–¦ã•ã‚Œã¾ã—ãŸã€‚ æœ‰åãªãƒãƒªãƒ»ã‚½ãƒ«ãƒˆã‚«ã‚¿ãƒ„ãƒ ãƒªã€‚", DescriptionKo = "ê¸¸ê±°ë¦¬ì—ì„œ ê°€ìž¥ ë¶„ì£¼í•œ ì „ì„¤ì ì¸ í¬ìž¥ ë§ˆì°¨. ë¯¸ì‰ë¦° ë¹• êµ¬ë¥´ë§ ìˆ˜ìƒìžê°€ ì¶”ì²œí–ˆìŠµë‹ˆë‹¤. ìœ ëª…í•œ ì¹ ë¦¬ ì†Œê¸ˆ ë‹¬íŒ½ì´.", DescriptionRu = "Ð›ÐµÐ³ÐµÐ½Ð´Ð°Ñ€Ð½Ñ‹Ð¹ ÐºÐ¸Ð¾ÑÐº, ÑÐ°Ð¼Ñ‹Ð¹ Ð¾Ð¶Ð¸Ð²Ð»ÐµÐ½Ð½Ñ‹Ð¹ Ð½Ð° ÑƒÐ»Ð¸Ñ†Ðµ. Ð ÐµÐºÐ¾Ð¼ÐµÐ½Ð´Ð¾Ð²Ð°Ð½Ð¾ Michelin Bib Gourmand. Ð—Ð½Ð°Ð¼ÐµÐ½Ð¸Ñ‚Ñ‹Ðµ ÑƒÐ»Ð¸Ñ‚ÐºÐ¸ Ñ Ñ‡Ð¸Ð»Ð¸ Ð¸ ÑÐ¾Ð»ÑŒÑŽ.", DescriptionIt = "Bancarella leggendaria, la piÃ¹ affollata della strada. Consigliato Michelin Bib Gourmand. Famose lumache al sale e peperoncino.", DescriptionPt = "MÃ­tico bar, o mais movimentado da rua. Recomendado pelo Michelin Bib Gourmand. Famosos caracÃ³is com sal de pimenta.",
                    Latitude = 10.76072,
                    Longitude = 106.70330,
                    Radius = 40,
                    TtsScript = "ChÃ o má»«ng báº¡n Ä‘áº¿n vá»›i á»c Oanh - Ã´ng trÃ¹m á»‘c phá»‘ VÄ©nh KhÃ¡nh. Äá»«ng quÃªn thá»­ á»‘c hÆ°Æ¡ng rang muá»‘i á»›t trá»© danh nhÃ©!",
                    TtsScriptEn = "Welcome to Oanh Snail - the boss of Vinh Khanh street. Don't forget to try the famous chili salt roasted sweet snails!",
                    TtsScriptEs = "Bienvenido a Oanh Snail: el jefe de la calle Vinh Khanh. Â¡No olvides probar los famosos caracoles dulces asados â€‹â€‹con sal y ajÃ­!", TtsScriptFr = "Bienvenue Ã  Escargot Oanh, le chef de la rue Vinh Khanh. N'oubliez pas d'essayer les cÃ©lÃ¨bres escargots doux rÃ´tis au sel et au chiliÂ !", TtsScriptDe = "Willkommen bei der Oanh-Schnecke â€“ dem Chefkoch der Vinh-Khanh-StraÃŸe. Vergessen Sie nicht, die berÃ¼hmten, mit Chili-Salz gerÃ¶steten sÃ¼ÃŸen Schnecken zu probieren!", TtsScriptZh = "æ¬¢è¿Žæ¥åˆ°èŽºèœ—ç‰›-æ°¸åº†è¡—è€æ¿ã€‚ä¸è¦å¿˜äº†å°è¯•è‘—åçš„è¾£æ¤’ç›çƒ¤èœ—ç‰›ï¼", TtsScriptJa = "ãƒ´ã‚£ãƒ³ã‚«ãƒ³é€šã‚Šã®ãƒœã‚¹ã§ã‚ã‚‹ã‚ªã‚¢ãƒ³ã‚«ã‚¿ãƒ„ãƒ ãƒªã¸ã‚ˆã†ã“ãã€‚ æœ‰åãªãƒãƒªãƒ»ã‚½ãƒ«ãƒˆãƒ­ãƒ¼ã‚¹ãƒˆã‚«ã‚¿ãƒ„ãƒ ãƒªã‚’è©¦ã™ã®ã‚’å¿˜ã‚Œãªã„ã§ãã ã•ã„ï¼", TtsScriptKo = "ë¹ˆì¹¸ ê±°ë¦¬ì˜ ë³´ìŠ¤ì¸ ì˜¤ì•ˆ ë‹¬íŒ½ì´ì— ì˜¤ì‹  ê²ƒì„ í™˜ì˜í•©ë‹ˆë‹¤. ìœ ëª…í•œ ì¹ ë¦¬ ì†Œê¸ˆ êµ¬ì´ ë‹¬íŒ½ì´ë¥¼ ìžŠì§€ ë§ˆì„¸ìš”!", TtsScriptRu = "Ð”Ð¾Ð±Ñ€Ð¾ Ð¿Ð¾Ð¶Ð°Ð»Ð¾Ð²Ð°Ñ‚ÑŒ Ð² ÐºÐ¾Ð¼Ð¿Ð°Ð½Ð¸ÑŽ Ð£Ð»Ð¸Ñ‚ÐºÐ° ÐžÐ°Ð½ÑŒ - Ð±Ð¾ÑÑÐ° ÑƒÐ»Ð¸Ñ†Ñ‹ Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ. ÐÐµ Ð·Ð°Ð±ÑƒÐ´ÑŒÑ‚Ðµ Ð¿Ð¾Ð¿Ñ€Ð¾Ð±Ð¾Ð²Ð°Ñ‚ÑŒ Ð·Ð½Ð°Ð¼ÐµÐ½Ð¸Ñ‚Ñ‹Ñ… ÑƒÐ»Ð¸Ñ‚Ð¾Ðº, Ð·Ð°Ð¿ÐµÑ‡ÐµÐ½Ð½Ñ‹Ñ… Ñ Ñ‡Ð¸Ð»Ð¸ Ð¸ ÑÐ¾Ð»ÑŒÑŽ!", TtsScriptIt = "Benvenuti a Oanh Snail, il capo della via Vinh Khanh. Non dimenticate di provare le famose lumache dolci arrosto con sale e peperoncino!", TtsScriptPt = "Bem-vindo ao Oanh Snail - o chefe da rua Vinh Khanh. NÃ£o se esqueÃ§a de experimentar os famosos caracÃ³is doces assados â€‹â€‹â€‹â€‹com sal e pimenta!",
                    Priority = 3,
                    ImageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
                },
                new Poi
                {
                    Id = 14,
                    Name = "BÃºn NÆ°á»›c CÃ´ CÃ³ - 39 VÄ©nh KhÃ¡nh",
                    NameEn = "Co Co Noodle - 39 Vinh Khanh",
                    NameEs = "Co Co Noodle - 39 Vinh Khanh", NameFr = "Nouilles Co Co - 39 Vinh Khanh", NameDe = "Co Co Nudel - 39 Vinh Khanh", NameZh = "Co Co æ±¤ç²‰ - 39 æ°¸åº†", NameJa = "Co Co ãƒŒãƒ¼ãƒ‰ãƒ« - 39 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "Co Co ëˆ„ë“¤ - 39 ë¹ˆì¹¸", NameRu = "Ð›Ð°Ð¿ÑˆÐ° Co Co - 39 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Spaghetti Co Co - 39 Vinh Khanh", NamePt = "MacarrÃ£o Co Co - 39 Vinh Khanh",
                    Description = "BÃºn nÆ°á»›c lÃ¨o Ä‘áº­m Ä‘Ã  Ä‘áº·c sáº£n SÃ i GÃ²n ngá»t thanh, topping háº£i sáº£n siÃªu ngon.",
                    DescriptionEn = "Special Saigon sweet broth noodles, super delicious seafood toppings.",
                    DescriptionEs = "Fideos especiales en caldo dulce de SaigÃ³n, ingredientes sÃºper deliciosos con mariscos.", DescriptionFr = "Nouilles spÃ©ciales au bouillon sucrÃ© de Saigon, garnitures de fruits de mer super dÃ©licieuses.", DescriptionDe = "Spezielle Saigon-SÃ¼ÃŸbrÃ¼he-Nudeln, super leckere MeeresfrÃ¼chte-Toppings.", DescriptionZh = "è¥¿è´¡ç‰¹è‰²ç”œæ±¤ç²‰ï¼Œéžå¸¸ç¾Žå‘³çš„æµ·é²œæµ‡å¤´ã€‚", DescriptionJa = "ç‰¹è£½ã®ã‚µã‚¤ã‚´ãƒ³ç”˜é…’ãƒŒãƒ¼ãƒ‰ãƒ«ã€ã¨ã¦ã‚‚ç¾Žå‘³ã—ã„ã‚·ãƒ¼ãƒ•ãƒ¼ãƒ‰ã®ãƒˆãƒƒãƒ”ãƒ³ã‚°ã€‚", DescriptionKo = "íŠ¹ë³„í•œ ì‚¬ì´ê³µ ë‹¨ êµ­ë¬¼ ëˆ„ë“¤, ì•„ì£¼ ë§›ìžˆëŠ” í•´ì‚°ë¬¼ í† í•‘.", DescriptionRu = "Ð¡Ð¿ÐµÑ†Ð¸Ð°Ð»ÑŒÐ½Ð°Ñ ÑÐ°Ð¹Ð³Ð¾Ð½ÑÐºÐ°Ñ Ð»Ð°Ð¿ÑˆÐ° Ð½Ð° ÑÐ»Ð°Ð´ÐºÐ¾Ð¼ Ð±ÑƒÐ»ÑŒÐ¾Ð½Ðµ, Ð¾Ñ‡ÐµÐ½ÑŒ Ð²ÐºÑƒÑÐ½Ñ‹Ðµ Ñ‚Ð¾Ð¿Ð¿Ð¸Ð½Ð³Ð¸ Ð¸Ð· Ð¼Ð¾Ñ€ÐµÐ¿Ñ€Ð¾Ð´ÑƒÐºÑ‚Ð¾Ð².", DescriptionIt = "Speciali spaghetti al brodo dolce di Saigon, condimenti per frutti di mare deliziosi.", DescriptionPt = "MacarrÃ£o especial com caldo doce de Saigon, coberturas de frutos do mar super deliciosas.",
                    Latitude = 10.76210,
                    Longitude = 106.70280,
                    Radius = 30,
                    TtsScript = "HÃ£y thá»­ qua BÃºn NÆ°á»›c CÃ´ CÃ³ nhÃ©. NÆ°á»›c lÃ¨o Ä‘áº­m Ä‘Ã , Äƒn má»™t láº§n lÃ  nhá»› mÃ£i.",
                    TtsScriptEn = "Try Co Co Noodle. The broth is rich, once you eat it you will remember it forever.",
                    TtsScriptEs = "Prueba Co Co Noodle. El caldo es rico, una vez que lo comas lo recordarÃ¡s para siempre.", TtsScriptFr = "Essayez les nouilles Co Co. Le bouillon est riche, une fois que vous l'aurez mangÃ©, vous vous en souviendrez pour toujours.", TtsScriptDe = "Probieren Sie Co Co Noodle. Die BrÃ¼he ist reichhaltig, wenn Sie sie einmal gegessen haben, werden Sie sich ewig daran erinnern.", TtsScriptZh = "è¯•è¯• Co Co æ±¤ç²‰ã€‚ è‚‰æ±¤æµ“éƒï¼Œåƒè¿‡ä¸€æ¬¡å°±ä¼šæ°¸è¿œè®°ä½ã€‚", TtsScriptJa = "Co CoãƒŒãƒ¼ãƒ‰ãƒ«ã‚’ãŠè©¦ã—ãã ã•ã„ã€‚ ã‚¹ãƒ¼ãƒ—ãŒæ¿ƒåŽšã§ã€ä¸€åº¦é£Ÿã¹ãŸã‚‰å¿˜ã‚Œã‚‰ã‚Œãªã„å‘³ã§ã™ã€‚", TtsScriptKo = "Co Co ëˆ„ë“¤ì„ ë¨¹ì–´ë³´ì„¸ìš”. êµ­ë¬¼ì´ ì§„í•´ì„œ í•œ ë²ˆ ë¨¹ìœ¼ë©´ ì˜ì›ížˆ ê¸°ì–µë  ê²ƒìž…ë‹ˆë‹¤.", TtsScriptRu = "ÐŸÐ¾Ð¿Ñ€Ð¾Ð±ÑƒÐ¹Ñ‚Ðµ Ð»Ð°Ð¿ÑˆÑƒ Co Co. Ð‘ÑƒÐ»ÑŒÐ¾Ð½ Ð½Ð°Ð²Ð°Ñ€Ð¸ÑÑ‚Ñ‹Ð¹, Ð¾Ð´Ð¸Ð½ Ñ€Ð°Ð· Ð¿Ð¾Ð¿Ñ€Ð¾Ð±ÑƒÐµÑˆÑŒ - Ð·Ð°Ð¿Ð¾Ð¼Ð½Ð¸ÑˆÑŒ Ð½Ð°Ð²ÑÐµÐ³Ð´Ð°.", TtsScriptIt = "Prova Co Co Noodle. Il brodo Ã¨ ricco, una volta mangiato lo ricorderai per sempre.", TtsScriptPt = "Experimente o Co Co Noodle. O caldo Ã© rico, depois de comÃª-lo vocÃª vai se lembrar dele para sempre.",
                    Priority = 1,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                },
                new Poi
                {
                    Id = 15,
                    Name = "Láº©u BÃ² Khu Ba - 180 VÄ©nh KhÃ¡nh",
                    NameEn = "Zone 3 Beef Hotpot - 180 Vinh Khanh",
                    NameEs = "Zone 3 Beef Hotpot - 180 Vinh Khanh", NameFr = "Fondue de boeuf Zone 3 - 180 Vinh Khanh", NameDe = "Rindfleisch Hotpot Zone 3 - 180 Vinh Khanh", NameZh = "ä¸‰åŒºç‰›è‚‰ç«é”… - 180 æ°¸åº†", NameJa = "ã‚¾ãƒ¼ãƒ³3ãƒ“ãƒ¼ãƒ•ãƒ›ãƒƒãƒˆãƒãƒƒãƒˆ - 180 ãƒ´ã‚£ãƒ³ã‚«ãƒ³", NameKo = "êµ¬ì—­ 3 ì†Œê³ ê¸° ì „ê³¨ - 180 ë¹ˆì¹¸", NameRu = "Ð“Ð¾Ð²ÑÐ¶Ð¸Ð¹ Ð¥Ð¾Ñ‚-Ð¿Ð¾Ñ‚ Ð—Ð¾Ð½Ð° 3 - 180 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ", NameIt = "Hotpot di manzo Zona 3 - 180 Vinh Khanh", NamePt = "Hotpot de carne Zona 3 - 180 Vinh Khanh",
                    Description = "QuÃ¡n láº©u bÃ², bÃ² nÆ°á»›ng trá»© danh bÃ¬nh dÃ¢n Ä‘Ã´ng Ä‘Ãºc. Thá»‹t bÃ² tÆ°Æ¡i ngon má»m máº¡i.",
                    DescriptionEn = "Famous affordable beef hotpot and grilled beef stall. Fresh and tender beef.",
                    DescriptionEs = "Famoso puesto asequible de estofado de ternera y ternera a la parrilla. Carne fresca y tierna.", DescriptionFr = "CÃ©lÃ¨bre stand abordable de fondue de bÅ“uf et de bÅ“uf grillÃ©. BÅ“uf frais et tendre.", DescriptionDe = "BerÃ¼hmter erschwinglicher Rindfleisch-Hotpot und gegrillter Rindfleisch-Stand. Frisches und zartes Rindfleisch.", DescriptionZh = "è‘—åçš„ä»·æ ¼å®žæƒ çš„ç‰›è‚‰ç«é”…å’Œçƒ¤ç‰›è‚‰æ‘Šã€‚ç‰›è‚‰æ–°é²œå«©æ»‘ã€‚", DescriptionJa = "æœ‰åãªæ‰‹é ƒãªä¾¡æ ¼ã®ãƒ“ãƒ¼ãƒ•ãƒ›ãƒƒãƒˆãƒãƒƒãƒˆã¨ç„¼ãè‚‰å±‹å°ã€‚ æ–°é®®ã§æŸ”ã‚‰ã‹ã„ç‰›è‚‰ã€‚", DescriptionKo = "ìœ ëª…í•˜ê³  ì €ë ´í•œ ì†Œê³ ê¸° ì „ê³¨ ë° êµ¬ìš´ ì†Œê³ ê¸° ë…¸ì . ì‹ ì„ í•˜ê³  ë¶€ë“œëŸ¬ìš´ ì†Œê³ ê¸°.", DescriptionRu = "Ð—Ð½Ð°Ð¼ÐµÐ½Ð¸Ñ‚Ñ‹Ð¹ Ð½ÐµÐ´Ð¾Ñ€Ð¾Ð³Ð¾Ð¹ Ð³Ð¾Ñ€ÑÑ‡Ð¸Ð¹ ÐºÐ¾Ñ‚ÐµÐ»Ð¾Ðº Ñ Ð³Ð¾Ð²ÑÐ´Ð¸Ð½Ð¾Ð¹ Ð¸ ÐºÐ¸Ð¾ÑÐº Ñ Ð³Ð¾Ð²ÑÐ´Ð¸Ð½Ð¾Ð¹ Ð½Ð° Ð³Ñ€Ð¸Ð»Ðµ. Ð¡Ð²ÐµÐ¶Ð°Ñ Ð¸ Ð½ÐµÐ¶Ð½Ð°Ñ Ð³Ð¾Ð²ÑÐ´Ð¸Ð½Ð°.", DescriptionIt = "Famosa bancarella di manzo caldo e manzo alla griglia a prezzi accessibili. Manzo fresco e tenero.", DescriptionPt = "Famosa e acessÃ­vel churrasqueira e fondue de carne. Carne fresca e macia.",
                    Latitude = 10.76061,
                    Longitude = 106.70425,
                    Radius = 30,
                    TtsScript = "HÃ£y ghÃ© thá»­ Láº©u BÃ² Khu Ba cá»±c ká»³ Ä‘Ã´ng Ä‘Ãºc á»Ÿ sá»‘ 180 VÄ©nh KhÃ¡nh. Thá»‹t sÆ°á»n bÃ² nÆ°á»›ng tuyá»‡t cÃº mÃ¨o.",
                    TtsScriptEn = "Try the incredibly crowded Zone 3 Beef Hotpot at 180 Vinh Khanh. The grilled beef ribs are fantastic.",
                    TtsScriptEs = "Pruebe el estofado de ternera de la Zona 3 increÃ­blemente concurrido en 180 Vinh Khanh. Las costillas de ternera asadas son fantÃ¡sticas.", TtsScriptFr = "Essayez la fondue au boeuf de la zone 3 incroyablement bondÃ©e au 180 Vinh Khanh. Les cÃ´tes de bÅ“uf grillÃ©es sont fantastiques.", TtsScriptDe = "Probieren Sie den unglaublich Ã¼berfÃ¼llten Zone 3 Beef Hotpot bei 180 Vinh Khanh. Die gegrillten Rinderrippen sind fantastisch.", TtsScriptZh = "åœ¨180æ°¸åº†å°è¯•æ‹¥æŒ¤çš„ä¸‰åŒºç‰›è‚‰ç«é”…ã€‚ çƒ¤ç‰›æŽ’æ£’æžäº†ã€‚", TtsScriptJa = "180ãƒ´ã‚£ãƒ³ã‚«ãƒ³ã«ã‚ã‚‹ä¿¡ã˜ã‚‰ã‚Œãªã„ã»ã©æ··é›‘ã—ã¦ã„ã‚‹ã‚¾ãƒ¼ãƒ³3ãƒ“ãƒ¼ãƒ•ãƒ›ãƒƒãƒˆãƒãƒƒãƒˆã‚’ãŠè©¦ã—ãã ã•ã„ã€‚ ç„¼ãç‰›ã‚«ãƒ«ãƒ“ã¯ç´ æ™´ã‚‰ã—ã„ã§ã™ã€‚", TtsScriptKo = "180 ë¹ˆì¹¸ì—ì„œ ì—„ì²­ë‚˜ê²Œ ë¶ë¹„ëŠ” êµ¬ì—­ 3 ì†Œê³ ê¸° ì „ê³¨ì„ ì‚¬ìš©í•´ë³´ì‹­ì‹œì˜¤. êµ¬ìš´ ì†Œê°ˆë¹„ëŠ” í™˜ìƒì ìž…ë‹ˆë‹¤.", TtsScriptRu = "ÐŸÐ¾Ð¿Ñ€Ð¾Ð±ÑƒÐ¹Ñ‚Ðµ Ð½ÐµÐ²ÐµÑ€Ð¾ÑÑ‚Ð½Ð¾ Ð¼Ð½Ð¾Ð³Ð¾Ð»ÑŽÐ´Ð½Ñ‹Ð¹ Ð¥Ð¾Ñ‚-Ð¿Ð¾Ñ‚ Ð¸Ð· Ð³Ð¾Ð²ÑÐ´Ð¸Ð½Ñ‹ Ð—Ð¾Ð½Ñ‹ 3, Ð½Ð° ÑƒÐ»Ð¸Ñ†Ðµ 180 Ð’Ð¸Ð½ÑŒ ÐšÑ…Ð°Ð½ÑŒ. Ð“Ð¾Ð²ÑÐ¶ÑŒÐ¸ Ñ€ÐµÐ±Ñ€Ñ‹ÑˆÐºÐ¸ Ð½Ð° Ð³Ñ€Ð¸Ð»Ðµ Ð¿Ñ€Ð¾ÑÑ‚Ð¾ Ñ„Ð°Ð½Ñ‚Ð°ÑÑ‚Ð¸Ñ‡ÐµÑÐºÐ¸Ðµ.", TtsScriptIt = "Prova l'incredibilmente affollato Hotpot di manzo della Zona 3 in 180 Vinh Khanh. Le costolette di manzo alla griglia sono fantastiche.", TtsScriptPt = "Experimente o Hotpot de carne incrivelmente lotado da Zona 3 em 180 Vinh Khanh. As costelas de vaca grelhadas sÃ£o fantÃ¡sticas.",
                    Priority = 2,
                    ImageUrl = "lau_bo_vinh_khanh_1773306661104.png"
                }
            };
        }
    }
}
