using System;
using System.Linq;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

/// <summary>
/// Creates new player color palettes for Age of Empires Definitive edition.
/// Saves the files in Palettes folder.
/// Has presets allowing easy color swaps between different color schemes.
/// </summary>
/// 
/// <!-- TODO: Add Action Info Screen -->
///  -Remove the pop up on palette creation.
/// Action info screen works like a console.
/// Hold 4 lines of info, light gray font color.
/// Writes in the action screen whenever you:
///  -Create new palettes
///  -Save palette preset
///  -Create new palette preset
///  -Delete palette preset
///  
/// <!-- TODO: Add Info Button -->
/// Small button, on click/hover showcase info on what the editor does and how to use it.
/// Showcased info:
///  -How and where to locate the palette folder.
///  -Where palettes are saved if the folder is not located.
///  -How the color editing works (color picker).
///  -Tell about ability to change colors even when in game (at main menu).
///
/// <!-- TODO: UI Rework -->
/// Player colors now showcase all the shades of player color and not only the main colors.
/// All objects now scale based on windows size.
/// Create blue button boxes.
/// Add brown background with a rough image. Remember to edit all pop ups.


namespace PlayerColorsWithWpf
{
    /// <summary>
    /// <br>Handles both UI and code related changes.</br>
    /// <br>This class holds the global entry point.</br>
    /// </summary>
    public partial class MainWindow : Window
    {
        ///<!-- Main(args[]) -->

        /// <summary>
        /// <br>All color presets are stored in this list.</br>
        /// <br>Always append the new preset to this list before calling "CreatePlayerPresetJSON()".</br>
        /// </summary>
        public static List<PlayerColorPreset> AllPalettePresets = new List<PlayerColorPreset>();

        /// <summary>
        /// This is the currently active color scheme across the whole project.
        /// </summary>
        public static readonly Vector3[] NewPlayerColors = { new Vector3(15, 70, 245), new Vector3(220, 35, 35), new Vector3(215, 215, 30), new Vector3(115, 60, 0), new Vector3(245, 135, 25), new Vector3(4, 165, 20), new Vector3(245, 95, 240), new Vector3(65, 245, 230) };

        /// <summary>
        /// <br>Editor default and game default color schemes shouldn't be overwritten.</br>
        /// <br>Users should do their own colors schemes with "Save as" button.</br>
        /// </summary>
        public static readonly int CountOfUnchangeablePresets = 3;

        //Default locations for color palettes. Used when looking for color palettes folder.
        public static readonly string[] SteamPaletteFolderLocations = { @"C:\Program Files (x86)\Steam\steamapps\common\AoEDE\Assets\Palettes", @"D:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes", @"E:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes", @"C:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes", @"F:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes" };

        //Color picker when user is changing player colors.
        private readonly ColorDialog PlayerColorPicker = new ColorDialog();
        private System.Drawing.Color GainedColor;

        public MainWindow() //Initialize everything through here, this ensures all folders and actions are created after the UI is loaded.
        {
            InitializeComponent();
            UserPreferences.LoadPreferences();
            PalettePresets.InitializePresets();
            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(UserPreferences.ActivePlayerColorPalette, true);
            ShowNewlySelectedColors();
            SetUIPathToPalettes();
            Debug.WriteLine("Program booted successfully.");
        }
        ///<!-- End of Main(args[]) -->


