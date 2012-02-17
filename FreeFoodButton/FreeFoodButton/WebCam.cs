using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCam_Capture;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FreeFoodButton
{
    class WebCam
    {
        //Block Memory Leak
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        string m_Filename = "TestImage.jpg";

        private WebCamCapture webcam;
        private int FrameNumber = 30;        
        BitmapSource mBitmapSource;

        public void Start()
        {
            webcam = new WebCamCapture();
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
            webcam.Start(0);

            Log.Write("camera started.");
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            IntPtr ip = ((System.Drawing.Bitmap)e.WebCamImage).GetHbitmap();
            mBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ip);         
        }

        public void SaveImage()
        {
            try
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(mBitmapSource));
                encoder.QualityLevel = 100;

                FileStream fstream = new FileStream(m_Filename, FileMode.Create);
                encoder.Save(fstream);
                fstream.Close();
            }
            catch 
            {
                Log.Write("Error Saving picture");
            }

        }

        public void Stop()
        {
            webcam.Stop();
            Log.Write("camera stopped.");
        }

        public string FileName
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }

    }
}
