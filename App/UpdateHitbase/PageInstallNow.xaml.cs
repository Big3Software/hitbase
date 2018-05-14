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
using Ionic.Zip;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace UpdateHitbase
{
    /// <summary>
    /// Interaction logic for PageWelcome.xaml
    /// </summary>
    public partial class PageInstallNow : UserControl, IWizardPage
    {
        public PageInstallNow()
        {
            InitializeComponent();
            textBlockCurrentFile.Visibility = System.Windows.Visibility.Collapsed;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        string hitbaseDirectory;

        private void buttonInstallNow_Click(object sender, RoutedEventArgs e)
        {
            bool hitbaseRunning = Process.GetProcesses().Any(prc => prc.ProcessName.ToLower() == "hitbase");

            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Big 3\\Hitbase 2012"))
            {
                if (regKey != null)
                    hitbaseDirectory = regKey.GetValue("InstallLocation") as string;
            }

            if (string.IsNullOrEmpty(hitbaseDirectory))
            {
                MessageBox.Show("Die Hitbase 2012 Installation konnte nicht gefunden werden. Hitbase 2012 muss bereits auf dem System installiert sein, damit Sie dieses Update einspielen können.",
                    "Hitbase Update", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (hitbaseRunning)
            {
                MessageBox.Show("Hitbase wird aktuell noch ausgeführt. Bitte beenden Sie Hitbase und starten Sie dann den Update-Vorgang erneut.",
                    "Hitbase Update", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            textBlockCurrentFile.Visibility = System.Windows.Visibility.Visible;
            progressBar.Visibility = System.Windows.Visibility.Visible;
            buttonInstallNow.Visibility = System.Windows.Visibility.Collapsed;
            Commands.HideNavigationButtons.Execute(null, this);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                InstallNow();
            };
            bw.RunWorkerCompleted += delegate
            {
                Finish();
            };
            bw.RunWorkerAsync();
        }

        private void Finish()
        {
            Commands.NextPage.Execute(null, this);
        }

        private void InstallNow()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream reader = assembly.GetManifestResourceStream("UpdateHitbase.SetupFiles.hitbase.zip");

            using (ZipFile zip = ZipFile.Read(reader))
            {
                Dispatcher.Invoke((Action)delegate
                {
                    progressBar.Maximum = zip.Count;
                });

                foreach (var zipFile in zip)
                {
                    App.LogMessages.AppendLine("Copy " + zipFile.FileName + "...");

                    try
                    {
                        zipFile.Extract(hitbaseDirectory, ExtractExistingFileAction.OverwriteSilently | ExtractExistingFileAction.Throw);
                        App.LogMessages.AppendLine("OK");
                    }
                    catch (Exception e)
                    {
                        App.ErrorCount++;
                        App.LogMessages.AppendLine(e.ToString());
                    }

                    Thread.Sleep(100);
                    Dispatcher.Invoke((Action)delegate
                    {
                        textBlockCurrentFile.Text = zipFile.FileName;
                        progressBar.Value++;
                    });

                }
            }
        }

        public bool NextButtonDisabled
        {
            get { return true; }
        }
    }
}
