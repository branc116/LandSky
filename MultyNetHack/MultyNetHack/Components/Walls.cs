using MultyNetHack.MyMath;
using MultyNetHack.MyEnums;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Impassable wall
    /// </summary>
    public class HorizontalWall : Component
    {
        public HorizontalWall(string name, Point centerLoc, int sizeLeft, int sizeRight) : base(name)
        {
            IsPassable = false;
            LocalX = centerLoc.x;
            LocalY = centerLoc.y;
            MadeOf = Material.HorisontalWall;
            Height = 1;
            Width = sizeLeft + sizeRight + 1;
        }
        public HorizontalWall(string name, Point centerLoc, int size) : base(name)
        {
            IsPassable = false;
            LocalX = centerLoc.x;
            LocalY = centerLoc.y;
            MadeOf = Material.HorisontalWall;
            Height = 1;
            Width = size;
        }
    }
    /// <summary>
    /// Impassable wall
    /// </summary>
    public class VerticalWall : Component
    {
        public VerticalWall(string name, Point location, int sizeUp, int sizeDown) : base(name)
        {
            IsPassable = false;
            LocalX = location.x;
            LocalY = location.y;
            Width = 1;
            Height = sizeUp + sizeDown;
            MadeOf = Material.VerticalWall;
        }
        public VerticalWall(string name, Point location, int size) : base(name)
        {
            IsPassable = false;
            LocalX = location.x;
            LocalY = location.y;
            Width = 1;
            Height = size;
            MadeOf = Material.VerticalWall;
        }
    }
}
