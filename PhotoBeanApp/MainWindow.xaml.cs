using EOSDigital.API;
using EOSDigital.SDK;
using PhotoBeanApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using FrameLib.Frame;
using FrameLib.Utils;
//using TestImage.Frame;
//using TestImage.Utils;

namespace PhotoBeanApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int numberOfCut;
        private int numberOfPrint;
        List<System.Windows.Controls.Image> imageList;
        Frames frames;
        private DispatcherTimer countdownTimer;
        private int remainingTimeInSeconds = 20;
        private int currentScreenIndex;
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory;
        string driveDirectory;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            numberOfCut = 1;
            numberOfPrint = 1;
            currentScreenIndex = 0;
            imageList = new List<System.Windows.Controls.Image>();
            projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            driveDirectory = Path.Combine(projectDirectory, "DriveImage");
            DeleteJPGFilesInDirectory(driveDirectory);
            string frameTypeJsonPath = Path.Combine(projectDirectory, "FrameType.json");
            List<FrameType> frameList = ReadAndParseJsonFileWithSystemTextJson.UseFileOpenReadTextWithSystemTextJson(frameTypeJsonPath);
            frames = Frames.Instance(frameList);
            frames.LoadTypeImage(Path.Combine(projectDirectory, "Frames\\1cut\\1a"), "1a");
            frames.LoadTypeImage(Path.Combine(projectDirectory, "Frames\\4cut\\4a"), "4a");
            frames.LoadTypeImage(Path.Combine(projectDirectory, "Frames\\6cut\\6a"), "6a");
            frames.LoadTypeImage(Path.Combine(projectDirectory, "Frames\\6cut\\6b"), "6b");
            WelcomeScreen welcomeScreen = new WelcomeScreen();
            welcomeScreen.StartButtonClick += WelcomeScreen_StartButtonClick;
            contentControl.Content = welcomeScreen;
        }
        private void InitializeTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingTimeInSeconds--;

            if (remainingTimeInSeconds <= 0)
            {
                StopTimer();
            }
            totalTime.Content = remainingTimeInSeconds.ToString();
        }

        private void StopTimer()
        {
            countdownTimer.Stop();
            switch (currentScreenIndex)
            {
                case 0:
                    break;
                case 1:
                    ResetApp();
                    break;
                case 2:
                    ResetApp();
                    break;
                case 3:
                    if (contentControl.Content is TakePhotoScreen takePhotoScreen)
                    {
                        TakePhotoScreen_ContinueButtonClick(takePhotoScreen, EventArgs.Empty);
                    }
                    break;
                case 4:
                    if (contentControl.Content is ChoosePhotoScreen choosePhotoScreen)
                    {
                        ChoosePhotoScreen_ButtonContinueClick(choosePhotoScreen, EventArgs.Empty);
                    }
                    break;
                case 5:
                    if (contentControl.Content is FrameScreen frameScreen)
                    {
                        FrameScreen_ButtonContinueClick(frameScreen, EventArgs.Empty);
                    }
                    break;
                case 6:
                    if (contentControl.Content is BackgroundScreen backgroundScreen)
                    {
                        BackgroundScreen_ButtonContinueClick(backgroundScreen, EventArgs.Empty);
                    }
                    break;
                case 7:
                    if (contentControl.Content is GoodbyeScreen goodbyeScreen)
                    {
                        ResetApp();
                    }
                    break;
            }
        }

        private void StartTimer()
        {
            remainingTimeInSeconds = 20;
            totalTime.Content = remainingTimeInSeconds.ToString();
            countdownTimer.Start();
        }
        private void ResetApp()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void DeleteJPGFilesInDirectory(string directory)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                foreach (FileInfo file in di.GetFiles("*.jpg"))
                {
                    file.Delete();
                }
                foreach (FileInfo file in di.GetFiles("*.JPG"))
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting files: {ex.Message}");
            }
        }
        private void WelcomeScreen_StartButtonClick(object sender, EventArgs e)
        {

            //SetUpScreen setUpScreen = new SetUpScreen();
            //setUpScreen.ButtonNextClick += SetUpScreen_ButtonNextClick;
            //contentControl.Content = setUpScreen;
            //setUpScreen.LoadFrames(setUpScreen.numberOfCut);
            currentScreenIndex = 3;
            TakePhotoScreen takePhotoScreen = new TakePhotoScreen(1, 1);
            takePhotoScreen.ContinueButtonClick += TakePhotoScreen_ContinueButtonClick;
            contentControl.Content = takePhotoScreen;
            StartTimer();
        }
        private void SetUpScreen_ButtonNextClick(object sender, EventArgs e)
        {
            SetUpScreen setUpScreen = (SetUpScreen)sender;
            numberOfCut = setUpScreen.numberOfCut;
            numberOfPrint = setUpScreen.numberOfPrint;
            PaymentScreen paymentScreen = new PaymentScreen(numberOfCut, numberOfPrint);
            paymentScreen.PaymentSuccess += PaymentScreen_PaymentSuccess;
            contentControl.Content = paymentScreen;
            StartTimer();
        }
        private void PaymentScreen_PaymentSuccess(object sender, EventArgs e)
        {
            //PaymentScreen paymentScreen = (PaymentScreen)sender;
            TakePhotoScreen takePhotoScreen = new TakePhotoScreen(numberOfCut, numberOfPrint);
            takePhotoScreen.ContinueButtonClick += TakePhotoScreen_ContinueButtonClick;
            contentControl.Content = takePhotoScreen;
            StartTimer();
        }

        private void TakePhotoScreen_ContinueButtonClick(object sender, EventArgs e)
        {
            currentScreenIndex = 4;
            TakePhotoScreen takePhotoScreen = (TakePhotoScreen)sender;
            takePhotoScreen.MoveImageToImagesFolder();
            takePhotoScreen.GetAllImages(takePhotoScreen.imagesDirectory);
            List<System.Windows.Controls.Image> imageList = takePhotoScreen.imageList;
            takePhotoScreen.MainCamera.CloseSession();
            takePhotoScreen.StopTimer();
            ChoosePhotoScreen choosePhotoScreen = new ChoosePhotoScreen(numberOfCut, imageList);
            choosePhotoScreen.ButtonContinueClick += ChoosePhotoScreen_ButtonContinueClick;
            contentControl.Content = choosePhotoScreen;
            StartTimer();
        }

        private void ChoosePhotoScreen_ButtonContinueClick(object sender, EventArgs e)
        {
            currentScreenIndex = 7;
            ChoosePhotoScreen choosePhotoScreen = (ChoosePhotoScreen)sender;
            imageList = choosePhotoScreen.selectedImages;
            FrameScreen frameScreen = new FrameScreen(imageList, frames, numberOfCut);
            System.Drawing.Bitmap image = frameScreen.imgTemp;
            string codeFrameType = frameScreen.codeFrameType;
            BackgroundScreen backgroundScreen = new BackgroundScreen(image, codeFrameType, frames, numberOfCut);
            Bitmap photo = backgroundScreen.imgTemp;
            GoodbyeScreen goodbyeScreen = new GoodbyeScreen(photo);
            contentControl.Content = goodbyeScreen;
            StartTimer();
        }

        private void FrameScreen_ButtonContinueClick(object sender, EventArgs e)
        {
            FrameScreen frameScreen = (FrameScreen)sender;
            System.Drawing.Bitmap image = frameScreen.imgTemp;
            string codeFrameType = frameScreen.codeFrameType;
            BackgroundScreen backgroundScreen = new BackgroundScreen(image, codeFrameType, frames, numberOfCut);
            backgroundScreen.ButtonContinueClick += BackgroundScreen_ButtonContinueClick;
            contentControl.Content = backgroundScreen;
            StartTimer();
        }

        private void BackgroundScreen_ButtonContinueClick(object sender, EventArgs e)
        {
            BackgroundScreen backgroundScreen = (BackgroundScreen)sender;
            Bitmap photo = backgroundScreen.imgTemp;
            GoodbyeScreen goodbyeScreen = new GoodbyeScreen(photo);
            contentControl.Content = goodbyeScreen;
            //StickerScreen stickerScreen = new StickerScreen(photo);
            //stickerScreen.ButtonContinueClick += StickerScreen_ButtonContinueClick;
            //contentControl.Content = stickerScreen;
        }

        private void StickerScreen_ButtonContinueClick(object sender, EventArgs e)
        {
            StickerScreen stickerScreen = (StickerScreen)sender;
            Bitmap photo = stickerScreen.imTemp;
            //GoodbyeScreen goodbyeScreen = new GoodbyeScreen(photo);
            //contentControl.Content = goodbyeScreen;
        }
    }

}
