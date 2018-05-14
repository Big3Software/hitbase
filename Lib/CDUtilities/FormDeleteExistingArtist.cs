using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormDeleteExistingArtist : Form
    {
        private DataBase dataBase = null;
        private PersonGroupDataSet.PersonGroupRow personGroupToDelete = null;

        public FormDeleteExistingArtist(DataBase db, PersonGroupDataSet.PersonGroupRow personGroup)
        {
            InitializeComponent();

            dataBase = db;
            personGroupToDelete = personGroup;

            FormThemeManager.SetTheme(this);

            buttonOK.Enabled = false;
        }

        private void FormDeleteExistingArtist_Load(object sender, EventArgs e)
        {
            PersonGroupTableAdapter ta = new PersonGroupTableAdapter(dataBase);
            PersonGroupDataSet.PersonGroupDataTable personGroups = new PersonGroupDataSet.PersonGroupDataTable();
            ta.Fill(personGroups);

            foreach (PersonGroupDataSet.PersonGroupRow personGroup in personGroups)
            {
                if (personGroup.Name != personGroupToDelete.Name)
                    comboBoxArtists.Items.Add(personGroup.Name);
            }

            label1.Text = string.Format(label1.Text, personGroupToDelete.Name);
        }

        private void comboBoxArtists_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = true;
        }
    }
}