using System.Runtime.Serialization;

namespace HomeAccountant.Core.Exceptions
{
    [Serializable]
    public class TokenStorageException : Exception
    {
        public TokenStorageException()
        {
        }

        public TokenStorageException(string? message) : base(message)
        {
        }

        public TokenStorageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TokenStorageException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
