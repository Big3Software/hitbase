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
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Miscellaneous;
using System.Windows.Controls.Primitives;

namespace Big3.Hitbase.CatalogView
{
    /// <summary>
    /// Interaction logic for CatalogListView.xaml
    /// </summary>
    public partial class CatalogListView : UserControl
    {
        public delegate void OpenContextMenuHandler(object sender, MouseButtonEventArgs e);
        public event OpenContextMenuHandler OpenContextMenu;
        public event OpenContextMenuHandler OpenHeaderContextMenu;

        public event EventHandler DoubleClick;
        public event EventHandler Refresh;
        public event EventHandler SelectionChanged;

        public event EventHandler DoDragDrop;

        public event EventHandler DeleteItem;

        public CatalogListView()
        {
            InitializeComponent();

            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserResizeRows = false;
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            dataGrid.RowHeaderWidth = 0;
            dataGrid.HorizontalGridLinesBrush = new SolidColorBrush(System.Windows.Media.Colors.WhiteSmoke);
            dataGrid.VerticalGridLinesBrush = new SolidColorBrush(System.Windows.Media.Colors.WhiteSmoke);
            //dataGrid.AlternatingRowBackground = new SolidColorBrush(Colors.WhiteSmoke);
            dataGrid.MouseRightButtonUp += new MouseButtonEventHandler(dataGrid_MouseRightButtonUp);
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
            dataGrid.MouseMove += new MouseEventHandler(dataGrid_MouseMove);
            dataGrid.EnableRowVirtualization = true;
            //dataGrid.EnableColumnVirtualization = true;
        }

        public DataBase DataBase { get; set; }

        void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(dataGrid);

                object selectedItem = dataGrid.SelectedItem;
                if (selectedItem != null)
                {
                    DependencyObject dep = (DependencyObject)e.OriginalSource;

                    while ((dep != null) && !(dep is DataGridCell))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }

                    if (dep == null)
                        return;

                    if (DoDragDrop != null)
                        DoDragDrop(this, new EventArgs());
/*                    DataGridRow container = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(selectedItem);
                    if (container != null)
                    {
                        DataObject dataObject = new DataObject();
                        
                        dataObject.SetData(DataFormats.FileDrop, "c:\\a.mp3");
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(container, dataObject, DragDropEffects.Copy);
                    }*/
                }
            }

        }

        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
        }

        private ScrollViewer scrollViewer = null;
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollViewer == null)
                {
                    DependencyObject border = VisualTreeHelper.GetChild(this, 0);
                    if (border != null)
                    {
                        scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                    }
                }

                return scrollViewer;
            }
        }

        public ColumnFieldCollection CurrentFields
        {
            get;
            set;
        }

        void dataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridColumnHeader)
            {
                if (OpenHeaderContextMenu != null)
                    OpenHeaderContextMenu(sender, e);
            }
            if (dep is DataGridCell)
            {
                if (OpenContextMenu != null)
                    OpenContextMenu(sender, e);
            }
        }

        public ItemCollection Items
        {
            get
            {
                return dataGrid.Items;
            }
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                return dataGrid.Columns;
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return dataGrid.ItemsSource;
            }
            set
            {
                dataGrid.ItemsSource = value;
            }
        }

        public IList SelectedItems
        {
            get
            {
                return dataGrid.SelectedItems;
            }
        }

        private void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;   // prevent the built-in sort from sorting            
            PerformCustomSort(e.Column);
        }

        private void PerformCustomSort(DataGridColumn column)
        {
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            column.SortDirection = direction;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            MySort mySort = new MySort(this, direction, column);
            lcv.CustomSort = mySort;    // provide our own sort    
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                if (DoubleClick != null)
                    DoubleClick(sender, new EventArgs());
            }
        }

        private void DataGridCell_MouseEnter(object sender, MouseEventArgs e)
        {
            DataGridCell dgc = (DataGridCell)sender;

            DependencyObject dep = (DependencyObject)sender;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            DataGridRow row = (DataGridRow)dep;

            row.Background = Brushes.AliceBlue;
        }

        private void DataGridCell_MouseLeave(object sender, MouseEventArgs e)
        {
            DataGridCell dgc = (DataGridCell)sender;

            DependencyObject dep = (DependencyObject)sender;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            DataGridRow row = (DataGridRow)dep;

            row.Background = null;
        }

        public class MySort : IComparer
        {
            public MySort(CatalogListView listView, ListSortDirection direction, DataGridColumn column)
            {
                DataBase = listView.DataBase;
                DataGrid = listView.dataGrid;
                Fields = listView.CurrentFields;
                Direction = direction;
                ColumnIndex = DataGrid.Columns.IndexOf(column);
            }

            private ColumnFieldCollection Fields { get; set; }

            private DataGrid DataGrid
            {
                get;
                set;
            }

            private DataBase DataBase { get; set; }

            public ListSortDirection Direction
            {
                get;
                private set;
            }

            public int ColumnIndex
            {
                get;
                private set;
            }

            int StringCompare(string s1, string s2)
            {
                if (s1 == null)
                    s1 = "";
                if (s2 == null)
                    s2 = "";
                if (Direction == ListSortDirection.Ascending)
                    return s1.CompareTo(s2);
                return s2.CompareTo(s1);
            }

            int Int32Compare(int val1, int val2)
            {
                if (Direction == ListSortDirection.Ascending)
                    return val1.CompareTo(val2);
                return val2.CompareTo(val1);
            }

            int IComparer.Compare(object X, object Y)
            {
                DataGridItem dgi1 = (DataGridItem)X;
                DataGridItem dgi2 = (DataGridItem)Y;

                int colIndex = ColumnIndex;
                if (dgi1.FirstColumnAddToPlaylist)
                    colIndex--;

                if (dgi1.Items[colIndex].Value is Int32)
                {
                    int val1 = (int)dgi1.Items[colIndex].Value;
                    int val2 = (int)dgi2.Items[colIndex].Value;

                    return Int32Compare(val1, val2);
                }

                if (IsDateField(dgi1.Items[colIndex].Field))
                {
                    string val1 = dgi1.Items[colIndex].Value as string;
                    string val2 = dgi2.Items[colIndex].Value as string;

                    return StringCompare(val1, val2);
                }

                string str1 = dgi1.Items[colIndex].DisplayValue;
                string str2 = dgi2.Items[colIndex].DisplayValue;

                return StringCompare(str1, str2);
            }

            private bool IsDateField(Field field)
            {
                if (field == Field.Date)
                    return true;

                if (field == Field.User1 || field == Field.User2 || field == Field.User3 || field == Field.User4 || field == Field.User5)
                {
                    UserFieldType uft = DataBase.GetUserFieldType(field);
                    if (uft == UserFieldType.Date)
                        return true;
                }

                return false;
            }
        }

        private void dataGrid_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (dataGrid.Items != null)
            {
                int index = 0;

                foreach (DataGridItem item in this.dataGrid.Items)
                {
                    if (item.Items[0].DisplayValue.ToLower().StartsWith(e.Text.ToLower()))
                    {
                        dataGrid.SelectedIndex = index;
                        dataGrid.ScrollIntoView(item);
                        break;
                    }

                    index++;
                }
            }
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                if (Refresh != null)
                    Refresh(sender, e);
                e.Handled = true;
                return;
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DeleteItem != null)
                    DeleteItem(this, e);
                e.Handled = true;
            }
        }
    }
}
