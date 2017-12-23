using System;

namespace SaftbotII.Commands
{
    /// <summary>
    /// A command that gets auto-registered if it's method is static
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Command : Attribute
    {
        /// <summary>
        /// The expected arguments for this command
        /// <> denotes a necessary parameter, [<>] an optional one
        /// </summary>
        public string Usage;

        public string Description;

        /// <summary>
        /// Permission level required
        /// 0: none, 1: DJ, 2: Admin, 3: Developer
        /// if smaller than 0 even ignored users cna run this command
        /// </summary>
        public int PermissionLevel;

        public Command(string desc, string use = "", int permissionLevel = 0)
        {
            Description = desc;
            Usage = use;
            PermissionLevel = permissionLevel;
        }

        public Command(string desc, int permissionLevel) : this(desc, "", permissionLevel)
        {   }
    }
}