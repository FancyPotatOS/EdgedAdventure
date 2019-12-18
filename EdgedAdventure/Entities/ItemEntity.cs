using System;
using System.IO;

namespace EdgedAdventure
{
    public class ItemEntity : Entity
    {

        public Item item;
        public float[] vel;
        public uint bounce;
        
        public static Random rand = new Random();


        public ItemEntity(float x, float y, int cX, int cY, int l, Item i) : base(2, 100, 100, @"text\question_mark", "Item", new LootDrop[0], 0, x, y, cX, cY, l, new float[,] { { -0.11F, -0.11F }, { 0.11F, 0.11F } })
        {
            if (i != null)
            {
                tile = i.tile;
            }
            item = i;
            vel = new float[3];
            bounce = 3;
        }

        public override object Update()
        {
            health--;
            if (health == 0)
            {
                return true;
            }
            if (vel[0] != 0)
            {

                //Entity.Game.DebugBreakPoint();

                float[] raw = { ((chunkX * 8) + X), ((chunkY * 8) + Y) };
                float[] dir = new float[2];
                dir[1] = 0;
                if (vel[0] > 0)
                {
                    vel[1] /= (1.25F);
                    dir[0] = Math.Max(-0.01F + vel[0], 0);
                    vel[0] = dir[0];
                }
                else if (vel[0] < 0)
                {
                    vel[1] /= (1.25F);
                    dir[0] = Math.Min(0.01F + vel[0], 0);
                    vel[0] = dir[0];
                }

                if (LCM.IsBlocking(raw))
                {

                    Entity.Game.DebugBreakPoint();

                    vel[0] = 0;
                }
                else
                {
                    LCM.MoveDir(this, dir);
                }
            }
            if (vel[1] != 0)
            {

                //Entity.Game.DebugBreakPoint();

                float[] raw = { ((chunkX * 8) + X), ((chunkY * 8) + Y) };
                float[] dir = new float[2];
                dir[0] = 0;
                if (vel[1] > 0)
                {
                    vel[1] /= (1.25F);
                    dir[1] = Math.Max(-0.1F + vel[1], 0);
                    vel[1] = dir[1];
                }
                else if (vel[1] < 0)
                {
                    vel[1] /= (1.25F);
                    dir[1] = Math.Min(0.1F + vel[1], 0);
                    vel[1] = dir[1];
                }

                if (LCM.IsBlocking(raw))
                {

                    Entity.Game.DebugBreakPoint();

                    vel[1] = 0;
                }
                else
                {
                    LCM.MoveDir(this, dir);
                }
            }
            if (bounce > 0)
            {
                Z += vel[2];
                if (Z < 0)
                {
                    Z = 0;
                    bounce--;
                    vel[2] = (bounce * 0.1F) + 0.03F;
                }
                if (vel[2] > -0.5F)
                {
                    vel[2] -= 0.08F;
                }
            }

            return null;
        }

        public override void GetExtra(string root)
        {
            item = Item.GetItem(root + @"\item\");
            tile = item.tile;
        }

        public override void Save(string root)
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

            // Save Item
            Directory.CreateDirectory(root + @"item\");
            Item.SaveItem(root + @"item\", item);
        }

        public static float[] VelCalc()
        {
            float[] velocity = new float[3];

            velocity[0] = (float)rand.NextDouble() / 4;
            velocity[1] = (float)rand.NextDouble() / 4;
            velocity[0] -= velocity[0]/2;
            velocity[1] -= velocity[1]/2;
            velocity[2] = 0.15F;

            return velocity;
        }

    }
}
