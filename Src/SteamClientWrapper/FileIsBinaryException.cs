using System;
using System.Runtime.Serialization;

namespace SteamClientWrapper
{
    [Serializable]
    public class FileIsBinaryException : Exception
    {
        public FileIsBinaryException()
        {
        }

        public FileIsBinaryException(string message) : base(message)
        {
        }

        public FileIsBinaryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileIsBinaryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
