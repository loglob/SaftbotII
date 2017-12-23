using Discord;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII
{
    public class Messaging
    {
        private ITextChannel textChannel;

        public Messaging(ITextChannel textChannel)
        {
            this.textChannel = textChannel;
        }

        public async Task Send(string message)
        {
            await textChannel.SendMessageAsync(message);
            await LoggingSystem.Log.Enter($"Sent message '{message}' to channel '{textChannel.Name}'");
        }

        public async Task Upload(string path, string comment = "")
        {
            await textChannel.SendFileAsync(path, comment);
            await LoggingSystem.Log.Enter($"Uploaded file '{Path.GetFileName(path)}' with comment '{comment}' " +
                                          $"to channel '{textChannel.Name}' (Full path: '{path}')");
        }
    }
}
