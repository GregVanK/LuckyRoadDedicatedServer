using System;
using System.Collections.Generic;
using System.Text;

namespace LuckyRoadDedicatedServer
{
    class Logger
    {
        //write good messages to the console :)
        public static void Info(string message) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] [INFO] {1}", Date(), message);
            Console.ResetColor();
        }

        //write warning messages
        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[{0}] [WARN] {1}", Date(), message);
            Console.ResetColor();
        }

        //write bad messages to the console :(
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[{0}] [ERROR] {1}", Date(), message);
            Console.ResetColor();
        }

        //write debug messages
        public static void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[{0}] [DEBUG] {1}", Date(), message);
            Console.ResetColor();
        }

        //get the date
        public static string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}
