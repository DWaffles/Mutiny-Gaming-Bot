using MutinyBot.Common;
using System;
using System.IO;
using System.Reflection;

namespace MutinyBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = FileHandler.ReadConfig(Path.Combine("data", "config.json"));
            MutinyBot mutinyBot = new(config);
            mutinyBot.ConnectAsync().GetAwaiter().GetResult();
        }
        /* 
         * This function is here only to compare the methods of accessing directory on different OS's
         * Only Environment.CurrentDirectory and Directory.GetCurrentDirectory() point to the desired directory on Unix
         */
        /// <summary>
        /// Function to print various methods of getting directories to console.
        /// </summary>
        /// <remarks>Debug function due to differences in Unix/Windows systems.</remarks>
        static void PrintDirectories()
        {
            Console.WriteLine("AD.CurrentDomain.Base:   " + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Env.Current:             " + Environment.CurrentDirectory);
            Console.WriteLine("Dir.GetCurrent():        " + Directory.GetCurrentDirectory());
            Console.WriteLine("A.GetExecuting().Loc:    " + Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("A.GetEntry().Loc:        " + Assembly.GetEntryAssembly().Location);
        }
    }
}