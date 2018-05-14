using System;
using System.Collections.Generic;
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
	/// Interaction logic for WaitProgress.xaml
	/// </summary>
	public partial class WaitProgress : UserControl
	{
		public WaitProgress()
		{
			this.InitializeComponent();
		}

        public Color WaitProgressColor
        {
            get { return (Color)GetValue(WaitProgressColorProperty); }
            set { SetValue(WaitProgressColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitProgressColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitProgressColorProperty =
            DependencyProperty.Register(
                "WaitProgressColor", 
                typeof(Color), 
                typeof(WaitProgress), 
                new UIPropertyMetadata(new PropertyChangedCallback(WaitProgressColorPropertyCallback)));



        public string WaitText
        {
            get { return (string)GetValue(WaitTextProperty); }
            set { SetValue(WaitTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitTextProperty =
            DependencyProperty.Register("WaitText", typeof(string), typeof(WaitProgress), new UIPropertyMetadata(new PropertyChangedCallback(WaitTextPropertyCallback)));



        private static void WaitProgressColorPropertyCallback(
            DependencyObject obj, 
            DependencyPropertyChangedEventArgs e)
        {
            WaitProgress wp = obj as WaitProgress;
            wp.Ellipse1.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse2.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse3.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse4.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse5.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse6.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse7.Fill = new SolidColorBrush((Color)e.NewValue);
            wp.Ellipse8.Fill = new SolidColorBrush((Color)e.NewValue);
        }

        private static void WaitTextPropertyCallback(
            DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            WaitProgress wp = obj as WaitProgress;

            wp.WaitTextBlock.Text = (string)e.NewValue;
            wp.WaitTextBlock.Visibility = Visibility.Visible;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Storyboard sb = this.FindResource("IndicatorStoryboard") as Storyboard;

            if ((bool)e.NewValue == true)
            {
                sb.Begin();
            }
            else
            {
                sb.Stop();
            }
        }

	}
}