using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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
            UserPreferences.UserPreferencesController.SaveToDisk();
        }
    }


}
