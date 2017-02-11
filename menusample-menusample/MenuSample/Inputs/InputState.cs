using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MenuSample.Inputs
{
    /// <summary>
    /// Classe d'entrée utilisateur minimaliste
    /// </summary>
    public class InputState : GameComponent
    {
        #region Fields

        private static KeyboardState _currentKeyboardState;
        private static GamePadState _currentGamePadState;
        private static KeyboardState _lastKeyboardState;
        private static GamePadState _lastGamePadState;
        public static bool GamePadWasConnected;

        #endregion

        #region Properties

        public static KeyboardState CurrentKeyboardState
        {
            get { return _currentKeyboardState; }
        }

        public static GamePadState CurrentGamePadState
        {
            get { return _currentGamePadState; }
        }

        #endregion

        #region Initialization

        public InputState(Game game)
            : base(game)
        {
            _currentKeyboardState = new KeyboardState();
            _currentGamePadState = new GamePadState();
            _lastKeyboardState = new KeyboardState();
            _lastGamePadState = new GamePadState();
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _lastKeyboardState = _currentKeyboardState;
            _lastGamePadState = _currentGamePadState;
            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(0);
            GamePadWasConnected = _currentGamePadState.IsConnected;
        }

        private static bool IsNewKeyPress(Keys key)
        {
            return (_currentKeyboardState.IsKeyDown(key) &&
                    _lastKeyboardState.IsKeyUp(key));
        }

        private static bool IsNewButtonPress(Buttons button)
        {
            return (_currentGamePadState.IsButtonDown(button) &&
                    _lastGamePadState.IsButtonUp(button));
        }

        public static bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) ||
                   IsNewButtonPress(Buttons.Start);
        }

        public static bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.B) ||
                   IsNewButtonPress(Buttons.Back);
        }

        public static bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up) ||
                   IsNewButtonPress(Buttons.DPadUp) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp);
        }

        public static bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down) ||
                   IsNewButtonPress(Buttons.DPadDown) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown);
            
        }

        public static Keys IsAlphabetPressed()
        {
            if (IsNewKeyPress(Keys.A) || IsNewKeyPress(Keys.Z) || IsNewKeyPress(Keys.E) || IsNewKeyPress(Keys.R) || IsNewKeyPress(Keys.T) || IsNewKeyPress(Keys.Y) || IsNewKeyPress(Keys.U) || IsNewKeyPress(Keys.I) || IsNewKeyPress(Keys.O) || IsNewKeyPress(Keys.P) || IsNewKeyPress(Keys.Q) || IsNewKeyPress(Keys.S) || IsNewKeyPress(Keys.D) || IsNewKeyPress(Keys.F) || IsNewKeyPress(Keys.G) || IsNewKeyPress(Keys.H) || IsNewKeyPress(Keys.J) || IsNewKeyPress(Keys.K) || IsNewKeyPress(Keys.L) || IsNewKeyPress(Keys.M) || IsNewKeyPress(Keys.W) || IsNewKeyPress(Keys.X) || IsNewKeyPress(Keys.C) || IsNewKeyPress(Keys.V) || IsNewKeyPress(Keys.B) || IsNewKeyPress(Keys.N) || IsNewKeyPress(Keys.Back)){
                return _currentKeyboardState.GetPressedKeys()[0];
            }
            return Keys.None ;
            
                

        }

        public static bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.Back) ||
                   IsNewButtonPress(Buttons.Start);
        }

        #endregion
    }
}
