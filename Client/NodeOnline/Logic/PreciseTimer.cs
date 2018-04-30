using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeOnline.Logic
{
    class PreciseTimer
    {
        Stopwatch sw;
        int _delay;

        bool running;

        public event EventHandler Tick;

        public PreciseTimer(int delay)
        {
            _delay = delay;
            sw = new Stopwatch();
            
            running = true;

            Thread thread = new Thread(new ThreadStart(DoWork));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        void DoWork()
        {
            long target = sw.ElapsedMilliseconds + _delay;
            sw.Start();
            while (running)
            {
                if (sw.ElapsedMilliseconds >= target)
                {
                    target += _delay;
                    Tick?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Stop()
        {
            running = false;
        } 
    }
}
