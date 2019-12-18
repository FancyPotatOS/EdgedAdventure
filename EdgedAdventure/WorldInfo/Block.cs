using System;

namespace EdgedAdventure
{
    public class Block
    {

        public readonly uint id;
        public readonly string tile;
        public byte damage;
        public byte maxDamage;
        public readonly byte resilience;
        public readonly LootDrop[] drops;
        public readonly bool solid;
        public static Random rand = new Random();

        public Block(uint ID, string t, byte r, LootDrop[] dr, bool s)
        {
            id = ID;
            tile = t;
            damage = 0;
            maxDamage = 0;
            resilience = r;
            drops = dr;
            solid = s;
        }

        public static Block GetBlock(uint value)
        {
            if (value == 0)
            {
                Block b = new Block(value, @"tiles\stone_wall", 0, new LootDrop[] { new LootDrop(1, 1, new uint[] { 1, 1 }) }, true);
                {
                    b.maxDamage = 1;
                    b.damage = 1;
                }
                return b;
            }
            else if (value == 1)
            {
                int pick = (int)(rand.NextDouble() * 5);
                Block b = new Block(value, (@"tiles\dirt" + pick), 5, new LootDrop[] { new LootDrop(1, 0, new uint[] { 1, 1 }) }, false);
                {
                    b.maxDamage = 5;
                    b.damage = 5;
                }
                return b;
            }
            return new Block(value, @"text\question_mark", 0, new LootDrop[0], false);
        }

        public bool Damage(Item tool)
        {
            if (tool == null)
            {
                if (damage == 0)
                {
                    return true;
                }
                else
                {
                    damage--;
                }
            }
            else if (tool.canDig)
            {
                if (tool.pierce >= resilience)
                {
                    if (((int)damage) - tool.mineBonus <= 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (damage == 0)
                {
                    return true;
                }
                else
                {
                    damage--;
                }
            }
            return false;
        }

    }
}
