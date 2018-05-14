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
using Big3.Hitbase.SharedResources;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for UploadCatalogWindow.xaml
    /// </summary>
    public partial class UploadCatalogWindow : Window
    {
        public UploadCatalogWindow()
        {
            InitializeComponent();
        }

        private bool canceled = false;

        public DataBase DataBase { get; set; }

        SafeObservableCollection<UploadDownloadItem> uploadItems = new SafeObservableCollection<UploadDownloadItem>();

        private int cdCount = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cdCount = (int)DataBase.ExecuteScalar("SELECT count(*) FROM CD WHERE [Type]=0");
            string str = string.Format(StringTable.StartUploadCatalog, cdCount);

            if (MessageBox.Show(str, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                Close();
                return;
            }


            dataGrid.ItemsSource = uploadItems;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                DoUpload();
            };
            bw.RunWorkerAsync();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.HideMinimizeAndMaximizeButtons();
        }

        private void DoUpload()
        {
    	    // Zuerst alle CDs
	        Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters.CDTableAdapter cdAdap = new Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters.CDTableAdapter(DataBase);
	        CDDataSet.CDDataTable cdQuery = cdAdap.GetDataByAudioCD();

            Dispatcher.BeginInvoke(new Action(delegate
                {
                    TextBlockTitle.Text = string.Format(StringTable.UploadCatalog, cdCount);
                    ProgressBarUpload.Maximum = cdCount;
                }
            )); 

	        int iCount = 0;

            CCDArchive cdarchive = new CCDArchive();

	        foreach (CDDataSet.CDRow cdRow in cdQuery)
	        {
		        if (canceled)
			        break;

		        CD cd = DataBase.GetCDById(cdRow.CDID);

                if (cd.Type != AlbumType.AudioCD)       // Nur richtige CDs
                    continue;

                Dispatcher.BeginInvoke(new Action(delegate
                    {
                        ProgressBarUpload.Value = iCount;
                    }
                ));

                UploadDownloadItem newItem = new UploadDownloadItem();
                newItem.Artist = cd.Artist;
                newItem.Title = cd.Title;
                newItem.Status = StringTable.SendingData + "...";

                Dispatcher.BeginInvoke(new Action(delegate
                    {
                        uploadItems.Add(newItem);
                        dataGrid.UpdateLayout();
                        dataGrid.ScrollIntoView(newItem);
                    }
                ));

		        int iResult = 0;
                string status = "";

                try
                {
                    cdarchive.Upload(cd, ref iResult, IntPtr.Zero, 0);

		            switch ((CDArchivUploadResults)iResult)
		            {
		                case CDArchivUploadResults.UPLOAD_ARCHIV_ERROR:
		                case CDArchivUploadResults.UPLOAD_ARCHIV_CD_ERROR:
                            {
    			                status = StringTable.Failed;
                                Dispatcher.BeginInvoke(new Action(delegate
                                    {
                                        TextBoxDetails.Text += cdarchive.m_sLastDetailMessage + "\r\n\r\n";
                                    }
                                ));
			                    break;
                            }
		                case CDArchivUploadResults.UPLOAD_ARCHIV_CD_EXISTS:
			                status = StringTable.CDExists;
			                break;
		                case CDArchivUploadResults.UPLOAD_ARCHIV_NEW_CD:
			                status = StringTable.Success;
			                break;
		                default:
			                System.Diagnostics.Debug.Assert(false);
                            break;
		            }
                }
                catch (Exception e)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        TextBoxDetails.Text += e.ToString() + "\r\n\r\n";
                    }
                    ));
                    status = StringTable.Failed;
                }
		
		        newItem.Status = status;

		        iCount++;
	        }

            Dispatcher.BeginInvoke(new Action(delegate
                {
                    TextBlockTitle.Text = StringTable.UploadCompleted;

        	        ButtonCancel.Content = StringTable.Close;
                }
            ));

            // Wenn fertig, dann setzen wir canceled auf true, damit im Cancel_Click Event das Fenster geschlossen wird.
            canceled = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (canceled)
            {
                Close();
            }

            canceled = true;
        }
    }

    public class UploadDownloadItem : INotifyPropertyChanged
    {
        private string artist;

        public string Artist
        {
            get 
            { 
                return artist; 
            }
            set 
            { 
                artist = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Artist"));
            }
        }

        private string title;

        public string Title
        {
            get 
            { 
                return title; 
            }
            set 
            { 
                title = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private string status;

        public string Status
        {
            get 
            { 
                return status; 
            }
            set 
            { 
                status = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
