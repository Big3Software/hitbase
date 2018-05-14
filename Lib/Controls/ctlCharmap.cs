using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;




namespace Big3.Hitbase.Controls
{
    public partial class ctlCharmap : UserControl
    {
        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(
                IntPtr hdc,          // handle to DC 
                IntPtr hgdiobj   // handle to object 
                );
        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetGlyphIndices(
                IntPtr hdc,       // handle to DC 
                [MarshalAs(UnmanagedType.LPWStr)] 
                        string lpstr, // string to convert 
                int c,         // number of characters in string 
                Int16[] pgi,    // array of glyph indices 
                int fl       // glyph options 
                );

        //**** Öffentliche Eigenschaften ****
        private Char charSelected;                  //**** Öffentliche Eigenschaft "Char" ****

        //**** Interne Variablen ****
        private int m_Rows = 10;
        private int m_Cols = 10;

        private int m_CellWidth = 30;
        private int m_CellHeight = 20;
        
        private int m_MarginLeft = 2;
        private int m_MarginTop = 2;
        private int m_MarginRight = 2;
        private int m_MarginBottom = 2;

        private int m_ScrollbarWidth = 20;         //**** TODO!!! Hier noch die Breite der Scrollbar korrekt berechnen ****

        private const UInt16 cst_StartPos = 33;    //**** Welches ASCII Zeichen ist das Erste ****

        List<Char> lkl_CharValidArray;


        public ctlCharmap()
        {
            InitializeComponent();

            lkl_CharValidArray = new List<Char>();
        }

        private void ctlCharmap_Load(object sender, EventArgs e)
        {
            Graphics lkv_Graphics = this.CreateGraphics();
            GlyphArrayFill(lkv_Graphics);

            //**** Datagrid initialisieren ****
            DataGridInitialize();

            //**** Tabelle berechnen und Spalten erstellen ****
            DataGridColumnsCreate();

            //**** Tabelle mit Inhalt füllen ****
            DataGridFill();
        }

        private void DataGridInitialize()
        {
            // 
            // ctlDataGrid
            // 
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.ctlDataGrid.AllowUserToAddRows = false;
            this.ctlDataGrid.AllowUserToDeleteRows = false;
            this.ctlDataGrid.AllowUserToResizeColumns = false;
            this.ctlDataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.ctlDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.ctlDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ctlDataGrid.ColumnHeadersVisible = false;
            this.ctlDataGrid.DefaultCellStyle = dataGridViewCellStyle;
            this.ctlDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlDataGrid.Location = new System.Drawing.Point(0, 0);
            this.ctlDataGrid.Name = "ctlDataGrid";
            this.ctlDataGrid.MultiSelect = false;
            this.ctlDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.ctlDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.ctlDataGrid.RowHeadersVisible = false;
            this.ctlDataGrid.Size = new System.Drawing.Size(309, 295);
            this.ctlDataGrid.TabIndex = 1;
            this.ctlDataGrid.Resize += new System.EventHandler(this.ctlDataGrid_Resize);
        }

        private void DataGridColumnsCreate()
        {
            int lkv_Index;

            m_Cols = (int)((this.Right - m_MarginRight) - (this.Left + m_MarginLeft + m_ScrollbarWidth)) / m_CellWidth;
            m_Rows = (int)((this.Bottom - m_MarginBottom) - (this.Top + m_MarginTop)) / m_CellHeight;

            //**** Spalten zuerst löschen und anschließend anlegen ****
            this.ctlDataGrid.Columns.Clear();

            this.Columns = new System.Windows.Forms.DataGridViewTextBoxColumn[m_Cols];
            for (lkv_Index = 0; lkv_Index < m_Cols; lkv_Index++)
            {
                this.Columns[lkv_Index] = new System.Windows.Forms.DataGridViewTextBoxColumn();
                this.ctlDataGrid.Columns.Add(this.Columns[lkv_Index]);

                this.Columns[lkv_Index].HeaderText = "";
                this.Columns[lkv_Index].Name = "Col" + (lkv_Index + 1);
                this.Columns[lkv_Index].Width = 30;
            }
        }

