using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MultyNetHack.MyEnums;
using MultyNetHack.MyMath;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Everything should be extended from this
    /// </summary>
    public abstract class Component
    {
        public int LocalX, LocalY, ZValue;
        public int GlobalX
        {
            get
            {
                if (IsRoot)
                {
                    return 0;
                }else
                {
                    return LocalX + Parent.LocalX;
                }
            }
        }
        public int GlobalY
        {
            get
            {
                if (IsRoot)
                {
                    return 0;
                }
                else
                {
                    return LocalY + Parent.LocalY;
                }
            }
        }
        public bool IsRoot;    
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(LocalY + Height / 2 + Height % 2, LocalX + Width / 2, LocalY - Height / 2, LocalX - Width / 2 - Width % 2);
            }
        }
        public Dictionary<string, Component> Controls;
        public List<string> Keys;
        public List<Sweep> sweep;
        public int Height, Width;
        public Component Parent;
        public Material MadeOf;
        public string Name;
        public int NumOfRooms
        {
            get
            {
                return Controls.Where(i => i.Value.GetType() == typeof(Room)).Count();
            }
        }
        public int NumOfWalls
        {
            get
            {
                return Controls.Where(i => i.Value.GetType() == typeof(HorizontalWall) || i.Value.GetType() == typeof(VerticalWall)).Count();
            }
        }
        public int NumOfPaths
        {
            get
            {
                return Controls.Where(i => i.Value.GetType() == typeof(Path)).Count();
            }
        }
        public bool IsPassable;
        private Random mRand;
        public Component(string name)
        {
            Controls = new Dictionary<string, Component>();
            Keys = new List<string>();
            sweep = new List<Sweep>();
            mRand = new Random();
            this.Name = name;
            IsRoot = false;
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
                return a.component.Name == b;
            }
            public static bool operator !=(Sweep a, string b)
            {
                return a.component.Name != b;
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
            this.Parent.Keys.Remove(this.Name);
            this.Parent.Controls.Remove(this.Name);
            sweep = null;
            Controls = null;
            Parent = null;

        }
        public void Insert(Component c)
        {
            if (c.Name == null) throw new Exception("Component must have a Name");
            this.Keys.Add(c.Name);
            this.Controls.Add(c.Name, c);
            if (c.Parent != null)
            {
                c.Parent.Keys.Remove(c.Name);
                c.Parent.Controls.Remove(c.Name);
            }
            c.Parent = this;
        }
        public void InsertInSweep(Component c)
        {

            int lb = 0, ub = sweep.Count;
            int x1 = c.Bounds.l;
            int x2 = c.Bounds.r;
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
            Component c = Controls[name];
            foreach (string s in c.Keys)
            {
                c.Controls[s].Delete(s);
            }
            foreach (Sweep s in sweep)
            {
                if (s == name)
                {
                    sweep.Remove(s);
                }
            }
            Keys.Remove(name);
            Controls.Remove(name);

        }
        public void Delete(int i)
        {
            Delete(Keys[i]);
        }
        public Component GetComponentOnLocation(int x, int y)
        {
            Point p = new Point(x, y);
            return GetComponentOnLocation(p);
        }
        public Component GetComponentOnLocation(Point point)
        {
            Point startEnd = GetStartEndEnter(point.x + 1);

            for (int i = startEnd.x; i < startEnd.y; i++)
            {
                if (sweep[i].enter == startEnd.enter)
                {
                    Component c = sweep[i].component;
                    if (c & point)
                        return c.GetComponentOnLocation(new Point(c.LocalX, c.LocalY) - point);
                }
            }
            Component solution = this;
            Controls.Where(i => i.Value.GetType() == typeof(Path)).ToList().ForEach((i) =>
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

            if (sweep.Count - lb < ub)
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
            Point startEnd = new Point(0, 0);
            if (sweep.Count == 0 || l <= sweep[0].x)
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
            int i = 0;
            while (ub + i < sweep.Count && sweep[ub + i] == sweep[ub])
            {
                i++;
            }
            ub += i;
            startEnd.y = Math.Min(sweep.Count - 1, ub);
            return startEnd;
        }

        public async Task GenerateRandomPaths(int n, List<string> names)
        {
            List<Path> mRange = new List<Path>();
            Task[] mTasks = new Task[n];
            int i = 0;
            foreach (string name in names)
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
            for (int i = 0; i < n; i++)
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
            int made = 0;
            foreach (Quadrant mQ in quads)
            {
                mTasks[i] = Task.Run(() => {
                    int id = pool++;
                    if (n % 4 > 0)
                    {
                        mN++;
                        n--;
                    }
                    List<Component> mRooms = new List<Component>();
                    for (int j = 0; j < mN; j++)
                    {
                        int breakint = 0;
                        Room mR = new Room(string.Format("{0}-{1}-{2}", names[j], j, quads[id]));
                        START:;
                        breakint++;
                        mR.GenerateRandom(quads[id], 500);
                        if ((mCheckCollision(mRooms, mR) || mCheckCollision(this.Controls, mR)) && breakint < 100)
                            goto START;
                        made++;
                        mRooms.Add(mR);
                    }
                    mQuad.Add(mRooms);
                });
                i++;
            }
            Task.WaitAll(mTasks);
            i = 0;
            foreach (List<Component> mComps in mQuad)
            {
                await mTasks[0];
                foreach (Room mRoom in mComps)
                {
                    if (mRoom.Bounds.width > 0 && mRoom.Bounds.height > 0)
                    {

                        mRoom.ZValue = this.ZValue + 3;
                        mRoom.GenerateWall();
                        this.Insert(mRoom);
                        this.InsertInSweep(mRoom);
                    }
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
            foreach (Component mComponent in Components)
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
            return (one.Bounds.l <= two.Bounds.r && one.Bounds.r >= two.Bounds.l &&
                one.Bounds.t >= two.Bounds.b && one.Bounds.b <= two.Bounds.t);
        }
        public static bool operator &(Component one, Player two)
        {
            return (one.Bounds.l <= two.Bounds.r && one.Bounds.r >= two.Bounds.l &&
                one.Bounds.t >= two.Bounds.b && one.Bounds.b <= two.Bounds.t);
        }
        public static bool operator &(Component one, Point two)
        {
            return (one.Bounds.t >= two.y && one.Bounds.b < two.y && one.Bounds.l <= two.x && one.Bounds.r >= two.x);
        }
        public static bool operator &(Component one, Rectangle two)
        {
            return (one.Bounds.l < two.r && one.Bounds.r > two.l &&
                    one.Bounds.t > two.b && one.Bounds.b < two.t);
        }
        public static bool operator ==(Component one, Component two)
        {
            if (one.Bounds.t == two.Bounds.t &&
                one.Bounds.b == two.Bounds.b &&
                one.Bounds.l == two.Bounds.l &&
                one.Bounds.r == two.Bounds.r &&
                one.Name == two.Name) return true;
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
            return new Rectangle(c.Bounds.t + p.y, c.Bounds.r + p.x, c.Bounds.b + p.y, c.Bounds.l + p.x);
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
}
