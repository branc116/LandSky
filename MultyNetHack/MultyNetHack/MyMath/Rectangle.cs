using static System.Math;

namespace MultyNetHack.MyMath
{
    /// <summary>
    /// Rectangle class
    /// </summary>
    public class Rectangle
    {
        private int mMx;
        private int mMy;

        public int LeftBound { get; set; }
        public int RightBound { get; set; }
        public int TopBound { get; set; }
        public int BottomBound { get; set; }

        public int DistaceToLeftBound => mMx - LeftBound;
        public int DistaceToRightBound => RightBound - mMx;
        public int DistaceToBottomBound => mMy - LeftBound;
        public int DistaceToTopBound => RightBound - mMx;

        public int X
        {
            get
            {
                return mMx;
            }
            set
            {
                mMx = value;
                int LeftTs = mMx - Width / 2;
                int RightTs = mMx + Width / 2 + Width % 2;
                LeftBound = LeftTs;
                RightBound = RightTs;
            }
        }
        public int Y
        {
            get
            {
                return mMy;
            }
            
            set
            {
                mMy = value;
                int TopTs = mMy + Height / 2;
                int BottomTs = mMy - Height / 2 - Height % 2;
                TopBound = TopTs;
                BottomBound = BottomTs;
            }
        }
        public Point Location => new Point(this.X, this.Y);
        public int Width => this.RightBound - this.LeftBound;
        public int Height => this.TopBound - this.BottomBound;

        private void InitXy()
        {
            X = (LeftBound + RightBound) / 2;
            Y = (TopBound + BottomBound) / 2;
        }
        
        public Rectangle(int T, int R, int B, int L)
        {
            this.LeftBound = L;
            this.RightBound = R;
            this.TopBound = T;
            this.BottomBound = B;
            InitXy();
        }
        public Rectangle(Point Location, int Width, int Height)
        {
            this.LeftBound = Location.X - Width / 2;
            this.RightBound = Location.X + Width / 2 + Width % 2;
            this.TopBound = Location.Y + Height / 2;
            this.BottomBound = Location.Y - Height / 2 + Height % 2;
            InitXy();
        }
        public static Rectangle DefineRectangleByWidthAndHeight(int X, int Y, int Width, int Height)
        {
            return new Rectangle(Y + Height / 2, X + Width / 2 + Width % 2, Y - Height / 2 - Height % 2, X - Width / 2);
        }
        /// <summary>
        /// Convert from Cartesian coordinates system to Top Left coordinates
        /// </summary>
        /// <param Name="x">X coordinate in Cartesian coordinate system </param>
        /// <param Name="y">Y coordinate in Cartesian coordinate system</param>
        /// <returns>Returns a Top Left style point </returns>
        public Point ToTopLeft(int x, int y)
        {
            int Top = Min(this.Height - 1, Max(1, this.TopBound - y));
            int Left = Min(this.Width - 1, Max(1, this.RightBound - x));
            Point P = new Point(Top, Left);
            return P;
        }
        /// <summary>
        /// Convert from Cartesian coordinates system to Top Left coordinates
        /// </summary>
        /// <param Name="Bounds">Rectangle you want to transform</param>
        /// <returns>Returns a Top Left style Rectangle </returns>
        public Rectangle ToTopLeft(Rectangle Bounds)
        {
            var TransformdT = Max(0, Min(this.Height - 1, this.TopBound - Bounds.TopBound));
            var TransformdR = Max(0, Min(this.Width - 1, Bounds.RightBound - this.LeftBound));
            var TransformdB = Max(1, Min(this.Height - 1, this.TopBound - Bounds.BottomBound));
            var TransformdL = Max(0, Min(this.Width - 1, Bounds.LeftBound - this.LeftBound));

            return new Rectangle(TransformdT, TransformdR, TransformdB, TransformdL);
        }
        public static Rectangle operator -(Rectangle Rc, Point P)
        {
            return new Rectangle(Rc.TopBound - P.Y, Rc.RightBound - P.X, Rc.BottomBound - P.Y, Rc.LeftBound - P.X);
        }
        public static Rectangle operator +(Rectangle Rc, Point P)
        {
            return new Rectangle(Rc.TopBound + P.Y, Rc.RightBound + P.X, Rc.BottomBound + P.Y, Rc.LeftBound + P.X);
        }
        public static Rectangle operator +(Rectangle One, Rectangle Two)
        {
            return new Rectangle(One.TopBound + Two.Y, One.RightBound + Two.X, One.BottomBound + Two.Y, One.LeftBound + Two.X);
        }
        public static bool operator &(Rectangle One, Rectangle Two)
        {
            return (One.LeftBound < Two.RightBound && One.RightBound > Two.LeftBound &&
                    One.TopBound > Two.BottomBound && One.BottomBound < Two.TopBound);
        }
        public static bool operator &(Rectangle One, Point Two) => One.LeftBound < Two.X && One.RightBound > Two.X &&
                                                                   One.TopBound > Two.Y && One.BottomBound < Two.Y; 
        public static bool operator &(Point One, Rectangle Two) => Two & One;

    }
}
