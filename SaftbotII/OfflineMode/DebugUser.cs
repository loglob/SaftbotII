using Discord;
using System;
using System.Threading.Tasks;

namespace SaftbotII.OfflineMode
{
    struct DebugUser : Discord.IUser
    {
        string IUser.AvatarId => "";

        string IUser.Discriminator => "";

        ushort IUser.DiscriminatorValue => 0;

        bool IUser.IsBot => false;

        bool IUser.IsWebhook => false;

        string IUser.Username => "0cool";

        DateTimeOffset ISnowflakeEntity.CreatedAt => DateTime.Today;

        ulong IEntity<ulong>.Id => DebugMode.DummyUser.UserID;

        string IMentionable.Mention => DebugMode.DummyUser.Mention;

        Game? IPresence.Game => null;

        UserStatus IPresence.Status => UserStatus.Online;

        string IUser.GetAvatarUrl(ImageFormat format, ushort size)
        {
            return "";
        }

        Task<IDMChannel> IUser.GetOrCreateDMChannelAsync(RequestOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
