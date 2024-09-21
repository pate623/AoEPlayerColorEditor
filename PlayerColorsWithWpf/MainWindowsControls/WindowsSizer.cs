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
        // TODO Move these public values to a separate file called ProgramState.cs
        public static double DefaultLeft { get; private set; }
        public static double DefaultTop { get; private set; }
        public static double DefaultWidth { get; private set; }
        public static double DefaultHeight { get; private set; }

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
            DefaultWidth = Application.Current.MainWindow.MinWidth;
            DefaultHeight = Application.Current.MainWindow.MinHeight;

            DefaultLeft = Application.Current.MainWindow.Left;
            DefaultTop = Application.Current.MainWindow.Top;

            WidthRatio = DefaultWidth / DefaultHeight;
            HeightRatio = DefaultHeight / DefaultWidth;

            Debug.WriteLine($"Windows sizer initializer with: " +
                $"WidthRatio {WidthRatio}, HeightRatio {HeightRatio} " +
                $"DefaultWidth {DefaultWidth}, DefaultHeight {DefaultHeight} " +
                $"DefaultLeft {DefaultLeft}, DefaultTop {DefaultTop}");
        }

        /// <summary>
        /// Saves the new window location and size to the user preferences and saves the user preferences to the disk after user stops adjusting the window.<br/>
        /// </summary>
        public static void UserChangedWindowSize()
        {
            double currenWidth = Application.Current.MainWindow.Width;
            double currentHeight = Application.Current.MainWindow.Height;

            if (currenWidth * DefaultWidth > currentHeight * DefaultHeight)
            {
                Application.Current.MainWindow.Width = currenWidth;
                Application.Current.MainWindow.Height = currenWidth * HeightRatio;
            }
            else
            {
                Application.Current.MainWindow.Height = currentHeight;
                Application.Current.MainWindow.Width = currentHeight * WidthRatio;
            }
            
            DefaultLeft = Application.Current.MainWindow.Left;
            DefaultTop = Application.Current.MainWindow.Top;

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
