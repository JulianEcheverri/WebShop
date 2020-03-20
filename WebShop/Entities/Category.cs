using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("Categories") ]
    public class Category
    {
        [Key, Column("CategoryId")]
        public int Id { get; set; }

        [Required, StringLength(100), Index("IX_Category", IsUnique = true)]
        public string Name { get; set; }
    }
}