using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormChooseColumnFields : Form
    {
        public FormChooseColumnFields(DataBase db, FieldType ft, ColumnFieldCollection columnFields, ColumnFieldCollection defaultColumnFields = null)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            chooseColumnFieldControl.Init(db, ft, columnFields, defaultColumnFields);
        }

        private void FormChooseColumnFields_Load(object sender, EventArgs e)
        {

        }

        public string Description
        {
            set
            {
                labelDescription.Text = value;
            }
        }

        public ColumnFieldCollection SelectedFields
        {
            get
            {
                return chooseColumnFieldControl.SelectedFields;
            }
        }
    }
}