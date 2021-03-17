using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MutinyBot.Common
{
    public class Configuration
    {
        [JsonProperty("Token")]
        public string Token { get; private set; }

        [JsonProperty("prefixes")]
        public string[] CommandPrefixes { get; private set; }

        [JsonProperty("HexCode")]
        public string HexCode { get; private set; } = "47200e";

        [JsonProperty("Debug")]
        public bool Debug { get; private set; } = false;

        //owner server id?
        public void ReadConfig()
        {
            string configName = "config.json";
            if (!File.Exists(configName))
            {
                string template = JsonConvert.SerializeObject(new Configuration(), Formatting.Indented);
                File.WriteAllText(configName, template, new UTF8Encoding(false));
                Console.WriteLine($"Missing {configName}, created template config file.");
                Console.ReadKey();
                return;
            }

            string json = File.ReadAllText(configName, new UTF8Encoding(false));
            var readConfig = JsonConvert.DeserializeObject<Configuration>(json);

            Token = readConfig.Token;
            CommandPrefixes = readConfig.CommandPrefixes;
            HexCode = readConfig.HexCode;
            Debug = readConfig.Debug;

            // Updating config with new fields
            json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(configName, json, new UTF8Encoding(false));
        }
        public void VerifyConfig()
        {
            if (string.IsNullOrEmpty(Token))
                throw new NullReferenceException("Please set the token in config.json");
            if (CommandPrefixes.Length == 0)
                throw new NullReferenceException("Please set a prefix in config.json");
        }
    }
}
