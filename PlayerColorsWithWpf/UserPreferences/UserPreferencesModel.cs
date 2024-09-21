namespace PlayerColorEditor.UserPreferences
{
    /// <summary>
    /// JSON coded "User Preferences" object.
    /// </summary>
    public class UserPreferencesModel(
        string paletteLocation, 
        int activeColorPalette, 
        int activeComparedToPalette,
        int activeInterpolationMode,
        int windowsWidth,
        int windowsHeight,
        int windowsLeft,
        int windowsTop)
    {
        public string PaletteLocation { get; set; } = paletteLocation;
        public int ActiveColorPalette { get; set; } = activeColorPalette;
        public int ActiveComparedTo { get; set; } = activeComparedToPalette;
        public int ActiveInterpolation { get; set; } = activeInterpolationMode;
        public int WindowsWidth { get; set; } = windowsWidth;
        public int WindowsHeight { get; set; } = windowsHeight;
        public int WindowsLeft { get; set; } = windowsLeft;
        public int WindowsTop { get; set; } = windowsTop;
    }
}
