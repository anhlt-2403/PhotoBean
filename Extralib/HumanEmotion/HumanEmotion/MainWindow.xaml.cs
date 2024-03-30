using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeminiChat.DotNet;
using Models;
using Newtonsoft;



using Newtonsoft.Json;

namespace HumanEmotion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string imageFilePath = "";
        private string CUSTOM_VISION_AI_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/customvision/v3.0/Prediction/4ccda526-812a-472c-be70-9e70d9803e5a/classify/iterations/EmotionDetector/image";
        private string PREDICTION_KEY = "a5feedbb88ad44d990b8659383a51506";
        private string CONTENT_TYPE_OCTET_STREAM = "application/octet-stream";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDlg.FileName))
            {
                imageFilePath = openFileDlg.FileName;
                var uri = new Uri(openFileDlg.FileName);
                var bitmap = new BitmapImage(uri);
                selectedImage.Source = bitmap;
            }
        }

        private async void btn_Run_Click(object sender, RoutedEventArgs e)
        {
            richText.Document.Blocks.Clear();
            btn_ChooseFile.IsEnabled = false;
            btn_Run.IsEnabled = false;
            var clientCustomVison = new HttpClient();
            HttpResponseMessage responseCustomVision;
            // Custome vision AI
            clientCustomVison.DefaultRequestHeaders.Add("Prediction-Key", PREDICTION_KEY);
            byte[] byteData = GetBytesFromImage(imageFilePath);
            var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(CONTENT_TYPE_OCTET_STREAM);

            // Send request
            responseCustomVision = await clientCustomVison.PostAsync(CUSTOM_VISION_AI_ENDPOINT, content);
            var responseObject = JsonConvert.DeserializeObject<ContentResponse>(await responseCustomVision.Content.ReadAsStringAsync());
            var maxProbability = responseObject.Predictions.Max(m => m.Probability);
            ProbabilityDetail probabilityDetailMax = responseObject.Predictions.SingleOrDefault(m => m.Probability == maxProbability)!;
            string emotion = probabilityDetailMax.TagName;
            var messageResponse = await CallGemini(emotion);
            
            if (messageResponse.Equals("disgust")) 
            {
                messageResponse = "anger";
            }
            richText.AppendText(messageResponse);
            btn_ChooseFile.IsEnabled = true;
            btn_Run.IsEnabled = true;
        }
        private static byte[] GetBytesFromImage(string imageFilePath)
        {
            FileStream fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fs);
            return binaryReader.ReadBytes((int)fs.Length);
        }
        private async Task<string> CallGemini(string emotion)
        {
            var service = new GeminiService("AIzaSyBWzmxke5pBZr4LL5zBVbD_AUPEebR95HY");
            service.AppendMessage($"Bây giờ tôi rất {emotion}. Hãy nói những gì phù hợp với tôi. Vui lòng trả lời bằng tiếng Việt. Giới hạn trong 100 từ");
            var story = string.Empty;
            await service.StreamResponseAsync((dataLine) =>
            {
                story += dataLine;
            });
            return story;
        }
    }
}