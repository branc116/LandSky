using System;
using static System.Math;

namespace LandSky.MyMath
{
    /// <summary>
    /// Rectangle class
    /// </summary>
    public class Rectangle
    {
        private int mMx;
        private int mMy;

        public int LeftBound { get; private set; }
        public int RightBound { get; private set; }
        public int TopBound { get; private set; }
        public int BottomBound { get; private set; }

        public int DistaceToLeftBound => LeftBound - mMx;
        public int DistaceToRightBound => RightBound - mMx;
        public int DistaceToBottomBound => BottomBound - mMy;
        public int DistaceToTopBound => TopBound - mMy;

        public int X
        {
            get
            {
                return mMx;
            }
            set
            {
                mMx = value;
                int RightTs = mMx + Width / 2;
                int LeftTs = RightTs - Width + 1;
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
                int BottomTs = mMy - Height / 2;
                int TopTs = BottomTs + Height - 1;
                TopBound = TopTs;
                BottomBound = BottomTs;
            }
        }

        public Point Location => new Point(this.X, this.Y);
        public int Width => this.RightBound - this.LeftBound + 1;
        public int Height => this.TopBound - this.BottomBound + 1;

        private void InitXy()
        {
            int LR = LeftBound + RightBound;
            int TB = TopBound + BottomBound;

            X = (LR) / 2 + (LR < 0 ? LR % 2 : 0);
            Y = TB / 2 + (TB < 0 ? 0 : TB % 2);
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
            if (Width == 0 || Height == 0)
                throw new Exception("Cant have width or height 0");
            Width--;
            Height--;
            this.LeftBound = Location.X - Width / 2;
            this.RightBound = Location.X + Width / 2 + Width % 2;
            this.TopBound = Location.Y + Height / 2;
            this.BottomBound = Location.Y - Height / 2 + Height % 2;
            InitXy();
        }

        public static Rectangle DefineRectangleByWidthAndHeight(int X, int Y, int Width, int Height)
        {
            int negX = X < 0 ? 1 : 0;
            int negY = Y < 0 ? 1 : 0;
            return new Rectangle(Y + Height / 2 - negY, X + Width / 2 + Width % 2 - negX, Y - Height / 2 - Height % 2 - negY, X - Width / 2 - negX);
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

            return new Rectangle(TransformdT, TransformdR, TransformdB, TransformdL)
            {
                TopBound = TransformdT,
                RightBound = TransformdR,
                BottomBound = TransformdB,
                LeftBound = TransformdL
            };
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

        public static bool operator &(Rectangle One, Point Two) => One.LeftBound <= Two.X && One.RightBound >= Two.X &&
                                                                   One.TopBound >= Two.Y && One.BottomBound <= Two.Y;

        public static bool operator &(Point One, Rectangle Two) => Two & One;
    }
}