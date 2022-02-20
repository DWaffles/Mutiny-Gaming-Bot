using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutinyBot.Modules
{
    public static class CommandCategories
    {
        public const string Bot = "Bot";
        public const string Information = "Information";
        public const string Certification = "Certification";
        public const string Pets = "Pets";
        public const string Tags = "Tags";
        public const string Mutiny = "Mutiny";
        public const string Moderation = "Moderation";
        public const string OtherCommands = "Other Commands";
        public static List<string> CategoryOrder { get; } = new() { Bot, Information, Certification, Pets, Tags, Mutiny, Moderation, OtherCommands }; 
    }
}
