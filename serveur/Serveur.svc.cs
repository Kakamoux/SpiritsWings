using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;


namespace serveur
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Single)
     ]
    public class Serveur : IServeur
    {
        private IServeurRappel callback = null;
        internal static Hashtable listeAircrafts = new Hashtable();
        internal static List<bulletReturn> listeBullets = new List<bulletReturn>();
        private String pseudo;


        public void DemarrerSession(String pseudo)
        {

            callback = OperationContext.Current.GetCallbackChannel<IServeurRappel>();
            
                if (!listeAircrafts.Contains(pseudo))
                {
                    this.pseudo = pseudo;
                    listeAircrafts.Add(pseudo, new Aircraft(callback));
                    OperationContext.Current.Channel.Closing += new EventHandler(Channel_Closing); 

                }
            
        }

        private void Channel_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Fermeture du channel de l'utilisateur {0}", this.pseudo);
            //si on n'est pas passé par FermerSession avant
            lock (listeAircrafts)
            {

                if (listeAircrafts.Contains(this.pseudo))
                    listeAircrafts.Remove(this.pseudo);
            }
        }

        public Vector3 move(Vector3 xwingPosition, Quaternion xwingRotation, float moveSpeed)
        {
            Vector3 addVector = (Vector3.Transform(new Vector3(0, 0, -1), xwingRotation)) * moveSpeed;
            //  Console.WriteLine(addVector.ToString());
            return addVector;
        }

        public void sychronizeRotation(Quaternion xwingRotation, String pseudo)
        {
            ((Aircraft)listeAircrafts[pseudo]).xwingRotation = xwingRotation;
        }
        public void sychronizePosition(Vector3 xwingPosition, String pseudo)
        {
            ((Aircraft)listeAircrafts[pseudo]).xwingPosition = xwingPosition;
        }

        public void newBullet(double p, Vector3 xwingPosition, Quaternion xwingRotation, string pseudo)
        {
           
            ((Aircraft)listeAircrafts[pseudo]).bulletList.Add(new Bullet(p, xwingPosition, xwingRotation));
        }

        public void removeBullet(int i, String pseudo)
        {
            ((Aircraft)listeAircrafts[pseudo]).bulletList.RemoveAt(i);
        }

        public void updateBullet(Vector3 vector3, Quaternion quaternion, string pseudo, int i)
        {
            ((Aircraft)listeAircrafts[pseudo]).bulletList[i].position = vector3;
            ((Aircraft)listeAircrafts[pseudo]).bulletList[i].rotation = quaternion;
        }

        public List<bulletReturn> getBullets(String pseudo)
        {
            List<bulletReturn> bullets = new List<bulletReturn>();
            foreach (String aircraftPlayer in listeAircrafts.Keys)
            {

                for (int i = 0; i < ((Aircraft)listeAircrafts[aircraftPlayer]).bulletList.Count(); i++)
                {
                    bulletReturn bullet = new bulletReturn();
                    bullet.position = ((Aircraft)listeAircrafts[aircraftPlayer]).bulletList[i].position;
                    bullet.rotation = ((Aircraft)listeAircrafts[aircraftPlayer]).bulletList[i].rotation;
                    bullet.pseudo = aircraftPlayer;

                    bullets.Add(bullet);
                }

            }

            return bullets;
        }

        public List<aircraftReturn> getAircrafts(String pseudo)
        {
            List<aircraftReturn> airs = new List<aircraftReturn>();


           
                foreach (String aircraftPlayer in listeAircrafts.Keys)
                {

                    aircraftReturn air = new aircraftReturn();

                    air.pseudo = aircraftPlayer;
                    if (((Aircraft)listeAircrafts[aircraftPlayer]).killed)
                    {
                        ((Aircraft)listeAircrafts[aircraftPlayer]).xwingPosition = new Vector3(40, 40, 40);
                        ((Aircraft)listeAircrafts[aircraftPlayer]).xwingRotation = Quaternion.Identity;
                        ((Aircraft)listeAircrafts[aircraftPlayer]).killed = false;
                    }
                    air.position = ((Aircraft)listeAircrafts[aircraftPlayer]).xwingPosition;
                    air.rotation = ((Aircraft)listeAircrafts[aircraftPlayer]).xwingRotation;
                    air.score = ((Aircraft)listeAircrafts[aircraftPlayer]).score;
                    
                    airs.Add(air);




                }
            

            return airs;
        }

        public void touched(int i, String pseudoCible, String pseudoAttaquant)
        {
            
            this.removeBullet(i, pseudoAttaquant);

            ((Aircraft)listeAircrafts[pseudoAttaquant]).score += 1;
            ((Aircraft)listeAircrafts[pseudoCible]).xwingPosition = new Vector3(40, 40, 40);
            ((Aircraft)listeAircrafts[pseudoCible]).xwingRotation = Quaternion.Identity;
            ((Aircraft)listeAircrafts[pseudoCible]).killed = true;
            
        }


        public void penalitate(String pseudo)
        {
            ((Aircraft)listeAircrafts[pseudo]).score -= 1;
        }


    }
}