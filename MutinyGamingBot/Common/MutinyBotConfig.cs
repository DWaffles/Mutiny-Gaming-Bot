using Newtonsoft.Json;
using System;

namespace MutinyBot.Common
{
    public class MutinyBotConfig
    {
        [JsonProperty("discord")]
        public MutinyBotDiscordConfig Discord { get; private set; } = new MutinyBotDiscordConfig();

        /// <summary>
        /// Whether to enables debug/verbose logging and registers test slash commands if slash commands are enabled.
        /// </summary>
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

        /// <summary>
        /// Whether to enable slash commands in authorized guilds. 
        /// </summary>
        [JsonProperty("register_slash_commands")]
        public bool RegisterSlashCommands { get; private set; } = true;

        [JsonProperty("mutiny_guild_id")]
        public ulong MutinyGuildId { get; private set; }

        /// <summary>
        /// Servers where slash commands will be registered.
        /// </summary>
        [JsonProperty("authorized_guild_ids")]
        public ulong[] AuthorizedServerIds { get; private set; } = Array.Empty<ulong>();
    }
}
