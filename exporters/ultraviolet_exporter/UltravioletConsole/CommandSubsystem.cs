using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltravioletConsole.Commands
{
    public delegate object[] ArgumentInterpreter(string rawArgs);
    public delegate CommandReturnInfo CommandExecutor(object[] args);
    public delegate void CommandExecutedEventArgs(CommandReturnInfo returnInfo);

    public enum ExitReason { User, System, Error }

    public class ExitEventArgs : EventArgs
    {
        ExitReason Reason { get; }

        public ExitEventArgs(ExitReason reason) { Reason = reason; }
    }
    public delegate void ExitEventHandler(object sender, ExitEventArgs e);

    public struct CommandReturnInfo
    {
        public readonly bool Success;
        public readonly string ReturnString;
        public readonly object ReturnValue;

        public CommandReturnInfo(bool success, string returnString, object returnValue)
        {
            Success = success;
            ReturnString = returnString;
            ReturnValue = returnValue;
        }
    }


    public class UVCommand
    {
        public class HelpInfo
        {
            /// <summary>
            /// Key is argument name, Value is argument information
            /// </summary>
            private Dictionary<string, string> Arguments = new Dictionary<string, string>();

            public string Description;
            public string Remarks;
            public List<string> Examples;

            public void AddArgumentInfo(string name, string info)
            {
                Arguments.Add(name, info);
            }

            public string Help
            {
                get
                {
                    string helpInfo;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(stream);
                        writer.WriteLine(Description + "\n");
                        if (Arguments.Count != 0)
                        {
                            writer.WriteLine();
                            foreach (var arg in Arguments)
                            {
                                writer.WriteLine("\t" + arg.Key + ": " + arg.Value);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Remarks))
                        {
                            writer.WriteLine();
                            writer.WriteLine("Remarks: " + Remarks);
                        }
                        if (Examples != null && Examples.Count != 0)
                        {
                            writer.WriteLine();
                            writer.WriteLine("Examples:");
                            foreach (var example in Examples)
                            {
                                writer.WriteLine(example);
                            } 
                        }

                        writer.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(stream);
                        helpInfo = reader.ReadToEnd();
                        writer.Dispose();
                        reader.Dispose();
                    }
                    return helpInfo.Replace("\n\r\n", "\n");

                }
            }
        }

        private HelpInfo helpInfo;

        public string Name;
        public ArgumentInterpreter Interpreter;
        public CommandExecutor Executor;
        public string HelpMessage { get => helpInfo.Help; }

        public UVCommand(string name, ArgumentInterpreter interpreter, CommandExecutor executor, HelpInfo info = null)
        {
            Name = name;
            Interpreter = interpreter;
            Executor = executor;
            helpInfo = info;
        }
    }


    public class CommandSubsystem
    {
        public enum InterpretMode
        {
            DEFAULT,
            IGNORE_QUOTES
        }

        public event ExitEventHandler SubsystemExit;
        public void Exit(ExitReason reason)
        {
            SubsystemExit.Invoke(this, new ExitEventArgs(reason));
        }

        private List<UVCommand> commands = new List<UVCommand>();

        private CommandSubsystem() { }

        public static CommandSubsystem Create()
        {
            CommandSubsystem subsystem = new CommandSubsystem();
            CommandExecutor exitExecutor = (object[] args) =>
            {
                subsystem.SubsystemExit.Invoke(subsystem, new ExitEventArgs(ExitReason.User));
                return new CommandReturnInfo(true, "Exiting Ultraviolet Console...", null);
            };
            ArgumentInterpreter exitInterpreter = (string args) =>
            {
                return null;
            };
            subsystem.commands.Add(new UVCommand("exit", exitInterpreter, exitExecutor, new UVCommand.HelpInfo { Description = "Exit: Exits Ultraviolet Console."}));

            CommandExecutor helpExecutor = (object[] args) =>
            {
                if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace((string)args[0]))
                {
                    Console.WriteLine("Listing all commands...\n");
                    bool nextLine = false;
                    foreach (string commandName in subsystem.commands.ConvertAll(x => x.Name))
                    {
                        Console.Write(("\t" + commandName) + ((nextLine) ? "\n" : "\t"));
                        nextLine = !nextLine;
                    }
                }
                else
                {
                    UVCommand command = subsystem.commands.Find(x => x.Name.ToLower() == (args[0] as string).ToLower());
                    if(command == null)
                    {
                        Console.WriteLine("ERROR: Command \"" + args[0] + "\" not found. Type \"help\" to see a list of commands");
                    }
                    else
                    {
                        Console.WriteLine(command.HelpMessage);
                    }
                }
                return new CommandReturnInfo(true, null, null);
            };
            ArgumentInterpreter helpInterpreter = (string args) =>
            {
                try
                {
                    return new object[] { args.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First() };
                }
                catch (Exception)
                {
                    return new object[] { string.Empty };
                }
            };

            UVCommand.HelpInfo helpHelpInfo = new UVCommand.HelpInfo
            {
                Description = "Help [command] displays information about one command, or lists all commands",
                Examples = new List<string> { "help exit" }
            };
            helpHelpInfo.AddArgumentInfo("command", "The command to display information about. If no command is given, the command will list all available commands.");

            subsystem.commands.Add(new UVCommand("help", helpInterpreter, helpExecutor, helpHelpInfo));
            return subsystem;
        }

        public static string[] SplitArgs(string arguments, InterpretMode interpret = InterpretMode.DEFAULT)
        {
            if(arguments.Count(x => x == '\"') == 0 || interpret == InterpretMode.IGNORE_QUOTES)
            {
                return arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<Tuple<int, int>> quoteIndices = new List<Tuple<int, int>>();
                int activeSingle = -1, activeDouble = -1;
                int i = 0;
                foreach(char c in arguments)
                {
                    if (c == '\'')
                    {
                        if(activeSingle != -1)
                        {
                            quoteIndices[activeSingle] = new Tuple<int, int>(quoteIndices[activeSingle].Item1, i);
                            activeSingle = -1;
                        }
                        else
                        {
                            activeSingle = quoteIndices.Count;
                            quoteIndices.Add(new Tuple<int, int>(i, -1));
                        }
                    }
                    else if (c == '\"')
                    {

                    }
                    i++;
                }
            }

        }

        /// <summary>
        /// Gets a command of the given name
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public UVCommand GetCommand(string commandName)
        {
            if (commands.Count(x => x.Name.ToLower() == commandName) > 0)
            {
                return commands.First(x => x.Name == commandName);
            }
            else
                return null;
        }

        public void DoCommand(string command)
        {
            try
            {
                command = command + " ";
                var uvCommand = GetCommand(command.Substring(0, command.IndexOf(' ')));
                var commandResult = uvCommand.Executor((uvCommand.Interpreter(command.Substring(command.IndexOf(' '))));

                if (!commandResult.Success)
                    Console.WriteLine(commandResult.ReturnString);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error: Command not found. Type \'help\' to see a list of commands.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: An unknown exception was thrown.");
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }
        }
    }
}
