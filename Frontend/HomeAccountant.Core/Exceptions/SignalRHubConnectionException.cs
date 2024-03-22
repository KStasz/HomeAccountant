using System.Runtime.Serialization;

namespace HomeAccountant.Core.Exceptions
{
    [Serializable]
    public class SignalRHubConnectionException : Exception
    {
        public SignalRHubConnectionException()
        {
        }

        public SignalRHubConnectionException(string? message) : base(message)
        {
        }

        public SignalRHubConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}