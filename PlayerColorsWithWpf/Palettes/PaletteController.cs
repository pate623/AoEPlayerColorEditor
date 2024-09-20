using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;

namespace PlayerColorEditor.Palettes
{
    /// <summary>
    /// <br>Creates new player color palettes for Age of Empires Definitive Edition.</br>
    /// <br>Call <see cref="CreateColors"/> with all the player colors as a parameter.</br>
    /// <br>Creates a total of 8 color palettes.</br>
    /// </summary>
    /// 
    /// From row 1 to row 3 use the default header data.
    /// From row 4 to 131 creates the player colors data.
    /// From row 132 to 259 populates the file with "0 0 0" rows.
    /// Row 260 is the last one and will be empty.
    /// 
    /// Player colors are created as follows:
    /// Start with the player color (RGB).
    /// Write it on the document.
    /// Use "ColorCodeSeperator" and "RGBColorSeperator" variables to separate each value.
    /// Get the first value of "InterpolatingIntoColors" array, and linearly interpolate into that value.
    /// 16 numbers interpolated in total, if counting starting and ending values
    
    /// <summary>
    /// <br>All presets are stored in a JSON file at the root of this program.</br>
    /// <br>In the code side files are stored in a JSON compatible object.</br>
    /// </summary>
    public static class PaletteController
    {
        private static readonly string PaletteStartingText =
            "JASC-PAL" +
            Environment.NewLine + "0100" +
            Environment.NewLine + "256";

        private static readonly string ColorCodeSeperator = " ";
        private static readonly string RGBColorSeperator = Environment.NewLine;

        /// <summary>Total of this +1 colors if counting both starting and ending colors.</summary>
        private const int ColorInterpolationCount = 15;

        private static readonly Vector3[] InterpolatingIntoColors = [
            new(0, 0, 0),
            new(32, 32, 32),
            new(64, 64, 64),
            new(128, 128, 128),
            new(192, 192, 192),
            new(224, 224, 224),
            new(255, 255, 255),
            new(128, 96, 64)];

        private static readonly string[] PaletteNames = [
            "playercolor_blue.pal",
            "playercolor_red.pal",
            "playercolor_yellow.pal",
            "playercolor_brown.pal",
            "playercolor_orange.pal",
            "playercolor_green.pal",
            "playercolor_purple.pal",
            "playercolor_teal.pal"];

        private static readonly string PlayerColorPresetFileLocation =
            Directory.GetCurrentDirectory() + @"\PlayerColorPresets.json";

        /// <summary>
        /// <br>Loads palette presets from JSON file into memory.</br>
        /// <br>Creates 3 palette presets if palette presets JSON file is not found.</br>
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(PlayerColorPresetFileLocation))
            {
                MainWindow.AllColorPalettePresets = DeserializeObjects<PaletteModel>(File.ReadAllText(PlayerColorPresetFileLocation)).ToList();
                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else
            {
                var editorDefaultPlayerColors = new PaletteModel(
                    name: "Editor Default",

                    blue: new(15, 70, 245),
                    red: new(220, 35, 35),
                    yellow: new(215, 215, 30),
                    brown: new(115, 60, 0),

                    orange: new(245, 135, 25),
                    green: new(4, 165, 20),
                    purple: new(210, 55, 200),
                    teal: new(126, 242, 225)
                );

                var gameDefaultPlayerColors = new PaletteModel(
                    name: "AOE:DE Default",

                    blue: new(45, 45, 245),
                    red: new(210, 40, 40),
                    yellow: new(215, 215, 30),
                    brown: new(142, 91, 0),

                    orange: new(255, 150, 5),
                    green: new(4, 165, 20),
                    purple: new(150, 15, 250),
                    teal: new(126, 242, 225)
                );

                var highContrastPlayerColors = new PaletteModel(
                    name: "High Contrast",

                    blue: new(43, 63, 247),
                    red: new(224, 27, 27),
                    yellow: new(230, 234, 53),
                    brown: new(96, 43, 11),

                    orange: new(234, 128, 21),
                    green: new(30, 165, 5),
                    purple: new(218, 3, 186),
                    teal: new(126, 241, 184)
                );

                MainWindow.AllColorPalettePresets.Add(editorDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(gameDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(highContrastPlayerColors);

                SaveToDisk();

                Debug.WriteLine("No Preset JSON found, new presets JSON file created and loaded into memory.");
            }

            Debug.WriteLine("Current number of palette presets: {0}.", MainWindow.AllColorPalettePresets.Count);
        }

        /// <summary>
        /// <br>Running this once creates all 8 player color palettes.</br>
        /// <br>The palette location is stored in the user preferences.</br>
        /// </summary>
        /// <param name="playerColors">Holds 8 player colors.</param>
        /// <returns>True if palette files were successfully created.</returns>
        public static bool CreateColors(Vector3[] playerColors)
        {
            if (Directory.Exists(UserPreferences.UserPreferencesController.PlayerColorPaletteLocation))
            {
                try
                {
                    for (int i = 0; i < 8; i++)
                    {
                        File.Delete(UserPreferences.UserPreferencesController.PlayerColorPaletteLocation + @"\" + PaletteNames[i]);
                    }
                    Debug.WriteLine("Previous player colors palettes removed");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Can't delete currently existing palette files\n{ex}");
                    return false;
                }
            }
            else
            {
                _ = Directory.CreateDirectory(UserPreferences.UserPreferencesController.PlayerColorPaletteLocation);
                Debug.WriteLine("No player color palette folder found, new player color palette folder created.");
            }

            for (int i = 0; i < 8; i++)
            {
                if (!CreatePlayerColorPalette(playerColors[i], PaletteNames[i]))
                {
                    Debug.WriteLine("Writing a palette file to disk failed: " + PaletteNames[i]);
                    return false;
                }
            }

            Debug.WriteLine("All player colors created.");
            return true;

            /// <summary>
            /// Creates a single color palette files. Saves that file to the disk.
            /// </summary>
            /// <param name="playerColor">Holds the main color (RGB).</param>
            /// <returns>True if palette file was successfully created.</returns>
            static bool CreatePlayerColorPalette(Vector3 playerColor, string paletteName)
            {
                string textToWriteInPaletteFile = PaletteStartingText;

                foreach (Vector3 interpolateIntoColor in InterpolatingIntoColors)
                {
                    for (int ineterpolateIndex = 0; ineterpolateIndex <= ColorInterpolationCount; ineterpolateIndex++)
                    {
                        Vector3 auxiliaryColors;

                        switch (MainWindow.PlayerColorInterpolationStyle)
                        {
                            case EInterpolationStyles.Default:
                                // Same style as the games default interpolation.
                                auxiliaryColors = InterpolateLinearly(
                                    playerColor, interpolateIntoColor, ineterpolateIndex);

                                textToWriteInPaletteFile += RGBColorSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.X);
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.Y);
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.Z);
                                break;

                            case EInterpolationStyles.OnlyMainColor:
                                // Write only the player color value.
                                textToWriteInPaletteFile += RGBColorSeperator;
                                textToWriteInPaletteFile += playerColor.X;
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += playerColor.Y;
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += playerColor.Z;
                                break;

                            case EInterpolationStyles.Glowing:
                                // Adds glow to the darkest colors, otherwise same as default style.
                                auxiliaryColors = InterpolateForGlow(
                                    playerColor, interpolateIntoColor, ineterpolateIndex);

                                textToWriteInPaletteFile += RGBColorSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.X);
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.Y);
                                textToWriteInPaletteFile += ColorCodeSeperator;
                                textToWriteInPaletteFile += Math.Round(auxiliaryColors.Z);
                                break;
                        }
                    }
                }

