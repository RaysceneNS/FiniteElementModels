using System;
using System.Runtime.Serialization;

namespace UI.Controls.Viewport
{
    [Serializable]
    public class GLException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GLException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GLException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GLException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        ///     The class name is null or
        ///     <see cref="P:System.Exception.HResult"></see> is zero (0).
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected GLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}