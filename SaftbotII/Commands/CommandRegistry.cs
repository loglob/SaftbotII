using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SaftbotII.Commands
{
    internal static class CommandRegistry
    {
        const string LogPrefix = "[CommandRegistry]";

        private static Dictionary<string, command> registry = new Dictionary<string, command>();

        public static void Register(command command)
            => registry.Add(command.Name.ToLower(), command);
        
        public static void Register(MethodInfo method)
        {
            var attributes = method.GetCustomAttribute(typeof(Command));

            if(attributes != null && method.IsStatic)
            {
                Command cmdAttr = (Command)attributes;
                command cmd = new command();

                cmd.Description = cmdAttr.Description;
                cmd.Usage = cmdAttr.Usage;
                cmd.Name = method.Name;

                if (method.GetParameters().Count() == 1 && method.GetParameters()[0].ParameterType == typeof(CommandInformation))
                {
                    cmd.function = a => method.Invoke(null, new object[] { a });
                    Register(cmd);
                    return;
                }
            }

            Log.Enter($"{LogPrefix} Couldn't register method '{method.Name}'!").Wait();
            return;
            
        }

        /// <summary>
        /// Registers ALL the commands
        /// </summary>
        public static async Task RegisterAll()
        {
            await Log.Enter($"{LogPrefix} Trying to register all viable commands...");

                           // Fetches all classes contained in the SaftbotII namespace (and sub-namespaces)
            var commands = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace.StartsWith("SaftbotII") && t.IsClass)
                                    // Selects all the methods of those classes
                                    .SelectMany(type => type.GetMethods())
                                    // Selects only static methods with a defined 'Command' attribute
                                    .Where(m => m.IsStatic && m.GetCustomAttributes(typeof(Command)).Count() > 0).ToList();

            await Log.Enter($"{LogPrefix} Found {commands.Count()} command(s)!");

            foreach (var cmd in commands)
                Register(cmd);
        }

        public static int RegisteredCount
            => registry.Count;

        public static command[] GetCommands
            => registry.Values.ToArray();

        public static async void Run(string commandName, CommandInformation cmdinfo)
        {
            commandName = commandName.ToLower();

            if (registry.ContainsKey(commandName))
            {
                command toRun = registry[commandName];

                if(cmdinfo.arguments.Length < toRun.ArgumentCount)
                {
                    await cmdinfo.messages.Send($"Not enough arguments given!\nProper usage: {toRun.Usage}");
                    return;
                }

                toRun.function(cmdinfo);
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

        public int ArgumentCount
            => Usage.Count(a => a == '<') - Usage.Count(a => a == '[');
    }
}
