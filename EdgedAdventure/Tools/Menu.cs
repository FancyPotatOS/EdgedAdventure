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
        private static readonly List<MenuPair> open_menu = new List<MenuPair>();
        private static readonly List<MenuPair> escape_menu = new List<MenuPair>();
        private static readonly List<MenuPair> e_options = new List<MenuPair>();
        private static readonly List<MenuPair> o_options = new List<MenuPair>();

        public Menu(List<MenuPair> lM)
        {
            if (!defMenusInitialized)
            {
                open_menu.Add(new MenuPair("Load", EdgedAdventure.MenuAction.load));
                open_menu.Add(new MenuPair("Test", EdgedAdventure.MenuAction.test));
                open_menu.Add(new MenuPair("New World", EdgedAdventure.MenuAction.new_world));
                open_menu.Add(new MenuPair("Settings", EdgedAdventure.MenuAction.o_options));
                open_menu.Add(new MenuPair("Quit", EdgedAdventure.MenuAction.exit));

                e_options.Add(new MenuPair("Size++", EdgedAdventure.MenuAction.window_size_up));
                e_options.Add(new MenuPair("Size--", EdgedAdventure.MenuAction.window_size_down));
                e_options.Add(new MenuPair("Back", EdgedAdventure.MenuAction.escape_menu));

                o_options.Add(new MenuPair("Size++", EdgedAdventure.MenuAction.window_size_up));
                o_options.Add(new MenuPair("Size--", EdgedAdventure.MenuAction.window_size_down));
                o_options.Add(new MenuPair("Back", EdgedAdventure.MenuAction.open_menu));

                escape_menu.Add(new MenuPair("Test", EdgedAdventure.MenuAction.test));
                escape_menu.Add(new MenuPair("Options", EdgedAdventure.MenuAction.e_options));
                escape_menu.Add(new MenuPair("Back", EdgedAdventure.MenuAction.world));
                escape_menu.Add(new MenuPair("Save", EdgedAdventure.MenuAction.save));
                escape_menu.Add(new MenuPair("Quit", EdgedAdventure.MenuAction.exit));

                defMenusInitialized = true;
            }
            currMenu = lM;
        }

        public Menu()
        {
            if (!defMenusInitialized)
            {
                open_menu.Add(new MenuPair("Load", EdgedAdventure.MenuAction.load));
                open_menu.Add(new MenuPair("Test", EdgedAdventure.MenuAction.test));
                open_menu.Add(new MenuPair("New World", EdgedAdventure.MenuAction.new_world));
                open_menu.Add(new MenuPair("Settings", EdgedAdventure.MenuAction.o_options));
                open_menu.Add(new MenuPair("Quit", EdgedAdventure.MenuAction.exit));

                e_options.Add(new MenuPair("Size++", EdgedAdventure.MenuAction.window_size_up));
                e_options.Add(new MenuPair("Size--", EdgedAdventure.MenuAction.window_size_down));
                e_options.Add(new MenuPair("Back", EdgedAdventure.MenuAction.escape_menu));

                o_options.Add(new MenuPair("Size++", EdgedAdventure.MenuAction.window_size_up));
                o_options.Add(new MenuPair("Size--", EdgedAdventure.MenuAction.window_size_down));
                o_options.Add(new MenuPair("Back", EdgedAdventure.MenuAction.open_menu));

                escape_menu.Add(new MenuPair("Test", EdgedAdventure.MenuAction.test));
                escape_menu.Add(new MenuPair("Options", EdgedAdventure.MenuAction.e_options));
                escape_menu.Add(new MenuPair("Back", EdgedAdventure.MenuAction.world));
                escape_menu.Add(new MenuPair("Save", EdgedAdventure.MenuAction.save));
                escape_menu.Add(new MenuPair("Quit", EdgedAdventure.MenuAction.exit));

                defMenusInitialized = true;
            }
            currMenu = null;
        }

        public void FillOpenMenu()
        {
            currMenu = open_menu;
        }

        public void FillEscapeMenu()
        {
            currMenu = escape_menu;
        }

        public void FillOOptionsMenu()
        {
            currMenu = o_options;
        }

        public void FillEOptionsMenu()
        {
            currMenu = e_options;
        }
    }
}
