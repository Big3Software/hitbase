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
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.RecordMedium
{
    /// <summary>
    /// Interaction logic for NormalizeOptionsUserControl.xaml
    /// </summary>
    public partial class NormalizeOptionsUserControl : UserControl, IModalUserControl
    {
        public NormalizeOptionsUserControl()
        {
            InitializeComponent();

            ToggleButtonNormalize.IsChecked = Settings.Current.NormalizeActive;
            numericBoxNormalize.Value = Settings.Current.NormalizePercent;
            numericBoxMin.Value = Settings.Current.NormalizePercentMin;
            numericBoxMax.Value = Settings.Current.NormalizePercentMax;

            UpdateView();
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(this, new EventArgs());

            Settings.Current.NormalizeActive = (bool)ToggleButtonNormalize.IsChecked;
            Settings.Current.NormalizePercent = (int)numericBoxNormalize.Value;
            Settings.Current.NormalizePercentMin = (int)numericBoxMin.Value;
            Settings.Current.NormalizePercentMax = (int)numericBoxMax.Value;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }
        private void UpdateView()
        {
            if (ToggleButtonNormalize.IsChecked == true)
            {
                textBlockInfoMinMax.Visibility = System.Windows.Visibility.Visible;
                textBlockNormPercent.Visibility = System.Windows.Visibility.Visible;
                textBlockAmpUnder.Visibility = System.Windows.Visibility.Visible;
                textBlockAmpOver.Visibility = System.Windows.Visibility.Visible;
                numericBoxMin.Visibility = System.Windows.Visibility.Visible;
                numericBoxMax.Visibility = System.Windows.Visibility.Visible;
                numericBoxNormalize.Visibility = System.Windows.Visibility.Visible;
                textBlockPercent1.Visibility = System.Windows.Visibility.Visible;
                textBlockPercent2.Visibility = System.Windows.Visibility.Visible;
                textBlockPercent3.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                textBlockInfoMinMax.Visibility = System.Windows.Visibility.Hidden;
                textBlockNormPercent.Visibility = System.Windows.Visibility.Hidden;
                textBlockAmpUnder.Visibility = System.Windows.Visibility.Hidden;
                textBlockAmpOver.Visibility = System.Windows.Visibility.Hidden;
                numericBoxMin.Visibility = System.Windows.Visibility.Hidden;
                numericBoxMax.Visibility = System.Windows.Visibility.Hidden;
                numericBoxNormalize.Visibility = System.Windows.Visibility.Hidden;
                textBlockPercent1.Visibility = System.Windows.Visibility.Hidden;
                textBlockPercent2.Visibility = System.Windows.Visibility.Hidden;
                textBlockPercent3.Visibility = System.Windows.Visibility.Hidden;
            }

        }
        private void ToggleButtonNormalize_Checked(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private void ToggleButtonNormalize_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private void buttonDefault_Click(object sender, RoutedEventArgs e)
        {
            numericBoxNormalize.Value = 98;
            numericBoxMin.Value = 85;
            numericBoxMax.Value = 99;
        }
    }
}
