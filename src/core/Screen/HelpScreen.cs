namespace LandSky.Screen
{
    /// <summary>
    /// Help screen (need to be implemented)
    /// </summary>
    public class HelpScreen :BaseScreen    
    {
        public HelpScreen(int Top, int Left) : base(Top, Left, "Help")
        {
            InitText();
            ScreenChange();
        }
        private void InitText()
        {
            PrintCenter("This is Land sky");
            PrintLine();
            PrintCenter("Rouge like multiplayer game");
            PrintLine();
            PrintCenter("Use h/j/k/l to move left/down/up/right");
            PrintCenter("Press ESC to get to the last screen");
            PrintCenter("Press 1-9 to chose option 1-9");
            PrintCenter("This is still in early stages so there isn't a lot of gameplay");
        }
    }
} 
