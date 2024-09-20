using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace PlayerColorEditor.UserPreferences
{
    /// <summary>
    /// User preferences are stored in a JSON file at the root of this program.
    /// </summary>
    public static class UserPreferencesController
    {
        public static string PlayerColorPaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";
        public static int ActivePlayerColorPalette = 0;
        public static int ActiveComparedToPalette = 1;
        public static int ActiveInterpolationStyle = 0;

        private static readonly string UserPreferenceFileLocation =
            Directory.GetCurrentDirectory() + @"\UserPreferences.json";

        /// <summary>
        /// <br>Loads user preferences from disk to memory.</br>
        /// <br>If no preferences are found creates the file with default settings.</br>
        /// </summary>
        public static void Initialize()
        {
            Debug.WriteLine("Started loading user preferences.");

            if (File.Exists(UserPreferenceFileLocation))
            {
                string preferencesFromDisk = File.ReadAllText(UserPreferenceFileLocation);

                var userPreferences = JsonSerializer.Deserialize<UserPreferencesModel>(preferencesFromDisk);

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
                Debug.WriteLine("No user preference file found.");
                Application.Current.MainWindow.Width = MainWindowsControls.WindowSizer.DefaultWidth;
                Application.Current.MainWindow.Height = MainWindowsControls.WindowSizer.DefaultHeight;
                ActiveComparedToPalette = 1; // Reads the default values as 0 even though it is set to 1.

                var newPreferences = new UserPreferencesModel
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette,
                    ActiveComparedTo = ActiveComparedToPalette,
                    ActiveInterpolation = ActiveInterpolationStyle,
                    WindowsWidth = (int)MainWindowsControls.WindowSizer.DefaultWidth,
                    WindowsHeight = (int)MainWindowsControls.WindowSizer.DefaultHeight,
                    WindowsLeft = (int)MainWindowsControls.WindowSizer.DefaultLeft,
                    WindowsTop = (int)MainWindowsControls.WindowSizer.DefaultTop
                };

                File.WriteAllText(
                    UserPreferenceFileLocation,
                    JsonSerializer.Serialize(newPreferences));

                Debug.WriteLine("New user preferences created.");
            }
        }

        /// <summary>
        /// <br>Locates all the values to be saved.</br>
        /// <br>Always overwrites the whole JSON regardless of how many actual values were changed.</br>
        /// </summary>
        public static void SaveToDisk()
        {
            var newPreferences = new UserPreferencesModel
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette,
                ActiveComparedTo = ActiveComparedToPalette,
                ActiveInterpolation = ActiveInterpolationStyle,
                WindowsWidth = (int)Application.Current.MainWindow.Width,
                WindowsHeight = (int)Application.Current.MainWindow.Height,
                WindowsLeft = (int)MainWindowsControls.WindowSizer.DefaultLeft,
                WindowsTop = (int)MainWindowsControls.WindowSizer.DefaultTop
            };

            File.WriteAllText(
                UserPreferenceFileLocation,
                JsonSerializer.Serialize(newPreferences));

            Debug.WriteLine("User preferences saved.");
        }
    }
}
