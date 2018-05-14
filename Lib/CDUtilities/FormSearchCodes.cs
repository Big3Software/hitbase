using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSearchCodes : Form
    {
        public class CodeSearchItem
        {
            public ComboBox ComboNot { get; set; }
            public ComboBox ComboCode { get; set; }
            public ComboBox ComboAndOr { get; set; }
        }

        private List<CodeSearchItem> codeSearchItems = new List<CodeSearchItem>();
        public List<CodeSearchItem> CodeSearchItems
        {
            get
            {
                return codeSearchItems;
            }
        }
        private DataBase dataBase;
        private Condition condition;

        public FormSearchCodes(DataBase db, Condition condition, bool trackCodes)
        {
            dataBase = db;
            this.condition = condition;

            InitializeComponent();

            codeSearchItems.Add(new CodeSearchItem() { ComboNot = comboBoxNot1, ComboCode = comboBoxCode1, ComboAndOr = comboBoxAndOr1 });
            codeSearchItems.Add(new CodeSearchItem() { ComboNot = comboBoxNot2, ComboCode = comboBoxCode2, ComboAndOr = comboBoxAndOr2 });
            codeSearchItems.Add(new CodeSearchItem() { ComboNot = comboBoxNot3, ComboCode = comboBoxCode3, ComboAndOr = comboBoxAndOr3 });
            codeSearchItems.Add(new CodeSearchItem() { ComboNot = comboBoxNot4, ComboCode = comboBoxCode4, ComboAndOr = comboBoxAndOr4 });
            codeSearchItems.Add(new CodeSearchItem() { ComboNot = comboBoxNot5, ComboCode = comboBoxCode5, ComboAndOr = comboBoxAndOr5 });

            foreach (CodeSearchItem item in codeSearchItems)
            {
                item.ComboNot.Items.Add("");
                item.ComboNot.Items.Add(StringTable.Not);

                FillCodeInComboBox(item.ComboCode);

                item.ComboAndOr.Items.Add(StringTable.And);
                item.ComboAndOr.Items.Add(StringTable.Or);
            }

            if (condition != null)
            {
                int count = 0;
                foreach (SingleCondition cond in condition)
                {
                    if (cond.Field == Field.Codes && !trackCodes ||
                        cond.Field == Field.TrackCodes && trackCodes)
                    {
                        char code = cond.Value.ToString()[0];
                        codeSearchItems[count].ComboCode.SelectedIndex = (int)code - 65 + 1;

                        if (cond.Operator == Operator.NotContains)
                            codeSearchItems[count].ComboNot.SelectedIndex = 1;

                        if (count > 0)
                        {
                            if (cond.Logical == Logical.Or)
                                codeSearchItems[count-1].ComboAndOr.SelectedIndex = 1;
                            else
                                codeSearchItems[count-1].ComboAndOr.SelectedIndex = 0;
                        }

                        count++;
                    }
                }
            }
        }

        private void FillCodeInComboBox(ComboBox comboBox)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Add("");
            for (int i = 0; i < 26; i++)
            {
                string str = string.Format("{0}: {1}", (char)(i + 65), dataBase.Codes[i]);

                comboBox.Items.Add(str);
            }
            comboBox.EndUpdate();
        }


        private void FormSearchCodes_Load(object sender, EventArgs e)
        {
        }
    }
}
