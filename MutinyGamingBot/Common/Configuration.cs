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
        public bool ReadConfig()
        {
            string configName = "config.json";
            if (!File.Exists(configName))
            {
                string template = JsonConvert.SerializeObject(new Configuration(), Formatting.Indented);
                File.WriteAllText(configName, template, new UTF8Encoding(false));
                Console.WriteLine($"Missing {configName}, created template config file.");
                Console.ReadKey();
                return false;
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

            return VerifyConfig();
        }
        private bool VerifyConfig()
        {
            if (string.IsNullOrEmpty(Token) || CommandPrefixes.Length == 0)
                return false;
            else
                return true;
        }
    }
}
