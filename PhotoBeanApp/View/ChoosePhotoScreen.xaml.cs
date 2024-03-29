using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBeanApp.View
{
    public partial class ChoosePhotoScreen : UserControl
    {
        public event EventHandler ButtonContinueClick;
        public List<Image> selectedImages;
        private List<Tuple<int, int>> emptySlots = new List<Tuple<int, int>>();
        private Dictionary<Image, Tuple<int, int>> initialPositions = new Dictionary<Image, Tuple<int, int>>();

        private int numberOfCut;
        private int numberOfRows;
        private int numberOfColumns;

        public ChoosePhotoScreen(int numberOfCut, List<Image> imageList)
        {
            InitializeComponent();
            this.numberOfCut = numberOfCut; 
            selectedImages = new List<Image>();
            numberOfColumns = 2;
            numberOfRows = numberOfCut / numberOfColumns;
            ContinueButton.Visibility = Visibility.Collapsed;
            InitializeEmptySlots();
            SetUpLeftGrid();
            SetUpRightGrid();
            LoadPhotos(imageList);
        }

        private void InitializeEmptySlots()
        {
            emptySlots.Clear();
            for (int row = 0; row < numberOfRows; row++)
            {
                for (int column = 0; column < numberOfColumns; column++)
                {
                    emptySlots.Add(new Tuple<int, int>(row, column));
                }
            }
        }

        private void SetUpLeftGrid()
        {
            double columnWidth = 250;
            double rowHeight = 200;
            ChoosePhoto.ColumnDefinitions.Clear();
            ChoosePhoto.RowDefinitions.Clear();

            if (numberOfCut == 1)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(columnWidth, GridUnitType.Pixel);
                ChoosePhoto.ColumnDefinitions.Add(columnDefinition);

                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Pixel);
                ChoosePhoto.RowDefinitions.Add(rowDefinition);
            }
            else
            {
                for (int i = 0; i < numberOfColumns; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new GridLength(columnWidth, GridUnitType.Pixel);
                    ChoosePhoto.ColumnDefinitions.Add(columnDefinition);
                }

                for (int i = 0; i < numberOfRows; i++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Pixel);
                    ChoosePhoto.RowDefinitions.Add(rowDefinition);
                }
            }

            ChoosePhoto.HorizontalAlignment = HorizontalAlignment.Center;
            ChoosePhoto.VerticalAlignment = VerticalAlignment.Center;
            ChoosePhoto.Background = Brushes.White;
        }
        private void SetUpRightGrid()
        {
            double columnWidth = 200;
            double rowHeight = 200;

            for (int i = 0; i < numberOfColumns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(columnWidth, GridUnitType.Pixel);
                Photos.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < numberOfRows + 1; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Pixel);
                Photos.RowDefinitions.Add(rowDefinition);
            }

            Photos.HorizontalAlignment = HorizontalAlignment.Center;
            Photos.VerticalAlignment = VerticalAlignment.Center;
        }

        private void LoadPhotos(List<Image> imageList)
        {
            int columnIndex = 0;
            int rowIndex = 0;
            foreach (Image img in imageList)
            {
                Image image = new Image();
                image.Source = img.Source;
                image.Stretch = Stretch.Uniform;
                image.Margin = new Thickness(5);
                Photos.Children.Add(image);
                Grid.SetColumn(image, columnIndex);
                Grid.SetRow(image, rowIndex);
                image.MouseDown += Image_MouseDown;
                initialPositions[image] = new Tuple<int, int>(rowIndex, columnIndex);
                columnIndex++;
                if (columnIndex == 2)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image clickedImage = sender as Image;

            if (Photos.Children.Contains(clickedImage))
            {
                if (ChoosePhoto.Children.Count == numberOfCut)
                {
                    MessageBox.Show("Bạn đã chọn đủ số lượng ảnh.");
                    return;
                }

                if (!ChoosePhoto.Children.Contains(clickedImage))
                {
                    Photos.Children.Remove(clickedImage);
                    ChoosePhoto.Children.Add(clickedImage);
                    Tuple<int, int> emptySlot = FindEmptySlot();
                    selectedImages.Add(clickedImage);
                    if(ChoosePhoto.Children.Count == numberOfCut)
                    {
                        ContinueButton.Visibility = Visibility.Visible;
                    }
                    if (emptySlot != null)
                    {
                        Grid.SetColumn(clickedImage, emptySlot.Item2);
                        Grid.SetRow(clickedImage, emptySlot.Item1);
                    }
                }
            }
            else if (ChoosePhoto.Children.Contains(clickedImage))
            {
                ContinueButton.Visibility= Visibility.Collapsed;
                ChoosePhoto.Children.Remove(clickedImage);
                selectedImages.Remove(clickedImage);

                Tuple<int, int> initialPosition = initialPositions[clickedImage];
                emptySlots.Add(initialPosition);
                Photos.Children.Add(clickedImage);
                Grid.SetColumn(clickedImage, initialPosition.Item2);
                Grid.SetRow(clickedImage, initialPosition.Item1);
            }
        }

        private Tuple<int, int> FindEmptySlot()
        {
            foreach (var emptySlot in emptySlots)
            {
                if (!IsOccupied(emptySlot.Item1, emptySlot.Item2))
                {
                    return emptySlot;
                }
            }
            return null;
        }

        private bool IsOccupied(int row, int column)
        {
            foreach (var image in selectedImages)
            {
                if (Grid.GetRow(image) == row && Grid.GetColumn(image) == column)
                {
                    return true;
                }
            }
            return false;
        }
        private void ReorderSelectedImages()
        {
            selectedImages.Clear();

            for (int row = 0; row < numberOfRows; row++)
            {
                for (int column = 0; column < numberOfColumns; column++)
                {
                    foreach (var child in ChoosePhoto.Children)
                    {
                        if (child is Image image && Grid.GetRow(image) == row && Grid.GetColumn(image) == column)
                        {
                            selectedImages.Add(image);
                            break;
                        }
                    }
                }
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if(numberOfCut != 1)
            {
                ReorderSelectedImages();
            }
            ButtonContinueClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
