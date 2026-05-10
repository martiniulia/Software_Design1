using FlowerShop.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int? FlowerId { get; set; }
    public Flower? Flower { get; set; }
    public int? BouquetId { get; set; }
    public Bouquet? Bouquet { get; set; }
}
