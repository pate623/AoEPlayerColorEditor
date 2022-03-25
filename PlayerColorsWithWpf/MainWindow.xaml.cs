using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;

/// <summary>
/// Creates new player color palettes for Age of Empires Definitive edition.
/// Saves the files in Palettes folder.
/// Has presets allowing easy color swaps between different color schemes.
/// </summary>
/// 
/// <!-- TODO: Major UI Rework -->
/// Player colors now showcase all the shades of player color and not only the main colors.
/// Create the player colors much larger, create the colors horizontally instead of vertically.
/// Create icon.
/// Create blue colored buttons. Add light brown background (remember to edit all pop ups).
/// Use double line border for the buttons.
/// Use a rough image for the buttons and to the window backgrounds.

namespace PlayerColorsWithWpf
{
    public partial class MainWindow : Window
    {
        public const int CountOfUnchangeableColorPresets = 3;
        public static int MaxLineCountInConsole = 5;
        
        public static List<PalettePresetJSON> AllColorPalettePresets = new List<PalettePresetJSON>();

        public static readonly string[] PaletteFolderDefaultLocations = {
            @"C:\Program Files (x86)\Steam\steamapps\common\AoEDE\Assets\Palettes",
            @"D:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"E:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"C:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"F:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes"
        };

        public static readonly Vector3[] CurrentlyActivePlayerColors = {
            new Vector3(15, 70, 245),
            new Vector3(220, 35, 35),
            new Vector3(215, 215, 30),
            new Vector3(115, 60, 0),
            new Vector3(245, 135, 25),
            new Vector3(4, 165, 20),
            new Vector3(245, 95, 240),
            new Vector3(65, 245, 230)
        };

        public MainWindow()
        {
            InitializeComponent();
            WindowSizer.InitializeWindowSizer();
            UserPreferences.LoadUserPreferences();
            PalettePresets.InitializePresets();
            UpdateCurrenltyActivePresetComboBox(UserPreferences.ActivePlayerColorPalette);
            ShowNewlySelectedColors();
            UpdateUIColorPalettesFolderLocation();
            UpdateComparedToPlayerColorDropDownChoices(UserPreferences.ActiveComparedToPalette);
            Debug.WriteLine("Program booted successfully.");
        }

        private void EditorInfo_Click(object sender, RoutedEventArgs e)
        {
            var infoPopUp = FindName("InfoPopUp") as StackPanel;
            infoPopUp.Visibility = infoPopUp.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <!-- Change colors in "NewPlayerColors" -->
        /// All Color boxes under "Your Edits text".
        /// Clicking these color boxes opens up a color picker.
        /// Each time the color picker is closed this script updates the 
        /// "NewPlayerColors" array and showcases these changes in the UI.
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

        private void OpenColorPicker(int selectedPlayerColor)
        {
            Debug.WriteLine("Color picker opened with player color id: " + selectedPlayerColor);

            var playerColorPicker = new ColorDialog{
                Color = System.Drawing.ColorTranslator.FromHtml("#FF" +
                ((byte)CurrentlyActivePlayerColors[selectedPlayerColor].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[selectedPlayerColor].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[selectedPlayerColor].Z).ToString("X2"))
            };

            _ = playerColorPicker.ShowDialog();

            CurrentlyActivePlayerColors[selectedPlayerColor] = new Vector3(
                playerColorPicker.Color.R,
                playerColorPicker.Color.G,
                playerColorPicker.Color.B);

            playerColorPicker.Dispose();

            Debug.WriteLine("Player colors edited by the user.");
            ShowNewlySelectedColors();
        }

        /// <summary>
        /// <para>Add a line of text to the console windows.</para>
        /// <para>Use empty line as parameter to apply the new maximum line count.</para>
        /// </summary>
        public void UpdateCustomConsole(string textToBeAdded)
        {
            var customConsole = FindName("CustomConsole") as TextBlock;
            string consoleText = customConsole.Text;
            int limit = MaxLineCountInConsole - 1;
            string newConsoleText = textToBeAdded == "" || textToBeAdded == null ? "" : textToBeAdded + "\n";

            foreach (char c in consoleText)
            {
                if (c == '\n')
                {
                    if (--limit <= 0)
                    {
                        break;
                    }
                }
                newConsoleText += c;
            }
            customConsole.Text = newConsoleText;
        }

        private void ComparedToPlayerColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Can't get index from the selected ComboBox item, has to use the ComboBox from the XAML to get access to the index.
            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex != -1)
            {
                Debug.WriteLine("New preset selected for compared to colors, with index: {0}.", presetSelection.SelectedIndex);
                UserPreferences.ActiveComparedToPalette = presetSelection.SelectedIndex;
                SetComparedToPlayerColorBoxes(presetSelection.SelectedIndex);
            }
        }

