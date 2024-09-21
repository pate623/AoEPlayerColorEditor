// Ignore Spelling: App

using System.Diagnostics;
using System.Windows;

namespace PlayerColorEditor
{
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
            MainWindow mainWindow = new();

            mainWindow.Show();
            mainWindow.LocatePlayerColorBoxes();
            MainWindowsControls.WindowSizer.Initialize();
            UserPreferences.UserPreferencesController.Initialize();
            Palettes.PaletteController.Initialize();
            mainWindow.DisplayNewlySelectedColors();
            mainWindow.DisplayPaletteFolderLocation();
            mainWindow.DisplaySelectedInterpolationStyle();
            mainWindow.DisplayColorPresetChoices(UserPreferences.UserPreferencesController.ActivePlayerColorPalette);
            mainWindow.DisplayComparedToColorChoices(UserPreferences.UserPreferencesController.ActiveComparedToPalette);
            mainWindow.ProgramBooted = true;
            Debug.WriteLine("Program booted successfully.");
        }
    }
}
