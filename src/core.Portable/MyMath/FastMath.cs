using System;

namespace LandSky.MyMath
{
    public static class FastMath
    {
        public static int ipow(int Base, int Exp)
        {
            int result = 1;
            while (0 != Exp)
            {
                if (0 != (Exp & 1))
                    result *= Base;
                Exp >>= 1;
                Base *= Base;
            }

            return result;
        }

        public static int DoubleToInt(double Double)
        {
            return (int)Math.Round(Double);
        }

        /// <summary>
        /// Returns true if this Num is inside this range [LowerBound, UpperBound>
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="UpperBound"></param>
        /// <param name="LowerBound"></param>
        /// <returns></returns>
        public static bool IsInside(int Num, int LowerBound, int UpperBound)
        {
            return LowerBound <= Num && Num < UpperBound;
        }
    }
}