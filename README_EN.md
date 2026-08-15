# HyperMC Executor // ModSync v2.0.0

Automated Minecraft Modpack Synchronization Tool  
Author: c0devX-fonq // HyperMC Team  
Version: 2.0.0

---

## 1. OVERVIEW

HyperMC Executor // ModSync v2.0.0 is a standalone portable application developed in C# WinForms, designed to automatically synchronize full Minecraft modpacks (Fabric/Forge) from Google Drive or direct download URLs directly into the game's mods directory with a single click.

---

## 2. CORE FUNCTIONALITIES

- Automatic Launcher Path Detection: Auto-detects mods folders across major launchers including Official Minecraft Launcher, TLauncher, TLegacy, Modrinth App, Prism Launcher / MultiMC, CurseForge, and SKLauncher, alongside custom directory selection.
- Google Drive Link Conversion: Automatically transforms Google Drive view URLs into direct download streams while bypassing large-file virus confirmation prompts.
- Modpack ZIP Archive Validation: Verifies PK magic bytes and inspects archive contents for valid .jar mod files or modpack manifests (manifest.json, modrinth.index.json, instance.cfg) to reject corrupt or invalid archives.
- Pre-Patch Metrics Confirmation: Prompts users with total file size (MB) and detected mod counts for confirmation prior to extracting and overwriting files.
- Mod Backup and Storage Management: Automatically creates timestamped backups (mods_backup_YYYYMMDD_HHMMSS) of current mods before applying patches, featuring a built-in storage utility to calculate and purge old backups.
- Asynchronous Download Cancellation: Supports mid-download cancellation with instant cleanup of temporary download files.
- Intelligent Input Auto-Routing: Auto-detects and corrects swapped inputs when a Windows path is pasted into the URL field or a web URL into the folder path field.
- Modern File Explorer Integration: Invokes native, full-sized resizable Windows File Explorer dialogs for intuitive folder browsing.
- Dual Language Support: Provides real-time switching between Vietnamese and English languages.
- Safe Configuration Management: Persists configuration within %APPDATA%\HyperMCModSync\ to prevent unwanted file creation on the Desktop.
- Single-File Portable Operation: Functions entirely within a single SyncMod.exe binary with no installation or external DLL dependencies required.

---

## 3. COMPILATION GUIDE

Compile directly using the Windows built-in C# compiler (csc.exe) via PowerShell:

```powershell
$csc = "$env:Windir\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) { $csc = "$env:Windir\Microsoft.NET\Framework\v4.0.30319\csc.exe" }

& $csc /target:winexe /win32icon:"icon.ico" /out:"SyncMod.exe" /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll,System.Net.Http.dll "SyncMod.cs"
```

---

## 4. CONFIGURATION SYSTEM

- config.json (Default Settings): Reads from the executable directory or initializes within %APPDATA%\HyperMCModSync\config.json for server defaults.
- settings.json (User Persistence): Automatically saves and restores the user's last selected mods folder, modpack URL, and language preferences across sessions.

---

## 5. SYSTEM REQUIREMENTS

- Operating System: Windows 7, 8.1, 10, 11 (32-bit & 64-bit).
- Runtime: .NET Framework 4.5 or higher.

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
