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

namespace Big3.Hitbase.SoundEngineGUI
{
    /// <summary>
    /// Interaction logic for CrossFadeUserDefinedSecondsUserControl.xaml
    /// </summary>
    public partial class CrossFadeUserDefinedSecondsUserControl : UserControl, IModalUserControl
    {
        public CrossFadeUserDefinedSecondsUserControl()
        {
            InitializeComponent();
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        public int Seconds { get; set; }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Seconds = Misc.Atoi(textBoxSeconds.Text);
            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxSeconds.Text = Seconds.ToString();
        }
    }
}
