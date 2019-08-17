using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgedAdventure
{
    class DirtItem : Item
    {
        public DirtItem(uint id, uint a) : base(id, "Dirt", @"item/dirt", a, 0, EdgedAdventure.ItemAction.place, 0, false, true) { }

        public override EdgedAdventure.ItemAction Use()
        {
            return EdgedAdventure.ItemAction.place;
        }
    }
}
