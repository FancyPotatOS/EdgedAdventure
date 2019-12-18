using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    public class Space
    {
        public List<Block> blocks;

        public Space(List<Block> b)
        {
            blocks = b;
        }

        public string GetTile()
        {
            if (blocks.Count != 0)
            {
                return blocks[blocks.Count - 1].tile;
            }
            else
            {
                return @"text\question_mark";
            }
        }

        public Space GetSpace(string saveString)
        {
            List<Block> bs = new List<Block>();
            string coll = "";
            for (int i = 0; i < saveString.Length; i++)
            {
                if (saveString[i] == '&')
                {
                    bs.Add(Block.GetBlock((uint)UInt32.Parse(coll)));
                    coll = "";
                }
            }
            bs.Add(Block.GetBlock((uint)UInt32.Parse(coll)));
            return new Space(bs);
        }

        public bool IsSolid()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].solid)
                {
                    return true;
                }
            }
            return false;
        }

        public string ToSaveString()
        {
            string coll = "";
            for (int i = 0; i < blocks.Count; i++)
            {
                coll += blocks[i].id;
                if (i != (blocks.Count - 1))
                {
                    coll += "&";
                }
            }
            return coll;
        }

    }
}
