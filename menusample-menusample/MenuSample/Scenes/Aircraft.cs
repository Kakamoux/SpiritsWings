using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using serveur;
using System.Collections;
using System.Diagnostics;
//using Nuclex.Fonts;
//using Nuclex.Graphics;



namespace Series3D1
{

    class Triangle
    {

        Vector3 coin1;
        Vector3 coin2;
        Vector3 coin3;

        public Triangle(Vector3 coin1, Vector3 coin2, Vector3 coin3)
        {
            this.coin1 = coin1;
            this.coin2 = coin2;
            this.coin3 = coin3;
        }

        public bool PointInTriangle(Vector3 R1, Vector3 R2)
        {

            Vector3 P1 = coin1;
            Vector3 P2 = coin2;
            Vector3 P3 = coin3;


            /*! @param PIP Point-in-Plane */
            Vector3 Normal, IntersectPos;

            // Find Triangle Normal
            Normal = Vector3.Cross(P2 - P1, P3 - P1);
            Normal.Normalize(); // not really needed?  Vector3f does this with cross.

            // Find distance from LP1 and LP2 to the plane defined by the triangle


            float Dist1 = Vector3.Dot(R1 - P1, Normal);
            float Dist2 = Vector3.Dot(R2 - P1, Normal);

            if ((Dist1 * Dist2) >= 0.0f)
            {
                //SFLog(@"no cross"); 
                return false;
            } // line doesn't cross the triangle.

            if (Dist1 == Dist2)
            {
                //SFLog(@"parallel"); 
                return false;
            } // line and plane are parallel

            // Find point on the line that intersects with the plane
            IntersectPos = R1 + (R2 - R1) * (-Dist1 / (Dist2 - Dist1));

            // Find if the interesection point lies inside the triangle by testing it against all edges
            Vector3 vTest;
            vTest = Vector3.Cross(Normal, P2 - P1);
            if (Vector3.Dot(vTest, IntersectPos - P1) < 0.0f)
            {
                //SFLog(@"no intersect P2-P1"); 
                return false;
            }

            vTest = Vector3.Cross(Normal, P3 - P2);
            if (Vector3.Dot(vTest, IntersectPos - P2) < 0.0f)
            {
                //SFLog(@"no intersect P3-P2"); 
                return false;
            }

            vTest = Vector3.Cross(Normal, P1 - P3);
            if (Vector3.Dot(vTest, IntersectPos - P1) < 0.0f)
            {
                //SFLog(@"no intersect P1-P3"); 
                return false;
            }

            //PIP = IntersectPos;

            return true;
        }

    }

    class Aircraft : Microsoft.Xna.Framework.Game
    {
        #region attributs

        public Model choper;
        public Effect effect;
        public Vector3 xwingPosition = new Vector3(40, 40, 40);
        public Quaternion xwingRotation = Quaternion.Identity;
        //public BoundingSphere modelSphere;
        List<Bullet> bulletList = new List<Bullet>();
        double lastBulletTime = 0;
        Texture2D bulletTexture;
        IServeur serveur;
        String pseudo;
        public int score = 0;
        public float speedMove = 0.07f;
        List<bulletReturn> bullets = new List<bulletReturn>();
        aircraftReturn air = new aircraftReturn();
        List<aircraftReturn> airs;
        SpriteBatch spriteBatch;
        SpriteFont Font;
        private bool showScore = false;
        private bool penaliti = false;
        Vector3 campos = new Vector3(0, 0.1f, 1.5f);

        //for visual collision
        List<Bullet> shpereCollisionList = new List<Bullet>();

       
        BoundingSphere modelSphereNose;
        BoundingSphere modelSphereMiddle;
        BoundingSphere modelSphereBack;
        BoundingSphere modelSphereTopLeft;
        BoundingSphere modelSphereTopRight;
        BoundingSphere modelSphereBotRight;
        BoundingSphere modelSphereBotLeft;




        #endregion

