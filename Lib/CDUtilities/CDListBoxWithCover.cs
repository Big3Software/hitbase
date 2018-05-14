using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class CDListBoxWithCover : ListBox
    {
        private Font fontBold = new Font("Arial", 8.25f, FontStyle.Bold);
        private Font fontNormal = new Font("Arial", 8.25f);
        private Font fontSmall = new Font("Arial", 7f, FontStyle.Underline);
        private ProfessionalColorTable ColorTable = new ProfessionalColorTable();

        private int Border = 5;

        public CDListBoxWithCover()
        {
            InitializeComponent();
        }

        public CDListBoxWithCover(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            imageList.Images.Add(Images.StarLight);
            imageList.Images.Add(Images.StarDark);
            imageList.Images.Add(Images.StarHalf);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            //base.OnDrawItem(e);
            if (e.Index >= Items.Count || e.Index < 0)
                return;

            Graphics g = e.Graphics;

            if ((e.State & DrawItemState.Selected) != 0)
            {
                g.FillRectangle(new SolidBrush(ColorTable.MenuItemSelected), e.Bounds);
                Rectangle rect = Rectangle.FromLTRB(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom - 1);
                g.DrawRectangle(new Pen(ColorTable.MenuItemBorder), rect);
            }
            else
                g.FillRectangle(Brushes.White, e.Bounds);

            CDListBoxWithCoverItem item = (CDListBoxWithCoverItem)Items[e.Index];

            string title = item.Title;
            if (item.CD != null && item.CD.Length > 1)
            {
                title += string.Format(" [{0} {1}]", item.CD.Length, StringTable.CDs);
            }

            g.DrawString(title, fontBold, Brushes.Black, new PointF(e.Bounds.Left + ItemHeight + Border * 2, e.Bounds.Top + Border));
            g.DrawString(item.Artist, fontNormal, Brushes.Black, new PointF(e.Bounds.Left + ItemHeight + Border * 2, e.Bounds.Top + 18));

            g.DrawString(item.Label, fontNormal, Brushes.Black, new PointF(e.Bounds.Left + ItemHeight + Border * 2, e.Bounds.Top + 42));

            string releaseDate = "";
            if (item.Year != null && item.Year.Length == 10)
            {
                DateTime dateTime = new DateTime(Convert.ToInt32(item.Year.Substring(0, 4)), Convert.ToInt32(item.Year.Substring(5, 2)), Convert.ToInt32(item.Year.Substring(8, 2)));
                releaseDate = string.Format("{0}: {1}", StringTable.ReleaseDate, dateTime.ToLongDateString());
            }

            if (item.Year != null && item.Year.Length == 7)              // Dann wurde wohl nur Jahr und Monat angegeben
            {
                DateTime dateTime = new DateTime(Convert.ToInt32(item.Year.Substring(0, 4)), Convert.ToInt32(item.Year.Substring(5, 2)), 1);

                releaseDate = string.Format("{0}: {1}", StringTable.ReleaseDate, dateTime.ToString("MMMM yyyy"));
            }

            if (item.Year != null && item.Year.Length == 4)              // Dann wurde wohl nur ein Jahr angegeben
            {
                releaseDate = string.Format("{0}: {1}", StringTable.ReleaseDate, item.Year.ToString());
            }

            g.DrawString(releaseDate, fontNormal, Brushes.Black, new PointF(e.Bounds.Left + ItemHeight + Border * 2, e.Bounds.Top + 55));

            if (item.Image != null)
                g.DrawImage(item.Image, new Rectangle(new Point(e.Bounds.Left + Border, e.Bounds.Top + Border), new Size(ItemHeight - Border * 2, ItemHeight - Border * 2)));

            int starXPos = e.Bounds.Right - 200;
            for (int i = 0; i < (int)item.Ranking; i++)
            {
                g.DrawImage(imageList.Images[0], new Point(starXPos, e.Bounds.Top + Border));
                starXPos += imageList.Images[0].Width;
            }

            if (item.Ranking != 0)
            {
                int count = (int)item.Ranking;
                float f = Convert.ToSingle(item.Ranking);
                if (f - (float)((int)item.Ranking) > 0.25 && f - (float)((int)item.Ranking) < 0.75)
                {
                    g.DrawImage(imageList.Images[2], new Point(starXPos, e.Bounds.Top + Border));
                    starXPos += imageList.Images[2].Width;
                    count++;
                }

                for (int i = count; i < 5; i++)
                {
                    g.DrawImage(imageList.Images[1], new Point(starXPos, e.Bounds.Top + Border));
                    starXPos += imageList.Images[1].Width;
                }
            }

            string more = StringTable.More + "...";
            g.DrawString(more, fontSmall, Brushes.Blue, new PointF(e.Bounds.Right - Border * 2 - e.Graphics.MeasureString(more, fontSmall).Width, e.Bounds.Top + 10));
        }

        private void CDListBoxWithCover_MouseMove(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.X, e.Y);

            if (index > -1)
            {
                if (PointInMore(index, e.Location))
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                string tooltip = "";
                CDListBoxWithCoverItem item = (CDListBoxWithCoverItem)Items[index];
                if (item.CD != null)
                {
                    for (int i = 0; i < item.CD[0].Tracks.Length; i++)
                    {
                        tooltip += string.Format("{0}. {1}", i + 1, item.CD[0].Tracks[i]);
                        if (i < item.CD[0].Tracks.Length - 1)
                            tooltip += "\r\n";
                    }
                }

                if (toolTip.GetToolTip(this) != tooltip)
                {
                    toolTip.SetToolTip(this, "");
                    toolTip.SetToolTip(this, tooltip);
                }
            }
            else
            {
                toolTip.SetToolTip(this, "");
            }
        }

        private bool PointInMore(int index, Point pt)
        {
            Rectangle rect = this.GetItemRectangle(index);

            string more = StringTable.More + "...";
            if (pt.X > rect.Right - Border * 2 - CreateGraphics().MeasureString(more, fontSmall).Width &&
                pt.X < rect.Right - Border * 2 &&
                pt.Y > rect.Top + 10 &&
                pt.Y < rect.Top + 10 + CreateGraphics().MeasureString(more, fontSmall).Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CDListBoxWithCover_MouseDown(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.X, e.Y);

            if (index > -1)
            {
                if (PointInMore(index, e.Location))
                {
                    // ACHTUNG! Dieser String muss eventuell angepasst werden, 
                    // wenn wir bei Amazon mal einen anderen Link nutzen müssen, oder
                    // eine neue Partner-ID bekommen.
                    string url = string.Format("http://www.amazon.de/exec/obidos/redirect?link_code=ur2&camp=1638&tag=hitbase-21&creative=6742&path=external-search%3Fsearch-type=ss%26keyword={0}%26index=music", ((CDListBoxWithCoverItem)Items[index]).ASIN);
                    try
                    {
                        Process.Start(url);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(StringTable.ErrorAmazonWebSiteOpen + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }

    public class CDItem
    {
        public string[] Tracks;
    }

    public class CDListBoxWithCoverItem
    {
        public string ASIN;
        public Image Image;
        public string Artist;
        public string Title;
        public string Year;
        public string Label;
        public int NumberOfCDs;
        public CDItem[] CD;
        public string EditorNotes;
        public decimal Ranking;
        public string EAN;

        public string smallImageUrl;
        public string mediumImageUrl;
        public string largeImageUrl;

        public string backCoverSmallImageUrl;
        public string backCoverMediumImageUrl;
        public string backCoverLargeImageUrl;
    }

}
