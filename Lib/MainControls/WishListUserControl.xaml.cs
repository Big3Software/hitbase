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
using Big3.Hitbase.CDUtilities;
using System.ComponentModel;
using Big3.Hitbase.Miscellaneous;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Xml;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for WishListUserControl.xaml
    /// </summary>
    public partial class WishListUserControl : UserControl, INotifyPropertyChanged, Big3.Hitbase.Controls.DragDrop.IDropTarget
    {
        public event EventHandler Closed;

        public WishListUserControl()
        {
            InitializeComponent();

            DataContext = this;

        }

        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private void RequestClose()
        {
            if (Closed != null)
                Closed(this, new EventArgs());
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RequestClose();
        }


        private Wishlist wishlist;

        public Wishlist Wishlist
        {
            get { return wishlist; }
            set 
            { 
                wishlist = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Wishlist"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void listBoxWishlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem lbi = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
                if (lbi != null)
                {
                    WishlistItem wishlistItem = lbi.DataContext as WishlistItem;

                    FormWishlistItem formWishlistItem = new FormWishlistItem(wishlistItem);
                    formWishlistItem.ShowDialog();
                }
            }
        }

        private void CommandBindingSaveWishlist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                XmlSerializer bf = new XmlSerializer(typeof(Wishlist));
                XmlTextWriter tw = new XmlTextWriter(saveFileDialog.FileName, Encoding.UTF8);
                bf.Serialize(tw, wishlist);
                tw.Close();
            }
        }

        public new void DragOver(Controls.DragDrop.IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = Big3.Hitbase.Controls.DragDrop.DropTargetAdorners.Insert;

            if (dropInfo.Data is PlaylistItem)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }

            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }

            if (dropInfo.Data is MyMusicListItem || dropInfo.Data is List<MyMusicListItem>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public new void Drop(Controls.DragDrop.IDropInfo dropInfo)
        {
            List<WishlistItem> wishlistItems = new List<WishlistItem>();

            if (dropInfo.Data is PlaylistItem)
            {
                PlaylistItem playlistItem = dropInfo.Data as PlaylistItem;

                WishlistItem item = new WishlistItem();
                item.Artist = playlistItem.Info.Artist;
                item.Title = playlistItem.Info.Title;
                item.TrackID = playlistItem.ID;
                wishlistItems.Add(item);
            }

            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                // Prüfen, ob ein Track einer CD gedropped wurde
                MainCDUserControl mainCDUserControl = VisualTreeExtensions.FindParent<MainCDUserControl>(dropInfo.DragInfo.VisualSource);
                if (mainCDUserControl != null)
                {
                }
                else
                {
                    Track track = dropInfo.Data as Track;
                    List<Track> trackList = dropInfo.Data as List<Track>;

                    if (track != null)
                    {
                        WishlistItem item = GetWishlistItemByTrack(track);
                        wishlistItems.Add(item);
                    }

                    if (trackList != null)
                    {
                        foreach (Track trackItem in trackList)
                        {
                            WishlistItem item = GetWishlistItemByTrack(track);
                            wishlistItems.Add(item);
                        }
                    }

                }
            }

            if (dropInfo.Data is MyMusicListItem)
            {
                MyMusicListItem item = dropInfo.Data as MyMusicListItem;
                Track track = DataBase.GetTrackById(item.ID);

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                addTracksParams.Filenames.Add(track.Soundfile);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is List<MyMusicListItem>)
            {
                List<MyMusicListItem> items = dropInfo.Data as List<MyMusicListItem>;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                foreach (MyMusicListItem item in items)
                {
                    addTracksParams.Filenames.Add(item.Soundfile);
                }
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (wishlistItems.Count > 0)
            {
                HitbaseCommands.AddToWishlist.Execute(wishlistItems, System.Windows.Application.Current.MainWindow);
            }
        }

        private WishlistItem GetWishlistItemByTrack(Track track)
        {
            WishlistItem item = new WishlistItem();

            item.TrackID = track.ID;
            item.Title = track.Title;
            item.Artist = track.Artist;
//??            item.ImageFilename = playlistItem.TrackImage;

            return item;
        }

        #region Print
        private System.Drawing.Printing.PrintDocument printDocument;
        private int currentPrintRecord;
        private int currentPage;

        private void CommandBindingPrintWishlist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.PrintPreviewDialog previewDialog = new System.Windows.Forms.PrintPreviewDialog();

            printDocument = new System.Drawing.Printing.PrintDocument();
            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument_PrintPage);

            previewDialog.Document = printDocument;

            currentPrintRecord = 0;
            currentPage = 1;

            previewDialog.ShowDialog();
        }

        void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool printThisPage = (e.PageSettings.PrinterSettings.PrintRange == System.Drawing.Printing.PrintRange.AllPages ||
                currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                currentPage <= e.PageSettings.PrinterSettings.ToPage);

            int yPosition = e.MarginBounds.Top;

            PrintHeader(e.Graphics, e.MarginBounds, ref yPosition);

            for (int i = currentPrintRecord; i < wishlist.Count; i++)
            {
                WishlistItem item = wishlist[i];

                if (yPosition + printFont.Height > e.MarginBounds.Bottom)
                {
                    currentPrintRecord = i;
                    yPosition = e.MarginBounds.Top;

                    if (e.PageSettings.PrinterSettings.PrintRange == System.Drawing.Printing.PrintRange.SomePages &&
                        currentPage >= e.PageSettings.PrinterSettings.ToPage)
                    {
                        e.HasMorePages = false;
                        break;
                    }

                    if (printThisPage)
                    {
                        currentPage++;
                        e.HasMorePages = true;
                        break;
                    }

                    currentPage++;

                    printThisPage = (e.PageSettings.PrinterSettings.PrintRange == System.Drawing.Printing.PrintRange.AllPages ||
                        currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                        currentPage <= e.PageSettings.PrinterSettings.ToPage);

                    if (printThisPage)
                        PrintHeader(e.Graphics, e.MarginBounds, ref yPosition);
                }

                int xPosition = e.MarginBounds.Left;

                System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
                sf.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;

                int xSize = e.MarginBounds.Width / 4;
                e.Graphics.DrawString(item.Artist, printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.Title, printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.From, printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.Comment, printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;

                yPosition += printFont.Height * 6 / 5;
            }
        }

        private System.Drawing.Font printFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Regular);
        private System.Drawing.Font printFontBold = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
        private System.Drawing.Font printFontHeader = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);

        void PrintHeader(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, ref int yPosition)
        {
            int xPosition = bounds.Left;

            System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
            sf.Alignment = System.Drawing.StringAlignment.Center;
            g.DrawString(StringTable.Wishlist, printFontHeader, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = System.Drawing.StringAlignment.Far;         // Seitennummer, rechtsbündig
            sf.LineAlignment = System.Drawing.StringAlignment.Far;
            g.DrawString(string.Format("{0} {1}", StringTable.Page, currentPage), printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = System.Drawing.StringAlignment.Near;         // Datum, linkgsbündig
            g.DrawString(DateTime.Now.ToShortDateString(), printFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            yPosition += printFontHeader.Height + printFont.Height;

            int xSize = bounds.Width / 4;
            g.DrawString(StringTable.Artist, printFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.Title, printFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.From, printFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.Comment, printFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(xPosition, yPosition));
            xPosition += xSize;

            yPosition += printFont.Height;

            g.DrawLine(System.Drawing.Pens.Black, bounds.Left, yPosition, bounds.Right, yPosition);

            yPosition += printFont.Height / 3;
        }
        #endregion

        private void CommandBindingDeleteWishlistItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void DeleteSelectedItems()
        {
            if (listBoxWishlist.SelectedIndex >= 0)
            {
                int oldSelectedIndex = listBoxWishlist.SelectedIndex;
                int nextFocusIndex;
                ListBoxItem lbi = null;

                for (int i = listBoxWishlist.SelectedItems.Count - 1; i >= 0; i--)
                {
                    Wishlist.Remove((WishlistItem)listBoxWishlist.SelectedItems[i]);
                }

                if (oldSelectedIndex >= listBoxWishlist.Items.Count)
                    nextFocusIndex = listBoxWishlist.Items.Count - 1;
                else
                    nextFocusIndex = oldSelectedIndex;
                if (nextFocusIndex >= 0 && nextFocusIndex < listBoxWishlist.Items.Count)
                    lbi = listBoxWishlist.ItemContainerGenerator.ContainerFromIndex(nextFocusIndex) as ListBoxItem;

                listBoxWishlist.SelectedIndex = oldSelectedIndex;
                if (lbi != null)
                    lbi.Focus();
            }
        }


        private void CommandBindingDeleteWishlistItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listBoxWishlist.SelectedIndex >= 0;
        }
    }
}