        /// <summary>
        /// Changes UI to showcase the newly selected player colors.
        /// </summary>
        private void SetComparedToPlayerColorBoxes(int presetID)
        {
            if (AllColorPalettePresets.Count <= presetID)
            {
                Debug.WriteLine("Index for compared to colors is too big");
                return;
            }
            // Convert vector values to .NET framework color values and showcase them in UI.
            var converter = new BrushConverter();

            var bluePlayer = FindName("BlueComparedToColor") as Rectangle;
            bluePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].BluePlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].BluePlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].BluePlayerColor[2]).ToString("X2"));

            var redPlayer = FindName("RedComparedToColor") as Rectangle;
            redPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].RedPlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].RedPlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].RedPlayerColor[2]).ToString("X2"));

            var yellowPlayer = FindName("YellowComparedToColor") as Rectangle;
            yellowPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].YellowPlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].YellowPlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].YellowPlayerColor[2]).ToString("X2"));

            var brownPlayer = FindName("BrownComparedToColor") as Rectangle;
            brownPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].BrownPlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].BrownPlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].BrownPlayerColor[2]).ToString("X2"));

            var orangePlayer = FindName("OrangeComparedToColor") as Rectangle;
            orangePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].OrangePlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].OrangePlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].OrangePlayerColor[2]).ToString("X2"));

            var greenPlayer = FindName("GreenComparedToColor") as Rectangle;
            greenPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].GreenPlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].GreenPlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].GreenPlayerColor[2]).ToString("X2"));

            var purplePlayer = FindName("PurpleComparedToColor") as Rectangle;
            purplePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].PurplePlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].PurplePlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].PurplePlayerColor[2]).ToString("X2"));

            var tealPlayer = FindName("TealComparedToColor") as Rectangle;
            tealPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)AllColorPalettePresets[presetID].TealPlayerColor[0]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].TealPlayerColor[1]).ToString("X2") +
                ((byte)AllColorPalettePresets[presetID].TealPlayerColor[2]).ToString("X2"));

            Debug.WriteLine("UI compared to player colors updated.");
            UserPreferences.SaveUserPreferences();
        }

        /// <summary>
        /// <br>Updates the users newly selected "compared to player colors".</br>
        /// <br>Updates the UI, and saves the currently active selection to the user preferences.</br>
        /// </summary>
        private void UpdateComparedToPlayerColorDropDownChoices(int presetID)
        {
            var presetsComboBox = FindName("ComparedToColorSelection");
            ((System.Windows.Controls.ComboBox)presetsComboBox).Items.Clear();

            foreach (PalettePresetJSON currentColorPreset in AllColorPalettePresets)
            {
                _ = ((System.Windows.Controls.ComboBox)presetsComboBox).Items.Add(currentColorPreset.PresetName);
            }

            ((System.Windows.Controls.ComboBox)presetsComboBox).SelectedIndex = presetID;
            Debug.WriteLine("Preset list in compared to drop down updated.");

            UserPreferences.ActiveComparedToPalette = presetID;
            UserPreferences.SaveUserPreferences();
        }

        /// <summary>
        /// <br>Updates the compared to player colors drop down to showcase the new player color choices.</br>
        /// <br>Call to this after user saves or deletes a color preset.</br>
        /// </summary>
        private void RefreshComparedToPlayerColorsDropwDown()
        {
            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            int toBesetIndex = presetSelection.SelectedIndex;

            if (AllColorPalettePresets.Count <= toBesetIndex)
            {
                Debug.WriteLine("Index too big for comparison drop down, reseted to index 1");
                UserPreferences.ActiveComparedToPalette = 1;
                UpdateComparedToPlayerColorDropDownChoices(1);
                SetComparedToPlayerColorBoxes(1);
            }
            else
            {
                UserPreferences.ActiveComparedToPalette = toBesetIndex;
                UpdateComparedToPlayerColorDropDownChoices(toBesetIndex);
                SetComparedToPlayerColorBoxes(toBesetIndex);
            }
        }

        private void LocateColorPalettes_Click(object sender, RoutedEventArgs e)
        {
            var findPaletteFolder = new FolderBrowserDialog();

            for (int i = 0; i < PaletteFolderDefaultLocations.Length; i++)
            {
                if (Directory.Exists(PaletteFolderDefaultLocations[i]))
                {
                    findPaletteFolder.SelectedPath = PaletteFolderDefaultLocations[i];
                    Debug.WriteLine("Palette location found automatically.");
                    break;
                }
            }

            var browseFileResult = findPaletteFolder.ShowDialog();

            if (browseFileResult == System.Windows.Forms.DialogResult.OK)
            {
                UserPreferences.PlayerColorPaletteLocation = findPaletteFolder.SelectedPath;
                Debug.WriteLine("Palette location changed. New palette location is:");
                Debug.WriteLine(UserPreferences.PlayerColorPaletteLocation);
            }

            findPaletteFolder.Dispose();
            UpdateUIColorPalettesFolderLocation();
            UserPreferences.SaveUserPreferences();
        }

        public void UpdateUIColorPalettesFolderLocation()
        {
            var colorPalettePathText = FindName("ColorPalettesLocation") as TextBlock;

            colorPalettePathText.Text = UserPreferences.PlayerColorPaletteLocation;
            Debug.WriteLine("UI Palette location string updated.");
        }

        ///<!-- Player color presets selection -->
        private void ColorPresetDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Can't get index from the selected ComboBox item, has to use the ComboBox from the XAML to get access to the index.
            var presetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex != -1)
            {
                Debug.WriteLine("New preset selected from combo box, with index: " + presetSelection.SelectedIndex + ".");
                UpdateDataToSelectedPreseset(presetSelection.SelectedIndex);
                ShowNewlySelectedColors();
            }
        }

        /// <summary>
        /// <para>Used to load the color preset after it has been selected from the drop down.</para>
        /// <br>Updates both; the UI and the code.</br>
        /// <br>In UI disables/enables "save" and "delete" preset buttons depending
        /// whether if the currently selected preset is one of the default presets.</br>
        /// <br>One the code side; loads the color presets colors into "NewPlayerColors" array.</br>
        /// <br>Updates user preferences.</br>
        /// </summary>
        private void UpdateDataToSelectedPreseset(int currentlyActivePresetIndex)
        {
            if (currentlyActivePresetIndex < AllColorPalettePresets.Count && currentlyActivePresetIndex >= 0)
            {
                CurrentlyActivePlayerColors[0] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].BluePlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].BluePlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].BluePlayerColor[2]);

                CurrentlyActivePlayerColors[1] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].RedPlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].RedPlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].RedPlayerColor[2]);

                CurrentlyActivePlayerColors[2] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].YellowPlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].YellowPlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].YellowPlayerColor[2]);

                CurrentlyActivePlayerColors[3] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].BrownPlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].BrownPlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].BrownPlayerColor[2]);

                CurrentlyActivePlayerColors[4] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].OrangePlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].OrangePlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].OrangePlayerColor[2]);

                CurrentlyActivePlayerColors[5] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].GreenPlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].GreenPlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].GreenPlayerColor[2]);

                CurrentlyActivePlayerColors[6] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].PurplePlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].PurplePlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].PurplePlayerColor[2]);

                CurrentlyActivePlayerColors[7] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].TealPlayerColor[0],
                    AllColorPalettePresets[currentlyActivePresetIndex].TealPlayerColor[1],
                    AllColorPalettePresets[currentlyActivePresetIndex].TealPlayerColor[2]);

                Debug.WriteLine("Newly selected color updated to the Vector3[] NewPlayerColors.");

                UserPreferences.ActivePlayerColorPalette = currentlyActivePresetIndex;
                UserPreferences.SaveUserPreferences();
            }
            else
            {
                Debug.WriteLine("Tried to update player colors 'NewPlayerColors', " +
                    "but given index number for player palettes was too big.");
                Debug.WriteLine("Given index number was: " + currentlyActivePresetIndex + 
                    ", and the count of palette presets is: " + AllColorPalettePresets.Count + ".");
            }

            // If currently selected preset is one of the default presets then disable save and delete buttons.
            if (FindName("SavePreset") is System.Windows.Controls.Button savePresetButton)
            {
                var deletePresetButton = FindName("DeletePreset") as System.Windows.Controls.Button;
                savePresetButton.IsEnabled = currentlyActivePresetIndex >= CountOfUnchangeableColorPresets;
                deletePresetButton.IsEnabled = currentlyActivePresetIndex >= CountOfUnchangeableColorPresets;
            }
        }

        /// <summary>
        /// <br>Updates the UI to reflect users selected player colors.</br>
        /// <br>Call this after user picks a player color preset from the combo box.</br>
        /// </summary>
        private void UpdateCurrenltyActivePresetComboBox(int activePresetIndex)
        {
            var presetsComboBox = FindName("PresetSelectionDropdown");

            ((System.Windows.Controls.ComboBox)presetsComboBox).Items.Clear();

            foreach (PalettePresetJSON currentColorPreset in AllColorPalettePresets)
            {
                _ = ((System.Windows.Controls.ComboBox)presetsComboBox).Items.Add(currentColorPreset.PresetName);
            }

            ((System.Windows.Controls.ComboBox)presetsComboBox).SelectedIndex = activePresetIndex;
            Debug.WriteLine("Preset list in ComboBox updated.");
        }

        /// <summary>
        /// Updates the "Your Edit" player colors shown in the UI.
        /// </summary>
        private void ShowNewlySelectedColors()
        {
            // Convert vector values to .NET framework color values and showcase them in UI.
            var converter = new BrushConverter();

            var bluePlayer = FindName("BluePlayerColor") as Rectangle;
            bluePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[0].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[0].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[0].Z).ToString("X2"));

            var redPlayer = FindName("RedPlayerColor") as Rectangle;
            redPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[1].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[1].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[1].Z).ToString("X2"));

            var yellowPlayer = FindName("YellowPlayerColor") as Rectangle;
            yellowPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[2].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[2].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[2].Z).ToString("X2"));

            var brownPlayer = FindName("BrownPlayerColor") as Rectangle;
            brownPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[3].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[3].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[3].Z).ToString("X2"));

            var orangePlayer = FindName("OrangePlayerColor") as Rectangle;
            orangePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[4].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[4].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[4].Z).ToString("X2"));

            var greenPlayer = FindName("GreenPlayerColor") as Rectangle;
            greenPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[5].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[5].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[5].Z).ToString("X2"));

            var purplePlayer = FindName("PurplePlayerColor") as Rectangle;
            purplePlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[6].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[6].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[6].Z).ToString("X2"));

            var tealPlayer = FindName("TealPlayerColor") as Rectangle;
            tealPlayer.Fill = (Brush)converter.ConvertFromString("#FF" +
                ((byte)CurrentlyActivePlayerColors[7].X).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[7].Y).ToString("X2") +
                ((byte)CurrentlyActivePlayerColors[7].Z).ToString("X2"));

            Debug.WriteLine("UI player colors updated.");
        }

        /// <summary>
        /// Saves current player colors into currently active player color preset.
        /// Can only be used if currently active color preset isn't one of the default color presets.
        /// </summary>
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            var presetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int currentPreset = presetSelection.SelectedIndex;

            AllColorPalettePresets[currentPreset].BluePlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[0].X,
                (int)CurrentlyActivePlayerColors[0].Y,
                (int)CurrentlyActivePlayerColors[0].Z
            };

            AllColorPalettePresets[currentPreset].RedPlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[1].X,
                (int)CurrentlyActivePlayerColors[1].Y,
                (int)CurrentlyActivePlayerColors[1].Z
            };

            AllColorPalettePresets[currentPreset].YellowPlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[2].X,
                (int)CurrentlyActivePlayerColors[2].Y,
                (int)CurrentlyActivePlayerColors[2].Z
            };

            AllColorPalettePresets[currentPreset].BrownPlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[3].X,
                (int)CurrentlyActivePlayerColors[3].Y,
                (int)CurrentlyActivePlayerColors[3].Z
            };

            AllColorPalettePresets[currentPreset].OrangePlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[4].X,
                (int)CurrentlyActivePlayerColors[4].Y,
                (int)CurrentlyActivePlayerColors[4].Z
            };

            AllColorPalettePresets[currentPreset].GreenPlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[5].X,
                (int)CurrentlyActivePlayerColors[5].Y,
                (int)CurrentlyActivePlayerColors[5].Z
            };

            AllColorPalettePresets[currentPreset].PurplePlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[6].X,
                (int)CurrentlyActivePlayerColors[6].Y,
                (int)CurrentlyActivePlayerColors[6].Z
            };

            AllColorPalettePresets[currentPreset].TealPlayerColor = new int[] {
                (int)CurrentlyActivePlayerColors[7].X,
                (int)CurrentlyActivePlayerColors[7].Y,
                (int)CurrentlyActivePlayerColors[7].Z
            };

            PalettePresets.SaveColorPresetsToDisk();
            RefreshComparedToPlayerColorsDropwDown();
            UpdateCustomConsole("Saved palette preset");
            Debug.WriteLine("Saved palette preset.");
        }

        /// <summary>
        /// Opens pop up asking a name for the new preset.
        /// </summary>
        private void SavePresetAs_Click(object sender, RoutedEventArgs e)
        {
            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Visible;

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = false;

            var currentlySelectedPreset = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;

            var newPresetName = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;
            newPresetName.Text = AllColorPalettePresets[currentlySelectedPreset.SelectedIndex].PresetName;

            Debug.WriteLine("Save preset as pop up opened.");
        }

        /// <summary>
        /// Closes the pop up asking a name for new preset without creating new preset.
        /// </summary>
        private void CancelNewPresetCreation_Click(object sender, RoutedEventArgs e)
        {
            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            Debug.WriteLine("Save preset as pop up closed.");
        }

        /// <summary>
        /// Creates new color preset from "NewPlayerColors" array.
        /// </summary>
        private void AcceptNewPresetName_Click(object sender, RoutedEventArgs e)
        {
            var presetNameString = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;

            // Add new color preset to the "AllPalettePresets" list and save it to the JSON file.
            var newPreset = new PalettePresetJSON
            {
                PresetName = presetNameString.Text,

                BluePlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[0].X,
                    (int)CurrentlyActivePlayerColors[0].Y,
                    (int)CurrentlyActivePlayerColors[0].Z
                },
                RedPlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[1].X,
                    (int)CurrentlyActivePlayerColors[1].Y,
                    (int)CurrentlyActivePlayerColors[1].Z
                },
                YellowPlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[2].X,
                    (int)CurrentlyActivePlayerColors[2].Y,
                    (int)CurrentlyActivePlayerColors[2].Z
                },
                BrownPlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[3].X,
                    (int)CurrentlyActivePlayerColors[3].Y,
                    (int)CurrentlyActivePlayerColors[3].Z
                },

                OrangePlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[4].X,
                    (int)CurrentlyActivePlayerColors[4].Y,
                    (int)CurrentlyActivePlayerColors[4].Z
                },
                GreenPlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[5].X,
                    (int)CurrentlyActivePlayerColors[5].Y,
                    (int)CurrentlyActivePlayerColors[5].Z
                },
                PurplePlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[6].X,
                    (int)CurrentlyActivePlayerColors[6].Y,
                    (int)CurrentlyActivePlayerColors[6].Z
                },
                TealPlayerColor = new int[] {
                    (int)CurrentlyActivePlayerColors[7].X,
                    (int)CurrentlyActivePlayerColors[7].Y,
                    (int)CurrentlyActivePlayerColors[7].Z
                },
            };

            AllColorPalettePresets.Add(newPreset);

            PalettePresets.SaveColorPresetsToDisk();
            Debug.WriteLine("Created new palette preset.");

            // Update UI to reflect all changes.
            UpdateCurrenltyActivePresetComboBox(AllColorPalettePresets.Count - 1);

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            RefreshComparedToPlayerColorsDropwDown();
            UpdateCustomConsole("Created new palette preset");
        }

        /// <summary>
        /// Opens a pop up menu confirming or declining the preset deletion.
        /// </summary>
        private void DeletePreset_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Visible;

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = false;

            Debug.WriteLine("Delete preset pop up opened.");
        }

        /// <summary>
        /// Closes the "delete preset" pop up without deleting the presets.
        /// </summary>
        private void CancelRemoval_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            Debug.WriteLine("Delete preset pop up closed.");
        }

        /// <summary>
        /// Closes the "delete preset" pop up and deletes the presets.
        /// </summary>
        private void AcceptRemoval_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            // Get the index of currently selected preset and remove it from the list.
            var presetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;

            AllColorPalettePresets.RemoveAt(presetSelection.SelectedIndex);

            PalettePresets.SaveColorPresetsToDisk();
            Debug.WriteLine("Player color preset deleted.");

            UpdateCurrenltyActivePresetComboBox(0);
            ShowNewlySelectedColors();
            RefreshComparedToPlayerColorsDropwDown();
            UpdateCustomConsole("Deleted palette preset");
        }

        private void CreateColors_Click(object sender, RoutedEventArgs e)
        {
            ColorPaletteCreation.CreateColors(CurrentlyActivePlayerColors);
            UpdateCustomConsole("Created the color palettes");
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            WindowSizer.UserChangedWindowSize();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            WindowSizer.UserChangedWindowSize();
        }
    }

    /// <summary>
    /// <br>Records changes on windows location and saves the values into user preferences.</br>
    /// <br>Users can only scale the window in fixed ratio.</br>
    /// </summary>
    public static class WindowSizer
    {
        public static double DefaultLeft;
        public static double DefaultTop;
        public static double DefaultWidth;
        public static double DefaultHeight;

        private static double WidthRatio;
        private static double HeightRatio;

        public static bool TimerIsRunning = false;
        public static bool TimerNeedsToBeRefreshed = false;

        public static void InitializeWindowSizer()
        {
            DefaultWidth = System.Windows.Application.Current.MainWindow.MinWidth;
            DefaultHeight = System.Windows.Application.Current.MainWindow.MinHeight;

            DefaultLeft = System.Windows.Application.Current.MainWindow.Left;
            DefaultTop = System.Windows.Application.Current.MainWindow.Top;

            WidthRatio = DefaultWidth / DefaultHeight;
            HeightRatio = DefaultHeight / DefaultWidth;
        }

        public static void UserChangedWindowSize()
        {
            double width = System.Windows.Application.Current.MainWindow.Width;
            double height = System.Windows.Application.Current.MainWindow.Height;

            if (width * DefaultWidth > height * DefaultHeight)
            {
                System.Windows.Application.Current.MainWindow.Width = width;
                System.Windows.Application.Current.MainWindow.Height = width * HeightRatio;
            }
            else
            {
                System.Windows.Application.Current.MainWindow.Height = height;
                System.Windows.Application.Current.MainWindow.Width = height * WidthRatio;
            }

            DefaultLeft = System.Windows.Application.Current.MainWindow.Left;
            DefaultTop = System.Windows.Application.Current.MainWindow.Top;

            // Timer for saving user preferences.
            // If timer is active only refreshes it.
            if (TimerIsRunning)
            {
                TimerNeedsToBeRefreshed = true;
            }
            else
            {
                TimerIsRunning = true;
                Debug.WriteLine("User started adjusting the windows size.");
                StartTimer();
            }
        }

        public static async void StartTimer()
        {
            await Task.Delay(200);
            while (TimerNeedsToBeRefreshed)
            {
                TimerNeedsToBeRefreshed = false;
                await Task.Delay(400);
            }

            TimerIsRunning = false;
            Debug.WriteLine("User stopped adjusting the windows size.");
            UserPreferences.SaveUserPreferences();
        }
    }

    /// <summary>
    /// JSON coded "User Preferences" object.
    /// </summary>
    public class UserPreferencesJSON
    {
        public string PaletteLocation { get; set; }
        public int ActiveColorPalette { get; set; }
        public int ActiveComparedTo { get; set; }
        public int WindowsWidth { get; set; }
        public int WindowsHeight { get; set; }
        public int WindowsLeft { get; set; }
        public int WindowsTop { get; set; }
    }

    /// <summary>
    /// Save and load user preferences stored in UserPreferences.json file.
    /// </summary>
    public static class UserPreferences
    {
        public static string PlayerColorPaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";
        public static int ActivePlayerColorPalette = 0;
        public static int ActiveComparedToPalette = 1;

        private static readonly string UserPreferenceFileLocation = Directory.GetCurrentDirectory() + @"\UserPreferences.json";

        /// <summary>
        /// <br>Loads user preferences from disk to memory.</br>
        /// <br>If no preferences are found creates the file with default settings.</br>
        /// </summary>
        public static void LoadUserPreferences()
        {
            Debug.WriteLine("Started loading preferences.");

            if (File.Exists(UserPreferenceFileLocation))
            {
                // Load user preferences.
                string loadedPreferences = File.ReadAllText(UserPreferenceFileLocation);

                // Convert JSON string back to object.
                var loadedPreferencesAsObject = System.Text.Json.JsonSerializer.Deserialize<UserPreferencesJSON>(loadedPreferences);

                PlayerColorPaletteLocation = loadedPreferencesAsObject.PaletteLocation;
                ActivePlayerColorPalette = loadedPreferencesAsObject.ActiveColorPalette;
                ActiveComparedToPalette = loadedPreferencesAsObject.ActiveComparedTo;
                System.Windows.Application.Current.MainWindow.Width = loadedPreferencesAsObject.WindowsWidth;
                System.Windows.Application.Current.MainWindow.Height = loadedPreferencesAsObject.WindowsHeight;
                System.Windows.Application.Current.MainWindow.Left = loadedPreferencesAsObject.WindowsLeft;
                System.Windows.Application.Current.MainWindow.Top = loadedPreferencesAsObject.WindowsTop;
                Debug.WriteLine("Previous user preference file found and loaded.");
            }
            else
            {
                Debug.WriteLine("No user preference file found.");
                System.Windows.Application.Current.MainWindow.Width = WindowSizer.DefaultWidth;
                System.Windows.Application.Current.MainWindow.Height = WindowSizer.DefaultHeight;
                ActiveComparedToPalette = 1;

                var newPreferences = new UserPreferencesJSON
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette,
                    ActiveComparedTo = ActiveComparedToPalette,
                    WindowsWidth = (int)WindowSizer.DefaultWidth,
                    WindowsHeight = (int)WindowSizer.DefaultHeight,
                    WindowsLeft = (int)WindowSizer.DefaultLeft,
                    WindowsTop = (int)WindowSizer.DefaultTop
                };

                File.WriteAllText(UserPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));

                Debug.WriteLine("New user preferences created.");
            }
        }

        /// <summary>
        /// <br>Locates all the values to be saved.</br>
        /// <br>Always overwrites the whole JSON regardless how many actual values were changed.</br>
        /// </summary>
        public static void SaveUserPreferences()
        {
            var newPreferences = new UserPreferencesJSON
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette,
                ActiveComparedTo = ActiveComparedToPalette,
                WindowsWidth = (int)System.Windows.Application.Current.MainWindow.Width,
                WindowsHeight = (int)System.Windows.Application.Current.MainWindow.Height,
                WindowsLeft = (int)WindowSizer.DefaultLeft,
                WindowsTop = (int)WindowSizer.DefaultTop
            };

            File.WriteAllText(UserPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));

            Debug.WriteLine("User preferences saved.");
        }
    }

    /// <summary>
    /// JSON coded "Player Color Palette Preset" object.
    /// </summary>
    public class PalettePresetJSON
    {
        public string PresetName { get; set; }

        public int[] BluePlayerColor { get; set; }
        public int[] RedPlayerColor { get; set; }
        public int[] YellowPlayerColor { get; set; }
        public int[] BrownPlayerColor { get; set; }
        public int[] OrangePlayerColor { get; set; }
        public int[] GreenPlayerColor { get; set; }
        public int[] PurplePlayerColor { get; set; }
        public int[] TealPlayerColor { get; set; }
    }

    /// <summary>
    /// <para>Saves and loads palette presets.</para>
    /// <br>All presets are stored in JSON file.</br>
    /// <br>All editor presets are created here.</br>
    /// <br>Creates 3 presets on initialization if no presets are detected.</br>
    /// </summary>
    public static class PalettePresets
    {
        private static readonly string PlayerColorPresetFileLocation = 
            Directory.GetCurrentDirectory() + @"\PlayerColorPresets.json";

        private static IEnumerable<T> DeserializeObjects<T>(string input)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using StringReader strreader = new StringReader(input);
            using JsonTextReader jsonreader = new JsonTextReader(strreader);
            jsonreader.SupportMultipleContent = true;
            while (jsonreader.Read())
            {
                yield return serializer.Deserialize<T>(jsonreader);
            }
        }

        public static async void SaveColorPresetsToDisk()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < MainWindow.AllColorPalettePresets.Count; i++)
            {
                jsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(
                    MainWindow.AllColorPalettePresets[i], options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileLocation, jsonTextToWriteInTheFile);

            Debug.WriteLine("Preset JSON created.");
        }

        /// <summary>
        /// Loads palette presets from JSON file into memory.
        /// Creates the palette presets file if none is found.
        /// </summary>
        public static void InitializePresets()
        {
            if (File.Exists(PlayerColorPresetFileLocation))
            {
                MainWindow.AllColorPalettePresets = DeserializeObjects<PalettePresetJSON>(
                    File.ReadAllText(PlayerColorPresetFileLocation)).ToList();

                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else
            {
                var editorDefaultPlayerColors = new PalettePresetJSON
                {
                    PresetName = "Editor Default",

                    BluePlayerColor = new int[] { 15, 70, 245 },
                    RedPlayerColor = new int[] { 220, 35, 35 },
                    YellowPlayerColor = new int[] { 215, 215, 30 },
                    BrownPlayerColor = new int[] { 115, 60, 0 },

                    OrangePlayerColor = new int[] { 245, 135, 25 },
                    GreenPlayerColor = new int[] { 4, 165, 20 },
                    PurplePlayerColor = new int[] { 210, 55, 200 },
                    TealPlayerColor = new int[] { 126, 242, 225 }
                };
                var gameDefaultPlayerColors = new PalettePresetJSON
                {
                    PresetName = "AOE:DE Default",

                    BluePlayerColor = new int[] { 45, 45, 245 },
                    RedPlayerColor = new int[] { 210, 40, 40 },
                    YellowPlayerColor = new int[] { 215, 215, 30 },
                    BrownPlayerColor = new int[] { 142, 91, 0 },

                    OrangePlayerColor = new int[] { 255, 150, 5 },
                    GreenPlayerColor = new int[] { 4, 165, 20 },
                    PurplePlayerColor = new int[] { 150, 15, 250 },
                    TealPlayerColor = new int[] { 126, 242, 225 }
                };
                var highContrastPlayerColors = new PalettePresetJSON
                {
                    PresetName = "High Contrast",

                    BluePlayerColor = new int[] { 15, 70, 245 },
                    RedPlayerColor = new int[] { 235, 18, 18 },
                    YellowPlayerColor = new int[] { 240, 240, 20 },
                    BrownPlayerColor = new int[] { 95, 50, 0 },

                    OrangePlayerColor = new int[] { 255, 150, 5 },
                    GreenPlayerColor = new int[] { 4, 165, 20 },
                    PurplePlayerColor = new int[] { 250, 30, 245 },
                    TealPlayerColor = new int[] { 103, 252, 252 }
                };

                MainWindow.AllColorPalettePresets.Add(editorDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(gameDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(highContrastPlayerColors);

                SaveColorPresetsToDisk();

                Debug.WriteLine("No Preset JSON found, new presets JSON file created and loaded into memory.");
            }
            Debug.WriteLine("Current number of palette presets: " + MainWindow.AllColorPalettePresets.Count + ".");
        }
    }

    /// <summary>
    /// <br>Creates new player color palettes for Age of Empires Definitive Edition.</br>
    /// <br>Call "CreateColors" function with all the player colors as a parameter.</br>
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
    /// Write it on the document. (use "ColorCodeSeperator" and "RGBColorSeperator" variables to separate each value.)
    /// Get the first value of "InterpolatingIntoColors" array, and linearly interpolate into that value.
    /// 16 numbers interpolated in total, if counting starting and ending values.
    /// Do the same for each value within "InterpolatingIntoColors" array.
    public static class ColorPaletteCreation
    {
        private static readonly string PaletteStartingText = 
            "JASC-PAL" + 
            Environment.NewLine + "0100" + 
            Environment.NewLine + "256";

        private static readonly string ColorCodeSeperator = " ";
        private static readonly string RGBColorSeperator = Environment.NewLine;

        // Total of this +1 colors if counting both starting and ending colors.
        private const int ColorInterpolationCount = 15; 

        private static readonly Vector3[] InterpolatingIntoColors = {
            new Vector3(0, 0, 0),
            new Vector3(32, 32, 32),
            new Vector3(64, 64, 64),
            new Vector3(128, 128, 128),
            new Vector3(192, 192, 192),
            new Vector3(224, 224, 224),
            new Vector3(255, 255, 255),
            new Vector3(128, 96, 64)
        };

        private static readonly string[] PaletteNames = {
            "playercolor_blue.pal",
            "playercolor_red.pal",
            "playercolor_yellow.pal",
            "playercolor_brown.pal",
            "playercolor_orange.pal",
            "playercolor_green.pal",
            "playercolor_purple.pal",
            "playercolor_teal.pal"
        };

        /// <summary>
        /// <br>Running this once creates all 8 player color palettes.</br>
        /// <br>The palette location is stored in the player preferences.</br>
        /// </summary>
        /// <param name="playerColors">Holds 8 player colors.</param>
        public static void CreateColors(Vector3[] playerColors)
        {
            static async void CreatePlayerColorPalette(Vector3 playerColor, string paletteName)
            {
                string textToWriteInPaletteFile = "";

                textToWriteInPaletteFile += PaletteStartingText;

                // Going through the target colors.
                foreach (Vector3 colorToInterpolateInto in InterpolatingIntoColors)
                {
                    // Each step in interpolation.
                    for (int i = 0; i <= ColorInterpolationCount; i++)
                    {
                        textToWriteInPaletteFile += RGBColorSeperator;

                        // Count the interpolation based on "i", "ColorToInterpolateInto" and "playerColor" variables.
                        // Go from "playerColor" to "ColorToInterpolateInto", do this linearly.
                        textToWriteInPaletteFile += Math.Round(
                            (playerColor.X * (ColorInterpolationCount - i) / ColorInterpolationCount) + 
                            (colorToInterpolateInto.X * i / ColorInterpolationCount));

                        textToWriteInPaletteFile += ColorCodeSeperator;
                        textToWriteInPaletteFile += Math.Round(
                            (playerColor.Y * (ColorInterpolationCount - i) / ColorInterpolationCount) + 
                            (colorToInterpolateInto.Y * i / ColorInterpolationCount));

                        textToWriteInPaletteFile += ColorCodeSeperator;
                        textToWriteInPaletteFile += Math.Round(
                            (playerColor.Z * (ColorInterpolationCount - i) / ColorInterpolationCount) + 
                            (colorToInterpolateInto.Z * i / ColorInterpolationCount));
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

                await File.WriteAllTextAsync(UserPreferences.PlayerColorPaletteLocation + 
                    @"\" + paletteName, textToWriteInPaletteFile);
            };

            ///<!-- Logic Starts Here -->
            if (Directory.Exists(UserPreferences.PlayerColorPaletteLocation))
            {
                for (int i = 0; i < 8; i++)
                {
                    File.Delete(UserPreferences.PlayerColorPaletteLocation + @"\" + PaletteNames[i]);
                }
                Debug.WriteLine("Previous player colors palettes removed");
            }
            else
            {
                _ = Directory.CreateDirectory(UserPreferences.PlayerColorPaletteLocation);
                Debug.WriteLine("No player color palette folder found, new player color palette folder created.");
            }

            for (int i = 0; i < 8; i++)
            {
                CreatePlayerColorPalette(playerColors[i], PaletteNames[i]);
            }
            Debug.WriteLine("All player colors created.");
        }
    }
}