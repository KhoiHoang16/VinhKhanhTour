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
$orig = $orig -replace 'Services\.LocalizationResourceManager\.Instance\["([^"]+)"\]\?\.ToString\(\) \?\? \1', 'GetLocalizedString("$1")'
$orig = $orig -replace 'Services\.LocalizationResourceManager\.Instance\["([^"]+)"\]', 'GetLocalizedString("$1")'
$orig = $orig -replace 'Services\.LocalizationResourceManager\.Instance\.CurrentLanguageCode', 'GetCurrentLanguageCode()'

[System.IO.File]::WriteAllText("VinhKhanhTour.Shared\Models\Poi.cs", $orig, [System.Text.Encoding]::UTF8)
