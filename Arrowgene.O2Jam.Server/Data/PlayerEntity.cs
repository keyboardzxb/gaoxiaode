using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("T_o2jam_charinfo")]
    public class PlayerEntity
    {
        [Key]
        [Column("USER_INDEX_ID")]
        public int UserIndexId { get; set; }

        [Required]
        [Column("USER_ID")]
        public string UserId { get; set; }

        [Required]
        [Column("USER_NICKNAME")]
        public string UserNickname { get; set; }

        [Column("Sex")]
        public bool Sex { get; set; }

        [Column("Level")]
        public int Level { get; set; }

        [Column("Battle")]
        public int Battle { get; set; }

        [Column("win")]
        public int Win { get; set; }

        [Column("Draw")]
        public int Draw { get; set; }

        [Column("Lose")]
        public int Lose { get; set; }

        [Column("Experience")]
        public int Experience { get; set; }

        [Column("AdminLevel")]
        public int AdminLevel { get; set; }

        [ForeignKey("UserIndexId")]
        public virtual UserEntity User { get; set; }
    }
}