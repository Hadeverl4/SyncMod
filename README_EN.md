# HyperMC Executor // ModSync v2.0.0

Automated Minecraft Modpack Synchronization Tool  
Author: c0devX-fonq // HyperMC Team  
Version: 2.0.0

---

## 1. OVERVIEW

HyperMC Executor // ModSync v2.0.0 is a standalone single-file portable application built with C# WinForms. It allows server administrators and Minecraft players to synchronize full modpacks (Fabric/Forge) directly from Google Drive or direct download URLs into their game's mods folder with just one click.

The interface is styled after the official Minecraft Launcher UI, featuring a left navigation rail, active green indicator bar, and an authentic 3D Minecraft Play button with a tactile sunken press animation.

---

## 2. KEY FEATURES IN VERSION 2.0.0

- HD Minecraft Launcher Aesthetics: Left navigation bar, green active indicator bar, and custom GDI+ pixelated 3D Minecraft button with sunken click feedback.
- Multi-Launcher Auto-Detection: Automatically detects mods directories for all popular launchers:
  + Official Minecraft Launcher (Auth) (.minecraft)
  + TLauncher (.minecraft)
  + TLegacy Launcher (.tlauncher/legacy)
  + Modrinth App (ModrinthApp/profiles)
  + Prism Launcher / MultiMC (PrismLauncher/instances)
  + CurseForge (curseforge/minecraft/Instances)
  + SKLauncher (.sklauncher)
  + Custom Directory
- Automatic Google Drive Link Converter: Automatically transforms Google Drive view links into direct download streams while bypassing Google Drive large-file virus confirmation screens.
- Modpack ZIP Archive Validation: Inspects ZIP magic headers (PK) and entry manifests to verify the presence of .jar mod files or modpack manifests (manifest.json, modrinth.index.json, instance.cfg). Rejects fake or corrupt archives.
- Pre-Patch Pause and Confirmation: Prompts the user with total archive size (MB) and mod count before overwriting and extracting files.
- Old Mod Backup and Space Cleaner: Automatically backs up existing mods into timestamped folders (mods_backup_YYYYMMDD_HHMMSS) and provides a built-in disk space cleaner to calculate and remove old backups.
- Dynamic Download Cancellation: Allows users to asynchronously cancel active downloads midway with instant cleanup of temporary files.
- Smart Auto-Routing: Auto-detects mistakenly swapped inputs (Windows folder paths pasted into the URL box, or web links pasted into the folder box) and reroutes them automatically.
- Modern Resizable Windows File Explorer Dialog: Invokes a full-sized Windows File Explorer dialog with navigation panes and search bars for custom folder selection.
- Bilingual UI Support: Instant toggling between Vietnamese and English languages.
- Multi-Resolution HD Icon: Embedded 10-layer DIB/PNG icon pack (256x256 down to 16x16) ensuring crisp rendering across all Windows Desktop view modes and DPI scales.
- Standalone Portable Executable: Runs 100% standalone in a single SyncMod.exe binary without needing installation or external DLL dependencies.

---

## 3. COMPILATION INSTRUCTIONS

The application can be compiled directly using Windows built-in C# compiler (csc.exe) via PowerShell:

```powershell
$csc = "$env:Windir\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) { $csc = "$env:Windir\Microsoft.NET\Framework\v4.0.30319\csc.exe" }

& $csc /target:winexe /win32icon:"icon.ico" /out:"SyncMod.exe" /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll,System.Net.Http.dll "SyncMod.cs"
```

---

## 4. CONFIGURATION SYSTEM

The tool utilizes a two-tier configuration structure:

1. config.json (Application Directory):
   Contains server default values set by the server administrator:
   ```json
   {
     "serverName": "HyperMC",
     "authorCredit": "Tool by c0devX-fonq",
     "targetVersion": "Fabric 1.21.1",
     "modpackUrl": "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID",
     "minecraftPath": "%APPDATA%\\.minecraft"
   }
   ```

2. %APPDATA%\HyperMCModSync\settings.json (User Local Persistence):
   Automatically saves and restores the user's last selected mods folder, last modpack URL, and language preference across sessions.

---

## 5. SYSTEM REQUIREMENTS

- Operating System: Windows 7 / Windows 8.1 / Windows 10 / Windows 11 (32-bit & 64-bit).
- Runtime: .NET Framework 4.5 or higher (pre-installed on Windows 10 and 11).

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
