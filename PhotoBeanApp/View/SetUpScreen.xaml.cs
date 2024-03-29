using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for SetUpScreen.xaml
    /// </summary>
    public partial class SetUpScreen : UserControl
    {
        public event EventHandler ButtonNextClick;
        public int numberOfCut;
        public int numberOfPrint;
        public SetUpScreen()
        {
            InitializeComponent();
            this.numberOfPrint = 1;
            this.numberOfCut = 4;
            btn4cut.Background = Brushes.LightCyan;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name.ToString())
            {
                case "btn1cut":
                    numberOfCut = 1;
                    btn1cut.Background = Brushes.LightCyan;
                    btn2cut.Background = Brushes.Transparent;
                    btn4cut.Background = Brushes.Transparent;
                    btn6cut.Background = Brushes.Transparent;
                    btn8cut.Background = Brushes.Transparent;
                    break;
                case "btn2cut":
                    numberOfCut = 2;
                    btn1cut.Background = Brushes.Transparent;
                    btn2cut.Background = Brushes.LightCyan; ;
                    btn4cut.Background = Brushes.Transparent;
                    btn6cut.Background = Brushes.Transparent;
                    btn8cut.Background = Brushes.Transparent;
                    break;
                case "btn4cut":
                    numberOfCut = 4;
                    btn1cut.Background = Brushes.Transparent;
                    btn2cut.Background = Brushes.Transparent;
                    btn4cut.Background = Brushes.LightCyan;
                    btn6cut.Background = Brushes.Transparent;
                    btn8cut.Background = Brushes.Transparent;
                    break;
                case "btn6cut":
                    numberOfCut = 6;
                    btn1cut.Background = Brushes.Transparent;
                    btn2cut.Background = Brushes.Transparent;
                    btn4cut.Background = Brushes.Transparent;
                    btn6cut.Background = Brushes.LightCyan;
                    btn8cut.Background = Brushes.Transparent;
                    break;
                case "btn8cut":
                    numberOfCut = 8;
                    btn1cut.Background = Brushes.Transparent;
                    btn2cut.Background = Brushes.Transparent;
                    btn4cut.Background = Brushes.Transparent;
                    btn6cut.Background = Brushes.Transparent;
                    btn8cut.Background = Brushes.LightCyan;
                    break;
            }
            FrameListGrid.Children.Clear();
            LoadFrames(numberOfCut);
        }

        public void LoadFrames(int numberOfCut)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string framesDirectory = Path.Combine(projectDirectory, $"Frames\\{numberOfCut}cut");
            List<string> defaultFramesList = GetDefaultFramesList(framesDirectory); 
            int count = 0;
            foreach (string frameFile in defaultFramesList)
            {
                if (count == 4)
                {
                    break;
                }
                Image frame = new Image();
                frame.Source = new BitmapImage(new Uri(frameFile));
                frame.Stretch = Stretch.Uniform;
                frame.Margin = new Thickness(10);
                frame.HorizontalAlignment = HorizontalAlignment.Center;
                frame.VerticalAlignment = VerticalAlignment.Center;
                int row = 0;
                int column = count;

                Grid.SetRow(frame, row);
                Grid.SetColumn(frame, column);
                FrameListGrid.Children.Add(frame);

                count++;
            }
        }


        private List<string> GetDefaultFramesList(string directory)
        {
            List<string> defaultframeLists = new List<string>();

            foreach (string file in Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories))
            {
                defaultframeLists.Add(file);
            }

            return defaultframeLists;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            ButtonNextClick?.Invoke(this, EventArgs.Empty);
        }

        private void increaseButton_Click(object sender, RoutedEventArgs e)
        {
            numberOfPrint = Int32.Parse(printLabel.Content.ToString());
            printLabel.Content = $"{++numberOfPrint}";
        }

        private void decreaseButton_Click(object sender, RoutedEventArgs e)
        {
            numberOfPrint = Int32.Parse(printLabel.Content.ToString());
            printLabel.Content = $"{--numberOfPrint}";
        }
    }
}
