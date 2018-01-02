using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaftbotII.OfflineMode
{
    /// <summary>
    /// Reroutes the chat messages to Log
    /// </summary>
    class DebugMessaging : Messaging
    {
        public const string LoggingPrefix = "[Dummy Text Channel]";

        public DebugMessaging() : base(null)
        {

        }

        override public Task Send(string text)
        {
            Log.Enter($"{LoggingPrefix} {text}");
            return Task.CompletedTask;
        }

        public override Task Upload(string path, string comment = "")
        {
            Log.Enter($"{LoggingPrefix} Tried uploading '{path}' with comment '{comment}'");
            return Task.CompletedTask;
        }
    }
}
