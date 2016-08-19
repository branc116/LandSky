using LandSky.MyEnums;
using LandSky.MyMath;

namespace LandSky.Components
{
    public class Wall : Component
    {
        public Wall(string Name, Rectangle Bounds) : base(Name)
        {
            MadeOf = Bounds.Width > Bounds.Height ? Material.HorisontalWall : Material.VerticalWall;
            base.Bounds = Bounds;
            IsPassable = false;
        }
    }
}