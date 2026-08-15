# HyperMC Executor // ModSync v2.0.0

Công cụ đồng bộ Modpack Minecraft tự động (Minecraft Modpack Synchronization Tool)  
Tác giả: c0devX-fonq // HyperMC Team  
Phiên bản: 2.0.0

---

## 1. GIỚI THIỆU CHUNG

HyperMC Executor // ModSync v2.0.0 là một phần mềm độc lập (Portable Executable) được thiết kế bằng C# WinForms, giúp chủ máy chủ và người chơi Minecraft đồng bộ hóa toàn bộ bộ Modpack (Fabric/Forge) từ Google Drive hoặc Link tải trực tiếp vào thư mục Mods của game chỉ với 1 cú click.

Giao diện phần mềm được thiết kế theo phong cách giao diện chính thức của Minecraft Launcher với thanh điều hướng dọc bên trái, nút đồng bộ 3D nổi và hiệu ứng lún bấm chân thật.

---

## 2. CÁC TÍNH NĂNG CHÍNH TRONG PHIÊN BẢN 2.0.0

- Giao diện Minecraft Launcher HD: Thanh Navigation Rail bên trái, đèn xanh phát sáng Active Indicator và nút bấm 3D kiểu Minecraft với hiệu ứng lún bấm (Sunken Press Effect).
- Tự động quét và nhận diện Launcher: Nhận diện và tự động phát hiện đường dẫn Mods của nhiều Launcher thông dụng:
  + Official Minecraft Launcher (Auth) (.minecraft)
  + TLauncher (.minecraft)
  + TLegacy Launcher (.tlauncher/legacy)
  + Modrinth App (ModrinthApp/profiles)
  + Prism Launcher / MultiMC (PrismLauncher/instances)
  + CurseForge (curseforge/minecraft/Instances)
  + SKLauncher (.sklauncher)
  + Thư mục Tùy chọn Custom
- Tự động chuyển đổi Link Google Drive: Tự động chuyển các đường link Google Drive thành Link tải trực tiếp và vượt qua trang xác nhận quét virus của Google với các file dung lượng lớn.
- Kiểm tra cấu trúc Modpack ZIP: Tự động kiểm tra định dạng mã hóa Magic Bytes (PK) và kiểm tra bên trong file ZIP xem có chứa các file mod .jar hoặc file manifest modpack (manifest.json, modrinth.index.json, instance.cfg) hay không. Tự động từ chối các file ZIP giả hoặc file lỗi.
- Bước xác nhận và Tạm dừng trước khi Patch: Tích hợp bước xác nhận hiển thị thông tin dung lượng (MB) và số lượng mod trước khi ghi đè và giải nén.
- Sao lưu và Dọn dẹp Mod cũ: Tự động tạo thư mục sao lưu (mods_backup_YYYYMMDD_HHMMSS) trước khi chép mod mới. Tích hợp nút và thông báo tính dung lượng đĩa (MB/GB) để xóa sạch các bản sao lưu cũ khi cần thiết.
- Nút Hủy tải về động (Dynamic Cancel Download): Trong quá trình đang tải file, nút chính tự động chuyển sang trạng thái Hủy tải về (Cancel Async) và dọn dẹp file tạm.
- Định tuyến thông minh (Smart Auto Route): Tự động phát hiện và chuyển hướng khi người dùng dán nhầm Link Web vào ô Thư mục hoặc dán đường dẫn Windows vào ô Link Modpack.
- Cửa sổ Windows File Explorer hiện đại: Nút Duyệt... mở trực tiếp cửa sổ Windows Explorer cỡ lớn, có thể phóng to và điều chỉnh kích thước để chọn thư mục dễ dàng.
- Hỗ trợ Đa ngôn ngữ (Bilingual): Chuyển đổi linh hoạt và tức thời giữa Tiếng Việt và Tiếng Anh.
- Icon HD Đa độ phân giải (Multi-Resolution HD Icon): Tích hợp file Icon HD đa lớp kích thước (256x256, 128x128, 96x96, 72x72, 64x64, 48x48, 32x32, 16x16 DIB ARGB) hiển thị sắc nét trên mọi chế độ view Desktop và độ phân giải màn hình.
- Single-File Portable: Hoàn toàn chạy độc lập 100% trong 1 file SyncMod.exe duy nhất, không cần cài đặt, không cần file DLL hoặc file .config đi kèm.

---

## 3. HƯỚNG DẪN BIÊN DỊCH (COMPILATION GUIDE)

Phần mềm được biên dịch trực tiếp bằng trình biên dịch built-in C# csc.exe có sẵn trên Windows bằng PowerShell:

```powershell
$csc = "$env:Windir\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) { $csc = "$env:Windir\Microsoft.NET\Framework\v4.0.30319\csc.exe" }

& $csc /target:winexe /win32icon:"icon.ico" /out:"SyncMod.exe" /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll,System.Net.Http.dll "SyncMod.cs"
```

---

## 4. CẤU TRÚC FILE CẤU HÌNH (CONFIG SYSTEM)

Phần mềm sử dụng 2 cấp file cấu hình:

1. config.json (Thư mục chương trình):
   Chứa thông tin cấu hình mặc định do chủ Server hoặc người tạo Tool thiết lập:
   ```json
   {
     "serverName": "HyperMC",
     "authorCredit": "Tool by c0devX-fonq",
     "targetVersion": "Fabric 1.21.1",
     "modpackUrl": "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID",
     "minecraftPath": "%APPDATA%\\.minecraft"
   }
   ```

2. %APPDATA%\HyperMCModSync\settings.json (Lưu trữ cá nhân người dùng):
   Tự động ghi nhớ và phục hồi đường dẫn mods cuối cùng, link modpack cuối cùng và ngôn ngữ ưu tiên của người dùng khi mở lại tool.

---

## 5. YÊU CẦU HỆ THỐNG

- Hệ điều hành: Windows 7 / Windows 8.1 / Windows 10 / Windows 11 (32-bit và 64-bit).
- Runtime: .NET Framework 4.5 trở lên (có sẵn 100% trên mọi máy Windows 10 và Windows 11).

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
