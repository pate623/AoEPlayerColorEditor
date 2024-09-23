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
        int? windowsWidth = null,
        int? windowsHeight = null,
        int? windowsLeft = null,
        int? windowsTop = null)
    {
        public string? PaletteFolderLocation { get; set; } = paletteLocation;
        public int ActiveColorPalettePreset { get; set; } = activeColorPalette;
        public int ActiveComparedToPalettePreset { get; set; } = activeComparedToPalette;
        public EInterpolationStyles ActiveInterpolationMode { get; set; } = activeInterpolationMode;
        public int WindowsWidth { get; set; } = windowsWidth ?? DefaultValues.MainWindowsWidth;
        public int WindowsHeight { get; set; } = windowsHeight ?? DefaultValues.MainWindowsHeight;
        public int WindowsLeft { get; set; } = windowsLeft ?? DefaultValues.MainWindowsLeft;
        public int WindowsTop { get; set; } = windowsTop ?? DefaultValues.MainWindowsTop;
    }
}
