using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandSky.Components
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
            int B = Rand.Next(T - 20, T - 15);
            int R = Rand.Next(L + 20, L + 70);
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
            Wall WT = new Wall("TopWall", new Rectangle(Bounds.DistaceToTopBound, Bounds.DistaceToRightBound, Bounds.DistaceToTopBound, Bounds.DistaceToLeftBound));
            Wall WB = new Wall("BottomWall", new Rectangle(Bounds.DistaceToBottomBound, Bounds.DistaceToRightBound, Bounds.DistaceToBottomBound, Bounds.DistaceToLeftBound));
            Wall WL = new Wall("LeftWall", new Rectangle(Bounds.DistaceToTopBound, Bounds.DistaceToLeftBound, Bounds.DistaceToBottomBound, Bounds.DistaceToLeftBound));
            Wall WR = new Wall("RightWall", new Rectangle(Bounds.DistaceToTopBound, Bounds.DistaceToRightBound, Bounds.DistaceToBottomBound, Bounds.DistaceToRightBound));

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