                // Fill in "0 0 0" line to get a total of 256 lines in color data.
                for (int i = 0; i < 128; i++)
                {
                    textToWriteInPaletteFile += RGBColorSeperator;
                    textToWriteInPaletteFile += "0 0 0";
                }

                // Original palette files had an empty line in the end so this one will also have one.
                textToWriteInPaletteFile += RGBColorSeperator;
                try
                {
                    File.WriteAllText($"{UserPreferences.UserPreferencesController.PlayerColorPaletteLocation}\\{paletteName}", textToWriteInPaletteFile);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create palette files \n{ex}");
                    return false;
                }

                /// <summary>
                /// Linearly interpolates between pColor and InterpolateColor, weights based on interpolateIteration.
                /// </summary>
                static Vector3 InterpolateLinearly(Vector3 pColor, Vector3 InterpolateColor, int interpolateIteration)
                {
                    float interpolateStep = (float)interpolateIteration / ColorInterpolationCount;

                    return new Vector3(
                        pColor.X * (1 - interpolateStep) + InterpolateColor.X * interpolateStep,
                        pColor.Y * (1 - interpolateStep) + InterpolateColor.Y * interpolateStep,
                        pColor.Z * (1 - interpolateStep) + InterpolateColor.Z * interpolateStep);
                }

                /// <summary>
                /// <br>Does 3d Interpolation from "pColor"to "InterpolateColor".</br>
                /// <br>Better color separation than default IneterpolateIndex, not as bad as "OnlyMainColor".</br>
                /// <br>Has some extra adjustments to prevent colors look burnt.</br>
                /// </summary>
                static Vector3 InterpolateForGlow(Vector3 pColor, Vector3 InterpolateColor, int interpolateIteration)
                {
                    // Use linear scaling for all but the darkest colors
                    if (InterpolateColor.X + InterpolateColor.Y + InterpolateColor.Z > 100)
                    {
                        float interpolateStep = (float)interpolateIteration / ColorInterpolationCount;

                        return new Vector3(
                            pColor.X * (1 - interpolateStep) + InterpolateColor.X * interpolateStep,
                            pColor.Y * (1 - interpolateStep) + InterpolateColor.Y * interpolateStep,
                            pColor.Z * (1 - interpolateStep) + InterpolateColor.Z * interpolateStep);
                    }

                    float scalar = (float)(ColorInterpolationCount - interpolateIteration) / ColorInterpolationCount;
                    return Vector3.Lerp(pColor, InterpolateColor, scalar);
                }
            }
        }

        /// <summary>
        /// Gets all objects from the <see cref="MainWindow.AllColorPalettePresets"/> variable and
        /// saves them to PlayerColorPresets.JSON file.
        /// </summary>
        public static async void SaveToDisk()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < MainWindow.AllColorPalettePresets.Count; i++)
            {
                jsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(
                    MainWindow.AllColorPalettePresets[i], options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileLocation, jsonTextToWriteInTheFile);

            Debug.WriteLine("Player color preset saved to the disk.");
        }

        private static IEnumerable<T> DeserializeObjects<T>(string input)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using StringReader strReader = new StringReader(input);
            using JsonTextReader jsonReader = new JsonTextReader(strReader);
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                yield return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
