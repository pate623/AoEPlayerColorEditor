using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PlayerColorEditor.Settings
{
    /// <summary>
    /// Config is stored in a JSON file at the root of this program.
    /// </summary>
    public static class ConfigController
    {
        public static ConfigModel Config { get; private set; } = new(
            null,
            activeColorPalette: DefaultValues.ActivePaletteDropDownSelection,
            activeComparedToPalette: DefaultValues.ComparedToPaletteDropDownSelection,
            activeInterpolationMode: DefaultValues.ActiveInterpolationMode);

        private static readonly FileInfo ConfigFile = new(DefaultValues.ConfigurationFileLocation);

        private static bool SaveDelayTimerIsRunning = false;
        private static bool KeepDelayingConfigSaving = false;

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
                Config.WindowsWidth = DefaultValues.MainWindowWidth;
                Config.WindowsHeight = DefaultValues.MainWindowHeight;
                Config.WindowsLeft = DefaultValues.MainWindowLeft;
                Config.WindowsTop = DefaultValues.MainWindowTop;

                Debug.WriteLine("New Config file created.");
            }
        }

        /// <summary>
        /// Locates all the values to be saved.<br/>
        /// Always overwrites the whole JSON regardless of how many actual values were changed.<br/>
        /// </summary>
        public static void SaveToDisk(bool delayedSaving = false)
        {
            if (!delayedSaving)
            {
                Utilities.Json.SaveToDisk(Config, ConfigFile.FullName, true);
                Debug.WriteLine("Config saved to disk.");
                return;
            }

            if (SaveDelayTimerIsRunning)
            {
                KeepDelayingConfigSaving = true;
            }
            else
            {
                SaveDelayTimerIsRunning = true;
                Debug.WriteLine("Delayed config saving started");
                DelayedConfigSaving();
            }
        }

        /// <summary>
        /// The windows size and location is saved to the config.<br/>
        /// This ensures there won't be unneeded file saving.<br/>
        /// </summary>
        private static async void DelayedConfigSaving()
        {
            await Task.Delay(DefaultValues.DelayedConfigSaveTimer);
            while (KeepDelayingConfigSaving)
            {
                KeepDelayingConfigSaving = false;
                await Task.Delay(DefaultValues.DelayedConfigSaveTimer);
            }

            SaveDelayTimerIsRunning = false;
            Debug.WriteLine("Delayed config saving ended, saving config to disk.");
            SaveToDisk();
        }
    }
}
