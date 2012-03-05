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
using System.Collections.ObjectModel;
using System.IO;

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
        private DispatcherTimer dispatcherTimer;
        private string m_filename = "C:\\inetpub\\wwwroot\\FreeFood\\Food.jpg";

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
            // Create default device
            SelectedWebcamMonikerString = (CapDevice.DeviceMonikers.Length > 0) ? CapDevice.DeviceMonikers[0].MonikerString : "";

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();           
        }

        #region Properties
        /// <summary>
        /// Wrapper for the WebcamRotation dependency property
        /// </summary>
        public double WebcamRotation
        {
            get { return (double)GetValue(WebcamRotationProperty); }
            set { SetValue(WebcamRotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WebcamRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WebcamRotationProperty =
            DependencyProperty.Register("WebcamRotation", typeof(double), typeof(MainWindow), new UIPropertyMetadata(180d));

        /// <summary>
        /// Wrapper for the SelectedWebcam dependency property
        /// </summary>
        public CapDevice SelectedWebcam
        {
            get { return (CapDevice)GetValue(SelectedWebcamProperty); }
            set { SetValue(SelectedWebcamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedWebcam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedWebcamProperty =
            DependencyProperty.Register("SelectedWebcam", typeof(CapDevice), typeof(MainWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Wrapper for the SelectedWebcamMonikerString dependency property
        /// </summary>
        public string SelectedWebcamMonikerString
        {
            get { return (string)GetValue(SelectedWebcamMonikerStringProperty); }
            set { SetValue(SelectedWebcamMonikerStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedWebcamMonikerString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedWebcamMonikerStringProperty = DependencyProperty.Register("SelectedWebcamMonikerString", typeof(string),
            typeof(MainWindow), new UIPropertyMetadata("", new PropertyChangedCallback(SelectedWebcamMonikerString_Changed)));

        /// <summary>
        /// Wrapper for the SelectedImages dependency property
        /// </summary>
        public ObservableCollection<BitmapSource> SelectedImages
        {
            get { return (ObservableCollection<BitmapSource>)GetValue(SelectedImagesProperty); }
            set { SetValue(SelectedImagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedImages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImagesProperty = DependencyProperty.Register("SelectedImages", typeof(ObservableCollection<BitmapSource>),
            typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<BitmapSource>()));
        #endregion

        #region Methods
        /// <summary>
        /// Invoked when the SelectedWebcamMonikerString dependency property has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void SelectedWebcamMonikerString_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get typed sender
            MainWindow typedSender = sender as MainWindow;
            if (typedSender != null)
            {
                // Get new value
                string newMonikerString = e.NewValue as string;

                // Update the device
                if (typedSender.SelectedWebcam == null)
                {
                    // Create it
                    typedSender.SelectedWebcam = new CapDevice("");
                }

                // Now set the moniker string
                typedSender.SelectedWebcam.MonikerString = newMonikerString;
            }
        }
        #endregion

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
            using (var fileStream = new FileStream(m_filename, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.FlipHorizontal = true; 
                encoder.Frames.Add(BitmapFrame.Create(player.CurrentBitmap));
                encoder.Save(fileStream);
                encoder = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            //webcam.Stop();
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

    public class ScaleConverter : IMultiValueConverter
    {


        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float v = (float)values[0];
            double m = (double)values[1];
            return v * m / 50;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
