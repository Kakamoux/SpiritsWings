using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using System;

namespace MenuSample
{
    /// <summary>
    /// Ceci est un sample montrant comment gérer différents états de jeu avec transitions.
    /// Démo de menus, scène de chargement, scène de jeu et scène de pause. Cette classe est
    /// extrèmement simple, tout se passe dans le gestionnaire de scènes: le SceneManager.
    /// </summary>
    public class MenuSampleGame : Game
    {
        GraphicsDeviceManager graphics;
        public MenuSampleGame()
        {
            Content.RootDirectory = "Content";

            // Initialisation du GraphicsDeviceManager
            // pour obtenir une fenêtre de dimensions 800*480
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 800, PreferredBackBufferHeight = 480 };

            // Création du gestionnaire de scènes
            var sceneMgr = new SceneManager(this, graphics);

            // Mise à jour automatique de Win... des entrées utilisateur
            // et du gestionnaire de scènes
            Components.Add(new InputState(this));
            Components.Add(sceneMgr);
           // this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f);

            // Activation des premières scènes
            new BackgroundScene(sceneMgr).Add();
            new MainMenuScene(sceneMgr).Add();
        }

        public static void Main()
        {
            // Point d'entrée
            using (var game = new MenuSampleGame())
                game.Run();
        }
    }
}
