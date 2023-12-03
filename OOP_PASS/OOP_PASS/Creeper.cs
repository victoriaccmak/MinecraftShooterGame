//Author: Victoria Mak
//File Name: Creeper.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Creeper class, which is a subclass of the Mob

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
    class Creeper : Mob
    {
        //Store the type number
        public const int TYPE = 1;

        //Store the explosion radius, damage to player, and maximum speed
        private const int EXPL_RADIUS = 100;
        public const int EXPL_DMG = 40;
        private const float MAX_SPEED = 200f;

        //Store the x and y distance from player and the angle
        private Vector2 distToPlayer;
        private float angle;

        //Store whether the points need to removed from the player
        private bool needsToRemovePts;

        //Store the player
        Player player;

        //Store the explosion sound
        SoundEffect explodeSnd;

        public Creeper(Texture2D creeperImg, Texture2D dyingImg, Rectangle creeperRec, Player player, SoundEffect explodeSnd) : base(creeperImg, creeperRec, dyingImg, 3, 40, new Vector2(0, MAX_SPEED), TYPE)
        {
            //Set the player, the explosion sound, and the distance to the player
            this.player = player;
            this.explodeSnd = explodeSnd;
            distToPlayer = new Vector2(creeperRec.X - player.GetRectangle().X, Game1.SCREEN_HEIGHT - player.GetRectangle().Height);

            //Set the creeper as not having to remove points
            needsToRemovePts = false;
        }

        //Pre: none
        //Post: Returns a bool for whether points need to be removed
        //Desc: Gets whether the points from the explosion damage needs to be removed from the player's score
        public bool GetNeedsToRemovePts()
        {
            //Return the bool for whether points need to be removed
            return needsToRemovePts;
        }

        //Pre: needsToRemovePts is a bool describing whether points need to be removed
        //Post: none
        //Desc: Sets whether points need to be removed from the player's score
        public void SetNeedsToRemovePts(bool needsToRemovePts)
        {
            //Set the status for whether points need to be removed
            this.needsToRemovePts = needsToRemovePts;
        }
        
        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected override void UpdateAlive(GameTime gameTime)
        {
            //Update speed and move the mob
            UpdateSpeed(gameTime);
            Move();
        }

        //Pre: gameTime is a GameTime and has the elapsed game time between updates as a positive number
        //Post: none
        //Desc: Updates the speed based on the player's location
        protected override void UpdateSpeed(GameTime gameTime)
        {
            //Explode the creeper when it hits the bottom of the screen or the player
            if (mobRec.Intersects(player.GetRectangle()) || mobRec.Bottom >= Game1.SCREEN_HEIGHT)
            {
                //Explode the creeper
                Explode();
            }
            else
            {
                //Set the x and y distance to player
                distToPlayer.X = player.GetRectangle().Center.X - mobRec.Center.X;
                distToPlayer.Y = player.GetRectangle().Center.Y - mobRec.Center.Y;

                //Set the angle of the path that the creeper has to move
                angle = (float)Math.Atan(distToPlayer.X / distToPlayer.Y);

                //Set the maximum speed for the x and y direction
                maxSpeed.X = (float)(MAX_SPEED * Math.Sin(angle));
                maxSpeed.Y = (float)(MAX_SPEED * Math.Cos(angle));

                //Set the current speed based on the max speed
                speed.X = (float)(maxSpeed.X * gameTime.ElapsedGameTime.TotalSeconds);
                speed.Y = (float)(maxSpeed.Y * gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        //Pre: damageBuffOn and pointsBuffOn are both bools that describe whether those buffs are active
        //Post: Returns an int for the amount of points awarded if the mob is killed
        //Desc: Reduces the health of the mob and returns the amount of points awarded for killing the mob
        public override int ReduceHealthAndReturnPoints(bool damageBuffOn, bool pointsBuffOn)
        {
            //Reduce the health
            health -= Math.Max(1, Convert.ToInt32(damageBuffOn) * DAMAGE_MULTIPLIER) * Arrow.MOB_DAMAGE;

            //Change the state of the pillager if it has 0 health
            if (health <= 0)
            {
                //Explode the creeper
                Explode();

                //Return the amount of points awarded
                return Math.Max(1, Convert.ToInt32(pointsBuffOn) * POINTS_MULTIPLIER) * GetAwardPts();
            }

            //Return no points as the creeper is still alive
            return 0;
        }

        //Pre: none
        //Post: none
        //Desc: Explodes the creeper by changing the state to dying and checking for whether the player is in the explosion radius
        private void Explode()
        {
            //Store the distance from the player to the creeper
            double playerCreeperDist = Math.Sqrt(Math.Pow(distToPlayer.X, 2) + Math.Pow(distToPlayer.Y, 2));
            
            //Change the state to dying and activate the dying timer
            state = DYING;
            dyingTimer.Activate();

            //Set the location of the dying rectangle
            dyingRec.X = mobRec.Center.X - dyingRec.Width / 2;
            dyingRec.Y = mobRec.Center.Y - dyingRec.Height / 2;

            //Determine whether points need to be removed by comparing the explosion radius to the distance
            if (playerCreeperDist < EXPL_RADIUS)
            {
                //Set points as needs to be removed
                needsToRemovePts = true;
            }

            //Play the explosion sound
            explodeSnd.CreateInstance().Play();
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: Draws the creeper based on its state
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (state == ALIVE)
            {
                spriteBatch.Draw(mobImg, mobRec, Color.White);
            }
            else
            {
                spriteBatch.Draw(dyingImg, dyingRec, Color.White);
            }
        }
    }
}
