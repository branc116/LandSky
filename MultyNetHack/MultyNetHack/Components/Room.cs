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
            Width = rand.Next(15, 40);
            Height = rand.Next(7, 20);
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

            LocalX = rand.Next(left, right);
            LocalY = rand.Next(bottom, top);
            Width = rand.Next(15, 40);
            Height = rand.Next(7, 20);

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

            HorizontalWall wT = new HorizontalWall("TopWall" + this.Name, new Point(0, Bounds.t - LocalY - 1), Width);
            HorizontalWall wB = new HorizontalWall("BottomWall" + this.Name, new Point(0, Bounds.b - LocalY), Width);
            VerticalWall wL = new VerticalWall("LeftWall" + this.Name, new Point(Bounds.l - LocalX + 1, 0), Height - 2);
            VerticalWall wR = new VerticalWall("RightWall" + this.Name, new Point(Bounds.r - LocalX, 0), Height - 2);
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
