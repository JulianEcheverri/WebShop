using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Entities
{
    [Table("Customers") ]
    public class Customer
    {
        [Key, Column("CustomerId")]
        public int Id { get; set; }

        [Required, StringLength(100), Index("IX_Customer", IsUnique = true)]
        public string Name { get; set; }
    }
}