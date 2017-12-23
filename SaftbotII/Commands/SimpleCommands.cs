using System;

namespace SaftbotII.Commands
{
    internal static class SimpleCommands
    {
        [Command("Tests if the bot is responsive")]
        public static async void Ping(CommandInformation cmdinfo)
            => await cmdinfo.messages.Send($"Pong! Took {Math.Round((DateTime.Now - cmdinfo.SocketMessage.CreatedAt).TotalMilliseconds)}ms");

        [Command("Rolls a fair die", "<Die size> [<Amount of rolls>]")]
        public static async void Roll(CommandInformation cmdinfo)
        {
            Random rng = new Random();
            
            if(! Int32.TryParse(cmdinfo.arguments[0], out int size))
            {
                await cmdinfo.messages.Send("Couldn't parse die size!");
                return;
            }

            if(!(cmdinfo.arguments.Length > 1 && Int32.TryParse(cmdinfo.arguments[1], out int rolls)))
                rolls = 1;

            string result = "";
            size = Math.Abs(size);
            rolls = Math.Abs(rolls);

            for (int i = 0; i < rolls; i++)
            {
                result += $"{rng.Next(1, size)}, ";
            }

            await cmdinfo.messages.Send(result.Substring(0, result.Length - 2));
        }

        [Command("Prints this list", "[<Command name>]")]
        public static async void Help(CommandInformation cmdinfo)
        {
            if(cmdinfo.arguments.Length > 0)
            {
                command? asked = CommandRegistry.Fetch(cmdinfo.arguments[0]);

                if (asked.HasValue)
                    await cmdinfo.messages.Send($"!{asked.Value.Name}: {asked.Value.Description}\n\tUsage: {asked.Value.Description}");
                else
                    await cmdinfo.messages.Send("Unknown command!");

                return;
            }

            string msg = "";

            foreach (command cmd in CommandRegistry.GetCommands)
                msg += $"!{cmd.Name}:\t{cmd.Description}\n";
            

            await cmdinfo.messages.Send(msg);
        }

        [Command("Bulk deletes messages from current channel", "<Amount>")]
        public static async void Purge(CommandInformation cmdinfo)
        {
            if (!Int32.TryParse(cmdinfo.arguments[0], out int toGet))
                await cmdinfo.messages.Send("Couldn't parse message count!");
            else
            {
                toGet = Math.Abs(toGet);

                if (toGet > 100)
                {
                    await cmdinfo.messages.Send("Maximum number of messages to delete at once is 100, sorry!");
                    toGet = 100;
                }

                var toDel = cmdinfo.SocketMessage.Channel.GetCachedMessages(toGet);
                await cmdinfo.SocketMessage.Channel.DeleteMessagesAsync(toDel);

                await cmdinfo.messages.Send($"Deleted {toGet} messages!");
            }
        }
    }
}