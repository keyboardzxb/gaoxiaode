using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// 命名空间现在正确地指向了 Server 项目内部的 Data 文件夹
namespace Arrowgene.O2Jam.Server.Data
{
    [Table("Accounts")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public virtual PlayerEntity Player { get; set; }
    }
}