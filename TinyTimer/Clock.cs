using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyTimer
{
    public class Clock
    {
        private int hour;
        private int minute;
        private int second;

        public delegate void TimeChangeHandler(object clock, TimeEventArgs timeInfo);

        public event TimeChangeHandler TimeChanged;

        public void RunClock(bool run)
        {
            //clock needs to be able to run and stop
            while (run)
            {
                
                //Thread.Sleep(100);
                DateTime currentTime = DateTime.Now;
                if (currentTime.Second != this.second)
                {
                    //we can assume that at least 1 second has passed
                    TimeEventArgs timeEventArgs = new TimeEventArgs()
                    {
                        //initialize the object and set it's properties
                        Hour = currentTime.Hour,
                        Minute = currentTime.Minute,
                        Second = currentTime.Second
                    };

                    if (TimeChanged != null)
                    {
                        //raise the delegate
                        //pass in this instance of the clock
                        //pass in our new timeEventArgs
                        TimeChanged(this, timeEventArgs);
                    }

                    //make sure to update the values that we're holding in the
                    //private member variables to the current time, so that the
                    //next time we go through this loop we can test it appropriately
                    this.hour = currentTime.Hour;
                    this.minute = currentTime.Minute;
                    this.second = currentTime.Second;
                }
            }
        }
    }
}
