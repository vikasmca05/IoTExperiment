using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Imaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Common.Contract;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Popups;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OptimaCognitive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _subscriptionKey = "f5ddf536a7af46afbd095a5ceee461ab";
        BitmapImage bitMapImage;
        public MainPage()
        {
            this.InitializeComponent();
        }
        
        private async Task<Emotion[]> UploadAndDetectEmotions(string url)
        {
            Debug.WriteLine("EmotionServiceClient is created");

            EmotionServiceClient esc = new EmotionServiceClient(_subscriptionKey);

            Debug.WriteLine("Caling EmotionServiceClient. RecognizeAsync() ... ");
            try {
                Emotion[] emotionResult = await esc.RecognizeAsync(url);
                return emotionResult;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Detection failed");
                Debug.WriteLine(ex.ToString());
                return null;
            }

        }

        private async void button_Clicked(Object sender, RoutedEventArgs e)
        {
            ImageCanvas.Children.Clear();
            String urlString = ImageURL.Text;
            Uri uri;
            try
            {
                uri = new Uri(urlString, UriKind.Absolute);
            }

            catch (UriFormatException ex)
            {
                Debug.WriteLine(ex.Message);

                var dialog = new MessageDialog("URL is not correct");

                await dialog.ShowAsync();

                return;
            }

            //Load image from URL
            bitMapImage = new BitmapImage();
            bitMapImage.UriSource = uri;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitMapImage;

            //Load image to UI
            ImageCanvas.Background = imageBrush;
            detectionStatus.Text = "Detecting ...";
            urlString = "http://blogs.cdc.gov/genomics/files/2015/11/ThinkstockPhotos-177826416.jpg";
            Emotion[] emotionResult = await UploadAndDetectEmotions(urlString);

            detectionStatus.Text = "Detections Done.";
            displayParsedResults(emotionResult);
            displayAllResults(emotionResult);
            DrawFaceRectangle(emotionResult, bitMapImage, urlString);
        }

        private void displayAllResults(Emotion[] resultList)
        {
            int index = 0;
            foreach (Emotion emotion in resultList)
            {
                ResultBox.Items.Add("\nFace #" + index
                + "\nAnger: " + emotion.Scores.Anger
                + "\nContempt: " + emotion.Scores.Contempt
                + "\nDisgust: " + emotion.Scores.Disgust
                + "\nFear: " + emotion.Scores.Fear
                + "\nHappiness: " + emotion.Scores.Happiness
                + "\nSurprise: " + emotion.Scores.Surprise);

                index++;
            }
        }

        private async void displayParsedResults(Emotion[] resultList)
        {
            int index = 0;
            string textToDisplay = "";
            foreach (Emotion emotion in resultList)
            {
                string emotionString = parseResults(emotion);
                textToDisplay += "Person number " + index.ToString() + " " + emotionString + "\n";
                index++;
            }
            ResultBox.Items.Add(textToDisplay);
        }


        private string parseResults(Emotion emotion)
        {
            float topScore = 0.0f;
            string topEmotion = "";
            string retString = "";
            //anger
            topScore = emotion.Scores.Anger;
            topEmotion = "Anger";
            // contempt
            if (topScore < emotion.Scores.Contempt)
            {
                topScore = emotion.Scores.Contempt;
                topEmotion = "Contempt";
            }
            // disgust
            if (topScore < emotion.Scores.Disgust)
            {
                topScore = emotion.Scores.Disgust;
                topEmotion = "Disgust";
            }
            // fear
            if (topScore < emotion.Scores.Fear)
            {
                topScore = emotion.Scores.Fear;
                topEmotion = "Fear";
            }
            // happiness
            if (topScore < emotion.Scores.Happiness)
            {
                topScore = emotion.Scores.Happiness;
                topEmotion = "Happiness";
            }
            // surprise
            if (topScore < emotion.Scores.Surprise)
            {
                topScore = emotion.Scores.Surprise;
                topEmotion = "Surprise";
            }

            retString = "is expressing " + topEmotion + " with " + topScore.ToString() + " certainty.";
            return retString;
        }

        public async void DrawFaceRectangle(Emotion[] emotionResult, BitmapImage bitMapImage, String urlString)
        {


            if (emotionResult != null && emotionResult.Length > 0)
            {
                Windows.Storage.Streams.IRandomAccessStream stream = await Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(urlString)).OpenReadAsync();


                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);


                double resizeFactorH = ImageCanvas.Height / decoder.PixelHeight;
                double resizeFactorW = ImageCanvas.Width / decoder.PixelWidth;


                foreach (var emotion in emotionResult)
                {

                    Microsoft.ProjectOxford.Common.Rectangle faceRect = emotion.FaceRectangle;

                    Image Img = new Image();
                    BitmapImage BitImg = new BitmapImage();
                    // open the rectangle image, this will be our face box
                    Windows.Storage.Streams.IRandomAccessStream box = await Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/rectangle.png", UriKind.Absolute)).OpenReadAsync();

                    BitImg.SetSource(box);

                    //rescale each facebox based on the API's face rectangle
                    var maxWidth = faceRect.Width * resizeFactorW;
                    var maxHeight = faceRect.Height * resizeFactorH;

                    var origHeight = BitImg.PixelHeight;
                    var origWidth = BitImg.PixelWidth;


                    var ratioX = maxWidth / (float)origWidth;
                    var ratioY = maxHeight / (float)origHeight;
                    var ratio = Math.Min(ratioX, ratioY);
                    var newHeight = (int)(origHeight * ratio);
                    var newWidth = (int)(origWidth * ratio);

                    BitImg.DecodePixelWidth = newWidth;
                    BitImg.DecodePixelHeight = newHeight;

                    // set the starting x and y coordiantes for each face box
                    Thickness margin = Img.Margin;

                    margin.Left = faceRect.Left * resizeFactorW;
                    margin.Top = faceRect.Top * resizeFactorH;

                    Img.Margin = margin;

                    Img.Source = BitImg;
                    ImageCanvas.Children.Add(Img);
                }
            }
        }
        //...
    }


}



