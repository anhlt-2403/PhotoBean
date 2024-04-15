using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FrameLib.Frame;
using FrameLib.Render;
using WPFStickerDemo;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using PhotoBeanApp.Helper.Classes;

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for StickerScreen.xaml
    /// </summary>
    public partial class StickerScreen : UserControl
    {
        public event EventHandler ButtonContinueClick;
        private List<IconInImage> _stickerList = new List<IconInImage>();
        public Bitmap imgTemp { get; set; }
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory;
        public StickerScreen(Bitmap photo)
        {
            InitializeComponent();

            projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string stickerDirectory = Path.Combine(projectDirectory, $"Helper\\Stickers");
            LoadSticker(stickerDirectory);
            Bin.Visibility = Visibility.Hidden;
            Photo.Loaded += Photo_Loaded;
            Photo.Source = ConvertToBitmapSource(photo);
        }
        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private void Photo_Loaded(object sender, RoutedEventArgs e)
        {
            double imageWidth = Photo.ActualWidth;
            double imageHeight = Photo.ActualHeight;

            canvasSticker.Width = imageWidth;
            canvasSticker.Height = imageHeight;
        }

        private void LoadSticker(string folderName)
        {
            System.Windows.Controls.Image image;
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"Folder '{folderName}' not found.");
                return;
            }

            string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

            foreach (string imagePath in imageFiles)
            {
                BitmapImage bitmapImage = new BitmapImage();    
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath);
                bitmapImage.EndInit();

                image = new System.Windows.Controls.Image();
                image.Source = bitmapImage;
                image.Height = 60;
                image.Stretch = System.Windows.Media.Stretch.Uniform;
                image.Margin = new Thickness(5);
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;

                wrapPanel.Children.Add(image);
            }
            image = new System.Windows.Controls.Image();
            image.Source = GetWeatherIcon("Ho Chi Minh City, VN");
            image.Width = image.Width / 2;
            image.Height = 60;
            image.Stretch = System.Windows.Media.Stretch.Uniform;
            image.Margin = new Thickness(5);
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            wrapPanel.Children.Add(image);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Controls.Image clickedImage = sender as System.Windows.Controls.Image;

            Sticker sticker = new Sticker();

            sticker.SetImageSource(clickedImage.Source as BitmapImage);


            double imageRealWidth = 0;
            double imageRealHeight = 0;

            if (Photo.Source is BitmapSource bitmapSource)
            {
                imageRealWidth = bitmapSource.PixelWidth;
                imageRealHeight = bitmapSource.PixelHeight;
            }

            IconInImage stickerInfo = new IconInImage();

            stickerInfo.IconBitmap = ConvertBitmapImageToBitmap(clickedImage.Source as BitmapImage);
            stickerInfo.Position = new System.Drawing.Point(0,0);
            stickerInfo.Size = new System.Drawing.Size((int)((clickedImage.ActualWidth)*(imageRealWidth/Photo.ActualWidth)), (int)(clickedImage.ActualHeight* (imageRealWidth / Photo.ActualWidth)));

            _stickerList.Add(stickerInfo);

            Canvas.SetLeft(sticker, 0);
            Canvas.SetTop(sticker, 0);

            sticker.StickerInfo = stickerInfo;

            sticker.MouseLeftButtonDown += Sticker_MouseLeftButtonDown;
            sticker.MouseLeftButtonUp += Sticker_MouseLeftButtonUp;
            sticker.MouseMove += Sticker_MouseMove;

            canvasSticker.Children.Add(sticker);

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(sticker);
            if (adornerLayer != null)
            {
                adornerLayer.Add(new ResizeAdorner(sticker, (imageRealWidth / Photo.ActualWidth), (imageRealWidth / Photo.ActualWidth)));
            }
        }
        private void Sticker_MouseLeftButtonUp(object sender,MouseButtonEventArgs e)
        {
            Bin.Visibility = Visibility.Hidden;
        }
        private void Sticker_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if (sender is Sticker clickedSticker)
            {
                Bin.Visibility = Visibility.Visible;

                if (canvasSticker.Children.Contains(clickedSticker))
                {
                    
                    canvasSticker.Children.Remove(clickedSticker);
                    canvasSticker.Children.Add(clickedSticker);
                    _stickerList.Remove(clickedSticker.StickerInfo);
                    _stickerList.Add(clickedSticker.StickerInfo);
                }
            }
        }

        private void Sticker_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Bin.Visibility = Visibility.Visible;
            }
            else
            {
                Bin.Visibility = Visibility.Hidden;

                System.Windows.Point dropPosition = e.GetPosition(canvasSticker);

                System.Windows.Point binPosition = Bin.TranslatePoint(new System.Windows.Point(0, 0), canvasSticker);

                double binX = binPosition.X;
                double binY = binPosition.Y;
                double binWidth = Bin.ActualWidth;
                double binHeight = Bin.ActualHeight;

                if (e.OriginalSource is System.Windows.Controls.Image draggedImage)
                {

                    FrameworkElement parentElement = draggedImage;
                    while (parentElement.Parent != null && !(parentElement.Parent is Sticker))
                    {
                        parentElement = parentElement.Parent as FrameworkElement;
                    }

                    if (parentElement.Parent is Sticker draggedSticker)
                    {
                        if (canvasSticker.Children.Contains(draggedSticker))
                        {
                            if (dropPosition.X >= binX && dropPosition.X <= binX + binWidth &&
                                dropPosition.Y >= binY && dropPosition.Y <= binY + binHeight)
                            {
                                canvasSticker.Children.Remove(draggedSticker);
                                _stickerList.Remove(draggedSticker.StickerInfo);
                            }
                        }
                    }
                }
            }
        }


        private void canvas_DragOver(object sender, DragEventArgs e)
        {

            System.Windows.Point dropPosition = e.GetPosition(canvasSticker);

            double canvasWidth = canvasSticker.Width;
            double canvasHeight = canvasSticker.Height;

            double imageRealWidth = 0;
            double imageRealHeight = 0;

            if (Photo.Source is BitmapSource bitmapSource)
            {
                imageRealWidth = bitmapSource.PixelWidth;
                imageRealHeight = bitmapSource.PixelHeight;
            }


            System.Windows.Point binPosition = Bin.TranslatePoint(new System.Windows.Point(0, 0), canvasSticker);

            double binX = binPosition.X;
            double binY = binPosition.Y;
            double binWidth = Bin.ActualWidth;
            double binHeight = Bin.ActualHeight;

            if (e.OriginalSource is System.Windows.Controls.Image draggedImage)
            {
                double x = draggedImage.ActualWidth / 2;
                double y = draggedImage.ActualHeight / 2;

                FrameworkElement parentElement = draggedImage;
                while (parentElement.Parent != null && !(parentElement.Parent is Sticker))
                {
                    parentElement = parentElement.Parent as FrameworkElement;
                }

                if (parentElement.Parent is Sticker draggedSticker)
                {
                    if (canvasSticker.Children.Contains(draggedSticker))
                    {

                        Canvas.SetLeft(draggedSticker, dropPosition.X - x);
                        Canvas.SetTop(draggedSticker, dropPosition.Y - y);

                        double curX = dropPosition.X - x;
                        double curY = dropPosition.Y - y;

                        if (dropPosition.X < x)
                        {
                            Canvas.SetLeft(draggedSticker, 0);
                            curX = 0;
                        }
                        if (dropPosition.Y < y)
                        {
                            Canvas.SetTop(draggedSticker, 0);
                            curY = 0;
                        }
                        if (dropPosition.X > canvasWidth - x)
                        {
                            Canvas.SetLeft(draggedSticker, canvasWidth - 2 * x);
                            curX = canvasWidth - 2 * x;
                        }
                        if (dropPosition.Y > canvasHeight - y)
                        {
                            Canvas.SetTop(draggedSticker, canvasHeight - 2 * y);
                            curY = canvasHeight - 2 * y;
                        }
                        draggedSticker.StickerInfo.Position = new System.Drawing.Point((int)(curX*(imageRealWidth/Photo.ActualWidth)), (int)(curY*(imageRealHeight/Photo.ActualHeight)));

                        //check if drop position is inside image bin
                        if (dropPosition.X >= binX && dropPosition.X <= binX + binWidth &&
                            dropPosition.Y >= binY && dropPosition.Y <= binY + binHeight)
                        {
                            draggedSticker.stickerImage.Opacity = 0.6;
                        }
                        else
                        {
                            draggedSticker.stickerImage.Opacity = 1.0;
                        }
                    }
                }
            }

        }

        private Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            Bitmap bitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);
                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }

        public BitmapImage GetWeatherIcon(string city)
        {
            BitmapImage iconImage = new BitmapImage();
            try
            {
                string apiKey = "b976b9fdcd1f9ddaa0ae7a3ee362df88";

                string url = $"http://api.openweathermap.org/data/2.5/weather?q={WebUtility.UrlEncode(city)}&appid={apiKey}&units=metric";

                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);

                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    double temperature = result.main.temp;
                    string iconCode = result.weather[0].icon;

                    string iconUrl = $"http://openweathermap.org/img/wn/{iconCode}.png";

                    byte[] iconData = client.DownloadData(iconUrl);
                    MemoryStream iconStream = new MemoryStream(iconData);
                    iconImage.BeginInit();
                    iconImage.StreamSource = iconStream;
                    iconImage.EndInit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); 
            }
            return iconImage;
        }
        
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            imgTemp = RenderManager.RenderIcons(ConvertBitmapImageToBitmap(Photo.Source as BitmapImage), _stickerList);
            string imageDirectory = Path.Combine(projectDirectory, $"DriveImage");
            imgTemp.Save(imageDirectory + "\\temp.jpg");
            ButtonContinueClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
