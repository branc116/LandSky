using static System.Math;

namespace MultyNetHack.MyMath
{
    /// <summary>
    /// Rectangle class
    /// </summary>
    public class Rectangle
    {

        private int mLeftBound;
        private int mRightBound;
        private int mTopBound;
        private int mBottomBound;

        public int LeftBound
        {
            get
            {
                return mLeftBound;
            }

            set
            {
                mLeftBound = value;
            }
        }
        public int RightBound
        {
            get
            {
                return mRightBound;
            }

            set
            {
                mRightBound = value;
            }
        }
        public int TopBound
        {
            get
            {
                return mTopBound;
            }

            set
            {
                mTopBound = value;
            }
        }
        public int BottomBound
        {
            get
            {
                return mBottomBound;
            }

            set
            {
                mBottomBound = value;
            }
        }

        public int X
        {
            get
            {
                return (LeftBound + RightBound) / 2;
            }
            set
            {
                LeftBound = value - Width / 2;
                RightBound = value + Width / 2;
            }
        }
        public int Y
        {
            get
            {
                return (TopBound + BottomBound) / 2;
            }
            set
            {
                TopBound = value + height / 2;
                BottomBound = value - height / 2;
            }
        }

        public int Width
        {
            get
            {
                return this.RightBound - this.LeftBound;
            }
        }
        public int height
        {
            get
            {
                return this.TopBound - this.BottomBound;
            }
        }

        
        public Rectangle(int t, int r, int b, int l)
        {
            this.LeftBound = l;
            this.RightBound = r;
            this.TopBound = t;
            this.BottomBound = b;
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
            int top = Min(this.height - 1, Max(1, this.TopBound - y));
            int left = Min(this.Width - 1, Max(1, this.RightBound - x));
            Point mP = new Point(top, left);
            return mP;


        }
        internal Rectangle ToTopLeft(Rectangle Bounds)
        {
            var TransformdT = Max(0, Min(this.height - 1, this.TopBound - Bounds.TopBound));
            var TransformdR = Max(0, Min(this.Width - 1, Bounds.RightBound - this.LeftBound));
            var TransformdB = Max(1, Min(this.height - 1, this.TopBound - Bounds.BottomBound));
            var TransformdL = Max(0, Min(this.Width - 1, Bounds.LeftBound - this.LeftBound));

            return new Rectangle(TransformdT, TransformdR, TransformdB, TransformdL);
        }
        public static Rectangle operator -(Rectangle rc, Point p)
        {
            return new Rectangle(rc.TopBound - p.y, rc.RightBound - p.x, rc.BottomBound - p.y, rc.LeftBound - p.x);
        }
        public static Rectangle operator +(Rectangle rc, Point p)
        {
            return new Rectangle(rc.TopBound + p.y, rc.RightBound + p.x, rc.BottomBound + p.y, rc.LeftBound + p.x);
        }
        public static Rectangle operator +(Rectangle one, Rectangle two)
        {
            return new Rectangle(one.TopBound + two.Y, one.RightBound + two.X, one.BottomBound + two.Y, one.LeftBound + two.X);
        }
        public static bool operator &(Rectangle one, Rectangle two)
        {
            return (one.LeftBound < two.RightBound && one.RightBound > two.LeftBound &&
                    one.TopBound > two.BottomBound && one.BottomBound < two.TopBound);
        }


    }
}
