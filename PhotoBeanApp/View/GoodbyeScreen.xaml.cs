using FrameLib.Drive;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for GoodbyeScreen.xaml
    /// </summary>
    public partial class GoodbyeScreen : UserControl
    {
        public event EventHandler ButtonContinueClick;
        private Bitmap _photo;
        public GoodbyeScreen(Bitmap photo)
        {
            InitializeComponent();
            _photo = photo;
            Print.Source = ConvertToBitmapSource(photo);
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string driveDirectory = Path.Combine(projectDirectory, "DriveImage");
            string jsonDirectory = Path.Combine(projectDirectory, "Credentials.json");
            string url = GoogleDrive.DriveUploadToFolder(jsonDirectory, driveDirectory + "\\temp.jpg", "1SXxG-xdh1m9-0vwW5PYIU5ayTl-61CBF");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImageBitmap = qrCode.GetGraphic(20);
            QRcodeImage.Source = BitmapToImageSource(qrCodeImageBitmap);
        }
        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {

            ButtonContinueClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
