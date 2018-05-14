// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Xml;
using Microsoft.SDK.Samples.VistaBridge.Library;

namespace BreadcrumbBarDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            
        }

        private void TheBreadcrumb_ItemSelected(object sender, EventArgs e)
        {
            XmlNode OriginalElement = sender as XmlNode;

            // Handle item selected
        }

        private void TheBreadcrumb_PopulateItem(object sender, EventArgs e)
        {
            XmlNode OriginalElement = sender as XmlNode;
            
            if (OriginalElement == null)
                return;

            // Generate Path
            XmlNode CurrentNode = OriginalElement as XmlNode;
            string currentPath = CurrentNode.Attributes["Name"].Value;
            
            while(CurrentNode.ParentNode is XmlElement)
            {
                CurrentNode = CurrentNode.ParentNode;
                string pathPrefix = CurrentNode.Attributes["Name"].Value.Trim('\\');

                if (pathPrefix.Contains(':'))
                    pathPrefix = pathPrefix + '\\';
                                
                currentPath = Path.Combine(pathPrefix, currentPath);                
            }

            currentPath += '\\';

            try
            {
                foreach (string directory in Directory.GetDirectories(currentPath.ToString()))
                {
                    XmlElement NewElement = OriginalElement.OwnerDocument.CreateElement("Folder");
                    NewElement.SetAttribute("Name", directory.Substring(directory.LastIndexOf('\\') + 1));

                    OriginalElement.AppendChild(NewElement);
                }
            }
            catch(System.UnauthorizedAccessException)
            {
                MessageBox.Show("Access to this path is denied: Unable to add subfolders.");
            }
        }
    }
}