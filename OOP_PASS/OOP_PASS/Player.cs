//Author: Victoria Mak
//File Name: Player.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Player class for the player in the game

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
    class Player
    {
        //Store the start speed in pixels per second and the fire rate in shots per second, as well as their multipliers
        private const int START_SPEED = 3;
        private const float START_FIRE_RATE = 3f;
        private const int SPEED_MULTIPLIER = 2;
        private const int FIRE_RATE_MULTIPLIER = 2;

        //Store the current speed in pixels per second
        private int curSpeed;

        //Store the current fire rate and the shoot timer
        private float fireRate;
        private Timer shootTimer;

        //Store player image and rectangle
        private Texture2D playerImg;
        private Rectangle playerRec;

        //Store the arrow image, sounds and whether the player is ready to shoot
        private Texture2D playerArrowImg;
        private SoundEffect bowShootSnd;
        private SoundEffect arrowImpactSnd;
        private bool readyToShoot;

        //Store whether or not the player is scared
        private bool isScared;

        public Player(Texture2D playerImg, Texture2D playerArrowImg, SoundEffect bowShootSnd, SoundEffect arrowImpactSnd)
        {
            //Set the player and arrow image, bow shoot sound, and the player rectangle
            this.playerImg = playerImg;
            this.playerArrowImg = playerArrowImg;
            this.bowShootSnd = bowShootSnd;
            this.arrowImpactSnd = arrowImpactSnd;
            playerRec = new Rectangle((Game1.SCREEN_WIDTH - playerImg.Width) / 2, Game1.SCREEN_HEIGHT - playerImg.Height, playerImg.Width, playerImg.Height);
            
            //Set starting speed and fire rate
            curSpeed = START_SPEED;
            fireRate = START_FIRE_RATE;

            //Set the shoot timer as a new timer and the player as ready to shoot
            shootTimer = new Timer(1000 / fireRate, false);
            readyToShoot = true;
            
            //Set the player as not scared
            isScared = false;
        }

        //Pre: none
        //Post: Returns the player rectangle
        //Desc: Gets the rectangle of the player
        public Rectangle GetRectangle()
        {
            //Return the player's rectangle
            return playerRec;
        }

        //Pre: none
        //Post: returns a bool for whether the player is ready to shoot
        //Desc: Gets whether the player is ready to shoot
        public bool GetReadyToShoot()
        {
            //Return the state for whether the player is ready to shoot
            return readyToShoot;
        }

        //Pre: speedBuffOn is a bool for whether the speed buff is on
        //Post: none
        //Desc: Sets the current speed of the player based on whether or not they have the speed buff
        public void SetCurSpeed(bool speedBuffOn)
        {
            //Set the current speed based on the speed buff
            if (speedBuffOn)
            {
                //Set the current speed as having the multiplier
                curSpeed = START_SPEED * SPEED_MULTIPLIER;
            }
            else
            {
                //Set the current speed as the starting speed
                curSpeed = START_SPEED;
            }
        }

        //Pre: fireRateBuffOn is a bool for whether the fire rate buff is on
        //Post: none
        //Desc: Sets the current fire rate of the player based on whether or not they have the fire rate buff
        public void SetFireRate(bool fireRateBuffOn)
        {
            //Set the current fire rate based on the fire rate buff
            if (fireRateBuffOn)
            {
                //Set the current fire rate as having the multiplier
                fireRate = START_FIRE_RATE * FIRE_RATE_MULTIPLIER;
            }
            else
            {
                //Set the fire rate as the starting fire rate
                fireRate = START_FIRE_RATE;
            }

            //Set the shoot timer as a new timer with the new fire rate
            shootTimer = new Timer(1000 / fireRate, true);
        }

        //Pre: isScared is a bool describing whether the player is/is not scared
        //Post: none
        //Desc: Sets the player as scared or not scared
        public void SetIsScared(bool isScared)
        {
            //Set the player's scared state
            this.isScared = isScared;
        }

        //Pre: kb is the current KeyboardState with data on the keys pressed and gameTime is a GameTime with the elapsed time between updates
        //Post: none
        //Desc: Moves the player based on the left/right arrow keys pressed
        public void Move(KeyboardState kb, GameTime gameTime)
        {
            //Only move the player if they are not scared
            if (!isScared)
            {
                //Move the player left/right if they are pressing the corresponding arrow key
                if (kb.IsKeyDown(Keys.Left))
                {
                    //Set the player's rectangle 3 pixels to the left
                    playerRec.X = Math.Max(0, playerRec.X - curSpeed);
                }
                else if (kb.IsKeyDown(Keys.Right))
                {
                    //Set the player's rectangle 3 pixels to the right
                    playerRec.X = Math.Min(Game1.SCREEN_WIDTH - playerImg.Width, playerRec.X + curSpeed);
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: Resets the player to the center of the screen for a new level
        public void ResetPlayer(bool fireRateBuffOn, bool speedBuffOn)
        {
            //Set the player to the center of the screen horizontally
            playerRec.X = Game1.SCREEN_WIDTH / 2 - playerImg.Width / 2;

            //Set the player as ready to shoot and reset the shoot timer
            readyToShoot = true;
            shootTimer.ResetTimer(false);

            //Set the current speed and fire rate
            SetCurSpeed(speedBuffOn);
            SetFireRate(fireRateBuffOn);
        }

        //Pre: kb is the current KeyboardState with data on the keys pressed and gameTime is a GameTime with the elapsed time between updates
        //Post: none
        //Desc: Updates the player by updating the shoot timer and their movement
        public void Update(GameTime gameTime, KeyboardState kb)
        {
            //Update the shoot timer
            shootTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Set the player as ready to shoot if the timer is finished and if they are not scared
            if (shootTimer.IsFinished() && !isScared)
            {
                //Set the player as ready to shoot
                readyToShoot = true;
            }

            //Move the player according to the keyboard buttons pressed
            Move(kb, gameTime);
        }

        //Pre: none
        //Post: Returns an arrow that moves up
        //Desc: Shoots an arrow from the player by returning a new arrow to be added to the gameplay's list of arrows
        public Arrow ShootArrow()
        {
            //Reset the shoot timer and set the player as not ready to shoot and return an arrow
            shootTimer.ResetTimer(true);
            readyToShoot = false;
            return new Arrow(playerArrowImg, new Rectangle(playerRec.Center.X - playerArrowImg.Width / 2, playerRec.Y - playerArrowImg.Height, playerArrowImg.Width, playerArrowImg.Height), Arrow.UP, bowShootSnd, arrowImpactSnd);
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: Draws the player at its location
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the player
            spriteBatch.Draw(playerImg, playerRec, Color.White);
        }
    }
}
