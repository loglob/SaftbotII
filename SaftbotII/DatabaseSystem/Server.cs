using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaftbotII.DatabaseSystem
{
    internal class Server
    {
        public ulong ServerID;
        public ushort SettingsHeader;
        public const ushort StandardSettings = 0;

        private List<User> Users;

        #region FetchingUsers
        /// <summary>
        /// Finds a user object for the given user ID
        /// </summary>
        /// <param name="UserID">The user's ID</param>
        /// <param name="createIfNotExists">Create a new entry if none are found</param>
        public User FetchUser(ulong UserID, bool createIfNotExists = true)
        {
            foreach (User user in Users)
            {
                if(user.UserID == UserID)
                    return user;
            }

            if (createIfNotExists)
            {
                User newUser = new User(this, UserID);
                Users.Add(newUser);
                return newUser;
            }

            throw new Exceptions.SaftDatabaseException("Nonexistant User requested");
        }

        /// <summary>
        /// Finds a user object for the given user ID
        /// Creates a new entry if none exists
        /// </summary>
        /// <param name="userID">The user's ID</param>
        public User this[ulong userID]
        {
            get
                => FetchUser(userID);
        }

        /// <summary>
        /// Finds a user object for the given ID
        /// </summary>
        /// <param name="userID">The user's ID</param>
        /// <param name="createIfNotExists">Create a new entry if none are found</param>
        /// <returns></returns>
        public User this[ulong userID, bool createIfNotExists]
        {
            get
                => FetchUser(userID, createIfNotExists);
        }
        #endregion

        #region Settings
        /// <summary>
        /// Access the given setting of this server
        /// </summary>
        /// <param name="setting">Setting to access</param>
        public bool this[ServerSettings setting]
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
                => Util.ChopFromShort(SettingsHeader, settingID);

            set
                => SettingsHeader = Util.SetBit(value, SettingsHeader, settingID);

        }
        #endregion

        #region Constructors
        /// <summary>
        /// Fetches a Server of the given serverID
        /// </summary>
        /// <param name="ID">The server's ID</param>
        public static explicit operator Server(ulong ID)
            => Database.Fetch(ID);

        /// <summary>
        /// Constructs a new server object with the given ID
        /// </summary>
        public Server(ulong serverID = 0)
        {
            Users = new List<User>();
            SettingsHeader = StandardSettings;
            ServerID = serverID;
        }

        /// <summary>
        /// Builds a new server object from the given raw data
        /// </summary>
        /// <param name="rawdata">The raw data</param>
        /// <param name="position">The starting position for reading the data</param>
        /// <param name="readbytes">How many bytes had to be read (at least 14)</param>
        public Server(byte[] rawdata, int position, out int readbytes)
        {
            readbytes = 0;
            Users = new List<User>();

            // Read the ID of the server
            ServerID = BitConverter.ToUInt64(rawdata, position);
            readbytes += 8;
            position += 8;

            // Read the settings-header
            SettingsHeader = BitConverter.ToUInt16(rawdata, position);
            readbytes += 2;
            position += 2;

            // Determine the length of the userdata-block
            int totalLen = BitConverter.ToInt32(rawdata, position);
            readbytes += 4;
            position += 4;

            int lastpos = position + totalLen;
            while (position < lastpos)
            {
                Users.Add(new User(rawdata, position, out int bytesread));
                position += bytesread;
            }
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Turn the server into a byte array of the structure:
        /// [8 bytes] ServerID
        /// [2 bytes] Settingsheader
        /// [4 bytes] Amount of user entries
        /// [? bytes] All user entries
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(ServerID));
            data.AddRange(BitConverter.GetBytes(SettingsHeader));
            data.AddRange(BitConverter.GetBytes(Users.Count));

            foreach (User user in Users)
            {
                data.AddRange(user.Serialize());
            }

            return data.ToArray();
        }

        /// <summary>
        ///  Update the databases data to disk
        /// </summary>
        public async Task UpdateData()
            => await Database.UpdateData();
        #endregion
    }
}
