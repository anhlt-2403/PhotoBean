using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoBeanApp.Helper.UserControls
{
    /// <summary>
    /// Interaction logic for CustomButton.xaml
    /// </summary>
    public partial class CustomButton : UserControl
    {
        public CustomButton()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked position relative to the Image
            Point clickedPoint = e.GetPosition(sender as Image);

            // Convert the position to pixel coordinates
            int x = (int)clickedPoint.X;
            int y = (int)clickedPoint.Y;

            // Get the ImageSource
            BitmapSource bitmapSource = ((Image)sender).Source as BitmapSource;

            // Create a drawing visual to render the image
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Draw the image
                drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight));
            }

            // Render the visual to a bitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(drawingVisual);

            // Get the pixel color at the clicked position
            byte[] pixel = new byte[4];
            renderTargetBitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            // Check if the alpha (transparency) value is zero
            bool isTransparent = pixel[3] == 0;

            // Now you know whether the clicked position is transparent or not
            if (isTransparent)
            {
                MessageBox.Show("Transparent pixel clicked.");
            }
            else
            {
                MessageBox.Show("Non-transparent pixel clicked.");
            }
        }
    }
}
