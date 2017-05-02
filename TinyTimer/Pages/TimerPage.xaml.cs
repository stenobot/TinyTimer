using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TinyTimer.DataModel;
using System.Threading.Tasks;
using Windows.UI.Core;


namespace TinyTimer.Pages
{
    public sealed partial class TimerPage : Page
    {
        private CountdownTime countdownCurrTime;

        private SoundPlayer soundPlayer;
        
        private int introCount;
        private bool timerCanStart;
        private bool timerFinished;
        private bool outroSmall;
        private int outroLoopCount;
        private double percentageInterval;
        private bool minutesOnly;

        private FontFamily verminFont = (FontFamily)Application.Current.Resources["VerminFont"];
        private FontFamily silkscreenFont = (FontFamily)Application.Current.Resources["SilkscreenFont"];
        private FontFamily vFiveFont = (FontFamily)Application.Current.Resources["vFiveXtenderFont"];
        private FontFamily digiffitiFont = (FontFamily)Application.Current.Resources["DigiffitiFont"];

        private DateTime currTime;
        private DateTime prevTime;

        private DispatcherTimer timer;

        public TimerPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            if (Settings.Current.ShowSeconds == false)
                minutesOnly = true;

            if (minutesOnly)
                VisualStateManager.GoToState(this, "MinutesOnly", true);

            switch (Settings.Current.FontModeIndex)
            {
                case 0:
                default:
                    SetClockFont(verminFont, 48);
                    break;
                case 1:
                    SetClockFont(silkscreenFont, 38);
                    break;
                case 2:
                    SetClockFont(vFiveFont, 36);
                    break;
                case 3:
                    SetClockFont(digiffitiFont, 52);
                    break;
            }

            outroLoopCount = 10;

            if (Settings.Current.ColorModeIndex == 1)
            {
                SetRandomColor();
            }
        }

        private void SetClockFont(FontFamily font, double size)
        {
            clockMinuteText.FontFamily = font;
            clockSecondText.FontFamily = font;
            clockColonText.FontFamily = font;

            clockMinuteText.FontSize = size;
            clockSecondText.FontSize = size;
            clockColonText.FontSize = size;
        }

        private void SetRandomColor()
        {
            SolidColorBrush brush = new SolidColorBrush(Settings.Current.CurrentRandomColor);

            clockColonText.Foreground = brush;
            clockMinuteText.Foreground = brush;
            clockSecondText.Foreground = brush;
            backgroundRectangle.Fill = brush;
            doneButton.Foreground = brush;
        }

        private void StartTimer(CountdownTime time)
        {
            // get the timer ready
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += Timer_Tick;

            // time stamps to keep our pseudo clock honest
            currTime = DateTime.Now; 

            // start the timer
            timer.Start();

            // set clock strings to starting values
            clockMinuteText.Text = ((time.Minutes < 10 && !minutesOnly) ? "0" : "") + time.Minutes.ToString();
            clockSecondText.Text = ((time.Seconds < 10) ? "0" : "") + time.Seconds.ToString();
        }

