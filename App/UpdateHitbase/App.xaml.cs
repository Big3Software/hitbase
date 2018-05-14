using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Text;

namespace UpdateHitbase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int ErrorCount = 0;
        public static StringBuilder LogMessages = new StringBuilder();

        HookResolver hookResolver = new HookResolver();
        public App()
        {
            DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception exception = e.Exception as Exception;
            MessageBox.Show(exception.ToString());
            e.Handled = true;            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            MessageBox.Show(exception.ToString());
/*            if (exception != null)
            {
                UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(exception);

                unhandledExceptionWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unhandled exception occured.");
            }*/
        }


    }

    public class HookResolver
    {
        Dictionary<string, Assembly> _loaded;

        public HookResolver()
        {
            _loaded = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Split(',')[0];
            Assembly asm = null;
            lock (_loaded)
            {
                try
                {
                    if (!_loaded.TryGetValue(name, out asm))
                    {
                        if (name.IndexOf("Ionic") >= 0)
                            name = "UpdateHitbase.Ionic.Zip.Reduced.dll";
                        using (Stream io = this.GetType().Assembly.GetManifestResourceStream(name))
                        {
                            byte[] bytes = new BinaryReader(io).ReadBytes((int)io.Length);
                            asm = Assembly.Load(bytes);
                            _loaded.Add(name, asm);
                        }
                    }
                }
                catch
                {

                }
            }
            return asm;
        }
    }
}
