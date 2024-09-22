using System.Numerics;

namespace PlayerColorEditor.PalettesPreset
{
    /// <summary>
    /// JSON coded "Player Color Palette Preset" object.<br/>
    /// Player color Blue is index 0 and Teal is index 7<br/>
    /// </summary>
    public class PalettePresetModel(
        string name,
        Vector3 blue, 
        Vector3 red, 
        Vector3 yellow, 
        Vector3 brown,
        Vector3 orange, 
        Vector3 green,
        Vector3 purple, 
        Vector3 teal)
    {
        public string PresetName { get; set; } = name;

        public int[] BluePlayerColor { get; set; } = [(int)blue.X, (int)blue.Y, (int)blue.Z];
        public int[] RedPlayerColor { get; set; } = [(int)red.X, (int)red.Y, (int)red.Z];
        public int[] YellowPlayerColor { get; set; } = [(int)yellow.X, (int)yellow.Y, (int)yellow.Z];
        public int[] BrownPlayerColor { get; set; } = [(int)brown.X, (int)brown.Y, (int)brown.Z];
        public int[] OrangePlayerColor { get; set; } = [(int)orange.X, (int)orange.Y, (int)orange.Z];
        public int[] GreenPlayerColor { get; set; } = [(int)green.X, (int)green.Y, (int)green.Z];
        public int[] PurplePlayerColor { get; set; } = [(int)purple.X, (int)purple.Y, (int)purple.Z];
        public int[] TealPlayerColor { get; set; } = [(int)teal.X, (int)teal.Y, (int)teal.Z];

        public void SetPlayerColor(Vector3 playerColor, int playerIndex)
        {
            switch (playerIndex)
            {
                case 0:
                    BluePlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 1:
                    RedPlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 2:
                    YellowPlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 3:
                    BrownPlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 4:
                    OrangePlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 5:
                    GreenPlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 6:
                    PurplePlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
                case 7:
                    TealPlayerColor = [(int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z];
                    break;
            }
        }

        /// <summary>
        /// Get RGB value of a player color.<br/>
        /// Player color Blue is index 0 and Teal is index 7<br/>
        /// </summary>
        /// <param name="index">The player which colors to get</param>
        /// <returns>The RGB player color</returns>
        public Vector3 GetPlayerColor(int index)
        {
            return index switch
            {
                0 => new Vector3(BluePlayerColor[0], BluePlayerColor[1], BluePlayerColor[2]),
                1 => new Vector3(RedPlayerColor[0], RedPlayerColor[1], RedPlayerColor[2]),
                2 => new Vector3(YellowPlayerColor[0], YellowPlayerColor[1], YellowPlayerColor[2]),
                3 => new Vector3(BrownPlayerColor[0], BrownPlayerColor[1], BrownPlayerColor[2]),
                4 => new Vector3(OrangePlayerColor[0], OrangePlayerColor[1], OrangePlayerColor[2]),
                5 => new Vector3(GreenPlayerColor[0], GreenPlayerColor[1], GreenPlayerColor[2]),
                6 => new Vector3(PurplePlayerColor[0], PurplePlayerColor[1], PurplePlayerColor[2]),
                7 => new Vector3(TealPlayerColor[0], TealPlayerColor[1], TealPlayerColor[2]),
                _ => new Vector3(0, 0, 0),
            };
        }
    }
}
