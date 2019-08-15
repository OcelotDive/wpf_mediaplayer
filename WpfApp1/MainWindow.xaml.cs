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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        DispatcherTimer timerDisplay;
        DispatcherTimer mouseLeftDownTimer;
        DispatcherTimer mouseStopTimer;
        string mediaFile;
        bool isDragging = false;
        int secsPlayed = 0;
        bool mediaLoop = false;
        TimeSpan ts;



        public MainWindow()
        {

            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);

            timerDisplay = new DispatcherTimer();
            timerDisplay.Interval = TimeSpan.FromMilliseconds(1000);
            timerDisplay.Tick += new EventHandler(DisplayTime);

            mouseLeftDownTimer = new DispatcherTimer();
            mouseLeftDownTimer.Interval = TimeSpan.FromMilliseconds(200);


            mouseStopTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            mouseStopTimer.Tick += new EventHandler(MouseStopTick);


        }

        private void MouseStopTick(object sender, EventArgs e)
        {
            mouseStopTimer.Stop();
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


            ts = GetMediaDuration(mediaFile);

            TimeSlider.Maximum = ts.TotalSeconds;
            TimeSlider.SmallChange = 1;
            TimeSlider.LargeChange = Math.Min(10, ts.Seconds / 10);
            /* use for info string timeResult = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                ts.Hours,
                ts.Minutes,
                ts.Seconds);
                 */

            timer.Start();

            timerDisplay.Start();

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
            isDragging = true;

        }

        void TimeSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;

            MediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value);


            secsPlayed = (int)TimeSlider.Value;

        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {

                TimeSlider.Value = MediaPlayer.Position.TotalSeconds;

            }

        }


        void DisplayTime(object sender, EventArgs e)
        {

            if (TimeDisplay.Text == "00h:00m:00s" && !mediaLoop) //media ends events
            {
                secsPlayed = 0;
                timerDisplay.Stop();
                MediaPlayer.Stop();
                PlayButton.Visibility = Visibility.Visible;
                PauseButton.Visibility = Visibility.Collapsed;
            }
            else if (TimeDisplay.Text == "00h:00m:00s" && mediaLoop)
            {
                secsPlayed = 0;
                timerDisplay.Stop();
                MediaPlayer.Stop();
                timerDisplay.Start();
                MediaPlayer.Play();
            }
            TimeSpan dt = GetMediaDuration(mediaFile);
            dt = dt.Subtract(TimeSpan.FromSeconds(secsPlayed));

            string timeResult = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
               dt.Hours,
               dt.Minutes,
               dt.Seconds);
            TimeDisplay.Text = timeResult;

            secsPlayed++;

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

            if (MediaPlayer.Position <= ts.Subtract(TimeSpan.FromSeconds(30)))
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value + 30);
                secsPlayed = secsPlayed + 30;
            }
            else
            {
                mouseLeftDownTimer.Stop();
                timerDisplay.Stop();
                secsPlayed = 0;
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
                secsPlayed = secsPlayed - 30;
            }
            else
            {
                mouseLeftDownTimer.Stop();
                timerDisplay.Stop();
                secsPlayed = 0;
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
           
            mediaLoop = false;
        }

        private void UnLoopButton_Click(object sender, RoutedEventArgs e)
        {
            LoopButton.Visibility = Visibility.Visible;
            UnLoopButton.Visibility = Visibility.Collapsed;
            mediaLoop = true;
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
            mouseStopTimer.Stop();
            mouseStopTimer.Start();
            
        }

      
    }
}