        private void Timer_Tick(object sender, object e)
        {
            prevTime = currTime;
            currTime = DateTime.Now;

            int extraTicks = 0;

            if (timerFinished)
            {
                if (!outroSmall)
                {
                    if (outroLoopCount >= 10)
                    {
                        soundPlayer.PlayRandomSound();
                        outroLoopCount = 0;
                    }

                    ScaleBkgRect.ScaleX = 1.2;
                    ScaleBkgRect.ScaleY = 1.2;
                    RotateBkgRect.Value -= 110;
                    LightBkgRect.Distance = 100;
                    outroSmall = true;
                    return;
                }
                else
                {
                    ScaleBkgRect.ScaleX = 0.7;
                    ScaleBkgRect.ScaleY = 0.7;
                    RotateBkgRect.Value -= 60;
                    LightBkgRect.Distance = 30;
                    outroLoopCount++;
                    outroSmall = false;
                    return;
                }
            }

            if (!timerCanStart)
            {
                if (introCount == 0)
                    FadeInBkgRectAnimation.Begin();

                if (introCount < 3)
                {
                    if (Settings.Current.SoundModeIndex > 0)
                        soundPlayer.PlaySound((Settings.Current.SoundModeIndex - 1) * 2);

                    RotateBkgRect.Value += 90;
                    ScaleBkgRect.ScaleX += 0.15;
                    ScaleBkgRect.ScaleY += 0.15;
                    LightBkgRect.Distance += 15;

                    introCount++;

                    FadeOutClockAnimation.Begin();

                    return;
                }
                else
                {
                    if (Settings.Current.SoundModeIndex > 0)
                        soundPlayer.PlaySound(((Settings.Current.SoundModeIndex - 1) * 2) + 1);

                    clockGrid.Opacity = 1;
                    FadeOutBkgRectAnimation.Begin();
                    RotateBkgRect.Value += 150;
                    ScaleBkgRect.ScaleX += 1.5;
                    ScaleBkgRect.ScaleY += 1.5;

                    if (Settings.Current.PlacementModeIndex == 1)
                    {
                        OffsetClock.StartAnimation();
                        ScaleClock.StartAnimation();
                    }
                    
                    timerCanStart = true;

                    pauseButton.Visibility = Visibility.Visible;
                    FadeInPauseButtonAnimation.Begin();
                }
            }

            // in order to keep the clock honest, we check that the current tick time and
            // previous tick time are less than 2 seconds apart
            // this is important in case the app gets minimized,
            // which sometimes causes the tick code to not run
            if ((currTime - prevTime) > TimeSpan.FromSeconds(2))
            {
                TimeSpan diffTime = currTime - prevTime;

                double diffTotalSeconds = Math.Floor(diffTime.TotalSeconds);

                // keep track of extra needed ticks
                // we'll need these to catch up the background animation
                extraTicks = (int)diffTotalSeconds;

                // if we haven't passed a minute mark, simply decrement the seconds wiht the diff time
                if (diffTotalSeconds <= countdownCurrTime.Seconds)
                {
                    countdownCurrTime.Seconds -= diffTime.Seconds;
                }
                else
                {
                    // otherwise, do some math to figure out how many minutes and seconds to decrement
                    int diffMinutes = ((int)Math.Floor((diffTotalSeconds - countdownCurrTime.Seconds) / 60) + 1);
                    int diffSeconds = ((int)diffTotalSeconds - countdownCurrTime.Seconds - (60 * (diffMinutes - 1)));

                    countdownCurrTime.Minutes -= diffMinutes;
                    countdownCurrTime.Seconds = 60 - diffSeconds;
                }
            }

            if (countdownCurrTime.Seconds <= 0)
            {
                if (countdownCurrTime.Minutes <= 0)
                {
                    EndTimer();
                    return;
                }
                countdownCurrTime.Minutes--;
                countdownCurrTime.Seconds = 59;
            }
            else
            {
                countdownCurrTime.Seconds--;
            }

            // increment the elapsed time percentage to update the background animation
            if (extraTicks > 0)
            {
                // something got off, so we need to account for the lost time
                // by artificially adding extra ticks
                for (int i = 0; i < extraTicks; i++)
                {
                    timeBackground.TimeElapsedPercentage += percentageInterval;
                }

                // reset our extra ticks variable
                extraTicks = 0;
            }
            else
            {
                // just a normal tick
                timeBackground.TimeElapsedPercentage += percentageInterval;
            }

            // update the clock
            clockMinuteText.Text = ((countdownCurrTime.Minutes < 10 && !minutesOnly) ? "0" : "") + countdownCurrTime.Minutes.ToString();
            clockSecondText.Text = ((countdownCurrTime.Seconds < 10) ? "0" : "") + countdownCurrTime.Seconds.ToString();
        }

        private void EndTimer()
        {
            timerFinished = true;
            FadeInBkgRectAnimation.Begin();
            FadeOutBackgroundTimer.Begin();
            ScaleBkgRect.ScaleX = 0.2;
            ScaleBkgRect.ScaleY = 0.2;
            RotateBkgRect.Duration = 3000;
            RotateBkgRect.Value = -100;

            FadeOutClockAnimation.Begin();

            doneGrid.Visibility = Visibility.Visible;
            FadeInDoneButton.Begin();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is CountdownTime)
            {
                soundPlayer = new SoundPlayer();
                await soundPlayer.InitializeSounds();

                // set current time to start time passed in from previous page
                countdownCurrTime = e.Parameter as CountdownTime;

                // start the timer with the current time
                StartTimer(countdownCurrTime);
                    
                // divide 100 by the total number of minutes to get percentage interval for background animation
                percentageInterval = (100 / ((double)(countdownCurrTime.Minutes * 60) + countdownCurrTime.Seconds));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (soundPlayer != null)
                soundPlayer.StopAllSounds();
        }


        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            doneGrid.Opacity = 0;
            ScaleBkgRect.ScaleX = 0.1;
            ScaleBkgRect.ScaleY = 0.1;
            RotateBkgRect.Value -= 400;
            await Task.Delay(500);
            Frame.Navigate(typeof(StartPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (pauseButtonIcon.Visibility == Visibility.Visible)
            {
                // pause and temporarily stop the timer
                timer.Stop();
                
                pauseButtonIcon.Visibility = Visibility.Collapsed;
                playButtonIcon.Visibility = Visibility.Visible;
            }
            else
            {
                // need to reset current time
                currTime = DateTime.Now;

                // resume the timer
                timer.Start();
                
                pauseButtonIcon.Visibility = Visibility.Visible;
                playButtonIcon.Visibility = Visibility.Collapsed;
            }
        }
    }
}
