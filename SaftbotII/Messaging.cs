using Discord;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII
{
    /// <summary>
    /// Allows sending of messages or uploading of files to a text channel
    /// </summary>
    public class Messaging
    {
        private ITextChannel textChannel;

        /// <summary>
        /// Initialized a messaging-wrapper for the given channel
        /// </summary>
        public Messaging(ITextChannel textChannel)
        {
            this.textChannel = textChannel;
        }

        /// <summary>
        /// Sends a message to the channel
        /// </summary>
        public virtual async Task Send(string message)
        {
            await textChannel.SendMessageAsync(message);
            Log.Enter($"Sent message '{message}' to channel '{textChannel.Name}'");
        }

        /// <summary>
        /// Uploads a file to the channel
        /// </summary>
        /// <param name="path">Path to the file</param>
        public virtual async Task Upload(string path, string comment = "")
        {
            await textChannel.SendFileAsync(path, comment);
            Log.Enter($"Uploaded file '{Path.GetFileName(path)}' with comment '{comment}' " +
                                          $"to channel '{textChannel.Name}' (Full path: '{path}')");
        }
    }
}
