using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
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
        public Rectangle(int t,int r,int b,int l)
        {
            this.l = l;
            this.r = r;
            this.t = t;
            this.b = b;
        }
        /// <summary>
        ///     |
        ///     | l,t___
        ///     |   |x,y|
        ///     |    ---
        /// -------------------
        ///     |
        ///     |
        ///     |
        ///     |
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point ToTopLeft(int x, int y)
        {
            int top = Math.Min(this.height-1, Math.Max(1, this.t - y));
            int left = Math.Min(this.width-1, Math.Max(1, x-this.l));
            Point mP = new Point(top,left);
            return mP;

        }
    }
    public class Point
    {
        public int x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Point ToTopLeft(Size s, int x, int y)
        {
            Point mP = new Point(Math.Max(1, Math.Min(s.width - 1, x - this.x)), Math.Max(1, Math.Min(s.height - 2, -y + this.y)));
            return mP;

        }
        public static Point operator -(Point a, Point b)
        {
            return new Point(b.x - a.x, b.y - b.y);
        }
    }
    public class Size
    {
        public int height, width;
        public Size(int width, int height)
        {
            this.height = height;
            this.width = width;
        }
        
    }
}
