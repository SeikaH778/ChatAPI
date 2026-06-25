using Microsoft.AspNetCore.SignalR;
using ChatApplication.DTO;
using ChatApplication.Interfaces;
using ChatDomain.Models;
namespace TestChatAPI.Hubs
{
    public class ChatHub:Hub<IChatClient>
    {
        public async Task JoinChat(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.chatId.ToString());
            await Clients.Group(connection.chatId.ToString()).ReceiveMessage("СИСТЕМА",$"{connection.userName} зашел в чат");
            
        }
        public async Task SendMessage(string chatId, string user, string message)
        {
            await Clients.Group(chatId).ReceiveMessage(user, message);
        }
    }
}
 