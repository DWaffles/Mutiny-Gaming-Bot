using System.ComponentModel.DataAnnotations;

namespace MutinyBot.Entities
{
    public abstract class DbEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
