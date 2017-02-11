using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MenuSample.Scenes.Core
{
    /// <summary>
    /// Le gestionnaire de scènes est un composant que gère des instances de scènes. Il
    /// maintient une pile de scène, appelle leur Update (le Draw est appelé automatiquement)
    /// et limite les entrées utilisateurs à la première scène active de la pile.
    /// </summary>
    public class SceneManager : DrawableGameComponent
    {
        #region Fields

        private readonly List<AbstractGameScene> _scenes = new List<AbstractGameScene>();
        private readonly List<AbstractGameScene> _scenesToUpdate = new List<AbstractGameScene>();
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _blankTexture;
        Game game;
        GraphicsDeviceManager graphics;

        #endregion

        #region Properties


        public GraphicsDeviceManager graphicsDeviceManager
        {
            get { return graphics; }
        }

        /// <summary>
        /// Un SpriteBatch partagé pour toutes les scènes.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        /// <summary>
        /// Une police partagée pour toutes les scènes.
        /// </summary>
        public SpriteFont Font
        {
            get { return _font; }
        }

        #endregion

        #region Initialization

        public SceneManager(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = content.Load<SpriteFont>("menufont");
            _blankTexture = content.Load<Texture2D>("blank");
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            // Fait un copie de la liste principale pour éviter la confusion des
            // processus mettant à jour une scène ou en retirant une.
            _scenesToUpdate.Clear();

            foreach (AbstractGameScene scene in _scenes)
                _scenesToUpdate.Add(scene);

            bool othersceneHasFocus = !Game.IsActive;
            bool coveredByOtherscene = false;

            while (_scenesToUpdate.Count > 0)
            {
                AbstractGameScene scene = _scenesToUpdate[_scenesToUpdate.Count - 1];
                _scenesToUpdate.RemoveAt(_scenesToUpdate.Count - 1);
                scene.Update(gameTime, othersceneHasFocus, coveredByOtherscene);

                if (scene.SceneState == SceneState.TransitionOn ||
                    scene.SceneState == SceneState.Active)
                {
                    // Si c'est la première scène, lui donner l'accès aux entrées utilisateur.
                    if (!othersceneHasFocus)
                    {
                        scene.HandleInput();
                        othersceneHasFocus = true;
                    }

                    // Si la scène courant n'est pas un popup et est active,
                    // informez les scènes suivantes qu'elles sont recouverte.
                    if (!scene.IsPopup)
                        coveredByOtherscene = true;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ajoute une nouvelle scène au gestionnaire. Normalement il faut appeler
        /// la méthode Add de la scène.
        /// </summary>
        public void AddScene(AbstractGameScene scene)
        {
            scene.IsExiting = false;
            Game.Components.Add(scene);
            _scenes.Add(scene);
        }

        /// <summary>
        /// Retire une scène du gestionnaire de scènes. Normalement, il faut appeler
        /// la méthode Remove de la scène qui permet de créer la transition de la scène
        /// avant son retrait du gestionnaire.
        /// </summary>
        public void RemoveScene(AbstractGameScene scene)
        {
            Game.Components.Remove(scene);
            _scenes.Remove(scene);
            _scenesToUpdate.Remove(scene);
        }

        /// <summary>
        /// Méthode aidant à créer le fade to black lors d'une popup
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);
            _spriteBatch.End();
        }

        #endregion
    }
}
