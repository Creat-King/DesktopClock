@echo off
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set WPF=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF
set REF=C:\Windows\Microsoft.NET\Framework64\v4.0.30319

echo Compiling DesktopClock...
%CSC% /target:winexe /out:DesktopClock.exe /platform:x64 /reference:%WPF%\PresentationCore.dll /reference:%WPF%\PresentationFramework.dll /reference:%WPF%\WindowsBase.dll /reference:%REF%\System.Windows.Forms.dll /reference:%REF%\System.Drawing.dll /reference:%REF%\System.Xaml.dll Program.cs ClockWindow.cs TrayManager.cs Win32.cs Settings.cs

if %errorlevel% == 0 (
    echo.
    echo Build successful! Output: DesktopClock.exe
    echo.
) else (
    echo.
    echo Build FAILED with error %errorlevel%
    echo.
)
pause
