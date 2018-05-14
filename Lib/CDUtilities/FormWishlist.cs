using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Drawing.Printing;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormWishlist : Form
    {
        private DataBase database;
        private Wishlist wishlist;

        //TODO_WPF!!!!!!!!!!!!!public event OnAddTracksToPlaylist AddTracksToPlaylist;

        private PrintDocument printDocument;
        private int currentPrintRecord;
        private int currentPage;

        public FormWishlist(DataBase database, Wishlist wishlist)
        {
            InitializeComponent();

            this.database = database;
            this.wishlist = wishlist;

            FillWishlist();
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonProperties.Enabled = (listViewWishlistItems.SelectedItems.Count > 0);
            buttonDelete.Enabled = (listViewWishlistItems.SelectedItems.Count > 0);
            buttonPlayNext.Enabled = (listViewWishlistItems.SelectedItems.Count > 0);
            buttonPlayImmediatly.Enabled = (listViewWishlistItems.SelectedItems.Count > 0); 
        }

        public void FillWishlist()
        {
            if (InvokeRequired)
            {
                // Anderer Thread
                Invoke(new MethodInvoker(FillWishlist));
                return;
            }

            listViewWishlistItems.Items.Clear();

            foreach (WishlistItem wli in wishlist)
            {
                ListViewItem newItem = listViewWishlistItems.Items.Add(wli.Artist);
                newItem.SubItems.Add(wli.Title);
                newItem.SubItems.Add(wli.From);
                newItem.SubItems.Add(wli.Comment);
                newItem.Tag = wli;
            }

            UpdateWindowState();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            ShowProperties();
        }

        private void ShowProperties()
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;
            FormWishlistItem formWishlistItem = new FormWishlistItem(wli);
            if (formWishlistItem.ShowDialog(this) == DialogResult.OK)
            {
                FillWishlist();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;
            wishlist.Remove(wli);
            FillWishlist();
        }

        private void buttonPlayNext_Click(object sender, EventArgs e)
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;
            string[] filenames = new string[1];

            Track track = database.GetTrackById(wli.TrackID);
            if (track != null)
            {
                filenames[0] = track.Soundfile;
                /*TODO_WPF!!!!!!!!!!!!!if (AddTracksToPlaylist != null)
                    AddTracksToPlaylist(filenames, AddToPlaylistType.Next);*/
            }
        }

        private void buttonPlayImmediatly_Click(object sender, EventArgs e)
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;
            string[] filenames = new string[1];

            Track track = database.GetTrackById(wli.TrackID);
            if (track != null)
            {
                filenames[0] = track.Soundfile;
                /*TODO_WPF!!!!!!!!!!!!!!if (AddTracksToPlaylist != null)
                    AddTracksToPlaylist(filenames, AddToPlaylistType.Now);*/
            }
        }

        private void listViewWishlistItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer bf = new XmlSerializer(typeof(Wishlist));
                XmlTextWriter tw = new XmlTextWriter(saveFileDialog.FileName, Encoding.UTF8);
                bf.Serialize(tw, wishlist);
                tw.Close();
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();

            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            previewDialog.Document = printDocument;

            currentPrintRecord = 0;
            currentPage = 1;

            previewDialog.ShowDialog();
        }

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool printThisPage = (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages ||
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

                    if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages &&
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

                    printThisPage = (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages ||
                        currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                        currentPage <= e.PageSettings.PrinterSettings.ToPage);

                    if (printThisPage)
                        PrintHeader(e.Graphics, e.MarginBounds, ref yPosition);
                }

                int xPosition = e.MarginBounds.Left;

                StringFormat sf = new StringFormat();
                sf.Trimming = StringTrimming.EllipsisCharacter;

                int xSize = e.MarginBounds.Width / 4;
                e.Graphics.DrawString(item.Artist, printFont, Brushes.Black, new RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.Title, printFont, Brushes.Black, new RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.From, printFont, Brushes.Black, new RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;
                e.Graphics.DrawString(item.Comment, printFont, Brushes.Black, new RectangleF(xPosition, yPosition, xSize, printFont.Height), sf);
                xPosition += xSize;

                yPosition += printFont.Height * 6 / 5;
            }
        }

        private Font printFont = new Font("Arial", 8, FontStyle.Regular);
        private Font printFontBold = new Font("Arial", 8, FontStyle.Bold);
        private Font printFontHeader = new Font("Arial", 12, FontStyle.Bold);

        void PrintHeader(Graphics g, Rectangle bounds, ref int yPosition)
        {
            int xPosition = bounds.Left;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            g.DrawString(StringTable.Wishlist, printFontHeader, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = StringAlignment.Far;         // Seitennummer, rechtsbündig
            sf.LineAlignment = StringAlignment.Far;
            g.DrawString(string.Format("{0} {1}", StringTable.Page, currentPage), printFont, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = StringAlignment.Near;         // Datum, linkgsbündig
            g.DrawString(DateTime.Now.ToShortDateString(), printFont, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            yPosition += printFontHeader.Height + printFont.Height;

            int xSize = bounds.Width / 4;
            g.DrawString(StringTable.Artist, printFont, Brushes.Black, new PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.Title, printFont, Brushes.Black, new PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.From, printFont, Brushes.Black, new PointF(xPosition, yPosition));
            xPosition += xSize;
            g.DrawString(StringTable.Comment, printFont, Brushes.Black, new PointF(xPosition, yPosition));
            xPosition += xSize;

            yPosition += printFont.Height;

            g.DrawLine(Pens.Black, bounds.Left, yPosition, bounds.Right, yPosition);

            yPosition += printFont.Height / 3;
        }

        private void listViewWishlistItems_DoubleClick(object sender, EventArgs e)
        {
            ShowProperties();
        }
    }
}
