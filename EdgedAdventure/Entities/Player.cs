using System;
using System.Collections.Generic;
using System.IO;

namespace EdgedAdventure
{
    public class Player : Entity
    {
        public List<Item> inv;
        public int selItem;

        public byte moveClock;
        public byte hurtCooldown;
        public uint maxEnergy;
        public int energy;
        public byte energyHesitate;
        public EdgedAdventure.Directions facing;

        public int sel;

        public static float movementSpeed = 0.05F;

        public enum Action
        {
            move_up, move_down, move_left, move_right,
            attack, none, test
        }

        public Player(float x, float y, int cX, int cY, int l) : base(uint.MaxValue, 20, 20, @"player\player_down_0", "Player", null, 5, x, y, cX, cY, l, new float[2, 2] { { -0.25F, 0F }, { 0.25F, 0.5F } })
        {
            inv = new List<Item>();
            selItem = -1;
            health = 20;
            maxHealth = 20;

            X = x;
            Y = y;

            chunkX = cX;
            chunkY = cY;
            layer = l;

            moveClock = 0;
            hurtCooldown = 0;
            maxEnergy = 400;
            energy = (int)maxEnergy;
            facing = EdgedAdventure.Directions.down;

            sel = -1;
        }

        public void Update(Action a)
        {
            if (a == Action.move_down || a == Action.move_up || a == Action.move_left || a == Action.move_right)
            {
                moveClock = (byte)((moveClock + 1) % 64);
                if (a == Action.move_down)
                {
                    facing = EdgedAdventure.Directions.down;
                    float[] raw = new float[2];
                    raw[0] = (chunkX * 8) + X;
                    raw[1] = (chunkY * 8) + Y + movementSpeed;
                    if (!LCM.IsBlocking(raw))
                    {
                        if (Y + movementSpeed >= 8)
                        {
                            { }

                            chunkY++;
                            Y = ((Y - (8 - movementSpeed)) % 8);
                        }
                        else { Y += movementSpeed; }
                    }
                }
                else if (a == Action.move_up)
                {
                    facing = EdgedAdventure.Directions.up;
                    float[] raw = new float[2];
                    raw[0] = (chunkX * 8) + X;
                    raw[1] = (chunkY * 8) + Y - movementSpeed;
                    

                    if (!LCM.IsBlocking(raw))
                    {
                        if (Y - movementSpeed < 0)
                        {
                            { }

                            chunkY--;
                            Y = ((Y + (8 - movementSpeed)) % 8);
                        }
                        else { Y -= movementSpeed; }
                    }
                }
                else if(a == Action.move_left)
                {
                    facing = EdgedAdventure.Directions.left;
                    float[] raw = new float[2];
                    raw[0] = (chunkX * 8) + X - movementSpeed;
                    raw[1] = (chunkY * 8) + Y;
                    if (!LCM.IsBlocking(raw))
                    {
                        if (X - movementSpeed < 0)
                        {
                            { }

                            chunkX--;
                            X = ((X + (8 - movementSpeed)) % 8);
                        }
                        else { X -= movementSpeed; }
                    }
                }
                else if (a == Action.move_right)
                {
                    facing = EdgedAdventure.Directions.right;
                    float[] raw = new float[2];
                    raw[0] = (chunkX * 8) + X + movementSpeed;
                    raw[1] = (chunkY * 8) + Y;
                    if (!LCM.IsBlocking(raw))
                    {
                        if (X + movementSpeed >= 8)
                        {
                            { }

                            chunkX++;
                            X = ((X - (8 - movementSpeed)) % 8);
                        }
                        else { X += movementSpeed; }
                    }
                }
            }
            else if (a == Action.attack)
            {
                if (energy > 0 && sel == -1)
                {

                    energy -= 40;
                    if ((0 <= moveClock && moveClock < 16) || (32 <= moveClock && moveClock < 48))
                    {
                        moveClock += 16;
                        moveClock %= 64;
                    }
                    else
                    {
                        moveClock += 32;
                        moveClock %= 64;
                    }

                    Game.DebugBreakPoint();

                    Effect slash = (Effect) Entity.GetEntity(1);
                    slash.chunkX = this.chunkX;
                    slash.chunkY = this.chunkY;
                    slash.layer = this.layer;
                    int[] dir = DirVector(facing);
                    slash.X = this.X + (dir[0] * 0.8F);
                    slash.Y = this.Y + (dir[1] * 0.8F);
                    slash.health = 1;
                    slash.tile = @"particles\slash_" + DirString(facing);
                    int[] index = LCM.GetIndices(new float[] { ((slash.chunkX * 8) + slash.X), ((slash.chunkY * 8) + slash.Y) });
                    LCM.world[index[0], index[1]].ents.Add(slash);

                    float[] raw = { ((chunkX * 8) + slash.X), ((chunkY * 8) + slash.Y) };
                    Entity attacked = LCM.GetAttackableEntityInRadius(raw, 0.8F);
                    if (attacked != null)
                    {
                        uint attackDamage = 1;
                        if (selItem != -1)
                        {
                            attackDamage += inv[selItem].attackBonus;
                        }
                        if (attacked.Attack(attackDamage))
                        {
                            LCM.RemoveEntity(attacked);
                        }
                    }
                    else
                    {
                        Item use = null;
                        if (selItem != -1)
                        {
                            use = inv[selItem];
                        }
                        LCM.MineAt(raw, use);
                    }

                }
            }
            else if (a == Action.test)
            {

                //Game.DebugBreakPoint();

                energy = 500;
            }

            X %= 8;
            Y %= 8;
        }