        private void DataGridFill()
        {
            int lkv_Index;
            int lkv_IndexSub;
            int lkv_RowNew;
            int lkv_Help;

            if ((lkl_CharValidArray.Count % m_Cols) != 0)
                lkv_Help = (lkl_CharValidArray.Count / m_Cols) + 1;
            else
                lkv_Help = lkl_CharValidArray.Count / m_Cols;

            for (lkv_Index = 0; lkv_Index < lkv_Help; lkv_Index++)
            {
                lkv_RowNew = ctlDataGrid.Rows.Add();
                for (lkv_IndexSub = 0; lkv_IndexSub < m_Cols; lkv_IndexSub++)
                {
                    if ((lkv_Index * m_Cols) + (lkv_IndexSub + 1) > lkl_CharValidArray.Count)
                    {
                        ctlDataGrid.Rows[lkv_RowNew].Cells[lkv_IndexSub].ReadOnly = true;
                    }
                    else
                    {
                        ctlDataGrid.Rows[lkv_RowNew].Cells[lkv_IndexSub].Value = lkl_CharValidArray[(lkv_Index * m_Cols) + lkv_IndexSub];
                    }

                }

            }

            //**** Das erste Feld ist aktiviert => Inhalt des Felder ist Rückgabewert ****
            CharSelected = this.ctlDataGrid.Rows[0].Cells[0].Value.ToString()[0];

            return;
        }

        //**** Füllt ein Array mit allen gültigen Zeichen einer Schriftart ****
        private void GlyphArrayFill(Graphics par_Graphics)
        {
            UInt16 lkv_Index;
            Char lkv_Char;
            FontFamily fontFamily;
            Font font;
            System.IntPtr lkv_Hdc;
            int count;
            Int16[] rtcode;
            System.IntPtr lkv_FontOld;

            lkv_Hdc = par_Graphics.GetHdc();
            fontFamily = new FontFamily("Arial");
            font = new Font(fontFamily,
                            14,
                            FontStyle.Regular,
                            GraphicsUnit.Point);
            lkv_FontOld = SelectObject(lkv_Hdc, font.ToHfont());

            lkl_CharValidArray.Clear();

            count = 1;
            rtcode = new Int16[count];
            lkv_Index = cst_StartPos;
            while (true)
            {
                lkv_Char = ((Char)(lkv_Index));

                GetGlyphIndices(lkv_Hdc, lkv_Char.ToString(), count, rtcode, 0xffff);
                if (rtcode[0] != 0)
                {
                    //**** Zeichen gefunden ****
                    lkl_CharValidArray.Add(lkv_Char);
                }

                //**** Abfrage muss hier gemacht werden, da ansonsten ein Überlauf eintritt ****
                if (lkv_Index == 65535)
                {
                    break;
                }

                lkv_Index++;
            }

            //**** Variablen aufräumen ****
            SelectObject(lkv_Hdc, lkv_FontOld);
            font.Dispose();
            fontFamily.Dispose();

            par_Graphics.ReleaseHdc(lkv_Hdc);
        }

        private void lblSingleChar_Click(object sender, EventArgs e)
        {
            Label lkv_Label = sender as Label;

            CharSelected = lkv_Label.Text[0];
        }

        public Char CharSelected
        {
            get { return charSelected; }
            set { charSelected = value; }
        }

        private void ctlDataGrid_Resize(object sender, EventArgs e)
        {
            if (((this.Right - m_MarginRight) - (this.Left + m_MarginLeft + m_ScrollbarWidth)) / m_CellWidth != m_Cols)
            {
                DataGridColumnsCreate();
                DataGridFill();
            }
        }

        private void ctlDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView lkv_DataGrid = sender as DataGridView;

            if (lkv_DataGrid.CurrentCell.Value != null)
                CharSelected = lkv_DataGrid.CurrentCell.Value.ToString()[0];
        }


    }
}