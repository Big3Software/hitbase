using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.MediaRipper;
using Big3.Hitbase.MediaRipper.MMedia;
using Big3.Hitbase.Configuration;
using System.Windows.Media.Animation;

/*
dwStructVersion - version of the structure, should be set to 1.
dwStructSize - size of the overall BE_CONFIG structure, in this case 331 bytes.
dwSampleRate - sample rate in Hz for the MP3 file. This can be set to one of: 
32000, 44100 or 48000 for MPEG-I
16000, 22050 or 24000 for MPEG-I
8000, 11025 or 12000 for MPEG-II.5
dwReSampleRate - specifes the resampling rate to use against the input stream. Setting this to 0 causes the encoder to decide the rate. 
nMode - the stereo mode of the Mp3 file. This can be one of: 
BE_MP3_MODE_STEREO
BE_MP3_MODE_JSTEREO
BE_MP3_MODE_DUALCHANNEL
BE_MP3_MODE_MONO
dwBitRate - specifies the bit rate to use. For Constant Bit-Rate (CBR) encoding, this is the actual bit rate, whereas for Variable Bit-Rate (VBR) encoding it specifies the minimum. Allowable values are: 
8 - MPEG-II only
16 - MPEG-II only
24 - MPEG-II only
32
40
48
56
64
80
96
112
128
144 - MPEG-II only
160
192 - MPEG-I only.
224 - MPEG-I only.
256 - MPEG-I only.
320 - MPEG-I only.
dwMaxBitrate - When VBR mode is enabled, this specifies the maximum allowed bitrate. The same values as dwBitRate are used.For CBR mode this setting is ignored.
nPreset - one of the preset encoding options to use. Note that presets can overwrite the other settings, since the call to apply the preset's setting is made just before the encoder is initialised. 
LQP_NOPRESET - don't use any presets
LQP_NORMAL_QUALITY - quality is set to 5.
LQP_LOW_QUALITY - quality is set to 9.
LQP_HIGH_QUALITY - quality is set to 2.
LQP_VOICE_QUALITY - use for voice encoding.
LQP_R3MIX - r3mix preset option
LQP_VERYHIGH_QUALITY - quality is set to 0
LQP_STANDARD - lame command line alt-preset standard
LQP_FAST_STANDARD - lame command line alt-preset fast standard
LQP_EXTREME - lame command line alt-preset extreme
LQP_FAST_EXTREME - lame command line alt-preset fast extreme.
LQP_INSANE - lame command line alt-preset insane.
LQP_ABR - lame command line alt-preset abr.
LQP_CBR - lame command line alt-preset cbr.
Note that some presets configure all of the settings, whereas others only affect parts of the configuration. For example, LQP_HIGH_QUALITY does not affect the bit-rate or sampling frequency, whereas LQP_STANDARD will set VBR mode and the bit rate to 200-240kbps. 
dwMpegVersion - the MPEG version to use, one of 
MPEG1 = 1
MPEG2 = 0
dwPsyModel - this parameter does not appear to be used currently. Assumption is that it would be for adjusting a parameter to the Psycho-Acoustic model.
dwEmphasis - - this parameter does not appear to be used currently. Assumption is to configure the emphasis amount.
bPrivate - If this is set to TRUE (1) the Private bit in the MP3 stream will be set.
bCRC - Set this to TRUE in order to enable CRC-checksum in the bitstream.
bCopyright - If this is set to TRUE the Copyright bit in the MP3 stream will be set.
bOriginal - If this is set to TRUE the Original bit in the MP3 stream will be set.
bWriteVBRHeader - Specifes if the XING VBR header should be written or not. When this option is enabled, you have to call the beWriteVBRHeader function when encoding has been completed. 
 * Keep in mind that the VBR info tag can also be written for CBR encoded files, the TAG info can be useful for additional info like encoder delay and the like.
bEnableVBR - Whether to enable VBR encoding.
nVBRQuality - VBR quality option, one of 
VBR_QUALITY_0_HIGHEST = 0
VBR_QUALITY_1_HIGH = 1
VBR_QUALITY_2_HIGH = 2
VBR_QUALITY_3_MID = 3
VBR_QUALITY_4_MID = 4
VBR_QUALITY_5_MID = 5
VBR_QUALITY_6_MID = 6
VBR_QUALITY_7_LOW = 7
VBR_QUALITY_8_LOW = 8
VBR_QUALITY_9_LOWEST = 9
dwVbrAbr_bps - specifies an Average Bit Rate (ABR) encoding. If this is specified, the lame encoder ignores the nVBRQuality settings. To use this, bEnableVBR must be set to TRUE and the nVbrMethod parameter should be set to VBR_METHOD_ABR). The allowed range for the average bit rate is an integer value between: 
MPEG-I: 32000 .. 320000 bps
MPEG-II: 8000 .. 160000 bps
nVBRMethod - the VBR method to use. One of 
VBR_METHOD_NONE = -1
VBR_METHOD_DEFAULT = 0
VBR_METHOD_OLD = 1
VBR_METHOD_NEW = 2
VBR_METHOD_ABR = 4
bNoBitRes - Disables the bit-resorvoir and disables the insertion of padded frames.
bStrictIso - Set strict ISO compatibility for the encoding.
nQuality - Set the quality, from 0 (highest) to 9 (lowest). For backward compatibility reasons, the quality value must have the high-byte set to Not the low-byte. 
            So the values will be: &HFF00 = 0, &HFE01 = 1 ... &HF609 = 9.
bPadding - Not used, provided for future expansion.
 */



