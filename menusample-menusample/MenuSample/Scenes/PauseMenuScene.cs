using System;
using MenuSample.Scenes.Core;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Le menu de pause vient s'afficher devant le jeu
    /// </summary>
    public class PauseMenuScene : AbstractMenuScene
    {
        #region Fields

        private readonly AbstractGameScene _parent;

        #endregion

        #region Initialization

        public PauseMenuScene(SceneManager sceneMgr, AbstractGameScene parent)
            : base(sceneMgr, "Pause")
        {
            _parent = parent;

            // Création des options
            var resumeGameMenuItem = new MenuItem("Revenir au jeu");
            var quitGameMenuItem = new MenuItem("Quitter le jeu");
            
            // Gestion des évènements
            resumeGameMenuItem.Selected += OnCancel;
            quitGameMenuItem.Selected += QuitGameMenuItemSelected;

            // Ajout des options du menu
            MenuItems.Add(resumeGameMenuItem);
            MenuItems.Add(quitGameMenuItem);
        }

        #endregion

        #region Handle Input

        private void QuitGameMenuItemSelected(object sender, EventArgs e)
        {
            const string message = "Etes vous sur de vouloir quitter ce jeu?\n";
            var confirmQuitMessageBox = new MessageBoxScene(SceneManager, message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
            confirmQuitMessageBox.Add();
        }

        private void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            Remove();
            _parent.Remove();  ((GameplayScene)_parent).channelFactory.Close();      
            LoadingScene.Load(SceneManager, false);
            
        }

        #endregion
    }
}
