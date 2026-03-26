using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string path = @"VinhKhanhTour.Shared\Models\Poi.cs";
        string content = File.ReadAllText(path, Encoding.UTF8);

        string searchStart = "public static List<Poi> GetSampleData()";
        int startIdx = content.IndexOf(searchStart);
        int mEnd = content.LastIndexOf("}", content.LastIndexOf("}") - 1); // Extract old method
        
        string newMethod = @"public static List<Poi> GetSampleData()
        {
            return new List<Poi>
            {
                new Poi
                {
                    Id = 1,
                    Name = ""Ốc Vũ - 37 Vĩnh Khánh"",
                    NameEn = ""Vu Snail - 37 Vinh Khanh"",
                    Description = ""Quán ốc bình dân nổi tiếng với các món xào bơ tỏi thơm lừng. Rất được lòng giới trẻ Sài Gòn."",
                    DescriptionEn = ""Popular snail stall famous for garlic butter stir-fry. Loved by young Saigonese."",
                    Latitude = 10.76140,
                    Longitude = 106.70270,
                    Radius = 30,
                    TtsScript = ""Ốc Vũ là điểm đến yêu thích của giới trẻ. Đừng bỏ qua món ốc móng tay xào bơ tỏi nhé."",
                    TtsScriptEn = ""Vu Snail is a favorite spot for young people. Don't miss the bamboo snails stir-fried with garlic butter."",
                    Priority = 1,
                    ImageUrl = ""oc_oanh_vinh_khanh_1773306578974.png""
                },
                new Poi
                {
                    Id = 2,
                    Name = ""Ốc Phát Quán - 46 Vĩnh Khánh"",
                    NameEn = ""Phat Snail - 46 Vinh Khanh"",
                    Description = ""Ốc len xào dừa và các món sa tế cay nồng là niềm tự hào của quán. Giá cả phải chăng."",
                    DescriptionEn = ""Coconut milk mud snails and spicy satay dishes are their pride. Affordable prices."",
                    Latitude = 10.76210,
                    Longitude = 106.70210,
                    Radius = 30,
                    TtsScript = ""Ốc Phát nổi bật với ốc len xào dừa béo ngậy. Món nướng mỡ hành cũng rất tuyệt."",
                    TtsScriptEn = ""Phat Snail stands out with creamy coconut mud snails. Their grilled dishes with scallion oil are also great."",
                    Priority = 3,
                    ImageUrl = ""oc_loan_vinh_khanh_1773306611397.png""
                },
                new Poi
                {
                    Id = 3,
                    Name = ""Lẩu Bò Khu Ba - 180 Vĩnh Khánh"",
                    NameEn = ""Zone 3 Beef Hotpot - 180 Vinh Khanh"",
                    Description = ""Quán lẩu bò, bò nướng trứ danh bình dân đông đúc. Thịt bò tươi ngon mềm mại."",
                    DescriptionEn = ""Famous affordable beef hotpot and grilled beef stall. Fresh and tender beef."",
                    Latitude = 10.76061,
                    Longitude = 106.70425,
                    Radius = 30,
                    TtsScript = ""Hãy ghé thử Lẩu Bò Khu Ba cực kỳ đông đúc ở số 180 Vĩnh Khánh. Thịt sườn bò nướng tuyệt cú mèo."",
                    TtsScriptEn = ""Try the incredibly crowded Zone 3 Beef Hotpot at 180 Vinh Khanh. The grilled beef ribs are fantastic."",
                    Priority = 2,
                    ImageUrl = ""lau_bo_vinh_khanh_1773306661104.png""
                }
            };
        }";

        string finalContent = content.Substring(0, startIdx) + newMethod + content.Substring(mEnd + 1);
        File.WriteAllText(path, finalContent, Encoding.UTF8);
        Console.WriteLine("Replaced GetSampleData with proper UTF-8 Vietnamese strings.");
    }
}
