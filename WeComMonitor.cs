using System;
using System.Text;
using System.Windows.Threading;

namespace DesktopClock
{
    internal class WeComMonitor
    {
        private DispatcherTimer _timer;
        private bool _hasUnreadMessages = false;

        public bool HasUnreadMessages { get { return _hasUnreadMessages; } }
        public event EventHandler StateChanged;

        public WeComMonitor()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        public void CheckNow()
        {
            CheckForeground();
        }

        public void HandleShellHookEvent(IntPtr wParam, IntPtr lParam)
        {
            int eventType = wParam.ToInt32();
            if (eventType == (int)Win32.HSHELL_FLASH && IsWeComWindow(lParam))
            {
                SetState(true);
            }
            else if (eventType == (int)Win32.HSHELL_WINDOWACTIVATED && IsWeComWindow(lParam))
            {
                SetState(false);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            CheckForeground();
        }

        private void CheckForeground()
        {
            IntPtr hwnd = Win32.FindWindow("WeWorkWindow", null);
            if (hwnd == IntPtr.Zero)
            {
                SetState(false);
                return;
            }

            if (Win32.GetForegroundWindow() == hwnd)
            {
                SetState(false);
            }
        }

        private bool IsWeComWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;
            StringBuilder sb = new StringBuilder(256);
            Win32.GetClassName(hwnd, sb, 256);
            return sb.ToString() == "WeWorkWindow";
        }

        private void SetState(bool hasMessages)
        {
            if (hasMessages != _hasUnreadMessages)
            {
                _hasUnreadMessages = hasMessages;
                if (StateChanged != null)
                    StateChanged(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            if (_timer != null)
                _timer.Stop();
        }
    }
}
