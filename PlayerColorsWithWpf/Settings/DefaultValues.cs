using System.IO;

namespace PlayerColorEditor.Settings
{
    public static class DefaultValues
    {
        public static string PaletteFolderLocation { get { return Path.Combine(Directory.GetCurrentDirectory(), "Palettes"); } }

        // TODO Find a way to get these values from MainWindow.xaml file.
        public static int MainWindowsWidth { get { return 355; } }
        public static int MainWindowsHeight { get { return 595; } }
        public static int MainWindowsLeft { get { return 120; } }
        public static int MainWindowsTop { get { return 120; } }
    }
}