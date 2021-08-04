namespace MutinyBot.Models
{
    public class PetModel
    {
        public int PetId { get; set; }
        public ulong OwnerId { get; set; }
        public ulong GuildId { get; set; }
        public string PetName { get; set; }
        public string PetImageUrl { get; set; }
    }
}