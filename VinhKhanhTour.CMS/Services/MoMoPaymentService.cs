using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// Service tạo link thanh toán MoMo và xác thực IPN callback.
    /// Tài liệu: https://developers.momo.vn/v3/docs/payment/api/wallet/onetime
    /// </summary>
    public class MoMoPaymentService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public MoMoPaymentService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient("MoMo");
        }

        /// <summary>
        /// Tạo link thanh toán MoMo. Trả về payUrl để redirect user.
        /// </summary>
        public async Task<MoMoCreateResponse?> CreatePaymentAsync(string orderId, long amount, string orderInfo, string extraData = "")
        {
            var partnerCode = _config["MoMo:PartnerCode"] ?? "";
            var accessKey = _config["MoMo:AccessKey"] ?? "";
            var secretKey = _config["MoMo:SecretKey"] ?? "";
            var endpoint = _config["MoMo:Endpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";
            var returnUrl = _config["MoMo:ReturnUrl"] ?? "";
            var ipnUrl = _config["MoMo:IpnUrl"] ?? "";
            var requestId = Guid.NewGuid().ToString();
            var requestType = "payWithMethod";

            // Tạo chữ ký HMAC SHA256
            var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}"
                + $"&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}"
                + $"&partnerCode={partnerCode}&redirectUrl={returnUrl}"
                + $"&requestId={requestId}&requestType={requestType}";

            var signature = ComputeHmacSha256(rawSignature, secretKey);

            var body = new
            {
                partnerCode,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl,
                extraData,
                requestType,
                signature,
                lang = "vi"
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<MoMoCreateResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// Xác thực chữ ký IPN từ MoMo callback.
        /// </summary>
        public bool VerifyIpnSignature(MoMoIpnRequest ipn)
        {
            var accessKey = _config["MoMo:AccessKey"] ?? "";
            var secretKey = _config["MoMo:SecretKey"] ?? "";

            var rawSignature = $"accessKey={accessKey}&amount={ipn.Amount}&extraData={ipn.ExtraData}"
                + $"&message={ipn.Message}&orderId={ipn.OrderId}&orderInfo={ipn.OrderInfo}"
                + $"&orderType={ipn.OrderType}&partnerCode={ipn.PartnerCode}"
                + $"&payType={ipn.PayType}&requestId={ipn.RequestId}"
                + $"&responseTime={ipn.ResponseTime}&resultCode={ipn.ResultCode}"
                + $"&transId={ipn.TransId}";

            var computed = ComputeHmacSha256(rawSignature, secretKey);
            return computed == ipn.Signature;
        }

        private static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var msgBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(msgBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public class MoMoCreateResponse
    {
        public string PartnerCode { get; set; } = "";
        public string OrderId { get; set; } = "";
        public string RequestId { get; set; } = "";
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; } = "";
        public int ResultCode { get; set; }
        public string PayUrl { get; set; } = "";
        public string QrCodeUrl { get; set; } = "";
    }

    public class MoMoIpnRequest
    {
        public string PartnerCode { get; set; } = "";
        public string OrderId { get; set; } = "";
        public string RequestId { get; set; } = "";
        public long Amount { get; set; }
        public string OrderInfo { get; set; } = "";
        public string OrderType { get; set; } = "";
        public long TransId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; } = "";
        public string PayType { get; set; } = "";
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; } = "";
        public string Signature { get; set; } = "";
    }
}
