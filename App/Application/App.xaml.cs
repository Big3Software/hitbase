using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Text;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Globalization;
using Big3.Hitbase.SharedResources.Themes;
using System.Threading;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Miscellaneous;
using System.IO;

namespace Big3.Hitbase.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /////////////////////////
        // Das hier bei jeder Version anpassen bzw. prüfen!!
        public static bool IsBetaVersion = false;

        public static DateTime BetaExpirationDate = new DateTime(2012, 5, 1);

        public static string VersionString = "RC 2";
        /////////////////////////

        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        }
        
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(e.Exception);

            unhandledExceptionWindow.ShowDialog();
            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        public static SplashScreenWindow SplashScreen = null;

        public bool ThisTimeNoSplashScreen = false;

        public static string FileToOpen = "";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (App.IsBetaVersion)
            {
                if (DateTime.Now >= App.BetaExpirationDate)
                {
                    MessageBox.Show(StringTable.BetaVersionExpired, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    System.Windows.Application.Current.Shutdown();
                }
            }

            /*Thread thread = new Thread(() =>
            {
                SplashScreen = new SplashScreenWindow();
                SplashScreen.Show();

                SplashScreen.Closed += delegate
                {
                 SplashScreen.Dispatcher.InvokeShutdown();
                };

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();*/

            /*SplashScreen = new SplashScreen("Images/Splashscreen.jpg");
            SplashScreen.Show(true);*/

            ParseCommandLine();

            if ((Settings.Current.ShowSplashScreen && !ThisTimeNoSplashScreen) || !Register.GetSerialActivation())
            {
                SplashScreen = new SplashScreenWindow();
                SplashScreen.Show();
            }

            // Muss ich hier wieder auf null setzen:
            // When you create a Window object, it checks to see if Application.Current is non-null, 
            // and if so, it checks to see if MainWindow is null. If it's null, it will set itself as
            // the MainWindow. This ensures that the first Window created is the main window (standard
            // and expected behavior).

            this.MainWindow = null;            

            switch (Settings.Current.CurrentColorStyle)
            {
                case ColorStyle.Default:
                    ThemeManager.Initialize("Default");
                    break;
                case ColorStyle.Black:
                    ThemeManager.Initialize("Black");
                    break;
                case ColorStyle.Silver:
                    ThemeManager.Initialize("Silver");
                    break;
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Für "billige" Linux-Proxies.
            System.Net.ServicePointManager.Expect100Continue = false; 
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(exception);

                unhandledExceptionWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unhandled exception occured.");
            }
        }

        private void ParseCommandLine()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 1)
            {
                for (int i = 1; i < commandLineArgs.Length; i++)
                {
                    string command = commandLineArgs[i];
                    string commandSwitch = "";

                    if (command.StartsWith("-") || command.StartsWith("/"))
                    {
                        commandSwitch = command.Mid(1);
                        if (commandSwitch == "NOSPLASHSCREEN")
                        {
                            ThisTimeNoSplashScreen = true;
                            continue;
                        }

                        continue;
                    }

                    // Kleiner Windows/.Net Konzept Bug Fix:
                    if (command.Length == 3)            // z.b. K:"
                    {
                        command = command.Left(2);
                    }

                    if (Path.GetExtension(command).ToLower() == ".hdbx" || Path.GetExtension(command).ToLower() == ".hdb")
                    {
                        Settings.Current.LastDataBase = command;
                    }
                    else
                    {
                        FileToOpen = command;
                    }
                }
            }

        }


    }
}
