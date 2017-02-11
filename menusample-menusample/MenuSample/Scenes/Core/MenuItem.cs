using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MenuSample.Scenes.Core
{
    /// <summary>
    /// Un élément de menu
    /// </summary>
    public class MenuItem
    {
        #region Fields

        private const float Scale = 0.8f;
        private string _text;
        private float _selectionFade;
        private Vector2 _position;
        public enum POSITION {LEFT, CENTER, RIGHT};
        public enum ALIGNMENT { INLINE, OUTLINE };
        private POSITION displayPostion;
        private ALIGNMENT displayAlignment = ALIGNMENT.OUTLINE;
        private bool TextField = false;
        private Keys curentKeysPressed;
        

        #endregion

        #region Properties

        public Keys getCurentKeysPressed
        {
            get { return this.curentKeysPressed; }
        }

        public void seetCurrentKeyPressed(Keys a)
        {
            curentKeysPressed = a;
        }

        public bool isTextField
        {
            get { return this.TextField; }
        }

        public POSITION getDisplayPostion
        {
            get { return this.displayPostion; }
        }

        public ALIGNMENT getDisplayAlignment
        {
            get { return this.displayAlignment; }
        }

        public string Text
        {
            set { _text = value; }
        }

        public void setIsTextField(bool a)
        {
            TextField= a;
        }

        public Vector2 Position
        {
            set { _position = value; }
        }

        #endregion

        #region Events

        public event EventHandler Selected;

        internal void OnSelectitem()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        #endregion

        #region Initialization

        public MenuItem(string text)
        {
            _text = text;
            this.displayPostion = POSITION.CENTER;
        }
        public MenuItem(string text, POSITION position)
        {
            _text = text;
            this.displayPostion = position;
            
        }

        public MenuItem(string text, POSITION position, ALIGNMENT alignment)
        {
            _text = text;
            this.displayPostion = position;
            this.displayAlignment = alignment;

        }

        public MenuItem(string text, ALIGNMENT alignment)
        {
            _text = text;
            this.displayPostion = POSITION.CENTER;
            this.displayAlignment = alignment;
        }


        #endregion

        #region Update and Draw

        public void Update(bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            _selectionFade = isSelected 
                ? Math.Min(_selectionFade + fadeSpeed, 1) 
                : Math.Max(_selectionFade - fadeSpeed, 0);
        }

        public void Draw(AbstractMenuScene scene, bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Black : Color.White;
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + Scale;
            float scale = Scale + pulsate * 0.05f * _selectionFade;
            color *= scene.TransitionAlpha;
            SceneManager sceneManager = scene.SceneManager;
            SpriteBatch spriteBatch = sceneManager.SpriteBatch;
            SpriteFont font = sceneManager.Font;
            var origin = new Vector2(0, font.LineSpacing / 2f);
            spriteBatch.DrawString(font, _text, _position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        public static int GetHeight(AbstractMenuScene scene)
        {
            return (int)(scene.SceneManager.Font.LineSpacing * Scale);
        }

        public int GetWidth(AbstractMenuScene scene)
        {
            return (int)(scene.SceneManager.Font.MeasureString(_text).X * Scale);
        }

        #endregion
    }
}
