using System;
using System.Collections.Generic;
using System.Linq;

using MultyNetHack.MyEnums;
using MultyNetHack.MyMath;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Passable area
    /// </summary>
    public class Room : Component
    {

        public Room(string Name) : base(Name)
        {
            Rand = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 7187 + DateTime.Now.Minute * 8167);
            IsPassable = true;
            Controls = new Dictionary<string, Component>();
        }
        public void GenerateRandom(int Top, int Left, int Bottom, int Right)
        {
            if (Top < Bottom)
            {
                int Temp = Top;
                Top = Bottom;
                Bottom = Temp;
            }
            if (Right < Left)
            {
                int Temp = Left; Left = Right; Right = Temp;
            }

            int L = Rand.Next(Left, Right);
            int T = Rand.Next(Bottom, Top);
            int B = Rand.Next(T - 40, T - 15);
            int R = Rand.Next(L + 7, L + 20);
            Bounds = new Rectangle(T, R, B, L);
            MadeOf = Material.Air;
        }
        public void GenerateRandom(Quadrant Quadrant, int Bound)
        {
            switch (Quadrant)
            {
                case Quadrant.First:
                    GenerateRandom(Bound, 0, 0, Bound);
                    break;
                case Quadrant.Second:
                    GenerateRandom(Bound, -Bound, 0, 0);
                    break;
                case Quadrant.Third:
                    GenerateRandom(0, -Bound, -Bound, 0);
                    break;
                case Quadrant.Fourth:
                    GenerateRandom(0, 0, -Bound, Bound);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Quadrant), Quadrant, null);
            }
        }
        public bool CollisionCheck(Room R)
        {
            return this.Parent.Controls.Any(N => N.Value.GetType() != typeof(Path) && N.Value.LocalBounds & R.LocalBounds);
        }
        public void GenerateWall()
        {
            Wall WT = new Wall("TopWall", new Rectangle(new Point(0, this.Height/2 - 1), this.Width, 1));
            Wall WB = new Wall("BottomWall", new Rectangle(new Point(0, -this.Height/2 + 1), this.Width, 1));
            Wall WL = new Wall("LeftWall", new Rectangle(new Point(-this.Width/2 - 1, 0), 1, this.Height));
            Wall WR = new Wall("RightWall", new Rectangle(new Point(this.Width/2, 0), 1, this.Height));

            WT.ZValue = ZValue + 1;
            WB.ZValue = ZValue + 1;
            WL.ZValue = ZValue + 1;
            WR.ZValue = ZValue + 1;
            this.Insert(WT);
            this.Insert(WB);
            this.Insert(WL);
            this.Insert(WR);
        }
    }
}
