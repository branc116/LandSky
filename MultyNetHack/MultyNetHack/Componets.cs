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
        public int top, left;
        public int l, r, t, b;
        public Dictionary<string, Component> controls;
        public List<string> keys;
        public int height, width, depht;
        public Component parent;
        public Material madeOf;
        public string name;

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


        //check for intersection in two rooms
        public static bool operator &(Component one, Component two)
        {
            return (one.l <= two.r && one.r >= two.l &&
                one.t <= two.b && one.b >= two.t);
        }
        public static bool operator &(Component one, MyPoint two)
        {
            return (one.l <= two.rightBound && one.r >= two.leftBound &&
                one.t >= two.bottomBound && one.b <= two.topBound);
        }
    }
    class Room : Component
    {
        private int genStack;
        public Room(string name)
        {
            this.name = name;
            this.keys = new List<string>();
            this.controls = new Dictionary<string, Component>();
            genStack = 0;
        }        
        public void generatRandom()
        {
            genStack++;
            if (genStack > 50)
                this.Dispose();
            Random rand = new Random(DateTime.Now.Second);
            if (parent.width < 30 || parent.height < 30) throw new Exception("Can't make room in Component this small (min is 30*30)");
            top = rand.Next(-500, 500);
            left = rand.Next(-500, 500);
            width = rand.Next(5, 16);
            height = rand.Next(5, 16);
            l = left;
            t = top;
            r = l + width;
            b = t - height;
            madeOf = Material.Air;
            if (CollisionCheck(this) || width*height<10)
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
        //check for intersection in two rooms
        public static bool operator &(Room one, Room two)
        {
            return (one.l <= two.r && one.r >= two.l &&
                one.t <= two.b && one.b >= two.t);
        } 
        
        public static bool operator ==(Room one, Room two)
        {
            if (one.t == two.t &&
                one.b == two.b &&
                one.l == two.l &&
                one.r == two.r &&
                one.name == two.name) return true;
            return false;
        }
        public static bool operator ==(Room one, Component two)
        {
            if (one.t == two.top &&
                one.b == two.top + two.height &&
                one.l == two.left &&
                one.r == two.left + two.width &&
                one.name == two.name) return true;
            return false;
        }
        public static bool operator !=(Room one, Component two)
        {
            return !(one == two);
        }
        public static bool operator !=(Room one, Room two)
        {
            return !(one == two);
        }
    }
}
