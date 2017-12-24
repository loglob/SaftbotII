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

        [Command("Grants a permission or takes it from users","<give/take> <Perm name> <users>")]
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
                foreach (var usr in cmdinfo.SocketMessage.MentionedUsers)
                {
                    cmdinfo.ServerEntry[usr.Id][permissionNames[cmdinfo.arguments[1].ToLower()]] = newVal;
                }

                await cmdinfo.messages.Send("Updated permissions!");
                await Database.UpdateData();
            }
            else
                await cmdinfo.messages.Send("Unknown permission name!");
        }
    }
}
