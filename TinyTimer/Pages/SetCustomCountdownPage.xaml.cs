using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using TinyTimer.DataModel;
using Windows.Storage;
using Windows.UI.Core;

namespace TinyTimer.Pages
{
    public sealed partial class SetCustomCountdownPage : Page
    {
        private UserTimes userTimes;
        private CountdownTime currentCountdownTime;

        public SetCustomCountdownPage()
        {
            this.InitializeComponent();

            if (Settings.Current.ShowSeconds)
                VisualStateManager.GoToState(this, "ShowSeconds", true);

            // initialize user times plus a current countdown time
            currentCountdownTime = new CountdownTime();
            userTimes = new UserTimes();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

    
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
             // commit minutes and seconds from text boxes to the current time
            currentCountdownTime.Minutes = Convert.ToInt32(minutesTextBox.Text);
            currentCountdownTime.Seconds = Convert.ToInt32(secondsTextBox.Text);

            // initialize a 4 deep int array for save data
            int[] previousTimeVals = new int[4];

            // try to get the local save data
            if (ApplicationData.Current.LocalSettings.Values["previousTimeVals"] != null)
            {
                userTimes.PreviousCountdownTime = new CountdownTime();
                userTimes.NextPreviousCountdownTime = new CountdownTime();

                int index = 0;
                foreach (int timeVal in (int[])ApplicationData.Current.LocalSettings.Values["previousTimeVals"])
                {
                    if (previousTimeVals.Count() >= index)
                    {
                        previousTimeVals[index] = timeVal;
                        index++;
                    }
                }

                // set previous with new values, move existing values to next previous
                userTimes.PreviousCountdownTime.Minutes = currentCountdownTime.Minutes;
                userTimes.PreviousCountdownTime.Seconds = currentCountdownTime.Seconds;
                userTimes.NextPreviousCountdownTime.Minutes = previousTimeVals[0];
                userTimes.NextPreviousCountdownTime.Seconds = previousTimeVals[1];

                // re set array in the right order to store locally
                previousTimeVals[0] = userTimes.PreviousCountdownTime.Minutes;
                previousTimeVals[1] = userTimes.PreviousCountdownTime.Seconds;
                previousTimeVals[2] = userTimes.NextPreviousCountdownTime.Minutes;
                previousTimeVals[3] = userTimes.NextPreviousCountdownTime.Seconds;

                ApplicationData.Current.LocalSettings.Values["previousTimeVals"] = previousTimeVals;
            }
        
            // all done, navigate to timer page and pass in the current time
            Frame.Navigate(typeof(TimerPage), currentCountdownTime, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void TimeTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text.Count() > 0)
            {
                foreach (char c in sender.Text)
                {
                    // text boxes support 2 (seconds) or 3 (minutes) digits
                    // each time text is entered, check each char to see if it's a letter
                    // if it is, remove it from the string
                    if (!IsCharNumber(c))
                    {
                        if (sender.Text.IndexOf(c) == 2 && sender.Text.Count() == 3)
                            sender.Text = sender.Text.Remove(sender.Text.Count() - 1);
                        else
                            sender.Text = sender.Text.Remove(
                                sender.Text.IndexOf(c), 
                                sender.Text.IndexOf(c) + ((sender.Text.IndexOf(c) == 0) ? 1 : 0)
                                );
                    }
                }
                
                // since we might be unnaturally removing a character from the string, 
                // always set selection to the end of the string
                sender.SelectionStart = sender.Text.Count();
            }

            // only enable the start button if there's at least 1 minute digit and 2 second digits
            if (minutesTextBox.Text.Count() > 0 && secondsTextBox.Text.Count() == 2)
                startButton.IsEnabled = true;
            else
                startButton.IsEnabled = false;
        }

        private static bool IsCharNumber(char text)
        {
            Regex regex = new Regex("[^0-9.-]+"); 
            return !regex.IsMatch(text.ToString());
        }
    }
}
