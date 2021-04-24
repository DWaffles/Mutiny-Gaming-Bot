namespace MutinyBot.Entities
{
    public class GuildEntity : DbEntity
    {
        public ulong GuildId { get; set; }
        public string GuildName { get; set; } = string.Empty;
        public ulong ModerationLogChannelId { get; set; }
        public bool JoinLogEnabled { get; set; } = false;
        public ulong JoinLogChannelId { get; set; } = 0;
        public ulong MuteRoleId { get; set; } = 0;
    }
}
