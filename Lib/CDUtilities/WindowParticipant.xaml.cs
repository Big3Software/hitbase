using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowParticipant.xaml
    /// </summary>
    public partial class WindowParticipant : Window
    {
        Participant participant;
        DataBase dataBase;

        public WindowParticipant()
        {
            InitializeComponent();

            this.ComboBoxName.Loaded += delegate
            {
                TextBox editTextBox = ComboBoxName.Template.FindName("PART_EditableTextBox", ComboBoxName) as TextBox;

                if (editTextBox != null)
                {
                    editTextBox.TextChanged += delegate { UpdateWindowState(); };
                }
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.HideMinimizeAndMaximizeButtons();
        }

        public WindowParticipant(DataBase db, Participant p, bool editParticipant) : this()
        {
            dataBase = db;
            participant = p;

            if (editParticipant)
                this.Title = StringTable.EditParticipant;

            FillRolesInComboBox();

            FillArtistsInComboBox();

            if (participant.TrackNumber > 0)
                TextBoxTrack.Text = Convert.ToString(participant.TrackNumber);
            TextBoxComment.Text = participant.Comment;

            UpdateWindowState();

            Settings.RestoreWindowPlacement(this, "WindowParticipant");
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            participant.Role = ComboBoxRole.Text;
            participant.Name = ComboBoxName.Text;

            participant.TrackNumber = Misc.Atoi(TextBoxTrack.Text);
            participant.Comment = TextBoxComment.Text;

            this.DialogResult = true; 
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; 
            this.Close();
        }

        private void FillArtistsInComboBox()
        {
            PersonGroupTableAdapter personGroupAdapter = new PersonGroupTableAdapter(dataBase);
            PersonGroupDataSet.PersonGroupDataTable personGroupDataTable = personGroupAdapter.GetData();

            foreach (PersonGroupDataSet.PersonGroupRow row in personGroupDataTable)
            {
                ComboBoxName.Items.Add(row.Name);
            }
                //ComboBoxName.ItemsSource = personGroupDataTable;
            //ComboBoxName.DisplayMemberPath = "Name";
            //ComboBoxName.SelectedValuePath = "Name";

            if (participant.Name != null)
                ComboBoxName.Text = participant.Name;
            else
                ComboBoxName.Text = "";

            UpdateWindowState();
        }

        private void FillRolesInComboBox()
        {
            ComboBoxRole.ItemsSource = dataBase.GetAllRoles().OrderBy(x => x.Name);
            ComboBoxRole.DisplayMemberPath = "Name";
            ComboBoxRole.SelectedValuePath = "Name";

            if (participant.Role != null)
                ComboBoxRole.SelectedValue = participant.Role;

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            ButtonOK.IsEnabled = (ComboBoxRole.SelectedIndex >= 0 && !string.IsNullOrEmpty(ComboBoxName.Text));
        }

        private void CommandBindingEditRoles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormEditRoles formEditRoles = new FormEditRoles(dataBase);

            formEditRoles.ShowDialog(new NativeWindowWrapper(this));

            ComboBoxRole.ItemsSource = dataBase.GetAllRoles();
        }

        private void CommandBindingEditPersonGroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PersonGroup personGroup = dataBase.GetPersonGroupByName(ComboBoxName.Text, false);

            if (personGroup != null)
            {
                PersonGroupWindow pgw = new PersonGroupWindow(dataBase, PersonType.Unknown, personGroup);
                pgw.ShowDialog();
            }
        }

        private void ComboBoxRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Settings.SaveWindowPlacement(this, "WindowParticipant");
        }
    }
}
