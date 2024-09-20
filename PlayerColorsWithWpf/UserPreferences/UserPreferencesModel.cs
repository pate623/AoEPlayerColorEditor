
namespace PlayerColorEditor.UserPreferences
{
    /// <summary>
    /// JSON coded "User Preferences" object.
    /// </summary>
    public class UserPreferencesModel
    {
        public string PaletteLocation { get; set; } = "";
        public int ActiveColorPalette { get; set; }
        public int ActiveComparedTo { get; set; }
        public int ActiveInterpolation { get; set; }
        public int WindowsWidth { get; set; }
        public int WindowsHeight { get; set; }
        public int WindowsLeft { get; set; }
        public int WindowsTop { get; set; }
    }
}
