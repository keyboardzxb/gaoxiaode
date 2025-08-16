using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("T_o2jam_userinfo")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("USER_INDEX_ID")]
        public int UserIndexId { get; set; }

        [Required]
        [Column("USER_ID")]
        public string UserId { get; set; }

        [Required]
        [Column("USER_NICKNAME")]
        public string UserNickname { get; set; }

        [Required]
        [Column("SEX")]
        public string Sex { get; set; }

        [Column("CREATE_TIME")]
        public DateTime? CreateTime { get; set; }
    }
}