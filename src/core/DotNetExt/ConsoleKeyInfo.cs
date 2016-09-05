namespace LandSky.DotNetExt
{
    /// <summary>
    /// My implementation of ConsoleKeyInfo. Made because default one sucks...
    /// This one has nice constructors and no shitty ConsoleKey property!
    /// </summary>
    internal class MyConsoleKeyInfo : IMyConsoleKeyInfo
    {
        public char PressedKey { get; set; }
        public bool Alt { get; set; } = false;
        public bool Ctrl { get; set; } = false;
        public bool Shift { get; set; } = false;

        public MyConsoleKeyInfo(char PressedKey)
        {
            this.PressedKey = PressedKey;
        }

        public MyConsoleKeyInfo(char PressedKey, bool Alt, bool Ctrl)
        {
            this.PressedKey = PressedKey;
            this.Alt = Alt;
            this.Ctrl = Ctrl;
            this.Shift = Shift;
        }

        public MyConsoleKeyInfo(char PressedKey, bool Alt, bool Ctrl, bool Shift)
        {
            this.PressedKey = PressedKey;
            this.Alt = Alt;
            this.Ctrl = Ctrl;
            this.Shift = Shift;
        }

        public static bool operator ==(MyConsoleKeyInfo One, MyConsoleKeyInfo Two) => One.PressedKey == Two.PressedKey &&
                                                                                     One.Alt == Two.Alt &&
                                                                                     One.Ctrl == Two.Ctrl &&
                                                                                     One.Shift == Two.Shift;

        public static bool operator !=(MyConsoleKeyInfo One, MyConsoleKeyInfo Two) => !(One.PressedKey == Two.PressedKey &&
                                                                                        One.Alt == Two.Alt &&
                                                                                        One.Ctrl == Two.Ctrl &&
                                                                                        One.Shift == Two.Shift);
    }
}