using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    class LootDrop
    {
        public float chance;
        public Item drop;
        public uint[] range;

        public static Random dropCalc = new Random();

        public LootDrop(float c, Item i, uint[] r)
        {
            range = r;
            chance = c;
            drop = i;
        }

        public Item GetDrop()
        {
            if (dropCalc.NextDouble() < chance)
            {
                uint calcAmount = (uint)Math.Floor((dropCalc.NextDouble() * (range[1]-range[0]))) + range[0];
                drop.amount = calcAmount;
                return drop;
            }
            else
            {
                return null;
            }
        }

        public static LootDrop GetLootDrop(string filePath)
        {
            if (!Directory.Exists(filePath)) return null;

            float ch = float.Parse(File.ReadAllText(filePath + @"\chance"));

            Item i = Item.GetItem(filePath + @"\drop");

            uint[] r = new uint[2];
            r[0] = UInt32.Parse(File.ReadAllText(filePath + @"\rangel"));
            r[1] = UInt32.Parse(File.ReadAllText(filePath + @"\rangeh"));

            return new LootDrop(ch, i, r);
        }

        public static void SaveLootDrop(string root, LootDrop lD)
        {

            File.WriteAllText(root + "chance", "" + lD.chance);
            File.WriteAllText(root + "rangel", "" + lD.range[0]);
            File.WriteAllText(root + "rangeh", "" + lD.range[1]);

            Directory.CreateDirectory(root + @"item");
            Item.SaveItem(root + @"item\", lD.drop);
        }
    }
}
