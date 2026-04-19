namespace FlowerShop.Services.Interfaces;
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string username, string email, string password);
    Task<AuthResult> LoginAsync(string email, string password);
}
