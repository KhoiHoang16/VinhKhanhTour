namespace VinhKhanhTour.Shared.Models
{
    /// <summary>
    /// Trạng thái phê duyệt nội dung POI.
    /// Pending = 0: Chờ Admin duyệt (mặc định khi tạo mới).
    /// Approved = 1: Đã được Admin phê duyệt, hiển thị công khai.
    /// Rejected = 2: Admin từ chối, kèm AdminNote giải thích lý do.
    /// </summary>
    public enum ApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
