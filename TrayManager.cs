using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Reflection;
using Microsoft.Win32;

namespace DesktopClock
{
    internal class TrayManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _menu;
        private ClockWindow _window;
        private Settings _settings;
        private Dispatcher _dispatcher;

        private ToolStripMenuItem _moveModeItem;
        private ToolStripMenuItem _startupItem;
        private ToolStripMenuItem _wecomItem;
        private ToolStripMenuItem[] _sizeItems;
        private ToolStripMenuItem[] _colorItems;
        private ToolStripMenuItem[] _opacityItems;
        private ToolStripMenuItem _characterItem;

        private static readonly double[] SIZES = { 28, 40, 56, 72 };
        private static readonly string[] SIZE_LABELS = { "Small 28", "Medium 40", "Large 56", "XLarge 72" };
        private static readonly string[] COLORS = { "#FFFFFF", "#FFD700", "#00FFFF", "#90EE90" };
        private static readonly string[] COLOR_LABELS = { "White", "Yellow", "Cyan", "Green" };
        private static readonly double[] OPACITIES = { 0.5, 0.7, 0.85, 1.0 };
        private static readonly string[] OPACITY_LABELS = { "50%", "70%", "85%", "100%" };

        public TrayManager(ClockWindow window, Settings settings)
        {
            _window = window;
            _settings = settings;
            _dispatcher = window.Dispatcher;

            BuildMenu();

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = CreateClockIcon();
            _notifyIcon.Text = "Desktop Clock";
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenuStrip = _menu;
            _notifyIcon.DoubleClick += OnDoubleClick;

            _window.MoveModeChanged += OnMoveModeChanged;
        }

        private void BuildMenu()
        {
            _menu = new ContextMenuStrip();

            _moveModeItem = new ToolStripMenuItem("Move Mode (Ctrl+Alt+M)");
            _moveModeItem.Click += OnMoveModeClick;
            _menu.Items.Add(_moveModeItem);

            _menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem sizeMenu = new ToolStripMenuItem("Font Size");
            _sizeItems = new ToolStripMenuItem[SIZES.Length];
            for (int i = 0; i < SIZES.Length; i++)
            {
                double size = SIZES[i];
                _sizeItems[i] = new ToolStripMenuItem(SIZE_LABELS[i]);
                _sizeItems[i].Tag = size;
                _sizeItems[i].Click += OnFontSizeClick;
                sizeMenu.DropDownItems.Add(_sizeItems[i]);
            }
            _menu.Items.Add(sizeMenu);

            ToolStripMenuItem colorMenu = new ToolStripMenuItem("Font Color");
            _colorItems = new ToolStripMenuItem[COLORS.Length];
            for (int i = 0; i < COLORS.Length; i++)
            {
                string color = COLORS[i];
                _colorItems[i] = new ToolStripMenuItem(COLOR_LABELS[i]);
                _colorItems[i].Tag = color;
                _colorItems[i].Click += OnColorClick;
                colorMenu.DropDownItems.Add(_colorItems[i]);
            }
            _menu.Items.Add(colorMenu);

            ToolStripMenuItem opacityMenu = new ToolStripMenuItem("Opacity");
            _opacityItems = new ToolStripMenuItem[OPACITIES.Length];
            for (int i = 0; i < OPACITIES.Length; i++)
            {
                double op = OPACITIES[i];
                _opacityItems[i] = new ToolStripMenuItem(OPACITY_LABELS[i]);
                _opacityItems[i].Tag = op;
                _opacityItems[i].Click += OnOpacityClick;
                opacityMenu.DropDownItems.Add(_opacityItems[i]);
            }
            _menu.Items.Add(opacityMenu);

            _menu.Items.Add(new ToolStripSeparator());

            _startupItem = new ToolStripMenuItem("Run at Startup");
            _startupItem.Checked = _settings.RunAtStartup;
            _startupItem.Click += OnStartupClick;
            _menu.Items.Add(_startupItem);

            _wecomItem = new ToolStripMenuItem("监控企业微信");
            _wecomItem.Checked = _settings.MonitorWeCom;
            _wecomItem.Click += OnWeComMonitorClick;
            _menu.Items.Add(_wecomItem);

            _characterItem = new ToolStripMenuItem("显示小新");
            _characterItem.Checked = _settings.ShowCharacter;
            _characterItem.Click += OnCharacterClick;
            _menu.Items.Add(_characterItem);

            _menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += OnExitClick;
            _menu.Items.Add(exitItem);

            UpdateChecks();
        }

