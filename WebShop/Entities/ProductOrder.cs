using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("ProductsOrders") ]
    public class ProductOrder
    {
        [Key, Column("ProductOrderId")]
        public int Id { get; set; }

        [Index("IX_ProductOrder", IsUnique = true, Order = 1)]
        public int OrderId { get; set; }

        [Index("IX_ProductOrder", IsUnique = true, Order = 2)]
        public int ProductId { get; set; }

        public int ProductAmount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}