        public void UpdateCounters()
        {
            if (energy < 0)
            {
                if ((energyHesitate % 2) == 0)
                {
                    energy++;
                }
                energyHesitate = (byte) ((energyHesitate + 1) % 2);
            }
            if (energy >= 0)
            {
                energy = Math.Min((int)maxEnergy, (energy + 1));
            }
        }

        public override string GetSprite()
        {
            if (0 <= moveClock && moveClock < 16)
            {
                return @"player\player_" + DirString(facing) + "_0";
            }
            else if (16 <= moveClock && moveClock < 32)
            {
                return @"player\player_" + DirString(facing) + "_1";
            }
            else if (32 <= moveClock && moveClock < 48)
            {
                return @"player\player_" + DirString(facing) + "_0";
            }
            else if (48 <= moveClock && moveClock < 64)
            {
                return @"player\player_" + DirString(facing) + "_2";
            }
            return @"text\question_mark";
        }

        public static string DirString(EdgedAdventure.Directions dir)
        {
            if (dir == EdgedAdventure.Directions.up)
            {
                return "up";
            }
            else if (dir == EdgedAdventure.Directions.down)
            {
                return "down";
            }
            else if (dir == EdgedAdventure.Directions.left)
            {
                return "left";
            }
            else if (dir == EdgedAdventure.Directions.right)
            {
                return "right";
            }
            return "middle";
        }

        public static int[] DirVector(EdgedAdventure.Directions dir)
        {
            if (dir == EdgedAdventure.Directions.up)
            {
                return new int[] { 0, -1 };
            }
            else if (dir == EdgedAdventure.Directions.down)
            {
                return new int[] { 0, 1 };
            }
            else if (dir == EdgedAdventure.Directions.left)
            {
                return new int[] { -1, 0 };
            }
            else if (dir == EdgedAdventure.Directions.right)
            {
                return new int[] { 1, 0 };
            }
            return new int[] { 0, 0 };
        }
        
        public void SavePlayer()
        {
            string root = @"world\player\";
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
            Directory.CreateDirectory(root);

            File.WriteAllText(root + "health", "" + health);

            Directory.CreateDirectory(root + "inv");
            for (int i = 0; i < inv.Count; i++)
            {
                Directory.CreateDirectory(root + @"inv\" + i);
                Item.SaveItem(root + @"inv\" + i + @"\", inv[i]);
            }

            Directory.CreateDirectory(root + "coords");
            root += @"coords\";
            File.WriteAllText(root + "cX", "" + chunkX);
            File.WriteAllText(root + "cY", "" + chunkY);
            File.WriteAllText(root + "l", "" + layer);
            File.WriteAllText(root + "X", "" + X);
            File.WriteAllText(root + "Y", "" + Y);

        }

        public static Player GetPlayer()
        {
            Player p = null;

            if (Directory.Exists(@"world\player"))
            {
                string root = @"world\player\";

                uint health = UInt32.Parse(File.ReadAllText(root + "health"));

                List<Item> inv = new List<Item>();
                string[] dir = Directory.GetDirectories(root + "inv");
                for (int i = 0; i < dir.Length; i++)
                {
                    inv.Add(Item.GetItem(root + @"inv\" + i));
                }

                root += @"coords\";

                int cX = Int32.Parse(File.ReadAllText(root + "cX"));
                int cY = Int32.Parse(File.ReadAllText(root + "cY"));
                int l = Int32.Parse(File.ReadAllText(root + "l"));

                float X = float.Parse(File.ReadAllText(root + "X"));
                float Y = float.Parse(File.ReadAllText(root + "Y"));

                p = new Player(X, Y, cX, cY, l);
                {
                    p.inv = inv;
                }
                return p;
            }
            else
            {
                return p;
            }

        }

    }
}
