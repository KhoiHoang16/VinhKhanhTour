using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.Shared.Models
{
    public class MerchantRegistrationRequest
    {
        // === Step 1: Thông tin khách hàng ===
        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [RegularExpression(@"^(03|05|07|08|09)+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [MinLength(3, ErrorMessage = "Mật khẩu phải có ít nhất 3 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận Mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // === Step 2: Thông tin quán ===
        [Required(ErrorMessage = "Vui lòng nhập Tên địa điểm (POI)")]
        public string PoiName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ cơ sở")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn Loại hình kinh doanh")]
        public string BusinessType { get; set; } = string.Empty;

        // === Step 3: Phương thức thanh toán ===
        public string PaymentMethod { get; set; } = "bank_transfer";
    }
}
