using System;
using System.IO;

namespace MutinyBot
{
    class Program
    {
        public static MutinyBot MutinyBot { get; private set; }
        static void Main(string[] args)
        {
            //PrintDirectories();
            MutinyBot = new MutinyBot();
            MutinyBot.ConnectAsync().GetAwaiter().GetResult();
            //Console.ReadLine();
        }
        static void PrintDirectories()
        {
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(Environment.CurrentDirectory);
            Console.WriteLine(Directory.GetCurrentDirectory());
            //Console.WriteLine(Assembly.GetExecutingAssembly().Location);
            //Console.WriteLine(Assembly.GetExecutingAssembly().CodeBase);
            //Console.WriteLine(Assembly.GetEntryAssembly().Location);
        }
    }
}