using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SaftbotII.Commands
{
    /// <summary>
    /// 
    /// </summary>
    internal static class CommandRegistry
    {
        /// <summary>
        /// Prepended to all Log messages from this module
        /// </summary>
        const string LogPrefix = "[CommandRegistry]";

        /// <summary>
        /// The internal registry for commands
        /// </summary>
        private static Dictionary<string, command> registry = new Dictionary<string, command>();

        /// <summary>
        /// Adds an enry for the given command
        /// </summary>
        /// <param name="command"></param>
        public static void Register(command command)
            => registry.Add(command.Name.ToLower(), command);
        
        /// <summary>
        /// Auto-registeres a given method as a command
        /// Requires method to have a Command attribute
        /// </summary>
        /// <param name="method">void method accepting CommandInformation as only argument</param>
        public static void Register(MethodInfo method)
        {
            var attribute = method.GetCustomAttribute(typeof(Command));

            if(attribute != null && method.IsStatic)
            {
                Command cmdAttr = (Command)attribute;
                command cmd = new command
                {
                    Description = cmdAttr.Description,
                    Usage = cmdAttr.Usage,
                    Name = method.Name,
                    PermissionLevel = cmdAttr.PermissionLevel
                };

                if (method.GetParameters().Count() == 1 && method.GetParameters()[0].ParameterType == typeof(CommandInformation))
                {
                    cmd.function = a => method.Invoke(null, new object[] { a });
                    Register(cmd);
                    return;
                }
            }

            Log.Enter($"{LogPrefix} Couldn't register method '{method.Name}'!");
            return;
            
        }

        /// <summary>
        /// Registers all static methods with a Command attribute that are currently defined as command
        /// </summary>
        public static void RegisterAll()
        {
            Log.Enter($"{LogPrefix} Trying to register all viable commands...");

                           // Fetches all classes contained in the SaftbotII namespace (and sub-namespaces)
            var commands = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace.StartsWith("SaftbotII") && t.IsClass)
                                    // Selects all the methods of those classes
                                    .SelectMany(type => type.GetMethods())
                                    // Selects only static methods with a defined 'Command' attribute
                                    .Where(m => m.IsStatic && m.GetCustomAttributes(typeof(Command)).Count() > 0).ToList();

            Log.Enter($"{LogPrefix} Found {commands.Count()} command(s)!");

            foreach (var cmd in commands)
                Register(cmd);
        }

        /// <summary>
        /// The Amount of registered commands
        /// </summary>
        public static int RegisteredCount
            => registry.Count;
        
        public static command[] GetCommands
            => registry.Values.ToArray();

        /// <summary>
        /// Automatically runs the command of the given name
        /// checks argument counts and permission levels
        /// </summary>
        public static async void Run(string commandName, CommandInformation cmdinfo)
        {
            commandName = commandName.ToLower();

            if (registry.ContainsKey(commandName))
            {
                command toRun = registry[commandName];

                if (cmdinfo.arguments.Length < toRun.ArgumentCount)
                   await cmdinfo.messages.Send($"Not enough arguments given!\nProper usage: {toRun.Usage}"); 
                else
                {
                    if (cmdinfo.AuthorEntry.PermissionLevel >= toRun.PermissionLevel)
                        toRun.function(cmdinfo);
                    else
                        await cmdinfo.messages.Send($"You do not have the required permissions to run this command!");
                }
            }
        }

        internal static command? Fetch(string command)
        {
            command = command.ToLower();

            if (!registry.ContainsKey(command))
                return null;

            return registry[command];
        }
    }
    
    struct command
    {
        public Action<CommandInformation> function;
        public string Usage;
        public string Description;
        public string Name;
        public int PermissionLevel;

        public int ArgumentCount
            => Usage.Count(a => a == '<') - Usage.Count(a => a == '[');
    }
}