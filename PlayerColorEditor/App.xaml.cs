// Ignore Spelling: App

using System;
using System.Diagnostics;
using System.Windows;

// TODO Create Logger using NLog
// https://nlog-project.org/
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
            try
            {
                Settings.ConfigController.Initialize();
                _ = new MainScreen.MainWindow();
                Debug.WriteLine("Program booted successfully.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Program crashed {ex}");
                Environment.Exit(1);
            }
        }
    }
}
