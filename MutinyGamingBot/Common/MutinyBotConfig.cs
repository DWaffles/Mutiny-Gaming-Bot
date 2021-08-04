using Newtonsoft.Json;

namespace MutinyBot.Common
{
    public class MutinyBotConfig
    {
        [JsonProperty("discord")]
        public MutinyBotDiscordConfig Discord { get; private set; } = new MutinyBotDiscordConfig();

        [JsonProperty("Debug")]
        public bool Debug { get; private set; } = false;

        [JsonProperty("HexCode")]
        public string HexCode { get; private set; } = "47200e";
    }
    public class MutinyBotDiscordConfig
    {
        [JsonProperty("Token")]
        public string Token { get; private set; } = "token";

        [JsonProperty("prefixes")]
        public string[] CommandPrefixes { get; private set; } = { "mg!", "%" };

        [JsonProperty("status")]
        public string BotStatus { get; private set; }
    }
}
