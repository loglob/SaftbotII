namespace SaftbotII.Commands
{
    public struct CommandInformation
    {
        public Messaging messages;

        public string[] arguments;

        public Discord.WebSocket.SocketMessage SocketMessage;
    }
}
