using Newtonsoft.Json;
using System;

namespace MutinyBot.Common
{
    public class MutinyBotConfig
    {
        [JsonProperty("discord")]
        public MutinyBotDiscordConfig Discord { get; private set; } = new MutinyBotDiscordConfig();

        /// <summary>
        /// Enables debug/verbose logging and registers debug slash commands if slash commands are enabled.
        /// </summary>
        [JsonProperty("debug")]
        public bool Debug { get; private set; } = false;

        /// <summary>
        /// Defines that the client should attempt to reconnect indefinitely. Will swallow all connection errors.
        /// </summary>
        [JsonProperty("reconnect_indefinitely")]
        public bool ReconnectIndefinitely { get; private set; } = false;

        [JsonProperty("hex_code")]
        public string HexCode { get; private set; } = "47200e";
    }
    public class MutinyBotDiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = "token";

        [JsonProperty("prefixes")]
        public string[] CommandPrefixes { get; private set; } = { "%" };

        [JsonProperty("status")]
        public string BotStatus { get; private set; }

        /// <summary>
        /// Enable slash commands in authorized guilds. 
        /// </summary>
        [JsonProperty("register_slash_commands")]
        public bool RegisterSlashCommands { get; private set; } = true;

        [JsonProperty("mutiny_guild_id")]
        public ulong MutinyGuildId { get; private set; } = 0;

        /// <summary>
        /// Servers where slash commands will be registered.
        /// </summary>
        [JsonProperty("authorized_guild_ids")]
        public ulong[] AuthorizedServerIds { get; private set; } = Array.Empty<ulong>();
    }
}
