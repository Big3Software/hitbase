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
using System.Windows.Shapes;
using System.Diagnostics;

namespace Big3.Hitbase.Application
{
    /// <summary>
    /// Interaction logic for WindowAbout.xaml
    /// </summary>
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();

            FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string version = string.Format("{0}.{1}.{2}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);

            textBlockVersion.Text = string.Format("Version {0}", version);

            if (App.IsBetaVersion)
            {
                textBlockLicense1.Foreground = Brushes.Red;
                textBlockLicense1.Text = App.VersionString;

                textBlockLicense2.Foreground = Brushes.Red;
                textBlockLicense2.Text = "Diese Version ist lauffähig bis: " + App.BetaExpirationDate.ToShortDateString();
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
