using System;
using System.Runtime.Serialization;

namespace UI.Controls.Viewport
{
    [Serializable]
    public class GLException : Exception
    {
        public GLException(string message)
            : base(message)
        {
        }

        protected GLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}