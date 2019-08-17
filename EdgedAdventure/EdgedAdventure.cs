using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EdgedAdventure
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class EdgedAdventure : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public int windowScale;
        private uint gameCounter;

        public static uint currSel;
        public static uint page;
        internal static Menu displayMenu;

        MouseState thisM;
        KeyboardState thisKB;
        public List<Keys> accountedKeys;
        public List<Keys> currKeys;
        public List<Keys> newKeys;

        bool worldProgresses;
        Chunk[,] world;
        Chunk[,] loadingWorld;
        List<Chunk> loadedChunks;
        List<Thread> threads;


        internal static Player player;

        public static WorldModes currMode;

        public enum WorldModes
        {
            menu, random, world
        }

        public enum MenuAction
        {
            blank, exit, load,
            new_world, settings, test
        }

        public enum ItemAction
        {
            none, place
        }

        public enum Directions
        {
            up, down, left, right, middle
        }

        public EdgedAdventure()
        {
            windowScale = 4;
            graphics = new GraphicsDeviceManager(this);
            {
                graphics.PreferredBackBufferHeight = 190 * windowScale;
                graphics.PreferredBackBufferWidth = 190 * windowScale;
                graphics.ApplyChanges();
            }
            Content.RootDirectory = "Content";

            currMode = WorldModes.menu;
            displayMenu = new Menu();
            displayMenu.FillEscapeMenu();
            gameCounter = 0;
            worldProgresses = false;

            accountedKeys = new List<Keys>();
            currKeys = new List<Keys>();
            newKeys = new List<Keys>();

            currSel = 0;
            thisM = Mouse.GetState();
            thisKB = Keyboard.GetState();

            world = new Chunk[5, 5];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    world[x, y] = new Chunk(x, y, 0);
                }
            }

            loadedChunks = new List<Chunk>();
            threads = new List<Thread>();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameCounter++;
            /*
            for (int i = 1; i < threads.Count; i++)
            {
                threads[i].Suspend();
            }
            if (!threads[0].IsAlive)
            {
                threads[0].Start();
            }
            */


            thisM = Mouse.GetState();
            thisKB = Keyboard.GetState();
            currKeys = thisKB.GetPressedKeys().OfType<Keys>().ToList();
            newKeys.Clear();

            for (int i = 0; i < currKeys.Count; i++)
            {
                if (!accountedKeys.Contains(currKeys[i]))
                {
                    newKeys.Add(currKeys[i]);
                    accountedKeys.Add(currKeys[i]);
                }
            }
            for (int i = 0; i < accountedKeys.Count; i++)
            {
                if (!currKeys.Contains(accountedKeys[i]))
                {
                    accountedKeys.RemoveAt(i);
                    i = Math.Max(0, i - 1);
                }
            }

            // Exit -> Escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            if (currMode == WorldModes.menu)
            {


                if (newKeys.Contains(Keys.Enter) || newKeys.Contains(Keys.M))
                {
                    DoSelectedMenuItem();
                }
                else if (newKeys.Contains(Keys.S))
                {
                    currSel = (uint)((currSel + 1) % displayMenu.currMenu.Count);
                }
                else if (newKeys.Contains(Keys.W))
                {
                    currSel = (uint)((currSel + (displayMenu.currMenu.Count - 1)) % (displayMenu.currMenu.Count));
                }
            }
            else if (currMode == WorldModes.world)
            {
                if (currKeys.Contains(Keys.W))
                {
                    player.Update(Player.Action.move_up);
                }
                if (currKeys.Contains(Keys.A))
                {
                    player.Update(Player.Action.move_left);
                }
                if (currKeys.Contains(Keys.S))
                {
                    player.Update(Player.Action.move_down);
                }
                if (currKeys.Contains(Keys.D))
                {
                    player.Update(Player.Action.move_right);
                }
                if (newKeys.Contains(Keys.M))
                {
                    player.Update(Player.Action.attack);
                }
            }

            if (worldProgresses)
            {
                ProgressWorld();
            }

            Draw(gameTime);

            if (currMode == WorldModes.world && (world[2, 2].coords[0] != player.chunkX || world[2, 2].coords[1] != player.chunkY))
            {
                loadingWorld = world;
                PrelimMoveWorld();
                Thread t = new Thread(MoveChunks);
                t.Start();
                //threads.Add(t);
                //MoveChunks();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            // TODO: Add your drawing code here

            if (currMode == WorldModes.random)
            {
                DrawRandom();
            }
            else if (currMode == WorldModes.menu)
            {
                DrawMenu();
            }
            else if (currMode == WorldModes.world)
            {
                DrawWorld();
            }

            {
                Point mousePos = thisM.Position;
                mousePos.X -= 1;
                mousePos.Y -= 1;
                Texture2D tex = this.Content.Load<Texture2D>("white_pixel");
                Rectangle rec = new Rectangle(mousePos, new Point(3, 3));
                spriteBatch.Draw(tex, rec, Color.Blue);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRandom()
        {
            Random r = new Random();
            for (int x = 0; x < graphics.PreferredBackBufferWidth / windowScale; x++)
            {
                for (int y = 0; y < graphics.PreferredBackBufferHeight / windowScale; y++)
                {
                    Texture2D tex = this.Content.Load<Texture2D>("white_pixel");
                    Color c = new Color((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    spriteBatch.Draw(tex, new Rectangle((x * windowScale), (y * windowScale), windowScale, windowScale), c);
                }
            }
        }

        private void DrawMenu()
        {
            // TODO: Print beginning of box

            float startY = (graphics.PreferredBackBufferHeight / 2) - (displayMenu.currMenu.Count) * (7.5F * windowScale) - (15 * windowScale);
            Texture2D tex;
            for (int i = 0; i < Math.Min(displayMenu.currMenu.Count, 10); i++)
            {

                string currMenuItem = displayMenu.currMenu[i].name;
                float startX = (graphics.PreferredBackBufferWidth / 2) - (currMenuItem.Length * (5 * windowScale));
                if (i == currSel)
                {
                    tex = this.Content.Load<Texture2D>("text/right_arrow");
                    Rectangle rec = new Rectangle(new Point((int)(startX - (20 * windowScale)), (int)(startY + (15 * windowScale) + (i * (15 * windowScale)))), new Point(10 * windowScale, 10 * windowScale));
                    spriteBatch.Draw(tex, rec, Color.White);
                }

                for (int j = 0; j < currMenuItem.Length; j++)
                {
                    tex = this.Content.Load<Texture2D>("text/" + TranslateSafeKey(currMenuItem[j]));
                    Rectangle rec = new Rectangle(new Point((int)(startX + (j * (10 * windowScale))), (int)(startY + (15 * windowScale) + (i * (15 * windowScale)))), new Point(10 * windowScale, 10 * windowScale));
                    spriteBatch.Draw(tex, rec, Color.White);
                }

                // TODO: Sides of box
            }

            // TODO: Print end of box
        }

        private void DrawWorld()
        {
            // windowScale*100 wide/tall
            // 21x21 blocks wide/tall

            int windowBlockX = graphics.PreferredBackBufferWidth / (10 * windowScale);
            int windowBlockY = graphics.PreferredBackBufferHeight / (10 * windowScale);

            if (newKeys.Contains(Keys.Y))
            {
                { }
                player = player;
                world = world;
            }

            for (float x = -3; x < (windowBlockX + 3); x++)
            {
                { }
                for (float y = -3; y < (windowBlockY + 3); y++)
                {
                    int xWorldIndex = (int)(player.X + x - ((int)(windowBlockX / 2)) + 64) % 8;
                    int yWorldIndex = (int)(player.Y + y - ((int)(windowBlockY / 2)) + 64) % 8;

                    int xChunkIndex = (int)(2 + ((player.X + x - (windowBlockX / 2)) / 8));
                    int yChunkIndex = (int)(2 + ((player.Y + y - (windowBlockY / 2)) / 8));
                    {
                        string tile = world[xChunkIndex, yChunkIndex].spaces[xWorldIndex, yWorldIndex].GetTile();

                        Texture2D tex = this.Content.Load<Texture2D>(tile);

                        // Go to pixels, and ensure it has appropriate off-set to to half-blocks
                        int xPos = (int)(((x - (player.X % 1) + 0.5F)) * (10 * windowScale));
                        int yPos = (int)(((y - (player.Y % 1) + 0.5F)) * (10 * windowScale));
                        Rectangle rec = new Rectangle(new Point((int)(xPos), (int)(yPos)), new Point(10 * windowScale, 10 * windowScale));
                        spriteBatch.Draw(tex, rec, Color.White);
                    }
                }
            }
            {
                int xCenterRaw = graphics.PreferredBackBufferWidth / 2 - (5 * windowScale);
                int yCenterRaw = graphics.PreferredBackBufferHeight / 2 - (5 * windowScale);
                Texture2D tex = this.Content.Load<Texture2D>(player.GetSprite());

                yCenterRaw -= (3 * windowScale);

                Rectangle rec = new Rectangle(new Point((int)xCenterRaw, (int)yCenterRaw), new Point(10 * windowScale, 10 * windowScale));
                spriteBatch.Draw(tex, rec, Color.White);
            }
            {
                string tempCoords = " " + player.chunkX + ", " + player.chunkY + ", " + player.layer;
                for(int i = 0; i < tempCoords.Length; i++)
                {
                    if (tempCoords[i] == ' ')
                    {
                        continue;
                    }
                    Texture2D tex = this.Content.Load<Texture2D>("text/" + TranslateSafeKey(tempCoords[i]));
                    Rectangle rec = new Rectangle(new Point(i * 13, 5), new Point(10, 10));
                    spriteBatch.Draw(tex, rec, Color.White);
                }
            }

        }

        // Enter:13
        // Escape:27
        // Back:8
        // Space32
        // CapsLock:17

        // State of Files/Keystrokes
        private char TranslateKey(string k, bool shift)
        {
            // Not shifted
            if (k == "Oemtilde" && !shift)
            {
                return '`';
            }
            else if (k == "OemMinus" && !shift)
            {
                return '-';
            }
            else if (k == "Oemplus" && !shift)
            {
                return '=';
            }
            else if (k == "OemOpenBrackets" && !shift)
            {
                return '[';
            }
            else if (k == "Oem6" && !shift)
            {
                return ']';
            }
            else if (k == "Oem5" && !shift)
            {
                return '\\';
            }
            else if (k == "Oem1" && !shift)
            {
                return ';';
            }
            else if (k == "Oem7" && !shift)
            {
                return '\'';
            }
            else if (k == "Oemcomma" && !shift)
            {
                return ',';
            }
            else if (k == "OemPeriod" && !shift)
            {
                return '.';
            }
            else if (k == "OemQuestion" && !shift)
            {
                return '/';
            }
            if (!shift && k.Length == 1 && 97 <= k[0] && k[0] <= 122)
            {
                return (char)k[0];
            }
            // Shifted
            else if (k == "Oemtilde" && shift)
            {
                return '~';
            }
            else if (k == "OemMinus" && shift)
            {
                return '_';
            }
            else if (k == "Oemplus" && shift)
            {
                return '+';
            }
            else if (k == "OemOpenBrackets" && shift)
            {
                return '{';
            }
            else if (k == "Oem6" && shift)
            {
                return '}';
            }
            else if (k == "Oem5" && shift)
            {
                return '|';
            }
            else if (k == "Oem1" && shift)
            {
                return ':';
            }
            else if (k == "Oem7" && shift)
            {
                return '"';
            }
            else if (k == "Oemcomma" && shift)
            {
                return '<';
            }
            else if (k == "OemPeriod" && shift)
            {
                return '>';
            }
            else if (k == "OemQuestion" && shift)
            {
                return '?';
            }
            else if (k == "Enter")
            {
                return (char)13;
            }
            else if (k == "Escape")
            {
                return (char)27;
            }
            else if (k == "Back")
            {
                return (char)8;
            }
            else if (k == "Space")
            {
                return (char)32;
            }
            else if (k == "CapsLock")
            {
                return (char)17;
            }
            else if (k == "," && shift)
            {
                return '<';
            }
            else if (k == "." && shift)
            {
                return '>';
            }
            if (shift && k.Length == 1 && 97 <= k[0] && k[0] <= 122)
            {
                return (char)(k[0] - 32);
            }
            return '\0';
        }

        private string TranslateSafeKey(char c)
        {
            if (c == '/')
            {
                return "fslash";
            }
            else if (c == '\\')
            {
                return "bslash";
            }
            else if (c == '?')
            {
                return "question_mark";
            }
            else if (c == ':')
            {
                return "colon";
            }
            else if (c == ' ')
            {
                return "space";
            }
            else
            {
                return "" + c;
            }
        }

        private void ClearFile(string filePath)
        {
            string empty = "";
            try
            {
                if (!filePath.EndsWith(".txt"))
                {
                    filePath += ".txt";
                }
                StreamWriter sw = new StreamWriter(filePath);
                sw.WriteLine(empty);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void WriteToFile(string save, string filePath)
        {
            try
            {
                if (!filePath.EndsWith(".txt"))
                {
                    filePath += ".txt";
                }
                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(save);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private string LoadFile(string filePath)
        {
            string coll = "";
            try
            {
                if (!filePath.EndsWith(".txt"))
                {
                    filePath += ".txt";
                }
                StreamReader sr = new StreamReader(filePath);
                coll = sr.ReadToEnd();
                sr.Close();

            }
            catch (Exception e)
            {
                int that = e.GetHashCode();
                return ("Error with hash " + that.ToString());
            }
            return coll;

        }

        // World Progression

        private void ProgressWorld()
        {

        }

        private void DoSelectedMenuItem()
        {
            if (displayMenu.currMenu[(int)currSel].action == MenuAction.exit)
            {
                Exit();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.new_world)
            {
                currMode = WorldModes.world;
                int[] size = { 3, 3, 1 };
                CreateWorld(size);
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.test)
            {
                byte[] blank = new byte[300];
                for (int i = 0; i < blank.Length; i++)
                {
                    blank[i] = 0b00000000;
                }
                FileStream file = File.Create("test_file.txt");
                file.Write(blank, 0, 300);
                file.Close();
            }
        }

        private void CreateWorld(int[] size)
        {
            // Wipe and Create
            if (Directory.Exists("world"))
            {
                Directory.Delete("world", true);
            }
            Directory.CreateDirectory("world");

            // Create chunks for specified size
            Directory.CreateDirectory(@"world\chunks");
            StreamWriter writer = null;
            for (int x = -5; x < 5; x++)
            {
                for (int y = -5; y < 5; y++)
                {
                    for (int l = 0; l < 1; l++)
                    {
                        Directory.CreateDirectory(@"world\chunks\" + x + "," + y + "," + l);
                        Directory.CreateDirectory(@"world\chunks\" + x + "," + y + "," + l + @"\ents");

                        writer = File.CreateText(@"world\chunks\" + x + "," + y + "," + l + @"\blocks.blocks");
                        string currblocks = "";
                        for (int i = 0; i < 64; i++)
                        {
                            currblocks += "0,";
                        }
                        writer.WriteLine(currblocks);
                        writer.Close();
                    }
                }
            }

            // Player
            Directory.CreateDirectory(@"world\player");
            // Health
            writer = File.CreateText(@"world\player\health.int");
            writer.WriteLine("20");
            writer.Close();

            // Inventory
            File.CreateText(@"world\player\inv.items").Close();

            // Position
            Directory.CreateDirectory(@"world\player\coords");
            // Chunk X
            writer = File.CreateText(@"world\player\coords\cX.int");
            writer.WriteLine("1");
            writer.Close();
            // Chunk Y
            writer = File.CreateText(@"world\player\coords\cY.int");
            writer.WriteLine("1");
            writer.Close();
            // Chunk Layer
            writer = File.CreateText(@"world\player\coords\cL.int");
            writer.WriteLine("0");
            writer.Close();
            // X
            writer = File.CreateText(@"world\player\coords\X.int");
            writer.WriteLine("4.3");
            writer.Close();
            // Y
            writer = File.CreateText(@"world\player\coords\Y.int");
            writer.WriteLine("4.5");
            writer.Close();

            player = new Player(4.3F, 4.5F, 1, 1, 0);

            int countX = 0;
            int countY = countX;

            for (int x = (player.chunkX - 2); x < (player.chunkX + 3); x++)
            {
                for (int y = (player.chunkY - 2); y < (player.chunkY + 3); y++)
                {

                    world[countX, countY] = Chunk.GetChunk(x, y, 0);
                    countY++;
                }
                countY = 0;
                countX++;
            }
            List<Block> bs = new List<Block>();
            {
                bs.Add(Block.GetBlock(1));
            }
            for (int i = 0; i < 8; i++)
            {
                world[2, 2].spaces[0, i] = new Space(bs);
                world[2, 2].spaces[7, i] = new Space(bs);
                world[2, 2].spaces[i, 0] = new Space(bs);
                world[2, 2].spaces[i, 7] = new Space(bs);
            }
            bs = new List<Block>();
            {
                bs.Add(Block.GetBlock(2));
            }
            for (int i = 0; i < 8; i++)
            {
                world[2, 1].spaces[0, i] = new Space(bs);
                world[2, 1].spaces[7, i] = new Space(bs);
                world[2, 1].spaces[i, 0] = new Space(bs);
                world[2, 1].spaces[i, 7] = new Space(bs);
            }
            world[2, 2].spaces[6, 6] = new Space(bs);

        }

        private void MoveChunks()
        {

            if (player.chunkX > loadingWorld[2, 2].coords[0])
            {
                SaveChunks(Directions.right);
                for (uint x = 0; x < 4; x++)
                {
                    for (uint y = 0; y < 5; y++)
                    {
                        loadingWorld[x, y] = loadingWorld[x + 1, y];
                    }
                }
                LoadChunks(Directions.right);
            }
            if (player.chunkX < loadingWorld[2, 2].coords[0])
            {
                SaveChunks(Directions.left);
                for (int x = 3; 0 <= x; x--)
                {
                    for (uint y = 0; y < 5; y++)
                    {
                        loadingWorld[x + 1, y] = loadingWorld[x, y];
                    }
                }
                LoadChunks(Directions.left);
            }
            if (player.chunkY > loadingWorld[2, 2].coords[1])
            {
                SaveChunks(Directions.down);
                for (uint x = 0; x < 5; x++)
                {
                    for (uint y = 0; y < 4; y++)
                    {
                        loadingWorld[x, y] = loadingWorld[x, y + 1];
                    }
                }
                LoadChunks(Directions.down);
            }
            if (player.chunkY < loadingWorld[2, 2].coords[1])
            {
                SaveChunks(Directions.up);
                for (uint x = 0; x < 5; x++)
                {
                    for (int y = 3; 0 <= y; y--)
                    {
                        loadingWorld[x, y + 1] = loadingWorld[x, y];
                    }
                }
                LoadChunks(Directions.up);
            }
            world = loadingWorld;
        }

        private void LoadChunks(Directions dir)
        {

            if (dir == Directions.up)
            {
                for (uint i = 0; i < 5; i++)
                {
                    loadingWorld[i, 0] = Chunk.GetChunk(loadingWorld[i, 0].coords[0], loadingWorld[i, 0].coords[1] - 1, loadingWorld[i, 0].coords[2]);
                }
            }
            else if (dir == Directions.down)
            {
                for (uint i = 0; i < 5; i++)
                {
                    loadingWorld[i, 4] = Chunk.GetChunk(loadingWorld[i, 4].coords[0], loadingWorld[i, 4].coords[1] + 1, loadingWorld[i, 4].coords[2]);
                }
            }
            else if (dir == Directions.left)
            {
                for (uint i = 0; i < 5; i++)
                {
                    loadingWorld[0, i] = Chunk.GetChunk(loadingWorld[0, i].coords[0] - 1, loadingWorld[0, i].coords[1], loadingWorld[0, i].coords[2]);
                }
            }
            else if (dir == Directions.right)
            {
                for (uint i = 0; i < 5; i++)
                {
                    loadingWorld[4, i] = Chunk.GetChunk(loadingWorld[4, i].coords[0] + 1, loadingWorld[4, i].coords[1], loadingWorld[4, i].coords[2]);
                }

            }
        }

        private void SaveChunks(Directions dir)
        {
            if (dir == Directions.up)
            {
                for (uint i = 0; i < 5; i++)
                {
                    Chunk.SaveChunk(loadingWorld[i, 4]);
                }
            }
            else if (dir == Directions.down)
            {
                for (uint i = 0; i < 5; i++)
                {
                    Chunk.SaveChunk(loadingWorld[i, 0]);
                }
            }
            else if (dir == Directions.left)
            {
                for (uint i = 0; i < 5; i++)
                {
                    Chunk.SaveChunk(loadingWorld[4, i]);
                }
            }
            else if (dir == Directions.right)
            {
                for (uint i = 0; i < 5; i++)
                {
                    Chunk.SaveChunk(loadingWorld[0, i]);

                }
            }
        }

        private void PrelimMoveWorld()
        {
            if (player.chunkY < world[2, 2].coords[1])
            {
                for (uint x = 0; x < 5; x++)
                {
                    for (int y = 3; 0 <= y; y--)
                    {
                        world[x, y + 1] = world[x, y];
                    }
                }
            }
            else if (player.chunkY > world[2, 2].coords[1])
            {
                for (uint x = 0; x < 5; x++)
                {
                    for (uint y = 0; y < 4; y++)
                    {
                        world[x, y] = world[x, y + 1];
                    }
                }
            }
            else if (player.chunkX < world[2, 2].coords[0])
            {
                for (int x = 3; 0 <= x; x--)
                {
                    for (uint y = 0; y < 5; y++)
                    {
                        world[x + 1, y] = world[x, y];
                    }
                }
            }
            else if (player.chunkX > world[2, 2].coords[0])
            {
                for (uint x = 0; x < 4; x++)
                {
                    for (uint y = 0; y < 5; y++)
                    {
                        world[x, y] = world[x + 1, y];
                    }
                }
            }
        }
    }
}
