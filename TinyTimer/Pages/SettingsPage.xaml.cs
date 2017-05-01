using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using TinyTimer.Controls;
using TinyTimer.DataModel;


namespace TinyTimer.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private DispatcherTimer timer;
        private TimerBackground timerBackground;
        private SoundPlayer soundPlayer;

        public SettingsPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            // set app version
            Windows.ApplicationModel.Package pkg = Windows.ApplicationModel.Package.Current;
            Windows.ApplicationModel.PackageVersion version = pkg.Id.Version;
            versionNumber.Text = "version: " + 
                version.Major.ToString() + "." + 
                version.Minor.ToString() + "." + 
                version.Build.ToString() + "." + 
                version.Revision.ToString();

            SetBackgroundMode();
            SetSecondsMode();
            SetColorMode();
            SetSoundMode();
            SetPlacementMode();
            SetFontMode();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            soundPlayer = new SoundPlayer();
            await soundPlayer.InitializeSounds();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (soundPlayer != null)
                soundPlayer.StopAllSounds();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (timerBackground != null)
                timerBackground.TimeElapsedPercentage += 1;
        }

        private void SetBackgroundMode()
        {
            backgroundModeComboBox.SelectedIndex = Settings.Current.BackgroundModeIndex;
        }

        private void SetSecondsMode()
        {
            if (Settings.Current.ShowSeconds)
                showSecondsCheckBox.IsChecked = true;
            else
                showSecondsCheckBox.IsChecked = false;
        }

        private void SetColorMode()
        {
            colorModeComboBox.SelectedIndex = Settings.Current.ColorModeIndex;
        }

        private void SetSoundMode()
        {
            soundModeComboBox.SelectedIndex = Settings.Current.SoundModeIndex;
        }

        private void SetPlacementMode()
        {
            placementModeComboBox.SelectedIndex = Settings.Current.PlacementModeIndex;
        }

        private void SetFontMode()
        {
            fontModeComboBox.SelectedIndex = Settings.Current.FontModeIndex;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            
            if (box.Name == "colorModeComboBox")
            {
                Settings.Current.ColorModeIndex = box.SelectedIndex;
                Settings.Current.SaveColorMode();

                if (Settings.Current.ColorModeIndex == 1)
                {
                    Settings.Current.SetRandomColor();
                }
                RefreshBackground();
            } else if (box.Name == "backgroundModeComboBox")
            {
                Settings.Current.BackgroundModeIndex = box.SelectedIndex;
                Settings.Current.SaveBackgroundMode();
                RefreshBackground();
            } else if (box.Name == "soundModeComboBox")
            {
                Settings.Current.SoundModeIndex = box.SelectedIndex;
                Settings.Current.SaveSoundMode();
                PlayCurrentSound(box.SelectedIndex);
            } else if (box.Name == "placementModeComboBox")
            {
                Settings.Current.PlacementModeIndex = box.SelectedIndex;
                Settings.Current.SavePlacementMode();
            } else if (box.Name == "fontModeComboBox")
            {
                Settings.Current.FontModeIndex = box.SelectedIndex;
                Settings.Current.SaveFontMode();
            }
        }

        private void ShowSecondsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            Settings.Current.ShowSeconds = (bool)box.IsChecked;

            Settings.Current.SaveSecondsMode();
        }

        private void RefreshBackground()
        {
            timerBackgroundGrid.Children.Clear();
            timerBackground = new TimerBackground();
            timerBackgroundGrid.Children.Add(timerBackground);
        }

        private void PlayCurrentSound(int index)
        {
            if (soundPlayer == null || index == 0)
                return;

            soundPlayer.PlaySound((index - 1) * 2);
        }
    }
}
