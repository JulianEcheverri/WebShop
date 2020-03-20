using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("Orders") ]
    public class Order
    {
        [Key, Column("OrderId")]
        public int Id { get; set; }

        [Required, StringLength(100), Index("IX_Order", IsUnique = true)]
        public string Number { get; set; }

        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}