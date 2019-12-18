using System;

namespace EdgedAdventure
{
    class Effect : Entity
    {
        public Effect(uint h, string t, float x, float y, int cX, int cY, int l) : base(1, h, h, t, "Effect", new LootDrop[0], 0, x, y, cX, cY, l, new float[,] { { 0, 0 }, { 0, 0 } }) { }

        public override Object Update()
        {
            if (health == 0)
            {
                return true;
            }
            health--;
            return null;
        }
    }
}
