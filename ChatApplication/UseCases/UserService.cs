using ChatApplication.DTO;
using ChatApplication.Interfaces;
using ChatDomain.Models;
using ChatPersistance;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.UseCases
{
    public class UserService : IUserService
    {
        private readonly ChatDBContext _context;

        public UserService(ChatDBContext context)
        {
            _context = context;
        }
        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userName == username);
                if (user == null)
                {
                    return new AuthResponse(false, null, "Пользователь не найден");
                }

                if (password!= user.password)
                {
                    return new AuthResponse(false, null, "Неверный пароль");
                }


                return new AuthResponse(true, user, "Успешный вход");
            }
            catch (Exception ex)
            {
                return new AuthResponse(false, null, $"Ошибка авторизации: {ex.Message}");
            }
        }

        public async Task<AuthResponse> RegisterAsync(string username, string password)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.userName == username))
                {
                    return new AuthResponse(false, null, "Имя занято");
                }

                var user = new User
                (
                    Guid.NewGuid(),
                    username,
                    password
                );

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new AuthResponse(true, user, "Успешно зарегестрирован");
            }
            catch (Exception ex)
            {
                return new AuthResponse(false, null, $"Ошибка регистрации: {ex.Message}");
            }
        }

    }
}
