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

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for WaitProgressUserControl.xaml
    /// </summary>
    public partial class WaitProgressUserControl : UserControl
    {
        public WaitProgressUserControl()
        {
            InitializeComponent();
        }

        public bool Canceled { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Canceled = true;
        }
    }
}
