//Author: Victoria Mak
//File Name: Mob.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Mob class, which is the parent of the child mob classes

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
    class Mob
    {
        //Store the points and damage multiplier
        protected const int POINTS_MULTIPLIER = 2;
        public const int DAMAGE_MULTIPLIER = 3;

        //Store the mob states
        public const int ALIVE = 0;
        protected const int DYING = 1;
        public const int OUT = 2;
        public const int DEAD = 3;

        //Store the health and points of the mob
        protected int health;
        protected int awardPts;

        //Store the image and rectangle of the mob
        protected Texture2D mobImg;
        protected Rectangle mobRec;

        //Store the dying image and rectangle
        protected Texture2D dyingImg;
        protected Rectangle dyingRec;

        //Store the dying timer
        protected Timer dyingTimer;

        //Store the current location and speed
        protected Vector2 location;
        protected Vector2 maxSpeed;
        protected Vector2 speed;
        
        //Store the mob state
        protected int state;

        //Store the mob type
        protected int type;

        public Mob(Texture2D mobImg, Rectangle mobRec, Texture2D dyingImg, int health, int awardPts, Vector2 maxSpeed, int type)
        {
            //Set the mob image and rectangle
            this.mobImg = mobImg;
            this.mobRec = mobRec;

            //Set the mob location, speed, and max speed
            location = mobRec.Location.ToVector2();
            this.maxSpeed = maxSpeed;
            speed = new Vector2();

            //Set the dying image
            this.dyingImg = dyingImg;
            dyingRec = new Rectangle(0, 0, dyingImg.Width, dyingImg.Height);

            //Set the dying timer
            dyingTimer = new Timer(500f, false);

            //Set the health and points
            this.health = health;
            this.awardPts = awardPts;
            
            //Set the state to alive
            state = ALIVE;

            //Set the type
            this.type = type;
        }

        //Pre: none
        //Post: returns the rectangle of the mob
        //Desc: gets the mob's rectangle
        public Rectangle GetRectangle()
        {
            //Return the rectangle of the mob
            return mobRec;
        }

        //Pre: none
        //Post: returns the type of the mob
        //Desc: gets the mob's type
        public int GetTypeNum()
        {
            //Return the type
            return type;
        }

        //Pre: none
        //Post: returns the state of the mob
        //Desc: gets the mob's state
        public int GetState()
        {
            //Return the state
            return state;
        }

        //Pre: none
        //Post: returns the award points of killing the mob
        //Desc: gets the mob's award points
        protected int GetAwardPts()
        {
            //Return the award points
            return awardPts;
        }

        //Pre: none
        //Post: sets the mob's rectangle to the location 
        //Desc: sets the mob's rectangle
        protected void SetRectLocation()
        {
            //Set the rectangle location
            mobRec.X = (int)location.X;
            mobRec.Y = (int)location.Y;
        }

        //Pre: gameTime is a GameTime and has the elapsed game time between updates as a positive number
        //Post: none
        //Desc: updates the mob based on their state
        public void Update(GameTime gameTime)
        {
            //Update the mob depending its state
            switch (state)
            {
                case ALIVE:
                    //Update the alive state
                    UpdateAlive(gameTime);
                    break;

                case DYING:
                    //Update the dying timer
                    UpdateDyingTimer(gameTime);
                    break;

                case DEAD:
                    break;

                case OUT:
                    break;
            }
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the speed of the mob
        protected virtual void UpdateSpeed(GameTime gameTime)
        {
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected virtual void UpdateAlive(GameTime gameTime)
        {
        }
        
        //Pre: none
        //Post: none
        //Desc: updates the location of the mob based on its speed
        protected void Move()
        {
            //Set the location of the mob
            location.X += speed.X;
            location.Y += speed.Y;

            //Set the rectangle location of the mob
            SetRectLocation();

            //Check if the mob is off screen
            CheckOffScreen();
        }

        //Pre: none
        //Post: none
        //Desc: checks whether the mob is off screen and update its state
        private void CheckOffScreen()
        {
            //Check if off screen
            if (mobRec.Left > Game1.SCREEN_WIDTH || mobRec.Right < 0 || mobRec.Bottom > Game1.SCREEN_HEIGHT)
            {
                //Set the state as out
                state = OUT;
            }
        }

        //Pre: damageBuffOn and pointsBuffOn are bools of whether the buffs are on
        //Post: returns the award points for killing the mob if the mob is killed
        //Desc: reduces health and return points to award the player
        public virtual int ReduceHealthAndReturnPoints(bool damageBuffOn, bool pointsBuffOn)
        {
            //Reduce the health by the damage multiplier or the start damage
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

                //Return the number of points awarded for the kill based on whether the points multiplier is on
                return Math.Max(1, Convert.ToInt16(pointsBuffOn) * POINTS_MULTIPLIER) * GetAwardPts();
            }

            //Return no points rewarded
            return 0;
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: draws the mob
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw the mob depending on if the mob is alive or dying
            if (state == ALIVE)
            {
                //Draw the mob
                spriteBatch.Draw(mobImg, mobRec, Color.White);
            }
            else
            {
                //Draw its dying image
                spriteBatch.Draw(dyingImg, dyingRec, Color.White);
            }
        }

        //Pre: gameTime has the elapsed game time of between updates 
        //Post: none
        //Desc: updates the mob's dying timer and sets the state when the mob is dead
        private void UpdateDyingTimer(GameTime gameTime)
        {
            //Update the dying timer
            dyingTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Set the state of the mob as dead if the dying timer is finished
            if (dyingTimer.IsFinished())
            {
                //Set the mob's state to dead
                state = DEAD;
            }
        }
    }
}
