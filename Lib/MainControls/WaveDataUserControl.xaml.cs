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
using Big3.Hitbase.SoundEngine;
using System.Windows.Threading;

namespace Big3.Hitbase.MainControls
{
    public enum SoundDisplayType 
    {
        None,
        Signal,
        FrequencyBand,
        Lyrics
    }

    /// <summary>
    /// Interaction logic for WaveDataUserControl.xaml
    /// </summary>
    public partial class WaveDataUserControl : UserControl
    {
        private Image theImage = new Image();

        private SoundDisplayType soundDisplayType = SoundDisplayType.FrequencyBand;

        public SoundDisplayType SoundDisplayType
        {
            get 
            { 
                return soundDisplayType; 
            }
            set 
            { 
                soundDisplayType = value;

                if (soundDisplayType == MainControls.SoundDisplayType.None)
                    ContentCtl.Visibility = System.Windows.Visibility.Collapsed;
                else
                    ContentCtl.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private WriteableBitmap writeableBitmapSignal = null;
        private byte[] pixelsSignal = null;

        private WriteableBitmap writeableBitmapFreqBand = null;
        private byte[] pixelsFreqBand = null;
        
        const int windowHeightSignal = 120;
        const int windowWidthSignal = 300;

        const int windowHeightFreqBand = 100;
        const int windowWidthFreqBand = 260;     // 130 Sample-Werte * Stereo 

        Color[] colorTable = new Color[windowHeightFreqBand];

        public WaveDataUserControl()
        {
            InitializeComponent();
            DispatcherTimer dtWaveData = new DispatcherTimer();
            dtWaveData.Interval = TimeSpan.FromMilliseconds(20).Duration();
            dtWaveData.Tick += new EventHandler(dtWaveData_Tick);
            dtWaveData.Start();

            writeableBitmapSignal = new WriteableBitmap(windowWidthSignal, windowHeightSignal, 96, 96, PixelFormats.Bgra32, null);
            pixelsSignal = new byte[writeableBitmapSignal.PixelHeight * writeableBitmapSignal.PixelWidth * writeableBitmapSignal.Format.BitsPerPixel / 8];

            writeableBitmapFreqBand = new WriteableBitmap(windowWidthFreqBand, windowHeightFreqBand, 96, 96, PixelFormats.Bgra32, null);
            pixelsFreqBand = new byte[writeableBitmapFreqBand.PixelHeight * writeableBitmapFreqBand.PixelWidth * writeableBitmapFreqBand.Format.BitsPerPixel / 8];

            this.ContentCtl.Content = theImage;
            theImage.Stretch = Stretch.Fill;
            RenderOptions.SetBitmapScalingMode(theImage, BitmapScalingMode.Linear);


            Color start = Color.FromArgb(255, 242, 111, 0);
            Color end = Color.FromArgb(255, 242, 111, 0);

            for (int i = 0; i < windowHeightFreqBand/2; i++)
            {
                int r = Interpolate(start.R, end.R, windowHeightFreqBand/2 - 1, i),
                    g = Interpolate(start.G, end.G, windowHeightFreqBand/2 - 1, i),
                    b = Interpolate(start.B, end.B, windowHeightFreqBand/2 - 1, i);

                colorTable[i] = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }

            start = Color.FromArgb(255, 242, 111, 0);
            end = Color.FromArgb(255, 242, 111, 0);

            for (int i = 0; i < windowHeightFreqBand / 2; i++)
            {
                int r = Interpolate(start.R, end.R, windowHeightFreqBand/2 - 1, i),
                    g = Interpolate(start.G, end.G, windowHeightFreqBand/2 - 1, i),
                    b = Interpolate(start.B, end.B, windowHeightFreqBand/2 - 1, i);

                colorTable[windowHeightFreqBand/2 + i] = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }
        }

        public Playlist Playlist { get; set; }

        void dtWaveData_Tick(object sender, EventArgs e)
        {
            if (Playlist != null && Playlist.IsPlaying)
            {
                switch (soundDisplayType)
                {
                    case SoundDisplayType.None:
                        break;
                    case SoundDisplayType.Signal:
                        DrawWaveData();
                        break;
                    case SoundDisplayType.FrequencyBand:
                        DrawSpectrum();
                        break;
                    case SoundDisplayType.Lyrics:
                        break;
                    default:
                        break;
                }
            }
        }

        private void DrawWaveData()
        {
            float[][] waveData = Big3.Hitbase.SoundEngine.SoundEngine.Instance.GetWaveData();

            // Calculate the number of bytes per pixel. 
            int _bytesPerPixel = (writeableBitmapSignal.Format.BitsPerPixel + 7) / 8;
            // Stride is bytes per pixel times the number of pixels.
            // Stride is the byte width of a single rectangle row.
            int _stride = writeableBitmapSignal.PixelWidth * _bytesPerPixel;

            // Bereich leeren
            Array.Clear(pixelsSignal, 0, pixelsSignal.Length);

            double step = (double)windowWidthSignal / (double)waveData[0].Length;
            double x = 0;
            for (int i = 0; i < waveData[0].Length; i++)
            {
                int y = (int)(waveData[0][i] * (windowHeightSignal/2 - 1)) + windowHeightSignal/2;
                int posX = (int)x * 4;
                int posY = (int)y * (writeableBitmapSignal.PixelWidth * _bytesPerPixel);
                int pos = posY + posX;

                pixelsSignal[pos] = 0;
                pixelsSignal[pos + 1] = 0;
                pixelsSignal[pos + 2] = 0;
                pixelsSignal[pos + 3] = 255;

                x += step;
            }

            Int32Rect rect = new Int32Rect(0, 0, writeableBitmapSignal.PixelWidth, writeableBitmapSignal.PixelHeight);
            writeableBitmapSignal.WritePixels(rect, pixelsSignal, _stride, 0);

            theImage.Source = writeableBitmapSignal;
        }

        private void DrawSpectrum()
        {
            float[][] waveData = Big3.Hitbase.SoundEngine.SoundEngine.Instance.GetSpectrum();

            // Calculate the number of bytes per pixel. 
            int _bytesPerPixel = (writeableBitmapFreqBand.Format.BitsPerPixel + 7) / 8;
            // Stride is bytes per pixel times the number of pixels.
            // Stride is the byte width of a single rectangle row.
            int _stride = writeableBitmapFreqBand.PixelWidth * _bytesPerPixel;

            // Bereich leeren
            Array.Clear(pixelsFreqBand, 0, pixelsFreqBand.Length);

            // BPM analyse
            double power = 0;

            int[] bands = {   1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              1, 1, 1, 1, 1, 1, 1, 1, 
                              
                              2, 2, 2, 2, 2, 2, 2, 2, 
                              2, 2, 2, 2, 2, 2, 2, 2, 
                              2, 2, 2, 2, 2, 2, 2, 2, 
                              2, 2, 2, 2, 2, 2, 2, 2, 
                              
                              4, 4, 4, 4, 4, 4, 4, 4,
                              4, 4, 4, 4, 4, 4, 4, 4, 

                              8, 8, 8, 8, 8, 8, 8, 8, 
                              
                              16, 16, 16, 16,
                              32, 32, 
                              64,
                              128,
                              256,
                              512
                            };
            double[][] graphdata = new double[2][];

            for (int channel = 0; channel < 2; channel++)
            {
                graphdata[channel] = new double[bands.Length];

                // Frequenzen zusammenfassen
                int offset = 0;
                for (int i = 0; i < bands.Length; i++)
                {
                    for (int j = 0; j < bands[i]; j++)
                    {
                        graphdata[channel][i] = Math.Max(graphdata[channel][i], waveData[channel][offset]);

                        offset++;
                    }
                }
            }

            // Linker Kanal
            for (int i = 0; i < graphdata[0].Length; i++)
            {
                if (graphdata[0][i] > 0.001)
                {
                    int posX = (windowWidthFreqBand / 2 - i - 1) * 4;

                    int ypos = (int)((1 - graphdata[0][i]) * (windowHeightFreqBand - 1));
                    for (int y = windowHeightFreqBand - 1; y >= ypos; y--)
                    {
                        int posY = (int)y * (writeableBitmapFreqBand.PixelWidth * _bytesPerPixel);
                        int pos = posY + posX;


                        pixelsFreqBand[pos] =     colorTable[y].B;
                        pixelsFreqBand[pos + 1] = colorTable[y].G;
                        pixelsFreqBand[pos + 2] = colorTable[y].R;
                        pixelsFreqBand[pos + 3] = colorTable[y].A;
                    }

                    // Nur den Bass-Anteil für die BPM-Analyse verwenden
                    /*if (i < 96)
                        power += (double)waveData[0][i] * (double)waveData[0][i];*/
                }
            }

            // Rechter Kanal
            for (int i = 0; i < graphdata[1].Length; i++)
            {
                if (graphdata[1][i] > 0.001)
                {
                    int ypos = (int)((1 - graphdata[1][i]) * (windowHeightFreqBand - 1));
                    int posX = (i + windowWidthFreqBand / 2) * 4;

                    for (int y = windowHeightFreqBand - 1; y >= ypos; y--)
                    {
                        int posY = (int)y * (writeableBitmapFreqBand.PixelWidth * _bytesPerPixel);
                        int pos = posY + posX;


                        pixelsFreqBand[pos] = colorTable[y].B;
                        pixelsFreqBand[pos + 1] = colorTable[y].G;
                        pixelsFreqBand[pos + 2] = colorTable[y].R;
                        pixelsFreqBand[pos + 3] = colorTable[y].A;
                    }

                    // Nur den Bass-Anteil für die BPM-Analyse verwenden
                    /*if (i < 96)
                        power += (double)waveData[0][i] * (double)waveData[0][i];*/
                }
            }

            //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": BPM analysis, bassIntensity: " + power.ToString());

            Int32Rect rect = new Int32Rect(0, 0, writeableBitmapFreqBand.PixelWidth, writeableBitmapFreqBand.PixelHeight);
            writeableBitmapFreqBand.WritePixels(rect, pixelsFreqBand, _stride, 0);

            theImage.Source = writeableBitmapFreqBand;
        }

        static int Interpolate(int start, int end, int steps, int count)
        {
            float s = start, e = end, final = s + (((e - s) / steps) * count);
            return (int)final;
        }  

    }
}
