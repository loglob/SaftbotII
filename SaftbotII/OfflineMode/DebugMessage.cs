using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaftbotII.OfflineMode
{
    internal struct DebugMessage : Discord.IMessage
    {
        public DebugMessage(string content)
        {
            crAt = DateTime.Now;
            this.content = content;
        }

        string content;
        DateTime crAt;

        MessageType IMessage.Type => MessageType.Default;

        MessageSource IMessage.Source => MessageSource.User;

        bool IMessage.IsTTS => false;

        bool IMessage.IsPinned => false;

        string IMessage.Content => content;

        DateTimeOffset IMessage.Timestamp => crAt;

        DateTimeOffset? IMessage.EditedTimestamp => crAt;

        IMessageChannel IMessage.Channel => null;

        IUser IMessage.Author => new DebugUser();

        IReadOnlyCollection<IAttachment> IMessage.Attachments => new List<IAttachment>();

        IReadOnlyCollection<IEmbed> IMessage.Embeds => new List<IEmbed>();

        IReadOnlyCollection<ITag> IMessage.Tags => new List<ITag>();

        IReadOnlyCollection<ulong> IDWithPrefix(string prefix)
        {
            List<ulong> mentions = new List<ulong>();

            foreach (string mention in content.Split(null))
            {
                if (mention.StartsWith(prefix))
                    if (UInt64.TryParse(mention.Substring(prefix.Length), out ulong ID))
                        mentions.Add(ID);
            }

            return mentions;
        }

        IReadOnlyCollection<ulong> IMessage.MentionedChannelIds
            => IDWithPrefix("#");

        IReadOnlyCollection<ulong> IMessage.MentionedRoleIds
            => IDWithPrefix("~");

        IReadOnlyCollection<ulong> IMessage.MentionedUserIds
            => IDWithPrefix("@");

        DateTimeOffset ISnowflakeEntity.CreatedAt => crAt;

        ulong IEntity<ulong>.Id => DebugMode.DummyUser.UserID;

        Task IDeletable.DeleteAsync(RequestOptions options)
        {
            return Task.CompletedTask;
        }
    }
}
