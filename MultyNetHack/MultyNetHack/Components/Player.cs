using MultyNetHack.MyEnums;
using MultyNetHack.MyMath;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Player with the location and texture
    /// </summary>
    public class Player : Component
    {
        public Player(string Name) : base($"Player{Name}")
        {
            this.ZValue = 25;
            this.Bounds = Rectangle.DefineRectangleByWidthAndHeight(0, 0, 0, 0);

            MadeOf = Material.Player;
        }
    }
}
