// Ignore Spelling: App

using System.Diagnostics;
using System.Windows;

// TODO Create Logger
namespace PlayerColorEditor
{
    // TODO Figure out best place to store global enums
    // Create globals.cs file for this?
    public enum EInterpolationStyles
    {
        Default = 0,
        OnlyMainColor = 1,
        Glowing = 2
    }

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
            Settings.ConfigController.Initialize();
            PalettesPreset.PalettePresetController.Initialize();
            _ = new MainWindow();
            Debug.WriteLine("Program booted successfully.");
        }
    }
}
