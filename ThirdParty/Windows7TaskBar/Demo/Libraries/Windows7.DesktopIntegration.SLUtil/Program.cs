// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Windows7.DesktopIntegration.ShellLibraryDemo
{
    class Program
    {
        static int Main(string[] args)
        {
            CommandLineInterpreter cli = new CommandLineInterpreter(args);
            return cli.Execute();
        }
    }
 }