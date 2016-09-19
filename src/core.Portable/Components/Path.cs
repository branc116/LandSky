using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

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
            IsInfinity = true;
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
                C.Controls.ElementAt(Rand.Next(0, C.Controls.Count/7 - 1)).Value.LocalLocation
            };
            N--;
            while (--N > 0)
            {
                Points.Add(C.Controls.Where(I => I.Value.GetType() != typeof(Path) && I.Value.GetType() != typeof(Player))
                                     .Where(K => Points.All(J => J.X != K.Value.LocalX))
                                     .OrderBy(I => Points
                                     .Sum(M => M.AbsDerivative(I.Value.LocalLocation)))
                                     .First().Value.LocalLocation);
            }

            GeneratePathThrueLocations(Points);
        }

        public bool CanFindTheSameX(IEnumerable<Point> Points, Point Point)
        {
            return Points.Any(P => Math.Abs(P.X - Point.X) == 0 || Math.Abs((P.Y - Point.Y) / (P.X - Point.X)) > 2);
        }

        public override Cell[][] GetRegin(Rectangle Rec)
        {
            var Area = new Cell[Rec.Height][];
            for (int i = 0; i < Rec.Height; i++)
            {
                Area[i] = new Cell[Rec.Width];
                for (int j = 0; j < Rec.Width; j++)
                {
                    Area[i][j] = new Cell(AsciiTexture.AsciiTextures[Material.Darknes]);
                }
            }
            for (int i = 0; i < Rec.Width; i++)
            {
                int from = (Poly.IntValueForX(Rec.LeftBound + i) + Abs(Poly.IntDerivativeForX(Rec.LeftBound + i)) - Rec.TopBound);
                int to = (Poly.IntValueForX(Rec.LeftBound + i) - Abs(Poly.IntDerivativeForX(Rec.LeftBound + i)) - Rec.TopBound);
                from = Max(0, Min(Area.Length - 1, from));
                to = Max(0, Min(Area.Length - 1, to));
                for (int j = from; j < to; j++)
                {
                    Area[j][i] = new Cell(AsciiTexture.AsciiTextures[Material.Air]) { Priority = ZValue };
                }
            }
            return Area;
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