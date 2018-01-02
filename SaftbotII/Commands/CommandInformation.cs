namespace SaftbotII.Commands
{
    internal struct CommandInformation
    {
        public Messaging messages;

        public string[] arguments;

        public DatabaseSystem.Server ServerEntry;

        public Discord.IMessage SocketMessage;

        public DatabaseSystem.User AuthorEntry;
    }
}