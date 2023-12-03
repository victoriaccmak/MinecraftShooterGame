//Author: Victoria Mak
//File Name: Arrow.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Arrow class for the arrows in the game

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
    class Arrow
    {
        //Store the constant values for up and down
        public const int UP = -1;
        public const int DOWN = 1;

        //Store the constant speed of the arrow in pixels per second and the damage to player and mob
        private const float MAX_SPEED = 480f;
        public const int DAMAGE_TO_PLAYER = 20;
        public const int MOB_DAMAGE = 1;

        //Store the y direction, current y speed, and location
        private int yDir;
        private float ySpeed;
        private Vector2 location;

        //Store the arrow image, rectangle and sound
        private Texture2D arrowImg;
        private Rectangle arrowRec;
        private SoundEffect arrowImpactSnd;

        public Arrow(Texture2D arrowImg, Rectangle arrowRec, int yDir, SoundEffect bowShootSnd, SoundEffect arrowImpactSnd)
        {
            //Set the arrow image, rectangle, sound and direction
            this.arrowImg = arrowImg;
            this.arrowRec = arrowRec;
            this.arrowImpactSnd = arrowImpactSnd; 
            this.yDir = yDir;

            //Set the location of the arrow based on the location of the rectangle
            location = new Vector2(arrowRec.X, arrowRec.Y);

            //Play the bow shoot sound
            bowShootSnd.CreateInstance().Play();
        }

        //Pre: none
        //Post: returns a bool of whether the arrow is off screen or not
        //Desc: Determines whether the arrow is off the screen to be removed
        public bool GetOffScreen()
        {
            //Return whether or not the arrow is of screen
            return arrowRec.Bottom < 0 || arrowRec.Y > Game1.SCREEN_HEIGHT;
        }

        //Pre: gameTime is a GameTime and has the elapsed game time between updates
        //Post: none
        //Desc: Moves the arrow by update the speed, location, and arrow rectangle
        public void Move(GameTime gameTime)
        {
            //Set the current y speed, increase the location by the speed, and set the arrow rectangle to the location
            ySpeed = yDir * (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            location.Y += ySpeed;
            arrowRec.Y = (int)location.Y;
        }

        //Pre: player is a Player in the game
        //Post: Returns a bool for whether the arrow has collided
        //Desc: Check whether or not the arrow has collided with the player
        public bool CheckPlayerCollision(Player player)
        {
            //Return whether or not the player rectangle intersects the arrow rectangle
            return player.GetRectangle().Intersects(arrowRec);
        }

        //Pre: mob is a subclass of Mob and has the state as alive
        //Post: Returns a bool for whether the arrow has collided
        //Desc: Check whether or not the arrow has collided with the player
        public bool CheckMobCollision(Mob mob)
        {
            //Return whether or not the mob rectangle intersects the arrow rectangle
            return mob.GetRectangle().Intersects(arrowRec);
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: Draws the arrow with a specific color depending on its direction
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the arrow depending on its direction
            if (yDir == DOWN)
            {
                //Draw the arrow as red
                spriteBatch.Draw(arrowImg, arrowRec, Color.Red);
            }
            else
            {
                //Draw the arrow as blue
                spriteBatch.Draw(arrowImg, arrowRec, Color.Blue);
            }
        }

        //Pre: none
        //Post: none
        //Desc: Plays the arrow impact sound
        public void PlayArrowImpactSnd()
        {
            //Play the arrow impact sound
            arrowImpactSnd.CreateInstance().Play();
        }
    }
}
