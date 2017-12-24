using System;
using System.Linq;

namespace SaftbotII.DatabaseSystem
{
    /// <summary>
    /// A user on a server
    /// </summary>
    internal class User
    {
        /// <summary>
        /// The server this user is from
        /// </summary>
        public Server Server;

        /// <summary>
        /// The user's UUID
        /// </summary>
        public ulong UserID;

        /// <summary>
        /// The raw settings value for this user
        /// </summary>
        public byte UserSetting;

        /// <summary>
        /// The setting value used by default
        /// </summary>
        public const byte StandardSetting = 0;

        public int PermissionLevel
        {
            get
            {
                if (IsDev)
                    return 3;
                else if (this[UserSettings.Admin])
                    return 2;
                else if (this[UserSettings.DJ] || Server[ServerSettings.PlebsCanDJ])
                    return 1;
                else if (this[UserSettings.Ignored])
                    return -1;
                else
                    return 0;
            }
        }

        public string Mention
            => $"<@{UserID}>";

        #region Constructors
        public User(Server from, ulong userID = 0)
        {
            UserSetting = StandardSetting;
            UserID = userID;
            Server = from;
        }

        /// <summary>
        /// Reads a user from serialized data
        /// </summary>
        /// <param name="rawdata">The raw bytes containing all serialized data</param>
        /// <param name="position">The position for the first byte of the user's data</param>
        /// <param name="bytesread">The amount of bytes read to produce a user</param>
        public User(Server from, byte[] rawdata, int position, out int bytesread)
        {
            Server = from;
            UserID = BitConverter.ToUInt64(rawdata, position);
            UserSetting = rawdata[position + 8];
            bytesread = 9;
        }
        #endregion

        /// <summary>
        /// Finds out if this user is a developer
        /// </summary>
        public bool IsDev
            => Saftbot.DevUUIDs.Contains(UserID);

        #region Settings
        /// <summary>
        /// Retrieves or sets the given setting for this user
        /// </summary>
        /// <param name="setting">Setting to retrieve</param>
        /// <returns></returns>
        public bool this[UserSettings setting]
        {
            get
            {
                return this[(int)setting];
            }

            set
            {
                this[(int)setting] = value;
            }
        }

        private bool this[int settingID]
        {
            get
                => Util.ChopFromByte(UserSetting, settingID);

            set
                => UserSetting = Util.SetBit(value, UserSetting, settingID);
        }
        #endregion

        /// <summary>
        /// Gets raw byte representation of this user
        /// </summary>
        public byte[] Serialize()
        {
            byte[] data = new byte[9];

            Array.Copy(BitConverter.GetBytes(UserID), data, 8);
            data[8] = UserSetting;

            return data;
        }
    }
}