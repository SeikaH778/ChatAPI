using ChatApplication.DTO;
using ChatApplication.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.ConstrainedExecution;

namespace TestChatAPI.Hubs
{
    public class DbHub : Hub<IDbActions>
    {
        private readonly IUserService _userService;
        private static readonly Dictionary<string, Guid> _connectedUsers = new();
        public DbHub(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            try
            {
                var result = await _userService.LoginAsync(username, password);

                if (result.Success && result.User != null)
                {
                    
                    _connectedUsers[Context.ConnectionId] = result.User.Id;

                    
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Пользователь{result.User.Id}");


                    Console.WriteLine($"Пользователь {username} уже зарегестрирован");
                }
                else
                {
                    Console.WriteLine($"Ошибка логина {username}: {result.Message}");
                }

                return result;
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message, $" {username}");
                return new AuthResponse(false, null, "Ошибка авторизации");
            }
        }

        public async Task<AuthResponse> RegisterAsync(string username, string password)
        {
            try
            {
                var result = await _userService.RegisterAsync(username, password);

                if (result.Success && result.User != null)
                {

                    Console.WriteLine($"Пользователь {username} уже зарегестрирован");
                }
                else
                {
                    Console.WriteLine($"Ошибка регистрации {username}: {result.Message}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, $"Ошибка во время регистрации {username}");
                return new AuthResponse(false, null, "Ошибка во время регистрации");
            }
        }
    }
}
