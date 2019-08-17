using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    class Space
    {
        public List<Block> blocks;

        public Space(List<Block> b)
        {
            blocks = b;
        }

        public string GetTile()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[(blocks.Count-i) - 1] != null)
                {
                    return blocks[i].tile;
                }
            }
            return @"text\question_mark";
        }

    }
}
