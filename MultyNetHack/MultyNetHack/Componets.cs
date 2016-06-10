using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace MultyNetHack
{
    public enum Material
    {
        Path,
        Wall,
        Trap,
        Player,
        Npc,
        Loot,
        Watter,
        Fire,  
        Air,
        Darknes
    }

    public abstract class Component {
        public int x, y;
        public int l, r, t, b;
        public Dictionary<string, Component> controls;
        public List<string> keys;
        public int height, width, depht;
        public Component parent;
        public Material madeOf;
        public string name;

        public Component(string name)
        {
            controls = new Dictionary<string, Component>();
            keys = new List<string>();
            this.name = name;

        }

        public void Dispose()
        {
            this.parent.keys.Remove(this.name);
            this.parent.controls.Remove(this.name);
            controls = null;
            parent = null;

        }
        public void Insert(Component c)
        {
            if (c.name == null) throw new Exception("Component must have a name");
            keys.Add(c.name);
            controls.Add(c.name, c);
            if (c.parent != null)
            {
                c.parent.keys.Remove(c.name);
                c.parent.controls.Remove(c.name);
            }
            c.parent = this;
        }
        public void Delete(string name)
        {
            Component c = controls[name];
            foreach (string s in c.keys)
            {
                c.controls[s].Delete(s);
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
            foreach (string key in this.keys)
            {
                if (this.controls[key] & point)
                    return GetComponentOnLocation(new Point(this.controls[key].x, this.controls[key].x) - point);
            }
            return this;
            
        }

        //check for intersection in two rooms
        public static bool operator &(Component one, Component two)
        {
            return (one.l <= two.r && one.r >= two.l &&
                one.t <= two.b && one.b >= two.t);
        }
        public static bool operator &(Component one, Player two)
        {
            return (one.l <= two.rightBound && one.r >= two.leftBound &&
                one.t >= two.bottomBound && one.b <= two.topBound);
        }
        public static bool operator &(Component one, Point two)
        {
            return (one.t>two.x &&one.b<two.x && one.l <two.y && one.r>two.y);
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
                one.r == two.r + two.width &&
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
        
    }
    public class Room : Component
    {
        private int genStack;
        public Room(string name) : base(name)
        {
            genStack = 0;
        }
        public void generatRandom()
        {
            genStack++;
            
            Random rand = new Random(DateTime.Now.Second);
            if (parent.width < 30 || parent.height < 30) throw new Exception("Can't make room in Component this small (min is 30*30)");
            x = rand.Next(-500, 500);
            y = rand.Next(-500, 500);
            width = rand.Next(5, 30);
            height = rand.Next(5, 30);
            l = x - width  /2 - width%2;
            t = y + height /2 + height%2;
            r = x + width  /2;
            b = y - height /2;

            madeOf = Material.Air;
            if (CollisionCheck(this) || width * height < 40)
                if (genStack > 50)
                {
                    this.Dispose();
                }
                else
                    this.generatRandom();
            
        }
        public bool CollisionCheck(Room r)
        {
            foreach(string key in r.parent.keys)
            {
                if (r == r.parent.controls[key])
                    continue;
                if (r & (Room)r.parent.controls[key])
                    return true;
            }
            return false;
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
    public class Root : Component
    {
        public Root() : base("Root")
        {
            madeOf = Material.Fire;
            width = int.MaxValue;
            height = int.MaxValue;
        }
    }
    
}
