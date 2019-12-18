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
        private uint gameCounter;

        public int windowScale;
        public int windowSize;

        public static uint currSel;
        public static uint page;
        internal static Menu displayMenu;

        MouseState thisM;
        KeyboardState thisKB;
        public List<Keys> accountedKeys;
        public List<Keys> currKeys;
        public List<Keys> newKeys;

        bool worldProgresses;
        public readonly int worldSize;
        LoadedChunkManager LCM;

        public Player player;

        public static WorldModes currMode;

        public enum WorldModes
        {
            menu, random, world
        }

        public enum MenuAction
        {
            blank, exit, load,
            new_world, settings, test,
            window_size_up, window_size_down, save,
            world, o_options, e_options,
            open_menu, escape_menu
        }

        public enum ItemAction
        {
            none, place, attack, mine
        }

        public enum Directions
        {
            up, down, left, right, middle
        }

        public EdgedAdventure()
        {
            windowScale = 4;
            windowSize = 11;
            graphics = new GraphicsDeviceManager(this);
            {
                graphics.PreferredBackBufferHeight = (windowSize * 10) * (windowScale + 1);
                graphics.PreferredBackBufferWidth = (windowSize * 10) * (windowScale + 1);
                graphics.ApplyChanges();
            }
            Content.RootDirectory = "Content";

            currMode = WorldModes.menu;
            displayMenu = new Menu();
            displayMenu.FillOpenMenu();
            gameCounter = 0;
            worldProgresses = false;

            accountedKeys = new List<Keys>();
            currKeys = new List<Keys>();
            newKeys = new List<Keys>();

            currSel = 0;
            thisM = Mouse.GetState();
            thisKB = Keyboard.GetState();

            worldSize = 7;
            Chunk[,] world = new Chunk[worldSize, worldSize];
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    world[x, y] = new Chunk(x, y, 0);
                }
            }
            LCM = new LoadedChunkManager(this, world);

            player = null;
            
            Entity.Game = this;
            Entity.LCM = this.LCM;

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

            // Exit -> Q
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                Exit();
            }

            gameCounter++;

            // Mouse/Key Manipulation
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

            // Player Input
            if (currMode == WorldModes.menu)
            {
                if (newKeys.Contains(Keys.Enter) || newKeys.Contains(Keys.M))
                {
                    DoSelectedMenuItem();
                    currSel = (uint)(currSel % displayMenu.currMenu.Count);
                }
                else if (newKeys.Contains(Keys.S))
                {
                    currSel = (uint)((currSel + 1) % displayMenu.currMenu.Count);
                }
                else if (newKeys.Contains(Keys.W))
                {
                    currSel = (uint)((currSel + (displayMenu.currMenu.Count - 1)) % (displayMenu.currMenu.Count));
                }
                else if (newKeys.Contains(Keys.Escape))
                {
                    for (uint i = 0; i < displayMenu.currMenu.Count; i++)
                    {
                        if (displayMenu.currMenu[(int) i].name == "Back")
                        {
                            currSel = i;
                            DoSelectedMenuItem();
                            break;
                        }
                    }
                }
            }
            else if (currMode == WorldModes.world)
            {
                worldProgresses = true;

                if (currKeys.Contains(Keys.Y))
                {
                    DebugBreakPoint();
                }

                if (currKeys.Contains(Keys.A))
                {
                    player.Update(Player.Action.move_left);
                }
                if (currKeys.Contains(Keys.D))
                {
                    player.Update(Player.Action.move_right);
                }
                if (currKeys.Contains(Keys.W))
                {
                    player.Update(Player.Action.move_up);
                }
                if (currKeys.Contains(Keys.S))
                {
                    player.Update(Player.Action.move_down);
                }
                if (newKeys.Contains(Keys.M))
                {
                    player.Update(Player.Action.attack);
                }
                if (currKeys.Contains(Keys.X))
                {
                    //DebugBreakPoint();
                    player.Update(Player.Action.test);
                }
                if (newKeys.Contains(Keys.Escape))
                {
                    currMode = WorldModes.menu;
                    displayMenu.FillEscapeMenu();
                    worldProgresses = false;
                }
                player.UpdateCounters();
            }

            // World Progression Tracker
            if (worldProgresses)
            {
                LCM.UpdateEntities();
            }

            Draw(gameTime);

            if (currMode == WorldModes.world)
            {
                LCM.Update();
            }

            base.Update(gameTime);
        }


        // Draw Methods
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

            for (float x = -2; x < (windowBlockX + 2); x++)
            {
                { }
                for (float y = -2; y < (windowBlockY + 2); y++)
                {
                    int xWorldIndex = (int)(player.X + x - ((int)(windowBlockX / 2)) + 64) % 8;
                    int yWorldIndex = (int)(player.Y + y - ((int)(windowBlockY / 2)) + 64) % 8;

                    int xChunkIndex = (int)((worldSize/2) + ((player.X + x - (windowBlockX / 2)) / 8));
                    int yChunkIndex = (int)((worldSize/2) + ((player.Y + y - (windowBlockY / 2)) / 8));

                    {
                        string tile = LCM.world[xChunkIndex, yChunkIndex].spaces[xWorldIndex, yWorldIndex].GetTile();

                        Texture2D tex = this.Content.Load<Texture2D>(tile);

                        // Go to pixels, and ensure it has appropriate off-set to the half-blocks
                        int xPos = (int)(((x - (player.X % 1))) * (10 * windowScale));
                        int yPos = (int)(((y - (player.Y % 1))) * (10 * windowScale));

                        Rectangle rec = new Rectangle(new Point((int)(xPos + (10 * windowScale)), (int)(yPos + (10 * windowScale))), new Point(10 * windowScale, 10 * windowScale));
                        spriteBatch.Draw(tex, rec, Color.White);
                    }
                }
            }
            {
                for (int x = 1; x < LCM.worldSize - 1; x++)
                {
                    for (int y = 1; y < LCM.worldSize - 1; y++)
                    {
                        List<Entity> ents = LCM.world[x, y].ents;
                        for (int i = 0; i < ents.Count; i++)
                        {
                            int xComp = (ents[i].chunkX - player.chunkX) * 8;
                            int yComp = (ents[i].chunkY - player.chunkY) * 8;

                            int xPos = (int)((xComp + (ents[i].X - player.X)) * (10 * windowScale));
                            int yPos = (int)((yComp + (ents[i].Y - player.Y)) * (10 * windowScale));
                            
                            int xCenterRaw = (int)((((windowBlockX / 2))) * (10 * windowScale)) + (5 * windowScale);
                            int yCenterRaw = (int)((((windowBlockY / 2))) * (10 * windowScale)) + (5 * windowScale);
                            yCenterRaw -= (int)((1 * windowScale));
                            xCenterRaw -= (int)((1 * windowScale));

                            xCenterRaw += xPos;
                            yCenterRaw += yPos;

                            // Height
                            yCenterRaw -= (int)((10 * windowScale) * ents[i].Z);
                            
                            Texture2D tex = this.Content.Load<Texture2D>(ents[i].GetSprite());
                            Rectangle rec = new Rectangle(new Point((int)xCenterRaw, (int)yCenterRaw), new Point(10 * windowScale, 10 * windowScale));
                            spriteBatch.Draw(tex, rec, Color.White);
                        }
                    }
                }
            }
            {

                int xCenterRaw = (int)((((windowBlockX / 2))) * (10 * windowScale)) + (5 * windowScale);
                int yCenterRaw = (int)((((windowBlockY / 2))) * (10 * windowScale)) + (5 * windowScale);

                Texture2D tex = this.Content.Load<Texture2D>(player.GetSprite());

                yCenterRaw -= (int)((1 * windowScale));
                xCenterRaw -= (int)((1 * windowScale));

                Rectangle rec = new Rectangle(new Point((int)xCenterRaw, (int)yCenterRaw), new Point(10 * windowScale, 10 * windowScale));
                spriteBatch.Draw(tex, rec, Color.White);
            }
            {
                string energy = "(" + player.chunkX + ", " + player.chunkY + ")";

                for (int i = 0; i < energy.Length; i++)
                {
                    if (energy[i] == ' ')
                    {
                        continue;
                    }
                    Texture2D tex = this.Content.Load<Texture2D>(@"text\" + TranslateSafeKey(energy[i]) + "");

                    Rectangle rec = new Rectangle(new Point(i * (10 * windowScale) + 5, 5), new Point(10 * windowScale, 10 * windowScale));
                    spriteBatch.Draw(tex, rec, Color.White);
                }
            }

        }


        // Text Manipulation
        // Enter:13
        // Escape:27
        // Back:8
        // Space32
        // CapsLock:17
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



        private void DoSelectedMenuItem()
        {
            if (displayMenu.currMenu[(int)currSel].action == MenuAction.exit)
            {
                Exit();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.test)
            {
                /*
                byte[] blank = new byte[300];
                for (int i = 0; i < blank.Length; i++)
                {
                    blank[i] = 0b00000000;
                }
                FileStream file = File.Create("test_file.txt");
                file.Write(blank, 0, 300);
                file.Close();
                */
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.world)
            {
                worldProgresses = true;
                currMode = WorldModes.world;
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.window_size_up)
            {
                windowScale = Math.Min((windowScale + 1), 6);
                graphics.PreferredBackBufferHeight = (windowSize * 10) * (windowScale + 1);
                graphics.PreferredBackBufferWidth = (windowSize * 10) * (windowScale + 1);
                graphics.ApplyChanges();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.window_size_down)
            {
                windowScale = Math.Max((windowScale - 1), 1);
                graphics.PreferredBackBufferHeight = (windowSize * 10) * (windowScale + 1);
                graphics.PreferredBackBufferWidth = (windowSize * 10) * (windowScale + 1);
                graphics.ApplyChanges();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.save)
            {
                LCM.SaveWorld();
                player.SavePlayer();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.new_world)
            {
                currMode = WorldModes.world;
                CreateWorld();
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.open_menu)
            {
                displayMenu.FillOpenMenu();
                currMode = WorldModes.menu;
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.escape_menu)
            {
                displayMenu.FillEscapeMenu();
                currMode = WorldModes.menu;
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.o_options)
            {
                displayMenu.FillOOptionsMenu();
                currMode = WorldModes.menu;
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.e_options)
            {
                displayMenu.FillEOptionsMenu();
                currMode = WorldModes.menu;
            }
            else if (displayMenu.currMenu[(int)currSel].action == MenuAction.load)
            {
                LoadWorld();
            }
        }


        // File State Dynamics

        private void CreateWorld()
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

                        writer = File.CreateText(@"world\chunks\" + x + "," + y + "," + l + @"\blocks");

                        string currblocks = "";
                        currblocks = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,";
                        if (x == 1 && y == 1)
                        {
                            currblocks = "";
                            Random r = new Random();
                            for (int i = 0; i < 64; i++)
                            {
                                int sel = (int)((r.NextDouble() * (1.5) + 0.499));
                                if (sel == 0)
                                {
                                    currblocks += "1&";
                                }
                                currblocks += sel;
                                currblocks += ",";
                            }
                        }
                        writer.WriteLine(currblocks);
                        writer.Close();
                    }
                }
            }

            // Player
            Directory.CreateDirectory(@"world\player");
            // Health
            writer = File.CreateText(@"world\player\health");
            writer.WriteLine("20");
            writer.Close();

            // Inventory
            Directory.CreateDirectory(@"world\player\inv");

            // Position
            Directory.CreateDirectory(@"world\player\coords");
            // Chunk X
            writer = File.CreateText(@"world\player\coords\cX");
            writer.WriteLine("1");
            writer.Close();
            // Chunk Y
            writer = File.CreateText(@"world\player\coords\cY");
            writer.WriteLine("1");
            writer.Close();
            // Chunk Layer
            writer = File.CreateText(@"world\player\coords\l");
            writer.WriteLine("0");
            writer.Close();
            // X
            writer = File.CreateText(@"world\player\coords\X");
            writer.WriteLine("4.3");
            writer.Close();
            // Y
            writer = File.CreateText(@"world\player\coords\Y");
            writer.WriteLine("4.5");
            writer.Close();

            player = new Player(4.3F, 4.5F, 1, 1, 0);

            LCM.CreateWorld();

        }

        private void LoadWorld()
        {
            player = Player.GetPlayer();
            LCM.LoadWorld(new int[] { player.chunkX - (worldSize / 2), player.chunkY - (worldSize / 2), player.layer }, 7);
            currMode = WorldModes.world;
        }

        public void DebugBreakPoint() { }
    }
}
