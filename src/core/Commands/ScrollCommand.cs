namespace LandSky.Commands
{
    /// <summary>
    /// Command for scrolling body part of the screen
    /// </summary>
    class ScrollCommand : BaseCommand
    {
        public int N;
        public ScrollCommand(int N)
        {
            this.N = N;
        }
        public static ScrollCommand Left()
        {
            return new ScrollCommand(-1);
        }
        public static ScrollCommand Right()
        {
            return new ScrollCommand(1);
        }
        public static ScrollCommand LeftTen()
        {
            return new ScrollCommand(-10);
        }
        public static ScrollCommand RightTen()
        {
            return new ScrollCommand(10);
        }
    }
}
