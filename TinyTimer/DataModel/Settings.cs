using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace TinyTimer.DataModel
{
    public class Settings
    {
        private static Settings current;
        private RandomColorGenerator randomColorGenerator;

        public static Settings Current
        {
            get
            {
                if (current == null)
                    current = new Settings();

                return current;
            }

            set
            {
                current = value;
            }
        }


        public int BackgroundModeIndex { get; set; }

        public int ColorModeIndex { get; set; }

        public Color CurrentRandomColor { get; set; }

        public bool ShowSeconds { get; set; }

        public int SoundModeIndex { get; set; }

        public Settings() { }

        public void Init()
        {
            TrySetBackgroundModeFromSave();
            TrySetSecondsModeFromSave();
            TrySetColorModeFromSave();
            TrySetSoundModeFromSave();

            randomColorGenerator = new RandomColorGenerator();

            if (ColorModeIndex == 1)
            {
                SetRandomColor();
            }
        }

        public void SetRandomColor()
        {
            CurrentRandomColor = randomColorGenerator.GetRandomColor();
        }

        public void SaveBackgroundMode()
        {
            ApplicationData.Current.LocalSettings.Values["backgroundMode"] = BackgroundModeIndex;
        }

        public void SaveSecondsMode()
        {
            ApplicationData.Current.LocalSettings.Values["secondsMode"] = (ShowSeconds ? 1 : 0);
        }

        public void SaveColorMode()
        {
            ApplicationData.Current.LocalSettings.Values["colorMode"] = ColorModeIndex;
        }

        public void SaveSoundMode()
        {
            ApplicationData.Current.LocalSettings.Values["soundMode"] = SoundModeIndex;
        }

        private void TrySetBackgroundModeFromSave()
        {
            if (ApplicationData.Current.LocalSettings.Values["backgroundMode"] != null)
            {
                BackgroundModeIndex = (int)ApplicationData.Current.LocalSettings.Values["backgroundMode"];
            }
            else
            {
                BackgroundModeIndex = 0;
                ApplicationData.Current.LocalSettings.Values["backgroundMode"] = 0;
            }
        }

        private void TrySetSecondsModeFromSave()
        {
            if (ApplicationData.Current.LocalSettings.Values["secondsMode"] != null)
            {
                ShowSeconds = ((int)ApplicationData.Current.LocalSettings.Values["secondsMode"] == 1) ? true : false;
            }
            else
            {
                ShowSeconds = true;
                ApplicationData.Current.LocalSettings.Values["secondsMode"] = 0;
            }
        }

        private void TrySetColorModeFromSave()
        {
            if (ApplicationData.Current.LocalSettings.Values["colorMode"] != null)
            {
                ColorModeIndex = (int)ApplicationData.Current.LocalSettings.Values["colorMode"];
            }
            else
            {
                ColorModeIndex = 0;
                ApplicationData.Current.LocalSettings.Values["colorMode"] = 0;
            }
        }

        private void TrySetSoundModeFromSave()
        {
            if (ApplicationData.Current.LocalSettings.Values["soundMode"] != null)
            {
                SoundModeIndex = (int)ApplicationData.Current.LocalSettings.Values["soundMode"];
            }
            else
            {
                SoundModeIndex = 3;
                ApplicationData.Current.LocalSettings.Values["soundMode"] = 4;
            }
        }
    }
}
