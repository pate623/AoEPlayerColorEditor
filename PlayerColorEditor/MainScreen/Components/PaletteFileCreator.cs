using System;
using System.IO;
using System.Numerics;

namespace PlayerColorEditor.MainScreen.Components {
    /// <summary>
    /// This class is used for writing the color data into palette files.
    /// </summary>
    class PaletteFileCreator {
        private readonly Logger Log = new(typeof(PaletteFileCreator));

        private readonly string PaletteFileStartingText = $"JASC-PAL{Environment.NewLine}0100{Environment.NewLine}256";

        private readonly string ColorCodeSeperator = " ";
        private readonly string RGBColorSeperator = Environment.NewLine;

        /// <summary>Total of this +1 colors if counting both starting (Player color) and ending <see cref="InterpolatingIntoColors"/> colors.</summary>
        const int ColorInterpolationCount = 15;

        private readonly Vector3[] InterpolatingIntoColors = [
            new(0, 0, 0), // Black
            new(32, 32, 32),
            new(64, 64, 64),
            new(128, 128, 128),
            new(192, 192, 192),
            new(224, 224, 224),
            new(255, 255, 255), // White
            new(128, 96, 64)]; // Brown

        private readonly string[] PaletteNames = [
            "playercolor_blue.pal",
            "playercolor_red.pal",
            "playercolor_yellow.pal",
            "playercolor_brown.pal",
            "playercolor_orange.pal",
            "playercolor_green.pal",
            "playercolor_purple.pal",
            "playercolor_teal.pal"];

