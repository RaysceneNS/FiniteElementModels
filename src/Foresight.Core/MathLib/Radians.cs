using System;

namespace Core.MathLib
{
    public static class Radians
    {
        /// <summary>
        ///     Convert radians to degrees
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static float ToDegrees(float angle)
        {
            return (float) (angle * 180.0 / Math.PI);
        }
    }
}