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
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace PlayerColorsWithWpf
{
    public enum EInterpolationStyles
    {
        Default = 0,
        OnlyMainColor = 1,
        Glowing = 2
    }

    public partial class MainWindow : Window
    {
        public static EInterpolationStyles PlyerColorInterpolationStyle = EInterpolationStyles.Default;
        public static List<PalettePresetJSON> AllColorPalettePresets = new List<PalettePresetJSON>();

        /// <summary>
        /// Some UI element trigger selection changes on load.
        /// These triggers can cause the boot order to change which can cause this program to crash.
        /// </summary>
        private static bool ProgramBooted = false;

        private const int CountOfUnchangeableColorPresets = 2;
        private static int MaxLineCountInConsole = 5;

        private static List<Rectangle> PlayerColorBoxes = new List<Rectangle>();
        private static List<Rectangle> CompraredToColorBoxes = new List<Rectangle>();

        private static readonly string[] PaletteFolderDefaultLocations = {
            @"C:\Program Files (x86)\Steam\steamapps\common\AoEDE\Assets\Palettes",
            @"C:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"D:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"E:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes",
            @"F:\SteamLibrary\steamapps\common\AoEDE\Assets\Palettes"};

        private static readonly Vector3[] CurrentlyActivePlayerColors = {
            new Vector3(15, 70, 245),
            new Vector3(220, 35, 35),
            new Vector3(215, 215, 30),
            new Vector3(115, 60, 0),
            new Vector3(245, 135, 25),
            new Vector3(4, 165, 20),
            new Vector3(245, 95, 240),
            new Vector3(65, 245, 230)};

        public MainWindow()
        {
            InitializeComponent();
            LocatePlayerColorBoxes();
            WindowSizer.Initialize();
            UserPreferences.Initialize();
            PalettePresets.Initialize();
            DisplayNewlySelectedColors();
            DisplayPaletteFolderLocation();
            DisplaySelectedInterpolationStyle();
            DisplayColorPresetChoices(UserPreferences.ActivePlayerColorPalette);
            DisplayComparedToColorChoices(UserPreferences.ActiveComparedToPalette);
            ProgramBooted = true;
            Debug.WriteLine("Program booted successfully.");
        }

        /// <summary>
        /// <br>Add a line of text to the console window.</br>
        /// <br>Use empty line as parameter to apply the new maximum line count.</br>
        /// <para>Get old text from the console, add it to a list,
        /// then insert the "textToBeAdded" parameter to that list at index 0.
        /// <br>Clear the console window and then add all the lines from the list to it, whilst
        /// making sure the row count doesn't exceed <see cref="MaxLineCountInConsole"/>.</br></para>
        /// </summary>
        public void PrintToConsole(string textToBeAdded, Color? textColor = null)
        {
            // Optional parameter has to be constant, have to declare default text color value here.
            textColor = textColor == null ? Color.FromRgb(0, 0, 0) : textColor;

            var newText = new Run(textToBeAdded + "\n")
            {
                Foreground = new SolidColorBrush((Color)textColor)
            };

            var customConsole = FindName("CustomConsole") as TextBlock;

            List<Inline> newConsoleText = customConsole.Inlines.ToList();
            newConsoleText.Insert(0, newText);

            customConsole.Inlines.Clear();

            for (int i = 0; i < newConsoleText.Count && i < MaxLineCountInConsole; i++)
            {
                customConsole.Inlines.Add(newConsoleText[i]);
            }
        }

        /// <summary>
        /// <br>Locates all player color squares and adds them to static lists.</br>
        /// <br>This way the colors can be set in a loop and the scripts needn't locate
        /// the color squares each time they need to be changed.</br>
        /// </summary>
        private void LocatePlayerColorBoxes()
        {
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
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var InfoPopUp = FindName("InfoPopUp") as StackPanel;
            bool infoPopUpVisible = InfoPopUp.Visibility == Visibility.Visible;
            InfoPopUp.Visibility = infoPopUpVisible ? Visibility.Collapsed : Visibility.Visible;
        }

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
        /// <br>Clicking the color boxes under "Your Edits text" opens up a color picker.</br>
        /// <br>Each time the color picker is closed this script updates
        /// <see cref="CurrentlyActivePlayerColors"/> and showcases these changes in the UI.</br>
        /// </summary>
        private void OpenColorPicker(int selectedPlayerColor)
        {
            Debug.WriteLine("Color picker opened with player color id: " + selectedPlayerColor);

            var playerColorPicker = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(0,
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

        private void ComparedToColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex == -1) return;

            Debug.WriteLine("New preset selected for compared to colors combo box, with index: {0}.",
                presetSelection.SelectedIndex);
            UserPreferences.ActiveComparedToPalette = presetSelection.SelectedIndex;
            DisplayComparedToPlayerColors(presetSelection.SelectedIndex);
        }

        /// <summary>
        /// Changes UI to show newly selected compared to player colors in the compared to player color squares.
        /// </summary>
        private void DisplayComparedToPlayerColors(int presetID)
        {
            if (AllColorPalettePresets.Count <= presetID)
            {
                Debug.WriteLine("Index for compared to colors is too big");
                return;
            }

            for (int i = 0; i < CompraredToColorBoxes.Count; i++)
            {
                CompraredToColorBoxes[i].Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)AllColorPalettePresets[presetID].GetPlayerColor(i).X,
                    (byte)AllColorPalettePresets[presetID].GetPlayerColor(i).Y,
                    (byte)AllColorPalettePresets[presetID].GetPlayerColor(i).Z));
            }

            Debug.WriteLine("UI compared to player colors updated.");
            UserPreferences.SaveToDisk();
        }

        /// <summary>
        /// Updates the Compared to player color ComboBox listings in the UI,
        /// and saves the currently active selection to the user preferences.
        /// </summary>
        /// <param name="presetID">ID of selected item.</param>
        private void DisplayComparedToColorChoices(int presetID)
        {
            var presetsComboBox = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;

            presetsComboBox.Items.Clear();

            foreach (var currentColorPreset in AllColorPalettePresets)
            {
                _ = presetsComboBox.Items.Add(currentColorPreset.PresetName);
            }

            presetsComboBox.SelectedIndex = presetID;
            Debug.WriteLine("Preset list in compared to combo box updated.");

            UserPreferences.ActiveComparedToPalette = presetID;
            UserPreferences.SaveToDisk();
        }

        /// <summary>
        /// <br>Updates the compared to player colors combo box UI to showcase the new player color choices.</br>
        /// <br>Also applies the new color to "compared to colors" squares.</br>
        /// <br>Called after user saves, saves as, or deletes a color preset.</br>
        /// </summary>
        private void ApplyPickedComparedToColors()
        {
            var presetSelection = FindName("ComparedToColorSelection") as System.Windows.Controls.ComboBox;
            int toBeSetIndex = presetSelection.SelectedIndex;

            if (AllColorPalettePresets.Count <= toBeSetIndex)
            {
                Debug.WriteLine("Index too big for comparison combo box, reseted to index 1");
                UserPreferences.ActiveComparedToPalette = 1;
                DisplayComparedToColorChoices(1);
                DisplayComparedToPlayerColors(1);
            }
            else
            {
                UserPreferences.ActiveComparedToPalette = toBeSetIndex;
                DisplayComparedToColorChoices(toBeSetIndex);
                DisplayComparedToPlayerColors(toBeSetIndex);
            }
        }

        /// <summary>
        /// <br>Opens a folder browser.</br>
        /// <br>Saves the picked folder as the folder where all palette files will be created.</br>
        /// </summary>
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

            DisplayPaletteFolderLocation();
            UserPreferences.SaveToDisk();
        }

        /// <summary>
        /// Get color palettes folder location from the user preferences and displays that string in the UI.
        /// </summary>
        private void DisplayPaletteFolderLocation()
        {
            var colorPalettePathText = FindName("ColorPalettesLocation") as TextBlock;

            colorPalettePathText.Text = UserPreferences.PlayerColorPaletteLocation;
            Debug.WriteLine("UI Palette location string updated.");
        }

        /// <summary>
        /// Updates UI and code to display the newly selected color preset as a currently active "Your Edit" color.
        /// </summary>
        private void ColorPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;

            if (presetSelection.SelectedIndex == -1) return;
            
            Debug.WriteLine("New preset selected from combo box with index: {0}.", presetSelection.SelectedIndex);
            UpdateDataToSelectedPreseset(presetSelection.SelectedIndex);
            DisplayNewlySelectedColors();
        }

        /// <summary>
        /// <para>Used to load the color preset after it has been selected from the Combo Box.</para>
        /// <br>Updates both; the UI and the code.</br>
        /// <br>In UI disables/enables "save" and "delete" preset buttons depending
        /// whether if the currently selected preset is one of the default presets.</br>
        /// <br>One the code side; loads the color presets colors into <see cref="CurrentlyActivePlayerColors"/></br>
        /// <br>Updates user preferences.</br>
        /// </summary>
        private void UpdateDataToSelectedPreseset(int currentlyActivePresetIndex)
        {
            if (currentlyActivePresetIndex < AllColorPalettePresets.Count && currentlyActivePresetIndex >= 0)
            {
                for (int i = 0; i < CurrentlyActivePlayerColors.Length; i++)
                {
                    CurrentlyActivePlayerColors[i] = new Vector3(
                    AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).X,
                    AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).Y,
                    AllColorPalettePresets[currentlyActivePresetIndex].GetPlayerColor(i).Z);
                }
                Debug.WriteLine("Newly selected color updated to the Vector3[] NewPlayerColors.");

                UserPreferences.ActivePlayerColorPalette = currentlyActivePresetIndex;
                UserPreferences.SaveToDisk();
            }
            else
            {
                Debug.WriteLine("Failed to update player colors; given index was too big.");
                Debug.WriteLine("Given index number was: {0} and the count of palette presets is: {1}.",
                    currentlyActivePresetIndex, AllColorPalettePresets.Count);
            }

            // If currently selected preset is one of the default presets then disable save and delete buttons.
            // Makes sure this doesn't get executed before all window elements are loaded.
            if (FindName("SavePreset") is System.Windows.Controls.Button savePresetButton)
            {
                savePresetButton.IsEnabled = currentlyActivePresetIndex >= CountOfUnchangeableColorPresets;

                var deletePresetButton = FindName("DeletePreset") as System.Windows.Controls.Button;
                deletePresetButton.IsEnabled = currentlyActivePresetIndex >= CountOfUnchangeableColorPresets;
            }
        }

        /// <summary>
        /// <br>Updates the currently active player colors combo box to display all available choices.</br>
        /// <br>Uses names from <see cref="AllColorPalettePresets"/> as the combo box choices.</br>
        /// </summary>
        /// <param name="activePresetIndex">Displays this as active color preset.</param>
        private void DisplayColorPresetChoices(int activePresetIndex)
        {
            var presetsComboBox = FindName("PresetSelection") as System.Windows.Controls.ComboBox;

            presetsComboBox.Items.Clear();

            foreach (PalettePresetJSON currentColorPreset in AllColorPalettePresets)
            {
                _ = presetsComboBox.Items.Add(currentColorPreset.PresetName);
            }

            presetsComboBox.SelectedIndex = activePresetIndex;
            Debug.WriteLine("Player color presets list in ComboBox updated.");
        }

        /// <summary>
        /// <br>Updates the "Your Edit" player colors squares shown in the UI.</br>
        /// <br>Uses the colors from <see cref="CurrentlyActivePlayerColors"/>.</br>
        /// </summary>
        private void DisplayNewlySelectedColors()
        {
            for (int i = 0; i < PlayerColorBoxes.Count(); i++)
            {
                PlayerColorBoxes[i].Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)CurrentlyActivePlayerColors[i].X,
                    (byte)CurrentlyActivePlayerColors[i].Y,
                    (byte)CurrentlyActivePlayerColors[i].Z));
            }

            Debug.WriteLine("UI player colors updated.");
        }

        /// <summary>
        /// Saves colors from <see cref="CurrentlyActivePlayerColors"/> to the
        /// <see cref="AllColorPalettePresets"/> at "currently active color preset" index.
        /// </summary>
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            int currentPreset = presetSelection.SelectedIndex;

            for (int i = 0; i < PlayerColorBoxes.Count(); i++)
            {
                AllColorPalettePresets[currentPreset].SetPlayerColor(CurrentlyActivePlayerColors[i], i);
            }

            PalettePresets.SaveToDisk();
            ApplyPickedComparedToColors();
            PrintToConsole("Saved palette preset", Color.FromRgb(50, 50, 50));
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

            var colorPresets = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            colorPresets.IsEnabled = true;

            Debug.WriteLine("Save preset as pop up closed.");
        }

        /// <summary>
        /// <br>Creates new color palettes object from <see cref="CurrentlyActivePlayerColors"/> and
        /// saves it to <see cref="AllColorPalettePresets"/>.</br>
        /// <br>Saves the palettes from <see cref="AllColorPalettePresets"/> to the disk and
        /// sets the UI elements to display the newly created color preset.</br>
        /// </summary>
        private void AcceptNewPresetName_Click(object sender, RoutedEventArgs e)
        {
            var presetNameString = FindName("NewPresetName") as System.Windows.Controls.TextBox;

            var newPreset = new PalettePresetJSON(
                presetNameString.Text,
                CurrentlyActivePlayerColors[0],
                CurrentlyActivePlayerColors[1],
                CurrentlyActivePlayerColors[2],
                CurrentlyActivePlayerColors[3],
                CurrentlyActivePlayerColors[4],
                CurrentlyActivePlayerColors[5],
                CurrentlyActivePlayerColors[6],
                CurrentlyActivePlayerColors[7]);

            AllColorPalettePresets.Add(newPreset);

            PalettePresets.SaveToDisk();
            Debug.WriteLine("Created new palette preset.");

            // Update UI to reflect all changes.
            DisplayColorPresetChoices(AllColorPalettePresets.Count - 1);

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            var presetNameBox = FindName("PresetNamePopUp") as StackPanel;
            presetNameBox.Visibility = Visibility.Collapsed;

            ApplyPickedComparedToColors();
            PrintToConsole("Created new palette preset", Color.FromRgb(50, 50, 50));
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
        /// <br>Closes the "delete preset" pop up and deletes the presets.</br>
        /// <br>Deletes a preset from <see cref="AllColorPalettePresets"/> at index of currently active
        /// "PresetSelection".</br>
        /// </summary>
        private void AcceptRemoval_Click(object sender, RoutedEventArgs e)
        {
            var deletePopup = FindName("PresetDeletePopUp") as StackPanel;
            deletePopup.Visibility = Visibility.Collapsed;

            var presetDropdown = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            presetDropdown.IsEnabled = true;

            var presetSelection = FindName("PresetSelection") as System.Windows.Controls.ComboBox;
            AllColorPalettePresets.RemoveAt(presetSelection.SelectedIndex);

            PalettePresets.SaveToDisk();
            Debug.WriteLine("Player color preset deleted.");

            // Update UI to reflect all changes.
            DisplayColorPresetChoices(0);
            DisplayNewlySelectedColors();
            ApplyPickedComparedToColors();
            PrintToConsole("Deleted palette preset", Color.FromRgb(190, 20, 20));
        }

        /// <summary>
        /// <br>Creates all color palettes from <see cref="CurrentlyActivePlayerColors"/>.</br>
        /// <br>Color palettes location is defined in the user preferences.</br>
        /// </summary>
        private void CreateColors_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPaletteCreation.CreateColors(CurrentlyActivePlayerColors))
            {
                PrintToConsole("Created the color palettes", Color.FromRgb(0, 102, 221));
            }
            else
            {
                PrintToConsole("Failed to create color palettes", Color.FromRgb(255, 0, 0));
            }
        }

        private void ColorInterpolation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var colorSelection = FindName("ColorInterpolationSelection") as System.Windows.Controls.ComboBox;

            if (colorSelection.SelectedIndex == -1) return;

            PlyerColorInterpolationStyle = (EInterpolationStyles)colorSelection.SelectedIndex;

            if (!ProgramBooted) return;

            Debug.WriteLine(((EInterpolationStyles)colorSelection.SelectedIndex).ToString() +
                " interpolation style selected.");
            UserPreferences.ActiveInterpolationStyle = colorSelection.SelectedIndex;
            UserPreferences.SaveToDisk();
        }

        private void DisplaySelectedInterpolationStyle()
        {
            var colorSelection = FindName("ColorInterpolationSelection") as System.Windows.Controls.ComboBox;
            colorSelection.SelectedIndex = UserPreferences.ActiveInterpolationStyle;
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
    /// <br>Fixed ratio is determined by the window minimum width and height written in the XAML file.</br>
    /// </summary>
    public static class WindowSizer
    {
        public static double DefaultLeft;
        public static double DefaultTop;
        public static double DefaultWidth;
        public static double DefaultHeight;

        private static double WidthRatio;
        private static double HeightRatio;

        private static bool TimerIsRunning = false;
        private static bool TimerNeedsToBeRefreshed = false;

        /// <summary>
        /// <br>Saves current window size and location.</br>
        /// <br>Uses the gained window width and height to determine windows fixed ratio.</br>
        /// </summary>
        public static void Initialize()
        {
            DefaultWidth = System.Windows.Application.Current.MainWindow.MinWidth;
            DefaultHeight = System.Windows.Application.Current.MainWindow.MinHeight;

            DefaultLeft = System.Windows.Application.Current.MainWindow.Left;
            DefaultTop = System.Windows.Application.Current.MainWindow.Top;

            WidthRatio = DefaultWidth / DefaultHeight;
            HeightRatio = DefaultHeight / DefaultWidth;
        }

        /// <summary>
        /// <br>Saves the new window location and size to the user preferences and
        /// saves the user preferences to the disk after user stops adjusting the window.</br>
        /// </summary>
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

        private static async void StartTimer()
        {
            await Task.Delay(200);
            while (TimerNeedsToBeRefreshed)
            {
                TimerNeedsToBeRefreshed = false;
                await Task.Delay(400);
            }

            TimerIsRunning = false;
            Debug.WriteLine("User stopped adjusting the windows size.");
            UserPreferences.SaveToDisk();
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
        public int ActiveInterpolation { get; set; }
        public int WindowsWidth { get; set; }
        public int WindowsHeight { get; set; }
        public int WindowsLeft { get; set; }
        public int WindowsTop { get; set; }
    }

    /// <summary>
    /// User preferences are stored in a JSON file at the root of this program.
    /// </summary>
    public static class UserPreferences
    {
        public static string PlayerColorPaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";
        public static int ActivePlayerColorPalette = 0;
        public static int ActiveComparedToPalette = 1;
        public static int ActiveInterpolationStyle = 0;

        private static readonly string UserPreferenceFileLocation =
            Directory.GetCurrentDirectory() + @"\UserPreferences.json";

        /// <summary>
        /// <br>Loads user preferences from disk to memory.</br>
        /// <br>If no preferences are found creates the file with default settings.</br>
        /// </summary>
        public static void Initialize()
        {
            Debug.WriteLine("Started loading user preferences.");

            if (File.Exists(UserPreferenceFileLocation))
            {
                string preferencesFromDisk = File.ReadAllText(UserPreferenceFileLocation);

                var userPreferences =
                    System.Text.Json.JsonSerializer.Deserialize<UserPreferencesJSON>(preferencesFromDisk);

                PlayerColorPaletteLocation = userPreferences.PaletteLocation;
                ActivePlayerColorPalette = userPreferences.ActiveColorPalette;
                ActiveComparedToPalette = userPreferences.ActiveComparedTo;
                ActiveInterpolationStyle = userPreferences.ActiveInterpolation;
                System.Windows.Application.Current.MainWindow.Width = userPreferences.WindowsWidth;
                System.Windows.Application.Current.MainWindow.Height = userPreferences.WindowsHeight;
                System.Windows.Application.Current.MainWindow.Left = userPreferences.WindowsLeft;
                System.Windows.Application.Current.MainWindow.Top = userPreferences.WindowsTop;
                Debug.WriteLine("Previous user preference file found and loaded.");
            }
            else
            {
                Debug.WriteLine("No user preference file found.");
                System.Windows.Application.Current.MainWindow.Width = WindowSizer.DefaultWidth;
                System.Windows.Application.Current.MainWindow.Height = WindowSizer.DefaultHeight;
                ActiveComparedToPalette = 1; // Reads the default values as 0 even though it is set to 1.

                var newPreferences = new UserPreferencesJSON
                {
                    PaletteLocation = PlayerColorPaletteLocation,
                    ActiveColorPalette = ActivePlayerColorPalette,
                    ActiveComparedTo = ActiveComparedToPalette,
                    ActiveInterpolation = ActiveInterpolationStyle,
                    WindowsWidth = (int)WindowSizer.DefaultWidth,
                    WindowsHeight = (int)WindowSizer.DefaultHeight,
                    WindowsLeft = (int)WindowSizer.DefaultLeft,
                    WindowsTop = (int)WindowSizer.DefaultTop
                };

                File.WriteAllText(
                    UserPreferenceFileLocation,
                    System.Text.Json.JsonSerializer.Serialize(newPreferences));

                Debug.WriteLine("New user preferences created.");
            }
        }

        /// <summary>
        /// <br>Locates all the values to be saved.</br>
        /// <br>Always overwrites the whole JSON regardless of how many actual values were changed.</br>
        /// </summary>
        public static void SaveToDisk()
        {
            var newPreferences = new UserPreferencesJSON
            {
                PaletteLocation = PlayerColorPaletteLocation,
                ActiveColorPalette = ActivePlayerColorPalette,
                ActiveComparedTo = ActiveComparedToPalette,
                ActiveInterpolation = ActiveInterpolationStyle,
                WindowsWidth = (int)System.Windows.Application.Current.MainWindow.Width,
                WindowsHeight = (int)System.Windows.Application.Current.MainWindow.Height,
                WindowsLeft = (int)WindowSizer.DefaultLeft,
                WindowsTop = (int)WindowSizer.DefaultTop
            };

            File.WriteAllText(
                UserPreferenceFileLocation,
                System.Text.Json.JsonSerializer.Serialize(newPreferences));

            Debug.WriteLine("User preferences saved.");
        }
    }

    /// <summary>
    /// <br>JSON coded "Player Color Palette Preset" object.</br>
    /// <br>Player color Blue is index 0 and Teal is index 7</br>
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

        public PalettePresetJSON() { }

        public PalettePresetJSON(string name,
            Vector3 blue, Vector3 red, Vector3 yellow, Vector3 brown,
            Vector3 orange, Vector3 green, Vector3 purple, Vector3 teal)
        {
            PresetName = name;

            BluePlayerColor = new int[] { (int)blue.X, (int)blue.Y, (int)blue.Z };
            RedPlayerColor = new int[] { (int)red.X, (int)red.Y, (int)red.Z };
            YellowPlayerColor = new int[] { (int)yellow.X, (int)yellow.Y, (int)yellow.Z };
            BrownPlayerColor = new int[] { (int)brown.X, (int)brown.Y, (int)brown.Z };

            OrangePlayerColor = new int[] { (int)orange.X, (int)orange.Y, (int)orange.Z };
            GreenPlayerColor = new int[] { (int)green.X, (int)green.Y, (int)green.Z };
            PurplePlayerColor = new int[] { (int)purple.X, (int)purple.Y, (int)purple.Z };
            TealPlayerColor = new int[] { (int)teal.X, (int)teal.Y, (int)teal.Z };
        }

        public void SetPlayerColor(Vector3 playerColor, int playerIndex)
        {
            switch (playerIndex)
            {
                case 0:
                    BluePlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 1:
                    RedPlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 2:
                    YellowPlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 3:
                    BrownPlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;

                case 4:
                    OrangePlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 5:
                    GreenPlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 6:
                    PurplePlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
                case 7:
                    TealPlayerColor = new int[] { (int)playerColor.X, (int)playerColor.Y, (int)playerColor.Z };
                    break;
            }
        }

        public Vector3 GetPlayerColor(int index)
        {
            return index switch
            {
                0 => new Vector3(BluePlayerColor[0], BluePlayerColor[1], BluePlayerColor[2]),
                1 => new Vector3(RedPlayerColor[0], RedPlayerColor[1], RedPlayerColor[2]),
                2 => new Vector3(YellowPlayerColor[0], YellowPlayerColor[1], YellowPlayerColor[2]),
                3 => new Vector3(BrownPlayerColor[0], BrownPlayerColor[1], BrownPlayerColor[2]),
                4 => new Vector3(OrangePlayerColor[0], OrangePlayerColor[1], OrangePlayerColor[2]),
                5 => new Vector3(GreenPlayerColor[0], GreenPlayerColor[1], GreenPlayerColor[2]),
                6 => new Vector3(PurplePlayerColor[0], PurplePlayerColor[1], PurplePlayerColor[2]),
                7 => new Vector3(TealPlayerColor[0], TealPlayerColor[1], TealPlayerColor[2]),
                _ => new Vector3(0,0,0),
            };
        }
    }

    /// <summary>
    /// <br>All presets are stored in a JSON file at the root of this program.</br>
    /// <br>In the code side files are stored in a JSON compatible object.</br>
    /// </summary>
    public static class PalettePresets
    {
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

                    BluePlayerColor = new int[] { 43, 63, 247 },
                    RedPlayerColor = new int[] { 224, 27, 27 },
                    YellowPlayerColor = new int[] { 230, 234, 53 },
                    BrownPlayerColor = new int[] { 96, 43, 11 },

                    OrangePlayerColor = new int[] { 234, 128, 21 },
                    GreenPlayerColor = new int[] { 30, 165, 5 },
                    PurplePlayerColor = new int[] { 218, 3, 186 },
                    TealPlayerColor = new int[] { 126, 241, 184 }
                };

                MainWindow.AllColorPalettePresets.Add(editorDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(gameDefaultPlayerColors);
                MainWindow.AllColorPalettePresets.Add(highContrastPlayerColors);

                SaveToDisk();

                Debug.WriteLine("No Preset JSON found, new presets JSON file created and loaded into memory.");
            }

            Debug.WriteLine("Current number of palette presets: {0}.", MainWindow.AllColorPalettePresets.Count);
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

        /// <summary>Total of this +1 colors if counting both starting and ending colors.</summary>
        private const int ColorInterpolationCount = 15;

        private static readonly Vector3[] InterpolatingIntoColors = {
            new Vector3(0, 0, 0),
            new Vector3(32, 32, 32),
            new Vector3(64, 64, 64),
            new Vector3(128, 128, 128),
            new Vector3(192, 192, 192),
            new Vector3(224, 224, 224),
            new Vector3(255, 255, 255),
            new Vector3(128, 96, 64)};

        private static readonly string[] PaletteNames = {
            "playercolor_blue.pal",
            "playercolor_red.pal",
            "playercolor_yellow.pal",
            "playercolor_brown.pal",
            "playercolor_orange.pal",
            "playercolor_green.pal",
            "playercolor_purple.pal",
            "playercolor_teal.pal"};

        /// <summary>
        /// <br>Running this once creates all 8 player color palettes.</br>
        /// <br>The palette location is stored in the user preferences.</br>
        /// </summary>
        /// <param name="playerColors">Holds 8 player colors.</param>
        /// <returns>True if palette files were successfully created.</returns>
        public static bool CreateColors(Vector3[] playerColors)
        {
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

                        switch (MainWindow.PlyerColorInterpolationStyle)
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
                    File.WriteAllText($"{UserPreferences.PlayerColorPaletteLocation}\\{paletteName}",
                        textToWriteInPaletteFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            };

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

            /// <!-- Logic Starts Here -->
            if (Directory.Exists(UserPreferences.PlayerColorPaletteLocation))
            {
                try
                {
                    for (int i = 0; i < 8; i++)
                    {
                        File.Delete(UserPreferences.PlayerColorPaletteLocation + @"\" + PaletteNames[i]);
                    }
                    Debug.WriteLine("Previous player colors palettes removed");
                }
                catch
                {
                    Debug.WriteLine("Can't delete currently existing palette files");
                    return false;
                }
            }
            else
            {
                _ = Directory.CreateDirectory(UserPreferences.PlayerColorPaletteLocation);
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
        }
    }
}
