using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormChooseFields : Form
    {
        public FormChooseFields(DataBase db, FieldType ft, FieldCollection fields)
        {
            InitializeComponent();
            
            chooseFieldControl.Init(db, ft, fields);
        }

        public FieldCollection SelectedFields
        {
            get
            {
                return chooseFieldControl.SelectedFields;
            }
        }
    }
}