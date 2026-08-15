# HyperMC Executor // ModSync v2.0.0

Công cụ đồng bộ Modpack Minecraft tự động  
Tác giả: c0devX-fonq // HyperMC Team  
Phiên bản: 2.0.0

---

## 1. TỔNG QUAN

HyperMC Executor // ModSync v2.0.0 là phần mềm độc lập (Portable Executable) phát triển bằng C# WinForms, hỗ trợ đồng bộ hóa tự động các bộ Modpack (Fabric/Forge) từ Google Drive hoặc liên kết tải trực tiếp vào thư mục game Minecraft chỉ với 1 thao tác.

---

## 2. CHỨC NĂNG CHÍNH

- Tự động nhận diện đường dẫn Launcher: Tự phát hiện thư mục Mods của hầu hết các Launcher phổ biến bao gồm Official Minecraft Launcher, TLauncher, TLegacy, Modrinth App, Prism Launcher / MultiMC, CurseForge và SKLauncher, cùng tùy chọn thư mục tùy chỉnh.
- Chuyển đổi liên kết Google Drive: Tự động biến đổi liên kết xem Google Drive thành luồng tải trực tiếp và bỏ qua màn hình xác nhận quét virus đối với các file dung lượng lớn.
- Kiểm tra hợp lệ file ZIP Modpack: Xác minh tiêu đề file (PK Magic Bytes) và quét cấu trúc bên trong để đảm bảo sự tồn tại của file mod .jar hoặc file cấu hình modpack (manifest.json, modrinth.index.json, instance.cfg) trước khi xử lý, ngăn chặn file rỗng hoặc file lỗi.
- Xác nhận thông số trước khi Patch: Hiển thị dung lượng file (MB) cùng số lượng mod phát hiện để người dùng xác nhận trước khi thực hiện ghi đè dữ liệu.
- Sao lưu và Dọn dẹp dung lượng đĩa: Tự động lưu bản sao mod hiện tại vào thư mục sao lưu có mốc thời gian (mods_backup_YYYYMMDD_HHMMSS) trước khi chép mod mới, tích hợp công cụ tính toán và xóa các bản sao lưu cũ để giải phóng đĩa cứng.
- Hủy tải về linh hoạt: Cho phép dừng quá trình tải về bất kỳ lúc nào và tự động dọn dẹp file tạm.
- Tự động sửa vị trí nhập liệu: Phát hiện và định tuyến lại thông minh khi dán nhầm đường dẫn Windows vào ô Link hoặc dán đường dẫn Web vào ô Thư mục.
- Tích hợp Windows File Explorer cỡ lớn: Mở trực tiếp cửa sổ chọn thư mục hệ thống có khả năng phóng to và tùy chỉnh kích thước.
- Hỗ trợ đa ngôn ngữ: Chuyển đổi giao diện tức thời giữa Tiếng Việt và Tiếng Anh.
- Lưu cấu hình an toàn: Quản lý cấu hình thông minh tại %APPDATA%\HyperMCModSync\, đảm bảo không tự sinh file rác trên màn hình Desktop.
- Chạy độc lập Portable: Hoạt động hoàn toàn trong 1 file SyncMod.exe duy nhất, không yêu cầu cài đặt hay file thư viện phụ thuộc.

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

- config.json (Cấu hình mặc định): Đọc từ cùng thư mục phần mềm hoặc tự khởi tạo tại %APPDATA%\HyperMCModSync\config.json để lưu các thông số máy chủ ban đầu.
- settings.json (Lưu trữ cá nhân): Tự động khôi phục đường dẫn đã chọn, link modpack và ngôn ngữ ưu tiên của người dùng ở lần khởi động sau.

---

## 5. YÊU CẦU HỆ THỐNG

- Hệ điều hành: Windows 7, 8.1, 10, 11 (32-bit & 64-bit).
- Runtime: .NET Framework 4.5 trở lên.

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
