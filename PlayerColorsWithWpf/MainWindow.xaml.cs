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
/// Can be placed inside the AoE folder, making it faster and easies to alter colors.
/// Has presets allowing easy color swaps between different color schemes.
/// </summary>
/// 
/// <!-- TODO: User Preferences -->
/// Add lastly used palette. Used in boot up to know which color preset to load.
/// 
/// <!-- TODO: Add Action Info Screen -->
///  -Remove the pop up on palette creation.
/// Action info screen works like a console.
/// Hold 3 lines of info, light gray font color.
/// Writes in the action screen whenever you:
///  -Create new palettes
///  -Save palette preset
///  -Create new palette preset
///  -Delete palette preset
///  
/// <!-- TODO: Add Info Button -->
/// Small button, on click/on hover showcase info on what the editor does and how to use this editor.
/// Shows info:
///  -How and where to locate the palette folder.
///  -Where palettes are saved if the folder is not located.
///  -How the color editing works (color picker).
///  -Tell about ability to change colors even when in game (at main menu).
///
/// <!-- TODO: UI Rework -->
/// Player colors now showcase all the shades of player color and not only the main colors.
/// All objects now scale based on windows size.
/// Use blue button boxes
/// Add brown background with a rough image. Remember to edit all pop ups.


namespace PlayerColorsWithWpf
{
    /// <summary>
    /// Handles UI related changes.
    /// This class hold the global entry point.
    /// </summary>
    public partial class MainWindow : Window
    {
        ///<!-- Main(args[]) -->
        public PalettePresets palettePresetHandling; //Give object reference, don't play around with static fields too much.

        //Use this to find  and edit palette presets across the whole project.
        public static List<PlayerColorPreset> AllPalettePresets = new List<PlayerColorPreset>();

        //This is the currently active color scheme across the whole project.
        public static readonly Vector3[] NewPlayerColors = { new Vector3(15, 70, 245), new Vector3(220, 35, 35), new Vector3(215, 215, 30), new Vector3(115, 60, 0), new Vector3(245, 135, 25), new Vector3(4, 165, 20), new Vector3(245, 95, 240), new Vector3(65, 245, 230) };

        //Editor default and game default color schemes shouldn't be changed by user.
        //Users should do their own colors schemes with "Save as" button.
        public static readonly int CountOfUnchangeablePresets = 3;

        public static string SteamPaletteFolderLocation = @"C:\Program Files (x86)\Steam\steamapps\common\AoEDE\Assets\Palettes";
        public static string SteamPaletteFolderLocation2 = @"D:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes";
        public static string SteamPaletteFolderLocation3 = @"E:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes";

        public MainWindow() //Initialize everything through here. Ensures all folders and actions are created after assets are loaded.
        {
            InitializeComponent();
            UserPreferences.LoadPreferences();
            palettePresetHandling = new PalettePresets();
            palettePresetHandling.InitializePresets();
            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(UserPreferences.ActivePlayerColorPalette, true);
            ShowNewlySelectedColors();
            SetUIPathToPalettes();
        }
        ///<!-- End of Main(args[]) -->


        ///<!-- Change colors in "NewPlayerColors" -->
        private int CurrentlySelectedPlayerColor = 0;
        private readonly ColorDialog PlayerColorPicker = new ColorDialog();
        private System.Drawing.Color GainedColor;

