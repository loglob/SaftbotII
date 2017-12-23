using System;
using System.Threading.Tasks;
using System.Linq;

namespace SaftbotII.DatabaseSystem
{
    internal class User
    {
        public Server Server;
        public ulong UserID;
        public byte UserSetting;
        public const byte StandardSetting = 0;

        public int PermissionLevel
        {
            get
            {
                if (IsDev)
                    return 3;
                else if (this[UserSettings.Admin])
                    return 2;
                else if (this[UserSettings.DJ])
                    return 1;
                else if (this[UserSettings.Ignored])
                    return -1;
                else
                    return 0;
            }
        }

        #region Constructors
        public User(Server from, ulong userID = 0)
        {
            UserSetting = StandardSetting;
            UserID = userID;
            Server = from;
        }

        public User(byte[] rawdata, int position, out int bytesread)
        {
            UserID = BitConverter.ToUInt64(rawdata, position);
            UserSetting = rawdata[position + 8];
            bytesread = 9;
        }
        #endregion

        public bool IsDev
            => Saftbot.DevUUIDs.Contains(UserID);

        #region Settings
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

        #region Serialization
        public byte[] Serialize()
        {
            byte[] data = new byte[9];

            Array.Copy(BitConverter.GetBytes(UserID), data, 8);
            data[8] = UserSetting;

            return data;

        }

        public async Task UpdateData()
            => await Database.UpdateData();
        #endregion
    }
}
