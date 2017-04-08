using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace TinyTimer.Pages
{
    public sealed partial class TrialMessagePage : Page
    {
        public TrialMessagePage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }


        private void Continue_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetCustomCountdownPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
