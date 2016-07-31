using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Web.Script.Serialization;

namespace MultyNetHack
{
    
    /// <summary>
    /// Eveyithing shuld be extened from this
    /// </summary>
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
        public bool isPassable;
        private Random mRand;
        public Component(string name)
        {
            controls = new Dictionary<string, Component>();
            keys = new List<string>();
            sweep = new List<Sweep>();
            mRand = new Random();
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
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
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

        public async Task GenerateRandomPaths(int n, List<string> names)
        {
            List<Path> mRange = new List<Path>();
            Task[] mTasks = new Task[n];
            int i = 0;
            foreach(string name in names)
            {
                mTasks[i] = Task.Run(() =>
                {
                    Path mP = new Path(name);
                    mP.generatePathThrueRandomChildren(this);
                    mRange.Add(mP);
                });
                i++;
            }
            Task.WaitAll(mTasks);
            foreach (Task mTask in mTasks)
            {
                await mTask;
            }
            foreach (Path mPath in mRange)
            {
                this.Insert(mPath);
            }
            
        }
        public async Task GenerateRandomPaths(int n)
        {
            List<string> names = new List<string>();
            for(int i = 0; i < n; i++)
            {
                names.Add(string.Format("Room{0}-{1}", i, Guid.NewGuid().ToString().Substring(0, 7)));
            }
            await GenerateRandomPaths(n, names);
        }
        public async Task GenerateRandomRooms(int n, List<string> names)
        {
            Task[] mTasks = new Task[4];
            List<List<Component>> mQuad = new List<List<Component>>();
            Quadrant[] quads = new Quadrant[4] { Quadrant.First, Quadrant.Second, Quadrant.Third, Quadrant.Fourth };
            int i = 0;
            int pool = 0;
            int mN = n / 4;
            int made=0;
            foreach (Quadrant mQ in quads) {
                mTasks[i] = Task.Run(() => {
                    int id = pool++;
                    if (n % 4 > 0)
                    {
                        mN++;
                        n--;
                    }
                    List<Component> mRooms = new List<Component>();
                    for(int j = 0; j < mN; j++)
                    {
                        int breakint = 0;
                        Room mR = new Room(string.Format("{0}-{1}-{2}", names[j], j, quads[id]));
                        START:;
                        breakint++;
                        mR.GenerateRandom(quads[id], 500);
                        if (!mCheckCollision(mRooms, mR) && !mCheckCollision(this.controls, mR) && breakint<100)
                            goto START;
                        mR.GenerateWall();
                        made++;
                        mRooms.Add(mR);
                    }
                    mQuad.Add(mRooms);
                });
                i++;
            }
            Task.WaitAll(mTasks);
            i = 0;
            foreach(List<Component> mComps in mQuad)
            {
                await mTasks[0];
                foreach(Room mRoom in mComps)
                {
                    this.Insert(mRoom);
                    this.InsertInSweep(mRoom);
                }
            }
            
        }
        public async Task GenerateRandomRooms(int n)
        {
            List<string> mNames = new List<string>();
            for (int i = 0; i < n; i++)
            {
                mNames.Add(string.Format("Room-{0}", Guid.NewGuid().ToString().Substring(0, 7)));
            }
            await GenerateRandomRooms(n, mNames);
        }

        private bool mCheckCollision(List<Component> Components, Component NewComponent)
        {
            foreach(Component mComponent in Components)
            {
                if (mComponent.GetType() != typeof(Path) && mComponent & NewComponent)
                    return true;
            }
            return false;
        }
        private bool mCheckCollision(Dictionary<string, Component> Components, Component NewComponent)
        {
            bool returnValue = false;
            Components.ToList().ForEach((i) =>
            {
                if (i.Value.GetType() != typeof(Path) && i.Value & NewComponent)
                    returnValue = true;
            });
            return returnValue;
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
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// Passable area
    /// </summary>
    public class Room : Component
    {
        private int genStack;
        private Random rand;

        public Room(string name) : base(name)
        {
            genStack = 0;
            rand = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 7187 + DateTime.Now.Minute * 8167);
            isPassable = true;
            sweep = new List<Sweep>();
            controls = new Dictionary<string, Component>();
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
        public void GenerateRandom(int top, int left, int bottom, int right)
        {
            if (top < bottom)
            {
                int temp = top; top = bottom; bottom = temp;
            }
            if (right < left)
            {
                int temp = left; left = right; right = left;
            }

            x = rand.Next(left, right);
            y = rand.Next(bottom, top);
            width = rand.Next(15, 40);
            height = rand.Next(7, 20);
            l = x - width / 2;
            t = y + height / 2;
            r = x + width / 2;
            b = y - height / 2;
            width = r - l;
            height = t - b;
            madeOf = Material.Air;    
        }
        public void GenerateRandom(Quadrant quadrant, int bound)
        {
            if (quadrant == Quadrant.First)
                GenerateRandom(bound, bound, 0, 0);
            else if(quadrant == Quadrant.Second)
                GenerateRandom(bound, 0,0, -bound);
            else if (quadrant == Quadrant.Second)
                GenerateRandom(0, 0,-bound, -bound);
            else if (quadrant == Quadrant.Second)
                GenerateRandom(0, bound, -bound, 0);
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
    /// <summary>
    /// Player with the location and texture
    /// </summary>
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
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// Unpassable wall
    /// </summary>
    public class HorisontalWall : Component
    {
        public HorisontalWall(string name, Point centerLoc, int sizeLeft, int sizeRight) : base(name)
        {
            isPassable = false;
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
    /// <summary>
    /// Unpassable wall
    /// </summary>
    public class VerticalWall : Component
    {
        public VerticalWall(string name, Point location, int sizeUp, int sizeDown) : base(name)
        {
            isPassable = false;
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
    /// <summary>
    /// Don't use!!!
    /// </summary>
    public class Root : Component
    {
        public Root() : base("Root")
        {
            isPassable = false;
            numOfPaths = numOfRooms = 0;
            z = 0;
            madeOf = Material.Darknes;
            width = int.MaxValue;
            height = int.MaxValue;

        }

        public string GimeJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
    /// <summary>
    /// Passable object that connects diferent rooms
    /// </summary>
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
            isPassable = true;
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