        public Aircraft(Effect effect, Model choper, Texture2D bullet, IServeur serveur, String pseudo, SpriteFont Font, SpriteBatch spriteBatch)
        {
            this.effect = effect;
            this.choper = choper;
            LoadModel();
            bulletTexture = bullet;
            this.serveur = serveur;
            this.pseudo = pseudo;
            this.Font = Font;
            this.spriteBatch = spriteBatch;

            modelSphereMiddle = new BoundingSphere(xwingPosition, 0.1f);

            




        }

        public void LoadModel()
        {

            /* foreach (ModelMesh mesh in choper.Meshes)
             {
                 foreach (ModelMeshPart meshpart in mesh.MeshParts)
                 {
                     meshpart.Effect = effect.Clone();
                 }
             }*/


        }

        public void DrawModel(Matrix viewMatrix, Matrix projectionMatrix, Vector3 cameraPosition, Vector3 cameraUpDirection, GraphicsDevice device)
        {


            Hashtable aircraftServ = new Hashtable();


            airs = serveur.getAircrafts(pseudo);
            ArrayList coords;

            foreach (aircraftReturn ar in airs)
            {
                coords = new ArrayList();
                coords.Add(ar.position);
                coords.Add(ar.rotation);
                if (ar.pseudo == pseudo)
                    this.score = ar.score;

                aircraftServ.Add(ar.pseudo, coords);
            }
            coords = new ArrayList();
            coords = (ArrayList)aircraftServ[pseudo];
            //synchronisation du client par rapport aux données du serveur
            xwingPosition = (Vector3)coords[0];
            xwingRotation = (Quaternion)coords[1];

            // air.pseudo = pseudo;
            //air.position = xwingPosition;
            //air.rotation = xwingRotation;

            //airs.Add(air);

            foreach (aircraftReturn ar in airs)
            {
                Matrix worldMatrix = Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) * /*Matrix.CreateRotationX(MathHelper.PiOver2) **/ Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion((Quaternion)((ar.rotation))) * Matrix.CreateTranslation((Vector3)((ar.position)));
                Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
                lightDirection.Normalize();
                Matrix[] xwingTransforms = new Matrix[choper.Bones.Count];
                choper.CopyAbsoluteBoneTransformsTo(xwingTransforms);
                foreach (ModelMesh mesh in choper.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = xwingTransforms[mesh.ParentBone.Index] * worldMatrix;
                        effect.View = viewMatrix;
                        effect.Projection = projectionMatrix;
                        effect.EnableDefaultLighting();

                        /*currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                        currentEffect.Parameters["xWorld"].SetValue(xwingTransforms[mesh.ParentBone.Index] * worldMatrix);
                        currentEffect.Parameters["xView"].SetValue(viewMatrix);
                        currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);


                        currentEffect.Parameters["xEnableLighting"].SetValue(true);
                        currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);

                        currentEffect.Parameters["xAmbient"].SetValue(0.5f);*/
                    }
                    mesh.Draw();
                }
                /*Text helloWorldText;
                VectorFont arialVectorFont;
                arialVectorFont = this._content.Load<VectorFont>("Content/Fonts/Arial");
                helloWorldText = arialVectorFont.Fill("Hello World!");
                TextBatch textBatch = ;
                textBatch.Begin();
                textBatch.DrawText(
                  helloWorldText, Color.Red);
                textBatch.End();*/

            }

            DrawBullets(viewMatrix, projectionMatrix, cameraPosition, cameraUpDirection, device, airs);
           // DrawSphereCollision(viewMatrix, projectionMatrix, cameraPosition, cameraUpDirection, device, airs);

