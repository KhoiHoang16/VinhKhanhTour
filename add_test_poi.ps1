$body = @{
    name = "Quán Ốc Thảo - 383 Vĩnh Khánh"
    nameEn = "Thao Snail - 383 Vinh Khanh"
    description = "Một trong những quán ăn đông khách nhất phố. Giá cả phải chăng, ốc tươi ngon."
    descriptionEn = "One of the busiest restaurants on the street. Affordable prices, fresh and delicious snails."
    latitude = 10.76185
    longitude = 106.70335
    radius = 30
    ttsScript = "Quán Ốc Thảo nằm ở số 383 Vĩnh Khánh, nổi tiếng với ốc hương nướng mỡ hành."
    ttsScriptEn = "Thao Snail at 383 Vinh Khanh is famous for grilled whelks with scallion oil."
    priority = 4
    imageUrl = "oc_oanh_vinh_khanh_1773306578974.png"
} | ConvertTo-Json -Depth 5

$bytes = [System.Text.Encoding]::UTF8.GetBytes($body)
$response = Invoke-WebRequest -Uri "https://vinhkhanhtour.onrender.com/api/poi" -Method Post -ContentType "application/json; charset=utf-8" -Body $bytes
[System.Text.Encoding]::UTF8.GetString($response.Content)
