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
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for VisualEffectsUserControl.xaml
    /// </summary>
    public partial class VisualEffectsUserControl : UserControl
    {
        private WriteableBitmap writeableBitmapSignal = null;
        private byte[] pixelsSignal = null;

        const int windowHeightSignal = 300;
        const int windowWidthSignal = 300;

        // Calculate the number of bytes per pixel. 
        int _bytesPerPixel;
        // Stride is bytes per pixel times the number of pixels.
        // Stride is the byte width of a single rectangle row.
        int _stride;

        private Plasma plasma = new Plasma();
        private Fire fire = new Fire();

        private string visualEffect;

        public VisualEffectsUserControl(string visualEffect)
        {
            InitializeComponent();

            this.visualEffect = visualEffect;

            writeableBitmapSignal = new WriteableBitmap(windowWidthSignal, windowHeightSignal, 96, 96, PixelFormats.Bgra32, null);
            pixelsSignal = new byte[writeableBitmapSignal.PixelHeight * writeableBitmapSignal.PixelWidth * writeableBitmapSignal.Format.BitsPerPixel / 8];

            // Calculate the number of bytes per pixel. 
            int _bytesPerPixel = (writeableBitmapSignal.Format.BitsPerPixel + 7) / 8;
            // Stride is bytes per pixel times the number of pixels.
            // Stride is the byte width of a single rectangle row.
            _stride = writeableBitmapSignal.PixelWidth * _bytesPerPixel;


            DispatcherTimer dtWaveData = new DispatcherTimer();
            dtWaveData.Interval = TimeSpan.FromMilliseconds(20).Duration();
            dtWaveData.Tick += new EventHandler(dtWaveData_Tick);
            dtWaveData.Start();

            /*DoubleAnimation da = new DoubleAnimation(40, 60, TimeSpan.FromSeconds(2).Duration());
            da.AutoReverse = true;
            da.EasingFunction = new BackEase();
            da.RepeatBehavior = RepeatBehavior.Forever;
            swirlEffect.BeginAnimation(SwirlEffect.SpiralStrengthProperty, da);*/
        }

        void dtWaveData_Tick(object sender, EventArgs e)
        {
            //DrawWaveData();
            switch (visualEffect)
            {
                case "Plasma":
                    this.theImage.Source = plasma.CreateFrame();
                    break;
                case "Fire":
                    this.theImage.Source = fire.CreateFrame();
                    break;
            }
        }

        public void DrawWaveData()
        {
            float[][] waveData = Big3.Hitbase.SoundEngine.SoundEngine.Instance.GetWaveData();

            // Bereich leeren
            Array.Clear(pixelsSignal, 0, pixelsSignal.Length);

            double step = (double)windowWidthSignal / (double)waveData[0].Length;
            double x = 0;
            int lastX = 0;
            int lastY = 0;
            for (int i = 0; i < waveData[0].Length; i++)
            {
                int y = (int)(waveData[0][i] * (windowHeightSignal / 2 - 1)) + windowHeightSignal / 2;
                int posX = (int)x * 4;
                int posY = (int)y * (writeableBitmapSignal.PixelWidth * _bytesPerPixel);
                int pos = posY + posX;

                /*pixelsSignal[pos] = 0;
                pixelsSignal[pos + 1] = 0;
                pixelsSignal[pos + 2] = 0;
                pixelsSignal[pos + 3] = 255;*/

                if (i>0)
                    DrawLine(lastX, (int)x, lastY, y);
                lastX = (int)x;
                lastY = y;

                x += step;
            }

            Int32Rect rect = new Int32Rect(0, 0, writeableBitmapSignal.PixelWidth, writeableBitmapSignal.PixelHeight);
            writeableBitmapSignal.WritePixels(rect, pixelsSignal, _stride, 0);

            theImage.Source = writeableBitmapSignal;
        }

        private void DrawLine(int x0, int x1, int y0, int y1)
        {
            int deltax = x1 - x0;

            if (deltax == 0)
                return;

            int deltay = y1 - y0;

            float error = 0;
            float deltaerr = deltay / deltax;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                int posX = (int)x * 4;
                int posY = (int)y * (writeableBitmapSignal.PixelWidth * _bytesPerPixel);
                int pos = posY + posX;

                pixelsSignal[pos] = 0;
                pixelsSignal[pos + 1] = 0;
                pixelsSignal[pos + 2] = 0;
                pixelsSignal[pos + 3] = 255;

                error += deltaerr;
                if (Math.Abs(error) >= 0.5)
                {
                    y++;
                    error--;
                }
            }
        }



    }

    public class Fire
    {
        private readonly Color[] colors = new Color[256];
        private const int SCREEN_WIDTH = 480;
        private const int SCREEN_HEIGHT = 60;

        private byte[] firePal = new byte[SCREEN_WIDTH * SCREEN_HEIGHT];

        WriteableBitmap bitmapPixels = new WriteableBitmap(SCREEN_WIDTH, SCREEN_HEIGHT, 96, 96, PixelFormats.Bgra32, null);
        private int[] firePixel = new int[SCREEN_WIDTH * SCREEN_HEIGHT];

        public Fire()
        {
            initFire();
        }

        void initFire()
        {
            int i;

            /* create a suitable fire palette, this is crucial for a good effect */
            /* black to blue, blue to red, red to yellow, yellow to white*/

            for (i = 0; i < 32; ++i)
            {
                /* black to blue, 32 values*/
                colors[i] = Color.FromRgb(0,0,(byte)(i << 1));

                /* blue to red, 32 values*/
                colors[i + 32] = Color.FromRgb((byte)(i << 3), 0, (byte)(64 - (i << 1)));

                /*red to yellow, 32 values*/
                colors[i + 64] = Color.FromRgb(255, (byte)(i << 3), 0);

                /* yellow to white, 162 */
                colors[i + 96] = Color.FromRgb(255, 255, (byte)(i << 2));
                colors[i + 128] = Color.FromRgb(255, 255, (byte)(64 + (i << 2))); 
                colors[i + 160] = Color.FromRgb(255, 255, (byte)(128 + (i << 2)));
                colors[i + 192] = Color.FromRgb(255, 255, (byte)(192 + i));
                colors[i + 224] = Color.FromRgb(255, 255, (byte)(224 + i));
            }
        }

        public WriteableBitmap CreateFrame()
        {
            short temp;
            int j = SCREEN_WIDTH * (SCREEN_HEIGHT - 1);
            Random rand = new Random();

            float[][] spectrum = SoundEngine.SoundEngine.Instance.GetSpectrum();

            for (int i=0;i<(SCREEN_WIDTH-1)/2;i++)
            {
                double leftValue = spectrum[0][i / 2] * 512;
                if (leftValue > 255)
                    leftValue = 255;
                double rightValue = spectrum[1][i / 2] * 512;
                if (rightValue > 255)
                    rightValue = 255;

                firePal[j + (SCREEN_WIDTH - 1) / 2 - i] = (byte)leftValue;
                firePal[j + (SCREEN_WIDTH - 1) / 2 + i] = (byte)rightValue;
            }

            /*for (int i = 0; i < SCREEN_WIDTH - 1; i++)
            {
                //int random = 1 + (int)(16.0 * (rand() / (RAND_MAX + 1.0)));
                int random = rand.Next(1, 17);
                if (random > 9) // the lower the value, the intenser the fire, compensate a lower value with a higher decay value
                    firePal[j + i] = 255; //maximum heat
                else
                    firePal[j + i] = 0;
            }*/

            /* move fire upwards, start at bottom*/

            for (int index = 0; index < SCREEN_HEIGHT - 1; ++index)
            {
                for (int i = 0; i < SCREEN_WIDTH - 1; ++i)
                {
                    if (i == 0) /* at the left border*/
                    {
                        temp = firePal[j];
                        temp += firePal[j + 1];
                        temp += firePal[j - SCREEN_WIDTH];
                        temp /= 3;
                    }
                    else if (i == SCREEN_WIDTH - 1) /* at the right border*/
                    {
                        temp = firePal[j + i];
                        temp += firePal[j - SCREEN_WIDTH + i];
                        temp += firePal[j + i - 1];
                        temp /= 3;
                    }
                    else
                    {
                        temp = firePal[j + i];
                        temp += firePal[j + i + 1];
                        temp += firePal[j + i - 1];
                        temp += firePal[j - SCREEN_WIDTH + i];
                        temp >>= 2;
                    }
                    if (temp > 1)
                        temp -= 1; /* decay */

                    firePal[j - SCREEN_WIDTH + i] = (byte)temp;
                }
                j -= SCREEN_WIDTH;
            }

            for (int i = 0; i < firePal.Length; i++)
            {
                var c = colors[firePal[i]];

                firePixel[i] = c.A << 24 | c.R << 16 | c.G << 8 | c.B;
            }

            Int32Rect rect = new Int32Rect(0, 0, bitmapPixels.PixelWidth, bitmapPixels.PixelHeight);

            int stride = bitmapPixels.PixelWidth * 4;

            bitmapPixels.WritePixels(rect, firePixel, stride, 0);

            return bitmapPixels;

            
            //image = (Uint8*)screen->pixels + (screen->pitch * SCREEN_HEIGHT);  /*start in the right bottom corner*/

            /* draw fire array to screen from bottom to top + 300*/

            /*for (int i = SCREEN_HEIGHT - 3; i >= 300; --i)
            {
                for (j = SCREEN_WIDTH - 1; j >= 0; --j)
                {
                    *image = fire[i * SCREEN_WIDTH + j];
                    image--;
                }
            }*/
        }


    }

    public class Plasma
    {
        private const int SCREEN_HEIGHT = 200;
        private const int SCREEN_WIDTH = 320;

        private ushort _pos1, _pos3, _tpos1, _tpos2, _tpos3, _tpos4;
        private readonly int[] _aSin = new int[512];
        private readonly Color[] _palette = new Color[256];

        WriteableBitmap bitmapPixels = new WriteableBitmap(SCREEN_WIDTH, SCREEN_HEIGHT, 96, 96, PixelFormats.Bgra32, null);
        int[] pixels = new int[SCREEN_WIDTH * SCREEN_HEIGHT];

        public Plasma()
        {
            _CreatePalette();
            _CreateSineTable();
        }

        private void _CreateSineTable()
        {
            for (var i = 0; i < 512; i++)
            {
                var rad = (i * 0.703125) * 0.0174532;
                _aSin[i] = (int)(Math.Sin(rad) * 1024);
            }
        }

        private void _CreatePalette()
        {
            for (var i = 0; i < 64; ++i)
            {
                var r = i << 2;
                var g = 255 - ((i << 2) + 1);
                _palette[i] = Color.FromArgb(255, (byte)r, (byte)g, 0);
                g = (i << 2) + 1;
                _palette[i + 64] = Color.FromArgb(255, 255, (byte)g, 0);
                r = 255 - ((i << 2) + 1);
                g = 255 - ((i << 2) + 1);
                _palette[i + 128] = Color.FromArgb(255, (byte)r, (byte)g, 0);
                g = (i << 2) + 1;
                _palette[i + 192] = Color.FromArgb(255, 0, (byte)g, 0);
            }
        }

        private float lastMaximum = 0.0f;
        private int lastDirection = 0;
        private DateTime lastDirectionChanged = DateTime.MinValue;

        public WriteableBitmap CreateFrame()
        {
            _tpos4 = 0;
            _tpos3 = _pos3;

            for (var i = 0; i < SCREEN_HEIGHT; ++i)
            {
                _tpos1 = (ushort)(_pos1 + 5);
                _tpos2 = 3;

                _tpos3 &= 511;
                _tpos4 &= 511;

                for (var j = 0; j < SCREEN_WIDTH; ++j)
                {
                    _tpos1 &= 511;
                    _tpos2 &= 511;

                    // plasma calculation
                    var x = _aSin[_tpos1] + _aSin[_tpos2] + _aSin[_tpos3] + _aSin[_tpos4];

                    // reference into palette
                    var index = (byte)(128 + (x >> 4));

                    // get color 
                    var c = _palette[index];

                    // shift it onto bitmap
                    pixels[i * SCREEN_WIDTH + j] = c.A << 24 | c.R << 16 | c.G << 8 | c.B;

                    _tpos1 += 5;
                    _tpos2 += 3;
                }

                _tpos4 += 3;
                _tpos3 += 1;
            }

            /* move plasma */

            float[][] spectrum  = SoundEngine.SoundEngine.Instance.GetSpectrum();

            float maximum = 0;
            for (int channel = 0; channel < 2; channel++)
            {
                for (int i = 0; i < spectrum[channel].Length / 50; i++)
                {
                    //if (spectrum[0][i] > maximum)
                    maximum += spectrum[channel][i];
                }
            }

            if (maximum - lastMaximum > 8 && DateTime.Now - lastDirectionChanged > TimeSpan.FromMilliseconds(2000))
            {
                if (lastDirection == 0)
                    lastDirection = 1;
                else
                    lastDirection = 0;

                lastDirectionChanged = DateTime.Now;
            }

            if (lastDirection == 0)
            {
                _pos1 += (ushort)(maximum + 1);
                _pos3 += (ushort)(maximum + 1);
            }
            else
            {
                _pos1 -= (ushort)(maximum + 1);
                _pos3 -= (ushort)(maximum + 1);
            }
            lastMaximum = maximum;

            Int32Rect rect = new Int32Rect(0, 0, bitmapPixels.PixelWidth, bitmapPixels.PixelHeight);

            int stride = bitmapPixels.PixelWidth * 4;

            bitmapPixels.WritePixels(rect, pixels, stride, 0);

            return bitmapPixels;
        }

    }
}
