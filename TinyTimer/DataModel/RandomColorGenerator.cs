using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace TinyTimer.DataModel
{
    public class RandomColorGenerator
    {
        List<Color> TinyTimerColors;

        Random random;

        public Color GetRandomColor()
        {
            int randomNumber = random.Next(0, TinyTimerColors.Count);
            return TinyTimerColors[randomNumber];
        }

        public RandomColorGenerator()
        {
            random = new Random();
            Color aqua = Colors.Aqua;
            Color aquamarine = Colors.Aquamarine;
            Color cadetBlue = Colors.CadetBlue;
            Color chartreuse = Colors.Chartreuse;
            Color cornflowerBlue = Colors.CornflowerBlue;
            Color cyan = Colors.Cyan;
            Color darkCyan = Colors.DarkCyan;
            Color darkGray = Colors.DarkGray;
            Color darkKhaki = Colors.DarkKhaki;
            Color darkOrchid = Colors.DarkOrchid;
            Color darkSeaGreen = Colors.DarkSeaGreen;
            Color deepPink = Colors.DeepPink;
            Color deepSkyBlue = Colors.DeepSkyBlue;
            Color goldenrod = Colors.Goldenrod;
            Color lawnGreen = Colors.LawnGreen;
            Color lightGreen = Colors.LightGreen;
            Color lightSeaGreen = Colors.LightSeaGreen;
            Color limeGreen = Colors.LimeGreen;
            Color mediumSpringGreen = Colors.MediumSpringGreen;
            Color orange = Colors.Orange;
            Color paleGreen = Colors.PaleGreen;
            Color springGreen = Colors.SpringGreen;
            Color yellowGreen = Colors.YellowGreen;

            TinyTimerColors = new List<Color>
            {
                aqua,
                aquamarine,
                cadetBlue,
                chartreuse,
                cornflowerBlue,
                cyan,
                darkCyan,
                darkGray,
                darkKhaki,
                darkOrchid,
                darkSeaGreen,
                deepPink,
                deepSkyBlue,
                goldenrod,
                lawnGreen,
                lightGreen,
                lightSeaGreen,
                limeGreen,
                mediumSpringGreen,
                orange,
                paleGreen,
                springGreen,
                yellowGreen
            };
        }
    }
}
