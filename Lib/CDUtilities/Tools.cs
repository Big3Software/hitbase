using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Big3.Hitbase.CDUtilities
{
    public class Tools
    {
        public static void FormatXPTable(XPTable.Models.Table table)
        {
            table.FullRowSelect = true;
            table.SelectionStyle = XPTable.Models.SelectionStyle.ListView;
            table.AlternatingRowColor = Color.WhiteSmoke;
            table.GridLines = XPTable.Models.GridLines.Both;
            table.GridLineStyle = XPTable.Models.GridLineStyle.Solid;
            table.NoItemsText = "";
        }
    }
}
