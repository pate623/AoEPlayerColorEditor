using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PlayerColorEditor.MainWindowComponents.PalettePreset
{
    /// <summary>
    /// Creates new player color palettes for Age of Empires Definitive Edition.<br/>
    /// Call <see cref="WritePlayerColorToPaletteFiles"/> with all the player colors as a parameter.<br/>
    /// Creates a total of 8 color palettes.<br/>
    /// <br/>
    /// All presets are stored in a JSON file at the root of this program.<br/>
    /// In the code side files are stored in a JSON compatible object.<br/>
    /// </summary>
    public class PalettePresetController
    {
        public List<PalettePresetModel> AllColorPalettePresets { get; private set; } = [];

        private readonly string PlayerColorPresetFileLocation = Settings.DefaultValues.PalettePresetFileLocation;

        /// <summary>
        /// Loads palette presets from JSON file into memory.<br/>
        /// Creates 3 default palette presets if the palette presets JSON file is not found.<br/>
        /// </summary>
        public PalettePresetController()
        {
            if (File.Exists(PlayerColorPresetFileLocation))
            {
                AllColorPalettePresets = Utilities.Json.DeserializeObjects<PalettePresetModel>(File.ReadAllText(PlayerColorPresetFileLocation)).ToList();
                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else
            {
                AllColorPalettePresets = [.. Settings.DefaultValues.PalettePresets()];
                SavePalettePresetsToDisk();
                Debug.WriteLine("No Preset JSON found, default presets JSON file created and loaded into memory.");
            }

            Debug.WriteLine($"Current number of palette presets: {AllColorPalettePresets.Count}.");
        }

        /// <summary>
        /// Gets all objects from the <see cref="MainWindow.AllColorPalettePresets"/> variable and saves them to PlayerColorPresets.JSON file.
        /// </summary>
        public async void SavePalettePresetsToDisk()
        {
            // TODO Move this to JSON utility.
            JsonSerializerOptions options = new() { WriteIndented = true };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < AllColorPalettePresets.Count; i++)
            {
                jsonTextToWriteInTheFile += JsonSerializer.Serialize(AllColorPalettePresets[i], options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileLocation, jsonTextToWriteInTheFile);

            Debug.WriteLine("Player color preset saved to the disk.");
        }
    }
}
