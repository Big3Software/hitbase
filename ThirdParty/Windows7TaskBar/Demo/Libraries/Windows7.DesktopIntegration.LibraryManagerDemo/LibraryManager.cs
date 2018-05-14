// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.SDK.Samples.VistaBridge.Library.StockIcons;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Interop;
using Microsoft.SDK.Samples.VistaBridge.Library.KnownFolders;
using Windows7.DesktopIntegration;

namespace Windows7.DesktopIntegration.LibraryManagerDemo
{
    /// <summary>
    /// This class manages a ShellLibrary instance
    /// </summary>
    class LibraryManager : DependencyObject
    {
        //used to break update loop of dependency properties
        private bool _isIgnoreEvent;

        //provide notification when shell library is changed
        private FileSystemWatcher _libraryWatcher; 

        /// <summary>
        /// The list of the folders in the library
        /// </summary>
        public ObservableCollection<string> FolderList { get; set; }

        /// <summary>
        /// The list of folder types for folder templates
        /// </summary>
        public ObservableCollection<string> FolderTypeList { get; set; }

        /// <summary>
        /// The library name 
        /// </summary>
        public string LibraryName
        {
            get { return (string)GetValue(LibraryNameProperty); }
            set { SetValue(LibraryNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LibraryName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LibraryNameProperty =
            DependencyProperty.Register("LibraryName", typeof(string), typeof(LibraryManager), new UIPropertyMetadata(""));


        /// <summary>
        /// The default save folder 
        /// </summary>
        public string DefaultSaveFolder
        {
            get { return (string)GetValue(DefaultSaveFolderProperty); }
            set { SetValue(DefaultSaveFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultSaveFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultSaveFolderProperty =
            DependencyProperty.Register("DefaultSaveFolder", typeof(string), typeof(LibraryManager), new UIPropertyMetadata(""));



        /// <summary>
        /// The icon path 
        /// </summary>
        public BitmapSource ShellIcon
        {
            get { return (BitmapSource)GetValue(ShellIconProperty); }
            set { SetValue(ShellIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShellIconProperty =
            DependencyProperty.Register("Icon", typeof(BitmapSource), typeof(LibraryManager), new UIPropertyMetadata(null));


        /// <summary>
        /// The pinned To Explorer navigation pane state
        /// </summary>
        public bool IsPinnedToNavigationPane
        {
            get { return (bool)GetValue(IsPinnedToNavigationPaneProperty); }
            set { SetValue(IsPinnedToNavigationPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPinnedToNavigationPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPinnedToNavigationPaneProperty =
            DependencyProperty.Register("IsPinnedToNavigationPane", typeof(bool), typeof(LibraryManager), new UIPropertyMetadata(true));


        /// <summary>
        /// The folder type template name
        /// </summary>
        public string FolderType
        {
            get { return (string)GetValue(FolderTypeProperty); }
            set { SetValue(FolderTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FolderType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderTypeProperty =
            DependencyProperty.Register("FolderType", typeof(string), typeof(LibraryManager), new UIPropertyMetadata(""));




        /// <summary>
        /// Create new LibraryManager instance
        /// </summary>
        public LibraryManager()
        {
            FolderList = new ObservableCollection<string>();
            InitFolderTypeList();
        }

        /// <summary>
        /// Populate the forlder type list
        /// </summary>
        private void InitFolderTypeList()
        {
             FolderTypeList = new ObservableCollection<string>() 
            {
                "Not Specified", "Documents", "Pictures", 
                "Music", "Videos", "RecordedTV", "SavedGames", 
                "User Files"
            };
        }

        /// <summary>
        /// This method is used to reflect the dependency property changes in the underline shell library.
        /// </summary>
        /// <param name="e">The DP that has changed</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch(e.Property.Name)
            {
                case "IsPinnedToNavigationPane":
                    SetPinnedToNavigationPaneState((bool)e.NewValue);
                    break;

                case "DefaultSaveFolder":
                    string value = e.NewValue as string;
                    if (!string.IsNullOrEmpty(value))
                        SetDefaultSaveFolder(value);
                    break;   
             
                case "FolderType":
                    SetFolderType(e.NewValue as string);
                    break;
            }
        }

        /// <summary>
        /// Open an existing shell library
        /// </summary>
        /// <param name="shellLibraryPath">The path to the .library-ms file</param>
        public void OpenShellLibrary(string shellLibraryPath)
        {
            using (ShellLibrary shellLibrary = ShellLibrary.Load(shellLibraryPath, true))
            {
                LibraryName = shellLibraryPath;
                ListenToShellLibraryChange();
                UpdateLibraryState(shellLibrary);
            }
        }

        /// <summary>
        /// Make the library manager state coherent with the underline shell library
        /// </summary>
        /// <param name="shellLibrary">The shell library path</param>
        private void UpdateLibraryState(ShellLibrary shellLibrary)
        {
            try
            {
                //break update loop
                _isIgnoreEvent = true;

                FolderList.Clear();
                foreach (string folder in shellLibrary.GetFolders())
                    FolderList.Add(folder);
                DefaultSaveFolder = shellLibrary.DefaultSaveFolder;
                IsPinnedToNavigationPane = shellLibrary.IsPinnedToNavigationPane;

                string iconPath = shellLibrary.Icon;
                ShellIcon = string.IsNullOrEmpty(iconPath) ? null : Helper.GetIconBitmap(iconPath);
                
                string folderType;
                try
                {
                    folderType = FolderTypes.GetFolderType(shellLibrary.FolderTypeId);
                }
                catch
                {
                    folderType = "";
                }
                FolderType = folderType;
            }
            finally
            {
                _isIgnoreEvent = false;
            }
        }

        /// <summary>
        /// Create new shell library instance
        /// </summary>
        /// <param name="locationPath">Where to create</param>
        /// <param name="name">the name of the library without the .library-ms</param>
        public void CreateShellLibrary(string locationPath, string name)
        {
            using (ShellLibrary shellLibrary = ShellLibrary.Create(name, locationPath))
            {
                LibraryName = shellLibrary.FullName;
            }

            DefaultSaveFolder = string.Empty;
            FolderList.Clear();
            ShellIcon = null;
            ListenToShellLibraryChange();
        }

        /// <summary>
        /// Use <see="FileSystemWatcher"> to update the library manager state whenever there is a change
        /// in the underline shell library
        /// </summary>
        private void ListenToShellLibraryChange()
        {
            if (_libraryWatcher != null)
            {
                _libraryWatcher.Dispose();
            }
            string directoryPath = System.IO.Path.GetDirectoryName(LibraryName);
            string fileName = System.IO.Path.GetFileName(LibraryName);

            _libraryWatcher = new FileSystemWatcher(directoryPath);
            _libraryWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _libraryWatcher.Filter = fileName;
            _libraryWatcher.IncludeSubdirectories = false;

            _libraryWatcher.Changed += (s, e) =>
            {
                //cross thread call
                this.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            //We open the library with write permissions for ResolveFolder
                            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
                            {
                                UpdateLibraryState(shellLibrary);
                            }
                        }
                        catch (Exception exp)
                        {
                            System.Diagnostics.Trace.WriteLine("Could not update the library:" + LibraryName + " state, Error: " + exp.Message);
                        }
                    }));
            };
            _libraryWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Add directory to the shell library
        /// </summary>
        /// <param name="folderPath">the folder path</param>
        public void AddDirectory(string folderPath)
        {
            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.AddFolder(folderPath);
            }
        }


        /// <summary>
        /// Remove directory from a shell library
        /// </summary>
        /// <param name="folderPath">the directory path to remove</param>
        public void RemoveDirectory(string folderPath)
        {
            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.RemoveFolder(folderPath);
            }
        }

        /// <summary>
        /// Show the explorer library manage user interface
        /// </summary>
        /// <param name="hOwnerWnd">the parent window handle</param>
        public void OpenLibraryManageUI(IntPtr hOwnerWnd)
        {
            ShellLibrary.ShowManageLibraryUI(LibraryName, hOwnerWnd, "The Windows Shell Explorer Library Manager", "Manage the " + LibraryName, true);
        }

        /// <summary>
        /// Delete the library
        /// </summary>
        public void DeleteLibrary()
        {
            ShellLibrary.Delete(LibraryName);

            try
            {
                _isIgnoreEvent = true;

                DefaultSaveFolder = string.Empty;
                LibraryName = string.Empty;
                FolderList.Clear();
                ShellIcon = null;
                _libraryWatcher.Dispose();
                _libraryWatcher = null;
            }
            finally
            {
                _isIgnoreEvent = false;
            }
        }

        /// <summary>
        /// Set the shell library default folder
        /// <remarks>Clients of this class use the <see cref="DefaultSaveFolder"/> property</remarks>
        /// </summary>
        /// <param name="folderPath">the path to the default folder</param>
        private void SetDefaultSaveFolder(string folderPath)
        {
            if (_isIgnoreEvent)
                return;

            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.DefaultSaveFolder = folderPath;
            }
        }

        /// <summary>
        /// Set the folder type template
        /// </summary>
        /// <param name="typeName">The folder type name</param>
        private void SetFolderType(string typeName)
        {
            Guid folderType = FolderTypes.GetFolderType(typeName);

            if (_isIgnoreEvent || folderType == Guid.Empty)
                return;

            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.FolderTypeId = folderType;
            }
        }

        /// <summary>
        /// Set the pinning state in the explorer navigation pane
        /// <remarks>Clients of this class use the <see cref="PinnedToNavigationPaneState"/> property</remarks>
        /// </summary>
        /// <param name="state"></param>
        private void SetPinnedToNavigationPaneState(bool state)
        {
            if (_isIgnoreEvent)
                return;

            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.IsPinnedToNavigationPane = state;
            }
        }

        /// <summary>
        /// Set the icon of the library
        /// </summary>
        /// <param name="iconId">the Shell stock icon ID</param>
        public void SetIcon(int iconId)
        {
            using (ShellLibrary shellLibrary = ShellLibrary.Load(LibraryName, true))
            {
                shellLibrary.Icon = Helper.GetIcon(iconId);
                ShellIcon = StockIcons.GetBitmapSource((StockIconIdentifier)iconId);
            }
        }

        /// <summary>
        /// This helper class provide interop gateway to native shell API that are not exported in the Vista Bridge
        /// </summary>
        class Helper
        {
            [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct StockIconInfo
            {
                internal UInt32 StuctureSize;
                internal IntPtr Handle;
                internal Int32 ImageIndex;
                internal Int32 Identifier;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                internal string Path;
            }

            public static string GetIcon(int identifier)
            {
                StockIconInfo info = new StockIconInfo();
                info.StuctureSize = (UInt32)System.Runtime.InteropServices.Marshal.SizeOf(typeof(StockIconInfo));

                int hResult = SHGetStockIconInfo(identifier, 0, ref info);

                if (hResult != 0)
                    throw new System.ComponentModel.Win32Exception("SHGetStockIconInfo execution failure " + hResult.ToString());

                return info.Path + "," + info.Identifier;
            }

            public static BitmapSource GetIconBitmap(string iconPath)
            {
                string[] iconInfo = iconPath.Split(',');

                IntPtr iconHandle = ExtractIcon(0, iconInfo[0], int.Parse(iconInfo[1]));

                if (iconHandle == IntPtr.Zero || iconHandle == new IntPtr(1))
                    throw new System.ComponentModel.Win32Exception("ExtractIcon execution failure ");

                BitmapSource imageSource = Imaging.CreateBitmapSourceFromHIcon(iconHandle, System.Windows.Int32Rect.Empty, null);
                DestroyIcon(iconHandle);
                return imageSource;
            }

            [DllImport("Shell32.dll", CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = false)]
            private static extern int SHGetStockIconInfo(
                int identifier,
                int flags,
                ref StockIconInfo info);

            [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
            private static extern IntPtr ExtractIcon(
                int hInst,
                string lpszExeFileName,
                int nIconIndex);


            [DllImport("User32.dll", CharSet = CharSet.Unicode)]
            private static extern bool DestroyIcon(IntPtr hIcon);
        }
    }
}