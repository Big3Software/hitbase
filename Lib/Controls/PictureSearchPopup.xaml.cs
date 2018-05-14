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
using System.Windows.Controls.Primitives;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for PictureSearchPopup.xaml
    /// </summary>
    public partial class PictureSearchPopup : Popup
    {
        public event EventHandler PictureSelected;

        public PictureSearchPopup()
        {
            InitializeComponent();
        }

        private void PictureSearchUserControl_CloseClicked(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        public void Search(string searchText)
        {
            PictureSearchUserControl.Search(searchText);
        }

        private void PictureSearchUserControl_PictureSelected(object sender, EventArgs e)
        {
            if (PictureSelected != null)
                PictureSelected(sender, e);
        }
    }
}
