using System;
using System.Threading;
using MenuSample.Inputs;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ServiceModel;
using System.Collections;
using System.Runtime.InteropServices;
using serveur;
using System.IO;
using Series3D1;
using System.Xml.XPath;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene, serveur.IServeurRappel
    {
        #region Fields

        private ContentManager _content;
        private SpriteFont _gameFont;
        private readonly Random _random = new Random();
        private float _pauseAlpha;
        


        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        Effect effect;
        BasicEffect basicEffect;

        Matrix viewMatrix;
        Matrix projectionMatrix;

        SpriteFont Font;
        SpriteBatch spriteBatch;

        MouseState originalMouseState;

        Terrain terrain;
        Aircraft aircraft;
        SkyBox skybox;


        Vector3 cameraPosition;
        Vector3 cameraUpDirection;
        Quaternion cameraRotation = Quaternion.Identity;

        Texture2D heightMap;


        Model sky;
        Model choper;

        float gameSpeed = 1f;
        float leftrightRot;
        float upDownRot;

        public DuplexChannelFactory<IServeur> channelFactory;
        IServeur serveur;
        String pseudo;

        int indexResolution;
        bool fullscreen;


        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScene(SceneManager sceneMgr)
            : base(sceneMgr)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            graphics = sceneMgr.graphicsDeviceManager;

            XPathDocument docxml = new XPathDocument("config.xml");
            XPathNavigator lir = docxml.CreateNavigator();
            // If the node has value
            XPathNodeIterator iterateur = lir.Select("Configs/config");

            while (iterateur.MoveNext())
            {
                // Move to fist element


                //Console.WriteLine(textReader.Name.ToString());
                //Console.WriteLine(textReader.Value.ToString());


                int.TryParse(iterateur.Current.SelectSingleNode("Resolution").Value, out indexResolution);
                Console.WriteLine(iterateur.Current.SelectSingleNode("Fullscreen").Value);
                if (iterateur.Current.SelectSingleNode("Fullscreen").Value.Equals("True"))
                    fullscreen = true;
                else if (iterateur.Current.SelectSingleNode("Fullscreen").Value.Equals("False"))
                    fullscreen = false;


                pseudo = iterateur.Current.SelectSingleNode("Pseudo").Value;


            }

            String resolution = OptionsMenuScene.Resolutions[indexResolution];
            int resolutionHeight;
            int resolutionWidht;

            int.TryParse(resolution.Substring(0, resolution.IndexOf("x")), out resolutionWidht);
           
            int.TryParse(resolution.Substring(resolution.IndexOf("x")+1), out resolutionHeight);

            

            graphics.PreferredBackBufferWidth = resolutionWidht;
            graphics.PreferredBackBufferHeight = resolutionHeight;
            Console.WriteLine(fullscreen);
            graphics.IsFullScreen = fullscreen;
            graphics.ApplyChanges();
            leftrightRot = MathHelper.PiOver2;
            upDownRot = -MathHelper.Pi / 10.0f;

            cameraPosition = new Vector3(0, 50, 0);

            channelFactory = new DuplexChannelFactory<IServeur>(this, "configClient");
            serveur = channelFactory.CreateChannel();
            String re = String.Empty;
            
            try
            {

                serveur.DemarrerSession(pseudo);
                

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");

basicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = false,
                VertexColorEnabled = false,
            };
            _gameFont = _content.Load<SpriteFont>("gamefont");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            effect = _content.Load<Effect>("effects");

            UpdateViewMatrix();

            Font = _gameFont;
            heightMap = _content.Load<Texture2D>("heightmap");
            sky = _content.Load<Model>("skybox");
            choper = _content.Load<Model>("matthieu");

            terrain = new Terrain(_content, device);
            skybox = new SkyBox(effect, sky);
            aircraft = new Aircraft(effect, choper, _content.Load<Texture2D>("bullet"), serveur, pseudo, Font, spriteBatch);

            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            
            

            // En cas de longs période de traitement, appelez cette méthode *tintintin*.
            // Elle indique au mécanisme de synchronisation du jeu que vous avez fini un
            // long traitement, et qu'il ne devrait pas essayer de rattraper le retard.
            // Cela évite un lag au début du jeu.
            SceneManager.Game.ResetElapsedTime();
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {
            base.Update(gameTime, othersceneHasFocus, false);

            _pauseAlpha = coveredByOtherscene 
                ? Math.Min(_pauseAlpha + 1f / 32, 1) 
                : Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {

                
                KeyboardState keyState = Keyboard.GetState();
                aircraft.UpdateCamera(ref viewMatrix, ref projectionMatrix, ref  cameraPosition, ref  cameraRotation, ref  cameraUpDirection, device);
               
                aircraft.ProcessKeyboard(gameTime, ref gameSpeed);
                float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 50.0f * gameSpeed;
                aircraft.move(terrain, moveSpeed, ref gameSpeed, gameTime);

            }
        }

        public override void HandleInput()
        {
            KeyboardState keyboardState = InputState.CurrentKeyboardState;
            GamePadState gamePadState = InputState.CurrentGamePadState;

            // Le menu de pause s'enclenche si un joueur appuie sur la touche assignée
            // au menu de pause, ou lorsque qu'une manette branchée est déconnectée
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       InputState.GamePadWasConnected;

            if (InputState.IsPauseGame() || gamePadDisconnected)
                new PauseMenuScene(SceneManager, this).Add();
            else
            {
                
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
                
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                //rs.FillMode = FillMode.WireFrame;
                device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                device.RasterizerState = rs;
                


                skybox.DrawSkybox(viewMatrix, projectionMatrix, device, aircraft);
                terrain.drawTerrain(viewMatrix, projectionMatrix);
                aircraft.DrawModel(viewMatrix, projectionMatrix, cameraPosition, cameraUpDirection, device);
            }


            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                SceneManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion


        public void pseudoUsed() {
            Console.WriteLine("toto");

        }

        

        #region useless
        /* Deplacement de la camera manuellement
        private void cameraInput(float amount)
        {

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDiference = currentMouseState.X - originalMouseState.X;
                float yDiference = currentMouseState.Y - originalMouseState.Y;

                leftrightRot -= rotationSpeed * xDiference * amount;
                upDownRot -= rotationSpeed * yDiference * amount;
                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                UpdateViewMatrix();

            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.E))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
        }*/

        //deplacement de l'aitcraft

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 finalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraOriginalVectorUp = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalVectorUp, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, finalTarget, cameraRotatedUpVector);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 300.0f);
        }

        /*
                private void AddToCameraPosition(Vector3 vectorToAdd)
                {
                    Vector3 tmp;
                    Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftrightRot);
                    Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
                    tmp = cameraPosition;
                    cameraPosition += moveSpeed * rotatedVector;
                  if (Iscolision())
                   {
                       cameraPosition = tmp; 

                   }
                    else
                    UpdateViewMatrix();
                }*/

        #endregion


    }
}
