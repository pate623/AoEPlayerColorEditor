using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace PlayerColorsWithWpf
{
    public enum EInterpolationStyles
    {
        Default = 0,
        OnlyMainColor = 1,
        Glowing = 2
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }

    /// <summary>
    /// <br>Records changes on windows location and saves the values into user preferences.</br>
    /// <br>Users can only scale the window in fixed ratio.</br>
    /// <br>Fixed ratio is determined by the window minimum width and height written in the XAML file.</br>
    /// </summary>
    public static class WindowSizer
    {
        public static double DefaultLeft;
        public static double DefaultTop;
        public static double DefaultWidth;
        public static double DefaultHeight;

        private static double WidthRatio;
        private static double HeightRatio;

        private static bool TimerIsRunning = false;
        private static bool TimerNeedsToBeRefreshed = false;

        /// <summary>
        /// <br>Saves current window size and location.</br>
        /// <br>Uses the gained window width and height to determine windows fixed ratio.</br>
        /// </summary>
        public static void Initialize()
        {
            DefaultWidth = Application.Current.MainWindow.MinWidth;
            DefaultHeight = Application.Current.MainWindow.MinHeight;

            DefaultLeft = Application.Current.MainWindow.Left;
            DefaultTop = Application.Current.MainWindow.Top;

            WidthRatio = DefaultWidth / DefaultHeight;
            HeightRatio = DefaultHeight / DefaultWidth;
        }

        /// <summary>
        /// <br>Saves the new window location and size to the user preferences and
        /// saves the user preferences to the disk after user stops adjusting the window.</br>
        /// </summary>
        public static void UserChangedWindowSize()
        {
            double width = Application.Current.MainWindow.Width;
            double height = Application.Current.MainWindow.Height;

            if (width * DefaultWidth > height * DefaultHeight)
            {
                Application.Current.MainWindow.Width = width;
                Application.Current.MainWindow.Height = width * HeightRatio;
            }
            else
            {
                Application.Current.MainWindow.Height = height;
                Application.Current.MainWindow.Width = height * WidthRatio;
            }

            DefaultLeft = Application.Current.MainWindow.Left;
            DefaultTop = Application.Current.MainWindow.Top;

            // Timer for saving user preferences.
            // If timer is active only refreshes it.
            if (TimerIsRunning)
            {
                TimerNeedsToBeRefreshed = true;
            }
            else
            {
                TimerIsRunning = true;
                Debug.WriteLine("User started adjusting the windows size.");
                StartTimer();
            }
        }

        private static async void StartTimer()
        {
            await Task.Delay(200);
            while (TimerNeedsToBeRefreshed)
            {
                TimerNeedsToBeRefreshed = false;
                await Task.Delay(400);
            }

            TimerIsRunning = false;
            Debug.WriteLine("User stopped adjusting the windows size.");
            UserPreferences.SaveToDisk();
        }
    }

    /// <summary>
    /// JSON coded "User Preferences" object.
    /// </summary>
    public class UserPreferencesJSON
    {
        public string PaletteLocation { get; set; }
        public int ActiveColorPalette { get; set; }
        public int ActiveComparedTo { get; set; }
        public int ActiveInterpolation { get; set; }
        public int WindowsWidth { get; set; }
        public int WindowsHeight { get; set; }
        public int WindowsLeft { get; set; }
        public int WindowsTop { get; set; }
    }

    /// <summary>
    /// User preferences are stored in a JSON file at the root of this program.
    /// </summary>
    public static class UserPreferences
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

                var userPreferences =
                    JsonSerializer.Deserialize<UserPreferencesJSON>(preferencesFromDisk);

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
                Application.Current.MainWindow.Width = WindowSizer.DefaultWidth;
                Application.Current.MainWindow.Height = WindowSizer.DefaultHeight;
                ActiveComparedToPalette = 1; // Reads the default values as 0 even though it is set to 1.

                var newPreferences = new UserPreferencesJSON
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette,
                    ActiveComparedTo = ActiveComparedToPalette,
                    ActiveInterpolation = ActiveInterpolationStyle,
                    WindowsWidth = (int)WindowSizer.DefaultWidth,
                    WindowsHeight = (int)WindowSizer.DefaultHeight,
                    WindowsLeft = (int)WindowSizer.DefaultLeft,
                    WindowsTop = (int)WindowSizer.DefaultTop
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
            var newPreferences = new UserPreferencesJSON
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette,
                ActiveComparedTo = ActiveComparedToPalette,
                ActiveInterpolation = ActiveInterpolationStyle,
                WindowsWidth = (int)Application.Current.MainWindow.Width,
                WindowsHeight = (int)Application.Current.MainWindow.Height,
                WindowsLeft = (int)WindowSizer.DefaultLeft,
                WindowsTop = (int)WindowSizer.DefaultTop
            };

            File.WriteAllText(
                UserPreferenceFileLocation,
                JsonSerializer.Serialize(newPreferences));

            Debug.WriteLine("User preferences saved.");
        }
    }
}
