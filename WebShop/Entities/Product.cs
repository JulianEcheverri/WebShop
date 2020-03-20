using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("Products") ]
    public class Product
    {
        [Key, Column("ProductId")]
        public int Id { get; set; }

        [Required, StringLength(100), Index("IX_Product", IsUnique = true)]
        public string Title { get; set; }

        public int Number { get; set; }
        public int Price { get; set; }
    }
}