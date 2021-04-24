using System;
using System.IO;
using System.Reflection;

namespace MutinyBot
{
    class Program
    {
        public static MutinyBot MutinyBot { get; private set; }
        static void Main(string[] args)
        {
            MutinyBot = new MutinyBot();
            MutinyBot.ConnectAsync().GetAwaiter().GetResult();
            //Console.ReadLine();
        }
        /* 
         * This function is here only to compare the methods of accessing directory on different OS'
         * Only Environment.CurrentDirectory and Directory.GetCurrentDirectory() point to the desired directory on Unix
         */
        static void PrintDirectories()
        {
            Console.WriteLine("AD.CurrentDomain.Base:   " + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Env.Current:             " + Environment.CurrentDirectory);
            Console.WriteLine("Dir.GetCurrent():        " + Directory.GetCurrentDirectory());
            Console.WriteLine("A.GetExecuting().Loc:    " + Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("A.GetExecuting().Code:   " + Assembly.GetExecutingAssembly().CodeBase);
            Console.WriteLine("A.GetEntry().Loc:        " + Assembly.GetEntryAssembly().Location);
        }
    }
}