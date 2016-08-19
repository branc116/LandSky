using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandSky.Components
{
    /// <summary>
    /// Passable object that connects different rooms
    /// </summary>
    public class Path : Component
    {
        public LinearInterpolator Poly;
        public List<Component> ConnectedComponent;

        public Path(string name) : base(name)
        {
            Rand = new Random(DateTime.Now.Millisecond + (1 + DateTime.Now.Second) * 1009 + (1 + DateTime.Now.Minute) * 62761 + (1 + DateTime.Now.Hour) * 3832999);
            ConnectedComponent = new List<Component>();
            Poly = new LinearInterpolator();
            IsPassable = true;
        }

        public void GeneratePathThrueLocations(List<Point> Points)
        {
            Poly.Interpolate(Points, KindOfMonom.Line);
        }

        public void GeneratePathThrueRandomChildren(Component C)
        {
            if (C.Controls.Count < 3) throw new Exception("You can't generate path in component that has less then 3 children... sorry :(");
            int N = Math.Min(3, Rand.Next(C.Controls.Count / 4, C.Controls.Count));
            var Points = new List<Point>(N + 1)
            {
                C.Controls.ElementAt(Rand.Next(0, C.Controls.Count/7 - 1)).Value.LocalBounds.Location
            };
            N--;
            while (--N > 0)
            {
                Points.Add(C.Controls.Where(I => I.Value.GetType() != typeof(Path) && I.Value.GetType() != typeof(Player))
                                     .Where(K => Points.All(J => J.X != K.Value.LocalBounds.Location.X))
                                     .OrderBy(I => Points
                                     .Sum(M => M.AbsDerivative(I.Value.LocalBounds.Location)))
                                     .First().Value.LocalBounds.Location);
            }

            GeneratePathThrueLocations(Points);
        }

        public bool CanFindTheSameX(IEnumerable<Point> Points, Point Point)
        {
            return Points.Any(P => Math.Abs(P.X - Point.X) == 0 || Math.Abs((P.Y - Point.Y) / (P.X - Point.X)) > 2);
        }

        public static bool operator &(Path One, Point Two)
        {
            return One.Poly.DerivativeForX(Two.X) + One.Poly.ValueForX(Two.X) + 2 > Two.Y && One.Poly.ValueForX(Two.X) - One.Poly.DerivativeForX(Two.X) - 2 < Two.Y;
        }

        public bool IsOnPath(Point point)
        {
            return this & point;
        }
    }
}