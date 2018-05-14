﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9A92DD23C46F25E6B9428CB182C65EE6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows7.DesktopIntegration.LibraryManagerDemo;


namespace Windows7.DesktopIntegration.LibraryManagerDemo {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _openLibraryButton;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _createLibraryButton;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _addDirectoryToLibraryButton;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _removeDirectoryFromLibraryButton;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _openLibraryManageUIButton;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _setDefaultSaveFolderButton;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _deleteLibraryButton;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.ComboBox _setFolderTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox _pinnedToNavigationPaneCheckBox;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button _setLibraryIcon;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.Image _imageIcon;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBlock _textBlockName;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBlock _textBlockDefaultSaveDirectory;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\MainWindow.xaml"
        internal System.Windows.Controls.ListBox _foldersListBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LibraryManagerDemo;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this._openLibraryButton = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\MainWindow.xaml"
            this._openLibraryButton.Click += new System.Windows.RoutedEventHandler(this.OpenLibraryButtonClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this._createLibraryButton = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\MainWindow.xaml"
            this._createLibraryButton.Click += new System.Windows.RoutedEventHandler(this.CreateLibraryButtonClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this._addDirectoryToLibraryButton = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\MainWindow.xaml"
            this._addDirectoryToLibraryButton.Click += new System.Windows.RoutedEventHandler(this.AddDirectoryToLibraryClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this._removeDirectoryFromLibraryButton = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\MainWindow.xaml"
            this._removeDirectoryFromLibraryButton.Click += new System.Windows.RoutedEventHandler(this.RemoveDirectoryFromLibraryClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this._openLibraryManageUIButton = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\MainWindow.xaml"
            this._openLibraryManageUIButton.Click += new System.Windows.RoutedEventHandler(this.OpenLibraryManageUIClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this._setDefaultSaveFolderButton = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\MainWindow.xaml"
            this._setDefaultSaveFolderButton.Click += new System.Windows.RoutedEventHandler(this.SetDefaultSaveFolderClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this._deleteLibraryButton = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\MainWindow.xaml"
            this._deleteLibraryButton.Click += new System.Windows.RoutedEventHandler(this.DeleteLibraryButtonClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this._setFolderTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 9:
            this._pinnedToNavigationPaneCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this._setLibraryIcon = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\MainWindow.xaml"
            this._setLibraryIcon.Click += new System.Windows.RoutedEventHandler(this.SetLibraryIconClick);
            
            #line default
            #line hidden
            return;
            case 11:
            this._imageIcon = ((System.Windows.Controls.Image)(target));
            return;
            case 12:
            this._textBlockName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this._textBlockDefaultSaveDirectory = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this._foldersListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}