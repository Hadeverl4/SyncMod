@echo off
title Building HyperMC Executor ModSync...
echo [INFO] Compiling SyncMod.cs to SyncMod.exe using .NET Framework C# Compiler...

set CSC_PATH=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC_PATH%" set CSC_PATH=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe

"%CSC_PATH%" /target:winexe /out:SyncMod.exe /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll,System.Net.Http.dll SyncMod.cs

if %ERRORLEVEL% EQU 0 (
    echo [SUCCESS] SyncMod.exe has been compiled successfully!
) else (
    echo [ERROR] Compilation failed.
)
pause
