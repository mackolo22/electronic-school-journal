using System;
using System.Runtime.Serialization;

namespace ApplicationCore.Exceptions.AzureStorage
{
    public class TableException : Exception
    {
        public TableException() { }

        public TableException(string message) : base(message) { }

        public TableException(string message, Exception innerException) : base(message, innerException) { }

        protected TableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
