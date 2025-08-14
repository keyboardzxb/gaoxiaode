using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("member")]
    public class MemberEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("userid")]
        public string UserId { get; set; }

        [Required]
        [Column("usernick")]
        public string UserNick { get; set; }

        [Column("sex")]
        public bool Sex { get; set; }

        [Required]
        [Column("passwd")]
        public string Password { get; set; }

        [Column("registdate")]
        public DateTime RegisterDate { get; set; }

        [Required]
        [Column("id9you")]
        public string Id9you { get; set; }

        [Column("vip")]
        public short Vip { get; set; }

        [Column("vipdate")]
        public DateTime? VipDate { get; set; }
    }
}
