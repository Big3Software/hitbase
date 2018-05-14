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

namespace Big3.Hitbase.CatalogView
{
    /// <summary>
    /// Interaction logic for CatalogTreeView.xaml
    /// </summary>
    public partial class CatalogTreeView : UserControl
    {
        public delegate void OpenContextMenuHandler(object sender, ContextMenuEventArgs e);
        public event OpenContextMenuHandler OpenContextMenu;
        public event EventHandler DoDragDrop;

        public event EventHandler SelectionChanged;
        public event EventHandler DoubleClick;
        public event EventHandler Refresh;

        public event EventHandler DeleteItem;

        public DataBase DataBase { get; set; }

        internal string FilterString { get; set; }

        public CatalogTreeView()
        {
            InitializeComponent();

            TreeView.MouseMove += new MouseEventHandler(TreeView_MouseMove);
            TreeView.PreviewMouseDoubleClick += new MouseButtonEventHandler(TreeView_PreviewMouseDoubleClick);

            Unloaded += new RoutedEventHandler(CatalogTreeView_Unloaded);

            TreeViewWidth.Width = new GridLength(Big3.Hitbase.Configuration.Settings.Current.CatalogTreeViewWidth);
        }

        void CatalogTreeView_Unloaded(object sender, RoutedEventArgs e)
        {
            Big3.Hitbase.Configuration.Settings.Current.CatalogTreeViewWidth = (int)TreeViewWidth.ActualWidth;
        }

        void TreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualTreeExtensions.FindParent<TreeViewItem>((DependencyObject)e.OriginalSource);

            if (item == null)
                return;

            e.Handled = true;

            if (DoubleClick != null)
                DoubleClick(sender, new EventArgs());
        }

        void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                while ((dep != null) && !(dep is TreeViewItem))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                TreeViewItem treeViewItem = dep as TreeViewItem;

                if (DoDragDrop != null)
                    DoDragDrop(this, new EventArgs());
            }
        }

        void newItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)sender;

            while ((dep != null) && !(dep is TreeViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            TreeViewItem treeViewItem = (TreeViewItem)dep;
            treeViewItem.IsSelected = true;
            treeViewItem.Focus();
            e.Handled = true;
        }

        private void TreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {            
            if (OpenContextMenu != null)
                OpenContextMenu(sender, e);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
        }

        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DeleteItem != null)
                    DeleteItem(sender, e);
                e.Handled = true;
            }

            if (e.Key == Key.F5)
            {
                if (Refresh != null)
                    Refresh(sender, e);
                e.Handled = true;
                return;
            }
        }

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;

            if (item != null && item.DataContext is ArtistOverviewItem)
            {
                if (!FilterArtistRow(item.DataContext as ArtistOverviewItem))
                {
                    System.ComponentModel.ICollectionView view = CollectionViewSource.GetDefaultView(item.ItemsSource);
                    if (view != null)
                        view.Filter = new Predicate<object>(FilterCDRow);
                }
            }

            if (item != null && item.DataContext is ArtistOverviewCDSetItem)
            {
                ArtistOverviewCDSetItem cdSetItem = item.DataContext as ArtistOverviewCDSetItem;

                if (!FilterTreeCDSetRow(cdSetItem, false) && (cdSetItem.Parent == null || !FilterArtistRow(cdSetItem.Parent)))
                {
                    System.ComponentModel.ICollectionView view = CollectionViewSource.GetDefaultView(item.ItemsSource);
                    if (view != null)
                        view.Filter = new Predicate<object>(FilterCDRow);
                }
            }

            if (item != null && item.DataContext is ArtistOverviewCDItem)
            {
                ArtistOverviewCDItem cdItem = item.DataContext as ArtistOverviewCDItem;

                if (cdItem.Tracks == null || cdItem.Tracks.Count == 1 && cdItem.Tracks[0].ID == 0)
                {
                    CD cd = DataBase.GetCDById(cdItem.ID);

                    cdItem.Tracks.Clear();

                    foreach (Track track in cd.Tracks)
                    {
                        ArtistOverviewTrackItem trackItem = new ArtistOverviewTrackItem();
                        trackItem.ID = track.ID;
                        trackItem.Title = GetTrackTitleText(cd, track);
                        trackItem.Soundfile = track.Soundfile;

                        cdItem.Tracks.Add(trackItem);
                    }

                    item.ItemsSource = cdItem.Tracks;
                }

                if (!FilterTreeCDRow(cdItem, false) && (cdItem.Parent == null || !FilterArtistRow(cdItem.Parent)))
                {
                    System.ComponentModel.ICollectionView view = CollectionViewSource.GetDefaultView(item.ItemsSource);
                    if (view != null)
                        view.Filter = new Predicate<object>(FilterTrackRow);
                }
            }

        }

        private string GetTrackTitleText(CD cd, Track track)
        {
            string text = "";

            if (cd.Sampler)
            {
                text = string.Format("{0}. {1} - {2}", track.TrackNumber, track.Artist, track.Title);
            }
            else
            {
                text = string.Format("{0}. {1}", track.TrackNumber, track.Title);
            }

            if (track.Length > 0)
                text += string.Format(" [{0}]", Misc.GetShortTimeString(track.Length));

            return text;
        }

        private void TreeView_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            for (int i = 0; i < TreeView.Items.Count;i++ )
            {
                ArtistOverviewItem item = TreeView.Items[i] as ArtistOverviewItem;

                if (item.TextSearch.StartsWith(e.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    item.IsSelected = true;
                    item.IsExpanded = true;
                    break;
                }
            }
        }

        internal bool FilterTreeRow(ArtistOverviewItem artistRow, string filterString)
        {
            if (FilterArtistRow(artistRow))
                return true;

            foreach (object item in artistRow.Items)
            { 
                ArtistOverviewCDItem cdItem = item as ArtistOverviewCDItem;

                if (cdItem != null && FilterTreeCDRow(cdItem, true))
                    return true;
            }

            return false;
        }

        private bool FilterArtistRow(ArtistOverviewItem artistRow)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            if (artistRow.Artist.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        private bool FilterCDRow(object row)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            ArtistOverviewCDItem cdItem = row as ArtistOverviewCDItem;
            if (cdItem != null)
            {
                string filterString = FilterString.ToLower();

                return FilterTreeCDRow(cdItem, true);
            }

            ArtistOverviewCDSetItem cdSetItem = row as ArtistOverviewCDSetItem;
            if (cdSetItem != null)
            {
                string filterString = FilterString.ToLower();

                return FilterTreeCDSetRow(cdSetItem, true);
            }

            return true;
        }

        internal bool FilterTreeCDSetRow(ArtistOverviewCDSetItem cdSetItem, bool withCDs)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            if (cdSetItem.Title.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            if (withCDs)
            {
                foreach (ArtistOverviewCDItem cdItem in cdSetItem.CDs)
                {
                    if (FilterTreeCDRow(cdItem, withCDs))
                        return true;
                }
            }

            return false;
        }

        internal bool FilterTreeCDRow(ArtistOverviewCDItem cdItem, bool withTracks)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            if (cdItem.Title.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            if (withTracks)
            {
                foreach (ArtistOverviewTrackItem trackItem in cdItem.Tracks)
                {
                    if (FilterTrackRow(trackItem))
                        return true;
                }
            }

            return false;
        }

        private bool FilterTrackRow(object item)
        {
            ArtistOverviewTrackItem trackItem = item as ArtistOverviewTrackItem;

            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            if (trackItem.Title.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