        ///<!-- Change colors in "NewPlayerColors" -->
        ///All Color boxes under "Your Edits text".
        ///Clicking these color boxes opens up a color picker.
        ///Each time the color picker is closed this script updates the 
        ///"NewPlayerColors" array and showcases these changes in the UI.

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
        /// <br>Allows user to select a new color. That color will be updated to the "NewPlayerColors" array.</br>
        /// <para>This script calls "ShowNewlySelectedColors()" in the end to automatically update the UI.</para>
        /// </summary>
        /// <param name="selectedPlayerColor">Which color from "NewPlayerColors" array will be replaced. 
        /// <br>Use index value.</br></param>
        private void OpenColorPicker(int selectedPlayerColor)
        {
            Debug.WriteLine("Color picker opened with player color id: " + selectedPlayerColor);
            PlayerColorPicker.Color = System.Drawing.ColorTranslator.FromHtml("#FF" + ((byte)NewPlayerColors[selectedPlayerColor].X).ToString("X2") + ((byte)NewPlayerColors[selectedPlayerColor].Y).ToString("X2") + ((byte)NewPlayerColors[selectedPlayerColor].Z).ToString("X2"));

            _ = PlayerColorPicker.ShowDialog();
            GainedColor = PlayerColorPicker.Color;

            NewPlayerColors[selectedPlayerColor] = new Vector3(GainedColor.R, GainedColor.G, GainedColor.B);
            Debug.WriteLine("New player color picked by the user.");
            ShowNewlySelectedColors();
        }
        ///<!-- End of change colors in "NewPlayerColors" -->


        ///<!-- Change color palette location -->
        ///Locate color palettes folder.
        ///If no folder is located a new folder is created in the same directory as this executable.

        /// <summary>
        /// Opens folder searcher. Uses windows default search tool.
        /// </summary>
        private void LocateColorPalettes_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog FindPaletteFolder = new FolderBrowserDialog();

            for (int i = 0; i < SteamPaletteFolderLocations.Length; i++)
            {
                if (Directory.Exists(SteamPaletteFolderLocations[i]))
                {
                    FindPaletteFolder.SelectedPath = SteamPaletteFolderLocations[i];
                    Debug.WriteLine("Palette location found automatically.");
                    break;
                }
            }

