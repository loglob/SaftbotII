using System;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII
{
    /// <summary>
    /// Allows logging into console and a local log file
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// The folder in which all new log files are created
        /// </summary>
        public const string FolderPath = "./logs/";

        /// <summary>
        /// The stream linked to the current log file
        /// </summary>
        private static StreamWriter CurrentStream;

        /// <summary>
        /// This gets prepended to any Discord.Net log messages
        /// </summary>
        const string DiscordNetPrefix = "[Discord.Net]";
        
        /// <summary>
        /// Initializes the log
        /// Requires to be a static constructor else initialization messages cannot be entered
        /// </summary>
        static Log()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            string currPath = GetNewFilename();
            CurrentStream = File.CreateText(currPath);
            CurrentStream.WriteLine($"Started new logfile at {DateTime.Now.ToString("hh:mm:ss, dd.MM.yyyy")}");
            CurrentStream.Flush();
        }
        
        /// <summary>
        /// Finds an unsused Datestamped filename
        /// </summary>
        public static string GetNewFilename()
        {
            string path = $"{FolderPath}{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            return Util.FindUnusedVariant(path);
        }
        
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="addTimestamp">Wether the file's lien should be prepended with a Timestamp</param>
        public static void Enter(string message, bool addTimestamp = true)
        {
            string prefix = addTimestamp ? DateTime.Now.ToString("[HH:mm:ss] "): "";
            string total = prefix + message;

            Console.WriteLine(message);
            CurrentStream.WriteLine(total);
            CurrentStream.Flush();
        }

        /// <summary>
        /// Logs all relevant data about an exception
        /// </summary>
        public static void Enter(Exception exception)
            => Enter($"[ERROR] {exception.GetType()}\n\tMessage: {exception.Message}" +
                        $"\n\tStackTrace: {exception.StackTrace}\n\tSource: {exception.Source}");
        
        /// <summary>
        /// Logs a Discord.Net debug message.
        /// </summary>
        #pragma warning disable CS1998
        public static async Task Enter(Discord.LogMessage message)
            => Enter($"{DiscordNetPrefix} {message.Message.ToString()}");
        
    }
}
