using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace DesktopClock
{
    internal class ClockWindow : Window
    {
        private TextBlock _timeText;
        private Border _border;
        private DispatcherTimer _timer;
        private Settings _settings;
        private IntPtr _hwnd = IntPtr.Zero;
        private bool _isInMoveMode = false;
        private string _lastText = "";

        public bool IsInMoveMode { get { return _isInMoveMode; } }
        public event EventHandler MoveModeChanged;

        public ClockWindow(Settings settings)
        {
            _settings = settings;

            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            Topmost = true;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Opacity = _settings.Opacity;

            _border = new Border();
            _border.CornerRadius = new CornerRadius(8);
            _border.BorderBrush = new SolidColorBrush(Color.FromArgb(160, 100, 200, 255));
            _border.BorderThickness = new Thickness(0);
            _border.Background = Brushes.Transparent;
            _border.Padding = new Thickness(12, 4, 12, 4);

            _timeText = new TextBlock();
            _timeText.FontFamily = new FontFamily("Segoe UI");
            _timeText.FontSize = _settings.FontSize;
            _timeText.FontWeight = FontWeights.SemiBold;
            _timeText.Foreground = CreateBrush(_settings.FontColor);
            _timeText.HorizontalAlignment = HorizontalAlignment.Center;
            _timeText.VerticalAlignment = VerticalAlignment.Center;
            _timeText.Effect = new DropShadowEffect
            {
                BlurRadius = 10,
                ShadowDepth = 0,
                Color = Colors.Black,
                Opacity = 0.8
            };

            _border.Child = _timeText;
            this.Content = _border;

            SetPosition();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();

            UpdateTime();
        }

        private SolidColorBrush CreateBrush(string colorHex)
        {
            try
            {
                Color c = (Color)ColorConverter.ConvertFromString(colorHex);
                return new SolidColorBrush(c);
            }
            catch
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        private void SetPosition()
        {
            if (_settings.Left >= 0 && _settings.Top >= 0)
            {
                Left = _settings.Left;
                Top = _settings.Top;
            }
            else
            {
                double w = SystemParameters.WorkArea.Width;
                double h = SystemParameters.WorkArea.Height;
                Left = w - 180;
                Top = h - 90;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hwnd = new WindowInteropHelper(this).Handle;
            Win32.SetClickThrough(_hwnd, true);
            Win32.RegisterHotKey(_hwnd, 1,
                Win32.MOD_CONTROL | Win32.MOD_ALT | Win32.MOD_NOREPEAT,
                Win32.VK_M);
            HwndSource source = HwndSource.FromHwnd(_hwnd);
            if (source != null)
                source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32.WM_HOTKEY && wParam.ToInt32() == 1)
            {
                ToggleMoveMode();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            string format = _settings.Use24Hour ? "HH:mm" : "hh:mm";
            string text = DateTime.Now.ToString(format);
            if (text != _lastText)
            {
                _timeText.Text = text;
                _lastText = text;
            }
        }

        public void ApplySettings(Settings settings)
        {
            _settings = settings;
            _timeText.FontSize = settings.FontSize;
            _timeText.Foreground = CreateBrush(settings.FontColor);
            Opacity = settings.Opacity;
            UpdateTime();
        }

        public void ToggleMoveMode()
        {
            if (_isInMoveMode)
                ExitMoveMode();
            else
                EnterMoveMode();
        }

        private void EnterMoveMode()
        {
            _isInMoveMode = true;
            if (_hwnd != IntPtr.Zero)
                Win32.SetClickThrough(_hwnd, false);
            _border.BorderThickness = new Thickness(1);
            if (MoveModeChanged != null)
                MoveModeChanged(this, EventArgs.Empty);
        }

        public void ExitMoveMode()
        {
            if (!_isInMoveMode) return;
            _isInMoveMode = false;
            if (_hwnd != IntPtr.Zero)
                Win32.SetClickThrough(_hwnd, true);
            _border.BorderThickness = new Thickness(0);
            _settings.Left = Left;
            _settings.Top = Top;
            _settings.Save();
            if (MoveModeChanged != null)
                MoveModeChanged(this, EventArgs.Empty);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (_isInMoveMode)
            {
                DragMove();
                e.Handled = true;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isInMoveMode)
            {
                ExitMoveMode();
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_timer != null)
                _timer.Stop();
            if (_hwnd != IntPtr.Zero)
                Win32.UnregisterHotKey(_hwnd, 1);
            base.OnClosed(e);
        }
    }
}
