using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using QRCoder;

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for PaymentScreen.xaml
    /// </summary>
    public partial class PaymentScreen : UserControl
    {
        public event EventHandler PaymentSuccess;
        public int numberOfCut, numberOfPrint;

        public PaymentScreen(int numberOfCut, int numberOfPrint)
        {
            InitializeComponent();
            this.numberOfCut = numberOfCut;
            this.numberOfPrint = numberOfPrint;
            LoadPayment();
        }
        public void LoadPayment()
        {
            cutLabel.Content = $"Số lượng tấm ảnh: {numberOfCut}";
            printLabel.Content = $"Số lượng bản in: {numberOfPrint}";
            totalLabel.Content = $"Tổng tiền: {50000 * numberOfPrint}";

            string paymentData = "Tuấn Anh đẹp trai =)))";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentData, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImageBitmap = qrCode.GetGraphic(20);

            qrCodeImage.Source = BitmapToImageSource(qrCodeImageBitmap);

            qrCodeImage.MouseLeftButtonDown += (sender, e) =>
            {
                MessageBox.Show("Thanh toán thành công");
                PaymentSuccess?.Invoke(this, EventArgs.Empty);
            };
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
