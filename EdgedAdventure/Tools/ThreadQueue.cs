using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EdgedAdventure
{
    class ThreadQueue
    {
        Thread thread;
        bool init;

        public ThreadQueue (Thread t)
        {
            thread = t;
            init = false;
        }

        public bool Update()
        {
            if (!init)
            {
                thread.Start();
                init = true;
                return false;
            }
            else if (init && !thread.IsAlive)
            {
                return true;
            }
            return false;
        }
    }
}
