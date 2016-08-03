using MultyNetHack.MyEnums;

namespace MultyNetHack.Components
{
    /// <summary>
    /// Player with the location and texture
    /// </summary>
    public class Player : Component
    {
        public void SizeOfScreen(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        public Player(int x, int y) : base("Player")
        {
            this.LocalX = x;
            this.LocalY = y;

            Width = 1;
            Height = 1;
            MadeOf = Material.Player;
        }

        public static Player operator +(Player a, Player b)
        {
            return new Player(a.LocalX + b.LocalX, a.LocalY + b.LocalY);
        }
        public static bool operator ==(Player a, Player b)
        {
            return a.LocalX == b.LocalX && a.LocalY == b.LocalY;
        }
        public static bool operator !=(Player a, Player b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
