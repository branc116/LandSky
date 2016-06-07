using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
    class MyRectangle
    {
        MyPoint topLeft, topRight, bottomLeft, bottomRight;
        public int left
        {
            get
            {
                return topLeft.x;
            }
        }
        public int right
        {
            get
            {
                return topRight.x;
            }
        }
        public int top
        {
            get
            {
                return topLeft.y;
            }
        }
        public int bottom
        {
            get
            {
                return bottomLeft.y;
            }
        }
        public MyRectangle(MyPoint topLeft, MyPoint topRight, MyPoint bottomRight, MyPoint bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }

    }
    public class MyPoint
    {
        public int x, y;
        

        public int leftBound
        {
            get
            {
                return x-width/2;
            }
        }
        public int rightBound
        {
            get
            {
                return x+width/2;
            }
        }
        public int topBound
        {
            get
            {
                return y+height/2;
            }
        }
        public int bottomBound
        {
            get
            {
                return y-height/2;
            }
        }
        public int width, height;

        public void SizeOfScreen(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public MyPoint(int x, int y)
        {
            this.x = x;
            this.y = y;

            width = 20;
            height = 20;

        }
        public MyPoint GetStandardCoordinat(int x,int y)
        {
            MyPoint mP = new MyPoint(Math.Max(1, Math.Min(width - 2, x - leftBound)), Math.Max(1, Math.Min(height - 2, -y + topBound)));
            return mP;

        }
        public static MyPoint operator +(MyPoint a, MyPoint b)
        {
            return new MyPoint(a.x + b.x, a.y + b.y);
        }
        public static bool operator ==(MyPoint a, MyPoint b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(MyPoint a, MyPoint b)
        {
            return !(a == b);
        }

    }
    
}
