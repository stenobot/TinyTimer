using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyTimer.DataModel
{
    public class CountdownTime
    {
        public int Minutes { get; set; }
        public int Seconds { get; set; }


    public CountdownTime(int minutes = 0, int seconds = 0)
        {
            Minutes = minutes;
            Seconds = seconds;
        }
    }
}
