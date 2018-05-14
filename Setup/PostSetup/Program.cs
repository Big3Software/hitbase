using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace PostSetup
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1 || args[0] != "-setup")
                return 0;

            RegistryKey regInstallLocation = Registry.LocalMachine.OpenSubKey("Software\\Big 3\\Hitbase 2012");

            string installationDirectory = (string)regInstallLocation.GetValue("InstallLocation");

            regInstallLocation.Close();

            // Gets the path to the Framework directory.
            string path = RuntimeEnvironment.GetRuntimeDirectory();

            string hitbasePath = Path.Combine(installationDirectory, "hitbase.exe");

            Process p;
            // Quotes the arguments, in case they have a space in them.
            ProcessStartInfo si = new ProcessStartInfo(path + "ngen.exe", "\"" + hitbasePath + "\"");
            si.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                p = Process.Start(si);
                p.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            // Installationsdatum in die Registry schreiben
            RegistryKey reg = Registry.ClassesRoot.CreateSubKey(".hdbx");

            int TimeBuf = DateTime.Now.Year * 365 + DateTime.Now.Month * 31 + DateTime.Now.Day;

            // Dieser Key muss bei jeder Hitbase-Version eindeutig neu vergeben werden, damit
            // nicht das Datum einer alten Shareware-Version erkannr wird.
            // Bisher:
            // Hitbase 2012: PrintEx
            // Hitbase 2010: OpenEx
            // Hitbase 2007: MoveEx
            // Hitbase 2005: CopyEx
            // Hitbase 2003: send
            // Hitbase 2001: print

            if (reg.GetValue("PrintEx") == null)
                reg.SetValue("PrintEx", TimeBuf);

            reg.Close();

            return 0;
        }
    }
}
