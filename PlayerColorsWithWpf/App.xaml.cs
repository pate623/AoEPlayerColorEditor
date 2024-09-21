﻿// Ignore Spelling: App

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace PlayerColorEditor
{
    // TODO Figure out best place to store global enums
    public enum EInterpolationStyles
    {
        Default = 0,
        OnlyMainColor = 1,
        Glowing = 2
    }

    /// <summary>
    /// The startup class.
    /// </summary>
    public partial class App : Application
    {
        // TODO Move these to a separate file called ProgramState.cs
        public static EInterpolationStyles PlayerColorInterpolationStyle = EInterpolationStyles.Default; // TODO Move this to config?
        public static List<PalettesPreset.PalettePresetModel> AllColorPalettePresets = [];

        /// <summary>
        /// Main function of this app.<br/>
        /// Initializes the Main windows and all other relevant tasks.<br/>
        /// </summary>
        void AppStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new();

            mainWindow.Show();
            mainWindow.LocatePlayerColorBoxes();
            MainWindowsControls.WindowSizer.Initialize();
            Settings.ConfigController.Initialize();
            PalettesPreset.PalettePresetController.Initialize();
            mainWindow.DisplayNewlySelectedColors();
            mainWindow.DisplayPaletteFolderLocation();
            mainWindow.DisplaySelectedInterpolationStyle();
            mainWindow.DisplayColorPresetChoices(Settings.ConfigController.Config.ActiveColorPalettePreset);
            mainWindow.DisplayComparedToColorChoices(Settings.ConfigController.Config.ActiveComparedToPalettePreset);
            mainWindow.ProgramBooted = true;
            Debug.WriteLine("Program booted successfully.");
        }
    }
}
