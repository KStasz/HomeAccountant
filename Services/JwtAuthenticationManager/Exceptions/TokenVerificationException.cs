using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager.Exceptions
{
    [Serializable]
    public class TokenVerificationException : Exception
    {
        public TokenVerificationException()
        {
        }

        public TokenVerificationException(string? message) : base(message)
        {
        }

        public TokenVerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TokenVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
