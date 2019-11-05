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
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer mediaScrubberTimer;
        DispatcherTimer timerDisplay;
        DispatcherTimer mouseLeftDownTimer;
        DispatcherTimer detectMouseStopTimer;
        DispatcherTimer imageSnapShotTimer;
        string mediaFile;
        bool scrubberIsDragging = false;
        int secsMediaHasPlayed = 0;
        bool mediaIsLooping = false;
        TimeSpan mediaDuration;
        string previousFilename;
        string previousFilePath;
        List<string> previousMediaPlays = new List<string>();
        

        public MainWindow()
        {

            InitializeComponent();
            mediaScrubberTimer = new DispatcherTimer();
            mediaScrubberTimer.Interval = TimeSpan.FromMilliseconds(200);
            mediaScrubberTimer.Tick += new EventHandler(mediaScrubberTimer_Ticker);

            timerDisplay = new DispatcherTimer();
            timerDisplay.Interval = TimeSpan.FromMilliseconds(1000);
            timerDisplay.Tick += new EventHandler(DisplayTime_Ticker);

            mouseLeftDownTimer = new DispatcherTimer();
            mouseLeftDownTimer.Interval = TimeSpan.FromMilliseconds(200);

            detectMouseStopTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            detectMouseStopTimer.Tick += new EventHandler(MouseStop_Ticker);

            imageSnapShotTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            imageSnapShotTimer.Tick += new EventHandler(TakeMediaImage_Ticker);

            CreateDir();
            AddImages(previousFilePath);
            
        }


        private void AddImages(string path)
        {
          AddImagesToList(path);

            var directorypath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Screendumps");
            ImageOne.Source = previousMediaPlays.Count > 0 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 1])) : ImageOne.Source;
            ImageTwo.Source = previousMediaPlays.Count > 1 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 2])) : ImageTwo.Source;
            ImageThree.Source = previousMediaPlays.Count > 2 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 3])) : ImageThree.Source;
            ImageFour.Source = previousMediaPlays.Count > 3 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 4])) : ImageFour.Source;
            ImageFive.Source = previousMediaPlays.Count > 4 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 5])) : ImageFive.Source;
            ImageSix.Source = previousMediaPlays.Count > 5 ? new BitmapImage(new Uri(directorypath + previousMediaPlays[previousMediaPlays.Count - 6])) : ImageSix.Source;
        }
        


        private void AddImagesToList(string path)
        {
            var images = Directory.GetFiles(path, "*");


            foreach (var image in images)
            {
                if (previousMediaPlays.Contains(image.ToString().Substring(image.LastIndexOf("\\"))))
                {
                   
                }
                else
                {
                    previousMediaPlays.Add((image.ToString().Substring(image.LastIndexOf("\\"))));
                }
            }
            
        }

        private void CreateDir()
        {

            var directorypath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Screendumps");
            previousFilePath = directorypath.ToString();
            var directory = new DirectoryInfo(directorypath);

            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        private void TakeMediaImage_Ticker(object sender, EventArgs e)
        {
            Size dpi = new Size(96, 96);

            RenderTargetBitmap bmp =
                new RenderTargetBitmap((int)window.Width, (int)window.Height - 120,
                    dpi.Width, dpi.Height, PixelFormats.Pbgra32);
            bmp.Render(mediaDisplay);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));


            


            previousFilename = Guid.NewGuid().ToString() + ".jpg";
            FileStream fs = new FileStream(System.IO.Path.Combine(previousFilePath,previousFilename), FileMode.Create);
            
            
            encoder.Save(fs);
            
            //string[] previousMedia =  Directory.GetFiles(@"C:\Users\david.jolliffe\Desktop\c#\mediaPlayer\WpfApp1\WpfApp1\Previous");
            fs.Close();

            imageSnapShotTimer.Stop();
            

           
        }

        private void MouseStop_Ticker(object sender, EventArgs e)
        {
            detectMouseStopTimer.Stop();
            Storyboard fadeControls = this.FindResource("AutoFadeControls") as Storyboard;
            Storyboard.SetTarget(fadeControls, this);
            fadeControls.Begin();
            fadeControls.Stop();
        }

        private void HandleCloseButtonClick(object sender, RoutedEventArgs e)
        {
            ApplicationClose();
        }

        private void ApplicationClose()
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void HandleArrowClick(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void HandleOpenFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                mediaFile = openFileDialog.FileName;
                PlayMedia(mediaFile);

                // string mediaDuration = GetMediaDuration(mediaFile).ToString().Substring(0, GetMediaDuration(mediaFile).ToString().LastIndexOf("."));

            }
        }

        private void PlayMedia(string mediaFile)
        {
            this.MediaPlayer.Source = new Uri(mediaFile);

            this.OpenMenu.Visibility = Visibility.Collapsed;

            this.mediaDisplay.Visibility = Visibility.Visible;
            this.MediaPlayer.Play();


            mediaDuration = GetMediaDuration(mediaFile);

            TimeSlider.Maximum = mediaDuration.TotalSeconds;
            TimeSlider.SmallChange = 1;
            TimeSlider.LargeChange = Math.Min(10, mediaDuration.Seconds / 10);
            /* use for info string timeResult = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                mediaDuration.Hours,
                mediaDuration.Minutes,
                mediaDuration.Seconds);
                 */

            mediaScrubberTimer.Start();
            
            timerDisplay.Start();
            imageSnapShotTimer.Start();
        }

        private static TimeSpan GetMediaDuration(string mediaFile)
        {
            using (var shell = ShellObject.FromParsingName(mediaFile))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }




        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.CanPause)
            {
                PauseButton.Visibility = Visibility.Visible;
                PlayButton.Visibility = Visibility.Collapsed;
                MediaPlayer.Play();
                timerDisplay.Start();
            }

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
            MediaPlayer.Pause();
            timerDisplay.Stop();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            MediaPlayer.Stop();
            MediaPlayer.Source = null;
            this.mediaDisplay.Visibility = Visibility.Collapsed;
            this.OpenMenu.Visibility = Visibility.Visible;
            timerDisplay.Stop();
        }


        //Timers
        void TimeSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            scrubberIsDragging = true;

        }

        void TimeSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            scrubberIsDragging = false;

            MediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value);


            secsMediaHasPlayed = (int)TimeSlider.Value;

        }
        void mediaScrubberTimer_Ticker(object sender, EventArgs e)
        {
            if (!scrubberIsDragging)
            {

                TimeSlider.Value = MediaPlayer.Position.TotalSeconds;

            }

        }


        void DisplayTime_Ticker(object sender, EventArgs e)
        {

            if (TimeDisplay.Text == "00h:00m:00s" && !mediaIsLooping) //media ends events
            {
                secsMediaHasPlayed = 0;
                timerDisplay.Stop();
                MediaPlayer.Stop();
                PlayButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
            }
            else if (TimeDisplay.Text == "00h:00m:00s" && mediaIsLooping)
            {
                secsMediaHasPlayed = 0;
                timerDisplay.Stop();
                MediaPlayer.Stop();
                timerDisplay.Start();
                MediaPlayer.Play();
            }
            TimeSpan dt = GetMediaDuration(mediaFile);
            dt = dt.Subtract(TimeSpan.FromSeconds(secsMediaHasPlayed));

            string timeResult = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
               dt.Hours,
               dt.Minutes,
               dt.Seconds);
            TimeDisplay.Text = timeResult;

            secsMediaHasPlayed++;

        }

        private void HandleMaximise(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            this.MaximizeButton.Visibility = Visibility.Collapsed;
            this.RestoreButton.Visibility = Visibility.Visible;
        }

        private void HandleRestore(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            this.MaximizeButton.Visibility = Visibility.Visible;
            this.RestoreButton.Visibility = Visibility.Collapsed;
        }
        private void HandleMinimize(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void HandleControlMouseEnter(object sender, MouseEventArgs e)
        {
            ControlGrid.Opacity = 50;
        }

        private void HandleControlMouseLeave(object sender, MouseEventArgs e)
        {
            ControlGrid.Opacity = 0;
        }



        private void speedButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseLeftDownTimer.Stop();
        }

        private void SpeedButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton.Name == "ForwardButton")
            {
                mouseLeftDownTimer.Tick += new EventHandler(MouseHeldDownForward);
            }
            else
            {
                mouseLeftDownTimer.Tick += new EventHandler(MouseHeldDownRewind);
            }
            mouseLeftDownTimer.Start();

        }

        private void MouseHeldDownForward(object sender, EventArgs e)
        {

            if (MediaPlayer.Position <= mediaDuration.Subtract(TimeSpan.FromSeconds(30)))
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value + 30);
                secsMediaHasPlayed = secsMediaHasPlayed + 30;
            }
            else
            {
                mouseLeftDownTimer.Stop();
                timerDisplay.Stop();
                secsMediaHasPlayed = 0;
                MediaPlayer.Stop();
                TimeDisplay.Text = "00:00:00";
                PlayButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
            }
        }

        private void MouseHeldDownRewind(object sender, EventArgs e)
        {

            if (MediaPlayer.Position >= TimeSpan.Parse("00:00:30"))

            {
                MediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value - 30);
                secsMediaHasPlayed = secsMediaHasPlayed - 30;
            }
            else
            {
                mouseLeftDownTimer.Stop();
                timerDisplay.Stop();
                secsMediaHasPlayed = 0;
                MediaPlayer.Stop();
                TimeDisplay.Text = "00:00:00";
                PlayButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)

        {
            LoopButton.Visibility = Visibility.Collapsed;
            UnLoopButton.Visibility = Visibility.Visible;

            mediaIsLooping = false;
        }

        private void UnLoopButton_Click(object sender, RoutedEventArgs e)
        {
            LoopButton.Visibility = Visibility.Visible;
            UnLoopButton.Visibility = Visibility.Collapsed;
            mediaIsLooping = true;
        }

        private void ShowVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            ShowVolumeButton.Visibility = Visibility.Collapsed;
            ShowVolumeButtonOff.Visibility = Visibility.Visible;
        }
        private void ShowVolumeButtonOff_Click(object sender, RoutedEventArgs e)
        {
            ShowVolumeButton.Visibility = Visibility.Visible;
            ShowVolumeButtonOff.Visibility = Visibility.Collapsed;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = VolumeSlider.Value;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
           detectMouseStopTimer.Stop();
            detectMouseStopTimer.Start();
            
        }

      
    }
}
