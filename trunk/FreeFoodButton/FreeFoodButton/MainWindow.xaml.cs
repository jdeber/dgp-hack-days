using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace FreeFoodButton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_SYSCOMMAND     = 0x112;
        private const int SC_SCREENSAVE     = 0xF140;
        private const int SC_MONITORPOWER   = 0xF170;
        private WebCam webcam;
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            SourceInitialized += delegate
            {
                HwndSource source = (HwndSource) PresentationSource.FromVisual(this);
                source.AddHook(ScreenSaverHook);
            };

            InitializeComponent();

            //load default settings
            Settings.LoadSettings("Settings.txt");
            //load email settings
            Settings.LoadSettings("EmailSettings.txt");
            //name log file
            Log.FileName = "log.txt";

            Log.Write("Program Starting", Log.Categories.System);

            //Web cam code
            webcam = new WebCam();
            webcam.Start();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();
        }

        private static IntPtr ScreenSaverHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND)
            {
                if (((long)wParam & 0xFFF0) == SC_SCREENSAVE || ((long)wParam & 0xFFF0) == SC_MONITORPOWER)
                    handled = true;
            }
            return IntPtr.Zero;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            webcam.SaveImage();
        }

        protected override void OnClosed(EventArgs e)
        {
            webcam.Stop();
            dispatcherTimer.Stop();
            base.OnClosed(e);
        }
    }

    internal static class NativeMethods
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
    }
}
