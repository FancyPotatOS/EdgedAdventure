using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    public class Entity
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
        public float Z;

        public int chunkX;
        public int chunkY;
        public int layer;

        public float[,] hitbox;

        public static LoadedChunkManager LCM;
        public static EdgedAdventure Game;

        public Entity(uint i, uint mH, uint h, string t, string n, LootDrop[] d, uint aD, float x, float y, int cX, int cY, int l, float[,] hB)
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
            Z = 0;

            chunkX = cX;
            chunkY = cY;
            layer = l;

            hitbox = hB;
        }

        public virtual Object Update() {
            return null;
        }

        public virtual Object Provide(Object o)
        {
            return null;
        }

        public static Entity GetEntity(uint value)
        {
            if (value == 0)
            {

            }
            else if (value == 1)
            {
                Effect e = new Effect(0, @"text\question_mark", -1F, -1F, -1, -1, -1);
                return e;
            }
            else if (value == 2)
            {
                ItemEntity iE = new ItemEntity(-1F, -1F, -1, -1, -1, null);
                return iE;
            }
            return new Entity(uint.MaxValue, 0, 0, @"text\question_mark", "Blank Entity", new LootDrop[0], 0, -1F, -1F, -1, -1, -1, new float[2,2]{ { -0.1F, -0.1F }, { 0.1F, 0.1F } });
        }

        public static Entity GetEntity(string filePath)
        {
            if (!Directory.Exists(filePath)) return null;

            uint id = UInt32.Parse(File.ReadAllText(filePath + @"\id"));
            uint h = UInt32.Parse(File.ReadAllText(filePath + @"\health"));
            string n = File.ReadAllText(filePath + @"\name");

            LootDrop[] ds = null;
            if (Directory.Exists(filePath + @"\lootDrops"))
            {
                string[] dirs = Directory.GetDirectories(filePath + @"\lootDrops");
                ds = new LootDrop[dirs.Length];
                for (int i = 0; i < dirs.Length; i++)
                {
                    ds[i] = LootDrop.GetLootDrop(dirs[i] + @"\" + i);
                }
            }

            float x = float.Parse(File.ReadAllText(filePath + @"\X"));
            float y = float.Parse(File.ReadAllText(filePath + @"\Y"));
            int cX = Int32.Parse(File.ReadAllText(filePath + @"\chunkX"));
            int cY = Int32.Parse(File.ReadAllText(filePath + @"\chunkY"));
            int l = Int32.Parse(File.ReadAllText(filePath + @"\layer"));

            Entity e = Entity.GetEntity(id);
            e.health = h;
            e.name = n;
            e.drops = ds;
            e.X = x;
            e.Y = y;
            e.chunkX = cX;
            e.chunkY = cY;
            e.layer = l;

            e.GetExtra(filePath);

            return e;

        }

        public virtual void Save(string root)
        {

            File.WriteAllText(root + "id", "" + id);
            File.WriteAllText(root + "health", "" + health);
            File.WriteAllText(root + "name", "" + name);
            File.WriteAllText(root + "X", "" + X);
            File.WriteAllText(root + "Y", "" + Y);
            File.WriteAllText(root + "chunkX", "" + chunkX);
            File.WriteAllText(root + "chunkY", "" + chunkY);
            File.WriteAllText(root + "layer", "" + layer);

            Game.DebugBreakPoint();

            Directory.CreateDirectory(root + @"lootDrops");
            for (int i = 0; i < drops.Length; i++)
            {
                Directory.CreateDirectory(root + @"lootDrops\" + i);
                LootDrop.SaveLootDrop(root + @"lootDrops\" + i + @"\", drops[i]);
            }
        }

        // Overwrite !
        public virtual void GetExtra(string root) { }


        public virtual string GetSprite()
        {
            return tile;
        }

        public bool Attack(uint aD)
        {
            if (((int)health) - aD < 0)
            {
                return true;
            }
            else
            {
                health -= aD;
                return false;
            }
        }

    }
}
