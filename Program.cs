using System;
using System.Windows;

namespace DesktopClock
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Settings settings = Settings.Load();

            Application app = new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ClockWindow window = new ClockWindow(settings);
            window.Show();

            TrayManager tray = new TrayManager(window, settings);

            app.Exit += delegate(object s, ExitEventArgs e)
            {
                tray.Dispose();
                settings.Save();
            };

            app.Run();
        }
    }
}