            if (showScore)
            {
                this.drawScore();
            }
            else
            {
                string text1 = string.Format("Score : {0}", score);
                spriteBatch.Begin();
                spriteBatch.DrawString(Font, text1, Vector2.Zero, Color.HotPink);
                spriteBatch.End();
            }

        }


        private void DrawBullets(Matrix viewMatrix, Matrix projectionMatrix, Vector3 cameraPosition, Vector3 cameraUpDirection, GraphicsDevice device, List<aircraftReturn> ar)
        {

            Hashtable bulletsServ = new Hashtable();

            bullets = serveur.getBullets(pseudo);


            if (bullets.Count > 0)
            {
                VertexPositionTexture[] bulletVertices = new VertexPositionTexture[bullets.Count * 6];
                int i = 0;
                foreach (bulletReturn currentBullet in bullets)
                {
                    Vector3 center = currentBullet.position;

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 0));

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                }

                effect.CurrentTechnique = effect.Techniques["PointSprites"];
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xProjection"].SetValue(projectionMatrix);
                effect.Parameters["xView"].SetValue(viewMatrix);
                effect.Parameters["xCamPos"].SetValue(cameraPosition);
                effect.Parameters["xTexture"].SetValue(bulletTexture);
                effect.Parameters["xCamUp"].SetValue(cameraUpDirection);
                effect.Parameters["xPointSpriteSize"].SetValue(0.1f);


                device.BlendState = BlendState.Additive;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawUserPrimitives(PrimitiveType.TriangleList, bulletVertices, 0, bullets.Count * 2);
                }

                device.BlendState = BlendState.Opaque;

                checkTouched(ar);
            }
        }

        private void DrawSphereCollision(Matrix viewMatrix, Matrix projectionMatrix, Vector3 cameraPosition, Vector3 cameraUpDirection, GraphicsDevice device, List<aircraftReturn> ar)
        {

            if (shpereCollisionList.Count > 0)
            {
                VertexPositionTexture[] bulletVertices = new VertexPositionTexture[shpereCollisionList.Count * 6];
                int i = 0;
                foreach (Bullet currentBullet in shpereCollisionList)
                {
                    Vector3 center = currentBullet.position;

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 0));

                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 1));
                    bulletVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                }

                effect.CurrentTechnique = effect.Techniques["PointSprites"];
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xProjection"].SetValue(projectionMatrix);
                effect.Parameters["xView"].SetValue(viewMatrix);
                effect.Parameters["xCamPos"].SetValue(cameraPosition);
                effect.Parameters["xTexture"].SetValue(bulletTexture);
                effect.Parameters["xCamUp"].SetValue(cameraUpDirection);
                effect.Parameters["xPointSpriteSize"].SetValue(0.1f);


                device.BlendState = BlendState.Additive;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, bulletVertices, 0, shpereCollisionList.Count * 2);
                }

                device.BlendState = BlendState.Opaque;

            }

            //Draw sufaces of collision
            //if (verticesCollision != null)
            //{
            //    effect.CurrentTechnique = effect.Techniques["PointSprites"];
            //    effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            //    effect.Parameters["xProjection"].SetValue(projectionMatrix);
            //    effect.Parameters["xView"].SetValue(viewMatrix);
            //    effect.Parameters["xCamPos"].SetValue(cameraPosition);
            //    effect.Parameters["xTexture"].SetValue(bulletTexture);
            //    effect.Parameters["xCamUp"].SetValue(cameraUpDirection);
            //    effect.Parameters["xPointSpriteSize"].SetValue(0.1f);


            //    device.BlendState = BlendState.Additive;
            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        device.DrawUserPrimitives(PrimitiveType.TriangleList, verticesCollision, 0, 1);
            //    }

            //    device.BlendState = BlendState.Opaque;
            //}



        }

        public void ProcessKeyboard(GameTime gameTime, ref float gameSpeed)
        {


            float leftRighSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 500.0f;
            leftRighSpeed *= 1.6f * gameSpeed;

            float UpDownSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1200.0f;
            UpDownSpeed *= 1.6f * gameSpeed;

            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Space))
            {
                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                if (currentTime - lastBulletTime > 250)
                {
                    Bullet newBullet = new Bullet(currentTime, xwingPosition, xwingRotation);

                    bulletList.Add(newBullet);
                    serveur.newBullet(newBullet.bulletStart, xwingPosition, xwingRotation, pseudo);

                    lastBulletTime = currentTime;
                }
            }

            //BACK
            if (keys.IsKeyDown(Keys.X))
                campos = new Vector3(0, 0.1f, -1.5f);
            if (keys.IsKeyUp(Keys.X))
                campos = new Vector3(0, 0.1f, 1.5f);

            Boolean keyApress = false;
            //DESSUS
           /* if (keys.IsKeyDown(Keys.A))
            {
                campos = new Vector3(0, 3f, 2f);
                this.speedMove = 0.01f;
                keyApress = true;
            }
            else
            {
                if (keys.IsKeyUp(Keys.A) & keyApress)
                {
                    campos = new Vector3(0, 0.1f, 1.5f);
                    this.speedMove = 0.04f;
                    keyApress = false;
                }
            }*/
            //TAB
            if (keys.IsKeyDown(Keys.Tab))
                showScore = true;
            if (keys.IsKeyUp(Keys.Tab))
                showScore = false;


            float leftRightRot = 0;
            float upDownRot = 0;


            if (speedMove != 0.0f)
            {
                if (keys.IsKeyUp(Keys.W))
                    this.speedMove -= 0.01f;
                if (keys.IsKeyDown(Keys.W))
                    this.speedMove += 0.01f;

                if (this.speedMove > 0.20f)
                    this.speedMove = 0.20f;
                else if (this.speedMove == 0.0f)
                    this.speedMove = 0.0f;
                else if (this.speedMove < 0.04f)
                    this.speedMove = 0.04f;


                if (keys.IsKeyDown(Keys.Right))
                    leftRightRot += leftRighSpeed;
                if (keys.IsKeyDown(Keys.Left))
                    leftRightRot -= leftRighSpeed;
                if (keys.IsKeyDown(Keys.Down))
                    upDownRot += UpDownSpeed;
                if (keys.IsKeyDown(Keys.Up))
                    upDownRot -= UpDownSpeed;

                xwingRotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot);

                updateCollisionSpherePosition();
                //Console.WriteLine(modelSphereNose.Center);

                serveur.sychronizeRotation(xwingRotation, pseudo);
            }

            if (keys.IsKeyDown(Keys.Enter) && penaliti == false)
            {
                xwingPosition = new Vector3(40, 40, 40);
                xwingRotation = Quaternion.Identity;
                gameSpeed = 1f;
                this.speedMove = 0.07f;
                serveur.penalitate(pseudo);
                penaliti = true;
            }
            if (keys.IsKeyUp(Keys.Enter))
                penaliti = false;





        }

        private void updateCollisionSpherePosition()
        {
            modelSphereMiddle.Center = xwingPosition;
            modelSphereNose.Center = Vector3.Transform(new Vector3(0, -0.05f, -0.4f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(xwingPosition));
            modelSphereBack.Center = Vector3.Transform(new Vector3(0, -0.05f, 0.1f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(xwingPosition));
            modelSphereTopLeft.Center = Vector3.Transform(new Vector3(0.3f, 0.1f, -0.05f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(modelSphereBack.Center));
            modelSphereTopRight.Center = Vector3.Transform(new Vector3(-0.3f, 0.1f, -0.05f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(modelSphereBack.Center));
            modelSphereBotRight.Center = Vector3.Transform(new Vector3(-0.3f, -0.1f, -0.05f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(modelSphereBack.Center));
            modelSphereBotLeft.Center = Vector3.Transform(new Vector3(0.3f, -0.1f, -0.05f), Matrix.CreateFromQuaternion(xwingRotation) * Matrix.CreateTranslation(modelSphereBack.Center));



        }

        public void UpdateCamera(ref Matrix viewMatrix, ref Matrix projectionMatrix, ref Vector3 cameraPosition, ref Quaternion cameraRotation, ref Vector3 cameraUpDirection, GraphicsDevice device)
        {
            cameraRotation = Quaternion.Lerp(cameraRotation, xwingRotation, 0.1f);



            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(cameraRotation));
            campos += xwingPosition;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(cameraRotation));

            viewMatrix = Matrix.CreateLookAt(campos, xwingPosition, camup);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);

            cameraPosition = campos;
            cameraUpDirection = camup;

        }

        private void UpdateBulletPositions(float moveSpeed, GameTime gameTime)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - bulletList[i].bulletStart > 1000)
                {
                    bulletList.Remove(bulletList[i]);
                    serveur.removeBullet(i, pseudo);
                }
                else
                {
                    Bullet currentBullet = bulletList[i];
                    MoveForward(ref currentBullet.position, currentBullet.rotation, moveSpeed * 2.0f);
                    bulletList[i] = currentBullet;
                    serveur.updateBullet(currentBullet.position, currentBullet.rotation, pseudo, i);

                }

            }


        }

        private void MoveForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotationQuat);
            position += addVector * speed;
        }

        public void move(Terrain terrain, float moveSpeed, ref float gameSpeed, GameTime gameTime)
        {

            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), xwingRotation) * this.speedMove;
            xwingPosition += addVector;
            serveur.sychronizePosition(xwingPosition, pseudo);
            if (Iscolision(terrain))
            {
                //xwingPosition = new Vector3(20, 20, 30);
                speedMove = 0.0f;
            }

            UpdateBulletPositions(moveSpeed, gameTime);
        }

        private bool Iscolision(Terrain terrain)
        {
            //A NOTER : Coordonnées Y et Z de xwing, inversées par rapport à la map

            //Object de référence pour la collision
            
          

           

            //************************Liste des sphères de collision
            List<BoundingSphere> objCollList = new List<BoundingSphere>();
            List<Triangle> planCollision = new List<Triangle>();
            // float terrainZ;
            int xTab;
            int yTab;

            float zGraphic;

            //Point Round
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2));
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2));
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));


            //Point Round x+1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) + 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2));
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round x-1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) - 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2));
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round y+1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2));
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) + 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round y-1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2));
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) - 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round x+1, y+1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) + 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) + 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round x+1, y-1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) + 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) - 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round x-1, y-1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) - 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) - 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            //Point Round x-1, y+1
            xTab = (int)Math.Abs(Math.Round(xwingPosition.X) + ((terrain.terrainWidth) / 2)) - 1;
            yTab = (int)Math.Abs((Math.Round(xwingPosition.Z)) - ((terrain.terrainHeight) / 2)) + 1;
            zGraphic = terrain.getHeightValue(xTab, yTab);
            objCollList.Add(new BoundingSphere(terrain.getGraphicCoordinate(xTab, yTab, zGraphic), 0.1f));

            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[1].Center, objCollList[3].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[2].Center, objCollList[4].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[2].Center, objCollList[3].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[4].Center, objCollList[1].Center));
            planCollision.Add(new Triangle(objCollList[5].Center, objCollList[1].Center, objCollList[3].Center));
            planCollision.Add(new Triangle(objCollList[6].Center, objCollList[1].Center, objCollList[4].Center));
            planCollision.Add(new Triangle(objCollList[7].Center, objCollList[2].Center, objCollList[4].Center));
            planCollision.Add(new Triangle(objCollList[8].Center, objCollList[2].Center, objCollList[3].Center));
            planCollision.Add(new Triangle(objCollList[3].Center, objCollList[0].Center, objCollList[5].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[5].Center, objCollList[1].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[1].Center, objCollList[6].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[6].Center, objCollList[4].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[4].Center, objCollList[7].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[7].Center, objCollList[2].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[2].Center, objCollList[8].Center));
            planCollision.Add(new Triangle(objCollList[0].Center, objCollList[8].Center, objCollList[3].Center));





            //FIN************************Liste des sphères de collision

            //Sortie des coordonnées
            //int idSphere = 0;
            //String mapheightInfos = " || mapH=>X:" + Math.Round(objCollList[idSphere].Center.X) + "-Y:" + Math.Round(objCollList[idSphere].Center.Z) + "-Z:" + Math.Round(objCollList[idSphere].Center.Y);
            //Console.WriteLine("xwin=>X:" + Math.Round(xwingPosition.X) + "-Y:" + Math.Round(xwingPosition.Z) + "-Z:" + Math.Round(xwingPosition.Y) + mapheightInfos);


            //Génère les sphères pour l'affichage
            shpereCollisionList.Clear();
            shpereCollisionList.Add(new Bullet(0, modelSphereMiddle.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereNose.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereBack.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereTopLeft.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereTopRight.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereBotLeft.Center, xwingRotation));
            shpereCollisionList.Add(new Bullet(0, modelSphereBotRight.Center, xwingRotation));





            foreach (BoundingSphere s in objCollList)
            {
                shpereCollisionList.Add(new Bullet(0, s.Center, xwingRotation));
            }

            if (checkCollisionSphereModelAndGroud(planCollision))
                return true;
            if (terrain.outOfTerrain(xwingPosition))
                return true;
            Console.WriteLine(xwingPosition);
           
            return false;
        }

        private bool checkCollisionSphereModelAndGroud(List<Triangle> planCollision)
        {
            if (intersectCollision(modelSphereMiddle, modelSphereNose, planCollision))
            {
              return true;
            }
            else if (intersectCollision(modelSphereMiddle, modelSphereBack, planCollision))
            {
                
                return true;

            }
            else if (intersectCollision(modelSphereBack, modelSphereTopLeft, planCollision))
            {

                return true;

            }
            else if (intersectCollision(modelSphereBack, modelSphereTopRight, planCollision))
            {

                return true;

            }
            else if (intersectCollision(modelSphereBack, modelSphereBotLeft, planCollision))
            {

                return true;

            }
            else if (intersectCollision(modelSphereBack, modelSphereBotRight, planCollision))
            {

                return true;

            }
            return false;
        }

        //Fonction qui détecte les collision
        private Boolean intersectCollision(BoundingSphere lineStart, BoundingSphere lineEnd, List<Triangle> objColls)
        {
            foreach (Triangle tr in objColls)
            {
                if (tr.PointInTriangle(lineStart.Center, lineEnd.Center))
                {
                    return true;
                }
            }
            return false;
        }

        private void checkTouched(List<aircraftReturn> ars)
        {
            foreach (aircraftReturn ar in ars)
            {
                if (ar.pseudo != pseudo)
                {
                    for (int i = 0; i < bulletList.Count(); i++)
                    {
                        if (isTouched(ar, bulletList[i]))
                        {
                            serveur.touched(i, ar.pseudo, pseudo);
                            bulletList.RemoveAt(i);

                        }
                    }
                }
            }
        }

        private bool isTouched(aircraftReturn ar, Bullet bl)
        {
            BoundingSphere bulletHit = new BoundingSphere(bl.position, 0.1f);
            BoundingSphere aircraftHit = new BoundingSphere(ar.position, 0.5f);
            if (bulletHit.Intersects(aircraftHit))
                return true;
            return false;
        }

        public void killed()
        {
            xwingPosition = new Vector3(40, 40, 40);
            xwingRotation = Quaternion.Identity;
        }

        private void drawScore()
        {
            string text;



            Vector2 drawpos = new Vector2();
            Vector2 decal = new Vector2(0, 30);
            drawpos = Vector2.Zero;
            Hashtable scoreTab = new Hashtable();


            foreach (aircraftReturn ar in airs)
            {
                scoreTab.Add(ar.pseudo, ar.score);

            }

            IComparer dicEntryComparer = new ComparerDictionnaryEntryOnDoubleValue();
            ArrayList al = new ArrayList();

            al.AddRange(scoreTab);

            al.Sort(dicEntryComparer);
            spriteBatch.Begin();
            foreach (DictionaryEntry dicEntry in al)
            {
                text = string.Format("{0} : {1}", dicEntry.Key, dicEntry.Value);
                spriteBatch.DrawString(Font, text, drawpos, Color.HotPink);
                drawpos += decal;
            }


            spriteBatch.End();
        }
    }

    public class ComparerDictionnaryEntryOnDoubleValue : IComparer
    {

        int IComparer.Compare(object right, object left)
        {

            // Conversion en double des valeurs.

            double l = Convert.ToDouble(((DictionaryEntry)left).Value);

            double r = Convert.ToDouble(((DictionaryEntry)right).Value);



            // On compare l'élément de gauche avec celui de droite.

            // Le boolean qui en résulte informe si l'élément de gauche


            return (Comparer<double>.Default.Compare(l, r)); // Tri en ordre décroissant.

            // return (Comparer<double>.Default.Compare(r, l)); // Tri en ordre croissant.

        }

    }
}
