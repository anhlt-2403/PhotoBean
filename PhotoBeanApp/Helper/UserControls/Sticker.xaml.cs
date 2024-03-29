using FrameLib.Frame;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FrameLib.Frame;

namespace WPFStickerDemo
{
    /// <summary>
    /// Interaction logic for Sticker.xaml
    /// </summary>
    public partial class Sticker : UserControl
    {
        public IconInImage StickerInfo { get; set; }
        public Sticker()
        {
            InitializeComponent();
        }


        public void SetImageSource(BitmapImage source)
        {
            stickerImage.Source = source;
            stickerImage.Loaded += StickerImage_Loaded;
        }

        private void StickerImage_Loaded(object sender, RoutedEventArgs e)
        {
            // Adjust sticker size to match the size of the stickerImage
            sticker.Width = stickerImage.ActualWidth;
            sticker.Height = stickerImage.ActualHeight;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sticker, new DataObject(DataFormats.Serializable, sticker), DragDropEffects.Move);
            }
        }
    }
}