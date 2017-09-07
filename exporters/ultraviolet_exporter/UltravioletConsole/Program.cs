using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltravioletConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            Commands.ExitEventHandler exitEvent = (object sender, Commands.ExitEventArgs e) =>
            {
                running = false;
            };
            Console.WriteLine("Ultraviolet Console v0.1.0.0");
            Commands.CommandSubsystem UVSubsystem = Commands.CommandSubsystem.Create();
            UVSubsystem.SubsystemExit += exitEvent;
            while(running)
            {
                Console.Write("UVSubsystem $ ");
                UVSubsystem.DoCommand(Console.ReadLine());
            }
        }
    }
}
