using static System.Convert;
using static System.Math;

namespace LandSky.MyMath
{
    /// <summary>
    /// Point class
    /// </summary>
    public class Point
    {
        public int X, Y;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static Point Origin()
        {
            return new Point(0, 0);
        }

        public Point ToTopLeft(Size S, int X, int Y)
        {
            Point P = new Point(Max(1, Min(S.Width - 1, X - this.X)), Max(1, Min(S.Height - 2, -Y + this.Y)));
            return P;
        }

        public int Distance(Point To)
        {
            return ToInt32(Sqrt(Pow(this.X - To.X, 2) + Pow(this.Y - To.Y, 2)));
        }

        public int DistanceOnXAxis(Point To)
        {
            return Abs(To.X - this.X);
        }

        public int AbsDerivative(Point To)
        {
            return Abs(ToInt32((this.Y - To.Y) / (this.X - To.X)));
        }

        public static Point operator -(Point A, Point B)
        {
            return new Point(B.X - A.X, B.Y - A.Y);
        }

        public static Point operator +(Point A, Point B)
        {
            return new Point(A.X + B.X, A.Y + B.Y);
        }
    }
}