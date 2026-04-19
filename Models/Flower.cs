using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FlowerShop.Models;
public class Flower
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    [MaxLength(50)]
    public string? Color { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? FloristId { get; set; }
    public Florist? Florist { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<BouquetFlower> BouquetFlowers { get; set; } = new List<BouquetFlower>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
