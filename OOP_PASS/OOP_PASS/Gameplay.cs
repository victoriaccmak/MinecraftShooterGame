//Author: Victoria Mak
//File Name: Gameplay.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Gameplay class, which runs the game logic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace OOP_PASS
{
    class Gameplay
    {
        //Store the spawn odds of the mobs
        private readonly int[] VILLAGER_SPAWN_ODDS = { 70, 50, 40, 50, 10 };
        private readonly int[] CREEPER_SPAWN_ODDS = { 90, 80, 60, 65, 30 };
        private readonly int[] SKELETON_SPAWN_ODDS = { 100, 100, 80, 80, 55 };
        private readonly int[] PILLAGER_SPAWN_ODDS = { 100, 100, 100, 95, 80 };
        private readonly int[] ENDERMAN_SPAWN_ODDS = { 100, 100, 100, 100, 100 };

        //Store the number of levels
        public const int NUM_LEVELS = 5;

        //Store the icon spacing
        private const int ICON_SPACING = 50;
        
        //Store the starting number of mobs and the amount of mobs to increase per level
        private const int LVL_1_MOBS = 10;
        private const int MOB_INCREASE_PER_LVL = 5;

        //Store the buff numbers
        public const int SPEED = 0;
        public const int DAMAGE = 1;
        public const int FIRE = 2;
        public const int POINTS = 3;
        
        //Store the number of mobs allowed at once and the spawn times of mobs in milliseconds
        private readonly int[] MOBS_ALLOWED_AT_ONCE = { 2, 3, 3, 5, 3 };
        private readonly float[] SPAWN_TIMES = { 2000f, 1700f, 1300f, 1200f, 1000f };

        //Store the player, list of mobs, and lists of arrows from players and from the mobs
        private Player player;
        private List<Mob> mobs = new List<Mob>();
        private List<Arrow> playerArrows = new List<Arrow>();
        private List<Arrow> mobArrows = new List<Arrow>();

        //Store the points and the level
        private int totPoints;
        private int level;
        
        //Store the number of mobs left and the spawn timer
        private int mobsLeft;
        private Timer spawnTimer;

        //Store the player box image and its rectangle
        private Texture2D playerBoxImg;
        private Rectangle playerBoxRec;

        //Store the mob images and the shield image
        private Texture2D villagerImg;
        private Texture2D creeperImg;
        private Texture2D skeletonImg;
        private Texture2D pillagerImg;
        private Texture2D endermanImg;
        private Texture2D shieldImg;

        //Store the dying images
        private Texture2D dyingImg;
        private Texture2D explosionImg;

        //Store the arrow images
        private Texture2D playerArrowImg;
        private Texture2D mobArrowImg;

        //Store the buff images
        private Texture2D speedBuffIcon;
        private Texture2D damageBuffIcon;
        private Texture2D fireRateBuffIcon;
        private Texture2D pointsBuffIcon;

        //Store the sound effects
        private SoundEffect arrowImpactSnd;
        private SoundEffect bowShootSnd;
        private SoundEffect endermanScreamSnd;
        private SoundEffect endermanTeleportSnd;
        private SoundEffect explodeSnd;
        private SoundEffect shieldHitSnd;

        //Store the buff rectangles
        private Rectangle speedIconRec;
        private Rectangle damageIconRec;
        private Rectangle fireRateIconRec;
        private Rectangle pointsIconRec;

        private Vector2 scoreMsgLoc;

        //Store the font used for to show the score
        private SpriteFont scoreFont;

        //Store the number of kills, shots fired, and hits in the level and the number of each type of mob killed
        private int lvlKills;
        private int lvlShotsFired;
        private int lvlHits;
        private int[] killsByType = new int[5] { 0, 0, 0, 0, 0 };
        
        //Store whether the buffs are on
        private bool[] buffsOn = new bool[4] { false, false, false, false };

        //Store the random number generator
        private Random rng = new Random();

        public Gameplay(SpriteFont scoreFont, Texture2D playerBoxImg, Texture2D playerImg, Texture2D villagerImg, Texture2D creeperImg, Texture2D skeletonImg, Texture2D pillagerImg, Texture2D endermanImg, Texture2D shieldImg, Texture2D playerArrowImg, Texture2D mobArrowImg, Texture2D dyingImg, Texture2D explosionImg, Texture2D speedBuffIcon, Texture2D damageBuffIcon, Texture2D fireRateBuffIcon, Texture2D pointsBuffIcon, SoundEffect arrowImpactSnd, SoundEffect bowShootSnd, SoundEffect endermanScreamSnd, SoundEffect endermanTeleportSnd, SoundEffect explodeSnd, SoundEffect shieldHitSnd)
        {
            //Set the score font
            this.scoreFont = scoreFont;

            //Set the arrow images
            this.playerArrowImg = playerArrowImg;
            this.mobArrowImg = mobArrowImg;

            //Set the player box image and its rectangle
            this.playerBoxImg = playerBoxImg;
            playerBoxRec = new Rectangle(0, Game1.SCREEN_HEIGHT - Game1.BLOCK_SIDE, Game1.SCREEN_WIDTH, Game1.BLOCK_SIDE);

            //Set the mob and shield images
            this.villagerImg = villagerImg;
            this.creeperImg = creeperImg;
            this.skeletonImg = skeletonImg;
            this.pillagerImg = pillagerImg;
            this.endermanImg = endermanImg;
            this.shieldImg = shieldImg;

            //Set the dying images
            this.dyingImg = dyingImg;
            this.explosionImg = explosionImg;

            //Set the icon images
            this.speedBuffIcon = speedBuffIcon;
            this.damageBuffIcon = damageBuffIcon;
            this.fireRateBuffIcon = fireRateBuffIcon;
            this.pointsBuffIcon = pointsBuffIcon;

            //Set the sound effects
            this.arrowImpactSnd = arrowImpactSnd;
            this.bowShootSnd = bowShootSnd;
            this.endermanScreamSnd = endermanScreamSnd;
            this.endermanTeleportSnd = endermanTeleportSnd;
            this.explodeSnd = explodeSnd;
            this.shieldHitSnd = shieldHitSnd;

            //Create a new player
            player = new Player(playerImg, playerArrowImg, bowShootSnd, arrowImpactSnd);

            //Set the level and points
            level = 1;
            totPoints = 0;

            //Set the spawn timer as a new timer and the number of mobs left
            spawnTimer = new Timer(SPAWN_TIMES[0], true);
            mobsLeft = LVL_1_MOBS;

            //Set the buff icon rectangles
            speedIconRec = new Rectangle(Game1.SCREEN_WIDTH - ICON_SPACING, Game1.SCREEN_HEIGHT - ICON_SPACING, speedBuffIcon.Width, speedBuffIcon.Height);
            damageIconRec = new Rectangle(Game1.SCREEN_WIDTH - ICON_SPACING, speedIconRec.Y - ICON_SPACING, speedBuffIcon.Width, speedBuffIcon.Height);
            fireRateIconRec = new Rectangle(Game1.SCREEN_WIDTH - ICON_SPACING, damageIconRec.Y - ICON_SPACING, speedBuffIcon.Width, speedBuffIcon.Height);
            pointsIconRec = new Rectangle(Game1.SCREEN_WIDTH - ICON_SPACING, fireRateIconRec.Y - ICON_SPACING, speedBuffIcon.Width, speedBuffIcon.Height);

            //Set the score message location
            scoreMsgLoc = new Vector2(50, 400);
        }

        //Pre: none
        //Post: Returns the total number of points 
        //Desc: Gets the number of points in total from the game
        public int GetTotPoints()
        {
            //Return the current number of points 
            return totPoints;
        }

        //Pre: none
        //Post: Returns the current level as a positive integer less than or equal to the max number of levels
        //Desc: Gets the current level
        public int GetLevel()
        {
            //Return the level
            return level;
        }

        //Pre: none
        //Post: Returns the array of ints for the number of mobs killed for each type
        //Desc: Gets the number of mobs killed for each type in an array of ints
        public int[] GetKillsByType()
        {
            //Return the number of mobs killed for each type
            return killsByType;
        }

        //Pre: none
        //Post: Returns the number of mobs killed for the level as a non-negative int
        //Desc: Get the number of kills in the level
        public int GetLvlKills()
        {
            //Return the number of kills in the level
            return lvlKills;
        }

        //Pre: none
        //Post: Returns the number of shots fired in the level as a non-negative int
        //Desc: Gets the number of shots fired in the level
        public int GetLvlShotsFired()
        {
            //Return the number of shots fired in that level
            return lvlShotsFired;
        }

        //Pre: none
        //Post: Returns the number of hits in the level as a non-negative int
        //Desc: Gets the number of hits in the level
        public int GetLvlHits()
        {
            //Return the number of hits in that level
            return lvlHits;
        }

        //Pre: none
        //Post: Returns an array of 4 bools to represent whether each buff is on
        //Desc: Gets whether or not the buffs are on
        public bool[] GetBuffsOn()
        {
            //Return whether the buffs are on
            return buffsOn;
        }

        //Pre: buffOn is a bool for whether the buff is on and typeOfBuff is an int from 0 to 3 representing the buff type
        //Post: none
        //Desc: Sets whether the specified buff is on as a bool
        public void SetBuffOn(bool buffOn, int typeOfBuff)
        {
            //Set whether the buff is on
            buffsOn[typeOfBuff] = buffOn;
        }

        //Pre: none
        //Post: none
        //Desc: Sets up the next level round
        public void SetupLevel(int totPoints)
        {
            //Set the total points
            this.totPoints = totPoints;

            //Increase the level, set the kills, shots fired, and hits as 0
            level++;
            lvlKills = 0;
            lvlShotsFired = 0;
            lvlHits = 0;

            //Set the spawn timer as a new timer and the mobs left
            spawnTimer = new Timer(SPAWN_TIMES[level - 1], true);
            mobsLeft = LVL_1_MOBS + MOB_INCREASE_PER_LVL * (level - 1);
            
            //Clear the list of mobs
            mobArrows.Clear();
            playerArrows.Clear();

            //Reset the player
            player.ResetPlayer(buffsOn[FIRE], buffsOn[SPEED]);
        }

        //Pre: kb is the current KeyboardState with data on the keys pressed, prevKb is the KeyboardState from the previous update, and gameTime is a GameTime with the elapsed time between updates
        //Post: returns whether the level is over as a bool
        //Desc: Updates the level by updatimg the different elements in the game
        public bool UpdateLevelDetermineEndGame(GameTime gameTime, KeyboardState kb, KeyboardState prevKb)
        {
            //Update the mobs, player, and arrows
            UpdateMobs(gameTime);
            UpdatePlayer(gameTime, kb, prevKb);
            UpdateArrows(gameTime);

            //Update the spawn timer
            spawnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Spawn a new mob or end the game depending on if there are mobs left to spawn and if the spawn timer is finished
            if (spawnTimer.IsFinished() && mobs.Count < MOBS_ALLOWED_AT_ONCE[level - 1] && mobsLeft > 0)
            {
                //Spawn a new mob and reset the spawn timer
                SpawnMob();
                spawnTimer.ResetTimer(true);
            }
            else if (mobsLeft <= 0 && mobs.Count == 0)
            {
                //Return the level as over
                return true;
            }

            //Return the level as not over
            return false;
        }

        //Pre: kb is the current KeyboardState with data on the keys pressed, prevKb is the KeyboardState from the previous update, and gameTime is a GameTime with the elapsed time between updates
        //Post: none
        //Desc: Updates the player by moving and shooting arrows
        private void UpdatePlayer(GameTime gameTime, KeyboardState kb, KeyboardState prevKb)
        {
            //Update the player
            player.Update(gameTime, kb);

            //Add an arrow the the list of arrows if the player pressed space and is ready to shoot
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space) && player.GetReadyToShoot())
            {
                //Increae the number of shots fired and add a new arrow into the game
                lvlShotsFired++;
                playerArrows.Add(player.ShootArrow());
            }
        }

        //Pre: none
        //Post: none
        //Desc: Spawns a new mob by randomizing the type of mob being spawned
        private void SpawnMob()
        {
            //Store the randomized mob number
            int mobNum = rng.Next(1, 101);

            //Spawn the corresponding mob to the random mob number
            if (mobNum <= VILLAGER_SPAWN_ODDS[level - 1])
            {
                //Add a new villager to the list of mobs
                mobs.Add(new Villager(villagerImg, dyingImg, new Rectangle(-villagerImg.Width, rng.Next(0, Game1.SCREEN_HEIGHT - Game1.BLOCK_SIDE - villagerImg.Height), villagerImg.Width, villagerImg.Height)));
            }
            else if (mobNum <= CREEPER_SPAWN_ODDS[level - 1])
            {
                //Add a new creeper to the list of mobs
                mobs.Add(new Creeper(creeperImg, explosionImg, new Rectangle(rng.Next(0, Game1.SCREEN_WIDTH - creeperImg.Width), -creeperImg.Height, creeperImg.Width, creeperImg.Height), player, explodeSnd));
            }
            else if (mobNum <= SKELETON_SPAWN_ODDS[level - 1])
            {
                //Add a new skeleton to the list of mobs
                mobs.Add(new Skeleton(skeletonImg, mobArrowImg, dyingImg, bowShootSnd, arrowImpactSnd));
            }
            else if (mobNum <= PILLAGER_SPAWN_ODDS[level - 1])
            {
                //Add a new pillager to the list of mobs
                mobs.Add(new Pillager(pillagerImg, new Rectangle(-pillagerImg.Width, rng.Next(0, Game1.SCREEN_HEIGHT - Game1.BLOCK_SIDE - pillagerImg.Height - 2 * Pillager.CUSP), pillagerImg.Width, pillagerImg.Height), dyingImg, shieldImg, shieldHitSnd));
            }
            else
            {
                //Add a new enderman to the list of mobs
                mobs.Add(new Enderman(endermanImg, dyingImg, player, endermanScreamSnd, endermanTeleportSnd));
            }

            //Decrease the number of mobs left
            mobsLeft--;
        }

        //Pre: gameTime is a GameTime containing the elapsed game time between updates
        //Post: none
        //Desc: Updates all the arrows shot by the player and mobs
        private void UpdateArrows(GameTime gameTime)
        {
            //Update the player's arrows
            for (int i = 0; i < playerArrows.Count; i++)
            {
                //Move each of the player's arrows
                playerArrows[i].Move(gameTime);

                //Remove the player's arrow or detect for a collision with a mob
                if (playerArrows[i].GetOffScreen())
                {
                    //Remove the arrow 
                    playerArrows.RemoveAt(i);
                    i--;
                }
                else if (CheckMobArrowCollision(playerArrows[i], buffsOn[DAMAGE], buffsOn[POINTS]))
                {
                    //Play the arrow impact sound
                    playerArrows[i].PlayArrowImpactSnd();

                    //Increase the number of hits in the level and remove that arrow
                    lvlHits++;
                    playerArrows.Remove(playerArrows[i]);
                    i--;
                }
            }

            //Update all of the mob's arrows
            for (int i = 0; i < mobArrows.Count; i++)
            {
                //Move each of the mob's arrows
                mobArrows[i].Move(gameTime);

                //Detect whether the arrow is off screen or collided with the player
                if (mobArrows[i].GetOffScreen())
                {
                    //Remove the mob's arrow
                    mobArrows.RemoveAt(i);
                    i--;
                }
                else if (mobArrows[i].CheckPlayerCollision(player))
                {
                    //Decrease the total points from the arrow's damage
                    totPoints = Math.Max(0, totPoints - Arrow.DAMAGE_TO_PLAYER);

                    //Set all buffs off
                    buffsOn[SPEED] = false;
                    buffsOn[DAMAGE] = false;
                    buffsOn[FIRE] = false;
                    buffsOn[POINTS] = false;

                    //Set the player's speed and fire rate according to the buff being off
                    player.SetCurSpeed(buffsOn[SPEED]);
                    player.SetFireRate(buffsOn[FIRE]);

                    //Play the arrow impact sound
                    mobArrows[i].PlayArrowImpactSnd();

                    //Remove that arrow
                    mobArrows.Remove(mobArrows[i]);
                    i--;
                }
            }
        }

        //Pre: playerArrow is an arrow shot by the player; damageBuffOn and pointsBuffOn are bools describing whether those buffs are on
        //Post: Returns a bool for whether the arrow collided with a mob
        //Desc: Detects whether the arrow shot by the player has collided with any of the mobs and reduce the mob's health
        private bool CheckMobArrowCollision(Arrow playerArrow, bool damageBuffOn, bool pointsBuffOn)
        {
            //Store the award points
            int awardPts;

            //Detect for a collision with all of the mobs on screen
            for (int j = 0; j < mobs.Count; j++)
            {
                //Only detect for a collision if the mob is alive
                if (mobs[j].GetState() == Mob.ALIVE)
                {
                    //Check for a collision with the mob
                    if (playerArrow.CheckMobCollision(mobs[j]))
                    {
                        //Set the points being awarded and increase the total points by the points awarded
                        awardPts = mobs[j].ReduceHealthAndReturnPoints(damageBuffOn, pointsBuffOn);
                        totPoints += awardPts;

                        //Increase the number of kills if points are being awarded
                        if (awardPts > 0)
                        {
                            //Increase the number of kills for that type of mob and for mobs in general
                            killsByType[mobs[j].GetTypeNum()]++;
                            lvlKills++;
                        }

                        //Return that the mob has been hit
                        return true;
                    }
                }
            }

            //Return the arrow as not collided with any mobs
            return false;
        }

        //Pre: gameTime is a GameTime containing the elapsed game time between updates
        //Post: none
        //Desc: Updates all the mobs on their actions based on their state
        private void UpdateMobs(GameTime gameTime)
        {
            //Update each mob in the list of mobs on screen
            for (int i = 0; i < mobs.Count; i++)
            {
                //Update the mob
                mobs[i].Update(gameTime);

                //Do special additional updates if the mob is a creeper or skeleton
                if (mobs[i] is Creeper)
                {
                    //Remove points if points are needed to be removed
                    if (((Creeper)mobs[i]).GetNeedsToRemovePts())
                    {
                        //Reduce the total points and set the status as no longer requiring the removal of points 
                        totPoints = Math.Max(0, totPoints - Creeper.EXPL_DMG);
                        ((Creeper)mobs[i]).SetNeedsToRemovePts(false);
                    }
                }
                else if (mobs[i] is Skeleton)
                {
                    //Shoot an arrow from the skeleton if it is ready to shoot
                    if (((Skeleton)mobs[i]).GetReadyToShoot())
                    {
                        //Add a new arrow into the list of arrows from mobs
                        mobArrows.Add(((Skeleton)mobs[i]).ShootArrow());
                    }
                }

                //Remove the mob if it is out or dead
                if (mobs[i].GetState() == Mob.OUT || mobs[i].GetState() == Mob.DEAD)
                {
                    //Remove the mob
                    mobs.Remove(mobs[i]);
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch and font is a SpriteFont used to show the score in the gameplay
        //Post: none
        //Desc: Draws the gameplay
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            //Draw the player box and the player
            spriteBatch.Draw(playerBoxImg, playerBoxRec, Color.Black * 0.5f);
            player.Draw(spriteBatch);

            //Draw all the mobs currently on screen
            for (int i = 0; i < mobs.Count; i++)
            {
                //Draw each mob
                mobs[i].Draw(spriteBatch);
            }

            //Draw all the player arrows
            for (int i = 0; i < playerArrows.Count; i++)
            {
                //Draw the arrow
                playerArrows[i].Draw(spriteBatch);
            }

            //Draw all the mob arrows
            for (int i = 0; i < mobArrows.Count; i++)
            {
                //Draw the arrow
                mobArrows[i].Draw(spriteBatch);
            }

            //Display the current total points 
            spriteBatch.DrawString(font, "Score: " + totPoints, scoreMsgLoc, Color.White);
            
            //Draw the buff icons
            spriteBatch.Draw(speedBuffIcon, speedIconRec, Color.White * ((Convert.ToInt32(buffsOn[SPEED]) + 1) * 0.5f));
            spriteBatch.Draw(damageBuffIcon, damageIconRec, Color.White * ((Convert.ToInt32(buffsOn[DAMAGE]) + 1) * 0.5f));
            spriteBatch.Draw(fireRateBuffIcon, fireRateIconRec, Color.White * ((Convert.ToInt32(buffsOn[FIRE]) + 1) * 0.5f));
            spriteBatch.Draw(pointsBuffIcon, pointsIconRec, Color.White * ((Convert.ToInt32(buffsOn[POINTS]) + 1) * 0.5f));
        }
    }
}
