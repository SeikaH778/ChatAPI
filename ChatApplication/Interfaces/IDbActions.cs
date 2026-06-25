
using ChatApplication.DTO;

namespace ChatApplication.Interfaces
{
    public interface IDbActions
    {
        Task<AuthResponse> LoginAsync(string username, string password);
        Task<AuthResponse> RegisterAsync(string username, string password);
    }
}
