using System;

namespace Core.MathLib
{
    public static class Degrees
    {
        /// <summary>
        ///     Convert degrees to radians
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static float ToRadians(float angle)
        {
            return  angle * (float)Math.PI / 180.0f;
        }
    }
}