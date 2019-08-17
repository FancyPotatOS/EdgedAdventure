using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    class Item : Entity
    {

        public uint amount;
        public uint attackBonus;
        public EdgedAdventure.ItemAction use;
        public uint mineBonus;
        public bool canDig;
        public bool canUse;

        public Item(uint thisID, string n, string t, uint a, uint at, EdgedAdventure.ItemAction iA, uint m, bool cD, bool cU) : base(thisID, 0, 0, t, n, null, 0, -1, -1, -1, -1, -1)
        {
            name = n;
            tile = t;
            amount = a;
            attackBonus = at;
            use = iA;
            mineBonus = m;
            canDig = cD;
            canUse = cU;
        }

        public static Item GetItem(uint value, uint amount)
        {
            if (value == 0)
            {
                return new DirtItem(0, amount);
            }
            else if (value == 1)
            {

            }
            return new Item(value, "Blank Item", @"text\question_mark", amount, 0, EdgedAdventure.ItemAction.none, 0, false, false);
        }

        public static Item GetItem(string filePath)
        {
            if (!Directory.Exists(filePath)) return null;

            uint ID = UInt32.Parse(File.ReadAllText(filePath + @"\id"));
            uint a = UInt32.Parse(File.ReadAllText(filePath + @"\amount"));

            return Item.GetItem(ID, a);
        }

        public virtual EdgedAdventure.ItemAction Use()
        {
            return EdgedAdventure.ItemAction.none;
        }

        public static void SaveItem(string root, Item i)
        {
            File.WriteAllText(root + "id", "" + i.id);
            File.WriteAllText(root + "amount", "" + i.amount);
        }
    }
}
