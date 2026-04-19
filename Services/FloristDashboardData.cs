using FlowerShop.Models;
namespace FlowerShop.Services;
public sealed class FloristDashboardData
{
    public List<Order> Pending { get; init; } = new();
    public List<Order> InPreparation { get; init; } = new();
    public string? AssignmentWarning { get; init; }
}
