// Ignore Spelling: App

using System;
using System.Windows;

namespace PlayerColorEditor {
    public partial class App : Application {
        /// <summary>
        /// Main function of this app.<br/>
        /// Initializes the Main windows and all other relevant tasks.<br/>
        /// </summary>
        void AppStartup(object sender, StartupEventArgs e) {
            Logger log = new(typeof(App));
            try {
                log.Debug("Starting program");
                Settings.ConfigController.Initialize();
                _ = new MainScreen.MainWindow();
                log.Info("Program booted successfully.");
            }
            catch (Exception ex) {
                log.Fatal($"Program crashed {ex}");
                Environment.Exit(1);
            }
        }
    }
}
