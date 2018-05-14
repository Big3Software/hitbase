// IMAPI2 Reference
// 
// http://msdn.microsoft.com/en-us/library/aa366450(v=VS.85).aspx
// WPF drag and drop
// http://wpftutorial.net/DragAndDrop.html?showallcomments
// WPF Drag and drop Adorner
// http://codeblitz.wordpress.com/2009/06/17/wpf-drag-drop-adorner/

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IMAPI2.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Management;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls.DragDrop;
using System.Threading;
using Big3.Hitbase.SoundEngine;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Win32;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for BurnMedium.xaml
    /// </summary>
    public partial class BurnMedium : Window, IDropTarget
    {
        private const string ClientName = "BurnMedia";
        
        Int64 _totalDiscSize;

        private bool _isBurning;
        private bool _isFormatting;
        private IMAPI_BURN_VERIFICATION_LEVEL _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;
        private bool _closeMedia;
        private bool _ejectMedia;
        private long _BurnSize = 0;
        private string _discLabel = "";
        private int _BurnType = 0;
        Brush _orgProgressBrush;
        private bool _BurnEndPlaySound;
        private string _BurnEndSoundFile;
        private bool _enableGap;
        private bool _formatNeeded = false;

        //private AutoResetEvent formatDone; // event wenn format fertig
        private IDiscRecorder2 _currentdiscRecorder;
        
        private BurnData _burnData = new BurnData();

        private ObservableCollection<MsftDiscRecorder2> allcdDrives = new ObservableCollection<MsftDiscRecorder2>();
        private ObservableCollection<WriteSpeed> allBurnSpeeds = new ObservableCollection<WriteSpeed>();

        private BackgroundWorker bwBurnDisc = new BackgroundWorker();
        private BackgroundWorker bwFormatDisc = new BackgroundWorker();

        private Playlist playlist;

        public ObservableCollection<MsftDiscRecorder2> cboitems
        {
            get
            {
                return allcdDrives;
            }
            set
            {
                allcdDrives = value;
            }
        }

        public ObservableCollection<WriteSpeed> cboSpeedItems
        {
            get
            {
                return allBurnSpeeds;
            }
            set
            {
                allBurnSpeeds = value;
            }
        }


        /// <summary>
        /// Treeview for select directory to burn
        /// </summary>
        private ObservableCollection<BurnTreeList> allburnitems = new ObservableCollection<BurnTreeList>();

        public ObservableCollection<BurnTreeList> AllBurnItems
        {
            get
            {
                return allburnitems;
            }
            set
            {
                allburnitems = value;
            }
        }


        private ObservableCollection<BurnTreeList> selectdirectories = new ObservableCollection<BurnTreeList>();

        public ObservableCollection<BurnTreeList> SelectDirectories
        {
            get
            {
                return selectdirectories;
            }
            set
            {
                selectdirectories = value;
            }
        }

        public BurnMedium(DataBase dataBase, Playlist playlist)
        {
            DataContext = this;

            this.playlist = playlist;

            InitializeComponent();            

            _orgProgressBrush = progressMediaSize.Foreground;

            bwBurnDisc.WorkerReportsProgress = true;
            bwBurnDisc.WorkerSupportsCancellation = true;
            bwBurnDisc.DoWork += new DoWorkEventHandler(backgroundBurnWorker_DoWork);
            bwBurnDisc.ProgressChanged += new ProgressChangedEventHandler(backgroundBurnWorker_ProgressChanged);

            bwBurnDisc.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundBurnWorker_RunWorkerCompleted);

            bwFormatDisc.WorkerReportsProgress = true;
            bwFormatDisc.WorkerSupportsCancellation = true;
            bwFormatDisc.DoWork += new DoWorkEventHandler(bwFormatDisc_DoWork);
            bwFormatDisc.ProgressChanged += new ProgressChangedEventHandler(bwFormatDisc_ProgressChanged);

            bwFormatDisc.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwFormatDisc_RunWorkerCompleted);


            foreach (string monitorDir in dataBase.Master.MonitoredDirectories)
            {
                BurnTreeList moniDir = new BurnTreeList();
                moniDir.Name = monitorDir.Substring(monitorDir.LastIndexOf('\\') + 1);
                moniDir.FullPath = monitorDir;
                moniDir.Items.Add(new BurnTreeList());
                SelectDirectories.Add(moniDir);
            }

            BurnTreeList myComputer = new BurnTreeList();
            myComputer.Name = "Computer";
            myComputer.Items.Add(new BurnTreeList());
            SelectDirectories.Add(myComputer);

            BurnGridDirs.Width = new GridLength((double)Settings.Current.BurnGridDirsSize, GridUnitType.Pixel);
            SelectGridDirs.Width = new GridLength((double)Settings.Current.SelectGridDirsSize, GridUnitType.Pixel);

            if (dataGridSelectDirectoryFiles.Columns.Count == 4)
            {
                    dataGridSelectDirectoryFiles.Columns[0].Width = new DataGridLength(Settings.Current.BurnSelectFilesColumn1, DataGridLengthUnitType.Pixel);
                    dataGridSelectDirectoryFiles.Columns[1].Width = new DataGridLength(Settings.Current.BurnSelectFilesColumn2, DataGridLengthUnitType.Pixel);
                    dataGridSelectDirectoryFiles.Columns[2].Width = new DataGridLength(Settings.Current.BurnSelectFilesColumn3, DataGridLengthUnitType.Pixel);
                    dataGridSelectDirectoryFiles.Columns[3].Width = new DataGridLength(Settings.Current.BurnSelectFilesColumn4, DataGridLengthUnitType.Pixel);
            }

            if (comboBoxType.SelectedIndex == -1)
            {
                dataGridBurnFileList.Columns[0].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn1, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[1].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn2, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[2].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn3, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[3].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn4, DataGridLengthUnitType.Pixel);
                comboBoxType.SelectedIndex = 0;
            }

            if (comboBoxType.Items.Count > Settings.Current.BurnTypeMedium)
                comboBoxType.SelectedIndex = Settings.Current.BurnTypeMedium;

            if (Settings.Current.BurnTypeMedium == 1) // Audio CD
            {
                textBoxLabel.IsEnabled = false;
                buttonToggleGAP.Visibility = Visibility.Visible;
                if (dataGridBurnFileList.Columns.Count == 4)
                {
                    dataGridBurnFileList.Columns[0].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn1, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[1].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn2, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[2].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn3, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[3].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn4, DataGridLengthUnitType.Pixel);
                }
                buttonNew.IsEnabled = false;
            }
            else // Data CD
            {
                textBoxLabel.IsEnabled = true;
                buttonToggleGAP.Visibility = Visibility.Hidden;
                if (dataGridBurnFileList.Columns.Count == 4)
                {
                    dataGridBurnFileList.Columns[0].Width = new DataGridLength(Settings.Current.BurnFilesColumn1, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[1].Width = new DataGridLength(Settings.Current.BurnFilesColumn2, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[2].Width = new DataGridLength(Settings.Current.BurnFilesColumn3, DataGridLengthUnitType.Pixel);
                    dataGridBurnFileList.Columns[3].Width = new DataGridLength(Settings.Current.BurnFilesColumn4, DataGridLengthUnitType.Pixel);
                }
                buttonNew.IsEnabled = true;
            }

            if (comboBoxType.Items.Count > Settings.Current.BurnTypeMedium)
                comboBoxType.SelectedIndex = Settings.Current.BurnTypeMedium;
            if (comboBoxVerify.Items.Count > Settings.Current.BurnVerifyType)
                comboBoxVerify.SelectedIndex = Settings.Current.BurnVerifyType;

            this.Width = Settings.Current.BurnDialogSizeX;
            this.Height = Settings.Current.BurnDialogSizeY;
            this.Top = Settings.Current.BurnDialogTop;
            this.Left = Settings.Current.BurnDialogLeft;            
            BurnGridDirs.Width = new GridLength((double)Settings.Current.BurnGridDirsSize, GridUnitType.Pixel);
            SelectGridDirs.Width = new GridLength((double)Settings.Current.SelectGridDirsSize, GridUnitType.Pixel);
            
            _ejectMedia = Settings.Current.BurnEjectMedia;
            buttonToggleEject.IsChecked = Settings.Current.BurnEjectMedia;

            _enableGap = Settings.Current.BurnGapTracks;
            buttonToggleGAP.IsChecked = Settings.Current.BurnGapTracks;
            buttonBurn.IsEnabled = false;
            _BurnEndPlaySound = Settings.Current.BurnEndPlaySound;
            buttonTogglePlaySound.IsChecked = Settings.Current.BurnEndPlaySound;
            
            if (!string.IsNullOrEmpty(Settings.Current.BurnEndSoundFile) && File.Exists(Settings.Current.BurnEndSoundFile))
            {
                _BurnEndSoundFile = Settings.Current.BurnEndSoundFile;
                buttonTogglePlaySound.ToolTip = "Sound Datei: " + _BurnEndSoundFile;
            }
            else
            {
                _BurnEndPlaySound = false;
                buttonTogglePlaySound.IsChecked = false;
                buttonTogglePlaySound.ToolTip = "Sound Datei: Keine ausgewählt!";
            }
            
            buttonDel.IsEnabled = false;
            
            progressMediaSize.Maximum = 100;
            progressMediaSize.Value = 0;
        }

        /// <summary>
        /// Überprüft, ob der Dateiname schon in der aktuellen BurnTreeList vorhanden ist
        /// </summary>
        /// <param name="btl"></param>
        /// <returns></returns>
        private bool CheckBurnName(ObservableCollection<BurnTreeList>currentBTL, string checkName)
        {
            foreach (BurnTreeList btl in currentBTL)
            {
                if (btl.Name == checkName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Überprüft, ob der Dateiname schon in der aktuellen BurnTreeList vorhanden ist
        /// </summary>
        /// <param name="btl"></param>
        /// <returns></returns>
        private bool CheckBurnFileName(ObservableCollection<DirectoryContent> currentBTL, string checkName)
        {
            foreach (DirectoryContent btl in currentBTL)
            {
                if (btl.Name == checkName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Hinzufügen von Dateien zur Brennliste
        /// </summary>
        /// <param name="directoryPath"></param>
        public long AddDirectoryItem(string directoryPath, BurnTreeList currentTree, bool Recursive=false)
        {
            long countBytes = 0;

            if (!Directory.Exists(directoryPath))
            {
                if (File.Exists(directoryPath))
                {
                    if (currentTree.FileItems == null)
                        currentTree.FileItems = new ObservableCollection<DirectoryContent>();

                    // TODO!!!!!!! GUN: Prüfen, ob Datei schon vorhanden ist.

                    FileInfo fileinfo = new FileInfo(directoryPath);

                    DirectoryContent newDirItem = new DirectoryContent();
                    newDirItem.Name = fileinfo.Name;
                    newDirItem.FullPath = fileinfo.FullName;
                    newDirItem.IsDirectory = false;
                    newDirItem.LastModified = fileinfo.LastWriteTime;
                    newDirItem.SourcePath = fileinfo.FullName;
                    newDirItem.Size = fileinfo.Length;
                    //
                    // Get some File Info
                    //
                    SHFILEINFO shinfo = new SHFILEINFO();

                    IntPtr hImg = Win32.SHGetFileInfo(directoryPath, 0, ref shinfo,
                        (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_SMALLICON | Win32.SHGFI_ICON);
                    newDirItem.ElementType = shinfo.szTypeName;
                    if (shinfo.hIcon != IntPtr.Zero)
                    {
                        //The icon is returned in the hIcon member of the shinfo struct
                        System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                        System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                        try
                        {
                            newDirItem.Image = ImageLoader.ConvertDrawingImageToWPFImage((System.Drawing.Image)
                                imageConverter.ConvertTo(icon, typeof(System.Drawing.Image)));
                        }
                        catch (NotSupportedException)
                        {
                        }

                        Win32.DestroyIcon(shinfo.hIcon);
                    }

                    int nCopy = 2;
                    string newItemOrg = newDirItem.Name;
                    // Ist der Dateiname schon da?
                    if (CheckBurnFileName(currentTree.FileItems, newDirItem.Name) == true)
                    {
                        do
                        {
                            newDirItem.Name = newItemOrg + " - (" + nCopy.ToString() + ")";
                            nCopy++;
                        } while (CheckBurnFileName(currentTree.FileItems, newDirItem.Name) == true);
                    }

                    if (_BurnType == 1) // Audio
                    {
                        string tempPath;

                        tempPath = ConvertWaveToMp3(newDirItem.FullPath);

                        if (!string.IsNullOrEmpty(tempPath))
                        {
                            newDirItem.FullPath = tempPath;

                            FileInfo fileinfo2 = new FileInfo(tempPath);
                            newDirItem.Size = fileinfo2.Length;
                            countBytes = countBytes + fileinfo2.Length;
                        }
                        else
                            return 0;
                    }
                    else
                    {
                        countBytes = countBytes + fileinfo.Length;
                    }

                    currentTree.FileItems.Add(newDirItem);
                }

                return countBytes;
                //throw new FileNotFoundException("The directory added to DirectoryItem was not found!", directoryPath);
            }
            
            BurnTreeList dt;
            BurnTreeList newItem = new BurnTreeList();
            try
            {
                //if (treeViewBurnDirectoryList.SelectedItem == null)
                //{
                //    dt = currentTree.Last<BurnTreeList>();
                //}
                //else
                //{
                //    dt = (BurnTreeList)treeViewBurnDirectoryList.SelectedItem;
                //}
                DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

                newItem.FullPath = directoryPath;
                newItem.Name = directoryPath.Substring(directoryPath.LastIndexOf('\\') + 1);
                newItem.LastModified = dirInfo.LastWriteTime;
                SHFILEINFO shinfo = new SHFILEINFO();

                IntPtr hImg = Win32.SHGetFileInfo(directoryPath, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_SMALLICON | Win32.SHGFI_ICON);
                newItem.ElementType = shinfo.szTypeName;
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    //The icon is returned in the hIcon member of the shinfo struct
                    System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                    System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                    try
                    {
                        newItem.Image = ImageLoader.ConvertDrawingImageToWPFImage((System.Drawing.Image)
                            imageConverter.ConvertTo(icon, typeof(System.Drawing.Image)));
                    }
                    catch (NotSupportedException)
                    {
                    }

                    Win32.DestroyIcon(shinfo.hIcon);
                }

                newItem.IsDirectory = true;

                int nCopy = 2;
                string newItemOrg = newItem.Name;
                
                // Ist der Dateiname schon da? Dann ne Nummer hinten dran
                if (CheckBurnName(currentTree.Items, newItem.Name) == true)
                {
                    do
                    {
                        newItem.Name = newItemOrg + " - (" + nCopy.ToString() + ")";
                        nCopy++;
                    } while (CheckBurnName(currentTree.Items, newItem.Name) == true);
                }

                // Not for audio CDs...
                if (_BurnType != 1)
                {
                    currentTree.Items.Add(newItem);
                    dt = currentTree.Items.Last<BurnTreeList>();
                }
                else
                {
                    if (currentTree.FileItems == null)
                        currentTree.FileItems = new ObservableCollection<DirectoryContent>();

                    dt = currentTree;
                }
            }
            catch (Exception ex)
            {
                return countBytes;
            }
            //displayName = fileInfo.Name;
            //
            // Get all the subdirectories
            //
            string[] directories = Directory.GetDirectories(directoryPath);
            Array.Sort(directories);
            foreach (string directory in directories)
            {
                //dt = AllBurnItems.Last<BurnTreeList>();
                //currentTree.Items.Last<BurnTreeList>()

                //dt.Items.Add(new BurnTreeList { FullPath = directory, Name = directory.Substring(directory.LastIndexOf('\\') + 1) });

                if (Recursive == true)
                {
                    //BurnTreeList oldtl = currentTree.Items.Last<BurnTreeList>();
                    long bytesdir;
                    if (_BurnType != 1)
                        bytesdir = AddDirectoryItem(directory, currentTree.Items.Last<BurnTreeList>(), true);
                    else
                        bytesdir = AddDirectoryItem(directory, currentTree, true);
                        
                    //oldtl.Parent.Size = oldtl.Parent.Size + bytesdir;
                    //dt.Size = dt.Size + bytesdir;
                    
                    countBytes = countBytes + bytesdir;
                }
            }

            //            
            // Get all the files in the directory
            //
            string[] files = Directory.GetFiles(directoryPath);
            //Array.Sort(files);
            if (dt.FileItems == null)
                dt.FileItems = new ObservableCollection<DirectoryContent>();
            
            foreach (string file in files)
            {
                FileInfo fileinfo = new FileInfo(file);
                //BurnTreeList dt = currentTree.Items.Last<BurnTreeList>();
                //dt.Items.Add(new BurnTreeList { Name = file, FileSize = fileinfo.Length, LastModified = fileinfo.LastWriteTime });
                //SelectDirectories.Add(new BurnTreeList { Name = file, FileSize = fileinfo.Length, LastModified = fileinfo.LastWriteTime });
                DirectoryContent newDirItem = new DirectoryContent();
                newDirItem.Name = fileinfo.Name;
                newDirItem.FullPath = fileinfo.FullName;
                newDirItem.IsDirectory = false;
                newDirItem.LastModified = fileinfo.LastWriteTime;
                newDirItem.SourcePath = fileinfo.FullName;
                newDirItem.Size = fileinfo.Length;
                //
                // Get some File Info
                //
                SHFILEINFO shinfo = new SHFILEINFO();

                IntPtr hImg = Win32.SHGetFileInfo(fileinfo.FullName, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_SMALLICON | Win32.SHGFI_ICON);
                newDirItem.ElementType = shinfo.szTypeName;
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    //The icon is returned in the hIcon member of the shinfo struct
                    System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                    System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                    try
                    {
                        newDirItem.Image = ImageLoader.ConvertDrawingImageToWPFImage((System.Drawing.Image)
                            imageConverter.ConvertTo(icon, typeof(System.Drawing.Image)));
                    }
                    catch (NotSupportedException)
                    {
                    }

                    Win32.DestroyIcon(shinfo.hIcon);
                }

                if (_BurnType == 1) // Audio
                {
                    int nCopy=2;
                    string tempPath;

                    tempPath = ConvertWaveToMp3(newDirItem.FullPath);

                    if (string.IsNullOrEmpty(tempPath))
                        continue;
                    string newItemOrg = newDirItem.Name;

                    // Ist der Dateiname schon da?
                    if (CheckBurnFileName(currentTree.FileItems, newDirItem.Name) == true)
                    {
                        do
                        {
                            newDirItem.Name = newItemOrg + " - (" + nCopy.ToString() + ")";
                            nCopy++;
                        } while (CheckBurnFileName(currentTree.FileItems, newDirItem.Name) == true);
                    }


                    newDirItem.FullPath = tempPath;

                    FileInfo fileinfo2 = new FileInfo(tempPath);
                    newDirItem.Size = fileinfo2.Length;
                    countBytes = countBytes + fileinfo2.Length;
                }
                else
                {
                    countBytes = countBytes + fileinfo.Length;
                }

                dt.FileItems.Add(newDirItem);
            }

            dt.Size = dt.Size + countBytes;
            //currentTree.FileSize = GetSizeInKB(currentTree.Size);
            return countBytes;
        }

        private void comboBurner_Loaded(object sender, RoutedEventArgs e)
        {
            //
            // Determine the current recording devices
            //
            MsftDiscMaster2 discMaster = null;
            try
            {
                discMaster = new MsftDiscMaster2();

                if (!discMaster.IsSupportedEnvironment)
                    return;
                foreach (string uniqueRecorderId in discMaster)
                {
                    var discRecorder2 = new MsftDiscRecorder2();
                    discRecorder2.InitializeDiscRecorder(uniqueRecorderId);

                    //comboBurner.Items.Add(getDeviceName(discRecorder2));
                    IDiscFormat2Data discFormatData = new MsftDiscFormat2Data();

                    if (discFormatData.IsRecorderSupported(discRecorder2))
                    {
                        allcdDrives.Add(discRecorder2);
                    }
                    
                }
                if (comboBurner.Items.Count > 0)
                {
                    if (comboBurner.Items.Count > Settings.Current.BurnSelectedBurner)
                        comboBurner.SelectedIndex = Settings.Current.BurnSelectedBurner;
                    else
                        comboBurner.SelectedIndex = 0;
                }
            }
            catch (COMException ex)
            {
                System.Windows.MessageBox.Show(ex.Message,
                    string.Format("Error:{0} - Please install IMAPI2", ex.ErrorCode),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            finally
            {
                if (discMaster != null)
                {
                    Marshal.ReleaseComObject(discMaster);
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //
            // Release the disc recorder items
            //

            foreach (MsftDiscRecorder2 discRecorder2 in comboBurner.Items)
            {
                if (discRecorder2 != null)
                {
                    Marshal.ReleaseComObject(discRecorder2);
                }
            }
        }
        /// <summary>
        /// converts an IMAPI_PROFILE_TYPE to it's string
        /// </summary>
        /// <param name="profileType"></param>
        /// <returns></returns>
        static string GetProfileTypeString(IMAPI_PROFILE_TYPE profileType)
        {
            switch (profileType)
            {
                default:
                    return string.Empty;

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_CD_RECORDABLE:
                    return "CD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_CD_REWRITABLE:
                    return "CD-RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVDROM:
                    return "DVD ROM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_RECORDABLE:
                    return "DVD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_RAM:
                    return "DVD-RAM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_R:
                    return "DVD+R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_RW:
                    return "DVD+RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_R_DUAL:
                    return "DVD+R Dual Layer";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_REWRITABLE:
                    return "DVD-RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_RW_SEQUENTIAL:
                    return "DVD-RW Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_R_DUAL_SEQUENTIAL:
                    return "DVD-R DL Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_R_DUAL_LAYER_JUMP:
                    return "DVD-R Dual Layer";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_RW_DUAL:
                    return "DVD+RW DL";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_ROM:
                    return "HD DVD-ROM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_RECORDABLE:
                    return "HD DVD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_RAM:
                    return "HD DVD-RAM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_ROM:
                    return "Blu-ray DVD (BD-ROM)";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_R_SEQUENTIAL:
                    return "Blu-ray media Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_R_RANDOM_RECORDING:
                    return "Blu-ray media";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_REWRITABLE:
                    return "Blu-ray Rewritable media";
            }
        }

        private void comboBurner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBurner.SelectedIndex == -1)
            {
                return;
            }

            var discRecorder =
                (IDiscRecorder2)comboBurner.Items[comboBurner.SelectedIndex];

            if (AllBurnItems.Count == 0)
            {
                AllBurnItems.Add(new BurnTreeList { 
                    Name = discRecorder.VolumePathNames[0].ToString(), 
                    FullPath = discRecorder.VolumePathNames[0].ToString(),
                    IsDirectory = true
                });
                
            }
            else
            {
                AllBurnItems[0].Name = discRecorder.VolumePathNames[0].ToString();
                AllBurnItems[0].FullPath = discRecorder.VolumePathNames[0].ToString();
            }
            
            CheckMedia();

            if (textBoxLabel.Text.Length > 0)
                AllBurnItems[0].Name = discRecorder.VolumePathNames[0].ToString() + " - [" + textBoxLabel.Text + "]";
            else
                AllBurnItems[0].Name = discRecorder.VolumePathNames[0].ToString();

            string driveletter = discRecorder.VolumePathNames[0].ToString();
            InitCDEngine(driveletter[0]);

            //((BurnTreeList)(treeViewBurnDirectoryList.Items[0])).IsSelected = true;

            foreach (int feat in discRecorder.CurrentFeaturePages)
            {
                if (feat == 0x23)
                {
                    // RW Medium found

                    //textBlockMedium.Text = "RW" + discRecorder.VolumePathNames[0];

                }
                //else
                    //textBlockMedium.Text = "Kein RW" + discRecorder.VolumePathNames[0];
            }

            //
            // Verify recorder is supported
            //
            IDiscFormat2Data discFormatData = null;
            try
            {
                discFormatData = new MsftDiscFormat2Data();

                StringBuilder supportedMediaTypes = new StringBuilder();
                
                
                int nCount = 0;
                foreach (IMAPI_PROFILE_TYPE profileType in discRecorder.SupportedProfiles)
                {
                    string profileName = GetProfileTypeString(profileType);

                    if (string.IsNullOrEmpty(profileName))
                        continue;

                    if (supportedMediaTypes.Length > 0)
                    {
                        if (nCount % 3 != 0)
                        {
                            supportedMediaTypes.Append(", ");
                        }
                        else
                        {

                            supportedMediaTypes.Append(",\r\n");
                        }
                    }
                    else
                    {
                        supportedMediaTypes.Append("Folgende Brenn-Formate werden unterstützt:\r\n");
                    }

                    supportedMediaTypes.Append(profileName);
                    nCount++;
                }

                System.Windows.Controls.ToolTip newTip = new System.Windows.Controls.ToolTip();
                ToolTipService.SetShowDuration(comboBurner, 60000);
                //newTip.Background = new LinearGradientBrush(Color.FromRgb(100, 120, 130), Color.FromRgb(200, 220, 230), 0); //Set border color 
                //newTip.BorderBrush = Brushes.Red;
                //newTip.BorderThickness = new Thickness(4);
                //newTip.HasDropShadow = true;
                //newTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; 
                //ObjectToDisplayToolTipOn.ToolTip = objTooltip;
                newTip.Content = supportedMediaTypes.ToString();

                comboBurner.ToolTip = newTip;
            }
            catch (COMException)
            {
                comboBurner.ToolTip = "Unterstützte Brenn Formate konnten nicht ermittelt werden.";
            }
            finally
            {
                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Updates the capacity progressbar
        /// </summary>
        private void UpdateCapacity()
        {
            //
            // Get the text for the Max Size
            //{#FF01D328}
            if (_totalDiscSize == 0)
            {
                textBlockTotalSize.Text = "0MB";
                return;
            }
            

            textBlockTotalSize.Text = _totalDiscSize < 1000000000 ?
                string.Format("{0}MB", _totalDiscSize / 1000000) :
                string.Format("{0:F2}GB", (float)_totalDiscSize / 1000000000.0);

            if (_BurnSize == 0)
            {
                progressMediaSize.Value = 0;
                //progressMediaSize.ForeColor = SystemColors.Highlight;
                buttonBurn.IsEnabled = false;
            }
            else
            {
                long percent = (long)((_BurnSize * 100) / _totalDiscSize);
                if (percent > 100)
                {
                    progressMediaSize.Value = 100;
                    progressMediaSize.Foreground = Brushes.Red;
                    buttonBurn.IsEnabled = false;
                }
                else
                {
                    progressMediaSize.Value = percent;
                    //progressMediaSize.ForeColor = SystemColors.Highlight;
                    progressMediaSize.Foreground = _orgProgressBrush;
                    if (_BurnSize > 0)
                        buttonBurn.IsEnabled = true;
                }
            }
         }
 
         /// <summary>
        /// 
        /// </summary>
        private void CheckMedia()
        {
            if (comboBurner.SelectedIndex == -1)
            {
                _totalDiscSize = 0;
                return;
            }
            
            string errortext;

            var discRecorder = (IDiscRecorder2)comboBurner.Items[comboBurner.SelectedIndex];

            errortext = "Medium nicht unterstützt!";

            foreach (int feat in discRecorder.CurrentFeaturePages)
            {
                if (feat == 0x1f)
                {
                    errortext = "Disc format";
                }
            }
            

            MsftFileSystemImage fileSystemImage = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                //
                // Create and initialize the IDiscFormat2Data
                //
                discFormatData = new MsftDiscFormat2Data();
                discFormatData.Recorder = discRecorder;

                cboSpeedItems.Clear();
                try
                {
                    if (discFormatData.WriteProtectStatus == 0)
                    {
                        // write is OK
                    }
                }
                catch (COMException exception)
                {
                    // Kein Mediuzm im Laufwerk?
                    textBlockCurrentMedium.Text = "Laufwerk leer";
                    _totalDiscSize = 0;
                    buttonBurn.IsEnabled = false;
                    if (discFormatData != null)
                    {
                        Marshal.ReleaseComObject(discFormatData);
                    }
                    return;
                }

                IMAPI_MEDIA_PHYSICAL_TYPE mediaType2 = discFormatData.CurrentPhysicalMediaType;
                //IMAPI_MEDIA_WRITE_PROTECT_STATE mediaType22 = discFormatData.WriteProtectStatus;

                discFormatData.Recorder = discRecorder;
                IMAPI_MEDIA_PHYSICAL_TYPE mediaType = discFormatData.CurrentPhysicalMediaType;

                //
                // Get the media type in the recorder
                //
                _formatNeeded = false;

                textBlockCurrentMedium.Text = GetMediaTypeString(mediaType);

                // Check for Read/Write Media or Write once
                if (mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW_DUALLAYER ||
                    mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW ||
                    mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW ||
                    mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW ||
                    mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDRE ||
                    mediaType2 == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM)
                {
                    // Rewritable Media detected

                    if ((discFormatData.CurrentMediaStatus & IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_ERASE_REQUIRED) > 0 || discFormatData.MediaHeuristicallyBlank == false)
                    {
                        textBlockCurrentMedium.Text = textBlockCurrentMedium.Text + " - Enthält bereits Daten!";
                        _formatNeeded = true;
                        if (_BurnSize > 0)
                            buttonBurn.IsEnabled = true;
                    }
                    else
                    {
                        textBlockCurrentMedium.Text = textBlockCurrentMedium.Text + " - Leer";
                        if (_BurnSize > 0)
                            buttonBurn.IsEnabled = true;
                    }
                }
                else
                {
                    // Write once media detected

                    if ((discFormatData.CurrentMediaStatus & IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_BLANK) > 0)
                    {
                        textBlockCurrentMedium.Text = textBlockCurrentMedium.Text + " - Leer";
                        if (_BurnSize > 0)
                            buttonBurn.IsEnabled = true;
                    }
                    else
                    {
                        textBlockCurrentMedium.Text = textBlockCurrentMedium.Text + " - bereits beschrieben!";
                        _totalDiscSize = 0;
                        if (discFormatData != null)
                        {
                            Marshal.ReleaseComObject(discFormatData);
                        }
                        buttonBurn.IsEnabled = false;
                        return;
                    }
                }

                WriteSpeed wsm = new WriteSpeed();
                wsm.SectorsPerSecond = -1;
                wsm.SpeedKB = "";
                wsm.SpeedX = "Maximal";
                cboSpeedItems.Add(wsm);
                comboBoxWriteSpeed.SelectedIndex = 0;
                // Get write speed get_SupportedWriteSpeedDescriptors 
                foreach (int writespeed in discFormatData.SupportedWriteSpeeds)
                {
                    int mbprosek = (writespeed * (int)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTOR_SIZE) / 1048576;
                    double mbs = ((double)writespeed * (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTOR_SIZE) / (double)1048576;
                    double xfach;

                    // Die Angaben für Write-Speed
                    switch (mediaType)
                    {
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDR:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDROM:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW:
                            xfach = (double)writespeed / (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTORS_PER_SECOND_AT_1X_CD;
                            break;
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR_DUALLAYER:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR_DUALLAYER:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW_DUALLAYER:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDROM:
                            xfach = (double)writespeed / (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTORS_PER_SECOND_AT_1X_DVD;
                            break;
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDR:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDRAM:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDROM:
                            xfach = (double)writespeed / (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTORS_PER_SECOND_AT_1X_HD_DVD;
                            break;
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDR:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDRE:
                        case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDROM:
                            xfach = (double)writespeed / (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTORS_PER_SECOND_AT_1X_BD;
                            break;
                        default:
                            xfach = (double)writespeed / (double)IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_SECTORS_PER_SECOND_AT_1X_CD;
                            break;
                    }

                    xfach = Math.Round(xfach, 1);
                    mbs = Math.Round(mbs, 1);
                    if (xfach.ToString().Length > 2)
                    {
                        if (xfach.ToString().Right(2) == ",9" || xfach.ToString().Right(2) == ".9")
                        {
                            xfach = Math.Round(xfach, 0);
                        }
                        if (xfach.ToString().Right(2) == ",1" || xfach.ToString().Right(2) == ".1")
                        {
                            xfach = Math.Round(xfach, 0);
                        }
                    }
                    WriteSpeed ws = new WriteSpeed();
                    ws.SectorsPerSecond = writespeed;
                    ws.SpeedKB = "(" + mbs.ToString() + " MB/s)";
                    ws.SpeedX = xfach.ToString() + "x";
                    cboSpeedItems.Add(ws);
                }

                //
                // Create a file system and select the media type
                //
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaultsForMediaType(mediaType);

                //
                // See if there are other recorded sessions on the disc
                //
                //if (!discFormatData.MediaHeuristicallyBlank)
                //{
                //    fileSystemImage.MultisessionInterfaces = discFormatData.MultisessionInterfaces;
                //    fileSystemImage.ImportFileSystem();
                //}

                Int64 freeMediaBlocks = fileSystemImage.FreeMediaBlocks;
                _totalDiscSize = 2048 * freeMediaBlocks;

            }
            catch (COMException exception)
            {
                System.Windows.MessageBox.Show(this, exception.Message, "Detect Media Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }

                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }


            UpdateCapacity();
        }

        private static string GetMediaTypeString(IMAPI_MEDIA_PHYSICAL_TYPE mediaType)
        {
            switch (mediaType)
            {
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_UNKNOWN:
                default:
                    return "Unbekanntes Medium";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDROM:
                    return "CD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDR:
                    return "CD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW:
                    return "CD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDROM:
                    return "DVD ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM:
                    return "DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR:
                    return "DVD+R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW:
                    return "DVD+RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR_DUALLAYER:
                    return "DVD+R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR:
                    return "DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW:
                    return "DVD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR_DUALLAYER:
                    return "DVD-R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DISK:
                    return "Random-Access Writes";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW_DUALLAYER:
                    return "DVD+RW DL";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDROM:
                    return "HD DVD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDR:
                    return "HD DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDRAM:
                    return "HD DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDROM:
                    return "Blu-ray DVD (BD-ROM)";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDR:
                    return "Blu-ray media";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDRE:
                    return "Blu-ray Rewritable media";
            }
        }

        /// <summary>
        /// Worker thread that Formats the Disc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwFormatDisc_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Erase discFormatErase = null;

            try
            {
                //
                // Create and initialize the IDiscRecorder2
                //
                discRecorder = new MsftDiscRecorder2();
                var activeDiscRecorder = (string)e.Argument;
                discRecorder.InitializeDiscRecorder(activeDiscRecorder);

                //
                // Create the IDiscFormat2Erase and set properties
                //
                discFormatErase = new MsftDiscFormat2Erase
                    {
                        Recorder = discRecorder,
                        ClientName = ClientName,
                        FullErase = false
                    };

                //
                // Setup the Update progress event handler
                //
                discFormatErase.Update += discFormatErase_Update;

                //
                // Erase the media here
                //
                try
                {
                    discFormatErase.EraseMedia();
                    
                    e.Result = 0;
                }
                catch (COMException ex)
                {
                    e.Result = ex.ErrorCode;
                    System.Windows.MessageBox.Show(ex.Message, "IDiscFormat2.EraseMedia failed",
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                }

                //
                // Remove the Update progress event handler
                //
                discFormatErase.Update -= discFormatErase_Update;

                //
                // Eject the media 
                //
                //discRecorder.EjectMedia();

            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                System.Windows.MessageBox.Show(exception.Message);
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatErase != null)
                {
                    Marshal.ReleaseComObject(discFormatErase);
                }

                //formatDone.Set();
            }
        }
 
        /// Event Handler for the Erase Progress Updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedSeconds"></param>
        /// <param name="estimatedTotalSeconds"></param>
        void discFormatErase_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            var percent = elapsedSeconds * 100 / estimatedTotalSeconds;
            //
            // Report back to the UI
            //
            bwFormatDisc.ReportProgress(percent);
        }

        private void bwFormatDisc_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBoxCurrentBurnAction.Text = string.Format("Formatiere {0}%...", e.ProgressPercentage);
            progressBarBurnStatus.Value = e.ProgressPercentage;
            TaskbarItemInfo.ProgressValue = e.ProgressPercentage;
        }

        private void bwFormatDisc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBoxCurrentBurnAction.Text = (int)e.Result == 0 ? "Format Medium fertig!" : "Fehler beim Format Medium!";

            progressBarBurnStatus.Value = 0;
            TaskbarItemInfo.ProgressValue = 0;
            _isFormatting = false;

            // Jetzt kann gebrannt werden... Disc ist formatiert.
            bwBurnDisc.RunWorkerAsync(_burnData);
        }

        // Raise for new media
        ManagementEventWatcher watchMediaDrive = null;

        private void InitCDEngine(char driveLetter)
        {
            //WMIEvent we = new WMIEvent();

            WqlEventQuery q;
            ManagementOperationObserver observer = new ManagementOperationObserver();

            if (watchMediaDrive != null)
            {
                watchMediaDrive.Stop();
            }
            // Bind to local machine
            ConnectionOptions opt = new ConnectionOptions();
            opt.EnablePrivileges = true; //sets required privilege
            ManagementScope scope =
                new ManagementScope("root\\CIMV2", opt);

            q = new WqlEventQuery();
            q.EventClassName = "__InstanceModificationEvent";
            q.WithinInterval = new TimeSpan(0, 0, 1);

            // DriveType - 5: CDROM
            string cond = string.Format(@"TargetInstance ISA 'Win32_LogicalDisk' and TargetInstance.DriveType = 5 AND TargetInstance.Name='{0}:'", driveLetter);
            q.Condition = cond;
            watchMediaDrive = new ManagementEventWatcher(scope, q);

            // register async. event handler
            watchMediaDrive.EventArrived += new EventArrivedEventHandler(CDREventArrived);
            watchMediaDrive.Start();
            /*TODO?            finally
                        {
                            watchMediaDrive.Stop();
                        }*/
        }

        /// <summary>
        /// Diese Event wird ausgelöst, wenn eine CD eingelegt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CDREventArrived(object sender, EventArrivedEventArgs e)
        {
            // Get the Event object and display it
            PropertyData pd = e.NewEvent.Properties["TargetInstance"];

            if (pd != null)
            {
                ManagementBaseObject mbo = pd.Value as ManagementBaseObject;

                foreach (PropertyData pd2 in mbo.Properties)
                {
                    if (pd2.Value != null)
                        System.Diagnostics.Debug.WriteLine(pd2.Name + ": " + pd2.Value.ToString());
                    else
                        System.Diagnostics.Debug.WriteLine(pd2.Name + ": <null>");

                }

                // if CD removed VolumeName == null
                //if (mbo.Properties["VolumeName"].Value != null)
                //{
                //    Console.WriteLine("CD has been inserted");
                //}
                //else
                //{
                //    Console.WriteLine("CD has been ejected");
                //}
               
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CheckMedia();
                }));

            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCurrentSelItem();
        }

        /// <summary>
        /// Hinzufügen der aktuellen Sekektion
        /// </summary>
        private void AddCurrentSelItem()
        {
            // allDir
            BurnTreeList dts=null;
            BurnTreeList dtb=null;
            DirectoryContent dcs=null;

            // Add only when focus on tree or data
            if (Keyboard.FocusedElement is TreeViewItem || Keyboard.FocusedElement is DataGridCell)
            {
                if (Keyboard.FocusedElement is TreeViewItem)
                {
                    dts = (BurnTreeList)treeViewSelectDirectory.SelectedItem;
                }
                else
                {
                    dcs = (DirectoryContent)dataGridSelectDirectoryFiles.SelectedItem;
                    //
                    //sts = (BurnTreeList)dataGridSelectDirectoryFiles.SelectedItem;
                }
            }
            else
            {
                return;
            }

            if (dts == null && dcs == null)
                return;

            if (treeViewBurnDirectoryList.SelectedItem != null)
            {
                dtb = (BurnTreeList)treeViewBurnDirectoryList.SelectedItem;
            }
            else
            {
                dtb = AllBurnItems.Last<BurnTreeList>();
            }

            if (Keyboard.FocusedElement is TreeViewItem)
            {

                _BurnSize += AddDirectoryItem(dts.FullPath, dtb, true);
            }
            if (Keyboard.FocusedElement is DataGridCell)
            {
                _BurnSize += AddDirectoryItem(dcs.FullPath, dtb, true);
            }
            UpdateBurnDirectory();
            UpdateDirectory();
            UpdateCapacity();
        }
        private class OldWindow : System.Windows.Forms.IWin32Window
        {        
            IntPtr _handle;        
            public OldWindow(IntPtr handle)    
            {        
                _handle = handle;    
            }       
            #region IWin32Window Members        
            IntPtr System.Windows.Forms.IWin32Window.Handle    
            {        
                get { return _handle; }    
            }        
            #endregion
        }

        private void buttonDel_Click(object sender, RoutedEventArgs e)
        {
            delItem();
        }

        /// <summary>
        /// Übernehmen der aktuellen Wiedergabeliste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetPlaylist_Click(object sender, RoutedEventArgs e)
        {

            foreach (PlaylistItem plitem in playlist)
            {
                // Brauch ich nicht zu checken, weil AddDirectoryItem prüft, ob die Datei vorhanden ist... Dateien die mit cd: beginnen gibt es nicht
                //string filename;
                //filename = plitem.Info.Filename.Substring(plitem.Info.Filename.LastIndexOf('\\') + 1);

                //if (filename.ToLower().StartsWith("cd:"))
                //    continue;

                BurnTreeList dtb = AllBurnItems.Last<BurnTreeList>();
                if ((BurnTreeList)treeViewBurnDirectoryList.SelectedItem != null)
                    dtb = (BurnTreeList)treeViewBurnDirectoryList.SelectedItem;

                _BurnSize += AddDirectoryItem(plitem.Info.Filename, dtb, true);
            }

            UpdateBurnDirectory();

            UpdateCapacity();
        }

        // Delete current selected item
        private void delItem()
        {
            // dataGridBurnFileList.IsFocused
            // treeViewBurnDirectoryList.IsFocused

            if (Keyboard.FocusedElement is TreeViewItem)
            {
                BurnTreeList dtParent = GetParentOfSelectedItem();

                _BurnSize -= ((BurnTreeList)treeViewBurnDirectoryList.SelectedItem).Size;

                // TODO - Update Size of complete medium

                // now delete Item
                if (dtParent != null)
                {
                    dtParent.Items.Remove((BurnTreeList)treeViewBurnDirectoryList.SelectedItem);
                }
                else
                {
                    // Root selected, delete all
                    if (((BurnTreeList)treeViewBurnDirectoryList.SelectedItem).Items != null)
                    {
                        _BurnSize = 0;
                        ((BurnTreeList)treeViewBurnDirectoryList.SelectedItem).Items.Clear();
                    }
                    if (((BurnTreeList)treeViewBurnDirectoryList.SelectedItem).FileItems != null)
                    {
                        ((BurnTreeList)treeViewBurnDirectoryList.SelectedItem).FileItems.Clear();
                    }
                }
            }

            if (Keyboard.FocusedElement is DataGridCell)
            {
                // TODO!!!!! GUN Element löschen!
                // Klappt nicht, wenn in Directory list nichts selektiert ist, wie z.B. bei Audio CDs oder wenn bei Daten CDs nichts links geklickt wird
                //DirectoryContent dts = (DirectoryContent)dataGridBurnFileList.SelectedItem;
                ObservableCollection<DirectoryContent> burnList = dataGridBurnFileList.ItemsSource as ObservableCollection<DirectoryContent>;

                BurnTreeList burnTreeList = treeViewBurnDirectoryList.SelectedItem as BurnTreeList;
                //treeViewBurnDirectoryList

                BurnTreeList foundTreeItem = null;

                for (int i = dataGridBurnFileList.SelectedItems.Count - 1; i >= 0; i--)
                {
                    DirectoryContent foundItem = null;

                    if (burnTreeList != null)
                    {
                        foreach (DirectoryContent dc in burnTreeList.FileItems)
                        {
                            if (dc.Name == ((DirectoryContent)dataGridBurnFileList.SelectedItems[i]).Name)
                            {
                                foundItem = dc;
                                break;
                            }
                        }
                    }
                    // Audio CD?
                    if (_BurnType == 1)
                    {
                        if (allburnitems.Count > 0)
                        {
                            foreach (DirectoryContent dcf in allburnitems[0].FileItems)
                            {
                                if (dcf.Name == ((DirectoryContent)dataGridBurnFileList.SelectedItems[i]).Name)
                                {
                                    _BurnSize -= dcf.Size;
                                    allburnitems[0].FileItems.Remove(dcf);
                                    break;
                                }
                            }
                        }
                    }
                    if (foundItem != null)
                    {
                        _BurnSize -= foundItem.Size;
                        burnTreeList.FileItems.Remove(foundItem);
                    }


                    foundTreeItem = null;
                    if (burnTreeList != null)
                    {
                        foreach (BurnTreeList tl in burnTreeList.Items)
                        {
                            if (tl.Name == ((DirectoryContent)dataGridBurnFileList.SelectedItems[i]).Name)
                            {
                                foundTreeItem = tl;
                                break;
                            }
                        }
                    }

                    if (foundTreeItem != null)
                    {
                        _BurnSize -= foundTreeItem.Size;
                        burnTreeList.Items.Remove(foundTreeItem);
                    }
                }
            }

            UpdateBurnDirectory();
            UpdateDirectory();

            UpdateCapacity();
        }

        private BurnTreeList GetParentOfSelectedItem()
        {
            TreeViewItem treeViewItemParent = Big3.Hitbase.Miscellaneous.VisualTreeExtensions.FindParent<TreeViewItem>(lastSelectedTreeViewItem);

            if (treeViewItemParent == null)
                return null;
            
            BurnTreeList dtParent = treeViewItemParent.DataContext as BurnTreeList;
            return dtParent;
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            BurnTreeList dt = treeViewBurnDirectoryList.SelectedItem as BurnTreeList;

            if (GetParentOfSelectedItem() == null)
                return;

            int index = GetParentOfSelectedItem().Items.IndexOf(dt);
            if (index > 0)
            {
                GetParentOfSelectedItem().Items.Move(index, index - 1);
            }
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            BurnTreeList dt = treeViewBurnDirectoryList.SelectedItem as BurnTreeList;

            if (GetParentOfSelectedItem() == null)
                return;

            int index = GetParentOfSelectedItem().Items.IndexOf(dt);
            if (index >= GetParentOfSelectedItem().Items.Count - 1)
            {

            }
            else
            {
                GetParentOfSelectedItem().Items.Move(index, index + 1);
            }
        }

        private void treeViewBurnDirectoryList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeViewBurnDirectoryList.SelectedItem != null)
            {
                buttonDel.IsEnabled = true;
            }
            else
            {
                buttonDel.IsEnabled = false;
            }
            UpdateBurnDirectory();
        }

        string GetSizeInKB (long filesize)
        {
            if (filesize == 0)
                return "0 KB";

            return String.Format("{0:#,###} KB", (filesize + 1023) / 1024);
        }

        /// <summary>
        /// Update select burn directory content view
        /// </summary>
        private void UpdateBurnDirectory()
        {
            BurnTreeList burnTreeList = treeViewBurnDirectoryList.SelectedItem as BurnTreeList;

            if (burnTreeList == null)
            {
                burnTreeList = AllBurnItems.Last<BurnTreeList>();
            }

            ObservableCollection<DirectoryContent> burnContent = new ObservableCollection<DirectoryContent>();

            if (burnTreeList != null)
            {
                dataGridBurnFileList.ItemsSource = burnContent;
            }

            //dataGridSelectDirectoryFiles.Items.Clear();
            try
            {
                //BurnTreeList burnInfo = new DirectoryInfo(burnTreeList.FullPath);

                foreach (BurnTreeList dir in burnTreeList.Items)
                {
                    DirectoryContent newItem = new DirectoryContent();
                    newItem.IsDirectory = true;
                    newItem.FullPath = dir.FullPath;
                    newItem.Name = dir.Name;
                    newItem.IsDirectory = true;
                    newItem.LastModified = dir.LastModified;
                    newItem.Size = dir.Size;
                    newItem.Image = dir.Image;

                    burnContent.Add(newItem);
                }
            }
            catch
            {
            }

            try
            {
                //DirectoryInfo dirInfo = new DirectoryInfo(BurnTreeList.FullPath);
                foreach (DirectoryContent dir in burnTreeList.FileItems)
                {
                    DirectoryContent newItem = new DirectoryContent();
                    newItem.Name = dir.Name;
                    newItem.FullPath = dir.FullPath;
                    newItem.Size = dir.Size;
                    newItem.IsDirectory = false;
                    newItem.LastModified = dir.LastModified;
                    newItem.SourcePath = dir.SourcePath;
                    newItem.ElementType = dir.ElementType;

                    newItem.Image = dir.Image;

                    burnContent.Add(newItem);
                }

            }
            catch (Exception ex)
            {
                return;
            }

        }

        /// <summary>
        /// Update select directory content view
        /// </summary>
        private void UpdateDirectory()
        {
            BurnTreeList BurnTreeList = treeViewSelectDirectory.SelectedItem as BurnTreeList;

            ObservableCollection<DirectoryContent> dirContent = new ObservableCollection<DirectoryContent>();

            if (BurnTreeList != null)
            {
                dataGridSelectDirectoryFiles.ItemsSource = dirContent;
            }

            //dataGridSelectDirectoryFiles.Items.Clear();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(BurnTreeList.FullPath);

                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    DirectoryContent newItem = new DirectoryContent();
                    newItem.IsDirectory = true;
                    newItem.FullPath = dir.FullName;
                    newItem.Name = dir.Name;
                    newItem.LastModified = dir.LastWriteTime;
                    //
                    // Get some File Info
                    //
                    SHFILEINFO shinfo = new SHFILEINFO();
                    IntPtr hImg = Win32.SHGetFileInfo(dir.FullName, 0, ref shinfo,
                        (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_SMALLICON | Win32.SHGFI_ICON);
                    newItem.ElementType = shinfo.szTypeName;
                    if (shinfo.hIcon != IntPtr.Zero)
                    {
                        //The icon is returned in the hIcon member of the shinfo struct
                        System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                        System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                        try
                        {
                            newItem.Image = ImageLoader.ConvertDrawingImageToWPFImage((System.Drawing.Image)
                                imageConverter.ConvertTo(icon, typeof(System.Drawing.Image)));
                        }
                        catch (NotSupportedException)
                        {
                        }

                        Win32.DestroyIcon(shinfo.hIcon);
                    }

                    dirContent.Add(newItem);
                }
            }
            catch
            {
            }

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(BurnTreeList.FullPath);
                foreach (FileInfo dir in dirInfo.GetFiles())
                {
                    DirectoryContent newItem = new DirectoryContent();
                    newItem.Name = dir.Name;
                    newItem.FullPath = dir.FullName;
                    newItem.Size = dir.Length;
                    newItem.IsDirectory = false;
                    newItem.LastModified = dir.LastWriteTime;
                    newItem.SourcePath = dir.FullName;
                    //
                    // Get some File Info
                    //
                    SHFILEINFO shinfo = new SHFILEINFO();
                    IntPtr hImg = Win32.SHGetFileInfo(dir.FullName, 0, ref shinfo,
                        (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_SMALLICON | Win32.SHGFI_ICON);
                    newItem.ElementType = shinfo.szTypeName;

                    if (shinfo.hIcon != IntPtr.Zero)
                    {
                        //The icon is returned in the hIcon member of the shinfo struct
                        System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                        System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                        try
                        {
                            newItem.Image = ImageLoader.ConvertDrawingImageToWPFImage((System.Drawing.Image)
                                imageConverter.ConvertTo(icon, typeof(System.Drawing.Image)));
                        }
                        catch (NotSupportedException)
                        {
                        }

                        Win32.DestroyIcon(shinfo.hIcon);
                    }

                    dirContent.Add(newItem);
                }

            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void treeViewSelectDirectory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BurnTreeList dtl = (BurnTreeList)treeViewSelectDirectory.SelectedItem;
            
            UpdateDirectory();
        }

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _BurnType = comboBoxType.SelectedIndex;
            if (AllBurnItems.Count > 0)
            {
                if (AllBurnItems[0].Items.Count > 0 || (AllBurnItems[0].FileItems != null && AllBurnItems[0].FileItems.Count > 0))
                {
                    if (MessageBox.Show("Wenn Sie den Brenn-Typ wechseln,\ngehen alle Daten in der Brennliste verloren!\nMöchten Sie wechseln?", "Achtung!", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                        return;

                    // Remove all Items
                    AllBurnItems[0].Items.Clear();
                    AllBurnItems[0].FileItems.Clear();
                    AllBurnItems[0].Size = 0;
                    _BurnSize = 0;
                    ((ObservableCollection<DirectoryContent>)dataGridBurnFileList.ItemsSource).Clear();
                }
            }

            if (comboBoxType.SelectedIndex == 1)
            {
                // Audio CD
                Settings.Current.BurnFilesColumn1 = (int)dataGridBurnFileList.Columns[0].ActualWidth;
                Settings.Current.BurnFilesColumn2 = (int)dataGridBurnFileList.Columns[1].ActualWidth;
                Settings.Current.BurnFilesColumn3 = (int)dataGridBurnFileList.Columns[2].ActualWidth;
                Settings.Current.BurnFilesColumn4 = (int)dataGridBurnFileList.Columns[3].ActualWidth;
                dataGridBurnFileList.Columns[0].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn1, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[1].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn2, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[2].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn3, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[3].Width = new DataGridLength(Settings.Current.BurnFilesAudioColumn4, DataGridLengthUnitType.Pixel);

                dataGridBurnFileList.Columns[0].CanUserSort = false;
                dataGridBurnFileList.Columns[1].CanUserSort = false;
                dataGridBurnFileList.Columns[2].CanUserSort = false;
                dataGridBurnFileList.Columns[3].CanUserSort = false;

                BurnGridDirs.Width = new GridLength(0, GridUnitType.Pixel);
                textBoxLabel.IsEnabled = false;
                //buttonToggleGAP.IsEnabled = true;
                //buttonToggleGAP.IsChecked = Settings.Current.BurnGapTracks;
                buttonToggleGAP.Visibility = Visibility.Visible;
                comboBoxVerify.IsEnabled = false;
                buttonNew.IsEnabled = false;
                UpdateCapacity();
            }
            else
            {
                // Data CD
                Settings.Current.BurnFilesAudioColumn1 = (int)dataGridBurnFileList.Columns[0].ActualWidth;
                Settings.Current.BurnFilesAudioColumn2 = (int)dataGridBurnFileList.Columns[1].ActualWidth;
                Settings.Current.BurnFilesAudioColumn3 = (int)dataGridBurnFileList.Columns[2].ActualWidth;
                Settings.Current.BurnFilesAudioColumn4 = (int)dataGridBurnFileList.Columns[3].ActualWidth;
                dataGridBurnFileList.Columns[0].Width = new DataGridLength(Settings.Current.BurnFilesColumn1, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[1].Width = new DataGridLength(Settings.Current.BurnFilesColumn2, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[2].Width = new DataGridLength(Settings.Current.BurnFilesColumn3, DataGridLengthUnitType.Pixel);
                dataGridBurnFileList.Columns[3].Width = new DataGridLength(Settings.Current.BurnFilesColumn4, DataGridLengthUnitType.Pixel);

                BurnGridDirs.Width = new GridLength(1, GridUnitType.Star);
                textBoxLabel.IsEnabled = true;
                //buttonToggleGAP.IsChecked = false;
                //buttonToggleGAP.IsEnabled = false;
                buttonToggleGAP.Visibility = Visibility.Hidden;
                comboBoxVerify.IsEnabled = true;
                buttonNew.IsEnabled = true;
                UpdateCapacity();
            }
        }

        private void treeViewSelectDirectory_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;

            BurnTreeList BurnTreeList = item.DataContext as BurnTreeList;

            BurnTreeList.Items.Clear();

            if (string.IsNullOrEmpty(BurnTreeList.FullPath))
            {
                // Get all logical Drives
                string[] allDrives = Environment.GetLogicalDrives();
                foreach (string strDrive in allDrives)
                {
                    BurnTreeList newSubDir = new BurnTreeList();
                    newSubDir.Name = strDrive;
                    newSubDir.FullPath = strDrive;
                    newSubDir.Items.Add(new BurnTreeList());
                    BurnTreeList.Items.Add(newSubDir);
                }

                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(BurnTreeList.FullPath);
            try
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    BurnTreeList newItem = new BurnTreeList();
                    newItem.FullPath = dir.FullName;
                    newItem.Name = dir.Name;
                    // Dummy-Item anlegen, damit wir die Untermenüs anzeigen können.
                    newItem.Items.Add(new BurnTreeList());

                    BurnTreeList.Items.Add(newItem);
                }
            }
            catch
            {
            }
        }

        private void dataGridSelectDirectoryFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridSelectDirectoryFiles.SelectedIndex < 0)
                return;
            DirectoryContent app = (DirectoryContent)dataGridSelectDirectoryFiles.Items[dataGridSelectDirectoryFiles.SelectedIndex];
            BurnTreeList dtl = (BurnTreeList)treeViewSelectDirectory.SelectedItem;
            if (dtl.IsExpanded == false)
            {
                dtl.IsExpanded = true;
            }
            foreach (BurnTreeList dtt in dtl.Items)
            {
                if (dtt.Name == app.Name)
                {
                    dtt.IsSelected = true;
                    dtt.IsExpanded = true;
                }
            }
        }

        private void buttonCreateDir_Click(object sender, RoutedEventArgs e)
        {
            BurnTreeList dt = treeViewBurnDirectoryList.SelectedItem as BurnTreeList;

            if (dt == null)
                return;
            BurnTreeList btl = new BurnTreeList();

            btl.Name = "Neu";
            btl.Size = 0;
            btl.IsDirectory = true;
            btl.FullPath = " ";
            btl.ElementType = "Verzeichnis";
            btl.IsExpanded = true;
            string orgName = btl.Name;
            int nCopy = 2;

            if (CheckBurnName(dt.Items, btl.Name) == true)
            {
                do
                {
                    btl.Name = orgName + " - (" + nCopy.ToString() + ")";
                    nCopy++;
                } while (CheckBurnName(dt.Items, btl.Name) == true);
            }
            dt.Items.Add(btl);
        }

        Point startPoint;
        Point startPointFiles;
        Point startPointBurnFiles;

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }


        /// <summary>
        /// Jetzt gehts los...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBurn_Click(object sender, RoutedEventArgs e)
        {
            if (comboBurner.SelectedIndex == -1)
            {
                return;
            }

            switch (comboBoxVerify.SelectedIndex)
            {
                case 0:
                    _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;
                    break;
                case 1:
                    _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_QUICK;
                    break;
                case 2:
                    _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_FULL;
                    break;
                default:
                    _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;
                    break;
            }

            this.watchMediaDrive.Stop();

            EnableBurnUI(false);

            // TODO
            //watchMediaDrive.Stop();

            if (_isBurning)
            {
                // TODO buttonBurn.Enabled = false;
                bwBurnDisc.CancelAsync();
            }
            else
            {
                _isBurning = true;
                //_closeMedia = checkBoxCloseMedia.Checked;
                //_ejectMedia = checkBoxEject.Checked;
                _isFormatting = true;

                _currentdiscRecorder = (IDiscRecorder2)comboBurner.Items[comboBurner.SelectedIndex];
                
                _burnData.uniqueRecorderId = _currentdiscRecorder.ActiveDiscRecorder;
                ObservableCollection<DirectoryContent> dca = (ObservableCollection<DirectoryContent>)AllBurnItems[0].FileItems;
                /*
                string destFile;

                foreach (DirectoryContent burnItem in dca)
                {
                    destFile = burnItem.FullPath + ".wav";

                    if (!burnItem.IsDirectory)
                    {
                        Big3.Hitbase.SoundEngine.SoundEngine.Instance.ConvertToWAVE(burnItem.FullPath, destFile);
                        textBoxCurrentBurnAction.Text = "Konvertiere: " + burnItem.FullPath;
                        FileStream fs = new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.Write);

                        // Muss der stream vielleicht 2352 byte aligned sein???
                        // Aber was passiert mit den gelöschten bytes?
                        long fsm = fs.Length % 2352;
                        long newfs = fs.Length - fsm;
                        fs.SetLength(newfs);
                        fs.Close();
                    }
                    else
                        continue;
                }
                */
                // RW Disc inserted but already written - format first...
                if (_formatNeeded == true)
                {
                    // Now format disc async
                    bwFormatDisc.RunWorkerAsync(_currentdiscRecorder.ActiveDiscRecorder);
                }
                else
                {
                    bwBurnDisc.RunWorkerAsync(_burnData);
                }
            }
        }

        private void saveSettings()
        {
            //foreach (DataGridColumn dgc in dataGridSelectDirectoryFiles.Columns)
            // dgc.Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);
            if (dataGridSelectDirectoryFiles.Columns.Count == 4)
            {
                Settings.Current.BurnSelectFilesColumn1 = (int)dataGridSelectDirectoryFiles.Columns[0].ActualWidth;
                Settings.Current.BurnSelectFilesColumn2 = (int)dataGridSelectDirectoryFiles.Columns[1].ActualWidth;
                Settings.Current.BurnSelectFilesColumn3 = (int)dataGridSelectDirectoryFiles.Columns[2].ActualWidth;
                Settings.Current.BurnSelectFilesColumn4 = (int)dataGridSelectDirectoryFiles.Columns[3].ActualWidth;
            }

            // Audio CD od Data?
            if (comboBoxType.SelectedIndex == 1)
            {
                Settings.Current.BurnFilesAudioColumn1 = (int)dataGridBurnFileList.Columns[0].ActualWidth;
                Settings.Current.BurnFilesAudioColumn2 = (int)dataGridBurnFileList.Columns[1].ActualWidth;
                Settings.Current.BurnFilesAudioColumn3 = (int)dataGridBurnFileList.Columns[2].ActualWidth;
                Settings.Current.BurnFilesAudioColumn4 = (int)dataGridBurnFileList.Columns[3].ActualWidth;
            }
            else
            {
                Settings.Current.BurnFilesColumn1 = (int)dataGridBurnFileList.Columns[0].ActualWidth;
                Settings.Current.BurnFilesColumn2 = (int)dataGridBurnFileList.Columns[1].ActualWidth;
                Settings.Current.BurnFilesColumn3 = (int)dataGridBurnFileList.Columns[2].ActualWidth;
                Settings.Current.BurnFilesColumn4 = (int)dataGridBurnFileList.Columns[3].ActualWidth;
            }

            Settings.Current.BurnDialogSizeX = (int)this.ActualWidth;
            Settings.Current.BurnDialogSizeY = (int)this.ActualHeight;

            Settings.Current.BurnGridDirsSize = (int)BurnGridDirs.ActualWidth;
            Settings.Current.SelectGridDirsSize = (int)SelectGridDirs.ActualWidth;

            Settings.Current.BurnSelectedBurner = comboBurner.SelectedIndex;
            Settings.Current.BurnTypeMedium = comboBoxType.SelectedIndex;
            Settings.Current.BurnVerifyType = comboBoxVerify.SelectedIndex;
            Settings.Current.BurnDialogTop = (int)this.Top;
            Settings.Current.BurnDialogLeft = (int)this.Left;
            Settings.Current.BurnEjectMedia = (bool)buttonToggleEject.IsChecked;
            Settings.Current.BurnGapTracks = (bool)buttonToggleGAP.IsChecked;
            
            Settings.Current.BurnEndPlaySound = (bool)buttonTogglePlaySound.IsChecked;
            Settings.Current.BurnEndSoundFile = _BurnEndSoundFile;
        }

        private void textBoxLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            var discRecorder = (IDiscRecorder2)comboBurner.Items[comboBurner.SelectedIndex];
            if (textBoxLabel.Text.Length > 0)
                AllBurnItems[0].Name = discRecorder.VolumePathNames[0].ToString() + " - [" + textBoxLabel.Text + "]";
            else
                AllBurnItems[0].Name = discRecorder.VolumePathNames[0].ToString();

            _discLabel = textBoxLabel.Text;
        }

        private void dataGridSelectDirectoryFiles_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPointFiles = e.GetPosition(null);
        }

        private void treeViewBurnDirectoryList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPointBurnFiles = e.GetPosition(null);
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            saveSettings();

            if (watchMediaDrive != null)
            {
                // Stop drive watch
                watchMediaDrive.Stop();
            }
        }

        TreeViewItem lastSelectedTreeViewItem = null;

        private void treeViewBurnDirectoryList_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            // set the last tree view item selected variable which may be used elsewhere as there is no other way I have found to obtain the TreeViewItem container (may be null)        
            this.lastSelectedTreeViewItem = tvi;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            DataGrid dataGrid = dropInfo.DragInfo.SourceItem as DataGrid;

            if (dropInfo.DragInfo.VisualSource == this.dataGridBurnFileList && this._BurnType == 1)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == dataGridBurnFileList && dropInfo.DragInfo.VisualSource == dataGridBurnFileList)
            {
                ObservableCollection<DirectoryContent> dca = dataGridBurnFileList.ItemsSource as ObservableCollection<DirectoryContent>;
                int oldIndex = dca.IndexOf((DirectoryContent)dropInfo.DragInfo.SourceItem);
                dca.Move(oldIndex, dropInfo.InsertIndex);

                return;
            }

            //BurnTreeList dts = new BurnTreeList();
            BurnTreeList dtb = new BurnTreeList();

            System.Windows.Controls.TreeViewItem treeViewItem = dropInfo.VisualTargetItem as TreeViewItem;

            DirectoryContent dragSource = dropInfo.DragInfo.SourceItem as DirectoryContent;
            List<DirectoryContent> dragSourceList = dropInfo.DragInfo.SourceItems as List<DirectoryContent>;

            BurnTreeList dragSourceDataDir = dropInfo.Data as BurnTreeList;
            DirectoryContent dragSourceDirectoryContent = dropInfo.Data as DirectoryContent;
            List<DirectoryContent> listDirectoryContents = dropInfo.Data as List<DirectoryContent>;
            //if (dropInfo.Data.GetType(). == FullName = "Big3.Hitbase.Controls.DragDrop.DropInfo"}	System.Type {System.RuntimeType}
            
            
            List<string> allDropItems = new List<string>();

            // Drop on Directory in directory view
            DirectoryContent dropOnDir = dropInfo.TargetItem as DirectoryContent;
            
            //if (dragSourceDataDir != null)
            //{
            //    dts = (BurnTreeList)treeViewSelectDirectory.SelectedItem;
            //}
            
            if (treeViewItem == null)
            {
                dtb = AllBurnItems.Last<BurnTreeList>();
                if ((BurnTreeList)treeViewBurnDirectoryList.SelectedItem != null)
                    dtb = (BurnTreeList)treeViewBurnDirectoryList.SelectedItem;
                if (dropOnDir != null && dropOnDir.IsDirectory == true)
                {
                    foreach (BurnTreeList btl in dtb.Items)
                    {
                        if (btl.Name == dropOnDir.Name)
                        {
                            dtb = btl;
                            break;
                        }
                    }
                }
            }
            else
            {
                dtb = (BurnTreeList)treeViewItem.DataContext;
            }
            
            dtb.IsExpanded = true;

            // Nur bei Audio CDs...
            if (listDirectoryContents != null)
            {
                foreach (DirectoryContent dirContent in listDirectoryContents)
                {
                    dtb.Size = AddDirectoryItem(dirContent.FullPath, dtb, true);
                    _BurnSize += dtb.Size;
                }
            }
            else
            {
                if (dragSourceDirectoryContent != null)
                {
                    dtb.Size = AddDirectoryItem(dragSourceDirectoryContent.FullPath, dtb, true);
                    _BurnSize += dtb.Size;
                }
                if (dragSourceDataDir != null)
                {
                    dtb.Size = AddDirectoryItem(dragSourceDataDir.FullPath, dtb, true);
                    _BurnSize += dtb.Size;
                }
            }

            UpdateBurnDirectory();
            UpdateCapacity();
        }

        // Convert in TEMP File to WAVE
        private string ConvertWaveToMp3(string convertFile)
        {
            int nCopy = 2;
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += delegate
            //{

            string tempPath = Path.GetTempPath();

            string destFile = tempPath + convertFile.Mid(convertFile.LastIndexOf('\\') + 1) + ".wav";

            string destFileOrg = destFile;
            // Ist der Dateiname schon da?
            if (File.Exists(destFile) == true)
            {
                do
                {
                    destFile = destFileOrg + " - (" + nCopy.ToString() + ")";
                    nCopy++;
                } while (File.Exists(destFile) == true);
            }


            //Big3.Hitbase.SoundEngine.SoundEngine.Instance.ConvertToWAVE(fullpath, destfile);
            if (Big3.Hitbase.SoundEngine.SoundEngine.Instance.ConvertToWAVE(convertFile, destFile, convertToWaveDelegate) == true)
            {
                FileStream fs = new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.Write);

                // Muss der stream vielleicht 2352 byte aligned sein???
                // Aber was passiert mit den gelöschten bytes?
                long fsm = fs.Length % 2352;
                long newfs = fs.Length - fsm;
                fs.SetLength(newfs);
                fs.Close();

                return destFile;
            }

            return null;
            //};
            //bw.RunWorkerCompleted += delegate
            //{
            //    UpdateBurnDirectory();
            //    UpdateCapacity();
            //    textBoxCurrentBurnAction.Text = "";
            //};

            //bw.RunWorkerAsync();
        }

        private void convertToWaveDelegate(string sourceFile, string targetFile, double percent)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                string convertFile = sourceFile.Mid(sourceFile.LastIndexOf('\\') + 1);
                textBoxCurrentBurnAction.Text = string.Format("Konvertiere '{0}':  {1:0.0}%", convertFile, percent);
            }));
        }

        private void dataGridBurnFileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridBurnFileList.SelectedIndex < 0)
                return;
            DirectoryContent app = (DirectoryContent)dataGridBurnFileList.Items[dataGridBurnFileList.SelectedIndex];
            BurnTreeList dtl = (BurnTreeList)treeViewBurnDirectoryList.SelectedItem;
            if (dtl.IsExpanded == false)
            {
                dtl.IsExpanded = true;
            }
            foreach (BurnTreeList dtt in dtl.Items)
            {
                if (dtt.Name == app.Name)
                {
                    dtt.IsSelected = true;
                    dtt.IsExpanded = true;
                }
            }
        }

        #region Burn Media Process

        /// <summary>
        /// The thread that does the burning of the media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = new MsftDiscRecorder2();

            try
            {
                // Burntype = 0 - Write data disc
                if (_BurnType == 0)
                {
                    MsftDiscFormat2Data discFormatData = null;
                    //
                    // Create and initialize the IDiscRecorder2 object
                    //
                    //discRecorder = new MsftDiscRecorder2();
                    var burnData = (BurnData)e.Argument;
                    discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);

                    //
                    // Create and initialize the IDiscFormat2Data
                    //
                    discFormatData = new MsftDiscFormat2Data
                    {
                        Recorder = discRecorder,
                        ClientName = ClientName,
                        ForceMediaToBeClosed = _closeMedia
                    };

                    //
                    // Set the verification level
                    //
                    var burnVerification = (IBurnVerification)discFormatData;
                    burnVerification.BurnVerificationLevel = _verificationLevel;

                    object[] multisessionInterfaces = null;
                    //
                    // Check if media is blank, (for RW media)
                    //
                    //object[] multisessionInterfaces = null;
                    //if (!discFormatData.MediaHeuristicallyBlank)
                    //{
                    //    multisessionInterfaces = discFormatData.MultisessionInterfaces;
                    //}

                    //
                    // Create the file system
                    //
                    IStream fileSystem;
                    if (!CreateMediaFileSystem(discRecorder, multisessionInterfaces, out fileSystem))
                    {
                        e.Result = -1;
                        return;
                    }

                    //
                    // add the Update event handler
                    //
                    discFormatData.Update += discFormatData_Update;

                    //
                    // Write the data here
                    //
                    try
                    {
                        discFormatData.Write(fileSystem);
                        e.Result = 0;
                    }
                    catch (COMException ex)
                    {
                        e.Result = ex.ErrorCode;
                        MessageBox.Show(ex.Message);
                        //textBoxCurrentBurnAction.Text = "IDiscFormat2Data.Write failed!";
                    }
                    finally
                    {
                        if (fileSystem != null)
                        {
                            Marshal.FinalReleaseComObject(fileSystem);
                        }
                    }

                    //
                    // remove the Update event handler
                    //
                    discFormatData.Update -= discFormatData_Update;

                    if (_ejectMedia == true)
                    {
                        discRecorder.EjectMedia();
                    }

                    if (_BurnEndPlaySound == true)
                    {
                        MediaPlayer mplayer = new MediaPlayer();
                        mplayer.Open(new Uri(_BurnEndSoundFile, UriKind.Absolute));
                        mplayer.Play();
                    }

                    if (discFormatData != null)
                    {
                        Marshal.ReleaseComObject(discFormatData);
                    }
                } // Burn data disc

                // Burn audio disc
                if (_BurnType == 1)
                {
                    MsftDiscMaster2 discMaster = new MsftDiscMaster2();
                    MsftDiscFormat2RawCD discFormatRawCD = new MsftDiscFormat2RawCD();
                    MsftRawCDImageCreator rawCDImageCreator = new MsftRawCDImageCreator();
                    
                    string destFile;
                    //
                    // Create and initialize the IDiscRecorder2 object
                    //
                    //discRecorder = new MsftDiscRecorder2();
                    //ObservableCollection<DirectoryContent> dca = (ObservableCollection<DirectoryContent>)AllBurnItems[0].FileItems;
                    ObservableCollection<DirectoryContent> dca = dataGridBurnFileList.ItemsSource as ObservableCollection<DirectoryContent>;

                    var burnData = (BurnData)e.Argument;
                    discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);

                    //
                    // Create and initialize the IDiscFormat2Data
                    //
                    discFormatRawCD = new MsftDiscFormat2RawCD
                    {
                        Recorder = discRecorder,
                        ClientName = ClientName,
                    };

                    //
                    // Set the verification level
                    // TODO
                    // Verify geht bei raw wohl nicht...
                    // var burnVerification = (IBurnVerification)discFormatRawCD;
                    //burnVerification.BurnVerificationLevel = _verificationLevel;
                    

                    //object[] multisessionInterfaces = null;

                    //rawCDImageCreator.DisableGaplessAudio = false;

                    //discFormatRawCD.ClientName = ClientName;
                    //discFormatRawCD.Recorder = discRecorder;
                    // ???
                    //discFormatRawCD.RequestedSectorType = IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE.IMAPI_FORMAT2_RAW_CD_SUBCODE_IS_RAW;
                    if (discFormatRawCD.IsCurrentMediaSupported(discRecorder) == false)
                    {
                        MessageBox.Show("Medium nicht unterstützt!");
                    }
                    

                    // Pause zwischen den Tracks?
                    if (_enableGap)
                        rawCDImageCreator.DisableGaplessAudio = true;
                    else
                        rawCDImageCreator.DisableGaplessAudio = false;

                    rawCDImageCreator.ResultingImageType = IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE.IMAPI_FORMAT2_RAW_CD_SUBCODE_IS_RAW;
                    
                    //rawCDImageCreator.DisableGaplessAudio = true;
                    //ObservableCollection<BurnTreeList> btt = AllBurnItems;
                    List<IStream> streams = new List<IStream>();
                    foreach (DirectoryContent burnItem in dca)
                    {
                        IStream stream = null;

                        // 0x04000000 : Datei löschen, wenn Sie geschlossen wird.
                        Win32.SHCreateStreamOnFileEx(burnItem.FullPath, Win32.STGM_READWRITE, 0x04000000, false, null, ref stream);
                        
                        try
                        {
                            //int ret = rawCDImageCreator.AddTrack(IMAPI_CD_SECTOR_TYPE.IMAPI_CD_SECTOR_AUDIO, dataStream);
                            //rawCDImageCreator.DisableGaplessAudio = true; 
                            int i = rawCDImageCreator.AddTrack(IMAPI_CD_SECTOR_TYPE.IMAPI_CD_SECTOR_AUDIO, stream);

                            streams.Add(stream);
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show(Ex.Message);
                        }
                    }
                    
                    //rawCDImageCreator.DisableGaplessAudio = false;

                    discFormatRawCD.Update += discAtOnce_Update;
                    
                    IStream resultStream = rawCDImageCreator.CreateResultImage();
                    
                    try
                    {
                        discFormatRawCD.PrepareMedia();
                        discFormatRawCD.RequestedSectorType = IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE.IMAPI_FORMAT2_RAW_CD_SUBCODE_IS_RAW;
                        discFormatRawCD.WriteMedia(resultStream);
                    }
                    catch (COMException ex)
                    {
                        e.Result = ex.ErrorCode;
                        MessageBox.Show(ex.Message);
                        //textBoxCurrentBurnAction.Text = "IDiscFormat2Data.Write failed!";
                    }

                    discFormatRawCD.Update -= discAtOnce_Update;

                    discFormatRawCD.ReleaseMedia();

                    Marshal.ReleaseComObject(rawCDImageCreator);

                    foreach (IStream stream in streams)
                    {
                        IntPtr streamPtr = Marshal.GetIUnknownForObject(stream);
                        Marshal.Release(streamPtr);
                        int refCount = Marshal.ReleaseComObject(stream);
                        int refCount1 = Marshal.FinalReleaseComObject(stream);
                    }

                    if (_ejectMedia == true)
                    {
                        discRecorder.EjectMedia();
                    }

                    if (_BurnEndPlaySound == true)
                    {
                        MediaPlayer mplayer = new MediaPlayer();
                        mplayer.Open(new Uri(_BurnEndSoundFile, UriKind.Absolute));
                        mplayer.Play();
                    }

                    if (discFormatRawCD != null)
                    {
                        Marshal.ReleaseComObject(discFormatRawCD);
                    }
                }

            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                MessageBox.Show(exception.Message);
                e.Result = exception.ErrorCode;
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }
            }

            // Entfernen von ein paar Temp Dateien für Audio CDs
            // TODO!!!!!!!!!!!!!!!!!!!!
            if (_BurnType == 1)
            {
                ObservableCollection<DirectoryContent> allBurnFiles = (ObservableCollection<DirectoryContent>)AllBurnItems[0].FileItems;
                string tempFile;

                foreach (DirectoryContent burnItem in allBurnFiles)
                {
                    tempFile = burnItem.FullPath;

                    if (!burnItem.IsDirectory)
                    {
                        // TODO
                        //File.Delete(tempFile);
                    }
                    else
                        continue;
                }
            }
        }
        /// <summary>
        /// Update notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        void discAtOnce_Update(object sender, object progress)
        {
            //
            // Check if we've cancelled
            //
            if (bwBurnDisc.CancellationPending)
            {
                IDiscFormat2RawCD dao = (IDiscFormat2RawCD)sender;

                dao.CancelWrite();
                return;
            }

            IDiscFormat2RawCDEventArgs eventArgs = (IDiscFormat2RawCDEventArgs)progress;
            
            IMAPI_FORMAT2_DATA_WRITE_ACTION writeaction;

            writeaction = IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA;

            if (eventArgs.CurrentAction == IMAPI_FORMAT2_RAW_CD_WRITE_ACTION.IMAPI_FORMAT2_RAW_CD_WRITE_ACTION_FINISHING)
            {
                writeaction = IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION;
            }
            if (eventArgs.CurrentAction == IMAPI_FORMAT2_RAW_CD_WRITE_ACTION.IMAPI_FORMAT2_RAW_CD_WRITE_ACTION_PREPARING)
            {
                writeaction = IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER;
            }
            if (eventArgs.CurrentAction == IMAPI_FORMAT2_RAW_CD_WRITE_ACTION.IMAPI_FORMAT2_RAW_CD_WRITE_ACTION_UNKNOWN)
            {
                writeaction = IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA;
            }
            if (eventArgs.CurrentAction == IMAPI_FORMAT2_RAW_CD_WRITE_ACTION.IMAPI_FORMAT2_RAW_CD_WRITE_ACTION_WRITING)
            {
                writeaction = IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA;
            }

            _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            //
            // IDiscFormat2TrackAtOnceEventArgs Interface
            //
            _burnData.currentAction = writeaction;
            _burnData.elapsedTime = eventArgs.ElapsedTime;
            _burnData.remainingTime = eventArgs.RemainingTime;

            //
            // IWriteEngine2EventArgs Interface
            //
            _burnData.currentAction = writeaction;
            _burnData.startLba = eventArgs.StartLba;
            _burnData.sectorCount = eventArgs.SectorCount;
            _burnData.lastReadLba = eventArgs.LastReadLba;
            _burnData.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData.freeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            bwBurnDisc.ReportProgress(0, _burnData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        void discFormatData_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (bwBurnDisc.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;

                format2Data.CancelWrite();
                _isBurning = false;
                EnableBurnUI(true);
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData.elapsedTime = eventArgs.ElapsedTime;
            _burnData.remainingTime = eventArgs.RemainingTime;
            _burnData.totalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData.currentAction = eventArgs.CurrentAction;
            _burnData.startLba = eventArgs.StartLba;
            _burnData.sectorCount = eventArgs.SectorCount;
            _burnData.lastReadLba = eventArgs.LastReadLba;
            _burnData.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData.freeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            bwBurnDisc.ReportProgress(0, _burnData);
        }

        /// <summary>
        /// Completed the "Burn" thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ObservableCollection<DirectoryContent> dca = (ObservableCollection<DirectoryContent>)AllBurnItems[0].FileItems;
            string destFile;
            if (e.Result == null)
                textBoxCurrentBurnAction.Text = "Medium ist fertig gebrannt!";
            else
                textBoxCurrentBurnAction.Text = (int)e.Result == 0 ? "Medium ist fertig gebrannt!" : "Fehler beim Brennen des Mediums!";

            progressBarBurnStatus.Value = 0;
            TaskbarItemInfo.ProgressValue = 0;

            _isBurning = false;
            EnableBurnUI(true);
            
            CheckMedia();
            
            this.watchMediaDrive.Start();
            /*
            foreach (DirectoryContent burnItem in dca)
            {
                destFile = burnItem.FullPath;

                if (!burnItem.IsDirectory)
                {
                    File.Delete(destFile);
                }
                else
                    continue;
            }
            */
        }

        /// <summary>
        /// Enables/Disables the "Burn" User Interface
        /// </summary>
        /// <param name="enable"></param>
        void EnableBurnUI(bool enable)
        {
            TextBlockBurn.Text = enable ? "Brennen!" : "Abbrechen";


            comboBoxVerify.IsEnabled = enable;
            comboBoxWriteSpeed.IsEnabled = enable;
            textBoxLabel.IsEnabled = enable;
            comboBoxType.IsEnabled = enable;
            comboBurner.IsEnabled = enable;
            buttonAdd.IsEnabled = enable;
            buttonNew.IsEnabled = enable;
            buttonDel.IsEnabled = enable;
            dataGridSelectDirectoryFiles.IsEnabled = enable;
            dataGridBurnFileList.IsEnabled = enable;
            treeViewBurnDirectoryList.IsEnabled = enable;
            treeViewSelectDirectory.IsEnabled = enable;
        }

        /// <summary>
        /// Event receives notification from the Burn thread of an event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //int percent = e.ProgressPercentage;
            var burnData = (BurnData)e.UserState;

            if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                textBoxCurrentBurnAction.Text = burnData.statusMessage;
            }
            else if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.currentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        textBoxCurrentBurnAction.Text = "Überprüfe Medium...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        textBoxCurrentBurnAction.Text = "Formatiere Medium...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        textBoxCurrentBurnAction.Text = "Initialisiere Brenner...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        textBoxCurrentBurnAction.Text = "Optimiere Laseritensität...";
                        break;

 
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.lastWrittenLba - burnData.startLba;

                        if (writtenSectors > 0 && burnData.sectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.sectorCount);
                            textBoxCurrentBurnAction.Text = string.Format("Fortschritt: {0}%", percent);
                            progressBarBurnStatus.Value = percent;
                            TaskbarItemInfo.ProgressValue = percent;
                        }
                        else
                        {
                            textBoxCurrentBurnAction.Text = "Fortschritt: 0%";
                            progressBarBurnStatus.Value = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        textBoxCurrentBurnAction.Text = "Fertigstellen...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        textBoxCurrentBurnAction.Text = "Fertig!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        textBoxCurrentBurnAction.Text = "Überprüfen...";
                        break;
                }
            }
        }

        /// <summary>
        /// Enable the Burn Button if items in the file listbox
        /// </summary>
        private void EnableBurnButton()
        {
            //buttonBurn.Enabled = (listBoxFiles.Items.Count > 0);
        }


        #endregion
        #region File System Process
        /// <summary>
        /// Rekursiv die Brennstruktur aufbauen...
        /// </summary>
        /// <param name="btt"></param>
        private void BuildMediumStruct(IFsiDirectoryItem rootItem, string BurnPath, ObservableCollection <BurnTreeList> btt)
        {
            foreach (BurnTreeList burnItem in btt)
            {
                string currentBurnPath = "";
                
                if (burnItem.Name.IndexOf(':') < 0)
                    currentBurnPath = BurnPath + "\\" + burnItem.Name;

                if (burnItem.IsDirectory)
                {
                    if (!string.IsNullOrEmpty(currentBurnPath))
                        rootItem.AddDirectory(currentBurnPath);

                    rootItem.CreationTime = burnItem.LastModified;
                    rootItem.LastModifiedTime = burnItem.LastModified;
                    rootItem.LastAccessedTime = burnItem.LastModified;

                    BuildMediumStruct(rootItem, currentBurnPath, burnItem.Items);
                }


                foreach (DirectoryContent dc in burnItem.FileItems)
                {
                    AddToFileSystem(rootItem, dc.FullPath, currentBurnPath + "\\" + dc.Name);

                    if (bwBurnDisc.CancellationPending)
                    {
                        break;
                    }
                }
            }

        }

        private bool CreateMediaFileSystem(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, out IStream dataStream)
        {
            MsftFileSystemImage fileSystemImage = null;
            try
            {
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaults(discRecorder);
                fileSystemImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                // TODO - Namen anpassen
                fileSystemImage.VolumeName = _discLabel;
                
                fileSystemImage.Update += fileSystemImage_Update;

                //
                // If multisessions, then import previous sessions
                //
                //if (multisessionInterfaces != null)
                //{
                //    fileSystemImage.MultisessionInterfaces = multisessionInterfaces;
                //    fileSystemImage.ImportFileSystem();
                //}

                //
                // Get the image root
                //
                IFsiDirectoryItem rootItem = fileSystemImage.Root;

                ObservableCollection<BurnTreeList> btt = AllBurnItems;
                // Add Files and Directories to File System Image
                //
//                foreach (IMediaItem mediaItem in listBoxFiles.Items)
                BuildMediumStruct(rootItem, "", btt);

                fileSystemImage.Update -= fileSystemImage_Update;

                //
                // did we cancel?
                //
                if (bwBurnDisc.CancellationPending)
                {
                    dataStream = null;
                    return false;
                }

                dataStream = fileSystemImage.CreateResultImage().ImageStream;
            }
            catch (COMException exception)
            {
                textBoxCurrentBurnAction.Text = "Create File System Error";
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataStream = null;
                return false;
            }
            finally
            {
                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }

            return true;
        }

        private bool AddToFileSystem(IFsiDirectoryItem rootItem, string filePath, string displayName)
        {
            IStream stream = null;

            try
            {
                Win32.SHCreateStreamOnFile(filePath, Win32.STGM_READ | Win32.STGM_SHARE_DENY_WRITE, ref stream);

                if (stream != null)
                {
                    rootItem.AddFile(displayName, stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error adding file",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.FinalReleaseComObject(stream);
                }
            }

            return false;
        }

        /// <summary>
        /// Event Handler for File System Progress Updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentFile"></param>
        /// <param name="copiedSectors"></param>
        /// <param name="totalSectors"></param>
        void fileSystemImage_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender,
            [In, MarshalAs(UnmanagedType.BStr)]string currentFile, [In] int copiedSectors, [In] int totalSectors)
        {
            var percentProgress = 0;
            if (copiedSectors > 0 && totalSectors > 0)
            {
                percentProgress = (copiedSectors * 100) / totalSectors;
            }

            if (!string.IsNullOrEmpty(currentFile))
            {
                var fileInfo = new FileInfo(currentFile);
                _burnData.statusMessage = "Adding \"" + fileInfo.Name + "\" to image...";

                //
                // report back to the ui
                //
                _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM;
                bwBurnDisc.ReportProgress(percentProgress, _burnData);
            }

        }
        #endregion

        private void treeViewBurnDirectoryList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                delItem();
            }
        }

        private void dataGridBurnFileList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                delItem();
            }
        }

        private void buttonToggleEject_Click(object sender, RoutedEventArgs e)
        {
            _ejectMedia = (bool)buttonToggleEject.IsChecked;
        }
        
        private void buttonTogglePlaySound_Click(object sender, RoutedEventArgs e)
        {
            _BurnEndPlaySound = (bool)buttonTogglePlaySound.IsChecked;
            if (_BurnEndPlaySound = true && !File.Exists(_BurnEndSoundFile))
            {
                if (SelectSoundFile() == false)
                {
                    buttonTogglePlaySound.IsChecked = false;
                    _BurnEndPlaySound = false;
                    buttonTogglePlaySound.ToolTip = "Sound Datei: Keine ausgewählt!";
                    return;
                }
            }
            buttonTogglePlaySound.ToolTip = "Sound Datei: " + _BurnEndSoundFile;
        }
        
        private bool SelectSoundFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Sound Dateien (MP3, WAV)|*.mp3;*.wav";

            if (ofd.ShowDialog() == true)
            {
                if (File.Exists(ofd.FileName))
                {
                    _BurnEndSoundFile = ofd.FileName;
                    return true;
                }
            }
            return false;
        }

        private void buttonSelectSound_Click(object sender, RoutedEventArgs e)
        {
            if (SelectSoundFile() == true)
                buttonTogglePlaySound.ToolTip = "Sound Datei: " + _BurnEndSoundFile;
        }

        private void buttonToggleGAP_Click(object sender, RoutedEventArgs e)
        {
            _enableGap = (bool)buttonToggleGAP.IsChecked;
        }

        private void dataGridBurnFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridBurnFileList.SelectedItems.Count > 0)
            {
                buttonDel.IsEnabled = true;
            }
            else 
            {
                buttonDel.IsEnabled = false;
            }
        }
    }

}
