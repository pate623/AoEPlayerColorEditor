// Ignore Spelling: App

using PlayerColorEditor;
using System.Windows;

namespace PlayerColorsWithWpf
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Main function of this app.<br/>
        /// Initializes the Main windows and all other relevant tasks.<br/>
        /// </summary>
        void AppStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
