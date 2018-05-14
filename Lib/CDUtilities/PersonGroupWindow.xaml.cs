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
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;
using Big3.Hitbase.SharedResources;
using System.ComponentModel;
using System.Data;
using System.IO;
using Big3.Hitbase.SoundEngine;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for PersonGroupWindow.xaml
    /// </summary>
    public partial class PersonGroupWindow : Window
    {
        public PersonGroupWindow()
        {
            InitializeComponent();

            KeyDown += new KeyEventHandler(PersonGroupWindow_KeyDown);

            Settings.RestoreWindowPlacement(this, "WindowPersonGroup");
        }

        void PersonGroupWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                DialogResult = false;
                Close();
            }
        }

        public bool ChangeAllSoundFiles { get; set; }

        private DataBase dataBase;
        private PersonGroup personGroup;
        private int oldPersonGroupId;
        private string oldPersonGroup;

        public PersonGroupWindow(DataBase db, PersonType personType, PersonGroup personGroup) : this()
        {
            dataBase = db;
            this.personGroup = personGroup;
            this.oldPersonGroup = personGroup.Name;
            this.oldPersonGroupId = personGroup.ID;

            personGroupProperties.DataBase = db;
            personGroupProperties.PersonType = personType;
            personGroupProperties.PersonGroup = personGroup;
        }

        private void personGroupProperties_OKClicked(object sender, EventArgs e)
        {
            DialogResult = true;

            if (ChangeAllSoundFiles && oldPersonGroup != personGroup.Name)
            {
                PersonGroupHelper.UpdateSoundfiles(this.dataBase, oldPersonGroupId, oldPersonGroup, personGroup.Name, null);
            }

            Close();
        }

        private void personGroupProperties_CancelClicked(object sender, EventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.RestoreWindowPlacement(this, "WindowPersonGroup");
        }


    }
}
