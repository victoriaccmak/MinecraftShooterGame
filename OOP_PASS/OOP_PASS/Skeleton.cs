//Author: Victoria Mak
//File Name: Skeleton.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Skeleton class, which is a subclass of the Mob

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
    class Skeleton : Mob
    {
        //Store the type
        public const int TYPE = 2;

        //Store the starting radius in pixels and the number of rotations
        private const int START_RADIUS = (Game1.SCREEN_HEIGHT - 2 * Game1.BLOCK_SIDE) / 2;
        private const int NUM_ROTATIONS = 4;

        //Store the period, total time spiraling in seconds, and the angular velocity
        private const float PERIOD = 3;
        private const float TOT_TIME_SPIRALING = PERIOD * NUM_ROTATIONS;
        private const float ANGLE_DECREASE_PER_SEC = (float)(2 * Math.PI / PERIOD);
        
        //Store the maximum decrease in radius in pixels per second
        private const float MAX_RAD_DECREASE_PER_SEC = START_RADIUS / TOT_TIME_SPIRALING;
        
        //Store the maximum speed for going straight
        private const float MAX_STRAIGHT_SPEED = 150f;

        //Store the states of the skeleton's movements
        private const int MOVING_DOWN = 0;
        private const int SPIRALING = 1;
        private const int MOVING_RIGHT = 2;

        //Store the fire rate in arrows per second and store the shoot timer
        private float fireRate = 1.5f;
        private Timer shootTimer;

        //Store the mob arrow image and sounds
        Texture2D mobArrowImg;
        SoundEffect bowShootSnd;
        SoundEffect arrowImpactSnd;

        //Store the current movement state
        private int mvmtState;

        //Store the radius, angle, and current maximum spiral speed
        private float radius;
        private float angle;
        private float curMaxSpiralSpeed;

        //Store whether another arrow is ready to shoot
        private bool readyToShoot;
        
        public Skeleton(Texture2D skeletonImg, Texture2D mobArrowImg, Texture2D dyingImg, SoundEffect bowShootSnd, SoundEffect arrowImpactSnd) : base(skeletonImg, new Rectangle(Game1.SCREEN_WIDTH / 2 + START_RADIUS - skeletonImg.Width / 2, -skeletonImg.Height, skeletonImg.Width, skeletonImg.Height), dyingImg, 4, 25, new Vector2(0, MAX_STRAIGHT_SPEED), TYPE)
        {
            //Set the mob arrow image and arrow sounds
            this.mobArrowImg = mobArrowImg;
            this.bowShootSnd = bowShootSnd;
            this.arrowImpactSnd = arrowImpactSnd;

            //Set the movement state as moving down and activate the shoot timer
            mvmtState = MOVING_DOWN;
            shootTimer = new Timer(1000 / fireRate, true);

            //Set the starting radius and angle
            radius = START_RADIUS;
            angle = (float)(NUM_ROTATIONS * 2 * Math.PI);

            //Set the pillager as unready to shoot
            readyToShoot = false;
        }

        //Pre: None
        //Post: Returns a bool of whether the skeleton is ready to shoot
        //Desc: Returns whether the mob is ready to shoot
        public bool GetReadyToShoot()
        {
            //Return whether it is ready to shoot
            return readyToShoot;
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected override void UpdateAlive(GameTime gameTime)
        {
            //Update the shoot timer, the speed, and move the skeleton
            UpdateShootTimer(gameTime);
            UpdateSpeed(gameTime);
            Move();
        }

        //Pre: gameTime is a GameTime and has the elapsed game time between updates as a positive number
        //Post: none
        //Desc: updates the mob based on their state
        protected override void UpdateSpeed(GameTime gameTime)
        {
            //Update the speed according to its movement state
            switch(mvmtState)
            {
                case MOVING_DOWN:
                    //Update the y direction speed
                    speed.Y = (float)(MAX_STRAIGHT_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    
                    //Change the state to spiraling once the mob reached the vertical midpoint of the spiral
                    if (location.Y >= START_RADIUS)
                    {
                        //Set the state to spiraling
                        mvmtState = SPIRALING;
                    }

                    break;

                case SPIRALING:
                    //Reduce the radius and the angle
                    radius -= (float)(MAX_RAD_DECREASE_PER_SEC * gameTime.ElapsedGameTime.TotalSeconds);
                    angle -= (float)(ANGLE_DECREASE_PER_SEC * gameTime.ElapsedGameTime.TotalSeconds);

                    //Set the current maximum spiral speed
                    curMaxSpiralSpeed = ANGLE_DECREASE_PER_SEC * radius;

                    //Set the maximum speed in the y and x direction
                    maxSpeed.Y = (float)Math.Cos(angle) * curMaxSpiralSpeed;
                    maxSpeed.X = (float)Math.Sin(angle) * curMaxSpiralSpeed;

                    //Set the current speed in the x and y direction
                    speed.X = (float)(maxSpeed.X * gameTime.ElapsedGameTime.TotalSeconds);
                    speed.Y = (float)(maxSpeed.Y * gameTime.ElapsedGameTime.TotalSeconds);

                    //Set the state to moving right when the radius reaches 0
                    if (radius <= 0)
                    {
                        //Set the y direction speed to 0 and change the state
                        speed.Y = 0;
                        mvmtState = MOVING_RIGHT;
                    }
                    
                    break;

                case MOVING_RIGHT:
                    //Update the horizontal speed
                    speed.X = (float)(MAX_STRAIGHT_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    break;
            }
        }

        //Pre: none
        //Post: returns an arrow
        //Desc: Shoots an arrow from the skeleton by returning an arrow to the level class
        public Arrow ShootArrow()
        {
            //Set the mob not ready to shoot and reset the shoot timer
            readyToShoot = false;
            shootTimer.ResetTimer(true);

            //Return a new arrow below the skeleton
            return new Arrow(mobArrowImg, new Rectangle(mobRec.Center.X, mobRec.Bottom, mobArrowImg.Width, mobArrowImg.Height), Arrow.DOWN, bowShootSnd, arrowImpactSnd);
        }

        //Pre: gameTime has an elapsed time of the time passed between updates
        //Post: none
        //Desc: Updates the shoot timer
        private void UpdateShootTimer(GameTime gameTime)
        {
            //Update the shoot timer
            shootTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Set the skeleton as ready to shoot if the shoot timer is finished
            if (shootTimer.IsFinished())
            {
                //Set the skeleton as ready to shoot
                readyToShoot = true;
            }
        }
    }
}
