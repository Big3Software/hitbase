// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Windows7.DesktopIntegration.ShellLibraryDemo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Usage { get; set; }
        public string Info { get; set; }
        public string Example { get; set; }
    }

    class CommandLineInterpreter
    {
        class CommandInfo
        {
            private readonly CommandAttribute _data;
            private readonly MethodInfo _function;

            public CommandInfo(CommandAttribute commandAttribute, MethodInfo function)
            {
                _data = commandAttribute;
                _function = function;
            }

            public void Execute(object[] parameters)
            {
                object[] callParameters = parameters;
                int numberOfParameters = _function.GetParameters().Length;
 
                if (parameters.Length < numberOfParameters)
                {
                    callParameters = new object[numberOfParameters];
                    parameters.CopyTo(callParameters, 0);
                }

                _function.Invoke(null, callParameters);
            }

            public string Name
            {
                get
                {
                    return _data.Name;
                }
            }

            public void DisplayUsage()
            {
                Console.WriteLine(_data.Usage);
            }

            public void DisplayInfo()
            {
                Console.WriteLine(_data.Info);
            }

            public void DisplayExample()
            {
                Console.WriteLine(_data.Example);
            }

            public void DisplayHelpInformation()
            {
                Console.WriteLine("Help for " + _data.Name + ":");
                DisplayInfo();
                Console.WriteLine();
                DisplayUsage();
                Console.WriteLine("\nExample:");
                DisplayExample();
                Console.WriteLine("\n");
            }
                
        }


        [Command(
            Name = "?",
            Usage = "SLUtil ? [CommandName]",
            Info = "Show SLUtil help",
            Example = "SLUtil ? Create")]
        public static void ShowHelp(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                CommandInfo[] commands = GetAllCommands();
                foreach (CommandInfo commandInfo in commands)
                {
                    commandInfo.DisplayUsage();
                }
            }
            else
            {
                CommandInfo command = FindCommand(commandName);
                command.DisplayHelpInformation();
            }
        }

        private string[] _arguments;

        public CommandLineInterpreter(string[] arguments)
        {
            _arguments = arguments;
        }

        public int Execute()
        {
            try
            {
                CommandInfo commandInfo = FindCommand(_arguments[0]);
                try
                {
                    commandInfo.Execute(_arguments.Skip(1).ToArray());
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex = ex.InnerException;

                    Console.WriteLine("{0} execution failed, Error: {1}", commandInfo.Name, ex.Message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Bad command, ? for command list");
                return -1;
            }
            return 0;
        }

        private static CommandInfo FindCommand(string commandName)
        {
            CommandInfo commandInfo = (from Type type in Assembly.GetExecutingAssembly().GetTypes()
                    from MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    let commandAttribute = methodInfo.GetCustomAttributes(typeof(CommandAttribute), false)
                    where commandAttribute.Length == 1 && (commandAttribute[0] as CommandAttribute).Name.StartsWith(commandName, StringComparison.InvariantCultureIgnoreCase)
                    select new CommandInfo(commandAttribute[0] as CommandAttribute, methodInfo)).First();
            return commandInfo;
        }

        private static CommandInfo[] GetAllCommands()
        {
            CommandInfo [] commandsInfo = (from Type type in Assembly.GetExecutingAssembly().GetTypes()
                                       from MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                       let commandAttribute = methodInfo.GetCustomAttributes(typeof(CommandAttribute), false)
                                       where commandAttribute.Length == 1
                                       select new CommandInfo(commandAttribute[0] as CommandAttribute, methodInfo)).ToArray();
            return commandsInfo;
        }
    }
}