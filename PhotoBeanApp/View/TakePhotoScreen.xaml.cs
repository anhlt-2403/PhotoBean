using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Threading.Tasks;


namespace PhotoBeanApp.View
{
    /// <summary>
    /// Interaction logic for TakePhotoScreen.xaml
    /// </summary>
    public partial class TakePhotoScreen : UserControl
    {
        private bool isPhotoTaken = false;
        private bool isDownloadCompleted = false;
        CanonAPI APIHandler;
        public EOSDigital.API.Camera MainCamera;

        List<EOSDigital.API.Camera> CamList;
        bool IsInit = false;
        int BulbTime = 30;
        ImageBrush bgbrush = new ImageBrush();
        Action<BitmapImage> SetImageAction;

        int ErrCount;
        object ErrLock = new object();
        private DispatcherTimer countdownTimer;
        private int remainingTimeInSeconds = 3;
        private int imageIndex = 1;
        private bool isTimerRunning = false;
        private int numberOfCut, numberOfPrint;
        public List<System.Windows.Controls.Image> imageList = new List<System.Windows.Controls.Image>();
        public event EventHandler ContinueButtonClick;
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory;
        string tempDirectory;
        public string imagesDirectory;
        public TakePhotoScreen(int numberOfCut, int numberOfPrint)
        {

            InitializeComponent();
            try
            {
                projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
                tempDirectory = System.IO.Path.Combine(projectDirectory, "Camera\\Temp");
                imagesDirectory = System.IO.Path.Combine(projectDirectory, "Camera\\Images");
                APIHandler = new CanonAPI();
                RefreshCamera();
                SetImageAction = (BitmapImage img) => { bgbrush.ImageSource = img; };
                InitializeTimer();
                this.numberOfCut = numberOfCut;
                this.numberOfPrint = numberOfPrint;
                StartButton.Visibility = Visibility.Collapsed;
                OpenSession();
                StartLiveView();
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    StartButton.Visibility = Visibility.Visible;
                };
                timer.Start();
                IsInit = true;
            }
            catch (DllNotFoundException) { ReportError("Canon DLLs not found!", true); }
            catch (Exception ex) { ReportError(ex.Message, true); }

        }
        private void RefreshCamera()
        {
            CamList = APIHandler.GetCameraList();
        }

        private void OpenSession()
        {
            MainCamera = CamList[0];
            MainCamera.OpenSession();
            MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
            MainCamera.SetCapacity(4096, int.MaxValue);

            MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
            MainCamera.ProgressChanged += MainCamera_ProgressChanged;
            MainCamera.DownloadReady += MainCamera_DownloadReady;
        }
        private void MainCamera_ProgressChanged(object sender, int progress)
        {
            try
            {
                MainProgressBar.Dispatcher.Invoke((Action)delegate
                {
                    MainProgressBar.Value = progress;
                    if (progress >= 100)
                    {
                        isDownloadCompleted = true;
                    }
                });
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }
        private void MainCamera_DownloadReady(EOSDigital.API.Camera sender, DownloadInfo Info)
        {
            sender.DownloadFile(Info, tempDirectory);
            MainProgressBar.Dispatcher.Invoke((Action)delegate { MainProgressBar.Value = 0; });
        }
        private void MainCamera_LiveViewUpdated(EOSDigital.API.Camera sender, Stream img)
        {
            try
            {
                using (WrapStream s = new WrapStream(img))
                {
                    img.Position = 0;
                    BitmapImage EvfImage = new BitmapImage();
                    EvfImage.BeginInit();
                    EvfImage.StreamSource = s;
                    EvfImage.CacheOption = BitmapCacheOption.OnLoad;
                    EvfImage.EndInit();
                    EvfImage.Freeze();
                    Application.Current.Dispatcher.BeginInvoke(SetImageAction, EvfImage);
                }
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }


        private void ReportError(string message, bool lockdown)
        {
            int errc;
            lock (ErrLock) { errc = ++ErrCount; }


            if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            lock (ErrLock) { ErrCount--; }
        }

        private void InitializeTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
            ContinueButton.Visibility = Visibility.Collapsed;
            ReplayButton.Visibility = Visibility.Collapsed;
            countdownLabel.Visibility = Visibility.Collapsed;
        }

        private void StartTimer()
        {
            remainingTimeInSeconds = 3;
            countdownLabel.Visibility = Visibility.Visible;
            countdownLabel.Content = remainingTimeInSeconds.ToString();
            countdownTimer.Start();
            isTimerRunning = true;
        }
        public void StopTimer()
        {
            countdownTimer.Stop();
            isTimerRunning = false;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingTimeInSeconds--;

            if (remainingTimeInSeconds <= 0)
            {
                if (!isPhotoTaken)
                {
                    TakePhoto();
                    isPhotoTaken = true;
                }
                if (isDownloadCompleted)
                {
                    StopLiveView();
                    StopTimer();
                    if ((numberOfCut == 1 && imageIndex == numberOfCut + 1) || imageIndex == numberOfCut + 2)
                    {
                        ContinueButton.Content = "Hoàn thành";
                    }
                    ContinueButton.Visibility = Visibility.Visible;
                    ReplayButton.Visibility = Visibility.Visible;
                    countdownLabel.Visibility = Visibility.Collapsed;
                }
                return;
            }
            countdownLabel.Content = remainingTimeInSeconds.ToString();
        }

        public void MoveImageToImagesFolder()
        {
            string[] files = Directory.GetFiles(tempDirectory);

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(imagesDirectory, fileName);
                    File.Move(file, destinationPath);
                }
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isTimerRunning)
            {
                if (numberOfCut == 1)
                {
                    countPhotoLable.Content = $"{imageIndex}/{numberOfCut + 1}";
                }
                else
                {
                    countPhotoLable.Content = $"{imageIndex}/{numberOfCut + 2}";
                }
                StartButton.Visibility = Visibility.Collapsed;
                StartTimer();
            }
        }

        public void GetAllImages(string directory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory, "*.JPG", SearchOption.AllDirectories))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(file);
                    bitmap.EndInit();

                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = bitmap.Clone();
                    imageList.Add(image);
                }

                foreach (string file in Directory.GetFiles(directory, "*.JPG", SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting images: {ex.Message}");
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            isPhotoTaken = false;
            isDownloadCompleted = false;
            StartLiveView();
            if (!isTimerRunning)
            {
                ContinueButton.Visibility = Visibility.Collapsed;
                ReplayButton.Visibility = Visibility.Collapsed;
                StartTimer();
                Button btn = (Button)sender;
                if (btn.Name.Equals("ContinueButton"))
                {
                    MoveImageToImagesFolder();
                    imageIndex++;
                    if (numberOfCut == 1)
                    {
                        countPhotoLable.Content = $"{imageIndex}/{numberOfCut + 1}";
                        if (imageIndex > numberOfCut + 1)
                        {
                            StopLiveView();
                            imageControl.Background = null;
                            GetAllImages(imagesDirectory);
                            ContinueButtonClick?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        countPhotoLable.Content = $"{imageIndex}/{numberOfCut + 2}";
                        if (imageIndex > numberOfCut + 2)
                        {
                            StopLiveView();
                            imageControl.Background = null;
                            GetAllImages(imagesDirectory);
                            ContinueButtonClick?.Invoke(this, EventArgs.Empty);
                        }
                    }

                }
            }
        }
        private void StartLiveView()
        {
            imageControl.Background = bgbrush;
            MainCamera.StartLiveView();
        }

        private void TakePhoto()
        {
            MainCamera.TakePhoto();
        }
        private void StopLiveView()
        {
            MainCamera.StopLiveView();
        }
    }
}
