using System;

namespace LandSky.MyMath
{
    internal class PointWithLifetime : Point
    {
        public DateTime LastUsed;

        public PointWithLifetime(int X, int Y) : base(X, Y)
        {
            LastUsed = DateTime.Now;
        }

        public Point GiveMePoint()
        {
            LastUsed = DateTime.Now;
            return this;
        }

        public static bool operator ==(PointWithLifetime one, PointWithLifetime two) => one.X == two.X &&
                                                                                   one.Y == two.Y;

        public static bool operator !=(PointWithLifetime one, PointWithLifetime two) => !(one == two);

        public static bool operator <(PointWithLifetime one, PointWithLifetime two) => one.LastUsed < two.LastUsed;

        public static bool operator >(PointWithLifetime one, PointWithLifetime two) => one.LastUsed > two.LastUsed;

        public static bool operator <=(PointWithLifetime one, PointWithLifetime two) => one.LastUsed <= two.LastUsed;

        public static bool operator >=(PointWithLifetime one, PointWithLifetime two) => one.LastUsed >= two.LastUsed;

        public override bool Equals(object obj) => obj is PointWithLifetime &&
                                                   (obj as PointWithLifetime) == this;

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash) + this.X.GetHashCode();
            hash = (hash * 7) + this.Y.GetHashCode();
            return hash;
        }
    }
}