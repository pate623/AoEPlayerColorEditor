using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerColorEditor.MainScreen.Components.PalettePreset {
    /// <summary>
    /// Creates new player color palettes for Age of Empires Definitive Edition.<br/>
    /// Call <see cref="WritePlayerColorToPaletteFiles"/> with all the player colors as a parameter.<br/>
    /// Creates a total of 8 color palettes.<br/>
    /// <br/>
    /// All presets are stored in a JSON file at the root of this program.<br/>
    /// In the code side files are stored in a JSON compatible object.<br/>
    /// </summary>
    public class PalettePresetController {
        public List<PalettePresetModel> AllColorPalettePresets { get; private set; } = [];

        private readonly Logger Log = new(typeof(PalettePresetModel));

        private readonly FileInfo PlayerColorPresetFile = new(Settings.DefaultValues.PalettePresetFileLocation);

        /// <summary>
        /// Loads palette presets from JSON file into memory.<br/>
        /// Creates 3 default palette presets if the palette presets JSON file is not found.<br/>
        /// </summary>
        public PalettePresetController() {
            if (PlayerColorPresetFile.Exists) {
                AllColorPalettePresets = Utilities.Json.DeserializeObjects<PalettePresetModel>(PlayerColorPresetFile).ToList();
                Log.Debug("Preset JSON found on star up, all presets loaded into memory.");
            }
            else {
                AllColorPalettePresets = [.. Settings.DefaultValues.PalettePresets()];
                SavePalettePresetsToDisk();
                Log.Debug("No Preset JSON found, default presets JSON file created and loaded into memory.");
            }

            Log.Debug($"Current number of palette presets: {AllColorPalettePresets.Count}.");
        }

        /// <summary>
        /// Gets all objects from the <see cref="MainWindow.AllColorPalettePresets"/> variable and saves them to PlayerColorPresets.JSON file.
        /// </summary>
        public void SavePalettePresetsToDisk() {
            Utilities.Json.SaveToDisk(AllColorPalettePresets, PlayerColorPresetFile, true);
            Log.Debug("Player color preset saved to the disk.");
        }
    }
}
