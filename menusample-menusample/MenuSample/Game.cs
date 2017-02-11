using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using System;

namespace MenuSample
{
    /// <summary>
    /// Ceci est un sample montrant comment g�rer diff�rents �tats de jeu avec transitions.
    /// D�mo de menus, sc�ne de chargement, sc�ne de jeu et sc�ne de pause. Cette classe est
    /// extr�mement simple, tout se passe dans le gestionnaire de sc�nes: le SceneManager.
    /// </summary>
    public class MenuSampleGame : Game
    {
        GraphicsDeviceManager graphics;
        public MenuSampleGame()
        {
            Content.RootDirectory = "Content";

            // Initialisation du GraphicsDeviceManager
            // pour obtenir une fen�tre de dimensions 800*480
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 800, PreferredBackBufferHeight = 480 };

            // Cr�ation du gestionnaire de sc�nes
            var sceneMgr = new SceneManager(this, graphics);

            // Mise � jour automatique de Win... des entr�es utilisateur
            // et du gestionnaire de sc�nes
            Components.Add(new InputState(this));
            Components.Add(sceneMgr);
           // this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f);

            // Activation des premi�res sc�nes
            new BackgroundScene(sceneMgr).Add();
            new MainMenuScene(sceneMgr).Add();
        }

        public static void Main()
        {
            // Point d'entr�e
            using (var game = new MenuSampleGame())
                game.Run();
        }
    }
}
