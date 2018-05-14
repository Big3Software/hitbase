using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Big3.Hitbase.SharedResources.Themes
{
    public class ThemeManager
    {
        private static System.IO.FileSystemWatcher themeFileWatcher = new System.IO.FileSystemWatcher();

        static ThemeManager()
        {
            themeFileWatcher.Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Themes");
            themeFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            themeFileWatcher.Filter = "*.*";

            themeFileWatcher.Changed += new System.IO.FileSystemEventHandler(fsw_Changed);

            themeFileWatcher.IncludeSubdirectories = true;
            themeFileWatcher.EnableRaisingEvents = true;
        }

        private static string currentTheme = "";
        public static string CurrentTheme
        {
            get
            {
                return currentTheme;
            }
        }

        public static string[] GetAvailableThemes()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Themes");

            DirectoryInfo di = new DirectoryInfo(path);

            DirectoryInfo[] subDirs = di.GetDirectories();

            List<string> themeDirs = new List<string>();
            foreach (DirectoryInfo subDir in subDirs)
            {
                themeDirs.Add(subDir.Name);
            }

            return themeDirs.ToArray();
        }

        private static object themeLock = new object();

        public static void Initialize(string theme)
        {
            SetTheme(theme);
        }

        public static void SetTheme(string theme)
        {
            lock (themeLock)
            {
                if (currentTheme == theme)
                    return;

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Themes");

                string skinUri = string.Format(@"{0}\{1}\Theme.xaml", path, theme);

                Uri uri = new Uri(skinUri, UriKind.Relative);

                FileStream fs = new FileStream(skinUri, FileMode.Open, FileAccess.Read);

                System.Windows.Markup.ParserContext pc = new System.Windows.Markup.ParserContext();
                pc.BaseUri = new Uri(path + "\\" + theme + "\\");
                ResourceDictionary rd = System.Windows.Markup.XamlReader.Load(fs, pc) as ResourceDictionary;

                fs.Close();

                int foundIndex = -1;
                int index = 0;
                foreach (ResourceDictionary resDict in System.Windows.Application.Current.Resources.MergedDictionaries)
                {
                    if (resDict.Source != null && resDict.Source.OriginalString.Contains("Theme.xaml"))
                    {
                        foundIndex = index;
                        break;
                    }

                    index++;
                }

                // OK, wir gehen hier davon aus, dass das Theme Resource Dictionary immer an erster Stelle steht!
                if (foundIndex == -1)
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd);
                else
                    System.Windows.Application.Current.Resources.MergedDictionaries[foundIndex] = rd;

                currentTheme = theme;
            }
        }

        static void fsw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            themeFileWatcher.EnableRaisingEvents = false;
            int pos = e.Name.IndexOf('\\');
            if (pos >= 0)
            {
                string theme = e.Name.Substring(0, pos);

                // Aktuelles Theme hat sich geändert?
                if (String.Compare(theme, currentTheme, true) == 0)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        System.Threading.Thread.Sleep(500);
                        SetTheme(theme);
                    });
                }
            }
            themeFileWatcher.EnableRaisingEvents = true;
        }
    }
}
