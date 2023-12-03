//Author: Victoria Mak
//File Name: Game1.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Play the game, Minecraft Shooter, where you try to shoot as many mobs as possible to get the highest score possible

using System;
using Helper;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace OOP_PASS
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Store the block side length and the number of blocks that make up the sceen dimension
        public const int BLOCK_SIDE = 64;
        private const int NUM_COLS = 12;
        private const int NUM_ROWS = 10;

        //Store the game states
        private const int MENU = 0;
        private const int PRE_GAME = 1;
        private const int PLAY = 2;
        private const int STATS = 3;
        private const int END_GAME = 4;
        private const int STORE = 5;

        //Store the block types
        private const int COBBLESTONE = 0;
        private const int DIRT = 1;
        private const int GRASS_1 = 2;
        private const int GRASS_2 = 3;

        //Store the window dimensions
        public const int SCREEN_WIDTH = NUM_COLS * BLOCK_SIDE;
        public const int SCREEN_HEIGHT = NUM_ROWS * BLOCK_SIDE;

        //Store the UI spacings
        private const int MENU_BTN_SPACING = 130;
        private const int SHOP_SPACING = 10;
        private const int INSTR_BOX_HEIGHT = 240;
        private const int INSTR_SPACING = 10;
        private const int STAT_LEFT_BUFFER = 20;
        private const int SHADOW_DISPLACEMENT = 3;

        //Store the costs of the buffs
        private readonly int[] BUFF_COSTS = { 100, 200, 300, 500 };
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        //Store the files
        private static StreamWriter outFile;
        private static StreamReader inFile;

        //Store the current and previous mouse and keyboard states
        private KeyboardState kb;
        private KeyboardState prevKb;
        private MouseState mouse;
        private MouseState prevMouse;

        //Store the fonts
        private SpriteFont titleFont;
        private SpriteFont instrFont;
        private SpriteFont boldFont;

        //Store the music
        private Song gameSong;
        private Song menuSong;
        private Song resultsSong;
        private Song shopSong;

        //Store the sound effects
        private SoundEffect arrowImpactSnd;
        private SoundEffect bowShootSnd;
        private SoundEffect buttonClickSnd;
        private SoundEffect endermanScreamSnd;
        private SoundEffect endermanTeleportSnd;
        private SoundEffect explodeSnd;
        private SoundEffect purchaseSnd;
        private SoundEffect shieldHitSnd;

        //Store the background images
        private Texture2D menuBgImg;
        private Texture2D statsBgImg;
        private Texture2D shopBgImg;

        //Store the single pixel image
        private Texture2D singlePixelImg;

        //Store the title and button images
        private Texture2D menuTitleImg;
        private Texture2D statsTitleImg;
        private Texture2D shopTitleImg;
        private Texture2D btnImg;

        //Store the player image
        private Texture2D playerImg;

        //Store the mob and shield images
        private Texture2D villagerImg;
        private Texture2D pillagerImg;
        private Texture2D creeperImg;
        private Texture2D skeletonImg;
        private Texture2D endermanImg;
        private Texture2D shieldImg;

        //Store the dying images
        private Texture2D dyingImg;
        private Texture2D explosionImg;

        //Store the arrow images
        private Texture2D playerArrowImg;
        private Texture2D mobArrowImg;

        //Store the block images
        private Texture2D[] blockImgs = new Texture2D[4];

        //Store the buff shop image and buff icons
        private Texture2D speedBuffShopImg;
        private Texture2D damageBuffShopImg;
        private Texture2D fireRateBuffShopImg;
        private Texture2D pointsBuffShopImg;
        private Texture2D speedBuffIcon;
        private Texture2D damageBuffIcon;
        private Texture2D fireRateBuffIcon;
        private Texture2D pointsBuffIcon;

        //Store the shop vendor image
        private Texture2D traderImg;

        //Store the background rectangle
        private Rectangle bgRec;

        //Store the title rectangles
        private Rectangle menuTitleRec;
        private Rectangle statsTitleRec;
        private Rectangle shopTitleRec;

        //Store the button rectangles
        private Rectangle playBtnRec;
        private Rectangle statsBtnRec;
        private Rectangle exitBtnRec;
        private Rectangle backBtnRec;
        private Rectangle shopPlayBtnRec;

        //Store the instructional box rectangle
        private Rectangle instrBoxRec;

        //Store the rectangles for the buffs
        private Rectangle[] shopBuffRecs = new Rectangle[4];

        //Store the rectangle for the trader
        private Rectangle traderRec;

        //Store the statistics
        private int hScore;
        private int gamesPlayed;
        private int shotsFired;
        private int shotsHit;
        private int topHitPercent;
        private int[] totKillsByType = new int[5];
        private int totKills;
        private int allTimeHitPercent;
        private double avgShotsPerGame;
        private double avgKillsPerGame;

        //Store the current level statistics
        private int curTotScore;
        private int[] lvlScores = new int[Gameplay.NUM_LEVELS];
        private int gameShotsFired;
        private int gameHits;
        private int lvlHitPercent;
        private int gameHitPercent;
        
        //Store the labels for the buttons
        private string playBtnLabel = "Play";
        private string statsBtnLabel = "Stats";
        private string exitBtnLabel = "Exit";
        private string backBtnLabel = "Back";

        //Store the instructions 
        private string[] instructions = new string[5] { "INSTRUCTIONS", "Use left and right arrows to move and press space to shoot", "Target: Kill the mobs before they leave", "Tips: Watch out for some dangerous mobs!", "Press SPACE to start!" };

        //Store the new high score message
        private string newHSMsg = "New High Score!!!";
        
        //Store the stats labels
        private string hScoreMsg = "High Score: ";
        private string gamesPlayedMsg = "Games Played: ";
        private string shotsFiredMsg = "Shots Fired: ";
        private string shotsHitMsg = "Shots Hit: ";
        private string topHitPercMsg = "Top Hit Percentage in a game: ";
        private string[] totMobsKilledMsgs = new string[5] { "Villagers killed: ", "Creepers killed: ", "Skeletons killed: ", "Pillagers killed: ", "Endermans killed: " };
        private string totKillsMsg = "Total Mobs Killed: ";
        private string allTimeHitPercMsg = "All Time Hit Percentage: ";
        private string avgShotsPerGameMsg = "Average Shots per Game: ";
        private string avgKillsPerGameMsg = "Average Kills Per Game: ";

        //Store the level stat messages
        private string curLevelMsg = "Level ";
        private string curTotScoreMsg = "Total score: ";
        private string[] lvlScoresMsgs = new string[Gameplay.NUM_LEVELS] { "Level 1 Score: Uncompleted", "Level 2 Score: Uncompleted", "Level 3 Score: Uncompleted", "Level 4 Score: Uncompleted", "Level 5 Score: Uncompleted" };
        private string lvlKillsMsg = "Level Kills: ";
        private string lvlShotsFiredMsg = "Level Shots Fired: ";
        private string lvlHitsMsg = "Level Hits: ";
        private string lvlHitPercMsg = "Level Hit Percentage: ";
        private string continueMsg = "Press Space to Continue";
        
        //Store the button label locations
        private Vector2 playLabelLoc;
        private Vector2 statsLabelLoc;
        private Vector2 exitLabelLoc;
        private Vector2 backLabelLoc;
        private Vector2 shopPlayLabelLoc;

        //Store the instructions location
        private Vector2[] instructionsLocs = new Vector2[5];

        //Store the stats label locations
        private Vector2 hScoreLoc;
        private Vector2 gamesPlayedLoc;
        private Vector2 shotsFiredLoc;
        private Vector2 shotsHitLoc;
        private Vector2 topHitPercentLoc;
        private Vector2[] totMobsKilledLocs = new Vector2[5];
        private Vector2 totKillsLoc;
        private Vector2 allTimeHitPercentLoc;
        private Vector2 avgShotsPerGameLoc;
        private Vector2 avgKillsPerGameLoc;

        //Store the level stats label locations
        private Vector2 curLevelLoc;
        private Vector2 curTotScoreLoc;
        private Vector2[] lvlScoresLocs = new Vector2[Gameplay.NUM_LEVELS];
        private Vector2 lvlKillsLoc;
        private Vector2 lvlShotsFiredLoc;
        private Vector2 lvlHitsLoc;
        private Vector2 lvlHitPercentLoc;
        private Vector2 curTotScoreShopLoc;
        private Vector2 continueMsgLoc;

        //Store the new high score message location
        private Vector2 newHSMsgLoc;

        //Store the current game state
        private int state = MENU;

        //Store whether the buttons in the menu or stats are being hovered over
        private bool hoverOverPlay;
        private bool hoverOverStats;
        private bool hoverOverExit;
        private bool hoverOverBack;

        //Store the gameplay
        private Gameplay gameplay;

        //Store the error message
        string readFileErrorMsg = "";
        string saveStatsErrorMsg = "";

        //Store the error message location
        Vector2 errorMsgStatsLoc;
        Vector2 errorMsgMenuLoc;

        //Store the background block rectangles and block types
        private Rectangle[,] bgBlockRecs = new Rectangle[NUM_ROWS, NUM_COLS];
        private int[,,] blockArrangement = new int[Gameplay.NUM_LEVELS, NUM_ROWS, NUM_COLS]
        {
            {
                {3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3 },
                {3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 },
                {0, 1, 1, 2, 2, 1, 1, 1, 2, 2, 1, 0 },
                {0, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 0 },
                {0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 },
                {0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 },
                {0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 0 },
                {0, 1, 1, 1, 2, 2, 2, 2, 2, 1, 1, 0 },
                {3, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 3 },
                {3, 3, 1, 1, 1, 1, 2, 1, 1, 1, 3, 3 }
            },
            {
                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 }, 
                {3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2 },
                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                {3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2 },
                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                {3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2 },
                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                {3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2 },
                {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                {3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2 }
            },
            {
                {3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2 },
                {3, 3, 2, 3, 3, 2, 3, 2, 2, 3, 3, 3 },
                {2, 3, 2, 3, 3, 3, 3, 3, 2, 3, 3, 3 },
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {2, 3, 3, 3, 3, 2, 3, 3, 1, 1, 3, 3 },
                {3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 3, 2 },
                {0, 0, 0, 0, 3, 3, 3, 2, 1, 1, 3, 3 },
                {0, 0, 0, 0, 3, 3, 3, 3, 1, 1, 2, 3 },
                {0, 0, 0, 0, 3, 2, 3, 2, 1, 1, 3, 3 },
            },
            {
                {1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1 },
                {1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1 },
                {0, 3, 3, 3, 0, 3, 0, 3, 3, 3, 0, 3 },
                {0, 3, 3, 3, 0, 3, 0, 0, 3, 0, 0, 3 },
                {3, 0, 3, 0, 3, 3, 0, 3, 0, 3, 0, 3 },
                {3, 3, 0, 3, 3, 3, 0, 3, 0, 3, 0, 3 },
                {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                {1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1 },
                {1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1 }
            },
            {
                {1, 1, 3, 3, 1, 3, 3, 0, 0, 3, 1, 1 },
                {1, 1, 1, 3, 3, 3, 3, 0, 0, 3, 1, 1 },
                {1, 1, 3, 3, 3, 3, 3, 0, 0, 3, 1, 1 },
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                {3, 3, 3, 3, 1, 3, 3, 0, 0, 3, 1, 1 },
                {3, 2, 2, 2, 2, 1, 3, 0, 0, 3, 1, 1 },
                {3, 2, 1, 2, 2, 3, 3, 0, 0, 3, 1, 1 },
                {3, 2, 2, 2, 2, 3, 3, 0, 0, 3, 1, 1 },
                {3, 3, 3, 3, 3, 3, 3, 0, 0, 3, 1, 1 }
            }
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            //Set the game screen size
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            
            //Set the mouse as visible
            IsMouseVisible = true;

            //Apply the changes to the game graphics
            graphics.ApplyChanges();

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
            //Load the fonts
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            instrFont = Content.Load<SpriteFont>("Fonts/InstrFont");
            boldFont = Content.Load<SpriteFont>("Fonts/BoldFont");

            //Load the music
            gameSong = Content.Load<Song>("Audio/Music/Gameplay");
            menuSong = Content.Load<Song>("Audio/Music/Menu");
            resultsSong = Content.Load<Song>("Audio/Music/Results");
            shopSong = Content.Load<Song>("Audio/Music/Shop");

            //Load the sound effects
            arrowImpactSnd = Content.Load<SoundEffect>("Audio/Sounds/ArrowImpact");
            bowShootSnd = Content.Load<SoundEffect>("Audio/Sounds/BowShoot");
            buttonClickSnd = Content.Load<SoundEffect>("Audio/Sounds/ButtonClick");
            endermanScreamSnd = Content.Load<SoundEffect>("Audio/Sounds/EndermanScream");
            endermanTeleportSnd = Content.Load<SoundEffect>("Audio/Sounds/EndermanTeleport");
            explodeSnd = Content.Load<SoundEffect>("Audio/Sounds/Explode");
            purchaseSnd = Content.Load<SoundEffect>("Audio/Sounds/Purchase");
            shieldHitSnd = Content.Load<SoundEffect>("Audio/Sounds/ShieldHit");

            //Load the background image
            menuBgImg = Content.Load<Texture2D>("Images/Sized/MenuBG1");
            statsBgImg = Content.Load<Texture2D>("Images/Sized/MenuBG2");
            shopBgImg = Content.Load<Texture2D>("Images/Sized/MenuBG3");

            //Load the instructional box image
            singlePixelImg = Content.Load<Texture2D>("Images/Sized/BlankPixel");

            //Load the title and button image
            menuTitleImg = Content.Load<Texture2D>("Images/Sized/Title");
            statsTitleImg = Content.Load<Texture2D>("Images/Sized/StatsTitle");
            shopTitleImg = Content.Load<Texture2D>("Images/Sized/ShopTitle");
            btnImg = Content.Load<Texture2D>("Images/Sized/Button");

            //Load the player image
            playerImg = Content.Load<Texture2D>("Images/Sized/Alex_64");

            //Load the mob images and the shield image
            villagerImg = Content.Load<Texture2D>("Images/Sized/Villager_64");
            pillagerImg = Content.Load<Texture2D>("Images/Sized/Pillager_64");
            endermanImg = Content.Load<Texture2D>("Images/Sized/Enderman_64");
            creeperImg = Content.Load<Texture2D>("Images/Sized/Creeper_64");
            skeletonImg = Content.Load<Texture2D>("Images/Sized/Skeleton_64");
            shieldImg = Content.Load<Texture2D>("Images/Sized/Shield_48");

            //Load the arrow images
            playerArrowImg = Content.Load<Texture2D>("Images/Sized/ArrowUp");
            mobArrowImg = Content.Load<Texture2D>("Images/Sized/ArrowDown");

            //Load the dying images
            dyingImg = Content.Load<Texture2D>("Images/Sized/Splat_64");
            explosionImg = Content.Load<Texture2D>("Images/Sized/Explode_200");

            //Load the block images
            blockImgs[COBBLESTONE] = Content.Load<Texture2D>("Images/Sized/Cobblestone_64");
            blockImgs[DIRT] = Content.Load<Texture2D>("Images/Sized/Dirt_64");
            blockImgs[GRASS_1] = Content.Load<Texture2D>("Images/Sized/Grass1_64");
            blockImgs[GRASS_2] = Content.Load<Texture2D>("Images/Sized/Grass2_64");

            //Load the buff signs and icons
            speedBuffShopImg = Content.Load<Texture2D>("Images/Sized/ShopSpeedBoost_300");
            damageBuffShopImg = Content.Load<Texture2D>("Images/Sized/ShopDamageBoost_300");
            fireRateBuffShopImg = Content.Load<Texture2D>("Images/Sized/ShopFireRateBoost_300");
            pointsBuffShopImg = Content.Load<Texture2D>("Images/Sized/ShopPointsBoost_300");
            speedBuffIcon = Content.Load<Texture2D>("Images/Sized/IconSpeed_32");
            damageBuffIcon = Content.Load<Texture2D>("Images/Sized/IconDamage_32");
            fireRateBuffIcon = Content.Load<Texture2D>("Images/Sized/IconFireRate_32");
            pointsBuffIcon = Content.Load<Texture2D>("Images/Sized/IconPoints_32");

            //Load the trader image
            traderImg = Content.Load<Texture2D>("Images/Sized/Trader_64");

            //Set the background rectangle
            bgRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

            //Set the title rectangles
            menuTitleRec = new Rectangle((SCREEN_WIDTH - menuTitleImg.Width) / 2, 0, menuTitleImg.Width, menuTitleImg.Height);
            statsTitleRec = new Rectangle((SCREEN_WIDTH - statsTitleImg.Width) / 2, 0, statsTitleImg.Width, statsTitleImg.Height);
            shopTitleRec = new Rectangle((SCREEN_WIDTH - shopTitleImg.Width) / 2, 0, shopTitleImg.Width, shopTitleImg.Height);

            //Set the button rectangles
            playBtnRec = new Rectangle((SCREEN_WIDTH - btnImg.Width) / 2, 220, btnImg.Width, btnImg.Height);
            statsBtnRec = new Rectangle((SCREEN_WIDTH - btnImg.Width) / 2, playBtnRec.Y + MENU_BTN_SPACING, btnImg.Width, btnImg.Height);
            exitBtnRec = new Rectangle((SCREEN_WIDTH - btnImg.Width) / 2, statsBtnRec.Y + MENU_BTN_SPACING, btnImg.Width, btnImg.Height);
            backBtnRec = exitBtnRec;
            shopPlayBtnRec = new Rectangle((SCREEN_WIDTH - btnImg.Width) / 2, SCREEN_HEIGHT / 2 + MENU_BTN_SPACING, btnImg.Width, btnImg.Height);

            //Set the button label locations
            playLabelLoc = new Vector2(playBtnRec.Center.X - titleFont.MeasureString(playBtnLabel).X / 2, playBtnRec.Center.Y - titleFont.MeasureString(playBtnLabel).Y / 2);
            statsLabelLoc = new Vector2(statsBtnRec.Center.X - titleFont.MeasureString(statsBtnLabel).X / 2, statsBtnRec.Center.Y - titleFont.MeasureString(statsBtnLabel).Y / 2);
            exitLabelLoc = new Vector2(exitBtnRec.Center.X - titleFont.MeasureString(exitBtnLabel).X / 2, exitBtnRec.Center.Y - titleFont.MeasureString(exitBtnLabel).Y / 2);
            backLabelLoc = new Vector2(backBtnRec.Center.X - titleFont.MeasureString(backBtnLabel).X / 2, backBtnRec.Center.Y - titleFont.MeasureString(backBtnLabel).Y / 2);
            shopPlayLabelLoc = new Vector2(shopPlayBtnRec.Center.X - titleFont.MeasureString(playBtnLabel).X / 2, shopPlayBtnRec.Center.Y - titleFont.MeasureString(playBtnLabel).Y / 2);

            //Set the instructional box rectangle
            instrBoxRec = new Rectangle(0, (SCREEN_HEIGHT - INSTR_BOX_HEIGHT) / 2, SCREEN_WIDTH, INSTR_BOX_HEIGHT);

            //Set the instructions locations
            instructionsLocs[0] = new Vector2((SCREEN_WIDTH - titleFont.MeasureString(instructions[0]).X) / 2, instrBoxRec.Y + INSTR_SPACING);
            instructionsLocs[1] = new Vector2((SCREEN_WIDTH - instrFont.MeasureString(instructions[1]).X) / 2, instructionsLocs[0].Y + titleFont.MeasureString(instructions[0]).Y + INSTR_SPACING);
            instructionsLocs[2] = new Vector2((SCREEN_WIDTH - instrFont.MeasureString(instructions[2]).X) / 2, instructionsLocs[1].Y + instrFont.MeasureString(instructions[1]).Y + INSTR_SPACING);
            instructionsLocs[3] = new Vector2((SCREEN_WIDTH - instrFont.MeasureString(instructions[3]).X) / 2, instructionsLocs[2].Y + instrFont.MeasureString(instructions[2]).Y + INSTR_SPACING);
            instructionsLocs[4] = new Vector2((SCREEN_WIDTH - boldFont.MeasureString(instructions[4]).X) / 2, instructionsLocs[3].Y + instrFont.MeasureString(instructions[3]).Y + INSTR_SPACING);

            //Set the stat message locations
            hScoreLoc = new Vector2(SCREEN_WIDTH / 2 - boldFont.MeasureString(hScoreMsg).X / 2, instrBoxRec.Y);
            gamesPlayedLoc = new Vector2(STAT_LEFT_BUFFER, hScoreLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            shotsFiredLoc = new Vector2(STAT_LEFT_BUFFER, gamesPlayedLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            shotsHitLoc = new Vector2(STAT_LEFT_BUFFER, shotsFiredLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            topHitPercentLoc = new Vector2(STAT_LEFT_BUFFER, shotsHitLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            avgShotsPerGameLoc = new Vector2(STAT_LEFT_BUFFER, topHitPercentLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            avgKillsPerGameLoc = new Vector2(STAT_LEFT_BUFFER, avgShotsPerGameLoc.Y + instrFont.MeasureString(hScoreMsg).Y + INSTR_SPACING);
            allTimeHitPercentLoc = new Vector2(SCREEN_WIDTH / 2, gamesPlayedLoc.Y);
            totKillsLoc = new Vector2(SCREEN_WIDTH / 2 - boldFont.MeasureString(totKillsMsg).X / 2, instrBoxRec.Bottom - INSTR_SPACING - boldFont.MeasureString(totKillsMsg).Y);

            //Set each level score location
            for (int i = 0; i < totKillsByType.Length; i++)
            {
                //Set the location of the labels showing the total mobs killed 
                totMobsKilledLocs[i] = new Vector2(SCREEN_WIDTH / 2, shotsFiredLoc.Y + (INSTR_SPACING + instrFont.MeasureString(hScoreMsg).Y) * i);
            }

            //Set the current level stats locations
            curLevelLoc = new Vector2(STAT_LEFT_BUFFER, instrBoxRec.Y + INSTR_SPACING);
            curTotScoreLoc = new Vector2(SCREEN_WIDTH - STAT_LEFT_BUFFER - boldFont.MeasureString(curTotScoreMsg).X, curLevelLoc.Y);
            lvlKillsLoc = new Vector2(STAT_LEFT_BUFFER, curTotScoreLoc.Y + boldFont.MeasureString(curTotScoreMsg).Y + INSTR_SPACING);
            lvlShotsFiredLoc = new Vector2(STAT_LEFT_BUFFER, lvlKillsLoc.Y + instrFont.MeasureString(curTotScoreMsg).Y + INSTR_SPACING);
            lvlHitsLoc = new Vector2(STAT_LEFT_BUFFER, lvlShotsFiredLoc.Y + instrFont.MeasureString(curTotScoreMsg).Y + INSTR_SPACING);
            lvlHitPercentLoc = new Vector2(STAT_LEFT_BUFFER, lvlHitsLoc.Y + instrFont.MeasureString(curTotScoreMsg).Y + INSTR_SPACING);
            continueMsgLoc = new Vector2((SCREEN_WIDTH - boldFont.MeasureString(continueMsg).X) / 2, instrBoxRec.Bottom - boldFont.MeasureString(continueMsg).Y - INSTR_SPACING);

            //Store the new high score message location
            newHSMsgLoc = new Vector2(SCREEN_WIDTH / 2 - boldFont.MeasureString(newHSMsg).X / 2, instrBoxRec.Y - INSTR_SPACING - boldFont.MeasureString(newHSMsg).Y);

            //Set each level score location
            for (int i = 0; i < Gameplay.NUM_LEVELS; i++)
            {
                //set the locations of the level score messages
                lvlScoresLocs[i] = new Vector2(SCREEN_WIDTH / 2, lvlKillsLoc.Y + (INSTR_SPACING + instrFont.MeasureString(curTotScoreMsg).Y) * i);
            }

            //Set the rectangles in the blocky background for each row
            for (int i = 0; i < bgBlockRecs.GetLength(0); i++)
            {
                //Set the rectangles for each column
                for (int j = 0; j < bgBlockRecs.GetLength(1); j++)
                {
                    //Set the rectangle for each block in the background
                    bgBlockRecs[i, j] = new Rectangle(j * BLOCK_SIDE, i * BLOCK_SIDE, BLOCK_SIDE, BLOCK_SIDE);
                }
            }

            //Set the rectangles of the buffs in the shop display
            shopBuffRecs[Gameplay.SPEED] = new Rectangle(SCREEN_WIDTH / 2 - speedBuffShopImg.Width - SHOP_SPACING, SCREEN_HEIGHT / 2 - speedBuffShopImg.Height - SHOP_SPACING, speedBuffShopImg.Width, speedBuffShopImg.Height);
            shopBuffRecs[Gameplay.DAMAGE] = new Rectangle(SCREEN_WIDTH / 2 - speedBuffShopImg.Width - SHOP_SPACING, SCREEN_HEIGHT / 2 + SHOP_SPACING, speedBuffShopImg.Width, speedBuffShopImg.Height);
            shopBuffRecs[Gameplay.FIRE] = new Rectangle(SCREEN_WIDTH / 2 + SHOP_SPACING, SCREEN_HEIGHT / 2 - fireRateBuffShopImg.Height - SHOP_SPACING, speedBuffShopImg.Width, speedBuffShopImg.Height);
            shopBuffRecs[Gameplay.POINTS] = new Rectangle(SCREEN_WIDTH / 2 + SHOP_SPACING, SCREEN_HEIGHT / 2 + SHOP_SPACING, speedBuffShopImg.Width, speedBuffShopImg.Height);

            //Set the location of the total score in the shop
            curTotScoreShopLoc = new Vector2(shopBuffRecs[Gameplay.SPEED].X, shopBuffRecs[Gameplay.POINTS].Bottom + SHOP_SPACING);

            //Set the location of the trader in the shop
            traderRec = new Rectangle(shopBuffRecs[Gameplay.SPEED].X, shopBuffRecs[Gameplay.SPEED].Y - traderImg.Height - SHOP_SPACING, traderImg.Width, traderImg.Height);

            //Set the error message location
            errorMsgStatsLoc = new Vector2(STAT_LEFT_BUFFER, instrBoxRec.Bottom);
            errorMsgMenuLoc = new Vector2(STAT_LEFT_BUFFER, 0);

            //Read the stats
            ReadStats();
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
            //Set the previous mouse state as the mouse state from the last update and update the new keyboard and mouse states to the current update
            prevKb = kb;
            prevMouse = mouse;
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();
            
            //Update the game depending on the state
            switch (state)
            {
                case MENU:
                    //Update the menu
                    UpdateMenu(gameTime);
                    break;

                case PRE_GAME:
                    //Update the pre game state
                    UpdatePreGame(gameTime);
                    break;

                case PLAY:
                    //Update the game state
                    UpdatePlay(gameTime);
                    break;

                case STATS:
                    //Update the stats state
                    UpdateStats(gameTime);
                    break;

                case END_GAME:
                    //Update the end game state
                    UpdateEndGame(gameTime);
                    break;

                case STORE:
                    //Update the shop state
                    UpdateStore(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draw the game depending on the state of the game
            switch (state)
            {
                case MENU:
                    //Draw the menu
                    DrawMenu();
                    break;

                case PRE_GAME:
                    //Draw the pre game state
                    DrawPreGame();
                    break;

                case PLAY:
                    //Draw the game state
                    DrawPlay();
                    break;

                case STATS:
                    //Draw the stats state
                    DrawStats();
                    break;

                case END_GAME:
                    //Draw the end game state
                    DrawEndGame();
                    break;

                case STORE:
                    //Draw the store
                    DrawStore();
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: none
        //Post: none
        //Desc: Save the stats by writing the values to the stats file
        private void SaveStats()
        {
            try
            {
                //Open the stats file
                outFile = File.CreateText("Stats.txt");

                //Write the highscore, games played, shots fired, shots hit, and the top hit percentage
                outFile.WriteLine(hScore);
                outFile.WriteLine(gamesPlayed);
                outFile.WriteLine(shotsFired);
                outFile.WriteLine(shotsHit);
                outFile.WriteLine(topHitPercent);

                //Write the total mobs killed for each type
                for (int i = 0; i < totKillsByType.Length; i++)
                {
                    //Write each value for the type of mob killed in total
                    outFile.Write(totKillsByType[i]);

                    //Write a comma between values to separate values
                    if (i < Enderman.TYPE)
                    {
                        //Write a comma
                        outFile.Write(",");
                    }
                }

                //Set the error message to nothing
                saveStatsErrorMsg = "";
            }
            catch (FileNotFoundException fnf)
            {
                //Set the eror message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: File was not found.";
                errorMsgMenuLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (IndexOutOfRangeException ore)
            {
                //Set the eror message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: The game tried reading past an array in the stats file.";
                errorMsgMenuLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (Exception e)
            {
                //Set the error message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: " + e.Message;
                errorMsgMenuLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            finally
            {
                //Close the stats file if it is not empty
                if (outFile != null)
                {
                    //Close the file
                    outFile.Close();
                }
            }
        }

        //Pre: none
        //Post: none
        //Desc: Read in the stats by reading in the values from the stats file
        private void ReadStats()
        {
            //Store the array of data 
            string[] data;

            try
            {
                //Open the stats file
                inFile = File.OpenText("Stats.txt");

                //Read in the high score, games played, shots fired, hits, and top hit percentage
                hScore = Convert.ToInt32(inFile.ReadLine());
                gamesPlayed = Convert.ToInt32(inFile.ReadLine());
                shotsFired = Convert.ToInt32(inFile.ReadLine());
                shotsHit = Convert.ToInt32(inFile.ReadLine());
                topHitPercent = Convert.ToInt32(inFile.ReadLine());

                //Set the data as the strings separated by commas
                data = inFile.ReadLine().Split(',');

                //Set the total mobs killed for each type
                for (int i = 0; i < totKillsByType.Length; i++)
                {
                    //Set the number of mobs killed for each type
                    totKillsByType[i] = Convert.ToInt32(data[i]);
                }

                //Set the stats messages
                hScoreMsg = "High Score: " + hScore;
                gamesPlayedMsg = "Games Played: " + gamesPlayed;
                shotsFiredMsg = "Shots Fired: " + shotsFired;
                shotsHitMsg = "Shots Hit: " + shotsHit;
                topHitPercMsg = "Top Hit Percentage in a game: " + topHitPercent + "%";
                totMobsKilledMsgs[Villager.TYPE] = "Villagers killed: " + totKillsByType[Villager.TYPE];
                totMobsKilledMsgs[Creeper.TYPE] = "Creepers killed: " + totKillsByType[Creeper.TYPE];
                totMobsKilledMsgs[Skeleton.TYPE] = "Skeletons killed: " + totKillsByType[Skeleton.TYPE];
                totMobsKilledMsgs[Pillager.TYPE] = "Pillagers killed: " + totKillsByType[Pillager.TYPE];
                totMobsKilledMsgs[Enderman.TYPE] = "Endermans killed: " + totKillsByType[Enderman.TYPE];

                //Recenter the location of the high score and total kills message
                hScoreLoc.X = SCREEN_WIDTH / 2 - boldFont.MeasureString(hScoreMsg).X / 2;
                totKillsLoc.X = SCREEN_WIDTH / 2 - boldFont.MeasureString(totKillsMsg).X / 2;

                //Calculate the total kills, all time hit percentage, and the average shots and kills per game
                totKills = totKillsByType.Sum();
                allTimeHitPercent = (int)Math.Round((double)shotsHit / Math.Max(1, shotsFired) * 100);
                avgShotsPerGame = Math.Round((double)shotsFired / Math.Max(1, gamesPlayed), 2);
                avgKillsPerGame = Math.Round((double)totKills / Math.Max(1, gamesPlayed), 2);

                //Set the messages for the calculated values
                totKillsMsg = "Total Mobs Killed: " + totKills;
                allTimeHitPercMsg = "All Time Hit Percentage: " + allTimeHitPercent + "%";
                avgShotsPerGameMsg = "Average Shots per Game: " + avgShotsPerGame;
                avgKillsPerGameMsg = "Average Kills Per Game: " + avgKillsPerGame;

                //Set the error message as empty
                readFileErrorMsg = "";
            }
            catch (FileNotFoundException fnf)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: File was not found.";
                errorMsgStatsLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (EndOfStreamException eos)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried reading past the stats file.";
                errorMsgStatsLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (IndexOutOfRangeException ore)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried reading past an array in the stats file.";
                errorMsgStatsLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (FormatException fe)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried converting invalid.";
                errorMsgStatsLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (Exception e)
            {
                //Set the error message to the default error
                readFileErrorMsg = "ERROR: " + e.Message;
                errorMsgStatsLoc.X = (SCREEN_WIDTH - boldFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            finally
            {
                //Close the file if it is not null
                if (inFile != null)
                {
                    //Close the stats file
                    inFile.Close();
                }
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the menu state
        private void UpdateMenu(GameTime gameTime)
        {
            //Play the music if it is not playing
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= menuSong.Duration)
            {
                //Play the menu music
                MediaPlayer.Play(menuSong);
            }

            //Set whether the mouse is hovering over the buttons
            hoverOverPlay = playBtnRec.Contains(mouse.Position);
            hoverOverStats = statsBtnRec.Contains(mouse.Position);
            hoverOverExit = exitBtnRec.Contains(mouse.Position);

            //Change the game state depending on the button clicked
            if (hoverOverPlay && MouseIsClicked())
            {
                //Change the state to pregame and create a new instance of a gameplay
                state = PRE_GAME;
                gameplay = new Gameplay(titleFont, singlePixelImg, playerImg, villagerImg, creeperImg, skeletonImg, pillagerImg, endermanImg, shieldImg, playerArrowImg, mobArrowImg, dyingImg, explosionImg, speedBuffIcon, damageBuffIcon, fireRateBuffIcon, pointsBuffIcon, arrowImpactSnd, bowShootSnd, endermanScreamSnd, endermanTeleportSnd, explodeSnd, shieldHitSnd);

                //Reset the statistics
                curTotScore = 0;
                gameShotsFired = 0;
                gameHits = 0;
                lvlHitPercent = 0;
                gameHitPercent = 0;

                //Reset the level scores and their messages
                for (int i = 0; i < Gameplay.NUM_LEVELS; i++)
                {
                    //Set each level score to 0 and the message to uncompleted
                    lvlScores[i] = 0;
                    lvlScoresMsgs[i] = "Level " + i + " Score: Uncompleted"; 
                }

                //Play the button click sound
                buttonClickSnd.CreateInstance().Play();

                //Stop the music
                MediaPlayer.Stop();
            }
            else if (hoverOverStats && MouseIsClicked())
            {
                //Change the state to the stats page
                state = STATS;

                //Play the button click sound
                buttonClickSnd.CreateInstance().Play();
            }
            else if (hoverOverExit && MouseIsClicked())
            {
                //Exit the game
                Exit();
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the pre-game state
        private void UpdatePreGame(GameTime gameTime)
        {
            //Change the state to play when space is pressed
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
            {
                //Change the state to play
                state = PLAY;
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the play state
        private void UpdatePlay(GameTime gameTime)
        {
            //Play music if the music is not playing
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= gameSong.Duration)
            {
                //Play the game song
                MediaPlayer.Play(gameSong);
            }

            //Update the level and end the game if the level is over
            if (gameplay.UpdateLevelDetermineEndGame(gameTime, kb, prevKb))
            {
                //Set the state to the end of game state
                state = END_GAME;

                //Set the current total score and level score to the total score
                curTotScore = gameplay.GetTotPoints();
                lvlScores[gameplay.GetLevel() - 1] = gameplay.GetTotPoints();

                //Set the level score
                for (int i = 0; i < gameplay.GetLevel() - 1; i++)
                {
                    //Subtract the previous level scores from the total score to get the level score
                    lvlScores[gameplay.GetLevel() - 1] -= lvlScores[i];
                }

                //Increase the hits, and shots fired from that level
                gameHits += gameplay.GetLvlHits();
                gameShotsFired += gameplay.GetLvlShotsFired();
                
                //Set the messages for the level stats
                curLevelMsg = "Level " + gameplay.GetLevel() + " Completed!";
                lvlScoresMsgs[gameplay.GetLevel() - 1] = "Level " + gameplay.GetLevel() + " Score: " + lvlScores[gameplay.GetLevel() - 1];
                curTotScoreMsg = "Total Score: " + curTotScore;
                lvlKillsMsg = "Level Kills: " + gameplay.GetLvlKills();
                lvlHitsMsg = "Level Hits: " + gameplay.GetLvlHits();
                lvlShotsFiredMsg = "Level Shots Fired: " + gameplay.GetLvlShotsFired();
                lvlHitPercent = (int)Math.Round((double)gameplay.GetLvlHits() / Math.Max(1, gameplay.GetLvlShotsFired()) * 100);
                lvlHitPercMsg = "Level Hit Percentage: " + lvlHitPercent + "%";

                //Change the current total score message location
                curTotScoreLoc.X = SCREEN_WIDTH - STAT_LEFT_BUFFER - boldFont.MeasureString(curTotScoreMsg).X;

                //Stop the music
                MediaPlayer.Stop();
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the stats state
        private void UpdateStats(GameTime gameTime)
        {
            //Set whether the mouse is hovering over the back button
            hoverOverBack = backBtnRec.Contains(mouse.Position);

            //If the mouse clicked on the back button change the game state
            if (hoverOverBack && MouseIsClicked())
            {
                //Set the state to menu
                state = MENU;

                //Play the button click sound
                buttonClickSnd.CreateInstance().Play();
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the end game state
        private void UpdateEndGame(GameTime gameTime)
        {
            //Play the end game music
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= resultsSong.Duration)
            {
                //Play the results song
                MediaPlayer.Play(resultsSong);
            }

            //Change to the next state if space is pressed
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
            {
                //Change to the menu and save the stats if all levels are completed
                if (gameplay.GetLevel() >= Gameplay.NUM_LEVELS)
                {
                    //Increase the games played and update the message
                    gamesPlayed++;
                    gamesPlayedMsg = "Games Played: " + gamesPlayed;

                    //Get the total mobs killed in that level for each type of mob
                    for (int i = 0; i < totKillsByType.Length; i++)
                    {
                        //Increase the total mobs killed for each type of mob
                        totKillsByType[i] += gameplay.GetKillsByType()[i];
                    }

                    //Set the messages for the total kills by type
                    totMobsKilledMsgs[Villager.TYPE] = "Villagers killed: " + totKillsByType[Villager.TYPE];
                    totMobsKilledMsgs[Creeper.TYPE] = "Creepers killed: " + totKillsByType[Creeper.TYPE];
                    totMobsKilledMsgs[Skeleton.TYPE] = "Skeletons killed: " + totKillsByType[Skeleton.TYPE];
                    totMobsKilledMsgs[Pillager.TYPE] = "Pillagers killed: " + totKillsByType[Pillager.TYPE];
                    totMobsKilledMsgs[Enderman.TYPE] = "Endermans killed: " + totKillsByType[Enderman.TYPE];

                    //Set the high score it is beat
                    if (curTotScore > hScore)
                    {
                        //Set the high score as the current total score, the high score message, and the location in the stats
                        hScore = curTotScore;
                        hScoreMsg = "High Score: " + hScore;
                        hScoreLoc.X = SCREEN_WIDTH / 2 - boldFont.MeasureString(hScoreMsg).X / 2;
                    }
                    
                    //Set the shots fired, hits, and kills and their messages
                    shotsFired += gameShotsFired;
                    shotsHit += gameHits;
                    totKills = totKillsByType.Sum();
                    shotsFiredMsg = "Shots fired: " + shotsFired;
                    shotsHitMsg = "Shots Hit: " + shotsHit;
                    totKillsMsg = "Total Mobs Killed: " + totKills;
                    
                    //Recenter the location of the total kills message
                    totKillsLoc.X = SCREEN_WIDTH / 2 - boldFont.MeasureString(totKillsMsg).X / 2;

                    //Set the all time hit percent, average shots per game, average kills per game, and their messages
                    allTimeHitPercent = (int)Math.Round((double)shotsHit / Math.Max(1, shotsFired) * 100);
                    avgShotsPerGame = Math.Round((double)shotsFired / Math.Max(1, gamesPlayed), 2);
                    avgKillsPerGame = (int)Math.Round((double)totKills / Math.Max(1, gamesPlayed), 2);
                    allTimeHitPercMsg = "All Time Hit Percentage: " + allTimeHitPercent + "%";
                    avgShotsPerGameMsg = "Average Shots per Game: " + avgShotsPerGame;
                    avgKillsPerGameMsg = "Average Kills Per Game: " + avgKillsPerGame;

                    //Set the game hit percentage
                    gameHitPercent = (int)Math.Round((double)gameHits / Math.Max(1, gameShotsFired) * 100);

                    //Set the high score it is beat
                    if (gameHitPercent > topHitPercent)
                    {
                        //Set the top hit percentage as the game hit percent and the top hit percent message
                        topHitPercent = gameHitPercent;
                        topHitPercMsg = "Top Hit Percentage: " + topHitPercent + "%";
                    }
                    

                    //Set the state to menu and save the stats
                    state = MENU;
                    SaveStats();
                }
                else
                {
                    //Change the state to the store
                    state = STORE;
                }

                //Stop the music
                MediaPlayer.Stop();
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the store state
        private void UpdateStore(GameTime gameTime)
        {
            //Play the music if it is not playing
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= shopSong.Duration)
            {
                //Play the shop song
                MediaPlayer.Play(shopSong);
            }

            //Set whether the mouse is hovering over the play button
            hoverOverPlay = shopPlayBtnRec.Contains(mouse.Position);

            //Change the state or purchase buffs if the specific button is clicked
            if (hoverOverPlay && MouseIsClicked())
            {
                //Set the state to the pre-game and setup the level
                state = PRE_GAME;
                gameplay.SetupLevel(curTotScore);

                //Stop the music
                MediaPlayer.Stop();

                //Play the button click sound
                buttonClickSnd.CreateInstance().Play();
            }
            else if (shopBuffRecs[Gameplay.SPEED].Contains(mouse.Position) && MouseIsClicked())
            {
                //Purchase the speed buff
                PurchaseBuff(Gameplay.SPEED);
            }
            else if (shopBuffRecs[Gameplay.DAMAGE].Contains(mouse.Position) && MouseIsClicked())
            {
                //Purchase the damage buff
                PurchaseBuff(Gameplay.DAMAGE);
            }
            else if (shopBuffRecs[Gameplay.FIRE].Contains(mouse.Position) && MouseIsClicked())
            {
                //Purchase the fire rate buff
                PurchaseBuff(Gameplay.FIRE);
            }
            else if (shopBuffRecs[Gameplay.POINTS].Contains(mouse.Position) && MouseIsClicked())
            {
                //Purchase the points buff
                PurchaseBuff(Gameplay.POINTS);
            }
        }
        
        //Pre: none
        //Post: none
        //Desc: Draw the menu state
        private void DrawMenu()
        {
            //Draw the background image
            spriteBatch.Draw(menuBgImg, bgRec, Color.White);

            //Draw the title
            spriteBatch.Draw(menuTitleImg, menuTitleRec, Color.White);

            //Draw the buttons
            spriteBatch.Draw(btnImg, playBtnRec, Color.White);
            spriteBatch.Draw(btnImg, statsBtnRec, Color.White);
            spriteBatch.Draw(btnImg, exitBtnRec, Color.White);

            //Draw the button labels
            DrawButton(hoverOverPlay, playBtnLabel, playLabelLoc, playBtnRec);
            DrawButton(hoverOverStats, statsBtnLabel, statsLabelLoc, statsBtnRec);
            DrawButton(hoverOverExit, exitBtnLabel, exitLabelLoc, exitBtnRec);

            //Draw the error message if there is an error message
            if (saveStatsErrorMsg.Length > 0)
            {
                //Draw the error message
                spriteBatch.DrawString(boldFont, saveStatsErrorMsg, errorMsgMenuLoc, Color.Red);
            }
        }

        //Pre: none
        //Post: none
        //Desc: Draw the pregame state
        private void DrawPreGame()
        {
            //Draw the blocky background
            DrawBlockyBackground();

            //Draw the instructions box
            spriteBatch.Draw(singlePixelImg, instrBoxRec, Color.Black * 0.5f);
            spriteBatch.DrawString(titleFont, instructions[0], instructionsLocs[0], Color.CadetBlue);
            spriteBatch.DrawString(instrFont, instructions[1], instructionsLocs[1], Color.White);
            spriteBatch.DrawString(instrFont, instructions[2], instructionsLocs[2], Color.White);
            spriteBatch.DrawString(instrFont, instructions[3], instructionsLocs[3], Color.White);
            spriteBatch.DrawString(boldFont, instructions[4], instructionsLocs[4], Color.Yellow);
        }

        //Pre: none
        //Post: none
        //Desc: Draw the play state
        private void DrawPlay()
        {
            //Draw the blocky background
            DrawBlockyBackground();
            
            //Draw the gameplay
            gameplay.Draw(spriteBatch, titleFont);
        }

        //Pre: none
        //Post: none
        //Desc: Draw the stats state
        private void DrawStats()
        {
            //Draw the background image
            spriteBatch.Draw(statsBgImg, bgRec, Color.White);

            //Draw the title
            spriteBatch.Draw(statsTitleImg, statsTitleRec, Color.White);

            //Draw the instructions box
            spriteBatch.Draw(singlePixelImg, instrBoxRec, Color.Black * 0.5f);

            //Write all the stats
            spriteBatch.DrawString(boldFont, hScoreMsg, hScoreLoc, Color.Cyan);
            spriteBatch.DrawString(instrFont, topHitPercMsg, topHitPercentLoc, Color.White);
            spriteBatch.DrawString(instrFont, gamesPlayedMsg, gamesPlayedLoc, Color.White);
            spriteBatch.DrawString(instrFont, shotsFiredMsg, shotsFiredLoc, Color.White);
            spriteBatch.DrawString(instrFont, shotsHitMsg, shotsHitLoc, Color.White);
            spriteBatch.DrawString(instrFont, allTimeHitPercMsg, allTimeHitPercentLoc, Color.White);
            spriteBatch.DrawString(instrFont, avgShotsPerGameMsg, avgShotsPerGameLoc, Color.White);
            spriteBatch.DrawString(instrFont, avgKillsPerGameMsg, avgKillsPerGameLoc, Color.White);
            spriteBatch.DrawString(boldFont, totKillsMsg, totKillsLoc, Color.Magenta);

            //Write the stats for the total number of each type of mob killed
            for (int i = 0; i < totKillsByType.Length; i++)
            {
                //Write the number of mobs killed for each type
                spriteBatch.DrawString(instrFont, totMobsKilledMsgs[i], totMobsKilledLocs[i], Color.White);
            }
            
            //Draw the back button
            DrawButton(hoverOverBack, backBtnLabel, backLabelLoc, backBtnRec);
                
            //Draw the error message if there is an error message message
            if (readFileErrorMsg.Length > 0)
            {
                //Draw the error message
                spriteBatch.DrawString(boldFont, readFileErrorMsg, errorMsgStatsLoc, Color.Red);
            }
        }

        //Pre: none
        //Post: none
        //Desc: Draw the end game state
        private void DrawEndGame()
        {
            //Draw the blocky background
            DrawBlockyBackground();

            //Draw the instructions box
            spriteBatch.Draw(singlePixelImg, instrBoxRec, Color.Black * 0.5f);

            //Draw the stats of the current level
            spriteBatch.DrawString(boldFont, curLevelMsg, curLevelLoc, Color.Yellow);
            spriteBatch.DrawString(boldFont, curTotScoreMsg, curTotScoreLoc, Color.Yellow);
            spriteBatch.DrawString(instrFont, lvlKillsMsg, lvlKillsLoc, Color.White);
            spriteBatch.DrawString(instrFont, lvlShotsFiredMsg, lvlShotsFiredLoc, Color.White);
            spriteBatch.DrawString(instrFont, lvlHitsMsg, lvlHitsLoc, Color.White);
            spriteBatch.DrawString(instrFont, lvlHitPercMsg, lvlHitPercentLoc, Color.White);

            //Write each level's score
            for (int i = 0; i < Gameplay.NUM_LEVELS; i++)
            {
                //Write the score for each level
                spriteBatch.DrawString(instrFont, lvlScoresMsgs[i], lvlScoresLocs[i], Color.White);
            }
            
            //Draw the continue message
            spriteBatch.DrawString(boldFont, continueMsg, continueMsgLoc, Color.Yellow);

            //Draw the new high score message if there is a new high score
            if (curTotScore > hScore && gameplay.GetLevel() >= Gameplay.NUM_LEVELS)
            {
                //Draw the new high score message
                spriteBatch.DrawString(boldFont, newHSMsg, newHSMsgLoc, Color.Yellow);
            }
        }

        //Pre: none
        //Post: none
        //Desc: Draw the store state
        private void DrawStore()
        {
            //Draw the shop background image
            spriteBatch.Draw(shopBgImg, bgRec, Color.White);

            //Draw the shop title
            spriteBatch.Draw(shopTitleImg, shopTitleRec, Color.White);

            //Draw the trader
            spriteBatch.Draw(traderImg, traderRec, Color.White);

            //Draw the buffs for purchase
            spriteBatch.Draw(speedBuffShopImg, shopBuffRecs[Gameplay.SPEED], Color.White * ((Convert.ToInt32(!gameplay.GetBuffsOn()[Gameplay.SPEED] && curTotScore >= BUFF_COSTS[Gameplay.SPEED]) + 1) * 0.5f));
            spriteBatch.Draw(damageBuffShopImg, shopBuffRecs[Gameplay.DAMAGE], Color.White * ((Convert.ToInt32(!gameplay.GetBuffsOn()[Gameplay.DAMAGE] && curTotScore >= BUFF_COSTS[Gameplay.DAMAGE]) + 1) * 0.5f));
            spriteBatch.Draw(fireRateBuffShopImg, shopBuffRecs[Gameplay.FIRE], Color.White * ((Convert.ToInt32(!gameplay.GetBuffsOn()[Gameplay.FIRE] && curTotScore >= BUFF_COSTS[Gameplay.FIRE]) + 1) * 0.5f));
            spriteBatch.Draw(pointsBuffShopImg, shopBuffRecs[Gameplay.POINTS], Color.White * ((Convert.ToInt32(!gameplay.GetBuffsOn()[Gameplay.POINTS] && curTotScore >= BUFF_COSTS[Gameplay.POINTS]) + 1) * 0.5f));

            //Draw the total score
            spriteBatch.DrawString(boldFont, curTotScoreMsg, curTotScoreShopLoc, Color.Yellow);
            
            //Draw the play button
            DrawButton(hoverOverPlay, playBtnLabel, shopPlayLabelLoc, shopPlayBtnRec);
        }

        //Pre: none
        //Post: returns whether the mouse is clicked
        //Desc: Determine whether the mouse is clicked based on the current and previous mouse state
        private bool MouseIsClicked()
        {
            //Return whether the mouse is clicked on the first update
            return mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed;
        }

        //Pre: hoverOverBtn is a bool for whether the mouse is hovering over the button, btnLabel is the button's label, labelLoc is the location of the label, and btnRec is the button's rectangle within the screen dimensions
        //Post: none
        //Desc: Draws the button with its label and highlight when the mouse is hovering over the button
        private void DrawButton(bool hoverOverBtn, string btnLabel, Vector2 labelLoc, Rectangle btnRec)
        {
            //Draw the button
            spriteBatch.Draw(btnImg, btnRec, Color.White);

            //Draw the letters depending on whether the button is being hovered over
            if (hoverOverBtn)
            {
                //Draw the label in yellow with a shadow
                spriteBatch.DrawString(titleFont, btnLabel, new Vector2(labelLoc.X + SHADOW_DISPLACEMENT, labelLoc.Y + SHADOW_DISPLACEMENT), Color.Black);
                spriteBatch.DrawString(titleFont, btnLabel, labelLoc, Color.Yellow);
            }
            else
            {
                //Draw the label in white
                spriteBatch.DrawString(titleFont, btnLabel, labelLoc, Color.White);
            }
        }

        //Pre: none
        //Post: none
        //Desc: Draw the background made up of blocks in the screen dimensions
        private void DrawBlockyBackground()
        {
            //Draw the background blocks in each row
            for (int i = 0; i < bgBlockRecs.GetLength(0); i++)
            {
                //Draw the blocks in each column
                for (int j = 0; j < bgBlockRecs.GetLength(1); j++)
                {
                    //Draw each block
                    spriteBatch.Draw(blockImgs[blockArrangement[gameplay.GetLevel() - 1, i, j]], bgBlockRecs[i, j], Color.White);
                }
            }
        }
        
        //Pre: buffType is an int from 0 to 3 representing the buff type
        //Post: none
        //Desc: Purchases the buff type by setting it on and reducing the points from the player
        private void PurchaseBuff(int buffType)
        {
            //Purchase the buff if it is not on and if there is enough points to buy it
            if (curTotScore >= BUFF_COSTS[buffType] && !gameplay.GetBuffsOn()[buffType])
            {
                //Subtract the cost from the current level score and total score, update the messages, and set the buff on
                lvlScores[gameplay.GetLevel() - 1] -= BUFF_COSTS[buffType];
                lvlScoresMsgs[gameplay.GetLevel() - 1] = "Level " + gameplay.GetLevel() + " Score: " + lvlScores[gameplay.GetLevel() - 1];
                curTotScore -= BUFF_COSTS[buffType];
                curTotScoreMsg = "Total Score: " + curTotScore;
                gameplay.SetBuffOn(true, buffType);

                //Play the purchase sound
                purchaseSnd.CreateInstance().Play();
            }
        }
    }
}
