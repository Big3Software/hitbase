using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using XPTable.Models;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSql : Form
    {
        private DataBase dataBase;

        public FormSql(DataBase db)
        {
            InitializeComponent();

            dataBase = db;

            Tools.FormatXPTable(tableResult);

/*m_sqlAbfrageCtrl.AddString(L"select * from cd");
	m_sqlAbfrageCtrl.AddString(L"select * from lied where szTitel like '*the*'");
	m_sqlAbfrageCtrl.AddString(L"select * from artist order by szArtistName");
	m_sqlAbfrageCtrl.AddString(L"select szArtistname as Artist, szTitel as Titel from cd, Artist where cd.idartist = artist.idartist");
	m_sqlAbfrageCtrl.AddString(L"select * from cd inner join cdset on cd.idcdset = cdset.idcdset");
*/
        }

        private void buttonExecuteSql_Click(object sender, EventArgs e)
        {
            DataTable dt;

            if (!IsSelectStatement(textBoxSql.Text))
            {
                MessageBox.Show(StringTable.OnlySelectStatements, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                dt = dataBase.ExecuteFreeSql(textBoxSql.Text);
            }
            catch (Exception ex)
            {
                string msg = string.Format(StringTable.ErrorFreeSql, ex.Message);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            tableResult.TableModel = new TableModel();
            tableResult.ColumnModel = new ColumnModel();

            foreach (DataColumn dc in dt.Columns)
            {
                TextColumn newColumn = new XPTable.Models.TextColumn(dc.Caption);
                newColumn.Editable = false;
                tableResult.ColumnModel.Columns.Add(newColumn);
            }

            foreach (DataRow dr in dt.Rows)
            {
                Row row = new Row();
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    object value = dr.ItemArray[i];

                    if (value != null)
                        row.Cells.Add(new Cell(value.ToString()));
                    else
                        row.Cells.Add(new Cell(""));
                }
                tableResult.TableModel.Rows.Add(row);
            }
        }

        private bool IsSelectStatement(string sql)
        {
            sql = sql.TrimStart().ToUpper();

            if (sql.StartsWith("SELECT"))
                return true;
            else
                return false;
        }
    }
}
