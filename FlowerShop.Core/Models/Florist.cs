using System.ComponentModel.DataAnnotations;
namespace FlowerShop.Models;
public class Florist
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Specialization { get; set; }
    [MaxLength(200)]
    public string? Bio { get; set; }
    [MaxLength(100)]
    public string? ContactEmail { get; set; }
    [MaxLength(20)]
    public string? Phone { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Flower> Flowers { get; set; } = new List<Flower>();
}
