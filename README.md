# DesktopClock - 透明桌面时钟

> 一个轻量级 Windows 桌面时钟，透明显示、点击穿透、不遮挡任何界面内容。支持企业微信消息提醒监控。

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)]()
[![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-purple.svg)]()
[![Size](https://img.shields.io/badge/Size-26KB-green.svg)]()

**English** | [中文](#中文说明)

---

## A truly invisible desktop clock

DesktopClock stays on top of all windows as a transparent time display. Mouse clicks pass straight through it to whatever is underneath. It won't appear in Alt+Tab, the taskbar, or any window switcher. Just a clean, always-visible time display floating on your screen.

### Why DesktopClock?

- **Zero dependencies** — Runs on any Windows 10/11 machine with .NET Framework 4.8 (pre-installed). No SDK, no runtime download, no installer.
- **26KB executable** — Smaller than most favicon files.
- **Real click-through** — Not a semi-transparent overlay. Mouse events pass through via `WS_EX_TRANSPARENT`, letting you interact with windows below.
- **Alt+Tab invisible** — Uses `WS_EX_TOOLWINDOW` to hide from the task switcher entirely.
- **No IDE required** — Pure C# code, no XAML. Compiled with the built-in `csc.exe`. Just run `build.bat`.
- **System tray integration** — Right-click the tray icon to change font size, color, opacity, enable auto-start.
- **Global hotkey** — Press `Ctrl+Alt+M` to enter move mode, drag the clock anywhere, press `Esc` to lock it in place.
- **Persistent settings** — Position and style are saved automatically.
- **WeCom message monitor** — A green/red indicator dot on the clock edge turns red and pulses when WeCom (企业微信) receives new messages. Uses Windows Shell Hook (`HSHELL_FLASH`) for real-time, event-driven detection — no polling, no false positives from other apps.
- **WeCom message monitor** — Green/red indicator dot on the clock edge. Flashes red when WeChat Work (企业微信) receives a new message, turns green when you open it. Powered by Shell Hook (`HSHELL_FLASH`), event-driven with zero polling overhead.
- **Animated character** — A cute animated cartoon character (Crayon Shin-chan) displayed above the clock. Toggle show/hide from the tray menu.

---

## 中文说明

一个真正「不挡手」的 Windows 桌面时钟。时间始终悬浮在所有窗口之上，但鼠标可以完全穿透它操作下方的界面。不出现在 Alt+Tab、不出现在任务栏、不干扰任何操作。支持企业微信新消息提醒监控。

### 核心特性

| 特性 | 说明 |
|------|------|
| 真正的点击穿透 | 通过 Win32 API `WS_EX_TRANSPARENT` 实现完全鼠标穿透，不是半透明遮罩 |
| Alt+Tab 隐身 | 使用 `WS_EX_TOOLWINDOW` 样式，不出现在任务切换器中 |
| 零依赖 | Windows 10/11 自带 .NET Framework 4.8，无需安装任何运行时或 SDK |
| 极小体积 | 编译后仅 26KB |
| 无 XAML 纯代码 | 全部用 C# 构建 UI，系统自带 `csc.exe` 直接编译，不需要 Visual Studio |
| 系统托盘 | 右键图标即可配置字体大小/颜色/透明度/开机自启 |
| 全局热键 | `Ctrl+Alt+M` 切换移动模式，拖动到任意位置 |
| 配置持久化 | 位置和样式自动保存，下次启动恢复 |
| 企业微信消息监控 | 时钟边缘的指示灯：无消息绿色常亮，有新消息红色闪烁。通过 Shell Hook 实时检测任务栏闪烁，仅响应企业微信，不影响其他软件 |
| 蜡笔小新动画 | 时钟上方显示可爱的蜡笔小新动画形象，可通过托盘菜单显示/隐藏 |

### 快速开始

1. 下载 `DesktopClock.exe`
2. 双击运行
3. 时钟出现在屏幕右下角，白色文字悬浮显示，左侧绿色指示灯表示企业微信监控已启用

就这样。不需要安装，不需要配置。

### 从源码编译

```bash
# 不需要 Visual Studio、不需要 .NET SDK
# 只需要 Windows 自带的 csc.exe
双击 build.bat
```

或手动编译：

```bash
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
  /target:winexe /out:DesktopClock.exe /platform:x64 ^
  /reference:PresentationCore.dll ^
  /reference:PresentationFramework.dll ^
  /reference:WindowsBase.dll ^
  /reference:System.Windows.Forms.dll ^
  /reference:System.Drawing.dll ^
  Program.cs ClockWindow.cs GifImage.cs TrayManager.cs Win32.cs Settings.cs WeComMonitor.cs
```

### 使用方法

| 操作 | 方式 |
|------|------|
| 移动时钟位置 | `Ctrl+Alt+M` 进入移动模式 → 鼠标拖动 → `Esc` 确认 |
| 打开设置菜单 | 右键系统托盘图标 |
| 调整字体大小 | 托盘菜单 → 字体大小 → 小/中/大/超大 |
| 调整颜色 | 托盘菜单 → 字体颜色 → 白色/浅黄/青色/浅绿 |
| 调整透明度 | 托盘菜单 → 透明度 → 50%/70%/85%/100% |
| 开机自启 | 托盘菜单 → 开机自启 |
| 开关企业微信监控 | 托盘菜单 → 监控企业微信 |
| 显示/隐藏小新 | 托盘菜单 → 显示小新 |
| 退出 | 托盘菜单 → 退出 |

所有设置自动保存到 `%AppData%\DesktopClock\settings.txt`。

### 企业微信消息监控

时钟左侧有一个小圆点指示灯：

- **绿色常亮** — 企业微信无新消息
- **红色闪烁** — 企业微信收到新消息（任务栏闪烁时触发）
- 打开企业微信窗口后自动恢复绿色

**检测原理：** 通过 `RegisterShellHookWindow` 注册系统级 Shell Hook，当任意窗口闪烁任务栏时收到 `HSHELL_FLASH` 事件。检查闪烁窗口的类名是否为 `WeWorkWindow`（企业微信主窗口），只有匹配才亮红灯。其他软件（微信、钉钉、QQ 等）的闪烁不会触发，不会造成干扰。

### 技术实现

**三重窗口样式实现透明+穿透+隐身：**

```
WS_EX_LAYERED     → 支持透明合成（WPF AllowsTransparency 底层机制）
WS_EX_TRANSPARENT  → 鼠标事件穿透到下方窗口
WS_EX_TOOLWINDOW   → 不出现在 Alt+Tab 和任务栏中
```

**关键技术点：**

- WPF 透明窗口 + Win32 API 互操作（`SetWindowLongPtr` 设置扩展样式）
- `DispatcherTimer` 每秒刷新，仅在分钟变化时更新 UI
- WinForms `NotifyIcon` 在 WPF 中的混合使用（无需外部 NuGet 包）
- `RegisterHotKey` 全局热键 + `HwndSource.AddHook` 在 WPF 中接收 WM_HOTKEY
- 程序化绘制托盘图标（`Graphics` + `GetHicon`），不依赖外部 .ico 文件
- 注册表 `HKCU\...\Run` 实现开机自启
- 轻量 key=value 配置文件，无 JSON 库依赖
- **Shell Hook 消息监控** — `RegisterShellHookWindow` + `RegisterWindowMessage("SHELLHOOK")` 实时监听任务栏闪烁事件，通过 `HSHELL_FLASH` + `GetClassName` 精确过滤企业微信窗口
- **WPF 动画** — `DoubleAnimation` 实现指示灯红色脉冲呼吸效果

### 项目结构

```
DesktopClock/
├── Program.cs          # 程序入口，创建 WPF Application + 托盘管理
├── ClockWindow.cs      # 透明置顶窗口 + 时钟显示 + 指示灯 + 穿透/拖动逻辑
├── GifImage.cs         # GIF 动画播放（BitmapDecoder 帧提取 + DispatcherTimer）
├── character.gif       # 蜡笔小新动画 GIF
├── WeComMonitor.cs     # 企业微信消息监控（Shell Hook 事件 + 前台检测）
├── TrayManager.cs      # 系统托盘图标 + 右键菜单 + 热键
├── Win32.cs            # P/Invoke：窗口样式、热键、Shell Hook、窗口查找
├── Settings.cs         # 配置模型 + 加载/保存
├── build.bat           # 一键编译脚本
├── DesktopClock.exe    # 编译好的可执行文件（26KB）
└── README.md
```

### 系统要求

- Windows 10/11（64 位）
- .NET Framework 4.8（Windows 10 1903+ / Windows 11 预装）

### License

[MIT](LICENSE)
