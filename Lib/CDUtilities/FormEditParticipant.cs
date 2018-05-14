using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormEditParticipant : Form
    {
        private class ComboBoxRoleItem
        {
            public RoleDataSet.RoleRow Role;

            public override string ToString()
            {
                return Role.Name;
            }
        }

        Participant participant;
        DataBase dataBase;

        public FormEditParticipant()
        {
            InitializeComponent();
        }

        public FormEditParticipant(DataBase db, Participant p, bool editParticipant)
            : this()
        {
            dataBase = db;
            participant = p;

            if (editParticipant)
                this.Text = StringTable.EditParticipant;
        }

        private void FormEditParticipant_Load(object sender, EventArgs e)
        {
            FillRolesInComboBox();

            FillArtistsInComboBox();

            if (participant.TrackNumber > 0)
                textBoxTrack.Text = Convert.ToString(participant.TrackNumber);
            textBoxComment.Text = participant.Comment;
        }

        private void FillArtistsInComboBox()
        {
            Cursor.Current = Cursors.WaitCursor;
            PersonGroupTableAdapter personGroupAdapter = new PersonGroupTableAdapter(dataBase);
            PersonGroupDataSet.PersonGroupDataTable personGroupDataTable = personGroupAdapter.GetData();

            comboBoxName.DataSource = personGroupDataTable;
            comboBoxName.DisplayMember = "Name";
            comboBoxName.ValueMember = "Name";

            Cursor.Current = Cursors.Default;

            if (participant.Name != null)
                comboBoxName.Text = participant.Name;
            else
                comboBoxName.Text = "";

            UpdateWindowState();
        }

        private void FillRolesInComboBox()
        {
            Cursor.Current = Cursors.WaitCursor;
            RoleTableAdapter roleAdapter = new RoleTableAdapter(dataBase);
            RoleDataSet roleDataset = new RoleDataSet();

            roleAdapter.Fill(roleDataset.Role);
            roleDataset.Role.DefaultView.Sort = "Name ASC";
            comboBoxRole.DataSource = roleDataset.Role.DefaultView;
            comboBoxRole.DisplayMember = "Name";
            comboBoxRole.ValueMember = "Name";

            if (participant.Role != null)
                comboBoxRole.SelectedValue = participant.Role;

            Cursor.Current = Cursors.Default;

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = (comboBoxRole.SelectedIndex >= 0);
        }

        private void buttonEditRoles_Click(object sender, EventArgs e)
        {
            FormEditRoles formEditRoles = new FormEditRoles(dataBase);

            formEditRoles.ShowDialog();

            FillRolesInComboBox();
        }

        private void buttonEditArtists_Click(object sender, EventArgs e)
        {
            FormEditArtists formArtists = new FormEditArtists(dataBase);

            formArtists.ShowDialog(this);
        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            participant.Role = comboBoxRole.Text;
            participant.Name = comboBoxName.Text;

            participant.TrackNumber = Misc.Atoi(textBoxTrack.Text);
            participant.Comment = textBoxComment.Text;
        }
    }
}