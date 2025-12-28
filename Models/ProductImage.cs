using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class ProductImage :BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        [MaxLength(512)]
        public string ImageUrl { get; set; }
    }
}
