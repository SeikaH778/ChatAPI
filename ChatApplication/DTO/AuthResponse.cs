
using ChatDomain.Models;

namespace ChatApplication.DTO
{
    public record AuthResponse(bool Success, User? User, string Message);
}
