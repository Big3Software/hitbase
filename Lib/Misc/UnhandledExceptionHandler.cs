using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Big3.Hitbase.Miscellaneous
{
    public class UnhandledExceptionHandler
    {
        public UnhandledExceptionHandler()
        {
        }

        public static void LogUnhandledException(Exception e)
        {
            // Die unhandled Exception wird im Tempverzeichnis des Users abgelegt
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "HitbaseCrashLog.txt"), e.ToString());
        }

        /// <summary>
        /// Prüft, ob Hitbase beim letzten Mal abgestürzt ist. Im Temp-Verzeichnis
        /// ist dann ein CrashLog verfügbar.
        /// </summary>
        public static void TestForCrashLog()
        {
            string crashLogFilename = Path.Combine(Path.GetTempPath(), "HitbaseCrashLog.txt");
            if (File.Exists(crashLogFilename))
            {
                FormShowCrashLog formShowCrashLog = new FormShowCrashLog();
                formShowCrashLog.Details = File.ReadAllText(crashLogFilename);
                formShowCrashLog.ShowDialog();
                File.Delete(crashLogFilename);
            }
        }
    }
}
