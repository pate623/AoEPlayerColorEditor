namespace PlayerColorEditor.Settings
{
    /// <summary>
    /// JSON coded Configuration object.
    /// </summary>
    public class ConfigModel(
        string? paletteLocation,
        int activeColorPalette, 
        int activeComparedToPalette,
        EInterpolationStyles activeInterpolationMode,
        int windowsWidth = 0,
        int windowsHeight = 0,
        int windowsLeft = 0,
        int windowsTop = 0)
    {
        public string? PaletteFolderLocation { get; set; } = paletteLocation;
        public int ActiveColorPalettePreset { get; set; } = activeColorPalette;
        public int ActiveComparedToPalettePreset { get; set; } = activeComparedToPalette;
        public EInterpolationStyles ActiveInterpolationMode { get; set; } = activeInterpolationMode;
        public int WindowsWidth { get; set; } = windowsWidth;
        public int WindowsHeight { get; set; } = windowsHeight;
        public int WindowsLeft { get; set; } = windowsLeft;
        public int WindowsTop { get; set; } = windowsTop;
    }
}
