using System;

namespace EdgedAdventure
{
    class Block
    {

        public readonly uint id;
        public readonly string tile;
        public byte damage;
        public readonly byte resilience;
        public readonly LootDrop[] drops;
        public readonly bool solid;
        public static Random rand = new Random();

        public Block(uint ID, string t, byte d, byte r, LootDrop[] dr, bool s)
        {
            id = ID;
            tile = t;
            damage = d;
            resilience = r;
            drops = dr;
            solid = s;
            if (rand != null)
            {
                rand = new Random();
            }
        }

        public static Block GetBlock(uint value)
        {
            if (value == 0)
            {
                return new Block(value, @"tiles\stone_wall", 0, 0, new LootDrop[0], false);
            }
            else if (value == 1)
            {
                int pick = (int)(rand.NextDouble() * 5);
                LootDrop[] drop = new LootDrop[1];
                drop[0] = new LootDrop(1, Item.GetItem(0, 0), new uint[] { 3, 5 });
                return new Block(value, (@"tiles\dirt" + pick), 0, 5, drop, false);
            }
            return new Block(value, @"text\question_mark", 0, 0, new LootDrop[0], false);
        }

    }
}
