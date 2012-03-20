using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace FreeFoodButton
{
    sealed class KeyboardManager
    {
        #region Private Members

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBHookStruct lParam);
        private static HookHandlerDelegate callbackPtr;
        private static IntPtr hookPtr = IntPtr.Zero;
        private const int LowLevelKeyboardHook = 13;
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate callbackPtr, IntPtr hInstance, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBHookStruct lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        #endregion

        public KeyboardManager() { }

        public void DisableSystemKeys()
        {
            if (callbackPtr == null)
            {
                callbackPtr = new HookHandlerDelegate(KeyboardHookHandler);
            }

            if (hookPtr == IntPtr.Zero)
            {
                // Note: This does not work in the VS host environment.  To run in debug mode:
                // Project -> Properties -> Debug -> Uncheck "Enable the Visual Studio hosting process"
                IntPtr hInstance = Marshal.GetHINSTANCE(Application.Current.GetType().Module);
                hookPtr = SetWindowsHookEx(LowLevelKeyboardHook, callbackPtr, hInstance, 0);
            }
        }

        public void EnableSystemKeys()
        {
            if (hookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookPtr);
                hookPtr = IntPtr.Zero;
            }
        }

        public event EventHandler KeyPressed;

        public void OnKeyPressed(EventArgs e)
        {
            EventHandler handler = KeyPressed;
            if (handler != null) handler(null, e);
        }

        private IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, ref KBHookStruct lParam)
        {
            if (nCode == 0)
            {
                if (
                ((lParam.vkCode == 0x5B) && (lParam.flags == 0x01)) ||      // Left Windows Key
                ((lParam.vkCode == 0x5C) && (lParam.flags == 0x01))       // Right Windows Key
                )      
                {
                    //compress the image
                    Image food = Image.FromFile(MainWindow.m_filename);
                    Bitmap resized = ResizeImage(food, 320, 240);
                    SaveJpeg(Email.EMAIL_IMAGE_PATH, (Image)resized, 90);

                    //send the email
                    Email.SendEmail(Settings.Email, "Free Food", Settings.Message);
                    OnKeyPressed(new EventArgs());
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(hookPtr, nCode, wParam, ref lParam);
        }

        public static void ActivateWindow(Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var currentForegroundWindow = GetForegroundWindow();
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);
            SetWindowPos(interopHelper.Handle, new IntPtr(0), 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
            window.Show();
            window.Activate();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }


        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid value was entered for image quality.
        /// </exception>
        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            //find the encoder with the image/jpeg mime-type
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici=null;
            foreach(ImageCodecInfo codec in codecs)
            {
                if(codec.MimeType=="image/jpeg")
                    ici=codec;
            }

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters();
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, ici, encoderParams);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }
}
