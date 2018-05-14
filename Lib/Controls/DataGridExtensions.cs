using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Big3.Hitbase.DataBaseEngine;
using System.Windows.Controls.Primitives;
using Big3.Hitbase.Miscellaneous;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Data;

namespace Big3.Hitbase.Controls
{
    public static class DataGridExtensions
    {
        // Attached property, um an die Spalten des DataGrids ein Field zu hängen. Die DateGridColumn hat leider kein Tag-Property.
        public static readonly DependencyProperty FieldProperty = DependencyProperty.RegisterAttached(
                "Field",
                typeof(Field),
                typeof(DataGridColumn),
                new FrameworkPropertyMetadata(Field.None)
                );
        public static void SetField(UIElement element, Boolean value)
        {
            element.SetValue(FieldProperty, value);
        }
        public static Boolean GetField(UIElement element)
        {
            return (Boolean)element.GetValue(FieldProperty);
        }

        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }
        public static int GetRowIndex(DataGridCell dataGridCell)
        {
            // Use reflection to get DataGridCell.RowDataItem property value.
            PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
        }
        public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
        {
            if (VisualTreeHelper.GetParent(dataGridPart) == null)
            {
                throw new NullReferenceException("Control is null.");
            }
            if (VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
            {
                return (DataGrid)VisualTreeHelper.GetParent(dataGridPart);
            }
            else
            {
                return GetDataGridFromChild(VisualTreeHelper.GetParent(dataGridPart));
            }
        }

    }

    public class DataGridEx : DataGrid
    {
        public DataGridEx()
        {
            this.Style = (Style)FindResource(typeof(DataGrid));

            Style style = new Style(typeof(DataGridCell), (Style)FindResource(typeof(DataGridCell)));
            style.Setters.Add(new EventSetter(PreviewKeyDownEvent, new KeyEventHandler(DataGridEx_PreviewKeyDown)));
            this.Resources.Add(typeof(DataGridCell), style);
            this.Resources.Add(typeof(DataGridRow), (Style)FindResource(typeof(DataGridRow)));
            BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(DataGridEx_BeginningEdit);
            CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(DataGridEx_CellEditEnding);
        }

        public bool IsInEditMode { get; set; }

        void DataGridEx_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SetValue(DragDrop.DragDrop.IsDragSourceProperty, true);
            IsInEditMode = false;
        }

        void DataGridEx_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            SetValue(DragDrop.DragDrop.IsDragSourceProperty, false);
            IsInEditMode = true;
        }

        void DataGridEx_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                DataGridCell cell = sender as DataGridCell;

                if (cell.IsEditing)
                {
                    int row = DataGridExtensions.GetRowIndex(cell);

                    if (row < Items.Count - 1)
                    {
                        CommitEdit();

                        CurrentCell = new DataGridCellInfo(Items[row + 1], cell.Column);
                        BeginEdit();

                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Up)
            {
                DataGridCell cell = sender as DataGridCell;

                if (cell.IsEditing)
                {
                    int row = DataGridExtensions.GetRowIndex(cell);

                    if (row > 0)
                    {
                        CommitEdit();

                        CurrentCell = new DataGridCellInfo(Items[row - 1], cell.Column);
                        BeginEdit();

                        e.Handled = true;
                    }
                }
            }
        }
    }

    public class DataGridMaxLengthTextColumn : DataGridTextColumn
    {
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(DataGridMaxLengthTextColumn), new UIPropertyMetadata(0));


        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            TextBox tb = base.GenerateEditingElement(cell, dataItem) as TextBox;

            tb.MaxLength = MaxLength;

            return tb;
        }
    }

    public class DataGridAutoCompleteTextColumn : DataGridMaxLengthTextColumn
    {
        public AutoCompleteTextBoxType AutoCompleteTextBoxType { get; set; }

        public DataBase DataBase { get; set; }

        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            TextBoxAutoComplete autoCompleteTextBox = new TextBoxAutoComplete();
            autoCompleteTextBox.AutoCompleteTextBoxType = AutoCompleteTextBoxType;
            autoCompleteTextBox.DataBase = DataBase;
            autoCompleteTextBox.DataContext = dataItem;
            autoCompleteTextBox.SetBinding(TextBox.TextProperty, Binding);

            return autoCompleteTextBox;
            //return base.GenerateEditingElement(cell, dataItem);
        }
    }

}
