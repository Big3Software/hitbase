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
using System.Windows.Media.Animation;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for WP7WaitUserControl.xaml
    /// </summary>
    public partial class WP7WaitUserControl : UserControl
    {
        public WP7WaitUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private DoubleAnimation CreateInAnimation(double beginTime)
        {
            DoubleAnimation da = new DoubleAnimation(-Width/2 - 100, Width/2 + 100, TimeSpan.FromMilliseconds(6000).Duration());
            OutInCubicEase ease = new OutInCubicEase();
            ease.EasingMode = EasingMode.EaseInOut;
            da.EasingFunction = ease;
            da.BeginTime = TimeSpan.FromMilliseconds(beginTime);
            da.RepeatBehavior = RepeatBehavior.Forever;
            return da;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                double beginTime = 2000;

                foreach (UIElement rect in GridRects.Children)
                {
                    TranslateTransform tt = new TranslateTransform();
                    tt.X = -9999;               // Damit man die Punkt initial nicht sieht
                    rect.RenderTransform = tt;

                    DoubleAnimation da = CreateInAnimation(beginTime);

                    tt.BeginAnimation(TranslateTransform.XProperty, da);
                    beginTime -= 200;
                }
            }
            else
            {
                foreach (UIElement rect in GridRects.Children)
                {
                    rect.RenderTransform = null;
                }
            }
        }
    }

    internal class OutInCubicEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return Math.Pow(normalizedTime, 0.1);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new OutInCubicEase();
        }
    }
}
