//Author: Victoria Mak
//File Name: Enderman.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Enderman class, which is a subclass of the Mob

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
    class Enderman : Mob
    {
        //Store the type and the spawn odds
        public const int TYPE = 4;

        //Store the size for the teleporting image
        private const int TELEPORT_IMG_SIDE = Game1.BLOCK_SIDE * 8;

        //Store the alive states
        private const int TELEPORTING = 0;
        private const int STOPPED = 1;

        //Store the number of teleporting stops
        private const int NUM_TELE_STOPS = 5;

        //Store the constant teleporting locations
        private readonly Vector2[] TELEPORT_LOCS = new Vector2[4]{ new Vector2(0, 0), new Vector2(300, 60), new Vector2(80, 400), new Vector2(600, 105) };

        //Store the constant times for teleporting and for stopping
        private const float STAY_TIME = 3000f;
        private const float TELE_TIME = 500f;

        //Store the current alive state and the teleporting stage
        private int aliveState;
        private int teleportStage;

        //Store the stop and teleporting timers
        private Timer stopTimer;
        private Timer teleTimer;

        //Store the random teleporting places left
        List<int> placesLeft;
        
        //Store the player
        Player player;

        //Store the sound effects
        private SoundEffect endermanScreamSnd;
        private SoundEffect endermanTeleportSnd;

        //Store the rectangle for the teleporting image
        Rectangle teleportRec = new Rectangle((Game1.SCREEN_WIDTH - TELEPORT_IMG_SIDE) / 2, (Game1.SCREEN_HEIGHT - TELEPORT_IMG_SIDE) / 2, TELEPORT_IMG_SIDE, TELEPORT_IMG_SIDE);

        public Enderman(Texture2D endermanImg, Texture2D dyingImg, Player player, SoundEffect endermanScreamSnd, SoundEffect endermanTeleportSnd) : base(endermanImg, new Rectangle(-100, -100, endermanImg.Width, endermanImg.Height), dyingImg, 5, 100, new Vector2(), TYPE)
        {
            //Set the player
            this.player = player;

            //Set the sound effects
            this.endermanScreamSnd = endermanScreamSnd;
            this.endermanTeleportSnd = endermanTeleportSnd;

            //Set the alive state to teleporting
            aliveState = TELEPORTING;

            //Set the stop timer and the teleporting timer
            stopTimer = new Timer(STAY_TIME, false);
            teleTimer = new Timer(TELE_TIME, true);

            //Set the teleporting stage to 0
            teleportStage = 0;

            //Set the places left list
            placesLeft = new List<int>{ 1, 2, 3 };
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected override void UpdateAlive(GameTime gameTime)
        {
            //Update the enderman when it is alive depending on whether it stopped or is teleporting
            switch (aliveState)
            {
                case TELEPORTING:
                    //Update the teleporting timer
                    teleTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //If the teleporting timer is finished change its state to stopped or out
                    if (teleTimer.IsFinished())
                    {
                        //Set the player as not scared
                        player.SetIsScared(false);

                        //If the teleporting stage is greater than the number of stops, set the enderman as out
                        if (teleportStage >= NUM_TELE_STOPS)
                        {
                            //Set the state as out
                            state = OUT;
                        }
                        else
                        {
                            //Teleport the enderman, change its state to stopped, and reset its stop timer
                            Teleport();
                            aliveState = STOPPED;
                            stopTimer.ResetTimer(true);
                        }
                    }
                    break;

                case STOPPED:
                    //Update the stop timer
                    stopTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //If the stop timer is finished, set the state of the enderman to teleporting
                    if (stopTimer.IsFinished())
                    {
                        //Reset the teleporting timer, set the state to teleporting, increase the stage number, set the mob off screen, and set the player as scared
                        teleTimer.ResetTimer(true);
                        aliveState = TELEPORTING;
                        teleportStage++;
                        mobRec.X = -100;
                        player.SetIsScared(true);
                        
                        //Play the teleport sound
                        endermanTeleportSnd.CreateInstance().Play();
                    }
                    break;
            }
        }

        //Pre: none
        //Post: none
        //Desc: sets the location of the enderman that it teleports to
        private void Teleport()
        {
            //Store the temporary randommized location number
            int teleportLocNum;

            //Teleport according to the stage number
            if (teleportStage == 0)
            {
                //Set the location as the first teleport location
                location = TELEPORT_LOCS[0];
            }
            else if (teleportStage < NUM_TELE_STOPS - 1)
            {
                //Randomize the teleport location number, set the location corresponding to the number from the places left, and remove that location from places left to teleport to
                teleportLocNum = new Random().Next(0, 4 - teleportStage);               
                location = TELEPORT_LOCS[placesLeft[teleportLocNum]];
                placesLeft.RemoveAt(teleportLocNum);
            }
            else
            {
                //Set the location of the enderman directly in front of the player
                location.X = player.GetRectangle().X;
                location.Y = player.GetRectangle().Y - mobImg.Height;

                //Play the scream sound
                endermanScreamSnd.CreateInstance().Play();
            }

            //Set the rectangle location
            SetRectLocation();
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: draws the enderman
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the enderman depending on whether it is alive or dying
            if (state == ALIVE)
            {
                //Draw the enderman depending on whether it is teleporting or stopped
                if (aliveState == TELEPORTING)
                {
                    //Draw the enderman as teleporting
                    spriteBatch.Draw(mobImg, teleportRec, Color.White * 0.5f);
                }
                else
                {
                    //Draw the enderman
                    spriteBatch.Draw(mobImg, mobRec, Color.White);
                }
            }
            else if (state == DYING)
            {
                //Draw the dying image
                spriteBatch.Draw(dyingImg, dyingRec, Color.White);
            }
        }
    }
}
