namespace MutinyBot.Entities
{
    public class PetEntity : DbEntity
    {
        public ulong OwnerId { get; set; }
        public ulong GuildId { get; set; }
        public string PetName { get; set; }
        public string PetImageUrl { get; set; }
        // id?? //int
    }
}
