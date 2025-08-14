using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("T_o2jam_charCash")]
    public class CashEntity
    {
        [Key]
        [Column("USER_INDEX_ID")]
        public int UserIndexId { get; set; }

        [Column("GEM")]
        public int Gem { get; set; }

        [Column("MCASH")]
        public int Mcash { get; set; }

        [Column("O2CASH")]
        public int O2cash { get; set; }

        [Column("MUSICCASH")]
        public int Musiccash { get; set; }

        [Column("ITEMCASH")]
        public int Itemcash { get; set; }

        [ForeignKey("UserIndexId")]
        public virtual UserEntity User { get; set; }
    }
}
