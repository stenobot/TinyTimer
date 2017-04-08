using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TinyTimer.DataModel;
using Windows.Storage;
using Windows.UI.Core;
using Windows.Services.Store;

namespace TinyTimer.Pages
{
    public sealed partial class StartPage : Page
    {
        private StoreContext context;
        private StoreAppLicense appLicense;
        private bool isTrial;

        private int[] previousTimeVals;
        private UserTimes userTimes;

        public StartPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            InitializeLicense();

            // initialize user times plus 4 deep int array for saved time data
            userTimes = new UserTimes();
            previousTimeVals = new int[4];

            TryGetSavedPreviousTimes();

            PopulatePreviousButtons();
            FadeInPageContent.Begin();
        }


        private async void InitializeLicense()
        {
            if (context == null)
                context = StoreContext.GetDefault();

            appLicense = await context.GetAppLicenseAsync();

            
            // register changed event, in case license change during app session
            context.OfflineLicensesChanged += Context_OfflineLicensesChanged;

            CheckLicense(appLicense);
        }

        private async void Context_OfflineLicensesChanged(StoreContext sender, object args)
        {
            appLicense = await context.GetAppLicenseAsync();
            CheckLicense(appLicense);         
        }

        private void CheckLicense(StoreAppLicense license)
        {
            if (license.IsActive && license.IsTrial)
            {
                isTrial = true;
                VisualStateManager.GoToState(this, "TrialState", true);             
            }
        }

        private void PopulatePreviousButtons()
        {
            // set content of previous button
            previousCountdownButton.Content = userTimes.PreviousCountdownTime.Minutes;

            // set content of next previous button 
            nextPreviousCountdownButton.Content = userTimes.NextPreviousCountdownTime.Minutes;

            if (Settings.Current.ShowSeconds)
            {
                previousCountdownButton.Content += (":" +
                ((userTimes.PreviousCountdownTime.Seconds < 10) ? "0" : "") +
                userTimes.PreviousCountdownTime.Seconds);

                nextPreviousCountdownButton.Content += (":" +
                ((userTimes.NextPreviousCountdownTime.Seconds < 10) ? "0" : "") +
                userTimes.NextPreviousCountdownTime.Seconds);
            }
        }

        private void TryGetSavedPreviousTimes()
        {
            // restore previous time data from local settings
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

                // set user times using retrieved local data
                userTimes.PreviousCountdownTime.Minutes = previousTimeVals[0];
                userTimes.PreviousCountdownTime.Seconds = previousTimeVals[1];
                userTimes.NextPreviousCountdownTime.Minutes = previousTimeVals[2];
                userTimes.NextPreviousCountdownTime.Seconds = previousTimeVals[3];
            }
            else
            {
                // first run, we need to initialize local save data
                InitializeSavedPreviousTimes();
            }
        }

        private void InitializeSavedPreviousTimes()
        {
            // set default values for previous and next previous
            userTimes.PreviousCountdownTime = new CountdownTime(25, 0);
            userTimes.NextPreviousCountdownTime = new CountdownTime(5, 0);

            // fill data array
            previousTimeVals[0] = userTimes.PreviousCountdownTime.Minutes;
            previousTimeVals[1] = userTimes.PreviousCountdownTime.Seconds;
            previousTimeVals[2] = userTimes.NextPreviousCountdownTime.Minutes;
            previousTimeVals[3] = userTimes.NextPreviousCountdownTime.Seconds;

            // save locally
            ApplicationData.Current.LocalSettings.Values["previousTimeVals"] = previousTimeVals;
        }

        private void PreviousCountdownButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == previousCountdownButton)
                Frame.Navigate(typeof(TimerPage), userTimes.PreviousCountdownTime, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            else if (sender == nextPreviousCountdownButton)
                Frame.Navigate(typeof(TimerPage), userTimes.NextPreviousCountdownTime, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void CustomCountdownButton_Click(object sender, RoutedEventArgs e)
        {
            if (isTrial)
                Frame.Navigate(typeof(TrialMessagePage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            else
                Frame.Navigate(typeof(SetCustomCountdownPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
