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
    }
}