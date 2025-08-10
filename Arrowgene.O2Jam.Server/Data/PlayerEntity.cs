using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("Characters")]
    public class PlayerEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int Level { get; set; }
        public int Gender { get; set; }
        public int Exp { get; set; }
        public int Gem { get; set; }
        public int Cash { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }
    }
}