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
using System.IO;
using System.Windows.Resources;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for FirstStepsUserControl.xaml
    /// </summary>
    public partial class FirstStepsUserControl : UserControl
    {
        public FirstStepsUserControl()
        {
            InitializeComponent();

            CheckBoxShowAtStartUp.IsChecked = Settings.Current.DontShowFirstSteps;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Settings.Current.DontShowFirstSteps = (bool)CheckBoxShowAtStartUp.IsChecked;
        }

        private void CheckBoxShowAtStartUp_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Current.DontShowFirstSteps = (bool)CheckBoxShowAtStartUp.IsChecked;
        }

        private void CheckBoxShowAtStartUp_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.DontShowFirstSteps = (bool)CheckBoxShowAtStartUp.IsChecked;
        }

    }
}
