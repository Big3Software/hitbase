using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.Drawing.Printing;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CatalogView
{
    public partial class FormPrintCatalog : Form
    {
        private DataBase dataBase;

        private PrintDocument printDocument;
        private int currentPrintRecord;
        private int currentPrintTrackRecord = 0;
        private int currentPage;
        private int numberOfPages;
        private float headerHeight = 0;

        private Font headerFont;
        private Brush detailBrush;

        private PrintType printType = PrintType.CDList;

        private CDQueryDataSet cdQuery = null;
        private DataTable queryTable = null;
        private TrackDataView TrackView = null;
        private CDDataView CDView = null;

        private ColumnFieldCollection cdListFields = new ColumnFieldCollection();
        private FieldCollection detailListFields = new FieldCollection();
        private ColumnFieldCollection trackListFields = new ColumnFieldCollection();
        private ColumnFieldCollection trackDetailsListFields = new ColumnFieldCollection();

        private SortFieldCollection cdSortFields = new SortFieldCollection();
        private SortFieldCollection trackSortFields = new SortFieldCollection();
        private Condition printFilter = new Condition();

        private bool detailListPrintEmptyFields = false;
        private float detailListMaximumFieldWidth = 0;

        private CD cd;

        public bool CurrentCD { get; set; }

        public enum PrintType
        {
            CDList,
            DetailList,
            DetailListWithTracks,
            TrackList,
            LoanedCDList,
            CurrentCD
        }

        public FormPrintCatalog(DataBase db)
        {
            InitializeComponent();

            dataBase = db;

            cdListFields.Add(new ColumnField(Field.ArtistCDName));
            cdListFields.Add(new ColumnField(Field.Title));
            cdListFields.Add(new ColumnField(Field.NumberOfTracks));
            cdListFields.Add(new ColumnField(Field.TotalLength));

            detailListFields.Add(Field.ArtistCDName);
            detailListFields.Add(Field.Title);
            detailListFields.Add(Field.ComposerCDName);
            detailListFields.Add(Field.NumberOfTracks);
            detailListFields.Add(Field.TotalLength);
            detailListFields.Add(Field.Sampler);
            detailListFields.Add(Field.Category);
            detailListFields.Add(Field.Medium);
            detailListFields.Add(Field.Date);
            detailListFields.Add(Field.Codes);
            detailListFields.Add(Field.Price);
            detailListFields.Add(Field.User1);
            detailListFields.Add(Field.User2);
            detailListFields.Add(Field.User3);
            detailListFields.Add(Field.User4);
            detailListFields.Add(Field.User5);

            trackListFields.Add(new ColumnField(Field.ArtistTrackName));
            trackListFields.Add(new ColumnField(Field.TrackTitle));
            trackListFields.Add(new ColumnField(Field.TrackLength));

            trackDetailsListFields.Add(new ColumnField(Field.TrackNumber));
            trackDetailsListFields.Add(new ColumnField(Field.ArtistTrackName));
            trackDetailsListFields.Add(new ColumnField(Field.TrackTitle));
            trackDetailsListFields.Add(new ColumnField(Field.TrackLength));

            cdListFields = ColumnFieldCollection.LoadFromRegistry("PrintCDListFields", cdListFields);
            trackListFields = ColumnFieldCollection.LoadFromRegistry("PrintTrackListFields", trackListFields);
            trackDetailsListFields = ColumnFieldCollection.LoadFromRegistry("PrintTrackDetailsListFields", trackDetailsListFields);

            cdSortFields.Add(new SortField(Field.ArtistCDName));
            cdSortFields.Add(new SortField(Field.Title));

            trackSortFields.Add(new SortField(Field.ArtistTrackName));
            trackSortFields.Add(new SortField(Field.TrackTitle));

            headerFont = new Font("Arial", 10, FontStyle.Bold);
            detailBrush = Brushes.Black;

            toolStripComboBoxZoom.Items.Add(new ZoomLevel(10));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(25));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(50));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(75));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(100));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(125));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(150));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(200));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(300));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(400));
            toolStripComboBoxZoom.Items.Add(new ZoomLevel(ZoomType.WholePage));
            toolStripComboBoxZoom.SelectedIndex = 10;

            radioButtonCDList.Checked = true;
            checkBoxHeaderPrintSort.Checked = Settings.Current.PrintHeaderSort;
            checkBoxHeaderPrintFilter.Checked = Settings.Current.PrintHeaderFilter;
            checkBoxHeaderPrintDate.Checked = Settings.Current.PrintHeaderDate;
        }

        public FormPrintCatalog(DataBase db, CD cd) : this(db)
        {
            this.cd = cd;
            printType = PrintType.CurrentCD;
            radioButtonCurrentCD.Checked = true;
            UpdateWindowState();
        }

        void printDocument_EndPrint(object sender, PrintEventArgs e)
        {
            numberOfPages = currentPage + 1;

            toolStripLabelPage.Text = string.Format(StringTable.NumberOfPages, printPreviewControl.StartPage+1, numberOfPages);
        }

        void printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            currentPrintRecord = 0;
            currentPage = 0;

            switch (printType)
            {
                case PrintType.CDList:
                    {
                        cdQuery = dataBase.ExecuteCDQuery();
                        CDView = new CDDataView(dataBase, cdQuery, printFilter, cdSortFields, cdListFields.GetFields());
                        break;
                    }
                case PrintType.DetailList:
                    {
                        cdQuery = dataBase.ExecuteCDQuery();
                        DataView view = dataBase.GetCDQueryView(cdQuery, detailListFields, printFilter, cdSortFields);
                        queryTable = view.ToTable();
                        break;
                    }
                case PrintType.DetailListWithTracks:
                    {
                        cdQuery = dataBase.ExecuteCDQuery();
                        CDView = new CDDataView(dataBase, cdQuery, printFilter, cdSortFields);
                        break;
                    }
                case PrintType.TrackList:
                    {
                        cdQuery = dataBase.ExecuteTrackQuery();
                        TrackView = new TrackDataView(dataBase, cdQuery, printFilter, trackSortFields);

                        break;
                    }
            }
        }

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintHeader(e);
            PrintData(e);
            //PrintFooter(e);
        }

        private void PrintHeader(PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            RectangleF firstHeaderLine = new RectangleF(x, y, e.MarginBounds.Width, headerFont.Height);

            e.Graphics.DrawString(Application.ProductName, Settings.Current.PrintDetailsFont, detailBrush, firstHeaderLine);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            string listTitle = "";
            switch (printType)
            {
                case PrintType.CDList:
                    listTitle = StringTable.CDList;
                    break;
                case PrintType.DetailList:
                case PrintType.DetailListWithTracks:
                    listTitle = StringTable.DetailList;
                    break;
                case PrintType.TrackList:
                    listTitle = StringTable.TrackList;
                    break;
                case PrintType.LoanedCDList:
                    listTitle = StringTable.LoanedCDList;
                    break;
                case PrintType.CurrentCD:
                    if (CurrentCD)
                        listTitle = StringTable.CurrentCD;
                    else
                        listTitle = StringTable.PrintCD;
                    break;
            }

            if (!string.IsNullOrEmpty(textBoxHeaderText.Text))
                listTitle = textBoxHeaderText.Text;

            e.Graphics.DrawString(listTitle, headerFont, detailBrush, firstHeaderLine, sf);

            sf.Alignment = StringAlignment.Far;         // Seitennummer, rechtsbündig
            e.Graphics.DrawString(string.Format("{0} {1}", StringTable.Page, currentPage + 1), Settings.Current.PrintDetailsFont, detailBrush, firstHeaderLine, sf);

            y += firstHeaderLine.Height;

            if (Settings.Current.PrintHeaderSort || Settings.Current.PrintHeaderDate)
            {
                RectangleF rect = new RectangleF(x, y, e.MarginBounds.Width, Settings.Current.PrintDetailsFont.Height);
                sf.Alignment = StringAlignment.Center;         // Filter
                string sortBy;
                
                if (printType == PrintType.TrackList)
                    sortBy = trackSortFields.GetSortString(dataBase);
                else
                    sortBy = cdSortFields.GetSortString(dataBase);

                if (Settings.Current.PrintHeaderSort && printType != PrintType.CurrentCD)
                    e.Graphics.DrawString(string.Format(StringTable.SortBy, sortBy), Settings.Current.PrintDetailsFont, detailBrush, rect, sf);

                if (Settings.Current.PrintHeaderDate)
                {
                    sf.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString(DateTime.Now.ToShortDateString(), Settings.Current.PrintDetailsFont, detailBrush, rect, sf);
                }

                y += Settings.Current.PrintDetailsFont.Height;
            }

            if (Settings.Current.PrintHeaderFilter && printType != PrintType.CurrentCD)
            {
                RectangleF rect = new RectangleF(x, y, e.MarginBounds.Width, Settings.Current.PrintDetailsFont.Height);
                sf.Alignment = StringAlignment.Center;         // Filter
                string filter = printFilter.GetConditionString(dataBase);
                e.Graphics.DrawString(string.Format(StringTable.Filter, filter), Settings.Current.PrintDetailsFont, detailBrush, rect, sf);
                y += Settings.Current.PrintDetailsFont.Height;
            }

            headerHeight = y - e.MarginBounds.Top + Settings.Current.PrintDetailsFont.Height;

            switch (printType)
            {
                case PrintType.CDList:
                    PrintHeaderCDList(e);
                    break;
                case PrintType.DetailList:
                    PrintHeaderDetailList(e);
                    break;
                case PrintType.DetailListWithTracks:
                case PrintType.CurrentCD:
                    PrintHeaderDetailList(e);
                    break;
                case PrintType.TrackList:
                    PrintHeaderTrackList(e);
                    break;
            }
        }

        private void PrintHeaderCDList(PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            float xPos = x;
            float[] percentageWidths = cdListFields.GetPercentageWidths();

            for (int col = 0; col < cdListFields.Count; col++)
            {
                string value = dataBase.GetNameOfField(cdListFields[col].Field);

                StringFormat sf = new StringFormat();
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                RectangleF rect = new RectangleF(xPos, y+headerHeight, colWidth, Settings.Current.PrintDetailsFont.Height);

                e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);

                xPos += (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
            }
            headerHeight += Settings.Current.PrintDetailsFont.Height*1.2f;
            e.Graphics.DrawLine(new Pen(Color.Black, 1), x, y+headerHeight, x+e.MarginBounds.Width, y+headerHeight);
            headerHeight += Settings.Current.PrintDetailsFont.Height * 0.2f;
        }

        private void PrintHeaderDetailList(PrintPageEventArgs e)
        {
        }

        private void PrintHeaderTrackList(PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            float xPos = x;
            float[] percentageWidths = trackListFields.GetPercentageWidths();

            for (int col = 0; col < trackListFields.Count; col++)
            {
                string value = dataBase.GetNameOfField(trackListFields[col].Field);
//                SizeF size = e.Graphics.MeasureString(value, Settings.Current.PrintDetailsFont);

                StringFormat sf = new StringFormat();
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                RectangleF rect = new RectangleF(xPos, y + headerHeight, colWidth, Settings.Current.PrintDetailsFont.Height);

                e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);

                xPos += (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
            }
            headerHeight += Settings.Current.PrintDetailsFont.Height;
            e.Graphics.DrawLine(new Pen(Color.Black, 1), x, y + headerHeight, x + e.MarginBounds.Width, y + headerHeight);
            headerHeight += Settings.Current.PrintDetailsFont.Height * 0.5f;
        }

        private void PrintData(PrintPageEventArgs e)
        {
            switch (printType)
            {
                case PrintType.CDList:
                    PrintDataCDList(e);
                    break;
                case PrintType.DetailList:
                    PrintDataDetailList(e);
                    break;
                case PrintType.DetailListWithTracks:
                case PrintType.CurrentCD:
                    PrintDataDetailListWithTracks(e);
                    break;
                case PrintType.TrackList:
                    PrintDataTrackList(e);
                    break;
            }
        }

        private void PrintDataCDList(PrintPageEventArgs e)
        {
            float y = e.MarginBounds.Top + headerHeight;

            for (int i = currentPrintRecord; i < CDView.Rows.Count; i++)
            {
                if (y > e.MarginBounds.Bottom)
                {
                    if (printDocument.PrintController.IsPreview)
                    {
                        // Im Preview nur die ersten 10 Seiten anzeigen, dauert sonst eventuell zu lang.
                        if (currentPage >= 9)
                            break;
                    }

                    e.HasMorePages = true;
                    currentPrintRecord = i;
                    currentPage++;
                    return;
                }

                //DataRow dataRow = queryTable.Rows[i];

                int x = e.MarginBounds.Left;

                float[] percentageWidths = cdListFields.GetPercentageWidths();

                for (int col = 0; col < cdListFields.Count; col++)
                {
                    string value = CDView.GetRowStringValue(i, cdListFields[col].Field);
                    if (value != null)
                    {
                        StringFormat sf = new StringFormat();
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                        int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                        RectangleF rect = new RectangleF(x, y, colWidth, Settings.Current.PrintDetailsFont.Height);
                        e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);
                    }

                    x += (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                }

                y += Settings.Current.PrintDetailsFont.Height;
            }

            PrintFooter(e, y);

            e.HasMorePages = false;
        }

        private void PrintDataDetailList(PrintPageEventArgs e)
        {
            // Maximale Länge der Feldnamen ermitteln
            foreach (Field field in detailListFields)
            {
                SizeF size = e.Graphics.MeasureString(dataBase.GetNameOfField(field) + ": ", Settings.Current.PrintDetailsFont);
                if (size.Width > detailListMaximumFieldWidth)
                    detailListMaximumFieldWidth = size.Width;
            }

            float y = e.MarginBounds.Top + headerHeight;

            for (int i = currentPrintRecord; i < queryTable.Rows.Count; i++)
            {
                int itemPrintHeight = Settings.Current.PrintDetailsFont.Height + detailListFields.Count * Settings.Current.PrintDetailsFont.Height;
                if (y + itemPrintHeight > e.MarginBounds.Bottom)
                {
                    if (printDocument.PrintController.IsPreview)
                    {
                        // Im Preview nur die ersten 10 Seiten anzeigen, dauert sonst eventuell zu lang.
                        if (currentPage >= 9)
                        {
                            e.HasMorePages = false;
                            return;
                        }
                    }

                    e.HasMorePages = true;
                    currentPrintRecord = i;
                    currentPage++;
                    return;
                }

                y += Settings.Current.PrintDetailsFont.Height / 2;

                e.Graphics.DrawLine(new Pen(Color.Black, 1), e.MarginBounds.Left, y, e.MarginBounds.Right, y);

                y += Settings.Current.PrintDetailsFont.Height / 2;

                DataRow dataRow = queryTable.Rows[i];

                for (int col = 0; col < detailListFields.Count; col++)
                {
                    float x = e.MarginBounds.Left;

                    string value = dataRow[col] != null ? dataRow[col].ToString() : "";
                    if (!string.IsNullOrEmpty(value) || detailListPrintEmptyFields)
                    {
                        string fieldName = dataBase.GetNameOfField(detailListFields[col]);
                        e.Graphics.DrawString(fieldName + ":", Settings.Current.PrintDetailsFont, detailBrush, new PointF(x, y));
                        x += detailListMaximumFieldWidth;
                        e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, new PointF(x, y));

                        y += Settings.Current.PrintDetailsFont.Height;
                    }
                }
            }

            PrintFooter(e, y);

            e.HasMorePages = false;
        }

        private void PrintDataDetailListWithTracks(PrintPageEventArgs e)
        {
            // Jetzt noch die Trackdaten
            CD cd;

            if (printType == PrintType.CurrentCD)
                cd = this.cd;
            else
                cd = dataBase.GetCDById(CDView.GetCDID(currentPrintRecord));

            // Maximale Länge der Feldnamen ermitteln
            foreach (Field field in detailListFields)
            {
                SizeF size = e.Graphics.MeasureString(dataBase.GetNameOfField(field) + ": ", Settings.Current.PrintDetailsFont);
                if (size.Width > detailListMaximumFieldWidth)
                    detailListMaximumFieldWidth = size.Width;
            }

            float y = e.MarginBounds.Top + headerHeight;
            float yCover = y;
            float xWidthMax = e.MarginBounds.Width;

            if (Settings.Current.PrintCDCover)
            {
                float coverWidth = (float)e.MarginBounds.Width / (float)100.0 * (float)Settings.Current.PrintCDCoverSize;

                float coverPos = e.MarginBounds.Left;
                if (Settings.Current.PrintCDCoverAlign == 1)
                    coverPos = (float)e.MarginBounds.Left + (float)e.MarginBounds.Width / 2.0f - coverWidth / 2.0f;
                if (Settings.Current.PrintCDCoverAlign == 2)
                    coverPos = (float)e.MarginBounds.Left + (float)e.MarginBounds.Width - coverWidth;

                try
                {
                    Image img = Image.FromFile(Misc.FindCover(cd.CDCoverFrontFilename));
                    float coverHeight = coverWidth * (float)img.Width / (float)img.Height;
                    RectangleF rect = new RectangleF(coverPos, y, coverWidth, coverHeight);
                    e.Graphics.DrawImage(img, rect);
                    yCover += coverHeight;

                    if (!Settings.Current.PrintTextUnderCDCover)
                        xWidthMax = coverPos - e.MarginBounds.Left;
                }
                catch
                {   // Ignorieren, wenn Cover nicht geladen werden kann.
                }

                if (Settings.Current.PrintTextUnderCDCover)
                    y = yCover;
            }

            for (int col = 0; col < detailListFields.Count; col++)
            {
                float x = e.MarginBounds.Left;

                string value;

                if (printType == PrintType.CurrentCD)
                    value = this.cd.GetStringByField(dataBase, detailListFields[col]);
                else
                    value = CDView.GetRowStringValue(currentPrintRecord, detailListFields[col]);

                if (!string.IsNullOrEmpty(value) || detailListPrintEmptyFields)
                {
                    string fieldName = dataBase.GetNameOfField(detailListFields[col]);
                    e.Graphics.DrawString(fieldName + ":", Settings.Current.PrintDetailsFont, detailBrush, new PointF(x, y));
                    x += detailListMaximumFieldWidth;
                    RectangleF rect = new RectangleF(x, y, xWidthMax - detailListMaximumFieldWidth, Settings.Current.PrintDetailsFont.Height);
                    StringFormat sf = new StringFormat();
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                    e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);

                    y += Settings.Current.PrintDetailsFont.Height;
                }
            }

            if (yCover > y && !Settings.Current.PrintTextUnderCDCover)
                y = yCover;

            y += Settings.Current.PrintDetailsFont.Height;

            // Header der Trackdaten
            float[] percentageWidthsHeader = trackDetailsListFields.GetPercentageWidths();

            float xpos = e.MarginBounds.Left;

            for (int col = 0; col < trackDetailsListFields.Count; col++)
            {
                string value = dataBase.GetNameOfField(trackDetailsListFields[col].Field);
//                SizeF size = e.Graphics.MeasureString(value, Settings.Current.PrintDetailsFont);

                StringFormat sf = new StringFormat();
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidthsHeader[col]);
                RectangleF rect = new RectangleF(xpos, y, colWidth, Settings.Current.PrintDetailsFont.Height);

                e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);

                xpos += ((float)e.MarginBounds.Width / (float)100.0 * percentageWidthsHeader[col]);
            }

            y += (float)Settings.Current.PrintDetailsFont.Height * 1.1f;

            e.Graphics.DrawLine(new Pen(Color.Black, 1), e.MarginBounds.Left, y, e.MarginBounds.Left + e.MarginBounds.Width, y);
            y += Settings.Current.PrintDetailsFont.Height * 0.2f;

            List<Track> tracks = cd.Tracks;
            for (int i = currentPrintTrackRecord; i < tracks.Count; i++)
            {
                if (y > e.MarginBounds.Bottom)
                {
                    if (printDocument.PrintController.IsPreview)
                    {
                        // Im Preview nur die ersten 10 Seiten anzeigen, dauert sonst eventuell zu lang.
                        if (currentPage >= 9)
                            break;
                    }

                    e.HasMorePages = true;
                    currentPrintTrackRecord = i;
                    currentPage++;
                    return;
                }

                int x = e.MarginBounds.Left;

                float[] percentageWidths = trackDetailsListFields.GetPercentageWidths();

                for (int col = 0; col < trackDetailsListFields.Count; col++)
                {
                    string value = cd.GetTrackStringByField(i, trackDetailsListFields[col].Field);
                    if (value != null)
                    {
//                        SizeF size = e.Graphics.MeasureString(value, Settings.Current.PrintDetailsFont);

                        StringFormat sf = new StringFormat();
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                        int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidthsHeader[col]);
                        RectangleF rect = new RectangleF(x, y, colWidth, Settings.Current.PrintDetailsFont.Height);

                        e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);
                    }

                    x += (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                }

                y += Settings.Current.PrintDetailsFont.Height;
            }

            currentPrintTrackRecord = 0;

            if (printType != PrintType.CurrentCD && currentPrintRecord < CDView.Rows.Count - 1)
            {
                if (printDocument.PrintController.IsPreview)
                {
                    // Im Preview nur die ersten 10 Seiten anzeigen, dauert sonst eventuell zu lang.
                    if (currentPage >= 9)
                    {
                        e.HasMorePages = false;
                        return;
                    }
                }

                e.HasMorePages = true;
                currentPrintRecord++;
                currentPage++;
                return;
            }

            e.HasMorePages = false;
        }

        private void PrintDataTrackList(PrintPageEventArgs e)
        {
            float y = e.MarginBounds.Top + headerHeight;

            for (int i = currentPrintRecord; i < TrackView.Rows.Count; i++)
            {
                if (y > e.MarginBounds.Bottom)
                {
                    if (printDocument.PrintController.IsPreview)
                    {
                        // Im Preview nur die ersten 10 Seiten anzeigen, dauert sonst eventuell zu lang.
                        if (currentPage >= 9)
                        {
                            e.HasMorePages = false;
                            return;
                        }
                    }

                    e.HasMorePages = true;
                    currentPrintRecord = i;
                    currentPage++;
                    return;
                }

                int x = e.MarginBounds.Left;

                float[] percentageWidths = trackListFields.GetPercentageWidths();

                for (int col = 0; col < trackListFields.Count; col++)
                {
                    string value = TrackView.GetRowStringValue(i, trackListFields[col].Field);
                    if (value != null)
                    {
                        //SizeF size = e.Graphics.MeasureString(value, Settings.Current.PrintDetailsFont);
                        StringFormat sf = new StringFormat();
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                        int colWidth = (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                        RectangleF rect = new RectangleF(x, y, colWidth, Settings.Current.PrintDetailsFont.Height);

                        e.Graphics.DrawString(value, Settings.Current.PrintDetailsFont, detailBrush, rect, sf);
                    }

                    x += (int)((float)e.MarginBounds.Width / (float)100.0 * percentageWidths[col]);
                }

                y += Settings.Current.PrintDetailsFont.Height;
            }

            PrintFooter(e, y);

            e.HasMorePages = false;
        }

        private void PrintFooter(PrintPageEventArgs e, float y)
        {
            int x = e.MarginBounds.Left;

            int totalCount;
            int printedCount;
            // Gesamtzahl der Datensätze ermitteln
            if (printType == PrintType.TrackList)
            {
                printedCount = TrackView.Rows.Count;
                totalCount = (int)dataBase.ExecuteScalar("SELECT Count(*) FROM Track");
            }
            else
            {
                printedCount = CDView.Rows.Count;
                totalCount = (int)dataBase.ExecuteScalar("SELECT Count(*) FROM CD");
            }

            y += Settings.Current.PrintDetailsFont.Height;

            e.Graphics.DrawLine(new Pen(Color.Black, 1), x, y, x + e.MarginBounds.Width, y);
            y += 3;
            e.Graphics.DrawLine(new Pen(Color.Black, 1), x, y, x + e.MarginBounds.Width, y);
            y += 5;

            e.Graphics.DrawString(StringTable.EndOfList, Settings.Current.PrintDetailsFont, detailBrush, new PointF(e.MarginBounds.Left, y));

            string totalResult = string.Format(StringTable.TotalPrinted, printedCount, totalCount);

            SizeF size = e.Graphics.MeasureString(totalResult, Settings.Current.PrintDetailsFont);

            e.Graphics.DrawString(totalResult, Settings.Current.PrintDetailsFont, detailBrush, new PointF(x + e.MarginBounds.Width - size.Width, y));
        }

        private void toolStripButtonOnePage_Click(object sender, EventArgs e)
        {
            printPreviewControl.Columns = 1;
            printPreviewControl.Rows = 1;
        }

        private void toolStripButtonTwoPages_Click(object sender, EventArgs e)
        {
            printPreviewControl.Columns = 2;
            printPreviewControl.Rows = 1;
        }

        private void toolStripButtonFourPages_Click(object sender, EventArgs e)
        {
            printPreviewControl.Columns = 2;
            printPreviewControl.Rows = 2;
        }

        private void toolStripComboBoxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxZoom.SelectedItem != null)
            {
                ZoomLevel zoomLevel = toolStripComboBoxZoom.SelectedItem as ZoomLevel;
                if (zoomLevel.ZoomType == ZoomType.WholePage)
                {
                    printPreviewControl.AutoZoom = true;
                    return;
                }
            }

            SetZoom(toolStripComboBoxZoom.Text);
        }

        /// <summary>
        /// Zoom in Prozent
        /// </summary>
        /// <param name="zoomLevel"></param>
        private void SetZoom(string zoomLevel)
        {
            if (zoomLevel.EndsWith("%"))
            {
                string zoomString = zoomLevel.Substring(0, zoomLevel.Length - 1);
                int zoomValue = 0;
                if (Int32.TryParse(zoomString, out zoomValue))
                {
                    printPreviewControl.Zoom = (double)zoomValue / 100.0;
                }
                
            }
        }

        private float[] GetAbsoluteColumnWidths(float totalWidth, float[] percentageWidths)
        {
            float[] absoluteWidths = new float[percentageWidths.Length];
            int i = 0;
            foreach (float width in percentageWidths)
            {
                absoluteWidths[i++] = totalWidth / 100.0f * width;
            }

            return absoluteWidths;
        }

        private void toolStripButtonPageUp_Click(object sender, EventArgs e)
        {
            if (printPreviewControl.StartPage > 0)
                printPreviewControl.StartPage--;

            toolStripLabelPage.Text = string.Format(StringTable.NumberOfPages, printPreviewControl.StartPage + 1, numberOfPages);
        }

        private void toolStripButtonPageDown_Click(object sender, EventArgs e)
        {
            if (printPreviewControl.StartPage < numberOfPages - 1)
                printPreviewControl.StartPage++;

            toolStripLabelPage.Text = string.Format(StringTable.NumberOfPages, printPreviewControl.StartPage + 1, numberOfPages);
        }

        private void buttonSelectFields_Click(object sender, EventArgs e)
        {
            switch (printType)
            {
                case PrintType.CDList:
                    {
                        FormChooseColumnFields formChooseFields = new FormChooseColumnFields(dataBase, FieldType.CD, cdListFields);
                        formChooseFields.Description = StringTable.ChooseFieldsForPrinting;
                        if (formChooseFields.ShowDialog(this) == DialogResult.OK)
                        {
                            cdListFields = formChooseFields.SelectedFields;
                            printPreviewControl.InvalidatePreview();
                        }
                        break;
                    }
                case PrintType.DetailList:
                case PrintType.DetailListWithTracks:
                    {
                        FormChooseFields formChooseFields = new FormChooseFields(dataBase, FieldType.CD, detailListFields);
                        if (formChooseFields.ShowDialog(this) == DialogResult.OK)
                        {
                            detailListFields = formChooseFields.SelectedFields;
                            printPreviewControl.InvalidatePreview();
                        }
                        break;
                    }
                case PrintType.TrackList:
                    {
                        FormChooseColumnFields formChooseFields = new FormChooseColumnFields(dataBase, FieldType.TrackAndCD, trackListFields);
                        formChooseFields.Description = StringTable.ChooseFieldsForPrinting;
                        if (formChooseFields.ShowDialog(this) == DialogResult.OK)
                        {
                            trackListFields = formChooseFields.SelectedFields;
                            printPreviewControl.InvalidatePreview();
                        }
                        break;
                    }
            }
        }

        private void radioButtonCDList_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCDList.Checked)
            {
                printType = PrintType.CDList;
                printPreviewControl.InvalidatePreview();
                UpdateWindowState();
            }
        }

        private void radioButtonTrackList_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTrackList.Checked)
            {
                printType = PrintType.TrackList;
                printPreviewControl.InvalidatePreview();
                UpdateWindowState();
            }
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            FormSort formSort;
            
            if (printType != PrintType.TrackList)
                formSort = new FormSort(dataBase, FieldType.CD, cdSortFields);
            else
                formSort = new FormSort(dataBase, FieldType.Track, trackSortFields);
                
            if (formSort.ShowDialog(this) == DialogResult.OK)
            {
                if (printType != PrintType.TrackList)
                    cdSortFields = formSort.SortFields;
                else
                    trackSortFields = formSort.SortFields;

                printPreviewControl.InvalidatePreview();
            }
        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = Settings.Current.PrintDetailsFont;
            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Current.PrintDetailsFont = fontDialog.Font;
                printPreviewControl.InvalidatePreview();
            }
        }

        private void radioButtonDetailList_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDetailList.Checked)
            {
                printType = PrintType.DetailList;
                printPreviewControl.InvalidatePreview();
                UpdateWindowState();
            }
        }

        private void radioButtonDetailListWithTracks_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDetailListWithTracks.Checked)
            {
                printType = PrintType.DetailListWithTracks;
                printPreviewControl.InvalidatePreview();
                UpdateWindowState();
            }
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            FormSearch formSearch;

            switch (printType)
            {
                case PrintType.TrackList:
                    formSearch = new FormSearch(dataBase, printFilter, true, false);
                    break;
                default:
                    formSearch = new FormSearch(dataBase, printFilter, false, true);
                    break;
            }

            if (formSearch.ShowDialog(this) == DialogResult.OK)
            {
                printFilter = formSearch.GetCondition();
                printPreviewControl.InvalidatePreview();
            }
            /*FormExtendedSearch formExtendedSearch = new FormExtendedSearch(dataBase, printFilter);

            if (formExtendedSearch.ShowDialog(this) == DialogResult.OK)
            {
                printFilter = formExtendedSearch.Condition;
                printPreviewControl.InvalidatePreview();
            }*/
        }

        private void checkBoxHeaderPrintSort_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Current.PrintHeaderSort = checkBoxHeaderPrintSort.Checked;
            printPreviewControl.InvalidatePreview();
        }

        private void checkBoxHeaderPrintFilter_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Current.PrintHeaderFilter = checkBoxHeaderPrintFilter.Checked;
            printPreviewControl.InvalidatePreview();
        }

        private void checkBoxHeaderPrintDate_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Current.PrintHeaderDate = checkBoxHeaderPrintDate.Checked;
            printPreviewControl.InvalidatePreview();
        }

        private void toolStripButtonPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = printDocument.PrinterSettings;

            if (pd.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Current.GlobalPrinterSettings = pd.PrinterSettings;
                printDocument.PrinterSettings = pd.PrinterSettings;
                printDocument.Print();
            }
        }

        private void textBoxHeaderText_Leave(object sender, EventArgs e)
        {
            printPreviewControl.InvalidatePreview();
        }

        private void FormPrintCatalog_Load(object sender, EventArgs e)
        {
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
            printDocument.BeginPrint += new PrintEventHandler(printDocument_BeginPrint);
            printDocument.EndPrint += new PrintEventHandler(printDocument_EndPrint);
            printPreviewControl.Document = printDocument;
            printDocument.PrinterSettings = Settings.Current.GlobalPrinterSettings;

            radioButtonCurrentCD.Enabled = this.cd != null;
        }

        private void radioButtonCurrentCD_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCurrentCD.Checked)
            {
                printType = PrintType.CurrentCD;
                printPreviewControl.InvalidatePreview();
                UpdateWindowState();
            }
        }

        private void UpdateWindowState()
        {
            buttonCDCover.Enabled = printType == PrintType.CurrentCD || printType == PrintType.DetailListWithTracks;
        }

        private void FormPrintCatalog_FormClosing(object sender, FormClosingEventArgs e)
        {
            cdListFields.SaveToRegistry("PrintCDListFields");
            trackListFields.SaveToRegistry("PrintTrackListFields");
            trackDetailsListFields.SaveToRegistry("PrintTrackDetailsListFields");
        }

        private void buttonCDCover_Click(object sender, EventArgs e)
        {
            FormPrintOptionsLongList formPrintOptionsLongList = new FormPrintOptionsLongList();

            if (formPrintOptionsLongList.ShowDialog(this) == DialogResult.OK)
            {
                printPreviewControl.InvalidatePreview();
            }
        }

        private void toolStripButtonPrintSetup_Click(object sender, EventArgs e)
        {
            PageSetupDialog pd = new PageSetupDialog();
            pd.Document = printDocument;

            pd.PrinterSettings = printDocument.PrinterSettings;
            pd.AllowMargins = true;
            pd.AllowPrinter = true;
            pd.EnableMetric = true;
            
            if (pd.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Current.GlobalPrinterSettings = pd.PrinterSettings;
                //printDocument.PrinterSettings = pd.PrinterSettings;
                //printDocument = pd.Document;
                printPreviewControl.InvalidatePreview();
            }
        }
    }

    public enum ZoomType
    {
        Percentage,
        WholePage
    }

    public class ZoomLevel
    {
        public int Percent { get; set; }
        
        public ZoomType ZoomType { get; set; }

        public ZoomLevel(int percent)
        {
            Percent = percent;
            ZoomType = ZoomType.Percentage;
        }

        public ZoomLevel(ZoomType zoomType)
        {
            ZoomType = zoomType;
        }

        public override string ToString()
        {
            switch (ZoomType)
            {
                case ZoomType.Percentage:
                    return Percent.ToString() + "%";
                case ZoomType.WholePage:
                    return StringTable.WholePage;
                default:
                    return base.ToString();
            }
        }
    }
}
