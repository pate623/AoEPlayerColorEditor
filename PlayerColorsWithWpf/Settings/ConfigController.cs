using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PlayerColorEditor.Settings
{
    /// <summary>
    /// Config is stored in a JSON file at the root of this program.
    /// </summary>
    public static class ConfigController
    {
        // TODO move this declaration to somewhere else
        private static readonly string DefaultPaletteFolderLocation = Path.Combine(Directory.GetCurrentDirectory(), "Palettes");

        public static ConfigModel Config { get; private set; } = new(
            paletteLocation: DefaultPaletteFolderLocation, 
            activeColorPalette: 0,
            activeComparedToPalette: 1,
            activeInterpolationMode: 0);

        // Uses UserPreferences.json name for backwards compatibility
        private static readonly FileInfo ConfigFile = new(Path.Combine(Directory.GetCurrentDirectory(), "UserPreferences.json"));

        /// <summary>
        /// Loads user preferences from disk to memory.<br/>
        /// If no preferences are found creates the file with default settings.<br/>
        /// </summary>
        public static void Initialize()
        {
            Debug.WriteLine("Started loading user preferences.");

            if (ConfigFile.Exists)
            {
                string preferencesFromDisk = File.ReadAllText(ConfigFile.FullName);
                Config = Utilities.Json.DeserializeObject<ConfigModel>(preferencesFromDisk);
                Debug.WriteLine("Previous Config file found and loaded.");
            }
            else
            {
                Debug.WriteLine("No Config file found, creating default Config.");
                Config.WindowsWidth = (int)MainWindowsControls.WindowSizer.DefaultWidth;
                Config.WindowsHeight = (int)MainWindowsControls.WindowSizer.DefaultHeight;
                Config.WindowsLeft = (int)MainWindowsControls.WindowSizer.DefaultLeft;
                Config.WindowsTop = (int)MainWindowsControls.WindowSizer.DefaultTop;

                Debug.WriteLine("New Config file created.");
            }

            // TODO move this to somewhere else
            SetMainWindowStartingLocation();
        }

        /// <summary>
        /// Locates all the values to be saved.<br/>
        /// Always overwrites the whole JSON regardless of how many actual values were changed.<br/>
        /// </summary>
        public static void SaveToDisk()
        {
            // TODO Create the delayed saving here.
            File.WriteAllText(ConfigFile.FullName, System.Text.Json.JsonSerializer.Serialize(Config));
            Debug.WriteLine("User preferences saved.");
        }

        private static void SetMainWindowStartingLocation()
        {
            Application.Current.MainWindow.Width = Config.WindowsWidth;
            Application.Current.MainWindow.Height = Config.WindowsHeight;
            Application.Current.MainWindow.Left = Config.WindowsLeft;
            Application.Current.MainWindow.Top = Config.WindowsTop;

        }
    }
}
