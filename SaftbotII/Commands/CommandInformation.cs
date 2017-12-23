namespace SaftbotII.Commands
{
    internal struct CommandInformation
    {
        public Messaging messages;

        public string[] arguments;

        public DatabaseSystem.Server ServerEntry;

        public Discord.WebSocket.SocketMessage SocketMessage;

        public DatabaseSystem.User AuthorEntry
            => ServerEntry[SocketMessage.Author.Id];
    }
}