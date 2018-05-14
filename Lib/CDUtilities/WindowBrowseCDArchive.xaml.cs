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
using Big3.Hitbase.Configuration;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using System.Net;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowBrowseCDArchive.xaml
    /// </summary>
    public partial class WindowBrowseCDArchive : Window
    {
        private DataBase dataBase;

        public CD CurrentCD { get; set; }

        public WindowBrowseCDArchive(DataBase dataBase)
        {
            InitializeComponent();

            this.dataBase = dataBase;

            SourceInitialized += new EventHandler(WindowBrowseCDArchive_SourceInitialized);

            Settings.RestoreWindowPlacement(this, "WindowBrowseCDArchive");
        }

        void WindowBrowseCDArchive_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillCDArchives();

            UpdateWindowState();
        }

        private void FillCDArchives()
        {
            CCDArchive cdarchive = new CCDArchive();
            
            foreach (CDArchiveConfig archivConfig in Settings.Current.CDArchives)
	        {
		        if (cdarchive.IsArchiveBrowseable(archivConfig))
		        {
			        ComboBoxCDArchives.Items.Add(new ComboBoxArchiveItem() { ArchiveConfig = archivConfig });
		        }
	        }

            if (ComboBoxCDArchives.Items.Count > 0)
                ComboBoxCDArchives.SelectedIndex = 0;
        }

        private void UpdateWindowState()
        {
            ButtonSearch.IsEnabled = !string.IsNullOrEmpty(TextBoxArtist.Text) ||
                !string.IsNullOrEmpty(TextBoxTitle.Text) ||
                !string.IsNullOrEmpty(TextBoxUPC.Text);

            ButtonShowDetails.IsEnabled = (DataGridResult.SelectedIndex >= 0);
            ButtonImportCDs.IsEnabled = (DataGridResult.SelectedIndex >= 0);
        }

        private void TextBoxArtist_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void TextBoxTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void TextBoxUPC_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void DataGridResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            progressBarSearch.Visibility = System.Windows.Visibility.Visible;

            ButtonSearch.IsEnabled = false;
            TextBlockStatus.Text = StringTable.OneMomentPlease + "...";

            CDArchiveConfig cdArchiveConfig = ((ComboBoxArchiveItem)ComboBoxCDArchives.SelectedItem).ArchiveConfig;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                if (cdArchiveConfig.Type == CDArchiveType.BIG3)
                {
                    SearchCDArchive(cdArchiveConfig);
                }
                if (cdArchiveConfig.Type == CDArchiveType.CDArchiveLocal)
                {
                    SearchCDArchiveLocal(cdArchiveConfig);
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                progressBarSearch.Visibility = System.Windows.Visibility.Collapsed;
                UpdateWindowState();
            };
            bw.RunWorkerAsync();

        }

        void SearchCDArchiveLocal(CDArchiveConfig cdArchiveConfig)
        {
	        int maxCount = 100;         // Z.zt. fest


	        String filename = cdArchiveConfig.ArchiveName;
	        if (string.IsNullOrEmpty(filename))
	        {
		        MessageBox.Show(StringTable.NoCDArchiveDefined, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
		        return;
	        }

            String filter = "";
            string artist = "";
            string title = "";
            string upc = "";
            string numberOfTracks = "";

            Dispatcher.Invoke((Action)(() =>
            {
                artist = this.TextBoxArtist.Text;
                title = this.TextBoxTitle.Text;
                upc = this.TextBoxUPC.Text;
                numberOfTracks = this.TextBoxNumberOfTracks.Text;
            }));

	        if (!string.IsNullOrEmpty(artist))
		        filter += "sArtist like '%" + artist + "%'";
	        
            if (!string.IsNullOrEmpty(title))
	        {
		        if (!string.IsNullOrEmpty(filter))
			        filter += " AND ";

		        filter += "sTitle like '%" + title + "%'";
	        }
	
            if (!string.IsNullOrEmpty(upc))
            {
	            if (!string.IsNullOrEmpty(filter))
		            filter += " AND ";

	            filter += "sUPC='" + upc + "'";
            }

	        if (Misc.Atoi(numberOfTracks) != 0)
	        {
		        if (!string.IsNullOrEmpty(filter))
			        filter += " AND ";

		        filter += string.Format("CD.cNumberOfTracks={0}", Misc.Atoi(numberOfTracks));
	        }

	        if (string.IsNullOrEmpty(filter))
	        {
		        return;
	        }

	        string sql;
	        sql = string.Format("SELECT * FROM CD INNER JOIN Artist ON CD.IDArtist = Artist.ID WHERE {0} ORDER BY Artist.sArtist, CD.sTitle", filter);

            List<CDItemResult> cdList = new List<CDItemResult>();
            int iCount = 0;

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source='" + filename + "'"))
            {
                conn.Open();

                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn);

                using (System.Data.OleDb.OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CDItemResult newItem = new CDItemResult();
                        newItem.Identity = (string)reader["sIdentity"];
                        newItem.Artist = (string)reader["sArtist"];
                        newItem.Title = (string)reader["sTitle"];
                        newItem.Length = (int)reader["dwTotalLength"];
                        newItem.NumberOfTracks = (int)(byte)reader["cNumberOfTracks"];

                        cdList.Add(newItem);
                        iCount++;
                    }

                }
            }

            Dispatcher.Invoke((Action)(() =>
            {
                DataGridResult.ItemsSource = cdList;

    	        String strResult;
	            strResult = string.Format("{0} {1}", iCount, StringTable.CDsFound);
	            TextBlockStatus.Text = strResult;
            }));
        }

        void SearchCDArchive(CDArchiveConfig cdArchiveConfig)
        {
	        String sURL = "";
	        int iMaxCount = 100;         // Z.zt. fest

            Dispatcher.Invoke((Action)(() =>
            {
    	        if (Misc.Atoi(TextBoxNumberOfTracks.Text) > 0)
	    	        sURL = string.Format("cdquery_browse.asp?User=hitbase&Password=hitbase2k&Artist={0}&Title={1}&UPC={2}&NumberOfTracks={3}&MaxCount={4}", TextBoxArtist.Text, TextBoxTitle.Text, TextBoxUPC.Text, Misc.Atoi(TextBoxNumberOfTracks.Text), iMaxCount);
	            else
		            sURL = string.Format("cdquery_browse.asp?User=hitbase&Password=hitbase2k&Artist={0}&Title={1}&UPC={2}&MaxCount={3}", TextBoxArtist.Text, TextBoxTitle.Text, TextBoxUPC.Text, iMaxCount);
            }));

            WebClient wc = new WebClient();
            wc.UseDefaultCredentials = true;
            wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string content = wc.DownloadString("http://" + cdArchiveConfig.ArchiveName + "/" + sURL);

            content = content.Replace("\r", "");
	        string[] saLines;

	        saLines = content.Split('\n');

	        //!!!!!!!!! Versionsnummer abfragen

            List<CDItemResult> cdList = new List<CDItemResult>();

	        bool bError = false;
	        int iLine = 1;
	        int iCount = 0;
	        while (iLine < saLines.Length)
	        {
		        String sValueID = "";
		        String sValueArtist = "";
		        String sValueTitle = "";
		        String sValueTotalLength = "";
		        String sValueNumberOfTracks = "";

		        if (iLine >= saLines.Length || !GetValue(saLines[iLine++], "ID", ref sValueID))
		        {
			        bError = true;
			        break;
		        }

		        if (iLine >= saLines.Length || !GetValue(saLines[iLine++], "Artist", ref sValueArtist))
		        {
			        bError = true;
			        break;
		        }

		        if (iLine >= saLines.Length || !GetValue(saLines[iLine++], "Title", ref sValueTitle))
		        {
			        bError = true;
			        break;
		        }

		        if (iLine >= saLines.Length || !GetValue(saLines[iLine++], "TotalLength", ref sValueTotalLength))
		        {
			        bError = true;
			        break;
		        }

		        if (iLine >= saLines.Length || !GetValue(saLines[iLine++], "NumberOfTracks", ref sValueNumberOfTracks))
		        {
			        bError = true;
			        break;
		        }

                CDItemResult newItem = new CDItemResult();
                newItem.Identity = sValueID;
                newItem.Artist = sValueArtist;
                newItem.Title = sValueTitle;
                newItem.Length = Misc.Atoi(sValueTotalLength);
                newItem.NumberOfTracks = Misc.Atoi(sValueNumberOfTracks);

                cdList.Add(newItem);

		        iCount++;
	        }

            Dispatcher.Invoke((Action)(() =>
            {
                DataGridResult.ItemsSource = cdList;

    	        String strResult;
	            strResult = string.Format("{0} {1}", iCount, StringTable.CDsFound);
	            TextBlockStatus.Text = strResult;
            }));
        }

        private bool GetValue(string sLine, string sParameter, ref string sValue)
        {
	        int iPos = sLine.IndexOf('=');
	        string sParamLower = sParameter;

	        if (iPos >= 0)
	        {
		        if (string.Compare(sLine.Left(iPos), sParamLower, true) != 0)
			        return false;

		        sValue = sLine.Mid(iPos+1);

		        iPos = sValue.IndexOf("<br>");
		        if (iPos >= 0)
			        sValue = sValue.Left(iPos);

		        return true;
	        }
	        else
	        {
		        return false;
	        }

        }

        private void ButtonShowDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowDetails();
        }

        private void ShowDetails()
        {
            CD cd = new CD();
            int error = 0;
            int canceled = 0;

            CDItemResult selItem = ((CDItemResult)this.DataGridResult.SelectedItem);
            if (selItem == null)
                return;

            CDArchiveConfig cdArchiveConfig = ((ComboBoxArchiveItem)ComboBoxCDArchives.SelectedItem).ArchiveConfig;

            if (cdArchiveConfig.Type == CDArchiveType.BIG3)
            {
                cd.Identity = selItem.Identity;

                CCDArchive cdarchive = new CCDArchive();

                cdarchive.SearchCDInInternetBig3(cdArchiveConfig, cd, ref canceled, ref error);
            }

            if (cdArchiveConfig.Type == CDArchiveType.CDArchiveLocal)
            {
                cd.Identity = selItem.Identity;

                CCDArchive cdarchive = new CCDArchive();

                CDArchiveLocalFile.SearchCDInCDArchiveLocalFile(cdArchiveConfig.ArchiveName, cd);
            }

            WindowAlbum windowAlbum = new WindowAlbum(cd, dataBase);
            windowAlbum.SaveAlbumOnOK = false;
            windowAlbum.ShowDialog();
        }

        public class CDItemResult
        {
            public string Identity { get; set; }
            public string Artist { get; set; }
            public string Title { get; set; }
            public int Length { get; set; }
            public int NumberOfTracks { get; set; }
        }

        private void DataGridResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                ShowDetails();
            }
        }

        private void ButtonImportCDs_Click(object sender, RoutedEventArgs e)
        {
        	// Alle markierten CDs in den aktuellen Katalog übernehmen.
            foreach (CDItemResult item in DataGridResult.SelectedItems)
            {
		        if (CurrentCD != null)
		        {
			        /*CString sSaveIdentity = CD->Identity;
			        int iSaveNumberOfTracks = CD->NumberOfTracks;
			        Big3::Hitbase::DataBaseEngine::CD^ cd = gcnew Big3::Hitbase::DataBaseEngine::CD();
			        BOOL bError = FALSE;
			        BOOL bCanceled = FALSE;

			        cd->Identity = gcnew String(m_saIdentities[iSelItem]);

			        theApp.m_CDArchives->DownloadCD(gcnew IntPtr(pHttpConnection), cd, bCanceled, bError, TRUE);

			        CD = cd;

			        CD->Identity = gcnew String(sSaveIdentity);
			        CD->NumberOfTracks = iSaveNumberOfTracks;*/
		        }
		        else
		        {
			        CD cd = new CD();
			        int error = 0;
			        int canceled = 0;

                    CDArchiveConfig cdArchiveConfig = ((ComboBoxArchiveItem)ComboBoxCDArchives.SelectedItem).ArchiveConfig;

			        if (cdArchiveConfig.Type == CDArchiveType.BIG3)
			        {
                        cd.Identity = item.Identity;

                        CCDArchive cdarchive = new CCDArchive();

                        cdarchive.SearchCDInInternetBig3(cdArchiveConfig, cd, ref canceled, ref error);
                    }

    			    if (cdArchiveConfig.Type == CDArchiveType.CDArchiveLocal)
			        {
                        cd.Identity = item.Identity;

                        CCDArchive cdarchive = new CCDArchive();

                        CDArchiveLocalFile.SearchCDInCDArchiveLocalFile(cdArchiveConfig.ArchiveName, cd);
                    } 

			        cd.Save(dataBase);
		        }
            }

            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.SaveWindowPlacement(this, "WindowBrowseCDArchive");
        }
    }

    public class ComboBoxArchiveItem
    {
        public ComboBoxArchiveItem()
        {
        }

        public CDArchiveConfig ArchiveConfig { get; set; }

        public override string ToString()
        {
            return ArchiveConfig.GetArchiveDisplayName();
        }
    }

}
