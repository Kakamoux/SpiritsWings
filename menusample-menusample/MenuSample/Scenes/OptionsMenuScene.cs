using System;
using MenuSample.Scenes.Core;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Un exemple de menu d'options
    /// </summary>
    public class OptionsMenuScene : AbstractMenuScene
    {
        #region Fields

        
        private readonly MenuItem _resolutionMenuItem;
        private readonly MenuItem _fullscreenMenuItem;
        private readonly MenuItem _pseudoMenuItem;
        private String pseudo;
       
        private enum Language
        {
            English,
            Francais,
            Espanol,
            Italiano,
            Nihongo,
        }

        private static Language _currentLanguage = Language.Francais;
        public static readonly string[] Resolutions = { "800x600", "1024x600", "1024x768", "1280x600", "1280x720", "1280x768", "1280x1024", "1366x768", "1680x1050" };
        private static int _currentResolution;
        private static bool _fullscreen;
        private static int _volume = 42;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScene(SceneManager sceneMgr)
            : base(sceneMgr, "Options")
        {
            // Création des options du menu
            XPathDocument docxml = new XPathDocument("config.xml");
            XPathNavigator lir = docxml.CreateNavigator();
            // If the node has value
            XPathNodeIterator iterateur = lir.Select("Configs/config");
            while (iterateur.MoveNext())
            {
                // Move to fist element
               
                
                //Console.WriteLine(textReader.Name.ToString());
                //Console.WriteLine(textReader.Value.ToString());

                
                        int.TryParse(iterateur.Current.SelectSingleNode("Resolution").Value, out _currentResolution);

                        if (iterateur.Current.SelectSingleNode("Fullscreen").Value.Equals("True"))
                            _fullscreen = true;
                        else if (iterateur.Current.SelectSingleNode("Fullscreen").Value.Equals("False"))
                            _fullscreen = false;
                        pseudo = iterateur.Current.SelectSingleNode("Pseudo").Value;                        
                
                
            }
            

            
            _resolutionMenuItem = new MenuItem(string.Empty);
            _fullscreenMenuItem = new MenuItem(string.Empty);
            _pseudoMenuItem = new MenuItem(string.Empty);
            _pseudoMenuItem.setIsTextField(true);
           
            SetMenuItemText();
            var back = new MenuItem("Retour", MenuItem.POSITION.RIGHT);
            var validate = new MenuItem("Valider", MenuItem.POSITION.LEFT, MenuItem.ALIGNMENT.INLINE);
            // Gestion des évènements
           
            _resolutionMenuItem.Selected += ResolutionMenuItemSelected;
            _fullscreenMenuItem.Selected += FullscreenMenuItemSelected;
            _pseudoMenuItem.Selected += PseudoMenuItemSelected;
            
            back.Selected += OnCancel;
            validate.Selected += OnValidate;
            
            // Ajout des options au menu
           
            MenuItems.Add(_resolutionMenuItem);
            MenuItems.Add(_fullscreenMenuItem);
            MenuItems.Add(_pseudoMenuItem);
            MenuItems.Add(validate);
            MenuItems.Add(back);
            
        }

        /// <summary>
        /// Mise à jour des valeurs du menu
        /// </summary>
        private void SetMenuItemText()
        {

            _pseudoMenuItem.Text = "Pseudo: " + pseudo;
            _resolutionMenuItem.Text = "Resolution: " + Resolutions[_currentResolution];
            _fullscreenMenuItem.Text = "Plein ecran: " + (_fullscreen ? "oui" : "non");
           
        }

        #endregion

        #region Handle Input

        private void LanguageMenuItemSelected(object sender, EventArgs e)
        {
            _currentLanguage++;

            if (_currentLanguage > Language.Nihongo)
                _currentLanguage = 0;

            SetMenuItemText();
        }

        private void ResolutionMenuItemSelected(object sender, EventArgs e)
        {
            _currentResolution = (_currentResolution + 1) % Resolutions.Length;
            SetMenuItemText();
        }

        private void PseudoMenuItemSelected(object sender, EventArgs e)
        {

            if (((MenuItem)sender).getCurentKeysPressed != Keys.Back)
            {
                pseudo = pseudo + ((MenuItem)sender).getCurentKeysPressed.ToString();
               
            }
            else
            {
                if(pseudo.Length>0)
               pseudo= pseudo.Remove(pseudo.Length - 1);
                
            }
            
            SetMenuItemText();
        }

        private void FullscreenMenuItemSelected(object sender, EventArgs e)
        {
            _fullscreen = !_fullscreen;
            SetMenuItemText();
        }

        private void VolumeMenuItemSelected(object sender, EventArgs e)
        {
            _volume++;
            SetMenuItemText();
        }

        protected void OnValidate(object sender, EventArgs e)
        {
           XmlDocument doc  = new XmlDocument();
           doc.Load("config.xml");
           XmlNodeList nodeList = doc.SelectNodes("Configs/config");
           foreach (XmlNode node in nodeList)
           {
               foreach (XmlNode n in node.ChildNodes)
               {
                   
                   if (n.Name.Equals("Resolution"))
                   {
                       
                       n.InnerXml = _currentResolution.ToString();
                   }
                   else if (n.Name.Equals("Fullscreen"))
                   {
                      
                       n.InnerXml = _fullscreen.ToString();
                   }
                   else if (n.Name.Equals("Pseudo"))
                   {
                       n.InnerXml = pseudo.ToString();
                   }

               }
               
           }
           doc.PreserveWhitespace = true;
           XmlTextWriter wrtr = new XmlTextWriter("config.xml", Encoding.UTF8);
           doc.WriteTo(wrtr);
           wrtr.Close();
           OnCancel();
                

        }

        #endregion
    }
}
