# HyperMC Executor // ModSync v2.0.0

Công cụ đồng bộ Modpack Minecraft tự động  
Tác giả: c0devX-fonq // HyperMC Team  
Phiên bản: 2.0.0

---

## 1. TỔNG QUAN

HyperMC Executor // ModSync v2.0.0 là phần mềm độc lập (Portable Executable) phát triển bằng C# WinForms, hỗ trợ đồng bộ hóa tự động các bộ Modpack (Fabric/Forge) từ Google Drive hoặc liên kết tải trực tiếp vào thư mục game Minecraft chỉ với 1 thao tác.

---

## 2. CHỨC NĂNG CHÍNH

- Tự động phát hiện thư mục Mods của hầu hết các Launcher phổ biến bao gồm Official Minecraft Launcher, TLauncher, TLegacy, Modrinth App, Prism Launcher / MultiMC, CurseForge, SKLauncher và vị trí tùy chỉnh.
- Chuyển đổi liên kết xem Google Drive thành luồng tải trực tiếp và tự động vượt qua màn hình xác nhận quét virus đối với file dung lượng lớn.
- Kiểm tra tiêu đề mã hóa PK Magic Bytes và quét file .jar hoặc cấu hình manifest bên trong file ZIP để ngăn chặn file hỏng.
- Hiển thị chi tiết dung lượng MB và số lượng mod phát hiện để người dùng xác nhận trước khi thực hiện ghi đè dữ liệu.
- Tạo bản sao lưu mod hiện tại có mốc thời gian (mods_backup_YYYYMMDD_HHMMSS) và tích hợp công cụ xóa bản sao lưu cũ để giải phóng đĩa cứng.
- Hỗ trợ dừng quá trình tải về bất kỳ lúc nào và tự động dọn dẹp các tệp tin tạm.
- Tự động phát hiện và định tuyến lại khi dán nhầm đường dẫn Windows vào ô Link hoặc dán đường dẫn Web vào ô Thư mục.
- Tích hợp cửa sổ Windows File Explorer chính thức có khả năng phóng to và tùy chỉnh kích thước để chọn thư mục dễ dàng.
- Chuyển đổi giao diện tức thời giữa Tiếng Việt và Tiếng Anh.
- Tự động quản lý cấu hình tại %APPDATA%\HyperMCModSync\ để giữ màn hình Desktop luôn sạch sẽ.
- Hoạt động hoàn toàn trong 1 file SyncMod.exe duy nhất mà không cần cài đặt hay file thư viện phụ thuộc.

---

## 3. HƯỚNG DẪN BIÊN DỊCH

Biên dịch trực tiếp bằng trình biên dịch C# (csc.exe) đi kèm Windows thông qua PowerShell:

```powershell
$csc = "$env:Windir\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) { $csc = "$env:Windir\Microsoft.NET\Framework\v4.0.30319\csc.exe" }

& $csc /target:winexe /win32icon:"icon.ico" /out:"SyncMod.exe" /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll,System.Net.Http.dll "SyncMod.cs"
```

---

## 4. HỆ THỐNG CẤU HÌNH

- config.json đọc từ thư mục ứng dụng hoặc tự khởi tạo tại %APPDATA%\HyperMCModSync\config.json để lưu thông số máy chủ.
- settings.json tự động khôi phục đường dẫn đã chọn, link modpack và ngôn ngữ ưu tiên của người dùng ở lần khởi động sau.

---

## 5. YÊU CẦU HỆ THỐNG

- Hệ điều hành: Windows 7, 8.1, 10, 11 (32-bit & 64-bit).
- Runtime: .NET Framework 4.5 trở lên.

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