namespace Big3.Hitbase.RecordMedium
{
    /// <summary>
    /// Interaction logic for MP3SettingsUserControl.xaml
    /// </summary>
    public partial class MP3SettingsUserControl : UserControl, IModalUserControl
    {
        private EditFormat editMP3Format;

        public MP3SettingsUserControl()
        {
            InitializeComponent();
            
            // Bitrate
            uint bitRate;

            if (Settings.Current.RecordMP3CustomCBR == true)
                toggleButtonCBR.IsChecked = true;
            else
                toggleButtonVBR.IsChecked = true;

            comboBoxCBRBitrates.SelectedValue = (UInt32)Settings.Current.RecordMP3CustomCBRBitRates;
            comboBoxVBRAverage.SelectedValue = (UInt32)Settings.Current.RecordMP3CustomVBRAverage;
            comboBoxVBRMin.SelectedValue = (UInt32)Settings.Current.RecordMP3CustomVBRMin;
            comboBoxVBRMax.SelectedValue = (UInt32)Settings.Current.RecordMP3CustomVBRMax;
            comboBoxVBRMethod.SelectedItem = Settings.Current.RecordMP3CustomVBRMethod;
            sliderVBRQuality.Value = Settings.Current.RecordMP3CustomVBRQuality;
            comboBoxChannels.SelectedIndex = Settings.Current.RecordMP3CustomChannels;

            if (toggleButtonCBR.IsChecked == true)
            {
                bitRate = (uint)comboBoxCBRBitrates.SelectedValue;
                DoubleAnimation da = new DoubleAnimation(0.2, TimeSpan.FromMilliseconds(500).Duration());
                da.Completed += delegate
                {
                    gridVBR.IsEnabled = false;
                };
                gridVBR.BeginAnimation(Grid.OpacityProperty, da);
            }
            else
            {
                bitRate = (uint)comboBoxVBRMin.SelectedValue;
                DoubleAnimation da = new DoubleAnimation(0.2, TimeSpan.FromMilliseconds(500).Duration());
                da.Completed += delegate
                {
                    gridCBR.IsEnabled = false;
                };
                gridCBR.BeginAnimation(Grid.OpacityProperty, da);
            }
            if (comboBoxVBRMethod.SelectedIndex == 0)
            {
                if (comboBoxVBRAverage != null && comboBoxVBRAverage.SelectedIndex >= 0)
                {
                    comboBoxVBRAverage.Visibility = System.Windows.Visibility.Hidden;
                    sliderVBRQuality.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR1.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR2.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR3.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBRBit.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                if (comboBoxVBRAverage != null && comboBoxVBRAverage.SelectedIndex >= 0)
                {
                    comboBoxVBRAverage.Visibility = System.Windows.Visibility.Visible;
                    sliderVBRQuality.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR1.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR2.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR3.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBRBit.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(this, new EventArgs());

            Settings.Current.RecordMP3CustomCBR = (bool)toggleButtonCBR.IsChecked;
            Settings.Current.RecordMP3CustomCBRBitRates = Convert.ToInt32(comboBoxCBRBitrates.SelectedValue);
            Settings.Current.RecordMP3CustomCopyright = false;
            Settings.Current.RecordMP3CustomCRC = true;
            Settings.Current.RecordMP3CustomOriginal = false;
            Settings.Current.RecordMP3CustomPrivate = false;
            Settings.Current.RecordMP3CustomChannels = comboBoxChannels.SelectedIndex;

            Settings.Current.RecordMP3CustomVBR = (bool)toggleButtonVBR.IsChecked;
            Settings.Current.RecordMP3CustomVBRMin =  Convert.ToInt32(comboBoxVBRMin.SelectedValue);
            Settings.Current.RecordMP3CustomVBRMax =  Convert.ToInt32(comboBoxVBRMax.SelectedValue);
            Settings.Current.RecordMP3CustomVBRAverage =  Convert.ToInt32(comboBoxVBRAverage.SelectedValue);
            Settings.Current.RecordMP3CustomVBRMethod = comboBoxVBRMethod.SelectedIndex;
            Settings.Current.RecordMP3CustomVBRQuality = (int)sliderVBRQuality.Value;

            //Big3.Hitbase.MediaRipper.Lame.BE_CONFIG cfg = new Big3.Hitbase.MediaRipper.Lame.BE_CONFIG(waveFormat, ComboBox...);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        private void buttonDefault_Click(object sender, RoutedEventArgs e)
        {
            comboBoxCBRBitrates.SelectedValue = (UInt32)224;
            toggleButtonVBR.IsChecked = true;
            toggleButtonCBR.IsChecked = false;
            comboBoxChannels.SelectedIndex = 1;

            comboBoxVBRAverage.SelectedValue = (UInt32)192;
            comboBoxVBRMax.SelectedValue = (UInt32)320;
            comboBoxVBRMin.SelectedValue = (UInt32)128;
            comboBoxVBRMethod.SelectedIndex = 0;
            sliderVBRQuality.Value = 7;
        }

        private void toggleButtonCBR_Checked(object sender, RoutedEventArgs e)
        {
            toggleButtonVBR.IsChecked = false;
            
            gridCBR.IsEnabled = true;
            DoubleAnimation da = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration());
            gridCBR.BeginAnimation(Grid.OpacityProperty, da);
        }

        private void toggleButtonVBR_Checked(object sender, RoutedEventArgs e)
        {
            toggleButtonCBR.IsChecked = false;
            
            gridVBR.IsEnabled = true;
            DoubleAnimation da = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration());
            gridVBR.BeginAnimation(Grid.OpacityProperty, da);
        }

        private void toggleButtonVBR_Unchecked(object sender, RoutedEventArgs e)
        {

            if (toggleButtonCBR.IsChecked == false)
            {
                toggleButtonVBR.IsChecked = true;
                return;
            }
            
            DoubleAnimation da = new DoubleAnimation(0.2, TimeSpan.FromMilliseconds(500).Duration());
            da.Completed += delegate
            {
                gridVBR.IsEnabled = false;
            };
            gridVBR.BeginAnimation(Grid.OpacityProperty, da);
        }

        private void toggleButtonCBR_Unchecked(object sender, RoutedEventArgs e)
        {
            if (toggleButtonVBR.IsChecked == false)
            {
                toggleButtonCBR.IsChecked = true;
                return;
            }

            DoubleAnimation da = new DoubleAnimation(0.2, TimeSpan.FromMilliseconds(500).Duration());
            da.Completed += delegate
            {
                gridCBR.IsEnabled = false;
            };
            gridCBR.BeginAnimation(Grid.OpacityProperty, da);
        }

        private void toggleButtonVBR_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboBoxVBRMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxVBRMethod.SelectedIndex == 0)
            {
                if (comboBoxVBRAverage != null && comboBoxVBRAverage.SelectedIndex >= 0)
                {
                    comboBoxVBRAverage.Visibility = System.Windows.Visibility.Hidden;
                    sliderVBRQuality.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR1.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR2.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBR3.Visibility = System.Windows.Visibility.Visible;
                    textBlockVBRBit.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                if (comboBoxVBRAverage != null && comboBoxVBRAverage.SelectedIndex >= 0)
                {
                    comboBoxVBRAverage.Visibility = System.Windows.Visibility.Visible;
                    sliderVBRQuality.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR1.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR2.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBR3.Visibility = System.Windows.Visibility.Hidden;
                    textBlockVBRBit.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}
