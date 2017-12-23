using System;

namespace SaftbotII.Exceptions
{
    internal class SaftEceptions : Exception
    {
        public const string repoLink = @"https://github.com/loglob/SaftbotNET";

        public SaftEceptions(string message = "", Exception innerException = null) : base(message, innerException)
        {
            HelpLink = repoLink;
        }
    }
}
