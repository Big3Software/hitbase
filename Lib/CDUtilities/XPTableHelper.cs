using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using Big3.Hitbase.Configuration;
using System.Windows.Forms;

namespace Big3.Hitbase.CDUtilities
{
    public class XPTableHelper
    {
        public class XPTableConfiguration 
        {
            public List<XPTableColumn> TableColumns = new List<XPTableColumn>();
            public int SortColumn { get; set; }
            public int SortOrder { get; set; }
        }

        public class XPTableColumn
        {
            public int Width;
            public bool Visible;
        }

        public static void SaveTableConfiguration(Table xpTable, string keyName)
        {
            try
            {
                XPTableConfiguration tableConfig = new XPTableConfiguration();

                foreach (Column col in xpTable.ColumnModel.Columns)
                {
                    XPTableColumn column = new XPTableColumn();
                    column.Width = col.Width;
                    column.Visible = col.Visible;
                    tableConfig.TableColumns.Add(column);
                }

                if (xpTable.SortingColumn >= 0)
                {
                    tableConfig.SortColumn = xpTable.SortingColumn;
                    tableConfig.SortOrder = (int)xpTable.ColumnModel.Columns[tableConfig.SortColumn].SortOrder;
                }

                XmlSerializer bf = new XmlSerializer(typeof(XPTableConfiguration));
                StringWriter stream = new StringWriter();
                bf.Serialize(stream, tableConfig);
                stream.Close();

                string xml = stream.ToString();

                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Settings.HitbaseRegistryKey))
                {
                    regKey.SetValue(keyName, xml);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static XPTableConfiguration LoadTableConfiguration(Table xpTable, string keyName)
        {
            try
            {
                string xml = "";
                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Settings.HitbaseRegistryKey))
                {
                    xml = (string)regKey.GetValue(keyName, "");
                }

                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                XmlSerializer bf = new XmlSerializer(typeof(XPTableConfiguration));
                StringReader xmlString = new StringReader(xml);
                XPTableConfiguration tableConfig = (XPTableConfiguration)bf.Deserialize(xmlString);

                int index = 0;
                foreach (XPTableColumn col in tableConfig.TableColumns)
                {
                    xpTable.ColumnModel.Columns[index].Width = col.Width;
                    xpTable.ColumnModel.Columns[index].Visible = col.Visible;

                    index++;
                }

                return tableConfig;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
    }
}
