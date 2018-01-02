using System;

namespace SaftbotII.Exceptions
{
    internal class SaftException : Exception
    {
        public const string repoLink = @"https://github.com/loglob/SaftbotII";

        public SaftException(string message = "", Exception innerException = null) : base(message, innerException)
        {
            HelpLink = repoLink;
        }
    }
}
