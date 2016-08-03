using System;
using System.Collections.Generic;

using MultyNetHack.MyMath;
using MultyNetHack.MyEnums;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Passable object that connects different rooms
    /// </summary>
    public class Path : Component
    {
        Random rnd;
        public LinearInterpolator Poly;
        public List<Component> ConnectedComponent;

        public Path(string name) : base(name)
        {
            rnd = new Random(DateTime.Now.Millisecond + (1 + DateTime.Now.Second) * 1009 + (1 + DateTime.Now.Minute) * 62761 + (1 + DateTime.Now.Hour) * 3832999);
            ConnectedComponent = new List<Component>();
            Poly = new LinearInterpolator();
            IsPassable = true;
        }

        public void generatePathThrueLocations(List<Point> Points)
        {
            Poly.Interpolate(Points, KindOfMonom.Line);
        }

        public void generatePathThrueRandomChildren(Component c)
        {
            if (c.Controls.Count < 3) throw new Exception("You can't generate path in component that has less then 3 children... sorry :(");
            int n = Math.Min(25, rnd.Next(c.Controls.Count / 25, c.Controls.Count));
            List<Point> Points = new List<Point>(n + 1);
            int counter = 0;
            while (n != 0 && counter < c.Controls.Count * 3 && Points.Count < 10)
            {
                counter++;
                int index = rnd.Next(0, c.Controls.Count);
                if (c.Controls[c.Keys[index]].GetType() == typeof(Room))
                {
                    Point candidat = new Point(c.Controls[c.Keys[index]].LocalX, c.Controls[c.Keys[index]].LocalY);
                    if (!Points.Contains(candidat) && !CanFindTheSameX(Points, candidat))
                    {
                        n--;
                        Points.Add(candidat);
                        ConnectedComponent.Add(c.Controls[c.Keys[index]]);
                    }
                }
            }
            generatePathThrueLocations(Points);
        }
        protected bool CanFindTheSameX(List<Point> Points, Point point)
        {
            foreach (Point p in Points)
            {
                if (Math.Abs(p.x - point.x) == 0 || Math.Abs((p.y - point.y) / (p.x - point.x)) > 2)
                    return true;
            }
            return false;
        }

    }

}
