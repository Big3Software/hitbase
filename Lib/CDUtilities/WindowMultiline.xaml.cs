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
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowMultiline.xaml
    /// </summary>
    public partial class WindowMultiline : Window
    {
        public WindowMultiline()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.HideMinimizeAndMaximizeButtons();

            textBox.Focus();
        }
    }
}
