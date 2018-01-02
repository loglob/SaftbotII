using SaftbotII.Commands;
using SaftbotII.DatabaseSystem;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII.OfflineMode
{
    /// <summary>
    /// Allows you to simulate receiving messages from a nonexistant dummy server
    /// </summary>
    internal static class DebugMode
    {
        public const string DummyDBPath = "./debug.sdb";
        public static Server DummyServer;
        public static User DummyUser;
        public static bool Active;

        public static async Task StartSession()
        {
            Active = true;
            await Database.BuildFromFile(DummyDBPath);
            Log.Enter("Initialized Database");
            
            CommandRegistry.RegisterAll();
            Log.Enter($"{CommandRegistry.RegisteredCount} Command(s) loaded.");

            SearchProvider.Initialize();
            DummyServer = (Server)0;
            DummyUser = DummyServer[(ulong)0];
            DummyUser[UserSettings.Admin] = true;

            Log.Enter("Fully initialized.");

            while (true)
            {
                string input = Console.ReadLine();

                try
                {
                    string cmd = input.Split(null)[0].ToLower();

                    if (cmd == "stop")
                        break;

                    CommandInformation cmdinfo = new CommandInformation
                    {
                        ServerEntry = DummyServer,
                        SocketMessage = new DebugMessage(input),
                        arguments = Util.SubArray(input.Split(null), 1),
                        messages = new DebugMessaging(),
                        AuthorEntry = DummyUser
                    };

                    CommandRegistry.Run(cmd, cmdinfo);
                }
                catch(Exception ex)
                {
                    Log.Enter($"Caught {ex.GetType().ToString()}.\n{ex.Message}");
                }
            }
            
            File.Delete(DummyDBPath);
        }
    }

}
