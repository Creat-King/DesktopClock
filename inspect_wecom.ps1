Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;
public class WinAuto {
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    public static AutomationElement FindWeCom() {
        IntPtr hwnd = FindWindow("WeWorkWindow", null);
        if (hwnd == IntPtr.Zero) return null;
        return AutomationElement.FromHandle(hwnd);
    }
}
"@

$root = [WinAuto]::FindWeCom()
if ($root -eq $null) { Write-Output "WeChat Work window not found"; exit }

Write-Output "Window: $($root.Current.Name)"
Write-Output "Class: $($root.Current.ClassName)"
Write-Output "ControlType: $($root.Current.ControlType.ProgrammaticName)"
Write-Output ""

# Walk the tree to find text elements and buttons
$walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
function Walk-Tree($element, $depth) {
    if ($depth -gt 4) { return }
    $name = $element.Current.Name
    $cls = $element.Current.ClassName
    $type = $element.Current.ControlType.ProgrammaticName
    if ($name -or $cls) {
        $indent = "  " * $depth
        Write-Output "${indent}[$type] Name='$name' Class='$cls'"
    }
    $child = $walker.GetFirstChild($element)
    while ($child -ne $null) {
        Walk-Tree $child ($depth + 1)
        $child = $walker.GetNextSibling($child)
    }
}

Write-Output "=== UI Tree (top 4 levels) ==="
Walk-Tree $root 0
