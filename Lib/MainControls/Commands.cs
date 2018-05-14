using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;
using System.Windows.Media;

namespace Big3.Hitbase.MainControls
{
    public class NewPlaylistCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Playlist.CreateEmptyPlaylist();
        }

        public string Caption { get; set; }

        public object Parameter { get; set; }

        #endregion
    }

    public class AddCDTracksToPlaylistParameter
    {
        public AddCDTracksToPlaylistParameter()
        {
            Tracks = new List<Track>();
        }

        public List<Track> Tracks { get; set; }

        public AddTracksToPlaylistType AddTracksType { get; set; }

        public int InsertIndex { get; set; }

        public bool ClearPlaylist { get; set; }

        public CD CD { get; set; }

        /// <summary>
        /// Soll ein Track aus Tracks direkt gespielt werden?
        /// </summary>
        public int PlayTrackId { get; set; }
    }


    public static class CatalogViewCommands
    {
        private static RoutedCommand changeView = new RoutedCommand("ChangeView", typeof(CatalogViewCommands));

        public static RoutedCommand ChangeView { get { return changeView; } }

        private static RoutedCommand addView = new RoutedCommand("AddView", typeof(CatalogViewCommands));

        public static RoutedCommand AddView { get { return addView; } }
    }

    public class ChangeViewCommandParameters
    {
        public CurrentViewMode ViewMode { get; set; }

        public Condition Condition { get; set; }
    }

    public class AddViewCommandParameters
    {
        public CurrentViewMode ViewMode { get; set; }

        public Condition Condition { get; set; }
        public Condition ConditionFromTree { get; set; }

        public string Title { get; set; }

        public string ImageResourceString { get; set; }
    }
}
