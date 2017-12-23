using Discord;
using Discord.WebSocket;
using SaftbotII.DatabaseSystem;
using System;
using System.IO;
using System.Threading.Tasks;

#pragma warning disable CS1998
#pragma warning disable CS4014
namespace SaftbotII
{
    public class Saftbot
    {
        public const string CommandPrefix = "!";

        private const string tokenPath = "./token.txt";
        private DiscordSocketClient client;

        private static Task<string> token
            => File.ReadAllTextAsync(tokenPath);

        internal static void Main(string[] args)
            => new Saftbot().Main().Wait();

        internal async Task Main()
        {
            // Be advised that I have been encounting errors when trying to run this under Windows 7.
            // Windows 10 or Linux appear to be running fine
            await Log.Enter("Initializing Discord.NET...");

            client = new DiscordSocketClient();
            client.Log += Log.Enter;
            client.MessageReceived += HandleMessage;
            client.JoinedGuild += OnNewServer;

            await client.LoginAsync(TokenType.Bot, await token);
            await client.StartAsync();
            await Log.Enter("Initialization complete!");

            await Database.BuildFromFile();
            await Log.Enter("Initialized Database");

            await Commands.CommandRegistry.RegisterAll();
            await Log.Enter($"{Commands.CommandRegistry.RegisteredCount} Command(s) loaded.");

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task HandleMessage(SocketMessage message)
        {
            if(!(message.Author.IsBot || message.Author.IsWebhook))
            {
                string content = message.Content.Trim();

                if(content.StartsWith(CommandPrefix))
                {
                    try
                    {
                        string[] splitMsg = content.Split(null);
                        string command = splitMsg[0].Substring(CommandPrefix.Length).ToLower();
                        Log.Enter($"Received command '{command}'!");

                        Commands.CommandInformation cmdinfo = new Commands.CommandInformation();
                        cmdinfo.SocketMessage = message;
                        cmdinfo.messages = new Messaging((ITextChannel)message.Channel);
                        cmdinfo.arguments = Util.SubArray(splitMsg, 1);

                        Commands.CommandRegistry.Run(command, cmdinfo);
                    }
                    catch(Exceptions.SaftEceptions saftEx)
                    {   // A saftbot-internal exception technically shouldn't end up here, but it isn't a big enough deal to warn the user
                        await Log.Enter($"Saftbot-internal Exception caught while trying to execute command:\n\t{message.Content}\n" +
                                        $"Content:\n{saftEx}");
                    }
                    catch(Exception ex)
                    {   // On the other hand, any other kind of exception indicates a fatal flaw in whatever command the user was calling
                        await new Messaging((ITextChannel)message.Channel).Send($"The command you tried caused an error!\n" +
                                            $"If this happend before, please report it at {Exceptions.SaftDatabaseException.repoLink}\n");


                        await Log.Enter($"Exception caught while trying to execute command:\n\t{message.Content}");
                        await Log.Enter(ex);
                    }
                }
            }
        }

        
        public async Task OnNewServer(SocketGuild guild)
        {
            Database.Fetch(guild.Id, guild.OwnerId)[UserSettings.Admin] = true;
        }
    }
}
