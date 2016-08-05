using MultyNetHack.MyMath;
using MultyNetHack.MyEnums;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Impassable wall
    /// </summary>
    public class HorizontalWall : Component
    {
        /// <summary>
        /// Deprecated!!!
        /// </summary>
        public HorizontalWall(string name, Point centerLoc, int sizeLeft, int sizeRight) : base(name)
        {
            IsPassable = false;
            LocalX = centerLoc.x;
            LocalY = centerLoc.y;
            MadeOf = Material.HorisontalWall;
            
        }
        /// <summary>
        /// Deprecated!!!
        /// </summary>
        public HorizontalWall(string name, Point centerLoc, int size) : base(name)
        { 
            IsPassable = false;
            LocalX = centerLoc.x;
            LocalY = centerLoc.y;
            MadeOf = Material.HorisontalWall;
            
        }
        public HorizontalWall(string Name, Rectangle Bounds) : base(Name)
        {
            mBounds = Bounds;
            IsPassable = false;
            MadeOf = Material.HorisontalWall;
        }
    }
    /// <summary>
    /// Impassable wall
    /// </summary>
    public class VerticalWall : Component
    {
        /// <summary>
        /// Deprecated!!!
        /// </summary>
        public VerticalWall(string name, Point location, int sizeUp, int sizeDown) : base(name)
        {
            IsPassable = false;
            LocalX = location.x;
            LocalY = location.y;
            
            MadeOf = Material.VerticalWall;
        }
        /// <summary>
        /// Deprecated!!!
        /// </summary>
        public VerticalWall(string name, Point location, int size) : base(name)
        {
            IsPassable = false;
            LocalX = location.x;
            LocalY = location.y;
            
            MadeOf = Material.VerticalWall;
        }
        public VerticalWall(string Name, Rectangle Bounds) : base(Name)
        {
            mBounds = Bounds;
            IsPassable = false;
            MadeOf = Material.HorisontalWall;
        }
    }
    public class Wall : Component
    {
        public Wall (string Name, Rectangle Bounds) : base(Name)
        {
            if (Bounds.Width > Bounds.height)
                MadeOf = Material.HorisontalWall;
            else
                MadeOf = Material.VerticalWall;
            mBounds = Bounds;
            IsPassable = false;
        }
    }
}
