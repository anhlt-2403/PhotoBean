using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FrameLib.Frame;
using FrameLib.Render;

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for BackgroundScreen.xaml
    /// </summary>
    public partial class BackgroundScreen : UserControl
    {
        public event EventHandler ButtonContinueClick;
        private Bitmap photo;
        private string codeFrameType;
        public Bitmap imgTemp;
        public Frames frameList;
        public int numberOfCut;
        public BackgroundScreen(Bitmap photo, string codeFrameType, Frames frameList, int numberOfCut)
        {
            InitializeComponent();
            this.photo = photo;
            this.codeFrameType = codeFrameType;
            this.frameList = frameList;
            this.numberOfCut = numberOfCut;
            SetUpRightGrid();
            LoadBackgrounds();
        }
        private void SetUpRightGrid()
        {
            double columnWidth = 250;
            double rowHeight = 250;
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string backgroundsDirectory = Path.Combine(projectDirectory, $"Frames\\{numberOfCut}cut\\{codeFrameType}");
            string[] backgroundFiles = Directory.GetFiles(backgroundsDirectory, $"*.png");
            for (int i = 0; i < 2; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(columnWidth, GridUnitType.Pixel);
                Backgrounds.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < backgroundFiles.Count() / 2 + 1; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Pixel);
                Backgrounds.RowDefinitions.Add(rowDefinition);
            }
            Backgrounds.HorizontalAlignment = HorizontalAlignment.Center;
            Backgrounds.VerticalAlignment = VerticalAlignment.Center;
        }
        private void LoadBackgrounds()
        {

            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string backgroundsDirectory = Path.Combine(projectDirectory, $"Frames\\{numberOfCut}cut\\{codeFrameType}");
            string[] backgroundFiles = Directory.GetFiles(backgroundsDirectory, $"*.png");
            int columnIndex = 0;
            int rowIndex = 0;
            foreach (string file in backgroundFiles)
            {
                System.Windows.Controls.Image frame = new System.Windows.Controls.Image();
                frame.Source = new BitmapImage(new Uri(file));
                frame.Stretch = Stretch.Uniform;
                frame.Margin = new Thickness(10);

                Backgrounds.Children.Add(frame);
                Grid.SetColumn(frame, columnIndex);
                Grid.SetRow(frame, rowIndex);
                frame.MouseLeftButtonDown += Background_MouseLeftButtonDown;
                columnIndex++;
                if (columnIndex == 2)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
            imgTemp = RenderManager.GhepBackground(frameList.GetType(codeFrameType), photo, "default.png");
            string imageDirectory = Path.Combine(projectDirectory, $"DriveImage");
            imgTemp.Save(imageDirectory + "\\temp.jpg");
            Print.Source = ConvertToBitmapSource(imgTemp);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonContinueClick?.Invoke(this, EventArgs.Empty);
        }

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image clickedBackground = sender as System.Windows.Controls.Image;
            string fileName = System.IO.Path.GetFileName(clickedBackground.Source.ToString());
            imgTemp = RenderManager.GhepBackground(frameList.GetType(codeFrameType), photo, fileName);
            Print.Source = ConvertToBitmapSource(imgTemp);
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
    }
}
