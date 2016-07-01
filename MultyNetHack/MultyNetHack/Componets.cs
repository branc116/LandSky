using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace MultyNetHack
{
    

    public abstract class Component {
        public int x, y, z;
        public int l, r, t, b;
        public Dictionary<string, Component> controls;
        public List<string> keys;
        public List<Sweep> sweep;
        public int height, width, depht;
        public Component parent;
        public Material madeOf;
        public string name;
        public int numOfRooms, numOfWalls, numOfPaths;

        public Component(string name)
        {
            controls = new Dictionary<string, Component>();
            keys = new List<string>();
            sweep = new List<Sweep>();
            this.name = name;

        }
        public struct Sweep
        {
            public Component component;
            public int x;
            public bool enter;

            public Sweep(Component component, int x, bool enter)
            {
                this.component = component;
                this.x = x;
                this.enter = enter;
            }

            public static bool operator <(Sweep a, Sweep b)
            {
                return a.x < b.x;
            }
            public static bool operator >(Sweep a, Sweep b)
            {
                return a.x > b.x;
            }
            public static bool operator <=(Sweep a, Sweep b)
            {
                return a.x <= b.x;
            }
            public static bool operator >=(Sweep a, Sweep b)
            {
                return a.x >= b.x;
            }
            public static bool operator ==(Sweep a, Sweep b)
            {
                return a.x == b.x;
            }
            public static bool operator !=(Sweep a, Sweep b)
            {
                return a.x != b.x;
            }
            public static bool operator <(Sweep a, int b)
            {
                return a.x < b;
            }
            public static bool operator >(Sweep a, int b)
            {
                return a.x > b;
            }
            public static bool operator <=(Sweep a, int b)
            {
                return a.x <= b;
            }
            public static bool operator >=(Sweep a, int b)
            {
                return a.x >= b;
            }
            public static bool operator ==(Sweep a, int b)
            {
                return a.x == b;
            }
            public static bool operator !=(Sweep a, int b)
            {
                return a.x != b;
            }

            public static bool operator ==(Sweep a, string b)
            {
                return a.component.name == b;
            }
            public static bool operator !=(Sweep a, string b)
            {
                return a.component.name != b;
            }

            public static bool operator ==(Sweep a, Component b)
            {
                return a.component == b;
            }
            public static bool operator !=(Sweep a, Component b)
            {
                return a.component != b;
            }
        }


        public void Dispose()
        {
            this.parent.keys.Remove(this.name);
            this.parent.controls.Remove(this.name);
            sweep = null;
            controls = null;
            parent = null;

        }
        public void Insert(Component c)
        {
            if (c.name == null) throw new Exception("Component must have a name");
            this.keys.Add(c.name);
            this.controls.Add(c.name, c);
            switch (c.GetType().ToString())
            {
                case "MultyNetHack.Room":
                    numOfRooms++;
                    break;
                case "MultyNetHack.Path":
                    numOfPaths++;
                    break;
                case "MultyNetHack.HorisontalWall":
                    numOfWalls++;
                    break;
                case "MultyNetHack.VerticalWall":
                    numOfWalls++;
                    break;
            }

            if (c.parent != null)
            {
                c.parent.keys.Remove(c.name);
                c.parent.controls.Remove(c.name);
            }
            c.parent = this;
        }
        public void InsertInSweep(Component c)
        {
            
            int lb = 0, ub = sweep.Count;
            int x1 = c.l;
            int x2 = c.r;
            if (sweep.Count == 0)
            {
                sweep.InsertRange(0, new Sweep[2] { new Sweep(c, x1, true), new Sweep(c, x2, false) });
                return;
            }
            if (x1 < sweep[0].x)
            {
                sweep.Insert(0, new Sweep(c, x1, true));
            }
            else
            {
                while (ub - lb > 1)
                {
                    int mid = (lb + ub) / 2;
                    if (sweep[mid] < x1)
                    {
                        lb = mid;
                    }
                    else if (sweep[mid] > x1)
                    {
                        ub = mid;
                    }
                    else
                    {
                        lb = ub = mid;
                    }
                }
                sweep.Insert(ub, new Sweep(c, x1, true));
            }
            
            ub = sweep.Count;
            while (ub - lb > 1)
            {
                int mid = (lb + ub) / 2;
                if (sweep[mid] < x2)
                {
                    lb = mid;
                }
                else if (sweep[mid] > x2)
                {
                    ub = mid;
                }
                else
                {
                    lb = ub = mid;
                }
            }
            sweep.Insert(ub, new Sweep(c, x2, false));

        }
        public void Delete(string name)
        {
            Component c = controls[name];
            foreach (string s in c.keys)
            {
                c.controls[s].Delete(s);
            }
            foreach(Sweep s in sweep)
            {
                if (s == name)
                {
                    sweep.Remove(s);
                }
            }
            keys.Remove(name);
            controls.Remove(name);

        }
        public void Delete(int i)
        {
            Delete(keys[i]);
        }
        public Component GetComponentOnLocation(int x, int y)
        {
            Point p = new Point(x, y);
            return GetComponentOnLocation(p);
        }
        public Component GetComponentOnLocation(Point point)
        {
            Point startEnd = GetStartEndEnter(point.x +1);
            
            for (int i = startEnd.x; i < startEnd.y; i++) {
                if (sweep[i].enter == startEnd.enter)
                {
                    Component c = sweep[i].component;
                    if (c & point)
                        return c.GetComponentOnLocation(new Point(c.x, c.y) - point);
                }
            }
            Component solution = this;
            controls.Where(i => i.Value.GetType() == typeof(Path)).ToList().ForEach((i) =>
              {
                  Path p = i.Value as Path;
                  int delta = Convert.ToInt32(Math.Abs(point.y - p.Poly.ValueForX(point.x)));
                  int range = Convert.ToInt32(2 + Math.Abs(p.Poly.DerivativeForX(point.x)));
                  if (delta <= range)
                      solution = p;
                   
              });
            return solution;
        }
        public Point GetStartEndEnter(int xx)
        {
            int lb = 0, ub = sweep.Count;
            Point startEnd = new Point(0, 0);
            while (ub - lb > 1)
            {
                int mid = (lb + ub) / 2;
                if (sweep[mid] < xx)
                {
                    lb = mid;
                }
                else if (sweep[mid] >= xx)
                {
                    ub = mid;
                }
                else
                {
                    lb = ub = mid;
                    ub++;
                }
            }

            if (sweep.Count-lb < ub)
            {
                startEnd.x = ub;
                startEnd.y = sweep.Count;
                startEnd.enter = false;
                return startEnd;
            }
            startEnd.x = 0;
            startEnd.y = ub;
            startEnd.enter = true;
            return startEnd;
        }
        public Point GetStartEndEnter(int l, int r)
        {
            int lb = 0, ub = sweep.Count;
            Point startEnd = new Point(0,0);
            if (sweep.Count==0 || l <= sweep[0].x)
            {
                startEnd.x = 0;
            }
            else
            {
                while (ub - lb > 1)
                {
                    int mid = (lb + ub) / 2;
                    if (sweep[mid] < l)
                    {
                        lb = mid;
                    }
                    else if (sweep[mid] > l)
                    {
                        ub = mid;
                    }
                    else
                    {
                        lb = ub = mid;
                    }
                }
                startEnd.x = ub;
                ub = sweep.Count;
            }
            while (ub - lb > 1)
            {
                int mid = (lb + ub) / 2;
                if (sweep[mid] < r)
                {
                    lb = mid;
                }
                else if (sweep[mid] > r)
                {
                    ub = mid;
                }
                else
                {
                    lb = ub = mid;
                }
            }
            //if(lb>0)
            //    startEnd.y = lb;
            //else
            //    startEnd.y = Math.Min(sweep.Count-1,ub);
            startEnd.y = Math.Min(sweep.Count-1, lb);
            return startEnd;
        }
        //check for intersection in two rooms
        public static bool operator &(Component one, Component two)
        {
            return (one.l <= two.r && one.r >= two.l &&
                one.t >= two.b && one.b <= two.t);
        }
        public static bool operator &(Component one, Player two)
        {
            return (one.l <= two.rightBound && one.r >= two.leftBound &&
                one.t >= two.bottomBound && one.b <= two.topBound);
        }
        public static bool operator &(Component one, Point two)
        {
            return (one.t>=two.y && one.b<two.y && one.l <=two.x && one.r>two.x);
        }
        public static bool operator &(Component one, Rectangle two)
        {
            return (one.l < two.r && one.r > two.l &&
                    one.t > two.b && one.b < two.t);
        }
        public static bool operator ==(Component one, Component two)
        {
            if (one.t == two.t &&
                one.b == two.b &&
                one.l == two.l &&
                one.r == two.r &&
                one.name == two.name) return true;
            return false;
        }
        public static bool operator !=(Component one, Component two)
        {
            try
            {
                return !(one == two);
            }
            catch
            {
                return false;
            }
        }
        public static Rectangle operator +(Component c, Point p)
        {
            return new Rectangle(c.t + p.y, c.r + p.x, c.b + p.y, c.l + p.x);
        }
    }
    public class Room : Component
    {
        private int genStack;
        private Random rand;

        public Room(string name) : base(name)
        {
            genStack = 0;
            rand = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 7187 + DateTime.Now.Minute * 8167);
        }
        public void GeneratRandom()
        {
        START:;
            genStack++;
            
            x = rand.Next(-500, 500);
            y = rand.Next(-500, 500);
            width = rand.Next(15, 40);
            height = rand.Next(7, 20);
            l = x - width  /2;
            t = y + height /2;
            r = x + width  /2;
            b = y - height /2;
            width = r - l;
            height = t - b;
            madeOf = Material.Air;
            z = parent.z + 2;
            if (CollisionCheck(this) || width * height < 40)
                if (genStack > 1000)
                {
                    this.Dispose();
                }
                else
                    goto START;
            else
            {
                parent.InsertInSweep(this);
                GenerateWall();
            }
        }
        public bool CollisionCheck(Room r)
        {
            Point startEnd;
            startEnd = parent.GetStartEndEnter(r.x);
            // make it faster!!!
            for  (int i = 0; i < parent.sweep.Count; i++)
            {
                if (parent.sweep[i].enter==startEnd.enter && parent.sweep[i].component & r)
                    return true;
            }
            return false;
        }
        public void GenerateWall()
        {

            HorisontalWall wT = new HorisontalWall("TopWall" + this.name, new Point(0, t - y), x - l, r - x);
            HorisontalWall wB = new HorisontalWall("BottomWall" + this.name, new Point(0, b - y + 1), x - l, r - x);
            VerticalWall wL = new VerticalWall("LeftWall" + this.name,new Point(l - x , 0), t - y - 1, y - b -1);
            VerticalWall wR = new VerticalWall("RightWall" + this.name,new Point(r - x - 1, 0), t - y - 1, y - b);
            wT.z = z + 1;
            wB.z = z + 1;
            wL.z = z + 1;
            wR.z = z + 1;
            this.Insert(wT);
            this.Insert(wB);
            this.Insert(wL);
            this.Insert(wR);
            this.InsertInSweep(wT);
            this.InsertInSweep(wB);
            this.InsertInSweep(wL);
            this.InsertInSweep(wR);
        }

    }
    public class Player : Component
    {
        
        public int leftBound
        {
            get
            {
                return x - width / 2;
            }
        }
        public int rightBound
        {
            get
            {
                return x + width / 2;
            }
        }
        public int topBound
        {
            get
            {
                return y + height / 2;
            }
        }
        public int bottomBound
        {
            get
            {
                return y - height / 2;
            }
        }

        public void SizeOfScreen(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public Player(int x, int y) : base("Player")
        {
            this.x = x;
            this.y = y;

            width = 1;
            height = 1;
            madeOf = Material.Player;
        }
        
        public static Player operator +(Player a, Player b)
        {
            return new Player(a.x + b.x, a.y + b.y);
        }
        public static bool operator ==(Player a, Player b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Player a, Player b)
        {
            return !(a == b);
        }

    }
    public class HorisontalWall : Component
    {
        public HorisontalWall(string name, Point centerLoc, int sizeLeft, int sizeRight) : base(name)
        {
            x = centerLoc.x;
            y = centerLoc.y;
            t = y;
            b = y - 1;
            l = -sizeLeft;
            r = sizeRight;
            madeOf = Material.HorisontalWall;
            height = t - b;
            width = r - l;
        }
    }
    public class VerticalWall : Component
    {
        public VerticalWall(string name, Point location, int sizeUp, int sizeDown) : base(name)
        {
            x = location.x;
            y = location.y;
            l = x;
            r = x + 1;
            width = 1;
            height = sizeUp + sizeDown;
            t = sizeUp;
            b = -sizeDown;
            madeOf = Material.VerticalWall;
        }
    }
    public class Root : Component
    {
        public Root() : base("Root")
        {
            numOfPaths = numOfRooms = 0;
            z = 0;
            madeOf = Material.Darknes;
            width = int.MaxValue;
            height = int.MaxValue;
        }
    }
    public class Path : Component
    {
        Random rnd;
        public LinearInterpolator Poly;
        public List<Component> ConnectedComponent;
        
        public Path(string name):base(name)
        {
            rnd = new Random(DateTime.Now.Millisecond + (1 + DateTime.Now.Second) * 1009 + (1 + DateTime.Now.Minute) * 62761 + (1 + DateTime.Now.Hour) * 3832999);
            ConnectedComponent = new List<Component>();
            Poly = new LinearInterpolator();
        }

        public void generatePathThrueLocations(List<Point> Points)
        {
            Poly.Interpolate(Points, KindOfMonom.Line);
        }

        public void generatePathThrueRandomChildren(Component c)
        {
            if (c.controls.Count < 3) throw new Exception("You can't generate path in component that has less then 3 children... sorry :(");
            int n = Math.Min(25, rnd.Next(c.controls.Count/25 , c.controls.Count));
            List<Point> Points = new List<Point>(n+1);
            int counter = 0;
            while (n!=0 && counter < c.controls.Count * 3 && Points.Count < 10)
            {
                counter++;
                int index = rnd.Next(0, c.controls.Count);
                if (c.controls[c.keys[index]].GetType() == typeof(Room))
                {
                    Point candidat = new Point(c.controls[c.keys[index]].x, c.controls[c.keys[index]].y);
                    if (!Points.Contains(candidat) && !CanFindTheSameX(Points, candidat))
                    {
                        n--;
                        Points.Add(candidat);
                        ConnectedComponent.Add(c.controls[c.keys[index]]);
                    }
                }
            }
            generatePathThrueLocations(Points);
        }
        protected bool CanFindTheSameX(List<Point> Points, Point point)
        {
            foreach(Point p in Points)
            {
                if (Math.Abs(p.x - point.x) == 0 || Math.Abs((p.y - point.y) / (p.x - point.x)) > 2)
                    return true;
            }
            return false;
        }

    }
    
}
