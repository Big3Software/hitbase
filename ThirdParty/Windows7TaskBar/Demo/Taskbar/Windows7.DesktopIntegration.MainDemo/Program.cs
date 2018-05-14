// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Windows7.DesktopIntegration.MainDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.CommandLine.Contains("/doc"))
            {
                MessageBox.Show(Environment.CommandLine, "Launched with command line");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Windows7Form());
        }
    }
}