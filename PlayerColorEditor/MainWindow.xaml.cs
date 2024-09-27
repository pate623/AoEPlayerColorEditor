using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlayerColorEditor
{
    public partial class MainWindow : Window
    {
        /// This warning is given whenever a main window element is being searched.
        /// Disabling these warnings here as a whole creates cleaner looking code than disabling these warning at each element search.
#pragma warning disable CS8602

        /// <summary>
        /// Some UI element trigger selection changes on load.<br/>
        /// These triggers can cause the boot order to change which can cause this program to crash.<br/>
        /// </summary>
        private bool MainWindowInitialized { get; set; } = false;

        private readonly List<Rectangle> PlayerColorBoxes = [];
        private readonly List<Rectangle> CompraredToColorBoxes = [];

        /// <summary>These are the player colors which are currently being edited.</summary>
        private readonly Vector3[] CurrentlyActivePlayerColors = new Vector3[8];

        private readonly MainWindowComponents.WindowSizer WindowSizer;
        private readonly MainWindowComponents.PaletteFileCreator PaletteCreator;
        private readonly MainWindowComponents.PalettePreset.PalettePresetController PalettePresets;

        public MainWindow()
        {
            InitializeComponent();
            Show();
            WindowSizer = new(this);
            PaletteCreator = new();
            PalettePresets = new();
            UpdateWindowLocationAndSize();
            LocatePlayerColorBoxes();
            DisplayPaletteFolderLocation();
            DisplaySelectedInterpolationStyle();
            DisplayColorPresetChoices(Settings.ConfigController.Config.ActiveColorPalettePreset);
            DisplayComparedToColorChoices(Settings.ConfigController.Config.ActiveComparedToPalettePreset);
            UpdateDataToSelectedPreset(Settings.ConfigController.Config.ActiveColorPalettePreset);
            DisplayNewlySelectedColors();
            MainWindowInitialized = true;

            /// <summary>Loads the window location from config and sets the main window to this size and location</summary>
            void UpdateWindowLocationAndSize()
            {
                Width = Settings.ConfigController.Config.WindowsWidth;
                Height = Settings.ConfigController.Config.WindowsHeight;
                Left = Settings.ConfigController.Config.WindowsLeft;
                Top = Settings.ConfigController.Config.WindowsTop;
            }

            /// <summary>
            /// Locates all player color squares and adds them to static lists.<br/>
            /// This way the colors can be set in a loop and the scripts needn't locate the color squares each time they need to be changed.<br/>
            /// </summary>
            void LocatePlayerColorBoxes()
            {
#pragma warning disable CS8604
                PlayerColorBoxes.Add(FindName("BluePlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("RedPlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("YellowPlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("BrownPlayerColor") as Rectangle);

                PlayerColorBoxes.Add(FindName("OrangePlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("GreenPlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("PurplePlayerColor") as Rectangle);
                PlayerColorBoxes.Add(FindName("TealPlayerColor") as Rectangle);

                CompraredToColorBoxes.Add(FindName("BlueComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("RedComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("YellowComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("BrownComparedToColor") as Rectangle);

                CompraredToColorBoxes.Add(FindName("OrangeComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("GreenComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("PurpleComparedToColor") as Rectangle);
                CompraredToColorBoxes.Add(FindName("TealComparedToColor") as Rectangle);
#pragma warning restore CS8604
            }

            void DisplaySelectedInterpolationStyle()
            {
                var colorSelection = FindName("ColorInterpolationSelection") as System.Windows.Controls.ComboBox;
                colorSelection.SelectedIndex = (int)Settings.ConfigController.Config.ActiveInterpolationMode;
            }
        }

        #region Active Player Color (Currently Edited)
        /// <summary>
        /// Updates UI and code to display the newly selected color preset as currently active "Your Edit" colors.
        /// </summary>
        private void ColorPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindowInitialized)
                return;

            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex == -1)
                return;

            Debug.WriteLine($"New preset selected from combo box with index {presetSelection.SelectedIndex}");
            UpdateDataToSelectedPreset(presetSelection.SelectedIndex);
            DisplayNewlySelectedColors();
        }

        #region Color Selecting
        private void BluePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(0);
        }
        private void RedPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(1);
        }
        private void YellowPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(2);
        }
        private void BrownPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(3);
        }
        private void OrangePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(4);
        }
        private void GreenPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(5);
        }
        private void PurplePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(6);
        }
        private void TealPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            OpenColorPicker(7);
        }

        /// <summary>
        /// Clicking the color boxes under "Your Edits text" opens up a color picker.<br/>
        /// This color selection allows users to edit the currently active colors to their likings.<br/>
        /// Each time the color picker is closed this script updates <see cref="CurrentlyActivePlayerColors"/> and showcases these changes in the UI.<br/>
        /// </summary>
        private void OpenColorPicker(int selectedPlayerColor)
        {
            Debug.WriteLine("Color picker opened with player color id: " + selectedPlayerColor);

            ColorDialog playerColorPicker = new()
            {
                Color = System.Drawing.Color.FromArgb(
                    0,
                    (byte)CurrentlyActivePlayerColors[selectedPlayerColor].X,
                    (byte)CurrentlyActivePlayerColors[selectedPlayerColor].Y,
                    (byte)CurrentlyActivePlayerColors[selectedPlayerColor].Z)
            };

            _ = playerColorPicker.ShowDialog();

            CurrentlyActivePlayerColors[selectedPlayerColor] = new Vector3(
                playerColorPicker.Color.R,
                playerColorPicker.Color.G,
                playerColorPicker.Color.B);

            playerColorPicker.Dispose();

            Debug.WriteLine("Player colors edited by the user.");
            DisplayNewlySelectedColors();
        }
        #endregion

        /// <summary>
        /// Updates the currently active player colors combo box to display all available choices.<br/>
        /// Uses names from <see cref="AllColorPalettePresets"/> as the combo box choices.<br/>
        /// </summary>
        /// <param name="activePresetIndex">Displays this as active color preset.</param>
        private void DisplayColorPresetChoices(int activePresetIndex)
        {
            var presetsComboBox = FindName("PresetSelection") as System.Windows.Controls.ComboBox;

            presetsComboBox.Items.Clear();

            foreach (var currentColorPreset in PalettePresets.AllColorPalettePresets)
            {
                _ = presetsComboBox.Items.Add(currentColorPreset.PresetName);
            }

            presetsComboBox.SelectedIndex = activePresetIndex;
            Debug.WriteLine("Player color presets list in ComboBox updated.");
        }

        /// <summary>
        /// Updates the "Your Edit" player colors squares shown in the UI.<br/>
        /// Uses the colors from <see cref="CurrentlyActivePlayerColors"/>.<br/>
        /// </summary>
        private void DisplayNewlySelectedColors()
        {
            for (int i = 0; i < PlayerColorBoxes.Count; i++)
            {
                PlayerColorBoxes[i].Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)CurrentlyActivePlayerColors[i].X,
                    (byte)CurrentlyActivePlayerColors[i].Y,
                    (byte)CurrentlyActivePlayerColors[i].Z));
            }

            Debug.WriteLine("UI player colors updated.");
        }

        /// <summary>
        /// <para>Used for loading the active color preset after it has been selected from the Combo Box.</para>
        /// Updates both; the UI and the code.<br/>
        /// In UI disables/enables "save" and "delete" preset buttons depending Whether if the currently selected preset is one of the default presets.<br/>
        /// One the code side; loads the color presets colors into <see cref="CurrentlyActivePlayerColors"/><br/>
        /// Updates user preferences.<br/>
        /// </summary>
        private void UpdateDataToSelectedPreset(int currentlyActivePresetIndex)
        {
            if (currentlyActivePresetIndex < PalettePresets.AllColorPalettePresets.Count && currentlyActivePresetIndex >= 0)
            {
                for (int i = 0; i < CurrentlyActivePlayerColors.Length; i++)
                {
                    CurrentlyActivePlayerColors[i] = new Vector3(
                        PalettePresets.AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).X,
                        PalettePresets.AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).Y,
                        PalettePresets.AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).Z);
                }
                Debug.WriteLine($"Newly selected color {currentlyActivePresetIndex} updated to the Vector3[] NewPlayerColors.");

                Settings.ConfigController.Config.ActiveColorPalettePreset = currentlyActivePresetIndex;
                Settings.ConfigController.SaveToDisk();
            }
            else
            {
                Debug.WriteLine("Failed to update player colors; given index was too big. " +
                    $"Given index number was {currentlyActivePresetIndex} " +
                    $"and the count of palette presets is {PalettePresets.AllColorPalettePresets.Count}.");
            }

            // If currently selected preset is one of the default presets then disable save and delete buttons.
            // Makes sure this doesn't get executed before all window elements are loaded.
            if (FindName("SavePreset") is System.Windows.Controls.Button savePresetButton)
            {
                savePresetButton.IsEnabled = currentlyActivePresetIndex >= Settings.DefaultValues.CountOfUnchangeableColorPresets;

                var deletePresetButton = FindName("DeletePreset") as System.Windows.Controls.Button;
                deletePresetButton.IsEnabled = currentlyActivePresetIndex >= Settings.DefaultValues.CountOfUnchangeableColorPresets;
            }
        }
        #endregion

        #region Compared to Color
        /// <summary>
        /// Updates the compared to player colors combo box UI to showcase the new player color choices.<br/>
        /// Also applies the new colors to "compared to colors" squares.<br/>
        /// Called after user saves, saves as, or deletes a color preset.<br/>
        /// </summary>
        private void ApplyPickedComparedToColors()
        {
            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;
            int toBeSetIndex = presetSelection.SelectedIndex;

            if (PalettePresets.AllColorPalettePresets.Count <= toBeSetIndex)
            {
                Debug.WriteLine("Index too big for comparison combo box, reseted to index 1");
                Settings.ConfigController.Config.ActiveComparedToPalettePreset = Settings.DefaultValues.ComparedToPaletteDropDownSelection;
                DisplayComparedToColorChoices(Settings.DefaultValues.ComparedToPaletteDropDownSelection);
                DisplayComparedToPlayerColors(Settings.DefaultValues.ComparedToPaletteDropDownSelection);
            }
            else
            {
                Settings.ConfigController.Config.ActiveComparedToPalettePreset = toBeSetIndex;
                DisplayComparedToColorChoices(toBeSetIndex);
                DisplayComparedToPlayerColors(toBeSetIndex);
            }
        }

        private void ComparedToColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindowInitialized)
                return;

            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex == -1)
                return;

            Debug.WriteLine($"New preset selected for compared to colors combo box, with index: {presetSelection.SelectedIndex}.");
            Settings.ConfigController.Config.ActiveComparedToPalettePreset = presetSelection.SelectedIndex;
            DisplayComparedToPlayerColors(presetSelection.SelectedIndex);
        }

        /// <summary>
        /// Updates the Compared to player color ComboBox listings in the UI, and saves the currently active selection to the user preferences.
        /// </summary>
        /// <param name="presetID">ID of selected item.</param>
        private void DisplayComparedToColorChoices(int presetID)
        {
            var presetsComboBox = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            presetsComboBox.Items.Clear();

            foreach (var currentColorPreset in PalettePresets.AllColorPalettePresets)
            {
                _ = presetsComboBox.Items.Add(currentColorPreset.PresetName);
            }

            presetsComboBox.SelectedIndex = presetID;
            Debug.WriteLine("Preset list in compared to combo box updated.");

            Settings.ConfigController.Config.ActiveComparedToPalettePreset = presetID;
            Settings.ConfigController.SaveToDisk();
        }

        /// <summary>
        /// Changes UI to show newly selected compared to player colors in the compared to player color squares.
        /// </summary>
        private void DisplayComparedToPlayerColors(int presetID)
        {
            if (PalettePresets.AllColorPalettePresets.Count <= presetID)
            {
                Debug.WriteLine("Index for compared to colors is too big");
                return;
            }

            for (int i = 0; i < CompraredToColorBoxes.Count; i++)
            {
                CompraredToColorBoxes[i].Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)PalettePresets.AllColorPalettePresets[presetID].GetPlayerColor(i).X,
                    (byte)PalettePresets.AllColorPalettePresets[presetID].GetPlayerColor(i).Y,
                    (byte)PalettePresets.AllColorPalettePresets[presetID].GetPlayerColor(i).Z));
            }

            Debug.WriteLine("UI compared to player colors updated.");
            Settings.ConfigController.SaveToDisk();
        }
        #endregion

        #region Color Palette Presets
        /// <summary>
        /// Creates new color palettes object from <see cref="CurrentlyActivePlayerColors"/> and saves it to <see cref="AllColorPalettePresets"/>.<br/>
        /// Saves the palettes from <see cref="AllColorPalettePresets"/> to the disk and sets the UI elements to display the newly created color preset.<br/>
        /// </summary>
        private void AcceptNewPresetName_Click(object sender, RoutedEventArgs e)
        {
            var presetNameString = FindName("NewPresetName") as System.Windows.Controls.TextBox;

            MainWindowComponents.PalettePreset.PalettePresetModel newPreset = new(
                name: presetNameString.Text,
                blue: CurrentlyActivePlayerColors[0],
                red: CurrentlyActivePlayerColors[1],
                yellow: CurrentlyActivePlayerColors[2],
                brown: CurrentlyActivePlayerColors[3],
                orange: CurrentlyActivePlayerColors[4],
                green: CurrentlyActivePlayerColors[5],
                purple: CurrentlyActivePlayerColors[6],
                teal: CurrentlyActivePlayerColors[7]);

            PalettePresets.AllColorPalettePresets.Add(newPreset);

            PalettePresets.SavePalettePresetsToDisk();
            Debug.WriteLine("Created new palette preset.");

            // Update UI to reflect all changes.
            DisplayColorPresetChoices(PalettePresets.AllColorPalettePresets.Count - 1);

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            ApplyPickedComparedToColors();
            PrintToConsole("Created new palette preset", Settings.DefaultValues.ConsoleTextSmallDetail);
        }

        /// <summary>
        /// Closes the "delete preset" pop up and deletes the presets.<br/>
        /// Deletes a preset from <see cref="AllColorPalettePresets"/> at index of currently active "PresetSelection".<br/>
        /// </summary>
        private void AcceptRemoval_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            PalettePresets.AllColorPalettePresets.RemoveAt(presetSelection.SelectedIndex);

            PalettePresets.SavePalettePresetsToDisk();
            Debug.WriteLine("Player color preset deleted.");

            // Update UI to reflect all changes.
            DisplayColorPresetChoices(Settings.DefaultValues.ActivePaletteDropDownSelection);
            DisplayNewlySelectedColors();
            ApplyPickedComparedToColors();
            PrintToConsole("Deleted palette preset", Settings.DefaultValues.ConsoleTextRemoval);
        }

        /// <summary>
        /// Closes the pop up asking a name for new preset without creating new preset.
        /// </summary>
        private void CancelNewPresetCreation_Click(object sender, RoutedEventArgs e)
        {
            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            var colorPresets = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            colorPresets.IsEnabled = true;

            Debug.WriteLine("Save preset as pop up closed.");
        }

        /// <summary>
        /// Closes the "delete preset" pop up without deleting the presets.
        /// </summary>
        private void CancelRemoval_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            Debug.WriteLine("Delete preset pop up closed.");
        }

        /// <summary>
        /// Opens a pop up menu confirming or declining the preset deletion.
        /// </summary>
        private void DeletePreset_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Visible;

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = false;

            Debug.WriteLine("Delete preset pop up opened.");
        }

        /// <summary>
        /// Opens pop up asking a name for the new preset.
        /// </summary>
        private void SavePresetAs_Click(object sender, RoutedEventArgs e)
        {
            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Visible;

            var colorPresets = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            colorPresets.IsEnabled = false;

            var currentlySelectedPreset = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            var newPresetName = FindName("NewPresetName") as System.Windows.Controls.TextBox;
            newPresetName.Text = PalettePresets.AllColorPalettePresets[currentlySelectedPreset.SelectedIndex].PresetName;

            Debug.WriteLine("Save preset as pop up opened.");
        }

        /// <summary>
        /// Saves colors from <see cref="CurrentlyActivePlayerColors"/> to the <see cref="AllColorPalettePresets"/> at "currently active color preset" index.
        /// </summary>
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            int currentPreset = presetSelection.SelectedIndex;

            for (int i = 0; i < PlayerColorBoxes.Count; i++)
            {
                PalettePresets.AllColorPalettePresets[currentPreset].SetPlayerColor(CurrentlyActivePlayerColors[i], i);
            }

            PalettePresets.SavePalettePresetsToDisk();
            ApplyPickedComparedToColors();
            PrintToConsole("Saved palette preset", Settings.DefaultValues.ConsoleTextSmallDetail);
        }
        #endregion

        #region Console
        /// <summary>
        /// Add a line of text to the console window.<br/>
        /// Use empty line as parameter to apply the new maximum line count.<br/>
        /// <para><br/>
        /// Get old text from the console, add it to a list, then insert the "textToBeAdded" parameter to that list at index 0.<br/>
        /// Clear the console window and then add all the lines from the list to it, whilst making sure the row count doesn't  exceed 
        /// <see cref="Settings.DefaultValues.MaxLineCountInConsole"/>.
        /// </para>
        /// </summary>
        private void PrintToConsole(string textToBeAdded, Color? textColor = null)
        {
            // Optional parameter has to be constant, have to declare default text color value here.
            textColor = textColor == null ? Settings.DefaultValues.ConsoleTextBase : textColor;

            var newText = new Run(textToBeAdded + "\n")
            {
                Foreground = new SolidColorBrush((Color)textColor)
            };

            var customConsole = FindName("CustomConsole") as TextBlock;

            List<Inline> newConsoleText = [.. customConsole.Inlines];
            newConsoleText.Insert(0, newText);

            customConsole.Inlines.Clear();

            for (int i = 0; i < newConsoleText.Count && i < Settings.DefaultValues.MaxLineCountInConsole; i++)
            {
                customConsole.Inlines.Add(newConsoleText[i]);
            }
        }
        #endregion

        #region Game Color Palette Creation
        private void ColorInterpolation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindowInitialized)
                return;

            var colorSelection = FindName("ColorInterpolationSelection") as System.Windows.Controls.ComboBox;

            if (colorSelection.SelectedIndex == -1)
                return;

            Debug.WriteLine($"{(EInterpolationStyles)colorSelection.SelectedIndex} interpolation style selected.");
            Settings.ConfigController.Config.ActiveInterpolationMode = (EInterpolationStyles)colorSelection.SelectedIndex;
            Settings.ConfigController.SaveToDisk();
        }

        /// <summary>
        /// Creates all color palettes from <see cref="CurrentlyActivePlayerColors"/>.<br/>
        /// Color palettes location is defined in the user preferences.<br/>
        /// </summary>
        private void CreateColors_Click(object sender, RoutedEventArgs e)
        {
            bool successfullyCreatedPalettes = PaletteCreator.WritePlayerColorToPaletteFiles(CurrentlyActivePlayerColors);
            if (successfullyCreatedPalettes)
            {
                PrintToConsole("Created the color palettes", Settings.DefaultValues.ConsoleTextAdding);
            }
            else
            {
                PrintToConsole("Failed to create color palettes", Settings.DefaultValues.ConsoleTextError);
            }
        }

        /// <summary>
        /// Get color palettes folder location from the user preferences and displays that string in the UI.
        /// </summary>
        private void DisplayPaletteFolderLocation()
        {
            var colorPalettePathText = FindName("ColorPalettesLocation") as TextBlock;

            colorPalettePathText.Text = Settings.ConfigController.Config.PaletteFolderLocation;
            Debug.WriteLine("UI Palette location string updated.");
        }

        /// <summary>
        /// Opens a folder browser.<br/>
        /// Saves the picked folder as the folder where all palette files will be created.<br/>
        /// </summary>
        private void LocateColorPalettes_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog findPaletteFolder = new();

            for (int i = 0; i < Settings.DefaultValues.ExpectedPaletteFolderLocations.Length; i++)
            {
                if (Directory.Exists(Settings.DefaultValues.ExpectedPaletteFolderLocations[i]))
                {
                    findPaletteFolder.SelectedPath = Settings.DefaultValues.ExpectedPaletteFolderLocations[i];
                    Debug.WriteLine("Palette location found automatically.");
                    break;
                }
            }

            DialogResult browseFileResult = findPaletteFolder.ShowDialog();

            if (browseFileResult == System.Windows.Forms.DialogResult.OK)
            {
                Settings.ConfigController.Config.PaletteFolderLocation = findPaletteFolder.SelectedPath;
                Debug.WriteLine("Palette location changed. New palette location is:");
                Debug.WriteLine(Settings.ConfigController.Config.PaletteFolderLocation);
            }

            findPaletteFolder.Dispose();

            DisplayPaletteFolderLocation();
            Settings.ConfigController.SaveToDisk();
        }
        #endregion

        #region Program Info
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var InfoPopUp = FindName("InfoPopUp") as StackPanel;
            bool infoPopUpVisible = InfoPopUp.Visibility == Visibility.Visible;
            InfoPopUp.Visibility = infoPopUpVisible ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        #region Window Controls
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (!MainWindowInitialized)
                return;

            WindowSizer.UserChangedWindowSize();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!MainWindowInitialized)
                return;

            WindowSizer.UserChangedWindowSize();
        }
        #endregion

#pragma warning restore CS8602
    }
}
