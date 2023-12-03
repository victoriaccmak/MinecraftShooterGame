//Author: Victoria Mak
//File Name: Pillager.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Pillager class, which is a subclass of the Mob

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace OOP_PASS
{

    class Pillager : Mob
    {
        //Store the type number
        public const int TYPE = 3;

        //Store the cusp, number of periods occuring, wavelength in pixels, period in seconds, and the angular velocity
        public const int CUSP = 100;
        private const float NUM_PERIODS = 1.5f;
        private const int WAVELENGTH = (int)(Game1.SCREEN_WIDTH / NUM_PERIODS);
        private const float PERIOD = 5;
        private const float ANGLE_DECREASE_PER_SEC = (float)(2 * Math.PI / PERIOD);
        
        //Store the current angle and whether the pillager has the shield
        private float angle;
        private bool hasShield;

        //Store the shield image, rectangle, and hit sound
        private Texture2D shieldImg;
        private Rectangle shieldRec;
        private SoundEffect shieldHitSnd;

        public Pillager(Texture2D pillagerImg, Rectangle pillagerRec, Texture2D dyingImg, Texture2D shieldImg, SoundEffect shieldHitSnd) : base(pillagerImg, pillagerRec, dyingImg, 2, 25, new Vector2(WAVELENGTH / PERIOD, (float)(2 * Math.PI * CUSP / PERIOD)), TYPE)
        {
            //Set the initial angle and the pillager as having the shield
            angle = (float)(2 * Math.PI * NUM_PERIODS);
            hasShield = true;

            //Set the shield image, sound, and rectangle
            this.shieldImg = shieldImg;
            this.shieldHitSnd = shieldHitSnd;
            shieldRec = new Rectangle(mobRec.Right - shieldImg.Width, mobRec.Bottom - shieldImg.Height, shieldImg.Width, shieldImg.Height);
        }

        //Pre: None
        //Post: None
        //Desc: Sets the shield rectangle location relative to the mob location
        private void SetShieldRecLoc()
        {
            //Set the location of the shield rectangle
            shieldRec.X = mobRec.Right - shieldImg.Width;
            shieldRec.Y = mobRec.Bottom - shieldImg.Height;
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected override void UpdateAlive(GameTime gameTime)
        {
            //Update the speed, move the mob, and set the shield location
            UpdateSpeed(gameTime);
            Move();
            SetShieldRecLoc();
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the speed of the pillager
        protected override void UpdateSpeed(GameTime gameTime)
        {
            //Decrease the angle
            angle -= (float)(ANGLE_DECREASE_PER_SEC * gameTime.ElapsedGameTime.TotalSeconds);

            //Set the maximum speed in the y direction
            maxSpeed.Y = (float)(Math.Sin(angle) * 2 * Math.PI * CUSP / PERIOD);

            //Set the current x and y speed
            speed.Y = (float)(maxSpeed.Y * gameTime.ElapsedGameTime.TotalSeconds);
            speed.X = (float)(maxSpeed.X * gameTime.ElapsedGameTime.TotalSeconds);
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: None
        //Desc: Draws the pillager depdending on its state
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the pillager depending on whether it is alive or dying
            if (state == ALIVE)
            {
                //Draw the pillager
                spriteBatch.Draw(mobImg, mobRec, Color.White);

                //Draw the shield if the pillager has the shield
                if (hasShield)
                {
                    //Draw the shield
                    spriteBatch.Draw(shieldImg, shieldRec, Color.White);
                }
            }
            else
            {
                //Draw the dying image
                spriteBatch.Draw(dyingImg, dyingRec, Color.White);
            }
        }

        //Pre: damageBuffOn and pointsBuffOn are bools of whether those buffs are on
        //Post: Returns the points rewarded depending on if the mob is killed
        //Desc: Reduces the health of the pillager and returns the points awarded
        public override int ReduceHealthAndReturnPoints(bool damageBuffOn, bool pointsBuffOn)
        {
            //Remove the shield if the pillager has a shield
            if (hasShield)
            {
                //Set the pillager to not having the shield and play the hit sound
                hasShield = false;
                shieldHitSnd.CreateInstance().Play();
            }
            else
            {
                //Reduce the health by the amount for the damage buff on or off
                health -= Math.Max(1, Convert.ToInt32(damageBuffOn) * DAMAGE_MULTIPLIER) * Arrow.MOB_DAMAGE;

                //Change the state of the pillager if it has 0 health
                if (health <= 0)
                {
                    //Set the stae to dying
                    state = DYING;

                    //Active the dying timer
                    dyingTimer.Activate();

                    //Set the location of the dying rectangle
                    dyingRec.X = mobRec.Center.X - dyingRec.Width / 2;
                    dyingRec.Y = mobRec.Center.Y - dyingRec.Height / 2;

                    //Return the amount of points awarded for the kill
                    return Math.Max(1, Convert.ToInt32(pointsBuffOn) * POINTS_MULTIPLIER) * GetAwardPts();
                }
            }

            //Return no points since the pillager is still alive
            return 0;
        }
    }
}
