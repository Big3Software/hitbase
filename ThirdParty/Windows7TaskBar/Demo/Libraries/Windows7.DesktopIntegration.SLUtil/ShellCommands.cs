// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Library.KnownFolders;
using System.IO;
using Windows7.DesktopIntegration;

namespace Windows7.DesktopIntegration.ShellLibraryDemo
{
    static class ShellCommands
    {
        [Command(
            Name = "Create",
            Usage = "SLUtil Create LibraryName",
            Info = "Create a new library",
            Example = "SLUtil Create MyLib")]
        public static void CreateLibrary(string name)
        {
            using (ShellLibrary library = ShellLibrary.Create(name, true))
            {
            }
        }

        [Command(
            Name = "AddFolder",
            Usage = "SLUtil AddFolder LibraryName FolderPath",
            Info = "Add a folder to a library",
            Example = @"SLUtil AddFolder Documents C:\Docs")]
        public static void AddFolder(string name, string folderPath)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                library.AddFolder(folderPath);
            }
        }

        [Command(
            Name = "RemoveFolder",
            Usage = "SLUtil RemoveFolder LibraryName FolderPath",
            Info = "Remove a folder from a library",
            Example = @"SLUtil RemoveFolder Documents C:\Docs")]
        public static void RemoveFolder(string name, string folderPath)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                library.RemoveFolder(folderPath);
            }
        }

        [Command(
            Name = "Delete",
            Usage = "SLUtil Delete LibraryName",
            Info = "Delete a library",
            Example = "SLUtil Delete MyLib")]
        public static void DeleteLibrary(string name)
        {
            ShellLibrary.Delete(ShellLibrary.CreateLibraryFullName(name));
        }

        [Command(
            Name = "Rename",
            Usage = "SLUtil Rename OldName NewName",
            Info = "Rename a library",
            Example = "SLUtil Rename MyLib MyLibNewName")]
        public static void RenameLibrary(string oldName, string newName)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(oldName), true))
            {
                library.Name = newName;
            }
        }


        [Command(
           Name = "SaveFolder",
           Usage = "SLUtil SaveFolder LibraryName [FolderPath]",
           Info = "Set or get the library's save folder path",
           Example = @"SLUtil SaveFolder Documents C:\Docs")]
        public static void SaveFolder(string name, string folderPath)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    Console.WriteLine("Save folder: {0}", library.DefaultSaveFolder);
                }
                else
                {
                    library.DefaultSaveFolder = folderPath;
                }
            }
        }

        [Command(
           Name = "NavPanePinnedState",
           Usage = "SLUtil NavPanePinnedState LibraryName [TRUE|FALSE]",
           Info = "Set or get the library's Pinned to navigation pane state",
           Example = @"SLUtil NavPanePinnedState MyLib TRUE")]
        public static void NavPanePinnedState(string name, string stateText)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                bool state;

                if (bool.TryParse(stateText, out state))
                {
                    library.IsPinnedToNavigationPane = state;
                }
                else
                {
                    Console.WriteLine("The library {0} is{1}pinned to the navigation pane.", name, library.IsPinnedToNavigationPane ? " " : " not ");
                }
            }
        }

        [Command(
            Name = "Icon",
            Usage = "SLUtil Icon LibraryName [Icon]",
            Info = "Set or get the library's icon",
            Example = @"SLUtil Icon MyLib imageres.dll,-1005")]
        public static void Icon(string name, string icon)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                if (string.IsNullOrEmpty(icon))
                {
                    Console.WriteLine("Icon: {0}", library.Icon);
                }
                else
                {
                    library.Icon = icon;
                }
            }
        }

        [Command(
           Name = "FolderType",
           Usage = "SLUtil FolderType LibraryName [Documents|Pictures|Music|Videos]",
           Info = "Set or get the library's folder template",
           Example = @"SLUtil MyLib Documents")]
        public static void FolderType(string name, string folderType)
        {
            using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
            {
                if (string.IsNullOrEmpty(folderType))
                {
                    Guid folderTypeId = library.FolderTypeId;
                    string folderTypeName = folderTypeId.ToString();
                    try
                    {
                        folderTypeName = FolderTypes.GetFolderType(folderTypeId);
                    }
                    catch
                    {
                    }
                    Console.WriteLine("Folder type: {0}", folderTypeName);
                }
                else
                {
                    Guid folderTypeId;
                    try
                    {
                        folderTypeId = FolderTypes.GetFolderType(folderType);
                    }
                    catch
                    {
                        folderTypeId = new Guid(folderType);
                    }
                    library.FolderTypeId = folderTypeId;
                }
            }
        }

        [Command(
            Name = "ShowInfo",
            Usage = "SLUtil ShowInfo [LibraryName]",
            Info = "Show Library information. If the LibraryName parameter is missing, show information of all libraries under the Library folder",
            Example = @"SLUtil ShowInfo Documents")]
        public static void ShowInfo(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                foreach (string libraryName in Directory.GetFiles(KnownFolders.Libraries))
                {
                    try
                    {
                        using (ShellLibrary library = ShellLibrary.Load(libraryName, true))
                        {
                            ShowInformation(library);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(libraryName + " is probably not a library, Error: " + ex.Message);
                    }
                }
            }
            else
            {
                using (ShellLibrary library = ShellLibrary.Load(ShellLibrary.CreateLibraryFullName(name), true))
                {
                    ShowInformation(library);
                }
            }
        }

        private static void ShowInformation(ShellLibrary library)
        {
            Console.WriteLine("\nShowing information of {0} library", library.Name);
            Console.WriteLine("\tLibrary path: {0}", library.FullName);
            Console.WriteLine("\tIs pinned to navigation pane: {0}", library.IsPinnedToNavigationPane);
            string saveFolder = library.DefaultSaveFolder;
            Console.WriteLine("\tSave folder: {0}", saveFolder);
            Console.WriteLine("\tIcon: {0}", library.Icon);
            Guid folderTypeId = library.FolderTypeId;
            string folderTypeName = folderTypeId.ToString();
            try
            {
                folderTypeName = FolderTypes.GetFolderType(folderTypeId);
            }
            catch
            {
            }
            Console.WriteLine("\tFolder type: {0}", folderTypeName);

            Console.WriteLine("\tFolder list:");
            foreach (string folder in library.GetFolders())
            {
                Console.WriteLine("\t\t{0} {1}", folder, saveFolder == folder ? "*" : "");
            }
        }

        [Command(
            Name = "ManageUI",
            Usage = "SLUtil ManageUI LibraryName",
            Info = "Show the Shell Library management UI",
            Example = @"SLUtil ManageUI Documents")]
        public static void ManageUI(string name)
        {
            ShellLibrary.ShowManageLibraryUI(ShellLibrary.CreateLibraryFullName(name), IntPtr.Zero, null, null, true);
        }
    }
}