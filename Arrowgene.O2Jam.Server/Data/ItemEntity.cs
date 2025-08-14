using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arrowgene.O2Jam.Server.Data
{
    [Table("T_o2jam_item")]
    public class ItemEntity
    {
        [Key]
        [Column("USER_INDEX_ID")]
        public int UserIndexId { get; set; }

        [Column("EQUIP1")] public short Equip1 { get; set; }
        [Column("EQUIP2")] public short Equip2 { get; set; }
        [Column("EQUIP3")] public short Equip3 { get; set; }
        [Column("EQUIP4")] public short Equip4 { get; set; }
        [Column("EQUIP5")] public short Equip5 { get; set; }
        [Column("EQUIP6")] public short Equip6 { get; set; }
        [Column("EQUIP7")] public short Equip7 { get; set; }
        [Column("EQUIP8")] public short Equip8 { get; set; }
        [Column("EQUIP9")] public short Equip9 { get; set; }
        [Column("EQUIP10")] public short Equip10 { get; set; }
        [Column("EQUIP11")] public short Equip11 { get; set; }
        [Column("EQUIP12")] public short Equip12 { get; set; }
        [Column("EQUIP13")] public short Equip13 { get; set; }
        [Column("EQUIP14")] public short Equip14 { get; set; }
        [Column("EQUIP15")] public short Equip15 { get; set; }
        [Column("EQUIP16")] public short Equip16 { get; set; }

        [Column("BAG1")] public short Bag1 { get; set; }
        [Column("BAG2")] public short Bag2 { get; set; }
        [Column("BAG3")] public short Bag3 { get; set; }
        [Column("BAG4")] public short Bag4 { get; set; }
        [Column("BAG5")] public short Bag5 { get; set; }
        [Column("BAG6")] public short Bag6 { get; set; }
        [Column("BAG7")] public short Bag7 { get; set; }
        [Column("BAG8")] public short Bag8 { get; set; }
        [Column("BAG9")] public short Bag9 { get; set; }
        [Column("BAG10")] public short Bag10 { get; set; }
        [Column("BAG11")] public short Bag11 { get; set; }
        [Column("BAG12")] public short Bag12 { get; set; }
        [Column("BAG13")] public short Bag13 { get; set; }
        [Column("BAG14")] public short Bag14 { get; set; }
        [Column("BAG15")] public short Bag15 { get; set; }
        [Column("BAG16")] public short Bag16 { get; set; }
        [Column("BAG17")] public short Bag17 { get; set; }
        [Column("BAG18")] public short Bag18 { get; set; }
        [Column("BAG19")] public short Bag19 { get; set; }
        [Column("BAG20")] public short Bag20 { get; set; }
        [Column("BAG21")] public short Bag21 { get; set; }
        [Column("BAG22")] public short Bag22 { get; set; }
        [Column("BAG23")] public short Bag23 { get; set; }
        [Column("BAG24")] public short Bag24 { get; set; }
        [Column("BAG25")] public short Bag25 { get; set; }
        [Column("BAG26")] public short Bag26 { get; set; }
        [Column("BAG27")] public short Bag27 { get; set; }
        [Column("BAG28")] public short Bag28 { get; set; }
        [Column("BAG29")] public short Bag29 { get; set; }
        [Column("BAG30")] public short Bag30 { get; set; }

        [ForeignKey("UserIndexId")]
        public virtual UserEntity User { get; set; }
    }
}
