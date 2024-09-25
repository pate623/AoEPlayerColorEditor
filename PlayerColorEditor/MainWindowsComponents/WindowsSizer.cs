namespace PlayerColorEditor.MainWindowsComponents
{
    /// <summary>
    /// Records changes on windows location and saves the values into user preferences.<br/>
    /// Users can only scale the window in fixed ratio.<br/>
    /// Fixed ratio is determined by the window minimum width and height written in the XAML file.<br/>
    /// </summary>
    public class WindowSizer(MainWindow parentObject)
    {
        private readonly double WidthRatio = (double)Settings.DefaultValues.MainWindowsWidth / Settings.DefaultValues.MainWindowsHeight;
        private readonly double HeightRatio = (double)Settings.DefaultValues.MainWindowsHeight / Settings.DefaultValues.MainWindowsWidth;

        private readonly MainWindow MainWindow = parentObject;

        /// <summary>
        /// Saves the new window location and size to the user preferences and saves the user preferences to the disk after user stops adjusting the window.<br/>
        /// </summary>
        public void UserChangedWindowSize()
        {
            double currenWidth = MainWindow.Width;
            double currentHeight = MainWindow.Height;

            bool useCurrentWidth = currenWidth * Settings.DefaultValues.MainWindowsWidth > currentHeight * Settings.DefaultValues.MainWindowsHeight;
            double newWidth = useCurrentWidth ? currenWidth : currentHeight * WidthRatio;
            double newHeight = useCurrentWidth ? currenWidth * HeightRatio : currentHeight;

            MainWindow.Width = newWidth;
            MainWindow.Height = newHeight;

            Settings.ConfigController.Config.WindowsWidth = (int)newWidth;
            Settings.ConfigController.Config.WindowsHeight = (int)newHeight;
            Settings.ConfigController.Config.WindowsTop = (int)MainWindow.Top;
            Settings.ConfigController.Config.WindowsLeft = (int)MainWindow.Left;

            Settings.ConfigController.SaveToDisk(delayedSaving: true);
        }
    }
}
