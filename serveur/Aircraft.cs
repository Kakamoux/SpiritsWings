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
    class Aircraft
    {
        public List<Bullet> bulletList = new List<Bullet>();
        public Vector3 xwingPosition = new Vector3(40, 40, 40);
        public Quaternion xwingRotation = Quaternion.Identity;
        public IServeurRappel callback = null;
        public bool killed = false;
        public int score = 0;

        public Aircraft(IServeurRappel callback)
        {
            this.callback = callback;
        }
    }
}