        private void UpdateChecks()
        {
            for (int i = 0; i < _sizeItems.Length; i++)
                _sizeItems[i].Checked = (Math.Abs(_settings.FontSize - SIZES[i]) < 0.01);
            for (int i = 0; i < _colorItems.Length; i++)
                _colorItems[i].Checked = (_settings.FontColor == COLORS[i]);
            for (int i = 0; i < _opacityItems.Length; i++)
                _opacityItems[i].Checked = (Math.Abs(_settings.Opacity - OPACITIES[i]) < 0.01);
        }

        private void OnMoveModeClick(object sender, EventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate
            {
                _window.ToggleMoveMode();
            }));
        }

        private void OnMoveModeChanged(object sender, EventArgs e)
        {
            _moveModeItem.Checked = _window.IsInMoveMode;
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate
            {
                _window.ToggleMoveMode();
            }));
        }

        private void OnFontSizeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            _settings.FontSize = (double)item.Tag;
            _settings.Save();
            _dispatcher.Invoke(new Action(delegate
            {
                _window.ApplySettings(_settings);
            }));
            UpdateChecks();
        }

        private void OnColorClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            _settings.FontColor = (string)item.Tag;
            _settings.Save();
            _dispatcher.Invoke(new Action(delegate
            {
                _window.ApplySettings(_settings);
            }));
            UpdateChecks();
        }

        private void OnOpacityClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            _settings.Opacity = (double)item.Tag;
            _settings.Save();
            _dispatcher.Invoke(new Action(delegate
            {
                _window.ApplySettings(_settings);
            }));
            UpdateChecks();
        }

        private void OnStartupClick(object sender, EventArgs e)
        {
            _settings.RunAtStartup = !_settings.RunAtStartup;
            _startupItem.Checked = _settings.RunAtStartup;
            _settings.Save();
            SetAutoStart(_settings.RunAtStartup);
        }

        private void OnWeComMonitorClick(object sender, EventArgs e)
        {
            _settings.MonitorWeCom = !_settings.MonitorWeCom;
            _wecomItem.Checked = _settings.MonitorWeCom;
            _settings.Save();
            _dispatcher.Invoke(new Action(delegate
            {
                _window.SetWeComMonitoringEnabled(_settings.MonitorWeCom);
            }));
        }

        private void OnCharacterClick(object sender, EventArgs e)
        {
            _settings.ShowCharacter = !_settings.ShowCharacter;
            _characterItem.Checked = _settings.ShowCharacter;
            _settings.Save();
            _dispatcher.Invoke(new Action(delegate
            {
                _window.SetCharacterVisible(_settings.ShowCharacter);
            }));
        }

        private void SetAutoStart(bool enable)
        {
            try
            {
                string keyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                if (key == null) return;
                if (enable)
                {
                    string exePath = Assembly.GetExecutingAssembly().Location;
                    key.SetValue("DesktopClock", "\"" + exePath + "\"");
                }
                else
                {
                    key.DeleteValue("DesktopClock", false);
                }
                key.Close();
            }
            catch { }
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            _window.ExitMoveMode();
            _settings.Save();
            if (_notifyIcon != null)
                _notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        private static Icon CreateClockIcon()
        {
            Bitmap bmp = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using (Pen pen = new Pen(Color.White, 2.5f))
            {
                g.DrawEllipse(pen, 3, 3, 26, 26);
                g.DrawLine(pen, 16, 16, 16, 8);
                g.DrawLine(pen, 16, 16, 22, 16);
            }
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, 14, 14, 4, 4);
            }
            g.Dispose();

            IntPtr hIcon = bmp.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return icon;
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            if (_menu != null)
                _menu.Dispose();
        }
    }
}
