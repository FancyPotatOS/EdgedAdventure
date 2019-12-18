using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    public class LootDrop
    {
        public float chance;
        public uint drop;
        public uint[] range;

        public static Random dropCalc = new Random();

        public LootDrop(float c, uint i, uint[] r)
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
                Item item = Item.GetItem(drop, calcAmount);
                return item;
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
            uint item = UInt32.Parse(File.ReadAllText(filePath + @"\drop"));

            uint[] r = new uint[2];
            r[0] = UInt32.Parse(File.ReadAllText(filePath + @"\rangel"));
            r[1] = UInt32.Parse(File.ReadAllText(filePath + @"\rangeh"));

            return new LootDrop(ch, item, r);
        }

        public static void SaveLootDrop(string root, LootDrop lD)
        {

            File.WriteAllText(root + "chance", "" + lD.chance);
            File.WriteAllText(root + "rangel", "" + lD.range[0]);
            File.WriteAllText(root + "rangeh", "" + lD.range[1]);
            File.WriteAllText(root + "drop", "" + lD.drop);
        }
    }
}
