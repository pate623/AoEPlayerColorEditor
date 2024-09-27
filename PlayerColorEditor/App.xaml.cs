// Ignore Spelling: App

using System.Diagnostics;
using System.Windows;

namespace PlayerColorEditor
{
    public partial class App : Application
    {
        /// <summary>
        /// Main function of this app.<br/>
        /// Initializes the Main windows and all other relevant tasks.<br/>
        /// </summary>
        void AppStartup(object sender, StartupEventArgs e)
        {
            Settings.ConfigController.Initialize();
            _ = new MainWindow();
            Debug.WriteLine("Program booted successfully.");
        }
    }
}
