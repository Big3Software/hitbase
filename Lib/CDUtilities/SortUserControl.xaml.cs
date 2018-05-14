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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for SortUserControl.xaml
    /// </summary>
    public partial class SortUserControl : UserControl, IModalUserControl
    {
        private DataBase dataBase;

        public SortUserControl(DataBase dataBase, bool cdOnly, SortFieldCollection sfc)
        {
            InitializeComponent();

            this.dataBase = dataBase;

            comboBoxSort1.ItemsSource = GetFieldList(cdOnly);
            comboBoxSort1.DisplayMemberPath = "DisplayName";
            comboBoxSort1.SelectedValuePath = "Field";

            if (sfc.Count > 0)
            {
                comboBoxSort1.SelectedValue = sfc[0].Field;
                radioButtonSort1Ascending.IsChecked = (sfc[0].SortDirection == SortDirection.Ascending);
                radioButtonSort1Descending.IsChecked = (sfc[0].SortDirection == SortDirection.Descending);
            }
            else
            {
                comboBoxSort1.SelectedIndex = 0;
            }

            comboBoxSort2.ItemsSource = GetFieldList(cdOnly);
            comboBoxSort2.DisplayMemberPath = "DisplayName";
            comboBoxSort2.SelectedValuePath = "Field";

            if (sfc.Count > 1)
            {
                comboBoxSort2.SelectedValue = sfc[1].Field;
                radioButtonSort2Ascending.IsChecked = (sfc[1].SortDirection == SortDirection.Ascending);
                radioButtonSort2Descending.IsChecked = (sfc[1].SortDirection == SortDirection.Descending);
            }
            else
            {
                comboBoxSort2.SelectedIndex = 0;
            }

            comboBoxSort3.ItemsSource = GetFieldList(cdOnly);
            comboBoxSort3.DisplayMemberPath = "DisplayName";
            comboBoxSort3.SelectedValuePath = "Field";

            if (sfc.Count > 2)
            {
                comboBoxSort3.SelectedValue = sfc[2].Field;
                radioButtonSort3Ascending.IsChecked = (sfc[2].SortDirection == SortDirection.Ascending);
                radioButtonSort3Descending.IsChecked = (sfc[2].SortDirection == SortDirection.Descending);
            }
            else
            {
                comboBoxSort3.SelectedIndex = 0;
            }
        }

        private List<ComboBoxFieldItem> GetFieldList(bool cdOnly)
        {
            List<ComboBoxFieldItem> fields = new List<ComboBoxFieldItem>();

            fields.Add(new ComboBoxFieldItem(null, Field.None, cdOnly));

            FieldCollection fc = null;
            if (cdOnly)
            {
                fc = FieldHelper.GetAllCDFields(false);
            }
            else
            {
                fc = FieldHelper.GetAllFields();
            }

            foreach (Field field in fc)
            {
                fields.Add(new ComboBoxFieldItem(this.dataBase, field, cdOnly));
            }

            return fields;
        }

        public SortFieldCollection SortFields
        {
            get;
            set;
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            SortFields = new SortFieldCollection();

            AddSortField(comboBoxSort1, radioButtonSort1Ascending, radioButtonSort1Descending);
            AddSortField(comboBoxSort2, radioButtonSort2Ascending, radioButtonSort2Descending);
            AddSortField(comboBoxSort3, radioButtonSort3Ascending, radioButtonSort3Descending);

            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        private void AddSortField(ComboBox comboBoxSort, RadioButton radioButtonSortAscending, RadioButton radioButtonSortDescending)
        {
            ComboBoxFieldItem selItem = comboBoxSort.SelectedItem as ComboBoxFieldItem;
            if (selItem != null && selItem.Field != Field.None)
            {
                SortFields.Add(new SortField(selItem.Field, radioButtonSortAscending.IsChecked == true ? SortDirection.Ascending : SortDirection.Descending));
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }
    }

    public class ComboBoxFieldItem
    {
        private DataBase dataBase;
        private Field field;
        private bool cdOnly;

        public ComboBoxFieldItem(DataBase dataBase, Field field, bool cdOnly)
        {
            this.dataBase = dataBase;
            this.field = field;
            this.cdOnly = cdOnly;
        }

        public string DisplayName
        {
            get
            {
                if (field == Field.None)
                {
                    return "<" + StringTable.None + ">";
                }
                else
                {
                    if (cdOnly)
                        return dataBase.GetNameOfField(field);
                    else
                        return dataBase.GetNameOfFieldFull(field);
                }
            }
        }

        public Field Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value;
            }
        }
    }
}
