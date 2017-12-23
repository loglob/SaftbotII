using System;

namespace SaftbotII.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Command : Attribute
    {
        public string Usage;
        public string Description;
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
