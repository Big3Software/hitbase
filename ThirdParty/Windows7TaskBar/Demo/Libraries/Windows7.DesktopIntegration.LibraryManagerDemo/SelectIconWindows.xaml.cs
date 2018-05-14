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
using Microsoft.SDK.Samples.VistaBridge.Library.StockIcons;

namespace Windows7.DesktopIntegration.LibraryManagerDemo
{
    /// <summary>
    /// Interaction logic for SelectIcon.xaml
    /// </summary>
    public partial class SelectIconWindow : Window
    {
        private int _iconId;

        public SelectIconWindow()
        {
            InitializeComponent();
            InsertIcons();
        }

        public int IconId
        {
            get
            {
                return _iconId;
            }
        }

        private void InsertIcons()
        {
            foreach (StockIconIdentifier iconId in Enum.GetValues(typeof(StockIconIdentifier)))
            {
                Image image = new Image();
                image.Source = StockIcons.GetBitmapSource((StockIconIdentifier)iconId);
                Button button = new Button();
                button.Content = image;
                button.Name = iconId.ToString();
                button.Click += (s, e) =>
                {
                    _iconId = (int)Enum.Parse(typeof(StockIconIdentifier), ((Button)e.Source).Name);
                    Close();
                };
                _wrapPanel.Children.Add(button);
            }
        }
    }
}