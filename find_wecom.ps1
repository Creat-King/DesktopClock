Add-Type @"
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
public class WinFind {
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    
    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }
    
    public static List<IntPtr> FindAll() {
        var result = new List<IntPtr>();
        EnumWindows((hWnd, lParam) => {
            if (IsWindowVisible(hWnd)) {
                StringBuilder title = new StringBuilder(512);
                StringBuilder cls = new StringBuilder(256);
                GetWindowText(hWnd, title, 512);
                GetClassName(hWnd, cls, 256);
                string t = title.ToString();
                string c = cls.ToString();
                if (t.Contains("WeChat") || t.Contains("WeWork") || t.Contains("WeCom") || 
                    c.Contains("WeChat") || c.Contains("WeWork") || c.Contains("WeCom") ||
                    t.Contains("企业微信") || t.Contains("WXWork") || c.Contains("WXWork") ||
                    c.Contains("Weixin") || t.Contains("微信")) {
                    result.Add(hWnd);
                }
            }
            return true;
        }, IntPtr.Zero);
        return result;
    }
}
"@

$windows = [WinFind]::FindAll()
Write-Output "Found $($windows.Count) window(s)"
foreach ($hwnd in $windows) {
    $sb = New-Object System.Text.StringBuilder(512)
    [WinFind]::GetWindowText($hwnd, $sb, 512) | Out-Null
    $cls = New-Object System.Text.StringBuilder(256)
    [WinFind]::GetClassName($hwnd, $cls, 256) | Out-Null
    $rect = New-Object WinFind+RECT
    [WinFind]::GetWindowRect($hwnd, [ref]$rect) | Out-Null
    $pid = 0
    [WinFind]::GetWindowThreadProcessId($hwnd, [ref]$pid) | Out-Null
    $proc = Get-Process -Id $pid -ErrorAction SilentlyContinue
    Write-Output "---"
    Write-Output "HWND: $hwnd"
    Write-Output "Title: '$($sb.ToString())'"
    Write-Output "Class: '$($cls.ToString())'"
    Write-Output "Process: $($proc.ProcessName) (PID=$pid)"
    Write-Output "Rect: $($rect.Left),$($rect.Top) - $($rect.Right),$($rect.Bottom)"
}
