using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Big3.Hitbase.DataBaseEngine;
using System.Drawing.Drawing2D;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class ArtistListBoxWithImage : ListBox
    {
        private Font fontBold = new Font("Arial", 8.25f, FontStyle.Bold);
        private Font fontNormal = new Font("Arial", 8.25f);
        private Font fontSmall = new Font("Arial", 7f, FontStyle.Underline);
        private ProfessionalColorTable ColorTable = new ProfessionalColorTable();

        private int Border = 5;

        private Dictionary<string, Image> imageCache = new Dictionary<string,Image>();

        public ArtistListBoxWithImage()
        {
            InitializeComponent();
        }

        public ArtistListBoxWithImage(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
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
            {
                LinearGradientBrush b = new LinearGradientBrush(e.Bounds, ProfessionalColors.ToolStripGradientBegin, ProfessionalColors.ToolStripGradientEnd, LinearGradientMode.Vertical);
                g.FillRectangle(b, e.Bounds);
            }

            int xpos = e.Bounds.Left + ItemHeight + Border * 2;

            PersonGroupDataSet.PersonGroupRow item = (PersonGroupDataSet.PersonGroupRow)Items[e.Index];

            g.DrawString(item.Name, fontBold, Brushes.Black, new PointF(xpos, e.Bounds.Top + Border));

            string artistType = DataBase.GetNameOfPersonGroupType((PersonGroupType)(item.IsTypeNull() ? 0 : item.Type));
            string artistSex = DataBase.GetNameOfPersonGroupSex((SexType)(item.IsSexNull() ? 0 : item.Sex));

            g.DrawString(artistType + ", " + artistSex, fontNormal, Brushes.Black, new PointF(xpos, e.Bounds.Top + 18));

            int yOffset = 36;

            if (!item.IsCountryNull() && item.Country.Length > 0)
            {
                g.DrawString(StringTable.Country + ": " + item.Country, fontNormal, Brushes.Black, new PointF(xpos, e.Bounds.Top + yOffset));
                yOffset = 54;
            }

            if (!item.IsBirthDayNull())
            {
                if ((PersonGroupType)item.Type == PersonGroupType.Single)
                    g.DrawString(StringTable.Born + ": " + Misc.FormatDate(item.BirthDay), fontNormal, Brushes.Black, new PointF(xpos, e.Bounds.Top + yOffset));
                else
                    g.DrawString(StringTable.Founded + ": " + Misc.FormatDate(item.BirthDay), fontNormal, Brushes.Black, new PointF(xpos, e.Bounds.Top + yOffset));
                //                yOffset += 16;
            }

            if (!item.IsDayOfDeathNull())
            {
                if ((PersonGroupType)item.Type == PersonGroupType.Single)
                    g.DrawString(StringTable.Died + ": " + Misc.FormatDate(item.DayOfDeath), fontNormal, Brushes.Black, new PointF(xpos + (Width - xpos) / 2, e.Bounds.Top + yOffset));
                else
                    g.DrawString(StringTable.BreakAway + ": " + Misc.FormatDate(item.DayOfDeath), fontNormal, Brushes.Black, new PointF(xpos + (Width - xpos) / 2, e.Bounds.Top + yOffset));
                yOffset += 16;
            }

            if (!item.IsImageFilenameNull() && item.ImageFilename.Length > 0)
            {
                if (!imageCache.ContainsKey(item.ImageFilename))
                {
                    try
                    {
                        byte[] imageBytes = File.ReadAllBytes(Misc.FindCover(item.ImageFilename));

                        MemoryStream m = new MemoryStream(imageBytes);

                        Image img = Image.FromStream(m);
                        m.Close();

                        imageCache.Add(item.ImageFilename, img);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (imageCache.ContainsKey(item.ImageFilename))
                {
                    Size origSize = new Size(ItemHeight - Border * 2, ItemHeight - Border * 2);
                    Size imageBestFitSize = GetBestFitSize(imageCache[item.ImageFilename], origSize);

                    Rectangle imageRect = new Rectangle(
                        new Point(e.Bounds.Left + Border + (origSize.Width - imageBestFitSize.Width) / 2,
                                  e.Bounds.Top + Border + (origSize.Height - imageBestFitSize.Height) / 2),
                                  imageBestFitSize);

                    g.DrawImage(imageCache[item.ImageFilename], imageRect);
                }
            }
            else
            {
                Size origSize = new Size(ItemHeight - Border * 2, ItemHeight - Border * 2);
                Size imageBestFitSize = GetBestFitSize(Images.MissingPersonImage, origSize);

                Rectangle imageRect = new Rectangle(
                    new Point(e.Bounds.Left + Border + (origSize.Width - imageBestFitSize.Width) / 2,
                              e.Bounds.Top + Border + (origSize.Height - imageBestFitSize.Height) / 2),
                              imageBestFitSize);

                g.DrawImage(Images.MissingPersonImage, imageRect);
            }

            if (!item.IsURLNull())
            {
                g.DrawString(item.URL, fontSmall, Brushes.Blue, new PointF(xpos + (Width - xpos) / 2, e.Bounds.Top + 10));
            }
        }

        public void ClearImageCache()
        {
            imageCache.Clear();
        }

        protected override void Sort()
        {
            QuickSort(0, Items.Count - 1);
        }

        private void QuickSort(int left, int right)
        {
            if (right > left)
            {
                int pivotIndex = left;
                int pivotNewIndex = QuickSortPartition(left, right, pivotIndex);

                QuickSort(left, pivotNewIndex - 1);
                QuickSort(pivotNewIndex + 1, right);
            }
        }

        private int QuickSortPartition(int left, int right, int pivot)
        {
            var pivotValue = (PersonGroupDataSet.PersonGroupRow)Items[pivot];
            Swap(pivot, right);

            int storeIndex = left;
            for (int i = left; i < right; ++i)
            {
                if (string.Compare(pivotValue.Name, ((PersonGroupDataSet.PersonGroupRow)Items[i]).Name, true) >= 0)
                {
                    Swap(i, storeIndex);
                    ++storeIndex;
                }
            }

            Swap(storeIndex, right);
            return storeIndex;
        }

        private void Swap(int left, int right)
        {
            var temp = Items[left];
            Items[left] = Items[right];
            Items[right] = temp;
        }

        /// <summary>
        /// Liefert die optimale Größe zurück, wenn der Aspect-Ratio beibehalten werden soll.
        /// </summary>
        /// <param name="origSize"></param>
        /// <returns></returns>
        private Size GetBestFitSize(Image img, Size boundingSize)
        {
            Size bestFitSize = new Size();
            double ratio = (double)img.Width / (double)img.Height;
            
            if (img.Width > img.Height)
            {
                bestFitSize.Width = boundingSize.Width;
                bestFitSize.Height = (int)((double)boundingSize.Width / ratio);
            }
            else
            {
                bestFitSize.Height = boundingSize.Height;
                bestFitSize.Width = (int)((double)boundingSize.Height * ratio);
            }

            return bestFitSize;
        }

        private bool PointInURL(int index, Point pt)
        {
            Rectangle rect = this.GetItemRectangle(index);

            PersonGroupDataSet.PersonGroupRow item = (PersonGroupDataSet.PersonGroupRow)Items[index];

            int xpos = rect.Left + ItemHeight + Border * 2;

            string url = item.URL;
            if (pt.X > xpos + (Width - xpos) / 2 &&
                pt.X < xpos + (Width - xpos) / 2 + CreateGraphics().MeasureString(url, fontSmall).Width &&
                pt.Y > rect.Top + 10 &&
                pt.Y < rect.Top + 10 + CreateGraphics().MeasureString(url, fontSmall).Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ArtistListBoxWithImage_MouseMove(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.X, e.Y);

            if (index > -1)
            {
                if (PointInURL(index, e.Location))
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ArtistListBoxWithImage_MouseDown(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.X, e.Y);

            if (index < 0)
                return;

            PersonGroupDataSet.PersonGroupRow item = (PersonGroupDataSet.PersonGroupRow)Items[index];

            if (index > -1)
            {
                if (PointInURL(index, e.Location))
                {
                    Process.Start(item.URL);
                }
            }
        }
    }
}
