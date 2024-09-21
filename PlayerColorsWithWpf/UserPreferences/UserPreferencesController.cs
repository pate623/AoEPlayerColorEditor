using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PlayerColorEditor.UserPreferences
{
    // TODO Completely rework this behavior and change this name to Config.

    /// <summary>
    /// User preferences are stored in a JSON file at the root of this program.
    /// </summary>
    public static class UserPreferencesController
    {
        public static string PlayerColorPaletteLocation = Path.Combine(Directory.GetCurrentDirectory(), "Palettes");
        public static int ActivePlayerColorPalette = 0;
        public static int ActiveComparedToPalette = 1;
        public static int ActiveInterpolationStyle = 0;

        private static readonly string UserPreferenceFileLocation = Path.Combine(Directory.GetCurrentDirectory(), "UserPreferences.json");

        /// <summary>
        /// Loads user preferences from disk to memory.<br/>
        /// If no preferences are found creates the file with default settings.<br/>
        /// </summary>
        public static void Initialize()
        {
            Debug.WriteLine("Started loading user preferences.");

            if (File.Exists(UserPreferenceFileLocation))
            {
                string preferencesFromDisk = File.ReadAllText(UserPreferenceFileLocation);
                UserPreferencesModel userPreferences = Utilities.Json.DeserializeObject<UserPreferencesModel>(preferencesFromDisk);

                PlayerColorPaletteLocation = userPreferences.PaletteLocation;
                ActivePlayerColorPalette = userPreferences.ActiveColorPalette;
                ActiveComparedToPalette = userPreferences.ActiveComparedTo;
                ActiveInterpolationStyle = userPreferences.ActiveInterpolation;
                Application.Current.MainWindow.Width = userPreferences.WindowsWidth;
                Application.Current.MainWindow.Height = userPreferences.WindowsHeight;
                Application.Current.MainWindow.Left = userPreferences.WindowsLeft;
                Application.Current.MainWindow.Top = userPreferences.WindowsTop;
                Debug.WriteLine("Previous user preference file found and loaded.");
            }
            else
            {
                Debug.WriteLine("No user preference file found, creating default preferences.");
                Application.Current.MainWindow.Width = MainWindowsControls.WindowSizer.DefaultWidth;
                Application.Current.MainWindow.Height = MainWindowsControls.WindowSizer.DefaultHeight;
                ActiveComparedToPalette = 1; // Reads the default values as 0 even though it is set to 1.

                UserPreferencesModel newPreferences = new(
                    paletteLocation: PlayerColorPaletteLocation,
                    activeColorPalette: ActivePlayerColorPalette,
                    activeComparedToPalette: ActiveComparedToPalette,
                    activeInterpolationMode: ActiveInterpolationStyle,
                    windowsWidth: (int)MainWindowsControls.WindowSizer.DefaultWidth,
                    windowsHeight: (int)MainWindowsControls.WindowSizer.DefaultHeight,
                    windowsLeft: (int)MainWindowsControls.WindowSizer.DefaultLeft,
                    windowsTop: (int)MainWindowsControls.WindowSizer.DefaultTop
                );

                File.WriteAllText(UserPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));

                Debug.WriteLine("New user preferences created.");
            }
        }

        /// <summary>
        /// Locates all the values to be saved.<br/>
        /// Always overwrites the whole JSON regardless of how many actual values were changed.<br/>
        /// </summary>
        public static void SaveToDisk()
        {
            UserPreferencesModel newPreferences = new(
                paletteLocation:  PlayerColorPaletteLocation,
                activeColorPalette: ActivePlayerColorPalette,
                activeComparedToPalette: ActiveComparedToPalette,
                activeInterpolationMode: ActiveInterpolationStyle,
                windowsWidth: (int)Application.Current.MainWindow.Width,
                windowsHeight: (int)Application.Current.MainWindow.Height,
                windowsLeft: (int)MainWindowsControls.WindowSizer.DefaultLeft,
                windowsTop: (int)MainWindowsControls.WindowSizer.DefaultTop
            );

            File.WriteAllText(UserPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));

            Debug.WriteLine("User preferences saved.");
        }
    }
}
