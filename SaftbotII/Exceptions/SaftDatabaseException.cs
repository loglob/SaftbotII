using System;

namespace SaftbotII.Exceptions
{
    internal class SaftDatabaseException : SaftEceptions
    {
        public SaftDatabaseException(string message = "", Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
