using System;

namespace SaftbotII.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Command : Attribute
    {
        public string Usage;
        public string Description;

        public Command(string desc, string use = "")
        {
            Description = desc;
            Usage = use;
        }
    }
}
