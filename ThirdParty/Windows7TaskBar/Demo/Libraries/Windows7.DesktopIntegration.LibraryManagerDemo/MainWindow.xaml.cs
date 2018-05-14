// Copyright (c) Microsoft Corporation.  All rights reserved.

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
using System.Globalization;
using System.Windows.Interop;
using Microsoft.SDK.Samples.VistaBridge.Library;

namespace Windows7.DesktopIntegration.LibraryManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibraryManager _libraryManager;

        /// <summary>
        /// <ain UI Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _libraryManager = new LibraryManager();
            _textBlockName.DataContext = _libraryManager;
            _textBlockDefaultSaveDirectory.DataContext = _libraryManager;
            _addDirectoryToLibraryButton.DataContext = _libraryManager;
            _removeDirectoryFromLibraryButton.DataContext = _libraryManager;
            _foldersListBox.DataContext = _libraryManager;
            _pinnedToNavigationPaneCheckBox.DataContext = _libraryManager;
            _imageIcon.DataContext = _libraryManager;
            _setFolderTypeComboBox.DataContext = _libraryManager;
        }

        private void OpenLibraryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOpenFileDialog pickLocationDialog = new CommonOpenFileDialog();
                pickLocationDialog.Title = "Pick a library";
                pickLocationDialog.ForceFileSystem = false;
                pickLocationDialog.FoldersOnly = true;
                pickLocationDialog.MultiSelect = false;

                CommonFileDialogResult result = pickLocationDialog.ShowDialog();
                if (result.Canceled)
                    return;

                _libraryManager.OpenShellLibrary(pickLocationDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateLibraryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOpenFileDialog pickLocationDialog = new CommonOpenFileDialog();
                pickLocationDialog.FoldersOnly = true;
                pickLocationDialog.Title = "Pick a location for the new library";
                pickLocationDialog.MultiSelect = false;
                pickLocationDialog.ForceFileSystem = false;

                CommonFileDialogResult result = pickLocationDialog.ShowDialog();
                if (result.Canceled)
                    return;

                WindowLibraryNamePickup libraryNamePickup = new WindowLibraryNamePickup();
                libraryNamePickup.ShowDialog();

                _libraryManager.CreateShellLibrary(pickLocationDialog.FileName, libraryNamePickup.LibraryName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddDirectoryToLibraryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOpenFileDialog pickLocationDialog = new CommonOpenFileDialog();
                pickLocationDialog.FoldersOnly = true;
                pickLocationDialog.Title = "Pick a directory to add to the library";
                pickLocationDialog.MultiSelect = false;
                pickLocationDialog.ForceFileSystem = true;

                CommonFileDialogResult result = pickLocationDialog.ShowDialog();
                if (result.Canceled)
                    return;

                _libraryManager.AddDirectory(pickLocationDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void RemoveDirectoryFromLibraryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _libraryManager.RemoveDirectory(_foldersListBox.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenLibraryManageUIClick(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowInteropHelper interopHelper = new WindowInteropHelper(this);
                _libraryManager.OpenLibraryManageUI(interopHelper.Handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteLibraryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _libraryManager.DeleteLibrary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void SetDefaultSaveFolderClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _libraryManager.DefaultSaveFolder = _foldersListBox.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetLibraryIconClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectIconWindow selectIconWindow = new SelectIconWindow();
                selectIconWindow.ShowDialog();

                _libraryManager.SetIcon(selectIconWindow.IconId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }    
    }

    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}