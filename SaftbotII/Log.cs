using System;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII
{
    public static class Log
    {
        public const string FolderPath = "./logs/";
        private static string CurrentPath;
        const string DiscordNetPrefix = "[Discord.Net]";

        static Log()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            CurrentPath = GetNewFilename();
            var streamwriter = File.CreateText(CurrentPath);
            streamwriter.Flush();
            streamwriter.Dispose();
            streamwriter.Close();
        }

        public static string GetNewFilename()
        {
            string path = $"{FolderPath}{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            return Util.FindUnusedVariant(path);
        }
        
        public static async Task Enter(string message, bool addTimestamp = true)
        {
            string prefix = addTimestamp ? DateTime.Now.ToString("[HH:mm:ss] "): "";
            string total = prefix + message;
            total += (total.EndsWith('\n')) ? "" : "\n";

            Console.WriteLine(message);
            await File.AppendAllTextAsync(CurrentPath, message);
        }

        public static async Task Enter(Exception exception)
            => await Enter($"[ERROR] {exception.GetType()}\n\tMessage: {exception.Message}" +
                        $"\n\tStackTrace: {exception.StackTrace}\n\tSource: {exception.Source}");
        

        public static async Task Enter(Discord.LogMessage message)
            => await Enter($"{DiscordNetPrefix} {message.Message.ToString()}");
        
    }
}
