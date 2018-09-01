namespace Core.Geometry
{
    /// <summary>
    ///     Utility class of helpful 3D functions
    /// </summary>
    public static class MathCore
    {
        private const float EPSILON_FLOAT = 0.0001f;
        private const double EPSILON_DOUBLE = 0.0001;

        /// <summary>
        ///     Test for near equivalence
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool EqualityTest(float a, float b)
        {
            return b - EPSILON_FLOAT < a && a < b + EPSILON_FLOAT;
        }

        /// <summary>
        ///     Test for near equivalence
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool EqualityTest(double a, double b)
        {
            if (b - EPSILON_DOUBLE < a)
                return a < b + EPSILON_DOUBLE;
            return false;
        }

        /// <summary>
        ///     Limit the range of the supplied value
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="val">The val.</param>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Clamp(int low, int val, int high)
        {
            if (val < low)
                return low;
            if (val > high)
                return high;
            return val;
        }

        /// <summary>
        ///     Limit the range of the supplied value
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="val">The val.</param>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static float Clamp(float low, float val, float high)
        {
            if (val < low)
                return low;
            if (val > high)
                return high;
            return val;
        }
    }
}