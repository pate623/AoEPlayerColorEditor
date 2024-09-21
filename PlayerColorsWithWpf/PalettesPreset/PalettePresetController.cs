using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PlayerColorEditor.PalettesPreset
{
    /// <summary>
    /// Creates new player color palettes for Age of Empires Definitive Edition.<br/>
    /// Call <see cref="WritePlayerColorToPaletteFiles"/> with all the player colors as a parameter.<br/>
    /// Creates a total of 8 color palettes.<br/>
    /// <br/>
    /// All presets are stored in a JSON file at the root of this program.<br/>
    /// In the code side files are stored in a JSON compatible object.<br/>
    /// </summary>
    public static class PalettePresetController
    {
        private static readonly string PlayerColorPresetFileLocation = Path.Combine(Directory.GetCurrentDirectory(), "PlayerColorPresets.json");

        /// <summary>
        /// Loads palette presets from JSON file into memory.<br/>
        /// Creates 3 default palette presets if the palette presets JSON file is not found.<br/>
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(PlayerColorPresetFileLocation))
            {
                App.AllColorPalettePresets = Utilities.Json.DeserializeObjects<PalettePresetModel>(File.ReadAllText(PlayerColorPresetFileLocation)).ToList();
                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else
            {
                CreateDefaultPalettePreset();
                SavePalettePresetsToDisk();
                Debug.WriteLine("No Preset JSON found, default presets JSON file created and loaded into memory.");
            }

            Debug.WriteLine($"Current number of palette presets: {App.AllColorPalettePresets.Count}.");
        }

        /// <summary>
        /// Gets all objects from the <see cref="MainWindow.AllColorPalettePresets"/> variable and saves them to PlayerColorPresets.JSON file.
        /// </summary>
        public static async void SavePalettePresetsToDisk()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < App.AllColorPalettePresets.Count; i++)
            {
                jsonTextToWriteInTheFile += JsonSerializer.Serialize(App.AllColorPalettePresets[i], options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileLocation, jsonTextToWriteInTheFile);

            Debug.WriteLine("Player color preset saved to the disk.");
        }

        /// <summary>
        /// Adds default palette presets to <see cref="App.AllColorPalettePresets"/> list.
        /// </summary>
        private static void CreateDefaultPalettePreset()
        {
            PalettePresetModel editorDefaultPlayerColors = new(
                name: "Editor Default",

                blue: new(15, 70, 245),
                red: new(220, 35, 35),
                yellow: new(215, 215, 30),
                brown: new(115, 60, 0),

                orange: new(245, 135, 25),
                green: new(4, 165, 20),
                purple: new(210, 55, 200),
                teal: new(126, 242, 225));

            PalettePresetModel gameDefaultPlayerColors = new(
                name: "AOE:DE Default",

                blue: new(45, 45, 245),
                red: new(210, 40, 40),
                yellow: new(215, 215, 30),
                brown: new(142, 91, 0),

                orange: new(255, 150, 5),
                green: new(4, 165, 20),
                purple: new(150, 15, 250),
                teal: new(126, 242, 225));

            PalettePresetModel highContrastPlayerColors = new(
                name: "High Contrast",

                blue: new(43, 63, 247),
                red: new(224, 27, 27),
                yellow: new(230, 234, 53),
                brown: new(96, 43, 11),

                orange: new(234, 128, 21),
                green: new(30, 165, 5),
                purple: new(218, 3, 186),
                teal: new(126, 241, 184));

            App.AllColorPalettePresets.Add(editorDefaultPlayerColors);
            App.AllColorPalettePresets.Add(gameDefaultPlayerColors);
            App.AllColorPalettePresets.Add(highContrastPlayerColors);
        }
    }
}
