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

        /// <summary>
        /// How the method is used.
        /// "<>" denotes a necessary argument
        /// "[<>]" an optional one
        /// </summary>
        public string Description;

        /// <summary>
        /// Permission level required
        /// 0: none, 1: DJ, 2: Admin, 3: Developer
        /// if smaller than 0 even ignored users can run this command
        /// </summary>
        public int PermissionLevel = 0;

        /// <summary>
        /// Mark a method as an executable command
        /// </summary>
        /// <param name="desc">The description of what the command does</param>
        /// <param name="use">How the command is used. "<>" marks necessary arguments, "[<>]" optional ones</param>
        public Command(string desc, string use = "")
        {
            Description = desc;
            Usage = use;
        }
    }
}