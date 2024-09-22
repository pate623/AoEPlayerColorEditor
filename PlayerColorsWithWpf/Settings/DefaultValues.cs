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

        /// <summary>The Color palettes this program has by default.</summary>
        public static PalettesPreset.PalettePresetModel[] PalettePresets()
        {
            PalettesPreset.PalettePresetModel editorDefaultPlayerColors = new(
                name: "Editor Default",
                blue: new(15, 70, 245),
                red: new(220, 35, 35),
                yellow: new(215, 215, 30),
                brown: new(115, 60, 0),
                orange: new(245, 135, 25),
                green: new(4, 165, 20),
                purple: new(210, 55, 200),
                teal: new(126, 242, 225));

            PalettesPreset.PalettePresetModel gameDefaultPlayerColors = new(
                name: "AOE:DE Default",
                blue: new(45, 45, 245),
                red: new(210, 40, 40),
                yellow: new(215, 215, 30),
                brown: new(142, 91, 0),
                orange: new(255, 150, 5),
                green: new(4, 165, 20),
                purple: new(150, 15, 250),
                teal: new(126, 242, 225));

            PalettesPreset.PalettePresetModel highContrastPlayerColors = new(
                name: "High Contrast",
                blue: new(43, 63, 247),
                red: new(224, 27, 27),
                yellow: new(230, 234, 53),
                brown: new(96, 43, 11),
                orange: new(234, 128, 21),
                green: new(30, 165, 5),
                purple: new(218, 3, 186),
                teal: new(126, 241, 184));

            return [editorDefaultPlayerColors, gameDefaultPlayerColors, highContrastPlayerColors];
        }
    }
}