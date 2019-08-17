using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EdgedAdventure
{
    class Chunk
    {

        public Space[,] spaces;
        public List<Entity> ents;
        public int[] coords;

        public Chunk(Space[,] s, List<Entity> e, int[] c)
        {

            spaces = s;
            ents = e;
            coords = c;
        }

        public Chunk(int x, int y, int l)
        {
            spaces = new Space[8, 8];
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    List<Block> bks = new List<Block>();
                    {
                        bks.Add(Block.GetBlock(0));
                    }
                    spaces[i, j] = new Space(bks);
                }
            }
            ents = new List<Entity>();
            coords = new int[]  { x, y, l };
        }

        public static Chunk GetChunk(int cX, int cY, int l)
        {

            { }

            StreamReader file;
            string root = @"world\chunks\" + cX + "," + cY + "," + l + @"\";

            if (!Directory.Exists(@"world\chunks\" + cX + "," + cY + "," + l))
            {
                return new Chunk(cX, cY, l);
            }

            string chunkRaw = null;
            Space[,] spaces = new Space[8, 8];
            if (File.Exists(root + @"blocks.blocks"))
            {
                file = File.OpenText(root + @"blocks.blocks");
                chunkRaw = file.ReadToEnd();
                file.Close();
            }

            List<Entity> e = new List<Entity>();
            if (Directory.Exists(root + @"ents"))
            {
                string[] entDirs = Directory.GetDirectories(root + @"ents");
                for (int i = 0; i < entDirs.Length; i++)
                {
                    e.Add(Entity.GetEntity(entDirs[i]));
                }
            }

            if (chunkRaw != null)
            {
                List<Block> currBlocks = new List<Block>();
                int x = 0;
                int y = 0;
                string coll = "";
                for (int i = 0; i < chunkRaw.Length; i++)
                {
                    if (chunkRaw[i] != ',' && chunkRaw[i] != '&')
                    {
                        coll += chunkRaw[i];
                    }
                    else if (chunkRaw[i] == '&')
                    {
                        currBlocks.Add(Block.GetBlock((uint)Int32.Parse(coll)));
                        coll = "";
                    }
                    else if (chunkRaw[i] == ',')
                    {
                        if (coll != "")
                        {
                            currBlocks.Add(Block.GetBlock((uint)Int32.Parse(coll)));
                        }
                        spaces[x, y] = new Space(currBlocks);
                        currBlocks = new List<Block>();
                        coll = "";
                        
                        if (y == 7) x++;
                        y = (y + 1) % 8;
                    }
                }
                return new Chunk(spaces, e, new int[] { cX, cY, l});
            }
            else
            {
                return null;
            }
        }

        public static void SaveChunk(Chunk c)
        {
            if (c == null) return;
            string root = @"world\chunks\" + c.coords[0] + "," + c.coords[1] + "," + c.coords[2];

            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
            Directory.CreateDirectory(root);

            root += @"\";

            string coll = "";
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    List<Block> bL = c.spaces[x, y].blocks;
                    for (int i = 0; i < bL.Count; i++)
                    {
                        coll += bL[i].id;
                        if (i != (bL.Count - 1))
                        {
                            coll += "&";
                        }
                    }
                    coll += ",";
                }
            }
            File.WriteAllText(root + "blocks.blocks", coll);

            Directory.CreateDirectory(root + "ents");
            for (int i = 0; i < c.ents.Count; i++)
            {
                Directory.CreateDirectory(root + @"ents\" + i);
                Entity.SaveEntity((root + @"ents\" + i + @"\"), c.ents[i]);
            }
        }
    }
}
