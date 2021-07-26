using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ToP.Bridge.Arguments;

namespace ToP.Bridge
{
    static class Program
    {
        static Main process;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            var keepAlive = true;
            ThreadPool.QueueUserWorkItem(MainProc);
            while (keepAlive)
            {
                var input = Console.ReadLine();
                switch (input.Split(' ')[0])
                {
                    case "exit":
                        process = null;
                        keepAlive = false;
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                    {
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        Type argType = Type.GetType($"ToP.Bridge.Arguments.{textInfo.ToTitleCase(input.Split(' ')[0])}Argument");
                        if (argType != null)
                        {
                            IArgument argInstance = (IArgument) Activator.CreateInstance(argType);
                            argInstance?.Process(process, Logger, input);
                        }
                        else Logger($"Command not found: {input.Split(' ')[0]}");
                    }
                    break;
                }
            }
        }

        public static void LogMain(Main context)
        {
            process = context;
        }

        public static void Logger(string message)
        {
            Console.WriteLine($@"[{DateTime.Now}] {message}");
        }

        static void MainProc(Object stateInfo)
        {
            process = new Main(LogMain, Logger);
            Task.Run(() => process.InitializeBridge());
        }
    }
}
