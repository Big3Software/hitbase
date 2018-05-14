using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using Big3.Hitbase.MainWindowDesigner.Controls;
using System.Reflection;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.MainWindowDesigner.View;
using System.Windows;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    [Serializable]
    public class HitbaseComboBox : HitbaseControl
    {
        [NonSerialized]
        private MyComboBox comboBox;

        public HitbaseComboBox(MainCDUserControl dlg)
            : base(dlg)
        {
            comboBox = new MyComboBox(this);

            comboBox.IsEditable = false;
            comboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(comboBox_SelectionChanged);
        }

        void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveControlData();
        }

        protected override FrameworkElement Control
        {
            get
            {
                return comboBox;
            }
        }

        private System.Windows.Forms.ComboBoxStyle dropDownStyle;
        /// <summary>
        /// Der Style der ComboBox.
        /// </summary>
        [Category("Hitbase")]
        public System.Windows.Forms.ComboBoxStyle DropDownStyle
        {
            get { return dropDownStyle; }
            set 
            {
                dropDownStyle = value;

                if (dropDownStyle == System.Windows.Forms.ComboBoxStyle.DropDown)
                {
                    comboBox.IsEditable = true;
                    /*TODO_WPF!!!!!!!!!!!!!!!!comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;*/
                }
            }
        }

        private string listValuesPropertyName;
        /// <summary>
        /// Enthält den Namen der Property, die die Elemente enthält, die in der Combobox angezeigt werden sollen.
        /// </summary>
        [Category("Hitbase")]
        public string ListValuesPropertyName
        {
            get { return listValuesPropertyName; }
            set { listValuesPropertyName = value; }
        }

        /// <summary>
        /// Aktiviert oder deaktiviert den Designmodus.
        /// </summary>
        /// <param name="activateDesignMode"></param>
        public override void SetDesignMode(bool activateDesignMode)
        {
            if (activateDesignMode == true)
            {
                comboBox.IsEditable = false;
            }

            base.SetDesignMode(activateDesignMode);
        }

        /// <summary>
        /// Aktualisiert das Control mit den neuen Daten.
        /// </summary>
        public override void UpdateControlData()
        {
            if (hitbaseMainWindowControl.theCd != null && ListValuesPropertyName != null)
            {
                comboBox.Items.Clear();

                switch (Field)
                {
                    case Field.Category:
                        foreach (Category category in DataBase.AllCategories)
                            comboBox.Items.Add(category.Name);
                        break;
                    case Field.Medium:
                        foreach (Medium medium in DataBase.AllMediums)
                            comboBox.Items.Add(medium.Name);
                        break;
                    case Field.Label:
                        foreach (string label in DataBase.AllLabels)
                            comboBox.Items.Add(label);
                        break;
                    case Field.Language:
                        foreach (string language in DataBase.AllLanguages)
                            comboBox.Items.Add(language);
                        break;
                    default:
                        break;
                }
            }

            object o = GetDataFromCD();

            if (DropDownStyle == System.Windows.Forms.ComboBoxStyle.DropDownList)
                comboBox.SelectedItem = o;
            else
                comboBox.Text = o as String;
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
            if (DropDownStyle == System.Windows.Forms.ComboBoxStyle.DropDownList)
            {
                SaveDataToCD(comboBox.SelectedItem);
            }
            else
            {
                SaveDataToCD(comboBox.Text);
            }
        }

        /// <summary>
        /// Füllt alle Elemente aus der Datenbank in das Control
        /// </summary>
        private void FillList()
        {
        }

        public void SetDataSource(DataTable table, string displayMember, string valueMember)
        {
            comboBox.ItemsSource = table.Rows;
            comboBox.DisplayMemberPath = displayMember;
            comboBox.SelectedValuePath = valueMember;
        }

        public override int Baseline
        {
            get
            {
                /*TODO_WPF!!!!!!!!!!!!!!!!!!int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5);

                return ascentPixel + 7;*/
                return 0;
            }
        }

        public override string ControlName
        {
            get
            {
                return "ComboBox";
            }
        }
    }
}
