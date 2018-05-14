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
using System.ComponentModel;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    /// <summary>
    /// Interaction logic for RatingUserControl.xaml
    /// </summary>
    public partial class RatingUserControl : UserControl
    {
        public event EventHandler ValueChanged;

        public RatingUserControl()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(RatingUserControl_MouseMove);
            this.MouseLeave += new MouseEventHandler(RatingUserControl_MouseLeave);
            
            Cursor = Cursors.Hand;
            
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ReadOnlyProperty, typeof(RatingUserControl));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate
                {
                    if (ReadOnly)
                        Cursor = Cursors.Arrow;
                    else
                        Cursor = Cursors.Hand;

                });
            }

            CreatePanel();
        }

        private void CreatePanel()
        {
            for (int i = 1; i <= 6; i++)
            {
                Image img = new Image();
                img.Height = 15;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/starDark.png"));
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
                Panel.Children.Add(img);
            }
        }

        void RatingUserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            UpdatePanel(Value);
        }

        void RatingUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (ReadOnly)
                return;

            int previewRating = (int)(e.GetPosition(this).X + 14) / 16;
            if (previewRating > 6)
                previewRating = 6;
            UpdatePanel(previewRating);
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set 
            { 
                SetValue(ValueProperty, value); 
                UpdatePanel(Value);
                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RatingUserControl), new UIPropertyMetadata(0));

        private void UpdatePanel(int rating)
        {
            for (int i=1;i<=6;i++)
            {
                Image img = Panel.Children[i-1] as Image;
                if (i <= rating)
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/star.png"));
                else
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/starDark.png"));
            }
        }

        void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ReadOnly)
                return;

            int previewRating = (int)(e.GetPosition(this).X + 14) / 16;
            if (previewRating > 6)
                previewRating = 6;

            Value = previewRating;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePanel(Value);
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set 
            { 
                SetValue(ReadOnlyProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(RatingUserControl), new UIPropertyMetadata(false));
    }
}
