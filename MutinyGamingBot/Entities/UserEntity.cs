namespace MutinyBot.Entities
{
    public class UserEntity : DbEntity
    {
        public ulong UserId { get; set; }
        public bool Banned { get; set; } = false;
    }
}
