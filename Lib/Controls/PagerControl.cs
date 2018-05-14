using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Controls
{
    public partial class PagerControl : UserControl
    {
        public delegate void PageChangedDelegate();
        public event PageChangedDelegate PageChanged;

        public PagerControl()
        {
            InitializeComponent();
        }

        private int numberOfPages;
        /// <summary>
        /// Die Anzahl der Seiten
        /// </summary>
        public int NumberOfPages
        {
            get { return numberOfPages; }
            set 
            { 
                numberOfPages = value;
                labelPages.Text = string.Format(StringTable.Pages, CurrentPage + 1, NumberOfPages);

                panelPages.Controls.Clear();
                CreatePageLabels();
            }
        }

        private int showPageNumberCount = 6;
        /// <summary>
        /// Die Anzahl der Seitennummern, die angezeigt werden soll.
        /// </summary>
        public int ShowPageNumberCount
        {
            get { return showPageNumberCount; }
            set
            {
                showPageNumberCount = value; 

                panelPages.Controls.Clear();
                CreatePageLabels();
            }
        }

        private int currentPage;
        /// <summary>
        /// Die Aktuelle Seite.
        /// </summary>
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        private void CreatePageLabels()
        {
            int realShowPageNumberCount = showPageNumberCount;
            if (ShowPageNumberCount > NumberOfPages)
            {
                realShowPageNumberCount = NumberOfPages;
            }

            int startPage = currentPage - realShowPageNumberCount / 2;
            if (startPage < 0)
                startPage = 0;
            if (startPage > NumberOfPages - realShowPageNumberCount)
                startPage = NumberOfPages - realShowPageNumberCount;
            int endPage = startPage + realShowPageNumberCount;
            if (endPage > NumberOfPages)
                endPage = NumberOfPages;

            Point pos = new Point(0, 0);
            int controlNumber = 0;
            for (int i = startPage+1; i < endPage+1; i++)
            {
                LinkLabel pageLabel;
                bool newControl = false;

                if (controlNumber >= panelPages.Controls.Count)
                {
                    pageLabel = new LinkLabel();
                    newControl = true;
                }
                else
                    pageLabel = panelPages.Controls[controlNumber] as LinkLabel;
                pageLabel.Text = i.ToString();
                pageLabel.Location = pos;
                pageLabel.AutoSize = true;

                if (i == CurrentPage + 1)
                    pageLabel.Font = new Font(LinkLabel.DefaultFont.FontFamily, 12, FontStyle.Bold);
                else
                    pageLabel.Font = new Font(LinkLabel.DefaultFont.FontFamily, 12, FontStyle.Regular);
                if (newControl)
                    pageLabel.Click += new EventHandler(pageLabel_Click);
                pageLabel.Tag = i-1;

                
                pageLabel.Enabled = (i <= NumberOfPages);
                    
                if (newControl)
                    panelPages.Controls.Add(pageLabel);
                pos.X += pageLabel.Width + 3;
                controlNumber++;
            }

            labelPages.Text = string.Format(StringTable.Pages, CurrentPage + 1, NumberOfPages);
        }

        void pageLabel_Click(object sender, EventArgs e)
        {
            LinkLabel linkLabel = sender as LinkLabel;
            if (linkLabel != null)
            {
                int page = (int)linkLabel.Tag;
                CurrentPage = page;
                CreatePageLabels();

                if (PageChanged != null)
                    PageChanged();
            }
        }

        private void PagerControl_Load(object sender, EventArgs e)
        {
            CreatePageLabels();
        }

        private void linkLabelFirst_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CurrentPage = 0;
            CreatePageLabels();
            if (PageChanged != null)
                PageChanged();
        }

        private void linkLabelPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentPage > 0)
                CurrentPage--;
            CreatePageLabels();
            if (PageChanged != null)
                PageChanged();
        }

        private void linkLabelNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentPage < NumberOfPages - 1)
                CurrentPage++;
            CreatePageLabels();
            if (PageChanged != null)
                PageChanged();
        }

        private void linkLabelLast_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (NumberOfPages == 0)
                CurrentPage = 0;
            else
                CurrentPage = NumberOfPages - 1;
            CreatePageLabels();
            if (PageChanged != null)
                PageChanged();
        }
    }
}
