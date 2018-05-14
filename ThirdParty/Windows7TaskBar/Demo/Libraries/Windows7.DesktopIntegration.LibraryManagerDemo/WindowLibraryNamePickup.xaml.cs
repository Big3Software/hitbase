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
using System.Windows.Shapes;
using System.Globalization;

namespace Windows7.DesktopIntegration.LibraryManagerDemo
{
    /// <summary>
    /// Interaction logic for WindowLibraryNamePickup.xaml
    /// </summary>
    public partial class WindowLibraryNamePickup : Window
    {
        public WindowLibraryNamePickup()
        {
            InitializeComponent();
            _textBoxName.DataContext = this;

        }

        private void LibraryNameSetButton(object sender, RoutedEventArgs e)
        {
           Close();
        }

      

       public string LibraryName { get; set; }

      
    }

    public class TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string s = value as string;
                return s.Length != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}