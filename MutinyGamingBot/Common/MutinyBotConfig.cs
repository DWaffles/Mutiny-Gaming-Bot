using Newtonsoft.Json;

namespace MutinyBot.Common
{
    public class MutinyBotConfig
    {
        [JsonProperty("discord")]
        public MutinyBotDiscordConfig Discord { get; private set; } = new MutinyBotDiscordConfig();

        [JsonProperty("debug")]
        public bool Debug { get; private set; } = false;

        [JsonProperty("hexCode")]
        public string HexCode { get; private set; } = "47200e";
    }
    public class MutinyBotDiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = "token";

        [JsonProperty("prefixes")]
        public string[] CommandPrefixes { get; private set; } = { "mg!", "%" };

        [JsonProperty("status")]
        public string BotStatus { get; private set; }

        [JsonProperty("mutiny_guild_id")]
        public ulong MutinyGuildId { get; private set; }

        [JsonProperty("authorized_guild_ids")]
        public ulong[] AuthorizedServerIds { get; private set; }
    }
    public class MutinyBotStartUpConfig
    {

    }
}
