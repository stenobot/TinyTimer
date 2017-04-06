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
using System.Diagnostics;
using TinyTimer.DataModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Core;
using Windows.Storage;



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

        private TimerModeEnums timerMode;
        
      //  private int clockMode;

        private DispatcherTimer timer;



        public TimerPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            TrySetTimerModeFromSave();

            outroLoopCount = 10;

            if (Settings.Current.ColorModeIndex == 1)
            {
                SetRandomColor();
            }

            // load event for changing timer mode
            mainTimerGrid.Tapped += MainTimerGrid_Tapped;
        }

        private void SetRandomColor()
        {
            SolidColorBrush brush = new SolidColorBrush(Settings.Current.CurrentRandomColor);

            clockColonText.Foreground = brush;
            clockMinuteText.Foreground = brush;
            clockSecondText.Foreground = brush;
            clockTwoMinuteText.Foreground = brush;
            clockTwoSecondText.Foreground = brush;
            backgroundRectangle.Fill = brush;
            doneButton.Foreground = brush;
        }

        private void TrySetTimerModeFromSave()
        {
            if (ApplicationData.Current.LocalSettings.Values["timerMode"] != null)
            {
                // save data is an int, so we need to cast save value to int
                int enumVal = (int)ApplicationData.Current.LocalSettings.Values["timerMode"];
                timerMode = (TimerModeEnums)enumVal;
            }
            else
            {
                // set timer mode default value and save as an int value
                timerMode = TimerModeEnums.MinutesOnly;
                ApplicationData.Current.LocalSettings.Values["timerMode"] = timerMode.GetHashCode();
            }
        }

        private void MainTimerGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (timerMode == TimerModeEnums.MinutesAndSeconds)
                timerMode = TimerModeEnums.MinutesOnly;
            else if (timerMode == TimerModeEnums.MinutesOnly)
                timerMode = TimerModeEnums.SecondsOnly;
            else if (timerMode == TimerModeEnums.SecondsOnly)
                timerMode = TimerModeEnums.MinutesAndSeconds;

            // saved changed timer mode to local settings as an int value
            ApplicationData.Current.LocalSettings.Values["timerMode"] = timerMode.GetHashCode();

            ChangeTimerModeVisuals();
        }

        private void StartTimer(CountdownTime time)
        {
            // special case for minutes only and seconds only mode
            if (timerMode == TimerModeEnums.MinutesOnly)
                clockTwoMinuteText.Opacity = 1;
            else if (timerMode == TimerModeEnums.SecondsOnly)
                clockTwoSecondText.Opacity = 1;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            clockMinuteText.Text = ((time.Minutes < 10) ? "0" : "") + time.Minutes.ToString();
            clockSecondText.Text = ((time.Seconds < 10) ? "0" : "") + time.Seconds.ToString();
            clockTwoMinuteText.Text = time.Minutes.ToString();
            clockTwoSecondText.Text = ":" + ((time.Seconds < 10) ? "0" : "") + time.Seconds.ToString();
        }

        private void Timer_Tick(object sender, object e)
        {
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
                    soundPlayer.PlaySound(Settings.Current.SoundModeIndex * 2);

                    RotateBkgRect.Value += 90;
                    ScaleBkgRect.ScaleX += 0.15;
                    ScaleBkgRect.ScaleY += 0.15;
                    LightBkgRect.Distance += 15;

                    introCount++;

                    if (timerMode == TimerModeEnums.MinutesAndSeconds)
                        FadeOutClockAnimation.Begin();
                    else if (timerMode == TimerModeEnums.MinutesOnly || timerMode == TimerModeEnums.SecondsOnly)
                        FadeOutClockTwoAnimation.Begin();

                    return;
                }
                else
                {
                    soundPlayer.PlaySound((Settings.Current.SoundModeIndex * 2) + 1);

                    FadeOutBkgRectAnimation.Begin();
                    RotateBkgRect.Value += 150;
                    ScaleBkgRect.ScaleX += 1.5;
                    ScaleBkgRect.ScaleY += 1.5;
                    OffsetClockTwo.StartAnimation();
                    ScaleClockTwo.StartAnimation();
                    timerCanStart = true;

                    ChangeTimerModeVisuals();

                }
            }

            if (countdownCurrTime.Seconds == 0)
            {
                if (countdownCurrTime.Minutes == 0)
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

            // increment the elapsed time percentage with the interval
            timeBackground.TimeElapsedPercentage += percentageInterval;

            clockMinuteText.Text = ((countdownCurrTime.Minutes < 10) ? "0" : "") + countdownCurrTime.Minutes.ToString();
            clockSecondText.Text = ((countdownCurrTime.Seconds < 10) ? "0" : "") + countdownCurrTime.Seconds.ToString();
            clockTwoMinuteText.Text = countdownCurrTime.Minutes.ToString();
            clockTwoSecondText.Text = ":" + ((countdownCurrTime.Seconds < 10) ? "0" : "") + countdownCurrTime.Seconds.ToString();
        }

        private void EndTimer()
        {
            mainTimerGrid.Tapped -= MainTimerGrid_Tapped;

            timerFinished = true;
            FadeInBkgRectAnimation.Begin();
            FadeOutBackgroundTimer.Begin();
            ScaleBkgRect.ScaleX = 0.2;
            ScaleBkgRect.ScaleY = 0.2;
            RotateBkgRect.Duration = 3000;
            RotateBkgRect.Value = -100;

            if (timerMode == TimerModeEnums.MinutesAndSeconds)
                FadeOutClockAnimation.Begin();
            else if (timerMode == TimerModeEnums.MinutesOnly || timerMode == TimerModeEnums.SecondsOnly)
                FadeOutClockTwoAnimation.Begin();

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



        private void ChangeTimerModeVisuals()
        {
            if (timerMode == TimerModeEnums.MinutesAndSeconds)
            {
                clockGrid.Opacity = 1;
                clockTwoGrid.Opacity = 0;
            }
            else if (timerMode == TimerModeEnums.MinutesOnly)
            {
                clockGrid.Opacity = 0;
                clockTwoGrid.Opacity = 1;
                clockTwoMinuteText.Opacity = 1;
                clockTwoSecondText.Opacity = 0;
            }
            else if (timerMode == TimerModeEnums.SecondsOnly)
            {
                clockGrid.Opacity = 0;
                clockTwoGrid.Opacity = 1;
                clockTwoMinuteText.Opacity = 0;
                clockTwoSecondText.Opacity = 1;
            }
        }

        private async void doneButton_Click(object sender, RoutedEventArgs e)
        {
            doneGrid.Opacity = 0;
            ScaleBkgRect.ScaleX = 0.1;
            ScaleBkgRect.ScaleY = 0.1;
            RotateBkgRect.Value -= 400;
            await Task.Delay(500);
            Frame.Navigate(typeof(SetCountdownPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
