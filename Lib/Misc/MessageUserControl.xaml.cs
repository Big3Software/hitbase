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
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Miscellaneous
{
    /// <summary>
    /// Interaction logic for MessageUserControl.xaml
    /// </summary>
    public partial class MessageUserControl : UserControl, IModalUserControl
    {
        public MessageUserControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(MessageUserControl), new UIPropertyMetadata(""));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MessageUserControl), new UIPropertyMetadata(""));




        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(MessageUserControl), new UIPropertyMetadata(""));




        public WpfMessageBoxButtons WpfMessageBoxButtons
        {
            get { return (WpfMessageBoxButtons)GetValue(WpfMessageBoxButtonsProperty); }
            set { SetValue(WpfMessageBoxButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WpfMessageBoxButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WpfMessageBoxButtonsProperty =
            DependencyProperty.Register("WpfMessageBoxButtons", typeof(WpfMessageBoxButtons), typeof(MessageUserControl), new UIPropertyMetadata(WpfMessageBoxButtons.None));





        public WpfMessageBoxResult Result
        {
            get { return (WpfMessageBoxResult)GetValue(WpfMessageBoxResultProperty); }
            set { SetValue(WpfMessageBoxResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WpfMessageBoxResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WpfMessageBoxResultProperty =
            DependencyProperty.Register("WpfMessageBoxResult", typeof(WpfMessageBoxResult), typeof(MessageUserControl), new UIPropertyMetadata(WpfMessageBoxResult.None));


        private bool zoomImage;

        public bool ZoomImage
        {
            get { return zoomImage; }
            set
            {
                zoomImage = value;
                this.image.Stretch = (value == true) ? Stretch.Uniform : Stretch.None;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (WpfMessageBoxButtons)
            {
                case WpfMessageBoxButtons.None:
                    break;
                case WpfMessageBoxButtons.YesNo:
                    ButtonOK.Content = StringTable.Yes;
                    ButtonCancel.Content = StringTable.No;
                    break;
                case WpfMessageBoxButtons.DeleteCancel:
                    ButtonOK.Content = StringTable.Delete;
                    ButtonCancel.Content = StringTable.Cancel;
                    break;
                default:
                    break;
            }
        }

    }
}
