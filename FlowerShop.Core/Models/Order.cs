using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FlowerShop.Models;
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }
    [MaxLength(200)]
    public string? DeliveryAddress { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public int? BouquetId { get; set; }  
    public int? AssignedFloristId { get; set; }
    public User User { get; set; } = null!;
    public Bouquet? Bouquet { get; set; }
    public Florist? AssignedFlorist { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
public enum OrderStatus
{
    Pending,
    Confirmed,
    InPreparation,
    OutForDelivery,
    Delivered,
    Cancelled
}
