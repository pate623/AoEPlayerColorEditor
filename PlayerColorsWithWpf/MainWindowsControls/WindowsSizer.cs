using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace PlayerColorEditor.MainWindowsControls
{
    /// <summary>
    /// Records changes on windows location and saves the values into user preferences.<br/>
    /// Users can only scale the window in fixed ratio.<br/>
    /// Fixed ratio is determined by the window minimum width and height written in the XAML file.<br/>
    /// </summary>
    public static class WindowSizer
    {
        private static double WidthRatio;
        private static double HeightRatio;

        private static bool saveDelayTimerIsRunning = false;
        private static bool KeepDelayingConfigSaving = false;

        /// <summary>
        /// Saves current window size and location.<br/>
        /// Uses the gained window width and height to determine windows fixed ratio.<br/>
        /// </summary>
        public static void Initialize()
        {
            WidthRatio = (double)Settings.DefaultValues.MainWindowsWidth / Settings.DefaultValues.MainWindowsHeight;
            HeightRatio = (double)Settings.DefaultValues.MainWindowsHeight / Settings.DefaultValues.MainWindowsWidth;

            Debug.WriteLine($"Windows sizer initializer with: WidthRatio {WidthRatio}, HeightRatio {HeightRatio}");
        }

        /// <summary>
        /// Saves the new window location and size to the user preferences and saves the user preferences to the disk after user stops adjusting the window.<br/>
        /// </summary>
        public static void UserChangedWindowSize()
        {
            double currenWidth = Application.Current.MainWindow.Width;
            double currentHeight = Application.Current.MainWindow.Height;

            bool useCurrentWidth = currenWidth * Settings.DefaultValues.MainWindowsWidth > currentHeight * Settings.DefaultValues.MainWindowsHeight;
            double newWidth = useCurrentWidth ? currenWidth : currentHeight * WidthRatio;
            double newHeight = useCurrentWidth ? currenWidth * HeightRatio : currentHeight;

            Application.Current.MainWindow.Width = newWidth;
            Application.Current.MainWindow.Height = newHeight;

            Settings.ConfigController.Config.WindowsWidth = (int)newWidth;
            Settings.ConfigController.Config.WindowsHeight = (int)newHeight;
            Settings.ConfigController.Config.WindowsTop = (int)Application.Current.MainWindow.Top;
            Settings.ConfigController.Config.WindowsLeft = (int)Application.Current.MainWindow.Left;

            if (saveDelayTimerIsRunning)
            {
                KeepDelayingConfigSaving = true;
            }
            else
            {
                saveDelayTimerIsRunning = true;
                Debug.WriteLine("User started adjusting the windows size.");
                DelayedConfigSaving();
            }
        }

        /// <summary>
        /// The windows size and location is saved to the config.<br/>
        /// This ensures there won't be unneeded file saving.<br/>
        /// </summary>
        private static async void DelayedConfigSaving()
        {
            await Task.Delay(200);
            while (KeepDelayingConfigSaving)
            {
                KeepDelayingConfigSaving = false;
                await Task.Delay(400);
            }

            saveDelayTimerIsRunning = false;
            Debug.WriteLine("User stopped adjusting the windows size.");
            Settings.ConfigController.SaveToDisk();
        }
    }
}
