using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
