using System.ComponentModel.DataAnnotations;
namespace FlowerShop.Models;
public class Category
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
    public ICollection<Flower> Flowers { get; set; } = new List<Flower>();
}
