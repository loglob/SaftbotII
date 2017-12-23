using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SaftbotII.DatabaseSystem
{
    /// <summary>
    /// A custom-made database because big fancy databases aren't needed here
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// The internal list of servers
        /// </summary>
        private static List<Server> Servers;

        public const string PathToDB = "./saftDB.sdb";
        
        #region Serialiation
        /// <summary>
        /// Loads the database from disk
        /// </summary>
        /// <param name="overridePath">Reads a specified .sdb rather than the standard file</param>
        public static async Task BuildFromFile(string overridePath = PathToDB)
        {
            if (File.Exists(PathToDB))
            {
                byte[] rawdata = await File.ReadAllBytesAsync(PathToDB);
                Servers = new List<Server>();
                int toJump;

                for (int i = 0; i < rawdata.Length; i += toJump)
                {
                    Servers.Add(new Server(rawdata, i, out toJump));
                }
            }
            else
            {
                Servers = new List<Server>();
                await UpdateData();
            }
        }
        
        /// <summary>
        /// Turns the database into a byte[] for writing it to disk
        /// </summary>
        public static byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            foreach (Server server in Servers)
            {
                data.AddRange(server.Serialize());
            }

            return data.ToArray();
        }

        /// <summary>
        /// Pushes the changes to the database to disk
        /// </summary>
        public static async Task UpdateData()
            => await File.WriteAllBytesAsync(PathToDB, Serialize());
        #endregion

        #region Fetching
        /// <summary>
        /// Retrieves the server object belonging to the given serverID
        /// If no server is found, either creates a new empty entry or throws an exception
        /// </summary>
        /// <param name="serverID">The server's ID</param>
        /// <param name="createIfNotExists">Inserts a new entry in the databse if none is found</param>
        public static Server Fetch(ulong serverID, bool createIfNotExists = true)
        {
            foreach (Server server in Servers)
            {
                if (server.ServerID == serverID)
                    return server;
            }

            if(createIfNotExists)
            {
                Server newServer = new Server(serverID);
                Servers.Add(newServer);
                return newServer;
            }

            throw new Exceptions.SaftDatabaseException("Nonexistant Server requested");
        }

        /// <summary>
        /// Retrieves the user object belonging to the given UserID on the Server under the given ID
        /// </summary>
        /// <param name="serverID">The server's ID</param>
        /// <param name="userID">The user's ID</param>
        /// <param name="createUserIfNotExists">Create user entry if none is found</param>
        /// <param name="createServerIfNotExists">Create server entry if none is found</param>
        public static User Fetch(ulong serverID, ulong userID, bool createUserIfNotExists = true, bool createServerIfNotExists = true)
        {
            return Fetch(serverID, createServerIfNotExists)[userID, createUserIfNotExists];
        }
        #endregion

        /// <summary>
        /// Creates a new entry for the given serverID
        /// </summary>
        public static void NewEntry(ulong serverID)
        {
            Servers.Add(new Server(serverID));
        }
    }
}
