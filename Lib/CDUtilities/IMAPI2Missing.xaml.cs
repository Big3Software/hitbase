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

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for IMAPI2Missing.xaml
    /// </summary>
    public partial class IMAPI2Missing : Window
    {
        public IMAPI2Missing()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/de-de/details.aspx?familyid=b5f726f1-4ace-455d-bad7-abc4dd2f147b&displaylang=de");
        }


        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
