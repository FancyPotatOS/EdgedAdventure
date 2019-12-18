using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    public class Item
    {

        public uint id;
        public string tile;
        public uint amount;
        public uint attackBonus;
        public EdgedAdventure.ItemAction use;
        public uint mineBonus;
        public uint pierce;
        public bool canDig;
        public bool canUse;

        public uint blockID;
        public uint[] placeableLayers;

        public Item(uint thisID, string t, uint a)
        {
            id = thisID;
            tile = t;
            amount = a;
        }

        public static Item GetItem(uint value, uint amount)
        {
            if (value == 0)
            {
                Item dirt = new Item(value, @"item\dirt", amount);
                {
                    dirt.canUse = true;
                    dirt.use = EdgedAdventure.ItemAction.place;
                    dirt.blockID = 1;
                    dirt.placeableLayers = new uint[] { 0 };
                }

                return dirt;
            }
            else if (value == 1)
            {
                Item door = new Item(value, @"tiles\wooden_door", amount);
                return door;
            }
            else
            {
                return new Item(value, @"text\question_mark", amount);
            }
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

        public Item Copy()
        {
            Item copy = new Item(id, tile, amount);
            {
                copy.attackBonus = this.attackBonus;
                copy.use = this.use;
                copy.mineBonus = this.mineBonus;
                copy.pierce = this.pierce;
                copy.canDig = this.canDig;
                copy.canUse = this.canUse;
            }
            return copy;
        }

    }
}
