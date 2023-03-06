using System;

namespace Almond.Util
{

    [Serializable]
    public class EventException : Exception
    {
        public EventException(String message)
            : base(message)
        {
        }

        public EventException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}