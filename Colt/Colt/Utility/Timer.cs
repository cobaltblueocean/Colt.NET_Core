using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Timer: System.Timers.Timer
    {
        private DateTime _initialDateTime;
        private DateTime _endDateTime;

        #region Constructors
        public Timer() : this(100)
        {
        }

        public Timer(int interval) :base(interval)
        {
        }
        #endregion

        public long Display
        {
            get
            {
                return _endDateTime.GetTime() - _initialDateTime.GetTime();
            }
        }

        public int Minutes()
        {
            return (int)(Display / 60000);
        }


        public new void Start()
        {
            _initialDateTime = DateTime.Now;
            base.Start();
        }

        public new void Stop()
        {
            _endDateTime = DateTime.Now;
            base.Stop();
        }

        public void Reset()
        {
            _initialDateTime = DateTime.Now;
            _endDateTime = DateTime.Now;
            base.Stop();
            base.Start();
        }
    }
}