        private void BluePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 0;
            OpenColorPicker();
        }
        private void RedPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 1;
            OpenColorPicker();
        }
        private void YellowPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 2;
            OpenColorPicker();
        }
        private void BrownPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 3;
            OpenColorPicker();
        }
        private void OrangePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 4;
            OpenColorPicker();
        }
        private void GreenPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 5;
            OpenColorPicker();
        }
        private void PurplePlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 6;
            OpenColorPicker();
        }
        private void TealPlayer_MouseDown(object sender, RoutedEventArgs e)
        {
            CurrentlySelectedPlayerColor = 7;
            OpenColorPicker();
        }

        private void OpenColorPicker()
        {
            PlayerColorPicker.Color = System.Drawing.ColorTranslator.FromHtml("#FF" + ((byte)NewPlayerColors[CurrentlySelectedPlayerColor].X).ToString("X2") + ((byte)NewPlayerColors[CurrentlySelectedPlayerColor].Y).ToString("X2") + ((byte)NewPlayerColors[CurrentlySelectedPlayerColor].Z).ToString("X2"));

            _ = PlayerColorPicker.ShowDialog();
            GainedColor = PlayerColorPicker.Color;

            NewPlayerColors[CurrentlySelectedPlayerColor] = new Vector3(GainedColor.R, GainedColor.G, GainedColor.B);
            ShowNewlySelectedColors();
        }
        ///<!-- End of change colors in "NewPlayerColors" -->


        ///<!-- Change color palette location -->
        private void LocateColorPalettes_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog findPaletteFolder = new FolderBrowserDialog();

            if (Directory.Exists(SteamPaletteFolderLocation))
            {
                findPaletteFolder.SelectedPath = SteamPaletteFolderLocation;
            }
            else if (Directory.Exists(SteamPaletteFolderLocation2))
            {
                findPaletteFolder.SelectedPath = SteamPaletteFolderLocation2;
            }
            else if (Directory.Exists(SteamPaletteFolderLocation3))
            {
                findPaletteFolder.SelectedPath = SteamPaletteFolderLocation3;
            }

            DialogResult BrowseFileResult = findPaletteFolder.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == BrowseFileResult)
            {
                UserPreferences.PlayerColorPaletteLocation = findPaletteFolder.SelectedPath;
            }
            SetUIPathToPalettes();
            UserPreferences.UpdatePlayerPreferences();
            findPaletteFolder.Dispose();
        }

        public void SetUIPathToPalettes()
        {
            TextBlock colorPalettePathText = FindName("ColorPalettesLocation") as TextBlock;

            colorPalettePathText.Text = UserPreferences.PlayerColorPaletteLocation;
        }
        ///<!-- End of change color palette location -->


        ///<!-- Delete Preset button -->
        private void DeletePreset_Click(object sender, RoutedEventArgs e)
        {
            //Open sub menu confirming or declining the deletion
            StackPanel deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Visible;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = false;
        }
        private void CancelRemoval_Click(object sender, RoutedEventArgs e)
        {
            //Close sub menu confirming or declining the deletion
            StackPanel deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;
        }
        private void AcceptRemoval_Click(object sender, RoutedEventArgs e)
        {
            //Close sub menu confirming or declining the deletion
            StackPanel deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;

            //Get the index of currently selected preset and Remove it from the list.
            System.Windows.Controls.ComboBox PresetSelection = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int toBeRemovedPreset = PresetSelection.SelectedIndex;

            AllPalettePresets.RemoveAt(toBeRemovedPreset);

            palettePresetHandling.CreatePlayerPresetJSON();

            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(0, true);
            ShowNewlySelectedColors();
        }
        ///<!-- end of Delete presets button -->


        ///<!-- Save presets button -->
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            //Get the index of currently selected preset and overwrite the colors in it.

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

            palettePresetHandling.CreatePlayerPresetJSON();
        }
        ///<!-- end of Save presets button -->


        ///<!-- Save presets As button -->
        private void SavePresetAs_Click(object sender, RoutedEventArgs e)//Opens the pop up asking a name for new preset.
        {
            StackPanel presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Visible;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = false;

            System.Windows.Controls.ComboBox currentlySelectedPreset = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            int currentlySelectedPresetIndex = currentlySelectedPreset.SelectedIndex;

            System.Windows.Controls.TextBox newPresetName = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;
            newPresetName.Text = AllPalettePresets[currentlySelectedPresetIndex].PresetName;
        }
        private void CancelNewPresetCreation_Click(object sender, RoutedEventArgs e)//Closes the pop up asking a name for new preset.
        {
            StackPanel presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;
        }
        private void AcceptNewPresetName_Click(object sender, RoutedEventArgs e)//Creates a new preset.
        {
            System.Windows.Controls.TextBox presetNameString = FindName("NewPresetNameInput") as System.Windows.Controls.TextBox;
            string userWrittenName = presetNameString.Text;

            System.Windows.Controls.ComboBox PresetDropdown = FindName("PresetSelectionDropdown") as System.Windows.Controls.ComboBox;
            PresetDropdown.IsEnabled = true;

            //Add new color to the "AllPalettePresets" list and save it to the JSON file.
            PlayerColorPreset newPlayerColors = new PlayerColorPreset
            {
                PresetName = userWrittenName,

                BluePlayerColor = new int[] { (int)NewPlayerColors[0].X, (int)NewPlayerColors[0].Y, (int)NewPlayerColors[0].Z },
                RedPlayerColor = new int[] { (int)NewPlayerColors[1].X, (int)NewPlayerColors[1].Y, (int)NewPlayerColors[1].Z },
                YellowPlayerColor = new int[] { (int)NewPlayerColors[2].X, (int)NewPlayerColors[2].Y, (int)NewPlayerColors[2].Z },
                BrownPlayerColor = new int[] { (int)NewPlayerColors[3].X, (int)NewPlayerColors[3].Y, (int)NewPlayerColors[3].Z },

                OrangePlayerColor = new int[] { (int)NewPlayerColors[4].X, (int)NewPlayerColors[4].Y, (int)NewPlayerColors[4].Z },
                GreenPlayerColor = new int[] { (int)NewPlayerColors[5].X, (int)NewPlayerColors[5].Y, (int)NewPlayerColors[5].Z },
                PurplePlayerColor = new int[] { (int)NewPlayerColors[6].X, (int)NewPlayerColors[6].Y, (int)NewPlayerColors[6].Z },
                TealPlayerColor = new int[] { (int)NewPlayerColors[7].X, (int)NewPlayerColors[7].Y, (int)NewPlayerColors[7].Z },
            };
            AllPalettePresets.Add(newPlayerColors);
            palettePresetHandling.CreatePlayerPresetJSON();

            //Update UI to reflect all changes.
            UpdatePresetComboBoxDropdownChoices();
            UpdateDataToSelectedPreseset(AllPalettePresets.Count - 1, true);

            StackPanel presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;
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
            Debug.WriteLine("Your current preset has index of: " + CurrentlySelected + ".");

            UpdateDataToSelectedPreseset(CurrentlySelected, false);
            ShowNewlySelectedColors();
        }

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

        private void UpdateDataToSelectedPreseset(int selectedPresetIndex, bool updateListDropdown)
        {
            if (updateListDropdown)
            {
                object PresetsComboBox = FindName("PresetSelectionDropdown");
                ((System.Windows.Controls.ComboBox)PresetsComboBox).SelectedIndex = selectedPresetIndex;
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
            }
            else
            {
                Debug.WriteLine("Tried to update player colors 'NewPlayerColors', but given index number for player palettes was too big.");
                Debug.WriteLine("Given index number was: " + selectedPresetIndex + ", and the count of palette presets is: " + AllPalettePresets.Count + ".");
            }

            //If currently selected preset is not one of the default presets.
            if (FindName("SavePreset") is System.Windows.Controls.Button savePresetButton)
            {
                System.Windows.Controls.Button deletePresetButton = FindName("DeletePreset") as System.Windows.Controls.Button;
                savePresetButton.IsEnabled = selectedPresetIndex >= CountOfUnchangeablePresets;
                deletePresetButton.IsEnabled = selectedPresetIndex >= CountOfUnchangeablePresets;
            }
        }

        private void ShowNewlySelectedColors()
        {
            //Convert vector values to .NET framework color values and showcase them in UI.
            BrushConverter converter = new BrushConverter();

            Rectangle BluePlayer = FindName("BluePlayerColor") as Rectangle;
            BluePlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[0].X).ToString("X2") + ((byte)NewPlayerColors[0].Y).ToString("X2") + ((byte)NewPlayerColors[0].Z).ToString("X2"));

            Rectangle RedPlayer = FindName("RedPlayerColor") as Rectangle;
            RedPlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[1].X).ToString("X2") + ((byte)NewPlayerColors[1].Y).ToString("X2") + ((byte)NewPlayerColors[1].Z).ToString("X2"));

            Rectangle YellowPlayer = FindName("YellowPlayerColor") as Rectangle;
            YellowPlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[2].X).ToString("X2") + ((byte)NewPlayerColors[2].Y).ToString("X2") + ((byte)NewPlayerColors[2].Z).ToString("X2"));

            Rectangle BrownPlayer = FindName("BrownPlayerColor") as Rectangle;
            BrownPlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[3].X).ToString("X2") + ((byte)NewPlayerColors[3].Y).ToString("X2") + ((byte)NewPlayerColors[3].Z).ToString("X2"));

            Rectangle OrangePlayer = FindName("OrangePlayerColor") as Rectangle;
            OrangePlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[4].X).ToString("X2") + ((byte)NewPlayerColors[4].Y).ToString("X2") + ((byte)NewPlayerColors[4].Z).ToString("X2"));

            Rectangle GreenPlayer = FindName("GreenPlayerColor") as Rectangle;
            GreenPlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[5].X).ToString("X2") + ((byte)NewPlayerColors[5].Y).ToString("X2") + ((byte)NewPlayerColors[5].Z).ToString("X2"));

            Rectangle PurplePlayer = FindName("PurplePlayerColor") as Rectangle;
            PurplePlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[6].X).ToString("X2") + ((byte)NewPlayerColors[6].Y).ToString("X2") + ((byte)NewPlayerColors[6].Z).ToString("X2"));

            Rectangle TealPlayer = FindName("TealPlayerColor") as Rectangle;
            TealPlayer.Fill = (Brush)converter.ConvertFromString("#FF" + ((byte)NewPlayerColors[7].X).ToString("X2") + ((byte)NewPlayerColors[7].Y).ToString("X2") + ((byte)NewPlayerColors[7].Z).ToString("X2"));

            Debug.WriteLine("Main Windows player colors updated.");
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
    /// Save and load player preferences.
    /// Saved in UserPreferences.json file.
    /// </summary>
    public class UserPreferences
    {
        public static string PlayerColorPaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";
        public static int ActivePlayerColorPalette = 0;

        private static readonly string PlayerPreferenceFileLocation = Directory.GetCurrentDirectory() + @"\UserPreferences.json";

        public static void LoadPreferences()
        {
            Debug.WriteLine("Started loading preferences.");

            if (File.Exists(PlayerPreferenceFileLocation))
            {
                //Load player preferences
                string loadedPreferences = File.ReadAllText(PlayerPreferenceFileLocation);

                //convert JSON string back to object
                UserPreferencesJSON loadedPreferencesAsObject = System.Text.Json.JsonSerializer.Deserialize<UserPreferencesJSON>(loadedPreferences);

                PlayerColorPaletteLocation = loadedPreferencesAsObject.PaletteLocation;
                ActivePlayerColorPalette = loadedPreferencesAsObject.ActiveColorPalette;

            }
            else //Create new player preferences if one doesn't exist.
            {
                UserPreferencesJSON newPreferences = new UserPreferencesJSON
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette
                };

                File.WriteAllText(PlayerPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));
            }
        }

        /// <summary>
        /// Called from system locate folder.
        /// </summary>
        public static void UpdatePlayerPreferences()
        {
            UserPreferencesJSON newPreferences = new UserPreferencesJSON
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette
            };

            File.WriteAllText(PlayerPreferenceFileLocation, System.Text.Json.JsonSerializer.Serialize(newPreferences));
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
    /// Saves and loads palette presets.
    /// All presets are stored in JSON file.
    /// All editor presets are created here.
    /// Creates 3 presets on start up if no presets are detected.
    /// </summary>
    public class PalettePresets
    {
        private readonly string PlayerColorPresetFileName = Directory.GetCurrentDirectory() + @"\PlayerColorPresets.json";

        /// <summary>
        /// Used to load JSON presets into memory.
        /// </summary>
        private static IEnumerable<T> DeserializeObjects<T>(string input)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using StringReader strreader = new StringReader(input);
            using JsonTextReader jsonreader = new JsonTextReader(strreader);
            jsonreader.SupportMultipleContent = true;
            while (jsonreader.Read())
            {
                yield return serializer.Deserialize<T>(jsonreader);
            }
        }
        public async void CreatePlayerPresetJSON()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < MainWindow.AllPalettePresets.Count; i++)
            {
                jsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(MainWindow.AllPalettePresets[i], options);
            }

            await File.WriteAllTextAsync(PlayerColorPresetFileName, jsonTextToWriteInTheFile);
            Debug.WriteLine("Preset JSON created.");
        }

        /// <summary>
        /// Loads palette presets from JSON file into memory.
        /// Creates the JSON file with default values if it doesn't exist.
        /// </summary>
        public void InitializePresets()
        {
            if (File.Exists(PlayerColorPresetFileName))
            {
                MainWindow.AllPalettePresets = DeserializeObjects<PlayerColorPreset>(File.ReadAllText(PlayerColorPresetFileName)).ToList();
                Debug.WriteLine("Preset JSON found on star up, all presets loaded into memory.");
            }
            else //Create the default JSON file.
            {
                PlayerColorPreset editorDefaultPlayerColors = new PlayerColorPreset
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
                PlayerColorPreset gameDefaultPlayerColors = new PlayerColorPreset
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
                PlayerColorPreset highContrastPlayerColors = new PlayerColorPreset
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

                MainWindow.AllPalettePresets.Add(editorDefaultPlayerColors);
                MainWindow.AllPalettePresets.Add(gameDefaultPlayerColors);
                MainWindow.AllPalettePresets.Add(highContrastPlayerColors);

                CreatePlayerPresetJSON();

                Debug.WriteLine("No Preset JSON found, new presets JSON file created and loaded into memory.");
            }
            Debug.WriteLine("Current number of palette presets: " + MainWindow.AllPalettePresets.Count + ".");
        }
    }


    /// <summary>
    /// Creates new player color palettes for Age of Empires Definitive Edition.
    /// </summary>
    /// Creates total of 8 color palettes, one for each player.
    /// 
    /// File names are listed in "PaletteNames" array.
    /// From row 1 to row 3 use the default header data.
    /// From row 4 to 131 create all player colors.
    /// From row 132 to 259 populate the file with "0 0 0" rows.
    /// Row 260 is the last one and will be empty.
    /// 
    /// Player colors are created as follows:
    /// Start with the player color (RGB).
    /// Write it on the document. (use "ColorCodeSeperator" and "RGBColorSeperator" variables to separate each value.)
    /// Get the first value of "InterpolatingIntoColors" array, and linearly Interpolate into that value.
    /// 16 numbers interpolated in total, if counting starting and ending values.
    /// Do the same for each value within "InterpolatingIntoColors" array.
    public class PlayerColorsPalettes
    {
        public static void CreateColors(Vector3[] playerColors)
        {
            string PaletteStartingText = "JASC-PAL" + Environment.NewLine + "0100" + Environment.NewLine + "256";

            string ColorCodeSeperator = " ";
            string RGBColorSeperator = Environment.NewLine;

            int ColorInterpolateCount = 15;

            Vector3[] InterpolatingIntoColors = { new Vector3(0, 0, 0), new Vector3(32, 32, 32), new Vector3(64, 64, 64), new Vector3(128, 128, 128), new Vector3(192, 192, 192), new Vector3(224, 224, 224), new Vector3(255, 255, 255), new Vector3(128, 96, 64) };


            string[] PaletteNames = { "playercolor_blue.pal", "playercolor_red.pal", "playercolor_yellow.pal", "playercolor_brown.pal", "playercolor_orange.pal", "playercolor_green.pal", "playercolor_purple.pal", "playercolor_teal.pal" };


            async void CreatePlayerColorPalette(Vector3 playerColor, string paletteName)
            {
                string TextToWriteInPaletteFile = "";

                TextToWriteInPaletteFile += PaletteStartingText;

                //Going through the target colors.
                foreach (Vector3 ColorToInterpolateInto in InterpolatingIntoColors)
                {
                    //Each step in interpolation.
                    for (int i = 0; i <= ColorInterpolateCount; i++)
                    {
                        TextToWriteInPaletteFile += RGBColorSeperator;

                        //Count the interpolation based on "i", "ColorToInterpolateInto" and "playerColor" variables
                        //Go from "playerColor" to "ColorToInterpolateInto", do this linearly.
                        TextToWriteInPaletteFile += Math.Round((playerColor.X * (ColorInterpolateCount - i) / ColorInterpolateCount) + (ColorToInterpolateInto.X * i / ColorInterpolateCount));
                        TextToWriteInPaletteFile += ColorCodeSeperator;
                        TextToWriteInPaletteFile += Math.Round((playerColor.Y * (ColorInterpolateCount - i) / ColorInterpolateCount) + (ColorToInterpolateInto.Y * i / ColorInterpolateCount));
                        TextToWriteInPaletteFile += ColorCodeSeperator;
                        TextToWriteInPaletteFile += Math.Round((playerColor.Z * (ColorInterpolateCount - i) / ColorInterpolateCount) + (ColorToInterpolateInto.Z * i / ColorInterpolateCount));
                    }
                }

                for (int i = 0; i < 128; i++)
                {
                    TextToWriteInPaletteFile += RGBColorSeperator;
                    TextToWriteInPaletteFile += "0 0 0";
                }

                TextToWriteInPaletteFile += RGBColorSeperator;

                await File.WriteAllTextAsync(UserPreferences.PlayerColorPaletteLocation + @"\" + paletteName, TextToWriteInPaletteFile);
            };


            ///<!-- Logic Starts Here -->
            if (Directory.Exists(UserPreferences.PlayerColorPaletteLocation))
            {
                //Only removes the player color palettes allowing this.exe to be placed inside the AOE:DE folder
                for (int i = 0; i < 8; i++)
                {
                    File.Delete(UserPreferences.PlayerColorPaletteLocation + @"\" + PaletteNames[i]);
                }
            }
            else
            {
                _ = Directory.CreateDirectory(UserPreferences.PlayerColorPaletteLocation);
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
