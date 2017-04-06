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
using System.ComponentModel;
using TinyTimer.DataModel;
using Windows.Storage;


namespace TinyTimer.Controls
{
    public sealed partial class TimerBackground : UserControl
    {
        public static readonly DependencyProperty TimeElapsedPercentageProperty =
            DependencyProperty.Register("TimeElapsedPercentage", typeof(double), typeof(TimerBackground), new PropertyMetadata(0.0));

     

        public double TimeElapsedPercentage
        {
            get { return (double)GetValue(TimeElapsedPercentageProperty); }
            set
            {
                SetValue(TimeElapsedPercentageProperty, value);
                UpdateTimerBackgroundVisuals();
            }
        }



        //public event PropertyChangedEventHandler TimeElapsedPercentageChanged;

        //void NotifySelectedPresetChanged(string info)
        //{
        //    TimeElapsedPercentageChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        //}

        public TimerBackground()
        {
            this.InitializeComponent();

            if (Settings.Current.ColorModeIndex == 1)
            {
                SetRandomColor();
            }

            if (Settings.Current.BackgroundModeIndex == 2)
                VisualStateManager.GoToState(this, "LinesMode", true);
            else if (Settings.Current.BackgroundModeIndex == 1)
                VisualStateManager.GoToState(this, "CirclesMode", true);
            else
                VisualStateManager.GoToState(this, "SquaresMode", true);

            FadeInAnimation.Begin();
        }

        private void SetRandomColor()
        {
            Settings.Current.SetRandomColor();
            SolidColorBrush brush = new SolidColorBrush(Settings.Current.CurrentRandomColor);

            if (Settings.Current.BackgroundModeIndex == 2)
            {
                foreach (Grid grid in linesAnimationGrid.Children)
                {
                    grid.Background = brush;
                }
            }
            else if (Settings.Current.BackgroundModeIndex == 1)
            {
                ellipse.Fill = brush;
            }
            else
            {
                foreach (Grid grid in squaresAnimationGrid.Children)
                {
                    grid.Background = brush;
                }
            }
        }

        private void UpdateTimerBackgroundVisuals()
        {
            one.Opacity = Math.Max(0, Math.Min((TimeElapsedPercentage * .25), 1.0));
            two.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 4) * .25), 1.0));
            three.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 8) * .25), 1.0));
            four.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 12) * .25), 1.0));
            five.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 16) * .25), 1.0));
            six.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 20) * .25), 1.0));
            seven.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 24) * .25), 1.0));
            eight.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 28) * .25), 1.0));
            nine.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 32) * .25), 1.0));
            ten.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 36) * .25), 1.0));
            eleven.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 40) * .25), 1.0));
            twelve.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 44) * .25), 1.0));
            thirteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 48) * .25), 1.0));
            fourteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 52) * .25), 1.0));
            fifteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 56) * .25), 1.0));
            sixteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 60) * .25), 1.0));
            seventeen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 64) * .25), 1.0));
            eighteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 68) * .25), 1.0));
            nineteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 72) * .25), 1.0));
            twenty.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 76) * .25), 1.0));
            twentyone.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 80) * .25), 1.0));
            twentytwo.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 84) * .25), 1.0));
            twentythree.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 88) * .25), 1.0));
            twentyfour.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 92) * .25), 1.0));
            twentyfive.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 96) * .25), 1.0));

            ScaleEllipse.ScaleX = TimeElapsedPercentage * .01;
            ScaleEllipse.ScaleY = TimeElapsedPercentage * .01;

            lineOne.Opacity = Math.Max(0, Math.Min((TimeElapsedPercentage * .25), 1.0));
            lineTwo.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 5) * .25), 1.0));
            lineThree.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 10) * .25), 1.0));
            lineFour.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 15) * .25), 1.0));
            lineFive.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 20) * .25), 1.0));
            lineSix.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 25) * .25), 1.0));
            lineSeven.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 30) * .25), 1.0));
            lineEight.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 35) * .25), 1.0));
            lineNine.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 40) * .25), 1.0));
            lineTen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 45) * .25), 1.0));
            lineEleven.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 50) * .25), 1.0));
            lineTwelve.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 55) * .25), 1.0));
            lineThirteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 60) * .25), 1.0));
            lineFourteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 65) * .25), 1.0));
            lineFifteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 70) * .25), 1.0));
            lineSixteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 75) * .25), 1.0));
            lineSeventeen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 80) * .25), 1.0));
            lineEighteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 85) * .25), 1.0));
            lineNineteen.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 90) * .25), 1.0));
            lineTwenty.Opacity = Math.Max(0, Math.Min(((TimeElapsedPercentage - 95) * .25), 1.0));
        }
    }
}
