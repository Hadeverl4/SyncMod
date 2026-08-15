# HyperMC Executor // ModSync v2.0.0

Language: **English** | [Tiếng Việt](README_VN.md)

Automated Minecraft Modpack Synchronization Tool  
Author: c0devX-fonq // HyperMC Team  
Version: 2.0.0

---

## 1. OVERVIEW

HyperMC Executor // ModSync v2.0.0 is a standalone portable application developed in C# WinForms, designed to automatically synchronize full Minecraft modpacks (Fabric/Forge) from Google Drive or direct download URLs directly into the game's mods directory with a single click.

---

## 2. CORE FUNCTIONALITIES

- Automatically detects mods folders across major launchers including Official Minecraft Launcher, TLauncher, TLegacy, Modrinth App, Prism Launcher / MultiMC, CurseForge, SKLauncher, and custom directories.
- Converts Google Drive view URLs into direct download streams while bypassing large-file virus confirmation prompts automatically.
- Verifies PK magic bytes and inspects archive contents for valid .jar mod files or modpack manifests to reject corrupt or invalid archives.
- Displays file size in MB and detected mod counts for user confirmation prior to extracting and overwriting files.
- Creates timestamped backups (mods_backup_YYYYMMDD_HHMMSS) of current mods before applying patches and includes a utility to purge old backups.
- Supports mid-download cancellation with instant cleanup of temporary files.
- Detects and corrects swapped inputs when a Windows folder path is pasted into the URL field or a web URL into the folder path field.
- Integrates native, full-sized resizable Windows File Explorer dialogs for intuitive folder browsing.
- Provides real-time switching between Vietnamese and English languages.
- Persists configuration within %APPDATA%\HyperMCModSync\ to prevent unwanted file creation on the Desktop.
- Functions entirely within a single SyncMod.exe binary with no installation or external DLL dependencies required.

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

- config.json reads from the executable directory or initializes within %APPDATA%\HyperMCModSync\config.json for server defaults.
- settings.json saves and restores the user's last selected mods folder, modpack URL, and language preferences across sessions.

---

## 5. SYSTEM REQUIREMENTS

- Operating System: Windows 7, 8.1, 10, 11 (32-bit & 64-bit).
- Runtime: .NET Framework 4.5 or higher.

---

Copyright (C) 2026 HyperMC Team // c0devX-fonq. All rights reserved.
