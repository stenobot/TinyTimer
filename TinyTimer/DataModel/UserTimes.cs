using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyTimer.DataModel
{
    public class UserTimes
    {
        public CountdownTime PreviousCountdownTime { get; set; }
        public CountdownTime NextPreviousCountdownTime { get; set; }
 
    }
}
