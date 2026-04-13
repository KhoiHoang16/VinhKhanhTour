using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// Service tạo URL thanh toán VNPay và xác thực chữ ký return/IPN.
    /// Tài liệu: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
    /// </summary>
    public class VNPayPaymentService
    {
        private readonly IConfiguration _config;

        public VNPayPaymentService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPay. Redirect user tới URL này.
        /// </summary>
        public string CreatePaymentUrl(string orderId, long amount, string orderInfo, string ipAddress)
        {
            var tmnCode = _config["VNPay:TmnCode"] ?? "";
            var hashSecret = _config["VNPay:HashSecret"] ?? "";
            var vnpUrl = _config["VNPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var returnUrl = _config["VNPay:ReturnUrl"] ?? "";

            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", (amount * 100).ToString() },  // VNPay yêu cầu x100
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderId },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            // Tạo query string và chữ ký
            var queryString = BuildQueryString(vnpParams);
            var signData = queryString;
            var secureHash = ComputeHmacSha512(signData, hashSecret);

            return $"{vnpUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        /// <summary>
        /// Xác thực chữ ký từ VNPay return URL / IPN.
        /// </summary>
        public bool ValidateSignature(IQueryCollection queryParams)
        {
            var hashSecret = _config["VNPay:HashSecret"] ?? "";
            var vnpSecureHash = queryParams["vnp_SecureHash"].ToString();

            // Lấy tất cả params trừ vnp_SecureHash và vnp_SecureHashType
            var sortedParams = new SortedDictionary<string, string>();
            foreach (var key in queryParams.Keys)
            {
                if (key != "vnp_SecureHash" && key != "vnp_SecureHashType" && !string.IsNullOrEmpty(queryParams[key]))
                {
                    sortedParams[key] = queryParams[key].ToString();
                }
            }

            var signData = BuildQueryString(sortedParams);
            var computedHash = ComputeHmacSha512(signData, hashSecret);

            return computedHash.Equals(vnpSecureHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse kết quả trả về từ VNPay.
        /// </summary>
        public VNPayReturnResult ParseReturn(IQueryCollection queryParams)
        {
            return new VNPayReturnResult
            {
                ResponseCode = queryParams["vnp_ResponseCode"].ToString(),
                TransactionNo = queryParams["vnp_TransactionNo"].ToString(),
                TxnRef = queryParams["vnp_TxnRef"].ToString(),
                Amount = long.TryParse(queryParams["vnp_Amount"].ToString(), out var amt) ? amt / 100 : 0,
                OrderInfo = queryParams["vnp_OrderInfo"].ToString(),
                IsSuccess = queryParams["vnp_ResponseCode"].ToString() == "00"
            };
        }

        private static string BuildQueryString(SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            foreach (var kvp in parameters)
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append($"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}");
            }
            return sb.ToString();
        }

        private static string ComputeHmacSha512(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var msgBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(msgBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public class VNPayReturnResult
    {
        public string ResponseCode { get; set; } = "";
        public string TransactionNo { get; set; } = "";
        public string TxnRef { get; set; } = "";
        public long Amount { get; set; }
        public string OrderInfo { get; set; } = "";
        public bool IsSuccess { get; set; }
    }
}