        /// <summary>
        /// Running this once creates all 8 player color palettes.<br/>
        /// The palette location is stored in the user preferences.<br/>
        /// </summary>
        /// <param name="playerColors">Holds 8 player colors.</param>
        /// <returns>True if palette files were successfully created.</returns>
        public bool WritePlayerColorToPaletteFiles(Vector3[] playerColors) {
            if (Directory.Exists(Settings.ConfigController.Config.PaletteFolderLocation)) {
                try {
                    for (int i = 0; i < PaletteNames.Length; i++) {
                        File.Delete(Settings.ConfigController.Config.PaletteFolderLocation + @"\" + PaletteNames[i]);
                    }
                    Log.Debug("Previous player colors palettes removed");
                }
                catch (Exception ex) {
                    Log.Debug($"Can't delete currently existing palette files", ex);
                    return false;
                }
            }
            else {
                _ = Directory.CreateDirectory(Settings.ConfigController.Config.PaletteFolderLocation ?? Settings.DefaultValues.PaletteFolderLocation);
                Log.Debug("No player color palette folder found, new player color palette folder created.");
            }

            for (int i = 0; i < PaletteNames.Length; i++) {
                if (!CreatePlayerColorPalette(playerColors[i], PaletteNames[i])) {
                    Log.Debug($"Writing a palette file to disk failed: {PaletteNames[i]}");
                    return false;
                }
            }

            Log.Debug("All player colors created.");
            return true;
        }

        /// <summary>
        /// Creates a single color palette files. Saves that file to the disk.<br/>
        /// From row 1 to row 3 use the default header data.<br/>
        /// From row 4 to 131 creates the player colors data.<br/>
        /// From row 132 to 259 populates the file with "0 0 0" rows.<br/>
        /// Row 260 is the last one and will be empty.<br/>
        /// <br/>
        /// Player colors are created as follows:<br/>
        /// Start with the player color (RGB).<br/>
        /// Write it on the document.<br/>
        /// Use "ColorCodeSeperator" and "RGBColorSeperator" variables to separate each value.<br/>
        /// Get the first value of "InterpolatingIntoColors" array, and linearly interpolate into that value.<br/>
        /// 16 numbers interpolated in total, if counting starting and ending values.<br/>
        /// </summary>
        /// <param name="playerColor">Holds the main color (RGB).</param>
        /// <returns>True if palette file was successfully created.</returns>
        private bool CreatePlayerColorPalette(Vector3 playerColor, string paletteName) {
            string textToWriteInPaletteFile = PaletteFileStartingText;

            foreach (Vector3 interpolateIntoColor in InterpolatingIntoColors) {
                for (int ineterpolateIndex = 0; ineterpolateIndex <= ColorInterpolationCount; ineterpolateIndex++) {
                    switch (Settings.ConfigController.Config.ActiveInterpolationMode) {
                        case EInterpolationStyles.Default: // Same style as the games default interpolation.
                            Vector3 baseInterpolatedColor = InterpolateLinearly(playerColor, interpolateIntoColor, ineterpolateIndex);
                            textToWriteInPaletteFile += RGBColorSeperator;
                            textToWriteInPaletteFile += Math.Round(baseInterpolatedColor.X);
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += Math.Round(baseInterpolatedColor.Y);
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += Math.Round(baseInterpolatedColor.Z);
                            break;
                        case EInterpolationStyles.OnlyMainColor:// Write only the player color value.
                            textToWriteInPaletteFile += RGBColorSeperator;
                            textToWriteInPaletteFile += playerColor.X;
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += playerColor.Y;
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += playerColor.Z;
                            break;
                        case EInterpolationStyles.Glowing: // Adds glow to the darkest colors, otherwise same as default style.
                            Vector3 glowingInterpolatedColor = InterpolateForGlow(playerColor, interpolateIntoColor, ineterpolateIndex);
                            textToWriteInPaletteFile += RGBColorSeperator;
                            textToWriteInPaletteFile += Math.Round(glowingInterpolatedColor.X);
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += Math.Round(glowingInterpolatedColor.Y);
                            textToWriteInPaletteFile += ColorCodeSeperator;
                            textToWriteInPaletteFile += Math.Round(glowingInterpolatedColor.Z);
                            break;
                    }
                }
            }

            // Fill in "0 0 0" line to get a total of 256 lines in color data.
            for (int i = 0; i < 128; i++) {
                textToWriteInPaletteFile += RGBColorSeperator;
                textToWriteInPaletteFile += "0 0 0";
            }

            // Original palette files had an empty line in the end so this one will also have one.
            textToWriteInPaletteFile += RGBColorSeperator;
            try {
                string paletteFolderLocation = Settings.ConfigController.Config.PaletteFolderLocation ?? Settings.DefaultValues.PaletteFolderLocation;
                File.WriteAllText(Path.Combine(paletteFolderLocation, paletteName), textToWriteInPaletteFile);
                return true;
            }
            catch (Exception ex) {
                Log.Error($"Failed to create palette files", ex);
                return false;
            }

            /// <summary>
            /// Linearly interpolates between pColor and InterpolateColor, weights based on interpolateIteration.
            /// </summary>
            static Vector3 InterpolateLinearly(Vector3 pColor, Vector3 InterpolateColor, int interpolateIteration) {
                float interpolateStep = (float)interpolateIteration / ColorInterpolationCount;

                return new Vector3(
                    pColor.X * (1 - interpolateStep) + InterpolateColor.X * interpolateStep,
                    pColor.Y * (1 - interpolateStep) + InterpolateColor.Y * interpolateStep,
                    pColor.Z * (1 - interpolateStep) + InterpolateColor.Z * interpolateStep);
            }

            /// <summary>
            /// Does 3d Interpolation from "pColor"to "InterpolateColor".<br/>
            /// Better color separation than default IneterpolateIndex, not as bad as "OnlyMainColor".<br/>
            /// Has some extra adjustments to prevent colors look burnt.<br/>
            /// </summary>
            static Vector3 InterpolateForGlow(Vector3 pColor, Vector3 InterpolateColor, int interpolateIteration) {
                // Use linear scaling for all but the darkest colors
                if (InterpolateColor.X + InterpolateColor.Y + InterpolateColor.Z > 100) {
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
}
