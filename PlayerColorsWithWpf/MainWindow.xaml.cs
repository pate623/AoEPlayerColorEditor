using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace PlayerColorsWithWpf
{
    /// <summary>
    /// Creates new player color palettes for Age of Empires Definitive edition.
    /// Saves the files in Palettes folder.
    /// Can be placed inside the AoE folder, making it fast and easy to alter colors.
    /// </summary>
    /// TODO: Create palette presets.
    /// Allow users to save and load their presets.
    /// Create 2 presets. (AOE:DE default, Editor Default).

    public partial class MainWindow : Window
    {
        private readonly Vector3[] NewPlayerColors = { new Vector3(15, 70, 245), new Vector3(220, 35, 35), new Vector3(215, 215, 30), new Vector3(115, 60, 0), new Vector3(245, 135, 25), new Vector3(4, 165, 20), new Vector3(245, 95, 240), new Vector3(65, 245, 230) };

        private int CurrentlySelectedPlayerColor = 0;

        private readonly ColorDialog PlayerColorPicker = new ColorDialog();
        private System.Drawing.Color GainedColor;


        public MainWindow()
        {
            InitializeComponent();
            ShowNewlySelectedColors();
        }

        private void CreateColors_Click(object sender, RoutedEventArgs e)
        {
            PlayerColors.CreateColors(NewPlayerColors);
        }


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

            _ = PlayerColorPicker.ShowDialog(); //this is blocking?
            GainedColor = PlayerColorPicker.Color;
            ColorPickerClosed();
        }

        private void ColorPickerClosed()
        {
            ///save the color into the "NewPlayerColors" vector array
            NewPlayerColors[CurrentlySelectedPlayerColor] = new Vector3(GainedColor.R, GainedColor.G, GainedColor.B);
            ShowNewlySelectedColors();
        }

        //Convert vector values to C# color values and showcase them in UI
        private void ShowNewlySelectedColors()
        {
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
        }
    }



    public class PlayerColors
    {
        /// <summary>
        /// Creates new player color palettes for Age of Empires Definitive edition.
        /// </summary>
        /// 
        /// Default folder for player color palettes can be found at:
        /// C:\Program Files (x86)\Steam\steamapps\common\AoEDE\Assets\Palettes
        /// 
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
        /// 
        /// Do the same for each value within "InterpolatingIntoColors" array.
        /// 
        /// Repeat these steps for all the 8 player colors.

        public static void CreateColors(Vector3[] playerColors)
        {
            string PaletteStartingText = "JASC-PAL" + Environment.NewLine + "0100" + Environment.NewLine + "256";

            string ColorCodeSeperator = " ";
            string RGBColorSeperator = Environment.NewLine;

            int ColorInterpolateCount = 15;

            Vector3[] InterpolatingIntoColors = { new Vector3(0, 0, 0), new Vector3(32, 32, 32), new Vector3(64, 64, 64), new Vector3(128, 128, 128), new Vector3(192, 192, 192), new Vector3(224, 224, 224), new Vector3(255, 255, 255), new Vector3(128, 96, 64) };

            string PaletteLocation = Directory.GetCurrentDirectory() + @"\Palettes";

            string[] PaletteNames = { "playercolor_blue.pal", "playercolor_red.pal", "playercolor_yellow.pal", "playercolor_brown.pal", "playercolor_orange.pal", "playercolor_green.pal", "playercolor_purple.pal", "playercolor_teal.pal" };



            async void CreatePlayerColorPalette(Vector3 playerColor, string paletteName)
            {
                string TextToWriteInPaletteFile = "";

                TextToWriteInPaletteFile += PaletteStartingText;

                //going through the target colors.
                foreach (Vector3 ColorToInterpolateInto in InterpolatingIntoColors)
                {
                    //each step in interpolation.
                    for (int i = 0; i <= ColorInterpolateCount; i++)
                    {
                        TextToWriteInPaletteFile += RGBColorSeperator;

                        //count the interpolation based on "i", "ColorToInterpolateInto" and "playerColor" variables
                        //go from "playerColor" to "ColorToInterpolateInto", do this linearly.
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

                await File.WriteAllTextAsync(PaletteLocation + @"\" + paletteName, TextToWriteInPaletteFile);
            };



            if (Directory.Exists(PaletteLocation))
            {
                //Only removes the player color palettes allowing this.exe to be placed inside the AOE:DE folder
                for (int i = 0; i < 8; i++)
                {
                    File.Delete(PaletteLocation + @"\" + PaletteNames[i]);
                }
            }
            else
            {
                _ = Directory.CreateDirectory(PaletteLocation);
            }

            for (int i = 0; i < 8; i++)
            {
                CreatePlayerColorPalette(playerColors[i], PaletteNames[i]);
            }

            _ = System.Windows.Forms.MessageBox.Show("New Player Colors Created.");
        }
    }
}
