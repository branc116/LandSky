using LandSky.MyEnums;
using static System.Math;

namespace LandSky.MyMath
{
    internal class Monom3D : Monom
    {
        /// <summary>
        /// It looks like this: f(x,y)=ParamaterA*sin(ParamaterB*x*y + ParamaterA)
        /// </summary>
        /// <param name="ParamterA"></param>
        /// <param name="ParamaterB"></param>
        public Monom3D(double ParamaterA, double ParamaterB) : base(KindOfMonom.Sine, ParamaterA, ParamaterB)
        {
        }

        public bool IsBiggerThanZero(int x, int y)
        {
            return Sin(ParamaterB * x * y + ParamaterA) > 0;
        }

        public double ValueForX(int x, int y)
        {
            return Cos(ParamaterA * x * y + ParamaterB) * Sin(ParamaterB * x * y + ParamaterA);
        }

        public double ValueForX(Point Location)
        {
            return ValueForX(Location.X, Location.Y);
        }
    }
}