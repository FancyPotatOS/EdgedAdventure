using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgedAdventure
{
    class Menu
    {

        public List<MenuPair> currMenu;

        private static bool defMenusInitialized = false;
        private static readonly List<MenuPair> escape_menu = new List<MenuPair>();

        public Menu(List<MenuPair> lM)
        {
            if (!defMenusInitialized)
            {
                escape_menu.Add(new MenuPair("Load", EdgedAdventure.MenuAction.load));
                escape_menu.Add(new MenuPair("CreateFile", EdgedAdventure.MenuAction.test));
                escape_menu.Add(new MenuPair("New World", EdgedAdventure.MenuAction.new_world));
                escape_menu.Add(new MenuPair("Settings", EdgedAdventure.MenuAction.settings));
                escape_menu.Add(new MenuPair("Quit Game", EdgedAdventure.MenuAction.exit));

                defMenusInitialized = true;
            }
            currMenu = lM;
        }

        public Menu()
        {
            if (!defMenusInitialized)
            {
                escape_menu.Add(new MenuPair("Load", EdgedAdventure.MenuAction.load));
                escape_menu.Add(new MenuPair("CreateFile", EdgedAdventure.MenuAction.test));
                escape_menu.Add(new MenuPair("New World", EdgedAdventure.MenuAction.new_world));
                escape_menu.Add(new MenuPair("Settings", EdgedAdventure.MenuAction.settings));
                escape_menu.Add(new MenuPair("Quit Game", EdgedAdventure.MenuAction.exit));
                defMenusInitialized = true;
            }
            currMenu = null;
        }

        public void FillEscapeMenu()
        {
            currMenu = escape_menu;
        }
    }
}
