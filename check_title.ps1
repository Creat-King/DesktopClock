Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;
public class WinTitle {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    
    [DllImport("user32.dll")]
    public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO {
        public uint cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public uint uCount;
        public uint dwTimeout;
    }
    
    public static string GetTitle() {
        IntPtr hwnd = FindWindow("WeWorkWindow", null);
        if (hwnd == IntPtr.Zero) return "NOT FOUND";
        StringBuilder sb = new StringBuilder(512);
        GetWindowText(hwnd, sb, 512);
        return sb.ToString();
    }
    
    public static IntPtr GetHwnd() {
        return FindWindow("WeWorkWindow", null);
    }
}
"@

$title = [WinTitle]::GetTitle()
$hwnd = [WinTitle]::GetHwnd()
Write-Output "Title: '$title'"
Write-Output "HWND: $hwnd"
