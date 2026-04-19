using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FlowerShop.Models;
public class BouquetFlower
{
    public int BouquetId { get; set; }
    public int FlowerId { get; set; }
    public int Quantity { get; set; }
    public Bouquet Bouquet { get; set; } = null!;
    public Flower Flower { get; set; } = null!;
}