            DialogResult BrowseFileResult = FindPaletteFolder.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == BrowseFileResult)
            {
                UserPreferences.PlayerColorPaletteLocation = FindPaletteFolder.SelectedPath;
                Debug.WriteLine("Palette location changed. New palette location is:");
                Debug.WriteLine(UserPreferences.PlayerColorPaletteLocation);
            }
            SetUIPathToPalettes();
            UserPreferences.SavePlayerPreferences();
            FindPaletteFolder.Dispose();
        }

        /// <summary>
        /// Only updates the UI palette location string.
        /// </summary>
        public void SetUIPathToPalettes()
        {
            TextBlock ColorPalettePathText = FindName("ColorPalettesLocation") as TextBlock;

            ColorPalettePathText.Text = UserPreferences.PlayerColorPaletteLocation;
            Debug.WriteLine("UI Palette location string updated.");
        }
        ///<!-- End of change color palette location -->


        ///<!-- Delete Preset button -->
        ///Used to delete user created color palettes presets.
        ///Has a pop up asking confirmation before deleting the preset.

        /// <summary>
        /// Opens a pop up menu confirming or declining the preset deletion.
        /// </summary>
        private void DeletePreset_Click(object sender, RoutedEventArgs e)
        {
            StackPanel DeletePopup = FindName("PresetDeletePopUp") as StackPanel;
            DeletePopup.Visibility = Visibility.Visible;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = false;
            Debug.WriteLine("Delete preset pop up opened.");
        }

        /// <summary>
        /// Closes the "delete preset" pop up without deleting the presets.
        /// </summary>
        private void CancelRemoval_Click(object sender, RoutedEventArgs e)
        {
            StackPanel DeletePopup = FindName("PresetDeletePopUp") as StackPanel;
            DeletePopup.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;
            Debug.WriteLine("Delete preset pop up closed.");
        }

        /// <summary>
        /// Closes the "delete preset" pop up and deletes the presets.
        /// </summary>
        private void AcceptRemoval_Click(object sender, RoutedEventArgs e)
        {
            StackPanel DeletePopup = FindName("PresetDeletePopUp") as StackPanel;
            DeletePopup.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;

            //Get the index of currently selected preset and remove it from the list.
            System.Windows.Controls.ComboBox PresetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int ToBeRemovedPreset = PresetSelection.SelectedIndex;

            AllPalettePresets.RemoveAt(ToBeRemovedPreset);

            PalettePresets.CreatePlayerPresetJSON();
            Debug.WriteLine("Player color preset deleted.");

            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(0, true);
            ShowNewlySelectedColors();
        }
        ///<!-- End of delete presets button -->


        ///<!-- Save presets button -->
        /// <summary>
        /// Save current player colors into currently active player color preset.
        /// Can only be used if currently active color preset isn't one of the default color presets.
        /// </summary>
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ComboBox PresetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int CurrentlySelected = PresetSelection.SelectedIndex;

            AllPalettePresets[CurrentlySelected].BluePlayerColor = new int[] { (int)NewPlayerColors[0].X, (int)NewPlayerColors[0].Y, (int)NewPlayerColors[0].Z };
            AllPalettePresets[CurrentlySelected].RedPlayerColor = new int[] { (int)NewPlayerColors[1].X, (int)NewPlayerColors[1].Y, (int)NewPlayerColors[1].Z };
            AllPalettePresets[CurrentlySelected].YellowPlayerColor = new int[] { (int)NewPlayerColors[2].X, (int)NewPlayerColors[2].Y, (int)NewPlayerColors[2].Z };
            AllPalettePresets[CurrentlySelected].BrownPlayerColor = new int[] { (int)NewPlayerColors[3].X, (int)NewPlayerColors[3].Y, (int)NewPlayerColors[3].Z };

            AllPalettePresets[CurrentlySelected].OrangePlayerColor = new int[] { (int)NewPlayerColors[4].X, (int)NewPlayerColors[4].Y, (int)NewPlayerColors[4].Z };
            AllPalettePresets[CurrentlySelected].GreenPlayerColor = new int[] { (int)NewPlayerColors[5].X, (int)NewPlayerColors[5].Y, (int)NewPlayerColors[5].Z };
            AllPalettePresets[CurrentlySelected].PurplePlayerColor = new int[] { (int)NewPlayerColors[6].X, (int)NewPlayerColors[6].Y, (int)NewPlayerColors[6].Z };
            AllPalettePresets[CurrentlySelected].TealPlayerColor = new int[] { (int)NewPlayerColors[7].X, (int)NewPlayerColors[7].Y, (int)NewPlayerColors[7].Z };

            PalettePresets.CreatePlayerPresetJSON();
            Debug.WriteLine("Saved palette preset.");
        }
        ///<!-- End of Save presets button -->


        ///<!-- Save presets As button -->
        ///Used to create a new color palette presets.

        /// <summary>
        /// Opens pop up asking a name for the new preset.
        /// </summary>
        private void SavePresetAs_Click(object sender, RoutedEventArgs e)
        {
            StackPanel PresetNameBox = FindName("PresetNamePopUp") as StackPanel;
            PresetNameBox.Visibility = Visibility.Visible;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = false;

            System.Windows.Controls.ComboBox CurrentlySelectedPreset = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int currentlySelectedPresetIndex = CurrentlySelectedPreset.SelectedIndex;

            System.Windows.Controls.TextBox NewPresetName = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;
            NewPresetName.Text = AllPalettePresets[currentlySelectedPresetIndex].PresetName;
            Debug.WriteLine("Save preset as pop up opened.");
        }

        /// <summary>
        /// Closes the pop up asking a name for new preset without creating new preset.
        /// </summary>
        private void CancelNewPresetCreation_Click(object sender, RoutedEventArgs e)
        {
            StackPanel PresetNameBox = FindName("PresetNamePopUp") as StackPanel;
            PresetNameBox.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;
            Debug.WriteLine("Save preset as pop up closed.");
        }

        /// <summary>
        /// Creates a new color preset from "NewPlayerColors" array.
        /// </summary>
        private void AcceptNewPresetName_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox PresetNameString = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;
            string UserWrittenName = PresetNameString.Text;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;

            //Add new color preset to the "AllPalettePresets" list and save it to the JSON file.
            PlayerColorPreset NewPreset = new PlayerColorPreset
            {
                PresetName = UserWrittenName,

                BluePlayerColor = new int[] { (int)NewPlayerColors[0].X, (int)NewPlayerColors[0].Y, (int)NewPlayerColors[0].Z },
                RedPlayerColor = new int[] { (int)NewPlayerColors[1].X, (int)NewPlayerColors[1].Y, (int)NewPlayerColors[1].Z },
                YellowPlayerColor = new int[] { (int)NewPlayerColors[2].X, (int)NewPlayerColors[2].Y, (int)NewPlayerColors[2].Z },
                BrownPlayerColor = new int[] { (int)NewPlayerColors[3].X, (int)NewPlayerColors[3].Y, (int)NewPlayerColors[3].Z },

                OrangePlayerColor = new int[] { (int)NewPlayerColors[4].X, (int)NewPlayerColors[4].Y, (int)NewPlayerColors[4].Z },
                GreenPlayerColor = new int[] { (int)NewPlayerColors[5].X, (int)NewPlayerColors[5].Y, (int)NewPlayerColors[5].Z },
                PurplePlayerColor = new int[] { (int)NewPlayerColors[6].X, (int)NewPlayerColors[6].Y, (int)NewPlayerColors[6].Z },
                TealPlayerColor = new int[] { (int)NewPlayerColors[7].X, (int)NewPlayerColors[7].Y, (int)NewPlayerColors[7].Z },
            };

            AllPalettePresets.Add(NewPreset);
            PalettePresets.CreatePlayerPresetJSON();
            Debug.WriteLine("Created new palette preset.");

            //Update UI to reflect all changes.
            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(AllPalettePresets.Count - 1, true);

            StackPanel PresetNameBox = FindName("PresetNamePopUp") as StackPanel;
            PresetNameBox.Visibility = Visibility.Collapsed;
        }
        ///<!-- End of Save presets As button -->


        ///<summary>
        ///Used to create all color palettes from currently active color scheme.
        ///</summary>
        private void CreateColors_Click(object sender, RoutedEventArgs e)
        {
            PlayerColorsPalettes.CreateColors(NewPlayerColors);
        }

        /// <summary>
        ///Loading presets from the drop down, be careful; this gets called in InitializePresets().
        /// </summary>
        private void PresetDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Can't get index from the selected ComboBox item, has to use the ComboBox from the XAML to get access to the index.
            System.Windows.Controls.ComboBox PresetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;

            int CurrentlySelected = PresetSelection.SelectedIndex;
            Debug.WriteLine("Your currently selected preset has index of: " + CurrentlySelected + ".");

            UpdateDataToSelectedPreseset(CurrentlySelected, false);
            ShowNewlySelectedColors();
        }

        /// <summary>
        /// <br>Only refreshes the list visible in the UI.</br>
        /// <br>Uses "AllPalettePresets" array for the palette names.</br>
        /// </summary>
        private void UpdatePresetComboBoxDropdownChoices()
        {
            object PresetsComboBox = FindName("PresetSelectionDropdown");

            ((System.Windows.Controls.ComboBox)PresetsComboBox).Items.Clear();

            foreach (PlayerColorPreset currentColorPreset in AllPalettePresets)
            {
                _ = ((System.Windows.Controls.ComboBox)PresetsComboBox).Items.Add(currentColorPreset.PresetName);
            }
            Debug.WriteLine("Preset list in ComboBox updated.");
        }

        /// <summary>
        /// <para>Used to load the color preset after it has been selected from the drop down.</para>
        /// <br>Updates both; the UI and the code.</br> 
        /// <br>In UI disables/enables "save" and "delete" preset buttons depending
        /// whether if the currently selected preset is one of the default presets.</br>
        /// <br>One the code side; loads the color presets colors into "NewPlayerColors" array.</br> 
        /// <br>Updates user preferences.</br> 
        /// </summary>
        /// <param name="selectedPresetIndex">Which preset should be set as the currently active color preset.</param>
        /// <param name="updateListDropdown"><br>Set false if called from the drop down.</br>
        /// <br>If leaved to true when called from the drop down this function will call itself infinitely.</br>
        /// <br>Use true when not called from the drop down to update the drop down list.</br></param>
        private void UpdateDataToSelectedPreseset(int selectedPresetIndex, bool updateListDropdown)
        {
            if (updateListDropdown)
            {
                Debug.WriteLine("Updating PresetSelectionDropdown from \"UpdateDataToSelectedPreseset\" function.");
                object PresetsComboBox = FindName("PresetSelectionDropdown");
                ((System.Windows.Controls.ComboBox)PresetsComboBox).SelectedIndex = selectedPresetIndex;

                //Stop the script here. This way the player preferences are not saved twice.
                return;
            }
            else if (selectedPresetIndex == -1)
            {
                //This function gets called when item is destroyed from the drop down list,
                //and it happens before the drop down list has been set to 0, which causes errors and double calls.
                return;
            }

            if (selectedPresetIndex < AllPalettePresets.Count && selectedPresetIndex >= 0)
            {
                NewPlayerColors[0] = new Vector3(AllPalettePresets[selectedPresetIndex].BluePlayerColor[0], AllPalettePresets[selectedPresetIndex].BluePlayerColor[1], AllPalettePresets[selectedPresetIndex].BluePlayerColor[2]);
                NewPlayerColors[1] = new Vector3(AllPalettePresets[selectedPresetIndex].RedPlayerColor[0], AllPalettePresets[selectedPresetIndex].RedPlayerColor[1], AllPalettePresets[selectedPresetIndex].RedPlayerColor[2]);
                NewPlayerColors[2] = new Vector3(AllPalettePresets[selectedPresetIndex].YellowPlayerColor[0], AllPalettePresets[selectedPresetIndex].YellowPlayerColor[1], AllPalettePresets[selectedPresetIndex].YellowPlayerColor[2]);
                NewPlayerColors[3] = new Vector3(AllPalettePresets[selectedPresetIndex].BrownPlayerColor[0], AllPalettePresets[selectedPresetIndex].BrownPlayerColor[1], AllPalettePresets[selectedPresetIndex].BrownPlayerColor[2]);
                NewPlayerColors[4] = new Vector3(AllPalettePresets[selectedPresetIndex].OrangePlayerColor[0], AllPalettePresets[selectedPresetIndex].OrangePlayerColor[1], AllPalettePresets[selectedPresetIndex].OrangePlayerColor[2]);
                NewPlayerColors[5] = new Vector3(AllPalettePresets[selectedPresetIndex].GreenPlayerColor[0], AllPalettePresets[selectedPresetIndex].GreenPlayerColor[1], AllPalettePresets[selectedPresetIndex].GreenPlayerColor[2]);
                NewPlayerColors[6] = new Vector3(AllPalettePresets[selectedPresetIndex].PurplePlayerColor[0], AllPalettePresets[selectedPresetIndex].PurplePlayerColor[1], AllPalettePresets[selectedPresetIndex].PurplePlayerColor[2]);
                NewPlayerColors[7] = new Vector3(AllPalettePresets[selectedPresetIndex].TealPlayerColor[0], AllPalettePresets[selectedPresetIndex].TealPlayerColor[1], AllPalettePresets[selectedPresetIndex].TealPlayerColor[2]);

                Debug.WriteLine("Newly selected color updated to the Vector3[] NewPlayerColors.");

                UserPreferences.ActivePlayerColorPalette = selectedPresetIndex;
                UserPreferences.SavePlayerPreferences();
            }
            else
            {
                Debug.WriteLine("Tried to update player colors 'NewPlayerColors', but given index number for player palettes was too big.");
                Debug.WriteLine("Given index number was: " + selectedPresetIndex + ", and the count of palette presets is: " + AllPalettePresets.Count + ".");
            }

            //If currently selected preset is not one of the default presets then enable save and delete buttons.
            //Otherwise disable the save and delete buttons.
            if (FindName("SavePreset") is System.Windows.Controls.Button SavePresetButton)
            {
                System.Windows.Controls.Button DeletePresetButton = FindName("DeletePreset") as System.Windows.Controls.Button;
                SavePresetButton.IsEnabled = selectedPresetIndex >= CountOfUnchangeablePresets;
                DeletePresetButton.IsEnabled = selectedPresetIndex >= CountOfUnchangeablePresets;
            }
        }

        /// <summary>
        /// Updates the "Your Edit" player colors shown in the UI.
        /// </summary>
        private void ShowNewlySelectedColors()
        {
            //Convert vector values to .NET framework color values and showcase them in UI.
            BrushConverter Converter = new BrushConverter();

            Rectangle BluePlayer = FindName("BluePlayerColor") as Rectangle;
            BluePlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[0].X).ToString("X2") + ((byte)NewPlayerColors[0].Y).ToString("X2") + ((byte)NewPlayerColors[0].Z).ToString("X2"));

            Rectangle RedPlayer = FindName("RedPlayerColor") as Rectangle;
            RedPlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[1].X).ToString("X2") + ((byte)NewPlayerColors[1].Y).ToString("X2") + ((byte)NewPlayerColors[1].Z).ToString("X2"));

            Rectangle YellowPlayer = FindName("YellowPlayerColor") as Rectangle;
            YellowPlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[2].X).ToString("X2") + ((byte)NewPlayerColors[2].Y).ToString("X2") + ((byte)NewPlayerColors[2].Z).ToString("X2"));

            Rectangle BrownPlayer = FindName("BrownPlayerColor") as Rectangle;
            BrownPlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[3].X).ToString("X2") + ((byte)NewPlayerColors[3].Y).ToString("X2") + ((byte)NewPlayerColors[3].Z).ToString("X2"));

            Rectangle OrangePlayer = FindName("OrangePlayerColor") as Rectangle;
            OrangePlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[4].X).ToString("X2") + ((byte)NewPlayerColors[4].Y).ToString("X2") + ((byte)NewPlayerColors[4].Z).ToString("X2"));

            Rectangle GreenPlayer = FindName("GreenPlayerColor") as Rectangle;
            GreenPlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[5].X).ToString("X2") + ((byte)NewPlayerColors[5].Y).ToString("X2") + ((byte)NewPlayerColors[5].Z).ToString("X2"));

            Rectangle PurplePlayer = FindName("PurplePlayerColor") as Rectangle;
            PurplePlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[6].X).ToString("X2") + ((byte)NewPlayerColors[6].Y).ToString("X2") + ((byte)NewPlayerColors[6].Z).ToString("X2"));

            Rectangle TealPlayer = FindName("TealPlayerColor") as Rectangle;
            TealPlayer.Fill = (Brush)Converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[7].X).ToString("X2") + ((byte)NewPlayerColors[7].Y).ToString("X2") + ((byte)NewPlayerColors[7].Z).ToString("X2"));

            Debug.WriteLine("UI player colors updated.");
        }
    }


    /// <summary>
    /// JSON coded "Player Preferences" object.
    /// </summary>
    public class UserPreferencesJSON
    {
        public string PaletteLocation { get; set; }
        public int ActiveColorPalette { get; set; }
    }

    /// <summary>
    /// Save and load player preferences stored in UserPreferences.json file.
    /// </summary>
    public static class UserPreferences
    {
        public static string PlayerColorPaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";
        public static int ActivePlayerColorPalette = 0;

        private static readonly string PlayerPreferenceFileLocation = Directory.GetCurrentDirectory() + @"\UserPreferences.json";

        /// <summary>
        /// <br>Loads user preferences from disk to memory.</br>
        /// <br>If no preferences are found creates the file with default settings.</br>
        /// </summary>
        public static void LoadPreferences()
        {
            Debug.WriteLine("Started loading preferences.");

            if (File.Exists(PlayerPreferenceFileLocation))
            {
                //Load player preferences.
                string LoadedPreferences = File.ReadAllText(PlayerPreferenceFileLocation);

                //Convert JSON string back to object.
                UserPreferencesJSON LoadedPreferencesAsObject = System.Text.Json.JsonSerializer.Deserialize<UserPreferencesJSON>(LoadedPreferences);

                PlayerColorPaletteLocation = LoadedPreferencesAsObject.PaletteLocation;
                ActivePlayerColorPalette = LoadedPreferencesAsObject.ActiveColorPalette;
                Debug.WriteLine("Previous user preference file found and loaded.");

            }
            else //Create new player preferences if one doesn't exist.
            {
                Debug.WriteLine("No user preference file found.");
                UserPreferencesJSON NewPreferences = new UserPreferencesJSON
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette
                };

                File.WriteAllText(PlayerPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(NewPreferences));
                Debug.WriteLine("New user preferences created.");
            }
        }

        /// <summary>
        /// Saves player preferences to the disk.
        /// </summary>
        public static void SavePlayerPreferences()
        {
            UserPreferencesJSON NewPreferences = new UserPreferencesJSON
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette
            };

            File.WriteAllText(PlayerPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(NewPreferences));
            Debug.WriteLine("User preferences saved.");
        }
    }


    /// <summary>
    /// JSON coded "Player Color Preset" object.
    /// </summary>
    public class PlayerColorPreset
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
        /// <summary>
        /// Holds full path to the player color presets JSON file.
        /// </summary>
        private static readonly string PlayerColorPresetFileName = Directory.GetCurrentDirectory() + @"\PlayerColorPresets.json";

        /// <summary>
        /// Used to load JSON presets into memory.
        /// </summary>
        private static IEnumerable<T> DeserializeObjects<T>(string input)
        {
            Newtonsoft.Json.JsonSerializer Serializer = new Newtonsoft.Json.JsonSerializer();
            using StringReader Strreader = new StringReader(input);
            using JsonTextReader Jsonreader = new JsonTextReader(Strreader);
            Jsonreader.SupportMultipleContent = true;
            while (Jsonreader.Read())
            {
                yield return Serializer.Deserialize<T>(Jsonreader);
            }
        }
        /// <summary>
        /// Saves all player color presets into the disk.
        /// </summary>
        public static async void CreatePlayerPresetJSON()
        {
            JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };
            string JsonTextToWriteInTheFile = "";

            for (int i = 0; i < MainWindow.AllPalettePresets.Count; i++)
            {
                JsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(MainWindow.AllPalettePresets[i], Options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileName, JsonTextToWriteInTheFile);
            Debug.WriteLine("Preset JSON created.");
        }

        /// <summary>
        /// Loads palette presets from JSON file into memory.
        /// Creates the JSON file with default values if it doesn't exist.
        /// </summary>
        public static void InitializePresets()
        {
            if (File.Exists(PlayerColorPresetFileName))
            {
                MainWindow.AllPalettePresets = DeserializeObjects<PlayerColorPreset>(File.ReadAllText(PlayerColorPresetFileName)).ToList();
                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else //Create the default JSON file.
            {
                PlayerColorPreset EditorDefaultPlayerColors = new PlayerColorPreset
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
                PlayerColorPreset GameDefaultPlayerColors = new PlayerColorPreset
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
                PlayerColorPreset HighContrastPlayerColors = new PlayerColorPreset
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

                MainWindow.AllPalettePresets.Add(EditorDefaultPlayerColors);
                MainWindow.AllPalettePresets.Add(GameDefaultPlayerColors);
                MainWindow.AllPalettePresets.Add(HighContrastPlayerColors);

                CreatePlayerPresetJSON();

                Debug.WriteLine("No Preset JSON found, new presets JSON file created and loaded into memory.");
            }
            Debug.WriteLine("Current number of palette presets: " + MainWindow.AllPalettePresets.Count + ".");
        }
    }


    /// <summary>
    /// <br>Creates new player color palettes for Age of Empires Definitive Edition.</br>
    /// <br>Call "CreateColors" function with all the player colors as a parameter.</br>
    /// </summary>
    /// Creates a total of 8 color palettes, one for each player.
    /// 
    /// File names are listed in "PaletteNames" array.
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
    public static class PlayerColorsPalettes
    {
        /// <summary>
        /// <br>Running this once creates all 8 player color palettes.</br>
        /// <br>The palette location is saved in the player preferences.</br>
        /// </summary>
        /// <param name="playerColors">Holds 8 player colors.</param>
        public static void CreateColors(Vector3[] playerColors)
        {
            string PaletteStartingText = "JASC-PAL" + Environment.NewLine + "0100" + Environment.NewLine + "256";

            string ColorCodeSeperator = " ";
            string RGBColorSeperator = Environment.NewLine;

            int ColorInterpolateCount = 15; //Total of 16 colors if counting both starting and ending colors.

            Vector3[] InterpolatingIntoColors = { new Vector3(0, 0, 0), new Vector3(32, 32, 32), new Vector3(64, 64, 64), new Vector3(128, 128, 128), new Vector3(192, 192, 192), new Vector3(224, 224, 224), new Vector3(255, 255, 255), new Vector3(128, 96, 64) };

            string[] PaletteNames = { "playercolor_blue.pal", "playercolor_red.pal", "playercolor_yellow.pal", "playercolor_brown.pal", "playercolor_orange.pal", "playercolor_green.pal", "playercolor_purple.pal", "playercolor_teal.pal" };

            /// <summary>
            /// Creates and saves one player color palette file.
            /// </summary>
            /// <param name="playerColor">Holds a single color.</param>
            /// <param name="paletteName">Holds the palette name.</param>
            async void CreatePlayerColorPalette(Vector3 playerColor, string paletteName)
            {
                string TextToWriteInPaletteFile = "";

                TextToWriteInPaletteFile += PaletteStartingText;

                //Going through the target colors.
                foreach (Vector3 colorToInterpolateInto in InterpolatingIntoColors)
                {
                    //Each step in interpolation.
                    for (int i = 0; i <= ColorInterpolateCount; i++)
                    {
                        TextToWriteInPaletteFile += RGBColorSeperator;

                        //Count the interpolation based on "i", "ColorToInterpolateInto" and "playerColor" variables.
                        //Go from "playerColor" to "ColorToInterpolateInto", do this linearly.
                        TextToWriteInPaletteFile += Math.Round((playerColor.X * (ColorInterpolateCount - i) / ColorInterpolateCount) + (colorToInterpolateInto.X * i / ColorInterpolateCount));
                        TextToWriteInPaletteFile += ColorCodeSeperator;
                        TextToWriteInPaletteFile += Math.Round((playerColor.Y * (ColorInterpolateCount - i) / ColorInterpolateCount) + (colorToInterpolateInto.Y * i / ColorInterpolateCount));
                        TextToWriteInPaletteFile += ColorCodeSeperator;
                        TextToWriteInPaletteFile += Math.Round((playerColor.Z * (ColorInterpolateCount - i) / ColorInterpolateCount) + (colorToInterpolateInto.Z * i / ColorInterpolateCount));
                    }
                }

                //Fill in "0 0 0" line to get a total of 256 lines in color data.
                for (int i = 0; i < 128; i++)
                {
                    TextToWriteInPaletteFile += RGBColorSeperator;
                    TextToWriteInPaletteFile += "0 0 0";
                }

                //Original palette files had an empty line in the end.
                //Which means this file should also end with an empty line.
                TextToWriteInPaletteFile += RGBColorSeperator;

                await File.WriteAllTextAsync(UserPreferences.PlayerColorPaletteLocation + @"\" + paletteName, TextToWriteInPaletteFile);
            };

            ///<!-- Logic Starts Here -->
            if (Directory.Exists(UserPreferences.PlayerColorPaletteLocation))
            {
                //Only remove the player color palettes.
                for (int i = 0; i < 8; i++)
                {
                    File.Delete(UserPreferences.PlayerColorPaletteLocation + @"\" + PaletteNames[i]);
                }
                Debug.WriteLine("Previous player colors palettes removed");
            }
            else
            {
                _ = Directory.CreateDirectory(UserPreferences.PlayerColorPaletteLocation);
                Debug.WriteLine("No player color palette folder found, new player colors palette folder created.");
            }

            for (int i = 0; i < 8; i++)
            {
                CreatePlayerColorPalette(playerColors[i], PaletteNames[i]);
            }

            _ = System.Windows.Forms.MessageBox.Show("New Player Colors Created.");
            Debug.WriteLine("All player colors created.");
        }
    }
}