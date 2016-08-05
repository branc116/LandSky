using System;
using System.Collections.Generic;

using MultyNetHack.MyEnums;
using MultyNetHack.MyMath;

namespace MultyNetHack.Components
{
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
            IsPassable = true;
            sweep = new List<Sweep>();
            Controls = new Dictionary<string, Component>();
        }
        public void GeneratRandom()
        {
            START:;
            genStack++;

            LocalX = rand.Next(-500, 500);
            LocalY = rand.Next(-500, 500);
            MadeOf = Material.Air;
            ZValue = Parent.ZValue + 2;
            if (CollisionCheck(this) || Width * Height < 40)
                if (genStack > 1000)
                {
                    this.Dispose();
                }
                else
                    goto START;
            else
            {
                Parent.InsertInSweep(this);
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

            int l = rand.Next(left, right);
            int t = rand.Next(bottom, top);
            int b = rand.Next(t - 40, t - 15);
            int r = rand.Next(l + 7, l + 20);
            mBounds = new Rectangle(t, r, b, l);
            MadeOf = Material.Air;
        }
        public void GenerateRandom(Quadrant quadrant, int bound)
        {
            if (quadrant == Quadrant.First)
                GenerateRandom(bound, 0, 0, bound);
            else if (quadrant == Quadrant.Second)
                GenerateRandom(bound, -bound, 0, 0);
            else if (quadrant == Quadrant.Third)
                GenerateRandom(0, -bound, -bound, 0);
            else if (quadrant == Quadrant.Fourth)
                GenerateRandom(0, 0, -bound, bound);
        }
        public bool CollisionCheck(Room r)
        {
            Point startEnd;
            startEnd = Parent.GetStartEndEnter(r.LocalX);
            // make it faster!!!
            for (int i = 0; i < Parent.sweep.Count; i++)
            {
                if (Parent.sweep[i].enter == startEnd.enter && Parent.sweep[i].component & r)
                    return true;
            }
            return false;
        }
        public void GenerateWall()
        {

            //HorizontalWall wT = new HorizontalWall("TopWall" + this.Name, new Point(0, LocalBounds.height/2 - 1), LocalBounds.Width);
            //HorizontalWall wB = new HorizontalWall("BottomWall" + this.Name, new Point(0, -LocalBounds.height/2), LocalBounds.Width);
            //VerticalWall wL = new VerticalWall("LeftWall" + this.Name, new Point(LocalBounds.Width/2 + 1, 0), LocalBounds.height - 2);
            //VerticalWall wR = new VerticalWall("RightWall" + this.Name, new Point(-LocalBounds.Width / 2, 0), LocalBounds.height - 2);
            Wall wT = new Wall("TopWall", new Rectangle(LocalBounds.TopBound, LocalBounds.RightBound, LocalBounds.TopBound - 1, LocalBounds.LeftBound));
            Wall wB = new Wall("BottomWall", new Rectangle(LocalBounds.BottomBound + 1, LocalBounds.RightBound, LocalBounds.BottomBound, LocalBounds.LeftBound));
            Wall wL = new Wall("LeftWall",new Rectangle(LocalBounds.TopBound - 1, LocalBounds.LeftBound + 1, LocalBounds.BottomBound + 1, LocalBounds.LeftBound));
            Wall wR = new Wall("RightWall", new Rectangle(LocalBounds.TopBound - 1, LocalBounds.RightBound, LocalBounds.BottomBound + 1, LocalBounds.RightBound - 1));

            wT.ZValue = ZValue + 1;
            wB.ZValue = ZValue + 1;
            wL.ZValue = ZValue + 1;
            wR.ZValue = ZValue + 1;
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
}
