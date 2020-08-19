using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AutomatedTestingLib {

    /// <summary>
    /// Used to capture screenshots of the active window or of the whole desktop, then saves it to the configured directory
    /// </summary>
    public class ScreenshotHandler {

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        /// <summary>
        /// Captures the desktop, saves it as a PNG, then returns the path string
        /// </summary>
        public static string CaptureDesktop(string testName) {
            string dateAndTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "__"
                + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();

            //creates file, stores screenshot in path
            string path = Path.Combine(ConfigurationManager.AppSettings.Get("AutomationScreenshots"), testName + "_" + dateAndTime);

            var img = CaptureWindow(GetDesktopWindow());
            img.Save(path + ".PNG", ImageFormat.Png);
            return path + ".PNG";
        }
        /// <summary>
        /// Captures the active, top window, saves it as a PNG, then returns the path string
        /// </summary>
        public static string CaptureActiveWindow(string testName) {
            string dateAndTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "__"
                + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();

            //creates file, stores screenshot in path
            string path = Path.Combine(ConfigurationManager.AppSettings.Get("AutomationScreenshots"), testName + "_" + dateAndTime);

            var img = CaptureWindow(GetForegroundWindow());
            img.Save(path + ".PNG", ImageFormat.Png);
            return path + ".PNG";
        }

        private static Bitmap CaptureWindow(IntPtr handle) {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result)) {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }
}
