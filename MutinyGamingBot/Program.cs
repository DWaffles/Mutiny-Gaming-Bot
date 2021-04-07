using System;
using System.IO;

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
    }
}