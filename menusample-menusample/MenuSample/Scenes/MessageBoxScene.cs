using System;
using MenuSample.Inputs;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Une popup utilisée pour afficher les messages de confirmation
    /// </summary>
    public class MessageBoxScene : AbstractGameScene
    {
        #region Fields

        private const float Scale = 0.8f;
        private readonly string _message;
        private Texture2D _gradientTexture;

        #endregion

        #region Events

        public event EventHandler Accepted;
        public event EventHandler Cancelled;

        #endregion

        #region Initialization

        /// <summary>
        /// Le constructeur permet à l'appeler de spécifier s'il veut utiliser
        /// ou non le message d'usage "A=Ok, B=Annuler"
        /// </summary>
        public MessageBoxScene(SceneManager sceneMgr, string message, bool includeUsageText = true)
            : base(sceneMgr)
        {
            const string usageText = "\nBouton A, Espace, Entree = Ok" +
                                     "\nBouton B, Echap = Annuler";

            _message = message;

            if (includeUsageText)
                _message += usageText;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        protected override void LoadContent()
        {
            ContentManager content = SceneManager.Game.Content;
            _gradientTexture = content.Load<Texture2D>("gradient");
        }

        #endregion

        #region Handle Input

        public override void HandleInput()
        {
            if (InputState.IsMenuSelect())
            {
                if (Accepted != null)
                    Accepted(this, new EventArgs());
                Remove();
            }
            else if (InputState.IsMenuCancel())
            {
                if (Cancelled != null)
                    Cancelled(this, new EventArgs());
                Remove();
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            SpriteFont font = SceneManager.Font;
            SceneManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            Viewport viewport = SceneManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(_message) * Scale;
            Vector2 textPosition = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();
            spriteBatch.Draw(_gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, _message, textPosition, color, 0, Vector2.Zero, Scale,SpriteEffects.None, 0);
            spriteBatch.End();
        }

        #endregion
    }
}
