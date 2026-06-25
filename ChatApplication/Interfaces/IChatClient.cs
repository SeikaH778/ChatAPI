

namespace ChatApplication.Interfaces
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string message, string userName);
        public Task SystemMessage(string message);
    }
}
