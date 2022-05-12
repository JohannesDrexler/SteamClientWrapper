using System;
using System.Runtime.Serialization;

namespace SteamClientWrapper
{
    /// <summary>
    /// This exception is thrown when a file is expected to be readable but is binary
    /// </summary>
    [Serializable]
    public class FileIsBinaryException : Exception
    {
#pragma warning disable CS1591 
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
#pragma warning restore CS1591
    }
}
