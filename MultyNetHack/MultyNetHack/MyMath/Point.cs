using static System.Math;

namespace MultyNetHack.MyMath
{
    /// <summary>
    /// Point class
    /// </summary>
    public class Point
    {
        public int x, y;
        public bool enter;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Point Origin()
        {
            return new Point(0, 0);
        }
        public Point ToTopLeft(Size s, int x, int y)
        {
            Point mP = new Point(Max(1, Min(s.width - 1, x - this.x)), Max(1, Min(s.height - 2, -y + this.y)));
            return mP;

        }
        public static Point operator -(Point a, Point b)
        {
            return new Point(b.x - a.x, b.y - a.y);
        }
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
    }
}
