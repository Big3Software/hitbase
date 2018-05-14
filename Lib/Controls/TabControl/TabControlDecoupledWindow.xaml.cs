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
    /// Interaction logic for TabControlDecoupledWindow.xaml
    /// </summary>
    public partial class TabControlDecoupledWindow : Window
    {
        public TabControlDecoupledWindow()
        {
            InitializeComponent();
        }

        public string ImageResourceString { get; set; }
    }
}
