using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("ProductsCategories") ]
    public class ProductCategory
    {
        [Key]
        [Column("ProductCategoryId")]
        public int Id { get; set; }

        [Index("IX_ProductCategory", IsUnique = true, Order = 1)]
        public int ProductId { get; set; }

        [Index("IX_ProductCategory", IsUnique = true, Order = 2)]
        public int CategoryId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Category Category { get; set; }
    }
}