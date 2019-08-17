using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    class Entity
    {

        public uint id;

        public uint maxHealth;
        public uint health;
        public string tile;
        public string name;
        public LootDrop[] drops;
        public uint attackDamage;

        public float X;
        public float Y;

        public int chunkX;
        public int chunkY;
        public int layer;

        public Entity(uint i, uint mH, uint h, string t, string n, LootDrop[] d, uint aD, float x, float y, int cX, int cY, int l)
        {
            id = i;
            maxHealth = mH;
            health = h;
            tile = t;
            name = n;
            drops = d;
            attackDamage = aD;

            X = x;
            Y = y;

            chunkX = cX;
            chunkY = cY;
            layer = l;
        }

        public static Entity GetEntity(uint value)
        {
            if (value == 0)
            {

            }
            return new Entity(uint.MaxValue, 0, 0, @"text\question_mark", "Blank Entity", new LootDrop[0], 0, -1F, -1F, -1, -1, -1);
        }

        public static Entity GetEntity(string filePath)
        {
            if (!Directory.Exists(filePath)) return null;

            uint id = UInt32.Parse(File.ReadAllText(filePath + @"\id"));
            uint mH = UInt32.Parse(File.ReadAllText(filePath + @"\maxHealth"));
            uint h = UInt32.Parse(File.ReadAllText(filePath + @"\health"));
            string t = File.ReadAllText(filePath + @"\tile");
            string n = File.ReadAllText(filePath + @"\name");

            LootDrop[] ds = null;
            if (Directory.Exists(filePath + @"lootDrops"))
            {
                string[] dirs = Directory.GetDirectories(filePath + @"lootDrops");
                ds = new LootDrop[dirs.Length];
                for (int i = 0; i < dirs.Length; i++)
                {
                    ds[i] = LootDrop.GetLootDrop(dirs[i] + @"\" + i);
                }
            }

            uint aD = UInt32.Parse(File.ReadAllText(filePath + @"\attackDamage"));
            float x = float.Parse(File.ReadAllText(filePath + @"\X"));
            float y = float.Parse(File.ReadAllText(filePath + @"\Y"));
            int cX = Int32.Parse(File.ReadAllText(filePath + @"\chunkX"));
            int cY = Int32.Parse(File.ReadAllText(filePath + @"\chunkY"));
            int l = Int32.Parse(File.ReadAllText(filePath + @"\layer"));

            return new Entity(id, mH, h, t, n, ds, aD, x, y, cX, cY, l);

        }

        public static void SaveEntity(string root, Entity e)
        {

            File.WriteAllText(root + "id", "" + e.id);
            File.WriteAllText(root + "maxHealth", "" + e.maxHealth);
            File.WriteAllText(root + "health", "" + e.health);
            File.WriteAllText(root + "tile", "" + e.tile);
            File.WriteAllText(root + "name", "" + e.name);
            File.WriteAllText(root + "attackDamage", "" + e.attackDamage);
            File.WriteAllText(root + "X", "" + e.X);
            File.WriteAllText(root + "Y", "" + e.Y);
            File.WriteAllText(root + "chunkX", "" + e.chunkX);
            File.WriteAllText(root + "chunkY", "" + e.chunkY);
            File.WriteAllText(root + "layer", "" + e.layer);

            Directory.CreateDirectory(root + @"lootDrops");
            for (int i = 0; i < e.drops.Length; i++)
            {
                Directory.CreateDirectory(root + @"lootDrops\" + i);
                LootDrop.SaveLootDrop(root + @"lootDrops\" + i + @"\", e.drops[i]);
            }
        }

    }
}
