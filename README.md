# 🍜 Vinh Khanh Tour - Smart Tourism System

![Vinh Khanh Tour](https://img.shields.io/badge/Project-Vinh_Khanh_Tour-orange?style=for-the-badge)
![.NET 9](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=.net)
![Blazor](https://img.shields.io/badge/Blazor-CMS-512BD4?style=for-the-badge&logo=blazor)
![MAUI](https://img.shields.io/badge/MAUI-Mobile_App-1384C8?style=for-the-badge&logo=dotnet)

**Vinh Khanh Tour** là hệ thống ứng dụng du lịch thông minh dành riêng cho Phố ẩm thực Vĩnh Khánh (Quận 4, TP.HCM). Hệ thống bao gồm Ứng dụng di động (Mobile App) hỗ trợ du khách khám phá các địa điểm, Hệ thống Quản trị Nội dung (CMS), và Web API đồng bộ dữ liệu.

---

## 🏗️ Kiến trúc Hệ thống

Dự án được chia thành các thành phần chính sau:

1. **`VinhKhanhTour.Api`**: Backend API xây dựng bằng ASP.NET Core, cung cấp các endpoint để đồng bộ dữ liệu Điểm đến (POI), Lịch sử sử dụng và Analytics.
2. **`VinhKhanhTour.CMS`**: Trang quản trị Web xây dựng bằng **Blazor Server**. Dùng để quản lý thông tin các cảnh điểm, theo dõi biểu đồ (Heatmap/Analytics), quét mã QR và tạo các bản dịch Audio (Text-to-Speech).
3. **`VinhKhanhTour.Shared`**: Thư viện dùng chung chứa các Models (như `Poi`, `Analytics`) để đồng nhất kiểu dữ liệu giữa API, CMS và Mobile App.
4. **`VinhKhanhTour (Mobile App)`**: Ứng dụng di động đa nền tảng viết bằng **.NET MAUI**, tích hợp bản đồ số, hệ thống đồng bộ SQLite offline-first và phát Audio song ngữ Việt/Anh.

---

## ✨ Tính năng nổi bật

### 📱 Ứng dụng Di động (MAUI)
* **Bản đồ Tương tác**: Hiển thị các điểm đến (POI), nhà hàng, quán ăn trên phố Vĩnh Khánh.
* **Đồng bộ Offline**: Dữ liệu được tải về và lưu vào SQLite nội bộ, cho phép xem bản đồ khi không có mạng.
* **Song ngữ & Thuyết minh**: Hỗ trợ tiếng Việt và tiếng Anh, tự động phát âm thanh mô tả địa điểm (TTS).
* **Quét QR Code**: Nhận diện các điểm dừng xe buýt hoặc các địa điểm đặc biệt.

### 💻 Hệ thống Quản trị (CMS)
* **Quản lý POI Trực quan**: Thêm/Sửa/Xóa điểm đến, hỗ trợ **Bản đồ chọn toạ độ trực tiếp (Map Picker bằng Leaflet.js)** cực kỳ tiện lợi.
* **Theo dõi Analytics**: Thống kê lượt truy cập, vẽ Heatmap các vị trí được người dùng quan tâm.
* **Quản lý Phương tiện**: Hỗ trợ dịch ngôn ngữ và tạo mã QR tự động.
* **Giao diện Hiện đại**: Hệ thống Dashboard trực quan với Darkmode/Cửa sổ nổi thân thiện với người quản trị.

---

## 🛠️ Công nghệ sử dụng

* **Nền tảng**: C# / .NET 9.0
* **Mobile**: .NET MAUI
* **Web CMS**: Blazor Web App (Interactive Server)
* **Backend**: ASP.NET Core Minimal APIs
* **Database**: SQLite (Mobile) & SQL Server / Entity Framework Core (Backend)
* **Bản đồ**: Leaflet.js (CMS) / Mapsui (Mobile)

---

## 🚀 Hướng dẫn Cài đặt & Chạy thử

### Yêu cầu hệ thống
- Visual Studio 2022 (phiên bản mới nhất có hỗ trợ .NET MAUI và ASP.NET).
- .NET 9.0 SDK.

### Khởi chạy CMS và API
1. Chọn Project Khởi động (Startup Project) là `VinhKhanhTour.CMS` và `VinhKhanhTour.Api` (Có thể dùng tính năng Multiple Startup Projects trong Visual Studio).
2. Chạy ứng dụng bằng `F5`.
3. CMS sẽ có thể truy cập qua trình duyệt tại địa chỉ `http://localhost:<port>`.

### Khởi chạy Mobile App
1. Đặt `VinhKhanhTour` làm Startup Project.
2. Cắm thiết bị Android hoặc cấu hình thư mục chạy Windows Machine (hoặc Android Emulator).
3. Đảm bảo API Backend đang chạy và URL của API đã được cập nhật đúng trong `ApiService` của MAUI để có thể kéo/đồng bộ dữ liệu chính xác.

---

*Phát triển bởi đội ngũ quản lý dự án **Vinh Khanh Tour**.*
