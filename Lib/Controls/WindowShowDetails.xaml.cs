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

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for WindowShowDetails.xaml
    /// </summary>
    public partial class WindowShowDetails : Window
    {
        public WindowShowDetails(string details)
        {
            InitializeComponent();

            TextBoxExceptionDetails.Text = details;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBoxExceptionDetails.Text);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
