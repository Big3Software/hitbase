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

namespace Community
{
    /// <summary>
    /// Interaction logic for ColorChooseSlider.xaml
    /// </summary>
    public partial class ColorChooseSlider : UserControl
    {
        public delegate void ColorChoosenHandler(Color color);
        public event ColorChoosenHandler ColorChoosen;

        GradientStopCollection gsc = new GradientStopCollection();

        Color[] colorValues = new Color[258];

        public ColorChooseSlider()
        {
            InitializeComponent();

            gsc.Add(new GradientStop(Color.FromRgb(255, 0,   0), 0));
            gsc.Add(new GradientStop(Color.FromRgb(255, 255, 0), 0.16));
            gsc.Add(new GradientStop(Color.FromRgb(0,   255, 0), 0.33));
            gsc.Add(new GradientStop(Color.FromRgb(0, 255, 255), 0.5));
            gsc.Add(new GradientStop(Color.FromRgb(0, 0, 255), 0.66));
            gsc.Add(new GradientStop(Color.FromRgb(255, 0, 255), 0.83));
            gsc.Add(new GradientStop(Color.FromRgb(255, 0, 0), 1));
            LinearGradientBrush lgb = new LinearGradientBrush(gsc, 90);
            gridColor.Background = lgb;

            int numberOfSteps = 258;
            int count = 0;
            for (int index = 0; index < gsc.Count-1; index++)
            {
                Color colorStart = gsc[index].Color;
                Color colorEnd = gsc[index+1].Color;

                int stepValues = numberOfSteps / (gsc.Count - 1);
                for (int i = 0; i < stepValues; i++)
                {
                    int r = colorStart.R + (int)(((double)colorEnd.R - (double)colorStart.R) / (double)stepValues * (double)i);
                    int g = colorStart.G + (int)(((double)colorEnd.G - (double)colorStart.G) / (double)stepValues * (double)i);
                    int b = colorStart.B + (int)(((double)colorEnd.B - (double)colorStart.B) / (double)stepValues * (double)i);

                    colorValues[count++] = Color.FromRgb((byte)r, (byte)g, (byte)b);
                }

            }

            sliderColor.Minimum = 0;
            sliderColor.Maximum = 257;
            sliderColor.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderColor_ValueChanged);
        }

        void sliderColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ColorChoosen != null)
                ColorChoosen(colorValues[257-(int)e.NewValue]);
        }
    }
}
