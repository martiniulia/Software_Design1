using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FlowerShop.Models;
public class Bouquet
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    [MaxLength(50)]
    public string? OccasionTag { get; set; }  
    public string? ImageUrl { get; set; }
    public ICollection<BouquetFlower> BouquetFlowers { get; set; } = new List<BouquetFlower>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
