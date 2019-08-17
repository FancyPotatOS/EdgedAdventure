using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgedAdventure
{
    class Player
    {
        public List<Item> inv;
        public int selItem;
        public uint health;
        public uint maxHealth;
        public float X;
        public float Y;

        public int chunkX;
        public int chunkY;
        public int layer;

        public byte moveClock;
        public byte hurtCooldown;
        public byte attackCooldown;
        public EdgedAdventure.Directions facing;

        public static float movementSpeed = 0.15F;

        public enum Action
        {
            move_up, move_down, move_left, move_right,
            attack, hurt
        }

        public Player(float x, float y, int cX, int cY, int l)
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
            attackCooldown = 0;
            facing = EdgedAdventure.Directions.down;
        }

        public void Update(Action a)
        {
            if (a == Action.move_down || a == Action.move_up || a == Action.move_left || a == Action.move_right)
            {
                moveClock = (byte)((moveClock + 1) % 28);
                if (a == Action.move_down)
                {
                    facing = EdgedAdventure.Directions.down;
                    if (Y - movementSpeed >= 8)
                    {
                        { }

                        chunkY++;
                        Y = ((Y - (8 - movementSpeed))%8);
                    }
                    else { Y += movementSpeed; }
                }
                else if (a == Action.move_up)
                {
                    facing = EdgedAdventure.Directions.up;
                    if (Y - movementSpeed < 0)
                    {
                        { }

                        chunkY--;
                        Y = ((Y + (8 - movementSpeed)) % 8);
                    }
                    else { Y -= movementSpeed; }
                }
                else if(a == Action.move_left)
                {
                    facing = EdgedAdventure.Directions.left;
                    if (X - movementSpeed < 0)
                    {
                        { }

                        chunkX--;
                        X = ((X + (8 - movementSpeed)) % 8);
                    }
                    else { X -= movementSpeed; }
                }
                else if (a == Action.move_right)
                {
                    facing = EdgedAdventure.Directions.right;
                    if (X + movementSpeed >= 8)
                    {
                        { }

                        chunkX++;
                        X = ((X - (8 - movementSpeed)) % 8);
                    }
                    else { X += movementSpeed; }
                }
            }
            else if (a == Action.attack)
            {
                attackCooldown = 50;
            }
            else if (a == Action.hurt)
            {
                hurtCooldown = 50;
            }
        }

        public string GetSprite()
        {
            return "player";
        }

    }
}
