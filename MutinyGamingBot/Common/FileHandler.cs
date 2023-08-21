using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Text;

namespace MutinyBot.Common
{
    /// <summary>
    /// Internal helper class for handling all the file I/O operations of the bot.
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Returns a <see cref="MutinyBotConfig"/> from the given path.
        /// </summary>
        /// <param name="configName">Relative path and name to the config file.</param>
        /// <exception cref="FileNotFoundException">The given file could not be found.</exception>
        /// <returns><see cref="MutinyBotConfig"/></returns>
        public static MutinyBotConfig ReadConfig(string configName)
        {
            if (!File.Exists(configName))
            {
                WriteDefaultToFile<MutinyBotConfig>(configName);
                Log.Error($"Missing {configName}, created template config file.");
                throw new FileNotFoundException($"{configName} does not exist, created template config file at location.");
            }
            else
                return ReadJsonFile<MutinyBotConfig>(configName, true);
        }

        /// <summary>
        /// Verifies if a <see cref="MutinyBotConfig"/> is a valid config.
        /// </summary>
        /// <remarks>
        /// Checks if the token is: null, empty, or 'token'. Checks if there is at least one non-whitespace prefix.
        /// </remarks>
        /// <param name="config">Config to check validity for.</param>
        /// <returns><see cref="bool"/></returns>
        public static bool VerifyConfig(MutinyBotConfig config)
        {
            return !(String.IsNullOrEmpty(config?.Discord?.Token)
                || config.Discord.Token.Equals("token", StringComparison.OrdinalIgnoreCase)
                || config.Discord.CommandPrefixes.Length == 0
                || string.IsNullOrWhiteSpace(config.Discord.CommandPrefixes[0]));
        }

        /// <summary>
        /// Will parse the given file and return a constructed object of the given generic type from the data.
        /// </summary>
        /// <remarks>Will update the file with missing object fields where applicable.</remarks>
        /// <typeparam name="Type">Generic type to parse and return.</typeparam>
        /// <param name="fileName">Relative path and name to the file.</param>
        /// <param name="critical">Throw a <see cref="FileNotFoundException"/> if the file does not exist.</param>
        /// <exception cref="FileNotFoundException">The given file could not be found.</exception>
        /// <returns>Object of given type.</returns>
        public static Type ReadJsonFile<Type>(string fileName, bool critical = false)
        {
            FileInfo file = new(fileName);
            if (!file.Exists)
            {
                if (critical)
                    throw new FileNotFoundException($"Was not able to find {fileName} at specified location.");
                else
                    return default;
            }

            string json = File.ReadAllText(file.FullName, new UTF8Encoding(false));
            var readObject = JsonConvert.DeserializeObject<Type>(json);

            // Updating config with new fields
            json = JsonConvert.SerializeObject(readObject, Formatting.Indented);
            File.WriteAllText(file.FullName, json, new UTF8Encoding(false));

            return readObject;
        }

        /// <summary>
        /// Will overwrite or create a file using the given name with a default object of the generic type.
        /// </summary>
        /// <typeparam name="Type">Generic type to output the default for.</typeparam>
        /// <param name="fileName">Relative path and name to the file.</param>
        private static void WriteDefaultToFile<Type>(string fileName) where Type : new()
        {
            string template = JsonConvert.SerializeObject(new Type(), Formatting.Indented);

            var file = new FileInfo(fileName);
            file.Directory.Create();
            File.WriteAllText(fileName, template, new UTF8Encoding(false));
        }
    }
}
