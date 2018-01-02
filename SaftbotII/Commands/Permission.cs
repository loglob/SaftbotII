using Discord.WebSocket;
using SaftbotII.DatabaseSystem;
using System.Collections.Generic;

namespace SaftbotII.Commands
{
    internal static class PermissionCommand
    {
        private static Dictionary<string, UserSettings> permissionNames = new Dictionary<string, UserSettings>
        {
            { "admin",      UserSettings.Admin },
            { "dj",         UserSettings.DJ },
            { "ignored",    UserSettings.Ignored }
        };

        [Command("Grants a permission or takes it from users","<give/take> <Perm name> <users>", PermissionLevel = 2)]
        public static async void Permission(CommandInformation cmdinfo)
        {
            bool newVal;

            if (cmdinfo.arguments[0] == "give")
                newVal = true;
            else if (cmdinfo.arguments[0] == "take")
                newVal = false;
            else
            {
                await cmdinfo.messages.Send("Expected 'give' or 'take' as first argument!");
                return;
            }

            if (permissionNames.ContainsKey(cmdinfo.arguments[1].ToLower()))
            {
                UserSettings toChange = permissionNames[cmdinfo.arguments[1].ToLower()];

                foreach (var usr in cmdinfo.SocketMessage.MentionedUserIds)
                {
                    // Prevent you from taking the admin status from this server
                    // or the debug mode's dummy user
                    if (((toChange == UserSettings.Admin) && (!newVal)) && (
                            (!OfflineMode.DebugMode.Active && (usr == ((SocketGuildChannel)cmdinfo.SocketMessage.Channel).Guild.OwnerId))
                            || (OfflineMode.DebugMode.Active && usr == OfflineMode.DebugMode.DummyUser.UserID)))
                    {
                        await cmdinfo.messages.Send("Cannot de-admin the server's owner!");
                        return;
                    }
                    else
                        cmdinfo.ServerEntry[usr][toChange] = newVal;
                }
                await cmdinfo.messages.Send("Updated permissions!");
                await Database.UpdateData();
            }
            else
                await cmdinfo.messages.Send("Unknown permission name!");
        }
    }
}
