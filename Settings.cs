using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace DesktopClock
{
    internal class Settings
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double FontSize { get; set; }
        public string FontColor { get; set; }
        public double Opacity { get; set; }
        public bool RunAtStartup { get; set; }
        public bool Use24Hour { get; set; }
        public bool MonitorWeCom { get; set; }
        public bool IsFirstRun { get; private set; }

        private static string SettingsDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DesktopClock");
            }
        }

        private static string SettingsPath
        {
            get { return Path.Combine(SettingsDir, "settings.txt"); }
        }

        public Settings()
        {
            Left = -1;
            Top = -1;
            FontSize = 40;
            FontColor = "#FFFFFF";
            Opacity = 0.85;
            RunAtStartup = false;
            Use24Hour = true;
            MonitorWeCom = true;
            IsFirstRun = true;
        }

        public static Settings Load()
        {
            Settings s = new Settings();
            try
            {
                if (File.Exists(SettingsPath))
                {
                    s.IsFirstRun = false;
                    string[] lines = File.ReadAllLines(SettingsPath);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (string line in lines)
                    {
                        int idx = line.IndexOf('=');
                        if (idx > 0)
                        {
                            string key = line.Substring(0, idx).Trim();
                            string val = line.Substring(idx + 1).Trim();
                            dict[key] = val;
                        }
                    }
                    if (dict.ContainsKey("Left")) s.Left = ParseDouble(dict["Left"], -1);
                    if (dict.ContainsKey("Top")) s.Top = ParseDouble(dict["Top"], -1);
                    if (dict.ContainsKey("FontSize")) s.FontSize = ParseDouble(dict["FontSize"], 40);
                    if (dict.ContainsKey("FontColor")) s.FontColor = dict["FontColor"];
                    if (dict.ContainsKey("Opacity")) s.Opacity = ParseDouble(dict["Opacity"], 0.85);
                    if (dict.ContainsKey("RunAtStartup")) s.RunAtStartup = ParseBool(dict["RunAtStartup"], false);
                    if (dict.ContainsKey("Use24Hour")) s.Use24Hour = ParseBool(dict["Use24Hour"], true);
                    if (dict.ContainsKey("MonitorWeCom")) s.MonitorWeCom = ParseBool(dict["MonitorWeCom"], true);
                }
            }
            catch { }
            return s;
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(SettingsDir))
                    Directory.CreateDirectory(SettingsDir);
                List<string> lines = new List<string>();
                lines.Add("Left=" + Left.ToString(CultureInfo.InvariantCulture));
                lines.Add("Top=" + Top.ToString(CultureInfo.InvariantCulture));
                lines.Add("FontSize=" + FontSize.ToString(CultureInfo.InvariantCulture));
                lines.Add("FontColor=" + FontColor);
                lines.Add("Opacity=" + Opacity.ToString(CultureInfo.InvariantCulture));
                lines.Add("RunAtStartup=" + RunAtStartup.ToString());
                lines.Add("Use24Hour=" + Use24Hour.ToString());
                lines.Add("MonitorWeCom=" + MonitorWeCom.ToString());
                File.WriteAllLines(SettingsPath, lines);
            }
            catch { }
        }

        private static double ParseDouble(string s, double def)
        {
            double r;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out r))
                return r;
            return def;
        }

        private static bool ParseBool(string s, bool def)
        {
            bool r;
            if (bool.TryParse(s, out r)) return r;
            return def;
        }
    }
}
