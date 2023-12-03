//Author: Victoria Mak
//File Name: Villager.cs
//Project Name: OOP_PASS
//Creation Date: March 24, 2023
//Modified Date: April 12, 2023
//Description: Define the Villager class, which is a subclass of the Mob

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
    class Villager : Mob
    {
        //Store the villager type number
        public const int TYPE = 0;

        public Villager(Texture2D villagerImg, Texture2D dyingImg, Rectangle villagerRec) : base(villagerImg, villagerRec, dyingImg, 1, 10, new Vector2(250f, 0), TYPE)
        {
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the speed of the pillager based on the maximum speed
        protected override void UpdateSpeed(GameTime gameTime)
        {
            //Update the speed of the villager
            speed.X = (float)(maxSpeed.X * gameTime.ElapsedGameTime.TotalSeconds);
        }

        //Pre: gameTime is a GameTime that has an elapsed game time between updates
        //Post: None
        //Desc: Updates the alive state of the mob
        protected override void UpdateAlive(GameTime gameTime)
        {
            //Update the speed and move the mob
            UpdateSpeed(gameTime);
            Move();
        }
    }
}
