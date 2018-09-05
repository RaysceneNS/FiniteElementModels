using System;
using System.Runtime.Serialization;

namespace UI.Controls.Viewport
{
    [Serializable]
    public class GLContextException : Exception
    {
        public GLContextException(string message)
            : base(message)
        {
        }

        protected GLContextException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}