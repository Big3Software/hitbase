using Big3.Hitbase.Miscellaneous;
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

namespace Big3.Hitbase.CDCover
{
    /// <summary>
    /// Interaction logic for SizeAndPositionUserControl.xaml
    /// </summary>
    public partial class SizeAndPositionUserControl : UserControl, IModalUserControl
    {
        public SizeAndPositionUserControl(CoverModel coverModel)
        {
            InitializeComponent();

            this.DataContext = coverModel;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(sender, e);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(sender, e);
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;
    }
}
