using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgedAdventure
{
    class MenuPair
    {

        public readonly string name;
        public readonly EdgedAdventure.MenuAction action;
        public int numValue;

        public MenuPair(string n, EdgedAdventure.MenuAction a)
        {
            name = n;
            action = a;
        }

        public MenuPair(string n, EdgedAdventure.MenuAction a, int v)
        {
            name = n;
            action = a;
            numValue = v;
        }
    }
}
