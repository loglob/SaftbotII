using SaftbotII.DatabaseSystem;
using System.Collections.Generic;

namespace SaftbotII.Commands
{
    internal static class WhoIsCommand
    {
        private static Dictionary<UserSettings, string> descriptors = new Dictionary<UserSettings, string>{
            { UserSettings.Ignored, "ignored"},
            { UserSettings.Admin,   "an admin" },
            { UserSettings.DJ,      "a DJ" }
        };

        [Command("Prints information about all mentiond users", "<Users>")]
        public static async void WhoIs(CommandInformation cmdinfo)
        {
            foreach (var usr in cmdinfo.SocketMessage.MentionedUserIds)
                await cmdinfo.messages.Send(GetInfoOf(cmdinfo.ServerEntry[usr]));
            
        }

        private static string GetInfoOf(User usr)
        {
            string msg = $"{usr.Mention} (ID: {usr.UserID}) is ";
            List<string> userRoles = new List<string>();

            if (usr.IsDev)
                userRoles.Add("a developer");

            foreach (var kvPair in descriptors)
                if (usr[kvPair.Key])
                    userRoles.Add(kvPair.Value);
            
            if (userRoles.Count > 0)
                return msg + userRoles.ToArray().AndJoin();
            else
                return msg + "a nobody";
        }
    }
}
