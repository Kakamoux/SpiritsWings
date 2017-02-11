using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace serveur
{
   

    public class Bullet
    {
        public Vector3 position;
        public Quaternion rotation;
        public double bulletStart;

        public Bullet(double bulletStart, Vector3 position, Quaternion rotation)
        {
            this.bulletStart = bulletStart;
            this.position = position;
            this.rotation = rotation;
        }
        
        


    }
}
