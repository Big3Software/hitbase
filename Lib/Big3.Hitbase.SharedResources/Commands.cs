using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Big3.Hitbase.SharedResources
{
    public static class HitbaseCommands
    {
        private static RoutedUICommand newPlaylist = new RoutedUICommand(StringTable.NewPlaylist, "NewPlaylist", typeof(HitbaseCommands));

        public static RoutedUICommand NewPlaylist
        {
            get { return newPlaylist; }
        }

        private static RoutedUICommand deletePlaylist = new RoutedUICommand(StringTable.DeletePlaylist, "DeletePlaylist", typeof(HitbaseCommands));

        public static RoutedUICommand DeletePlaylist
        {
            get { return deletePlaylist; }
        }

        public static RoutedUICommand AddTracksToPlaylist = new RoutedUICommand("AddTracksToPlaylist", "AddTracksToPlaylist", typeof(HitbaseCommands));
        public static RoutedUICommand AddTracksToPlaylistNow = new RoutedUICommand("AddTracksToPlaylistNow", "AddTracksToPlaylistNow", typeof(HitbaseCommands));
        public static RoutedUICommand AddTracksToPlaylistNext = new RoutedUICommand("AddTracksToPlaylistNext", "AddTracksToPlaylistNext", typeof(HitbaseCommands));
        public static RoutedUICommand AddTracksToPlaylistLast = new RoutedUICommand("AddTracksToPlaylistLast", "AddTracksToPlaylistLast", typeof(HitbaseCommands));
        
        public static RoutedUICommand AddAlbumToPlaylistNow = new RoutedUICommand("AddAlbumToPlaylistNow", "AddAlbumToPlaylistNow", typeof(HitbaseCommands));

        public static RoutedUICommand AddAlbumToPlaylistNext = new RoutedUICommand("AddAlbumToPlaylistNext", "AddAlbumToPlaylistNext", typeof(HitbaseCommands));

        public static RoutedUICommand AddAlbumToPlaylistLast = new RoutedUICommand("AddAlbumToPlaylistLast", "AddAlbumToPlaylistLast", typeof(HitbaseCommands));

        private static RoutedUICommand preListenTrack = new RoutedUICommand("PreListenTrack", "PreListenTrack", typeof(HitbaseCommands));

        public static RoutedUICommand PreListenTrack
        {
            get { return preListenTrack; }
        }

        private static RoutedUICommand openInNewTab = new RoutedUICommand(StringTable.OpenInNewTab, "OpenInNewTab", typeof(HitbaseCommands));

        public static RoutedUICommand OpenInNewTab
        {
            get { return openInNewTab; }
        }

        private static RoutedUICommand closeTab = new RoutedUICommand(StringTable.CloseTab, "CloseTab", typeof(HitbaseCommands), new InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control) });

        public static RoutedUICommand CloseTab
        {
            get { return closeTab; }
        }

        private static RoutedUICommand configureMusicLibrary = new RoutedUICommand(StringTable.ConfigureMusicLibrary, "ConfigureMusicLibrary", typeof(HitbaseCommands));

        public static RoutedUICommand ConfigureMusicLibrary
        {
            get { return configureMusicLibrary; }
        }

        private static RoutedUICommand editCategories = new RoutedUICommand(StringTable.EditCategories, "EditCategories", typeof(HitbaseCommands));

        public static RoutedUICommand EditCategories
        {
            get { return editCategories; }
        }

        private static RoutedUICommand editMediums = new RoutedUICommand(StringTable.EditMediums, "EditMediums", typeof(HitbaseCommands));

        public static RoutedUICommand EditMediums
        {
            get { return editMediums; }
        }

        private static RoutedUICommand editCodes = new RoutedUICommand(StringTable.EditCodes, "EditCodes", typeof(HitbaseCommands));

        public static RoutedUICommand EditCodes
        {
            get { return editCodes; }
        }

        private static RoutedUICommand editRoles = new RoutedUICommand(StringTable.EditRoles, "EditRoles", typeof(HitbaseCommands));

        public static RoutedUICommand EditRoles
        {
            get { return editRoles; }
        }

        private static RoutedUICommand editCDSets = new RoutedUICommand(StringTable.EditCDSets, "EditCDSets", typeof(HitbaseCommands));

        public static RoutedUICommand EditCDSets
        {
            get { return editCDSets; }
        }

        private static RoutedUICommand editPersonGroup = new RoutedUICommand(StringTable.EditPersonGroup, "EditPersonGroup", typeof(HitbaseCommands));

        public static RoutedUICommand EditPersonGroup
        {
            get { return editPersonGroup; }
        }

        private static RoutedUICommand personGroupProperties = new RoutedUICommand(StringTable.EditPersonGroup, "PersonGroupProperties", typeof(HitbaseCommands));

        public static RoutedUICommand PersonGroupProperties
        {
            get { return personGroupProperties; }
        }


        private static RoutedUICommand changeCode = new RoutedUICommand(StringTable.ChangeCode, "ChangeCode", typeof(HitbaseCommands));

        public static RoutedUICommand ChangeCode
        {
            get { return changeCode; }
        }

        private static RoutedUICommand saveCurrentCD = new RoutedUICommand(StringTable.SaveCurrentCD, "SaveCurrentCD", typeof(HitbaseCommands));

        public static RoutedUICommand SaveCurrentCD
        {
            get { return saveCurrentCD; }
        }

        private static RoutedUICommand addNewAlbum = new RoutedUICommand(StringTable.AddNewAlbum, "AddNewAlbum", typeof(HitbaseCommands));

        public static RoutedUICommand AddNewAlbum
        {
            get { return addNewAlbum; }
        }

        private static RoutedUICommand searchAmazon = new RoutedUICommand(StringTable.SearchAmazon, "SearchAmazon", typeof(HitbaseCommands));

        public static RoutedUICommand SearchAmazon
        {
            get { return searchAmazon; }
        }

        private static RoutedUICommand loanManager = new RoutedUICommand(StringTable.LoanManager, "LoanManager", typeof(HitbaseCommands));

        public static RoutedUICommand LoanManager
        {
            get { return loanManager; }
        }

        private static RoutedUICommand wishlist = new RoutedUICommand(StringTable.Wishlist, "Wishlist", typeof(HitbaseCommands));

        public static RoutedUICommand Wishlist
        {
            get { return wishlist; }
        }

        private static RoutedUICommand cdLoaned = new RoutedUICommand(StringTable.Loaned, "CDLoaned", typeof(HitbaseCommands));

        public static RoutedUICommand CDLoaned
        {
            get { return cdLoaned; }
        }

        private static RoutedUICommand printAlbum = new RoutedUICommand(StringTable.PrintAlbum, "PrintAlbum", typeof(HitbaseCommands));

        public static RoutedUICommand PrintAlbum
        {
            get { return printAlbum; }
        }

        private static RoutedUICommand openAlbum = new RoutedUICommand(StringTable.Properties, "OpenAlbum", typeof(HitbaseCommands));

        public static RoutedUICommand OpenAlbum
        {
            get { return openAlbum; }
        }

        private static RoutedUICommand openTrack = new RoutedUICommand(StringTable.Properties, "OpenTrack", typeof(HitbaseCommands));

        public static RoutedUICommand OpenTrack
        {
            get { return openTrack; }
        }

        private static RoutedUICommand showRecordOptions = new RoutedUICommand(StringTable.ShowRecordOptions, "ShowRecordOptions", typeof(HitbaseCommands));

        public static RoutedUICommand ShowRecordOptions
        {
            get { return showRecordOptions; }
        }

        private static RoutedUICommand startRecord = new RoutedUICommand(StringTable.StartRecord, "StartRecord", typeof(HitbaseCommands));

        public static RoutedUICommand StartRecord
        {
            get { return startRecord; }
        }

        private static RoutedUICommand selectCodes = new RoutedUICommand(StringTable.SelectCodes, "SelectCodes", typeof(HitbaseCommands));

        public static RoutedUICommand SelectCodes
        {
            get { return selectCodes; }
        }

        private static RoutedUICommand chooseColumns = new RoutedUICommand(StringTable.ChooseColumns, "ChooseColumns", typeof(HitbaseCommands));

        public static RoutedUICommand ChooseColumns
        {
            get { return chooseColumns; }
        }

        private static RoutedUICommand copyToClipboard = new RoutedUICommand(StringTable.CopyToClipboard, "CopyToClipboard", typeof(HitbaseCommands));

        public static RoutedUICommand CopyToClipboard
        {
            get { return copyToClipboard; }
        }

        private static RoutedCommand showMainStatusText = new RoutedCommand("ShowMainStatusText", typeof(HitbaseCommands));

        public static RoutedCommand ShowMainStatusText
        {
            get { return showMainStatusText; }
        }

        private static RoutedCommand setMainStatusText = new RoutedCommand("SetMainStatusText", typeof(HitbaseCommands));

        public static RoutedCommand SetMainStatusText
        {
            get { return setMainStatusText; }
        }

        private static RoutedUICommand cancelRecord = new RoutedUICommand(StringTable.CancelRecord, "CancelRecord", typeof(HitbaseCommands));

        public static RoutedUICommand CancelRecord
        {
            get { return cancelRecord; }
        }

        private static RoutedUICommand editDatabaseFields = new RoutedUICommand(StringTable.EditDatabaseFields, "EditDatabaseFields", typeof(HitbaseCommands));

        public static RoutedUICommand EditDatabaseFields
        {
            get { return editDatabaseFields; }
        }

        private static RoutedUICommand searchAndReplace = new RoutedUICommand(StringTable.SearchAndReplace, "SearchAndReplace", typeof(HitbaseCommands));

        public static RoutedUICommand SearchAndReplace
        {
            get { return searchAndReplace; }
        }

        private static RoutedUICommand copyFields = new RoutedUICommand(StringTable.CopyFields, "CopyFields", typeof(HitbaseCommands));

        public static RoutedUICommand CopyFields
        {
            get { return copyFields; }
        }

        private static RoutedUICommand burnCD = new RoutedUICommand(StringTable.BurnCD, "BurnCD", typeof(HitbaseCommands));

        public static RoutedUICommand BurnCD
        {
            get { return burnCD; }
        }

        private static RoutedUICommand showPlayer = new RoutedUICommand(StringTable.ShowPlayer, "ShowPlayer", typeof(HitbaseCommands));

        public static RoutedUICommand ShowPlayer
        {
            get { return showPlayer; }
        }

        private static RoutedUICommand addToWishlist = new RoutedUICommand(StringTable.AddToWishlist, "AddToWishlist", typeof(HitbaseCommands));

        public static RoutedUICommand AddToWishlist
        {
            get { return addToWishlist; }
        }

        private static RoutedUICommand playInFullScreen2D = new RoutedUICommand(StringTable.PlayInFullScreen, "PlayInFullScreen2D", typeof(HitbaseCommands));

        public static RoutedUICommand PlayInFullScreen2D
        {
            get { return playInFullScreen2D; }
        }

        private static RoutedUICommand playInFullScreen3D = new RoutedUICommand(StringTable.PlayInFullScreen, "PlayInFullScreen3D", typeof(HitbaseCommands));

        public static RoutedUICommand PlayInFullScreen3D
        {
            get { return playInFullScreen3D; }
        }

        private static RoutedUICommand refresh = new RoutedUICommand(StringTable.Refresh, "Refresh", typeof(HitbaseCommands));

        public static RoutedUICommand Refresh
        {
            get { return refresh; }
        }


        private static RoutedUICommand printCatalog = new RoutedUICommand(StringTable.Print, "PrintCatalog", typeof(HitbaseCommands));

        public static RoutedUICommand PrintCatalog
        {
            get { return printCatalog; }
        }

        private static RoutedUICommand renameSearch = new RoutedUICommand(StringTable.RenameSearch, "RenameSearch", typeof(HitbaseCommands));

        public static RoutedUICommand RenameSearch
        {
            get { return renameSearch; }
        }

        private static RoutedUICommand deleteSearch = new RoutedUICommand(StringTable.DeleteSearch, "DeleteSearch", typeof(HitbaseCommands));

        public static RoutedUICommand DeleteSearch
        {
            get { return deleteSearch; }
        }

        private static RoutedUICommand deleteAlbum = new RoutedUICommand(StringTable.Delete, "DeleteAlbum", typeof(HitbaseCommands), new InputGestureCollection { new KeyGesture(Key.Delete, ModifierKeys.None, StringTable.DelKey) });

        public static RoutedUICommand DeleteAlbum
        {
            get { return deleteAlbum; }
        }


        private static RoutedUICommand searchCDInCDArchive = new RoutedUICommand(StringTable.SearchCDInCDArchive, "SearchCDInCDArchive", typeof(HitbaseCommands));

        public static RoutedUICommand SearchCDInCDArchive
        {
            get { return searchCDInCDArchive; }
        }

        private static RoutedUICommand sendCDToCDArchive = new RoutedUICommand(StringTable.SendCDToCDArchive, "SendCDToCDArchive", typeof(HitbaseCommands));

        public static RoutedUICommand SendCDToCDArchive
        {
            get { return sendCDToCDArchive; }
        }


        private static RoutedUICommand sendCatalogToInternet = new RoutedUICommand(StringTable.SendCatalogToInternet, "SendCatalogToInternet", typeof(HitbaseCommands));

        public static RoutedUICommand SendCatalogToInternet
        {
            get { return sendCatalogToInternet; }
        }

        private static RoutedUICommand browseCDArchive = new RoutedUICommand(StringTable.BrowseCDArchive, "BrowseCDArchive", typeof(HitbaseCommands));

        public static RoutedUICommand BrowseCDArchive
        {
            get { return browseCDArchive; }
        }



        private static RoutedUICommand chooseSkin = new RoutedUICommand(StringTable.ChooseSkin, "ChooseSkin", typeof(HitbaseCommands));

        public static RoutedUICommand ChooseSkin
        {
            get { return chooseSkin; }
        }

        private static RoutedUICommand openTrackLocation = new RoutedUICommand(StringTable.OpenTrackLocation, "OpenTrackLocation", typeof(HitbaseCommands));

        public static RoutedUICommand OpenTrackLocation
        {
            get { return openTrackLocation; }
        }

        private static RoutedUICommand play = new RoutedUICommand(StringTable.Play, "Play", typeof(HitbaseCommands));

        public static RoutedUICommand Play
        {
            get { return play; }
        }

        private static RoutedUICommand pause = new RoutedUICommand(StringTable.Pause, "Pause", typeof(HitbaseCommands));

        public static RoutedUICommand Pause
        {
            get { return pause; }
        }

        private static RoutedUICommand prevTrack = new RoutedUICommand(StringTable.PrevTrack, "PrevTrack", typeof(HitbaseCommands));

        public static RoutedUICommand PrevTrack
        {
            get { return prevTrack; }
        }

        private static RoutedUICommand nextTrack = new RoutedUICommand(StringTable.NextTrack, "NextTrack", typeof(HitbaseCommands));

        public static RoutedUICommand NextTrack
        {
            get { return nextTrack; }
        }

        private static RoutedUICommand saveWishlist = new RoutedUICommand(StringTable.SaveWishlist, "SaveWishlist", typeof(HitbaseCommands));

        public static RoutedUICommand SaveWishlist
        {
            get { return saveWishlist; }
        }

        private static RoutedUICommand printWishlist = new RoutedUICommand(StringTable.PrintWishlist, "PrintWishlist", typeof(HitbaseCommands));

        public static RoutedUICommand PrintWishlist
        {
            get { return printWishlist; }
        }

        private static RoutedUICommand copyToExternalMedium = new RoutedUICommand(StringTable.CopyToExternalMedium, "CopyToExternalMedium", typeof(HitbaseCommands));

        public static RoutedUICommand CopyToExternalMedium
        {
            get { return copyToExternalMedium; }
        }

        private static RoutedUICommand uploadToMyHitbase = new RoutedUICommand(StringTable.UploadToMyHitbase, "UploadToMyHitbase", typeof(HitbaseCommands));

        public static RoutedUICommand UploadToMyHitbase
        {
            get { return uploadToMyHitbase; }
        }


        private static RoutedUICommand showExtendedQualityDialog = new RoutedUICommand(StringTable.ShowExtendedQualityDialog, "ShowExtendedQualityDialog", typeof(HitbaseCommands));

        public static RoutedUICommand ShowExtendedQualityDialog
        {
            get { return showExtendedQualityDialog; }
        }


        private static RoutedUICommand linkCD = new RoutedUICommand(StringTable.LinkCD, "LinkCD", typeof(HitbaseCommands));

        public static RoutedUICommand LinkCD
        {
            get { return linkCD; }
        }

        private static RoutedUICommand adjustSpelling = new RoutedUICommand(StringTable.AdjustSpelling, "AdjustSpelling", typeof(HitbaseCommands));

        public static RoutedUICommand AdjustSpelling
        {
            get { return adjustSpelling; }
        }

        private static RoutedUICommand copyCDArtistToTracks = new RoutedUICommand(StringTable.CopyCDArtistToTracks, "CopyCDArtistToTracks", typeof(HitbaseCommands));

        public static RoutedUICommand CopyCDArtistToTracks
        {
            get { return copyCDArtistToTracks; }
        }

        private static RoutedUICommand aboutHitbase = new RoutedUICommand(StringTable.About, "AboutHitbase", typeof(HitbaseCommands));

        public static RoutedUICommand AboutHitbase
        {
            get { return aboutHitbase; }
        }

        private static RoutedUICommand showNormalizeOptions = new RoutedUICommand(StringTable.ShowNormalizeOptions, "ShowNormalizeOptions", typeof(HitbaseCommands));

        public static RoutedUICommand ShowNormalizeOptions
        {
            get { return showNormalizeOptions; }
        }

        private static RoutedUICommand openCDDrive = new RoutedUICommand(StringTable.OpenCDDrive, "OpenCDDrive", typeof(HitbaseCommands));

        public static RoutedUICommand OpenCDDrive
        {
            get { return openCDDrive; }
        }

        private static RoutedUICommand deletePlaylistItem = new RoutedUICommand(StringTable.Delete, "DeletePlaylistItem", typeof(HitbaseCommands), new InputGestureCollection { new KeyGesture(Key.Delete, ModifierKeys.None, StringTable.DelKey) });

        public static RoutedUICommand DeletePlaylistItem
        {
            get { return deletePlaylistItem; }
        }

        private static RoutedUICommand deleteWishlistItem = new RoutedUICommand(StringTable.Delete, "DeleteWishlistItem", typeof(HitbaseCommands), new InputGestureCollection { new KeyGesture(Key.Delete, ModifierKeys.None, StringTable.DelKey) });

        public static RoutedUICommand DeleteWishlistItem
        {
            get { return deleteWishlistItem; }
        }

        private static RoutedUICommand firstSteps = new RoutedUICommand(StringTable.FirstSteps, "FirstSteps", typeof(HitbaseCommands));

        public static RoutedUICommand FirstSteps
        {
            get { return firstSteps; }
        }

        public static RoutedUICommand OpenCatalog = new RoutedUICommand(StringTable.OpenCatalog, "OpenCatalog", typeof(HitbaseCommands));

        public static RoutedUICommand SaveAs = new RoutedUICommand(StringTable.SaveAs, "SaveAs", typeof(HitbaseCommands));

        public static RoutedUICommand PlayAlbumNow = new RoutedUICommand(StringTable.PlayAlbumNow, "PlayAlbumNow", typeof(HitbaseCommands));
        public static RoutedUICommand PlayAlbumNext = new RoutedUICommand(StringTable.PlayAlbumNext, "PlayAlbumNext", typeof(HitbaseCommands));
        public static RoutedUICommand PlayAlbumLast = new RoutedUICommand(StringTable.PlayAlbumLast, "PlayAlbumLast", typeof(HitbaseCommands));

        public static RoutedUICommand EjectCD = new RoutedUICommand(StringTable.EjectCD, "EjectCD", typeof(HitbaseCommands));

        public static RoutedUICommand Rename = new RoutedUICommand(StringTable.Rename, "Rename", typeof(HitbaseCommands), new InputGestureCollection { new KeyGesture(Key.F2, ModifierKeys.None, "F2") });

        public static RoutedUICommand TrackInformation = new RoutedUICommand(StringTable.TrackInformation, "TrackInformation", typeof(HitbaseCommands));
        public static RoutedUICommand PinTab = new RoutedUICommand(StringTable.PinTab, "PinTab", typeof(HitbaseCommands));

        public static RoutedUICommand CompactDataBase = new RoutedUICommand(StringTable.CompactDataBase, "CompactDataBase", typeof(HitbaseCommands));

        public static RoutedUICommand ShowVisualization = new RoutedUICommand("ShowVirtualization", "ShowVisualization", typeof(HitbaseCommands));

        public static RoutedUICommand AddNewAlbumFromDirectory = new RoutedUICommand("AddNewAlbumFromDirectory", "AddNewAlbumFromDirectory", typeof(HitbaseCommands));

        public static RoutedUICommand OpenHitbaseHomepage = new RoutedUICommand("OpenHitbaseHomepage", "OpenHitbaseHomepage", typeof(HitbaseCommands));
        public static RoutedUICommand OpenHitbaseForum = new RoutedUICommand("OpenHitbaseForum", "OpenHitbaseForum", typeof(HitbaseCommands));
        public static RoutedUICommand OpenCDArchiveHomepage = new RoutedUICommand("OpenCDArchiveHomepage", "OpenCDArchiveHomepage", typeof(HitbaseCommands));
        public static RoutedUICommand ShowHelp = new RoutedUICommand("ShowHelp", "ShowHelp", typeof(HitbaseCommands));

        public static RoutedUICommand EditSort = new RoutedUICommand("EditSort", "EditSort", typeof(HitbaseCommands));

        public static RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(HitbaseCommands));

        public static RoutedUICommand ExportHTML = new RoutedUICommand("ExportHTML", "ExportHTML", typeof(HitbaseCommands));
        public static RoutedUICommand ExportTXT = new RoutedUICommand("ExportTXT", "ExportTXT", typeof(HitbaseCommands));

        public static RoutedUICommand OrderHitbase = new RoutedUICommand("OrderHitbase", "OrderHitbase", typeof(HitbaseCommands));
        public static RoutedUICommand ActivateHitbase = new RoutedUICommand("ActivateHitbase", "ActivateHitbase", typeof(HitbaseCommands));

        public static RoutedUICommand StartRecordSelectedTracks = new RoutedUICommand("StartRecordSelectedTracks", "StartRecordSelectedTracks", typeof(HitbaseCommands));

        public static RoutedUICommand ShowFrequencyBand = new RoutedUICommand("ShowFrequencyBand", "ShowFrequencyBand", typeof(HitbaseCommands));
        public static RoutedUICommand ShowLyrics = new RoutedUICommand("ShowLyrics", "ShowLyrics", typeof(HitbaseCommands));

        public static RoutedUICommand CorrectTrackNumbers = new RoutedUICommand("CorrectTrackNumbers", "CorrectTrackNumbers", typeof(HitbaseCommands));

        public static RoutedUICommand ShowPartyMode = new RoutedUICommand("ShowPartyMode", "ShowPartyMode", typeof(HitbaseCommands));

        public static RoutedUICommand SendEMail = new RoutedUICommand("SendEMail", "SendEMail", typeof(HitbaseCommands));

        public static RoutedUICommand ShowOptions = new RoutedUICommand("ShowOptions", "ShowOptions", typeof(HitbaseCommands));
        public static RoutedUICommand NewCatalog = new RoutedUICommand("NewCatalog", "NewCatalog", typeof(HitbaseCommands));

        public static RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(HitbaseCommands));
        public static RoutedUICommand PrintCDCover = new RoutedUICommand("PrintCDCover", "PrintCDCover", typeof(HitbaseCommands));
        public static RoutedUICommand Print = new RoutedUICommand("Print", "Print", typeof(HitbaseCommands));
        public static RoutedUICommand PrintCD = new RoutedUICommand("PrintCD", "PrintCD", typeof(HitbaseCommands));

        public static RoutedUICommand ShowBorders = new RoutedUICommand("ShowBorders", "ShowBorders", typeof(HitbaseCommands));

        public static RoutedUICommand Zoom = new RoutedUICommand("Zoom", "Zoom", typeof(HitbaseCommands));
        public static RoutedUICommand ZoomWholePage = new RoutedUICommand("ZoomWholePage", "ZoomWholePage", typeof(HitbaseCommands));
        public static RoutedUICommand ZoomWholeWidth = new RoutedUICommand("ZoomWholeWidth", "ZoomWholeWidth", typeof(HitbaseCommands));

        public static RoutedUICommand ChooseFont = new RoutedUICommand("ChooseFont", "ChooseFont", typeof(HitbaseCommands));
        public static RoutedUICommand SizeAndPosition = new RoutedUICommand("SizeAndPosition", "SizeAndPosition", typeof(HitbaseCommands));

        public static RoutedUICommand DeleteCDTracksFromPlaylist = new RoutedUICommand("DeleteCDTracksFromPlaylist", "DeleteCDTracksFromPlaylist", typeof(HitbaseCommands));
        public static RoutedUICommand SavePlaylist = new RoutedUICommand("SavePlaylist", "SavePlaylist", typeof(HitbaseCommands));

        public static RoutedUICommand SetRecordSpeed = new RoutedUICommand("SetRecordSpeed", "SetRecordSpeed", typeof(HitbaseCommands));
        
    }

    public class HitbaseRoutedUICommand : RoutedUICommand
    {

    }

    public class AddTracksToPlaylistParameter
    {
        public AddTracksToPlaylistParameter()
        {
            Filenames = new List<string>();
            TrackIds = new List<int>();
        }

        public List<string> Filenames { get; set; }

        public List<int> TrackIds { get; set; }

        public AddTracksToPlaylistType AddTracksType { get; set; }

        public int InsertIndex { get; set; }
    }

    public enum AddTracksToPlaylistType
    {
        None,
        Now,
        Next,
        End,
        InsertAtIndex
    }

}
