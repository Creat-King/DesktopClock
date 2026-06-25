using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DesktopClock
{
    internal class GifImage : Image
    {
        private BitmapFrame[] _frames;
        private double[] _frameDelays;
        private DispatcherTimer _timer;
        private int _currentFrame = 0;
        private const double SlowdownFactor = 1;

        public GifImage(string gifPath)
        {
            Width = 50;
            Height = 60;
            Stretch = System.Windows.Media.Stretch.Uniform;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            if (!File.Exists(gifPath))
                return;

            try
            {
                BitmapDecoder decoder = BitmapDecoder.Create(
                    new Uri(gifPath),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                int count = decoder.Frames.Count;
                _frames = new BitmapFrame[count];
                _frameDelays = new double[count];
                decoder.Frames.CopyTo(_frames, 0);

                for (int i = 0; i < count; i++)
                {
                    _frameDelays[i] = ReadFrameDelay(_frames[i]);
                }

                if (count > 1)
                {
                    Source = _frames[0];
                    _timer = new DispatcherTimer();
                    _timer.Tick += OnTimerTick;
                    SetTimerInterval();
                    _timer.Start();
                }
                else if (count == 1)
                {
                    Source = _frames[0];
                }
            }
            catch
            {
            }
        }

        private static double ReadFrameDelay(BitmapFrame frame)
        {
            double delayMs = 100;
            try
            {
                BitmapMetadata metadata = frame.Metadata as BitmapMetadata;
                if (metadata != null)
                {
                    object delayValue = metadata.GetQuery("/grctlext/Delay");
                    if (delayValue is short)
                        delayMs = (short)delayValue * 10.0;
                    else if (delayValue is ushort)
                        delayMs = (ushort)delayValue * 10.0;
                }
            }
            catch
            {
            }
            delayMs *= SlowdownFactor;
            if (delayMs < 100)
                delayMs = 200;
            return delayMs;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _currentFrame = (_currentFrame + 1) % _frames.Length;
            Source = _frames[_currentFrame];
            SetTimerInterval();
        }

        private void SetTimerInterval()
        {
            double delayMs = _frameDelays[_currentFrame];
            _timer.Interval = TimeSpan.FromMilliseconds(delayMs);
        }

        public void Stop()
        {
            if (_timer != null)
                _timer.Stop();
        }

        public static string GetGifPath()
        {
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exeDir, "character.gif");
        }
    }
}
