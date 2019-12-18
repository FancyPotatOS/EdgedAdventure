using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace EdgedAdventure
{
    public class LoadedChunkManager
    {

        readonly List<ThreadQueue> threads;
        readonly List<SaveEntity> saveEnt;
        
        public Chunk[,] world;
        public Chunk[,] loadingWorld;
        public int worldSize;
        bool isLoading;
        readonly EdgedAdventure Game;

        public enum Directions
        {
            up, down, left, right, middle
        }

        public LoadedChunkManager(EdgedAdventure g, Chunk[,] w)
        {

            Game = g;

            world = w;
            worldSize = w.GetLength(0);
            loadingWorld = null;
            threads = new List<ThreadQueue>();
            saveEnt = new List<SaveEntity>();
            isLoading = false;
        }

        public void Update()
        {
            // Global Thread Updater/Remover
            if (threads.Count > 0)
            {
                if (threads[0].Update())
                {
                    threads.RemoveAt(0);
                }
            }

            if (world[worldSize / 2, worldSize / 2].coords[0] != Game.player.chunkX || world[worldSize / 2, worldSize / 2].coords[1] != Game.player.chunkY)
            {
                if (!isLoading)
                {
                    isLoading = true;
                    loadingWorld = new Chunk[worldSize, worldSize];
                    for (int x = 0; x < worldSize; x++)
                    {
                        for (int y = 0; y < worldSize; y++)
                        {
                            loadingWorld[x, y] = world[x, y];
                        }
                    }
                    PrelimMoveWorld();
                    Thread t = new Thread(MoveChunks);
                    threads.Add(new ThreadQueue(t));
                }
            }
        }

        private void PrelimMoveWorld()
        {
            if (Game.player.chunkY < world[worldSize / 2, worldSize / 2].coords[1])
            {
                for (uint x = 0; x < worldSize; x++)
                {
                    for (int y = (worldSize - 2); 0 <= y; y--)
                    {
                        world[x, y + 1] = world[x, y];
                    }
                }
            }
            else if (Game.player.chunkY > world[worldSize / 2, worldSize / 2].coords[1])
            {
                for (uint x = 0; x < worldSize; x++)
                {
                    for (uint y = 0; y < (worldSize - 1); y++)
                    {
                        world[x, y] = world[x, y + 1];
                    }
                }
            }
            else if (Game.player.chunkX < world[worldSize / 2, worldSize / 2].coords[0])
            {
                for (int x = (worldSize - 2); 0 <= x; x--)
                {
                    for (uint y = 0; y < worldSize; y++)
                    {
                        world[x + 1, y] = world[x, y];
                    }
                }
            }
            else if (Game.player.chunkX > world[worldSize / 2, worldSize / 2].coords[0])
            {
                for (uint x = 0; x < (worldSize - 1); x++)
                {
                    for (uint y = 0; y < worldSize; y++)
                    {
                        world[x, y] = world[x + 1, y];
                    }
                }
            }
        }


        // Thread Function
        private void MoveChunks()
        {
            
            if (Game.player.chunkX > loadingWorld[worldSize / 2, worldSize / 2].coords[0])
            {
                SaveChunks(Directions.right);
                Chunk[] cs = LoadChunks(Directions.right);
                for (uint x = 0; x < (worldSize - 1); x++)
                {
                    for (uint y = 0; y < worldSize; y++)
                    {
                        loadingWorld[x, y] = loadingWorld[x + 1, y];
                    }
                }
                for (int i = 0; i < worldSize; i++)
                {
                    loadingWorld[(worldSize - 1), i] = cs[i];
                }
            }
            if (Game.player.chunkX < loadingWorld[worldSize / 2, worldSize / 2].coords[0])
            {
                SaveChunks(Directions.left);
                Chunk[] cs = LoadChunks(Directions.left);
                for (int x = (worldSize - 2); 0 <= x; x--)
                {
                    for (uint y = 0; y < worldSize; y++)
                    {
                        loadingWorld[x + 1, y] = loadingWorld[x, y];
                    }
                }
                for (int i = 0; i < worldSize; i++)
                {
                    loadingWorld[0, i] = cs[i];
                }
            }
            if (Game.player.chunkY > loadingWorld[worldSize / 2, worldSize / 2].coords[1])
            {
                SaveChunks(Directions.down);
                Chunk[] cs = LoadChunks(Directions.down);
                for (uint x = 0; x < worldSize; x++)
                {
                    for (uint y = 0; y < (worldSize - 1); y++)
                    {
                        loadingWorld[x, y] = loadingWorld[x, y + 1];
                    }
                }
                for (int i = 0; i < worldSize; i++)
                {
                    loadingWorld[i, (worldSize - 1)] = cs[i];
                }
            }
            if (Game.player.chunkY < loadingWorld[worldSize / 2, worldSize / 2].coords[1])
            {
                SaveChunks(Directions.up);
                Chunk[] cs = LoadChunks(Directions.up);
                for (uint x = 0; x < worldSize; x++)
                {
                    for (int y = (worldSize - 2); 0 <= y; y--)
                    {
                        loadingWorld[x, y + 1] = loadingWorld[x, y];
                    }
                }
                for (int i = 0; i < worldSize; i++)
                {
                    loadingWorld[i, 0] = cs[i];
                }
            }
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    world[x, y] = loadingWorld[x, y];
                }
            }
            isLoading = false;
        }


        private Chunk[] LoadChunks(Directions dir)
        {

            Chunk[] cs = new Chunk[worldSize];

            if (dir == Directions.up)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    cs[i] = Chunk.GetChunk(loadingWorld[i, 0].coords[0], loadingWorld[i, 0].coords[1] - 1, loadingWorld[i, 0].coords[2]);
                }
            }
            else if (dir == Directions.down)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    cs[i] = Chunk.GetChunk(loadingWorld[i, 4].coords[0], loadingWorld[i, 4].coords[1] + 1, loadingWorld[i, 4].coords[2]);
                }
            }
            else if (dir == Directions.left)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    cs[i] = Chunk.GetChunk(loadingWorld[0, i].coords[0] - 1, loadingWorld[0, i].coords[1], loadingWorld[0, i].coords[2]);
                }
            }
            else if (dir == Directions.right)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    cs[i] = Chunk.GetChunk(loadingWorld[4, i].coords[0] + 1, loadingWorld[4, i].coords[1], loadingWorld[4, i].coords[2]);
                }
            }
            return cs;
        }

        private void SaveChunks(Directions dir)
        {
            if (dir == Directions.up)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    Chunk.SaveChunk(loadingWorld[i, (worldSize - 1)]);
                }
            }
            else if (dir == Directions.down)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    Chunk.SaveChunk(loadingWorld[i, 0]);
                }
            }
            else if (dir == Directions.left)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    Chunk.SaveChunk(loadingWorld[(worldSize - 1), i]);
                }
            }
            else if (dir == Directions.right)
            {
                for (uint i = 0; i < worldSize; i++)
                {
                    Chunk.SaveChunk(loadingWorld[0, i]);

                }
            }
        }

        public void CreateWorld()
        {

            int countX = 0;
            int countY = countX;
            for (int x = (Game.player.chunkX - (worldSize / 2)); x < (Game.player.chunkX + (worldSize / 2) + 1); x++)
            {
                for (int y = (Game.player.chunkY - (worldSize / 2)); y < (Game.player.chunkY + (worldSize / 2) + 1); y++)
                {
                    world[countX, countY] = Chunk.GetChunk(x, y, 0);
                    countY++;
                }
                countY = 0;
                countX++;
            }
        }

        public void SaveWorld()
        {
            if (isLoading)
            {
                return;
            }
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    Chunk.SaveChunk(world[x, y]);   
                }
            }
        }

        public bool IsBlocking(float[] raw)
        {

            while (raw[0] < 0)
            {
                raw[0] += 8;
            }
            while (raw[1] < 0)
            {
                raw[1] += 8;
            }
            int[] index = { (int)(raw[0] % 8), (int)(raw[1] % 8) };
            int[] coords = GetIndices(raw);

            if (coords[0] < 0 || coords[0] >= worldSize || coords[1] < 0 || coords[1] >= worldSize)
            {
                return false;
            }

            if (world[coords[0], coords[1]].spaces[index[0], index[1]].IsSolid())
            {
                return true;
            }

            List<Entity> ents = world[coords[0], coords[1]].ents;
            for (int i = 0; i < ents.Count; i++)
            {
                float[,] range = { { ents[i].hitbox[0,0] + ents[i].X, ents[i].hitbox[0, 1] + ents[i].X }, { ents[i].hitbox[1, 0] + ents[i].Y, ents[i].hitbox[1, 1] + ents[i].Y } };
                float[] indexFloat = { raw[0] % 8, raw[1] % 8 };
                if (ContainsPoint(range, indexFloat))
                {
                    return true;
                }
            }

            /*
            for (float x = hitbox[0, 0]; x < hitbox[1, 0]; x += 0.1F)
            {
                for (float y = hitbox[0, 1]; y < hitbox[1, 1]; y += 0.1F)
                {
                    if (-0.001 < x && x < 0.001)
                    {
                        x = 0;
                    }
                    if (-0.001 < y && y < 0.001)
                    {
                        y = 0;
                    }
                    float[] newDir = new float[] { dir[0] + x, dir[1] + y };

                    //Game.DebugBreakPoint();
                    { }
                    float[] worldIndex = new float[] { (coords[0] + newDir[0] + 8) % 8, (coords[1] + newDir[1] + 8) % 8 };
                    worldIndex[0] += 8;
                    worldIndex[1] += 8;
                    worldIndex[0] %= 8;
                    worldIndex[1] %= 8;
                    float[] tempWorldChunk = { (coords[0] + newDir[0]), (coords[1] + newDir[1]) };
                    int[] worldChunk = GetIndices(tempWorldChunk);

                    if (worldChunk[0] < 0 || worldChunk[1] < 0)
                    {
                        Game.DebugBreakPoint();

                        return true;
                    }
                    else if (worldChunk[0] > worldSize || worldChunk[1] > worldSize)
                    {
                        Game.DebugBreakPoint();

                        return true;
                    }
                    if (world[worldChunk[0], worldChunk[1]].spaces[(int)worldIndex[0], (int)worldIndex[1]].IsSolid())
                    {
                        return true;
                    }
                    else if (world[worldChunk[0], worldChunk[1]].EntityIsTouching(worldIndex))
                    {

                        Game.DebugBreakPoint();

                        return true;
                    }
                }
            }

            */

            return false;
        }

        public int[] GetIndices(float[] raw)
        {
            if (raw[0] < 0)
            {
                raw[0] -= 8;
            }
            if (raw[1] < 0)
            {
                raw[1] -= 8;
            }

            int[] index = new int[] { (int)raw[0] / 8, (int)raw[1] / 8 };
            index[0] = index[0] - world[0, 0].coords[0];
            index[1] = index[1] - world[0, 0].coords[1];

            return index;
        }

        public void LoadWorld(int[] top_left, int size)
        {
            if (isLoading)
            {
                return;
            }
            worldSize = size;
            world = new Chunk[size, size];
            loadingWorld = null;
            for (int x = top_left[0]; x < top_left[0] + worldSize; x++)
            {
                for (int y = top_left[1]; y < top_left[1] + worldSize; y++)
                {
                    world[x - top_left[0], y - top_left[1]] = Chunk.GetChunk(x, y, top_left[2]);
                }
            }
        }

        public Entity GetEntityInRadius(float[] raw, float rad)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    Chunk curr = world[x, y];
                    for (int i = 0; i < curr.ents.Count; i++)
                    {
                        Entity currEnt = curr.ents[i];
                        float[] currRaw = { ((currEnt.chunkX * 8) + currEnt.X), ((currEnt.chunkY * 8) + currEnt.Y) };
                        float dis = (float)Math.Sqrt((Math.Pow((currRaw[0] - raw[0]), 2)) + (Math.Pow((currRaw[1] - raw[1]), 2)));
                        if (dis <= rad)
                        {
                            return currEnt;
                        }
                    }
                }
            }
            return null;
        }

        public Entity GetAttackableEntityInRadius(float[] raw, float rad)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    Chunk curr = world[x, y];
                    for (int i = 0; i < curr.ents.Count; i++)
                    {
                        Entity currEnt = curr.ents[i];
                        float hitboxSize = ((currEnt.hitbox[0, 0] - currEnt.hitbox[1, 0]));
                        if (hitboxSize == 0)
                        {
                            return null;
                        }

                        float[] currRaw = { ((currEnt.chunkX * 8) + currEnt.X), ((currEnt.chunkY * 8) + currEnt.Y) };
                        float dis = (float)Math.Sqrt((Math.Pow((currRaw[0] - raw[0]), 2)) + (Math.Pow((currRaw[1] - raw[1]), 2)));
                        if (dis <= rad)
                        {
                            return currEnt;
                        }
                    }
                }
            }
            return null;
        }

        public void RemoveEntity(Entity e)
        {
            int[] index = GetIndices(new float[] { ((e.chunkX * 8) + e.X), ((e.chunkY * 8) + e.Y) });
            world[index[0], index[1]].ents.Remove(e);
        }

        public void MineAt(float[] raw, Item tool)
        {
            while (raw[0] < 0)
            {
                raw[0] += 8;
            }
            while (raw[1] < 0)
            {
                raw[1] += 8;
            }

            int[] worldIndex = GetIndices(raw);
            Chunk c = world[worldIndex[0], worldIndex[1]];

            int[] blockIndex = { (int)(raw[0] % 8), (int)(raw[1] % 8) };
            Space s = c.spaces[blockIndex[0], blockIndex[1]];

            List<Block> blocks = s.blocks;

            if (blocks.Count > 0)
            {
                Block block = blocks[blocks.Count - 1];
                
                if (block.Damage(tool))
                {
                    blocks.Remove(block);
                    LootDrop[] loot = block.drops;
                    for (int i = 0; i < loot.Length; i++)
                    {
                        Item curr = loot[i].GetDrop();
                        for(int j = 0; j < curr.amount; j++)
                        {
                            Item currSingle = curr.Copy();
                            currSingle.amount = 1;
                            int[] chunkCoord = { (int)(raw[0] / 8), (int)(raw[1] / 8) };
                            if (raw[0] < 0)
                            {
                                chunkCoord[0]--;
                            }
                            if (raw[1] < 0)
                            {
                                chunkCoord[1]--;
                            }
                            ItemEntity ie = new ItemEntity(blockIndex[0], blockIndex[1], chunkCoord[0], chunkCoord[1], world[0,0].coords[2], currSingle);
                            {
                                ie.vel = ItemEntity.VelCalc();
                                ie.X += 0.5F;
                                ie.Y += 0.5F;
                                ie.bounce = 3;
                            }
                            world[worldIndex[0], worldIndex[1]].ents.Add(ie);
                        }
                    }
                }
            }
        }

        public void MoveDir(Entity e, float[] mag)
        {

            int[] index = { -1, -1 };
            {
                bool found = false;
                for (int x = 0; x < worldSize && !found; x++)
                {
                    for (int y = 0; y < worldSize && !found; y++)
                    {
                        if (world[x, y].ents.Contains(e))
                        {
                            found = true;
                            index[0] = x;
                            index[1] = y;
                            break;
                        }
                    }
                }
            }

            if (index[0] == -1 || index[1] == -1)
            {
                return;
            }
            world[index[0], index[1]].ents.Remove(e);

            e.X += mag[0];
            e.Y += mag[1];

            bool movedChunk = false;
            int[] chunkMove = new int[2];
            if (e.X < 0)
            {

                Game.DebugBreakPoint();

                movedChunk = true;
                chunkMove[0] = -1;
                e.chunkX--;
            }
            else if (e.X >= 8)
            {

                Game.DebugBreakPoint();

                movedChunk = true;
                chunkMove[0] = 1;
                e.chunkX++;
            }
            else
            {
                chunkMove[0] = 0;
            }

            if (e.Y < 0)
            {

                Game.DebugBreakPoint();

                movedChunk = true;
                chunkMove[1] = -1;
                e.chunkY--;
            }
            else if (e.Y >= 8)
            {

                Game.DebugBreakPoint();

                movedChunk = true;
                chunkMove[1] = 1;
                e.chunkY++;
            }
            else
            {
                chunkMove[1] = 0;
            }

            e.X = (e.X + 8) % 8;
            e.Y = (e.Y + 8) % 8;

            int[] chunkCoords = { index[0] + chunkMove[0], index[1] + chunkMove[1] };

            if (chunkCoords[0] < 1 || chunkCoords[0] >= (worldSize - 1) || chunkCoords[1] < 1 || chunkCoords[1] >= (worldSize - 1))
            {

                Game.DebugBreakPoint();

                int[] des = { (world[index[0], index[1]].coords[0] + chunkMove[0]), (world[index[0], index[1]].coords[1] + chunkMove[1]), (world[index[0], index[1]].coords[2]) };
                SaveEntity sE = new SaveEntity(e, des);
                saveEnt.Add(sE);
                Thread t = new Thread(SaveStoredEntity);
                threads.Add(new ThreadQueue(t));
            }
            else if (movedChunk)
            {
                
                Game.DebugBreakPoint();

                world[chunkCoords[0], chunkCoords[1]].ents.Add(e);
            }
            else
            {
                world[chunkCoords[0], chunkCoords[1]].ents.Add(e);
            }

        }
        
        public void UpdateEntities()
        {
            for (int x = 1; x < (worldSize - 1); x++)
            {
                for (int y = 1; y < (worldSize - 1); y++)
                {
                    Chunk curr = world[x, y];
                    curr.Update();
                }
            }
        }
        
        public void SaveStoredEntity()
        {
            SaveEntity sE = saveEnt[0];
            saveEnt.RemoveAt(0);
            Entity e = sE.entity;
            int[] des = sE.destination;

            int[] worldIndex = GetIndices(new float[] { des[0] * 8, des[1] * 8, des[2] });
            if (worldIndex[0] < 0 || worldIndex[0] >= worldSize || worldIndex[1] < 0 || worldIndex[1] >= worldSize)
            {
                Chunk.SaveToChunk(des, e);
            }
            else
            {
                world[worldIndex[0], worldIndex[1]].ents.Add(e);
            }
        }

        public bool ContainsPoint(float[,] box, float[] coords)
        {
            if (box[0,0] < coords[0] && box[0, 1] > coords[0] && box[1, 0] < coords[1] && box[1, 1] > coords[1])
            {
                return true;
            }
            return false;
        }

    }
}
