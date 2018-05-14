using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.Deployment.WindowsInstaller;

namespace HitbaseInstaller
{
    [RunInstaller(true)]
    public partial class HitbaseInstaller : Installer
    {
        public HitbaseInstaller()
        {
            InitializeComponent();
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string args = this.Context.Parameters["args"];

            if (string.IsNullOrEmpty(args))
            {
                throw new InstallException("no arguments specified");
            }

            // Gets the path to the Framework directory.
            string path = RuntimeEnvironment.GetRuntimeDirectory();

            Process p;
            // Quotes the arguments, in case they have a space in them.
            ProcessStartInfo si = new ProcessStartInfo(path + "ngen.exe", "\"" + args + "\"");
            si.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                p = Process.Start(si);
                p.WaitForExit();
            }
            catch (Exception e)
            {
                throw new InstallException(e.Message);
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
        }
    }
}