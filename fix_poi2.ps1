$orig = [System.IO.File]::ReadAllText("Models\Poi.cs", [System.Text.Encoding]::UTF8)

# Replace namespace
$orig = $orig -replace "namespace VinhKhanhTour.Models", "using VinhKhanhTour.Shared.Services;`n`nnamespace VinhKhanhTour.Shared.Models"

# Add the localization support methods inside the class
$localizationMethods = @"
        public static ILocalizationService? LocalizationService { get; set; }

        private string GetLocalizedString(string key)
        {
            return LocalizationService?.GetString(key) ?? key;
        }

        private string GetCurrentLanguageCode()
        {
            return LocalizationService?.CurrentLanguageCode ?? `"en`";
        }
"@

$orig = $orig -replace "public partial class Poi : ObservableObject\s*\{", "public partial class Poi : ObservableObject`n    {`n$localizationMethods"

# Replace LocalizationResourceManager usages
$orig = $orig.Replace('Services.LocalizationResourceManager.Instance["Cách bạn"]', 'GetLocalizedString("Cách bạn")')
$orig = $orig.Replace('Services.LocalizationResourceManager.Instance["Đang định vị..."]', 'GetLocalizedString("Đang định vị...")')
$orig = $orig.Replace('Services.LocalizationResourceManager.Instance["Bán kính"]', 'GetLocalizedString("Bán kính")')
$orig = $orig.Replace('Services.LocalizationResourceManager.Instance[key]?.ToString() ?? key', 'GetLocalizedString(key)')
$orig = $orig.Replace('Services.LocalizationResourceManager.Instance.CurrentLanguageCode', 'GetCurrentLanguageCode()')

[System.IO.File]::WriteAllText("VinhKhanhTour.Shared\Models\Poi.cs", $orig, [System.Text.Encoding]::UTF8)
