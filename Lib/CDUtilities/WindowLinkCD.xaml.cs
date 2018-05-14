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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowLinkCD.xaml
    /// </summary>
    public partial class WindowLinkCD : Window
    {
        private DataBase dataBase;
        private CD cdInDrive;

        public WindowLinkCD(DataBase dataBase, CD cdInDrive)
        {
            InitializeComponent();

            this.dataBase = dataBase;
            this.cdInDrive = cdInDrive;

            // Diese Sachen direkt speichern, wenn einmal gesucht wurde.
            checkBoxSameTrackCount.IsChecked = (int)Settings.GetValue("LinkSameNumber", 1) == 1 ? true : false;
            checkBoxNotAssigned.IsChecked = (int)Settings.GetValue("LinkNotLinked", 1) == 1 ? true : false;

            Search();

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonShowCD.IsEnabled = (DataGridResult.SelectedIndex >= 0);
            buttonOK.IsEnabled = (DataGridResult.SelectedIndex >= 0);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void DataGridResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void DataGridResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                ShowDetails();
            }
        }

        private void buttonShowCD_Click(object sender, RoutedEventArgs e)
        {
            ShowDetails();
        }

        private void ShowDetails()
        {
            CDItemResult selectedItem = DataGridResult.SelectedItem as CDItemResult;
            CD selectedCD = dataBase.GetCDById(selectedItem.CDID);

            WindowAlbum windowAlbum = new WindowAlbum(selectedCD, dataBase);

            windowAlbum.ShowDialog();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
	        Big3.Hitbase.DataBaseEngine.Condition sel = new Big3.Hitbase.DataBaseEngine.Condition();
	
        	// Diese Sachen direkt speichern, wenn einmal gesucht wurde.
	        Settings.SetValue("LinkSameNumber", checkBoxSameTrackCount.IsChecked == true ? (int)1 : (int)0);
	        Settings.SetValue("LinkNotLinked", checkBoxNotAssigned.IsChecked == true ? (int)1 : (int)0);

            // Nur CDs.
            sel.Add(Field.AlbumType, Operator.Equal, 0);
            
            if (this.checkBoxNotAssigned.IsChecked == true)
		        sel.Add(Field.Identity, Operator.Empty, 0);
	
	        if (this.checkBoxSameTrackCount.IsChecked == true)
	        {
		        sel.Add(Field.NumberOfTracks, Operator.Equal, cdInDrive.NumberOfTracks);
	        }

	        if (textBoxArtist.Text != "")
		        sel.Add(Field.ArtistCDName, Operator.Contains, textBoxArtist.Text);

	        if (textBoxTitle.Text != "")
		        sel.Add(Field.Title, Operator.Contains, textBoxTitle.Text);

	        SortFieldCollection sortKeys = new SortFieldCollection();
	        sortKeys.Add(Field.ArtistCDName);
	        sortKeys.Add(Field.Title);

            FieldCollection fc = new FieldCollection();
            fc.Add(Field.ArtistCDName);
            fc.Add(Field.Title);
            fc.Add(Field.TotalLength);
            fc.Add(Field.NumberOfTracks);

            List<CDItemResult> items = new List<CDItemResult>();

	        using (DataBaseView albumView = AlbumView.CreateView(dataBase, fc, sortKeys, 0, sel))
            {
                object[] values;

                while ((values = albumView.Read()) != null)
                {
                    CDItemResult newItem = new CDItemResult();
                    newItem.CDID = (int)values[0];
                    newItem.Artist = (string)values[1];
                    newItem.Title = values[2] is DBNull ? "" : (string)values[2];
                    newItem.Length = (int)values[3];
                    newItem.NumberOfTracks = (int)values[4];
                    
                    items.Add(newItem);
	            }
            }

            DataGridResult.ItemsSource = items;

	        return ;
        }

        public class CDItemResult
        {
            public int CDID { get; set; }
            public string Identity { get; set; }
            public string Artist { get; set; }
            public string Title { get; set; }
            public int Length { get; set; }
            public int NumberOfTracks { get; set; }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            CDItemResult selectedItem = DataGridResult.SelectedItem as CDItemResult;
            CD selectedCD = dataBase.GetCDById(selectedItem.CDID);
		
		    if (selectedCD.ID == cdInDrive.ID)
		    {
			    MessageBox.Show(StringTable.CDAlreadyLinkedToThisCD, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
			    return;
		    }

            string sql = string.Format("UPDATE CD Set [Identity]='' WHERE [Identity]='{0}'", cdInDrive.Identity);
            int count = dataBase.ExecuteNonQuery(sql);

		    selectedCD.TotalLength = cdInDrive.TotalLength;
		    selectedCD.NumberOfTracks = cdInDrive.NumberOfTracks;
            selectedCD.InitTracks(cdInDrive.Tracks.Count);
		
		    selectedCD.Identity = cdInDrive.Identity;
		
		    // Jus 10-Apr-98: Liedlängen werden auch von der eingelegten CD übernommen!
		    for (int i=0;i < cdInDrive.Tracks.Count;i++)
			    selectedCD.Tracks[i].Length = cdInDrive.Tracks[i].Length;

		    selectedCD.Save(dataBase);

            DialogResult = true;
            Close();
        }
    }
}
