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

namespace Big3.Hitbase.Miscellaneous
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionWindow.xaml
    /// </summary>
    public partial class UnhandledExceptionWindow : Window
    {
        public UnhandledExceptionWindow(Exception e)
        {
            InitializeComponent();

            TextBoxExceptionDetails.Text = e.ToString();
        }

        public UnhandledExceptionWindow(Exception e, string additionalInfo)
        {
            InitializeComponent();

            TextBoxExceptionDetails.Text = additionalInfo + "\r\n" + e.ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBoxExceptionDetails.Text);
        }
    }
}
