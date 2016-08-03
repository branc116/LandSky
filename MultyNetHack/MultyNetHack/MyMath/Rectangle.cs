using static System.Math;

namespace MultyNetHack.MyMath
{
    /// <summary>
    /// Rectangle class
    /// </summary>
    public class Rectangle
    {

        public int l;
        public int r;
        public int t;
        public int b;
        public int width
        {
            get
            {
                return this.r - this.l;
            }
        }
        public int height
        {
            get
            {
                return this.t - this.b;
            }
        }
        public Rectangle(int t, int r, int b, int l)
        {
            this.l = l;
            this.r = r;
            this.t = t;
            this.b = b;
        }
        /// <summary>
        ///     |
        ///     | LeftBound,t___
        ///     |   |x,y|
        ///     |    ---
        /// -------------------
        ///     |
        ///     |
        ///     |
        ///     |
        /// </summary>
        /// <param Name="x"></param>
        /// <param Name="y"></param>
        /// <returns></returns>
        public Point ToTopLeft(int x, int y)
        {
            int top = Min(this.height - 1, Max(1, this.t - y));
            int left = Min(this.width - 1, Max(1, this.r - x));
            Point mP = new Point(top, left);
            return mP;


        }
        internal Rectangle ToTopLeft(Rectangle Bounds)
        {
            var TransformdT = Max(0, Min(this.height - 1, this.t - Bounds.t));
            var TransformdR = Max(0, Min(this.width - 1, Bounds.r - this.l));
            var TransformdB = Max(1, Min(this.height - 1, this.t - Bounds.b));
            var TransformdL = Max(0, Min(this.width - 1, Bounds.l - this.l));

            return new Rectangle(TransformdT, TransformdR, TransformdB, TransformdL);
        }
        public static Rectangle operator -(Rectangle rc, Point p)
        {
            return new Rectangle(rc.t - p.y, rc.r - p.x, rc.b - p.y, rc.l - p.x);
        }
        public static Rectangle operator +(Rectangle rc, Point p)
        {
            return new Rectangle(rc.t + p.y, rc.r + p.x, rc.b + p.y, rc.l + p.x);
        }
        public static bool operator &(Rectangle one, Rectangle two)
        {
            return (one.l < two.r && one.r > two.l &&
                    one.t > two.b && one.b < two.t);
        }


    }
}
