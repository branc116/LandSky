namespace LandSky.DotNetExt
{
    /// <summary>
    /// My implementation of ConsoleKeyInfo. Made because default one sucks...
    /// This one has nice constructors and no shitty ConsoleKey property!
    /// </summary>
    public class MyConsoleKeyInfo : IMyConsoleKeyInfo
    {
        /// <summary>
        /// Key in a form of a char
        /// </summary>
        public char KeyChar { get; set; }

        /// <summary>
        /// Is alt pressed
        /// </summary>
        public bool Alt { get; set; } = false;

        /// <summary>
        /// Is Ctrl pressed
        /// </summary>
        public bool Ctrl { get; set; } = false;

        /// <summary>
        /// Is shift pressed
        /// </summary>
        public bool Shift { get; set; } = false;

        public MyConsoleKeyInfo(char KeyChar)
        {
            this.KeyChar = KeyChar;
        }

        public MyConsoleKeyInfo(char KeyChar, bool Alt, bool Ctrl)
        {
            this.KeyChar = KeyChar;
            this.Alt = Alt;
            this.Ctrl = Ctrl;
            this.Shift = Shift;
        }

        public MyConsoleKeyInfo(char KeyChar, bool Alt, bool Ctrl, bool Shift)
        {
            this.KeyChar = KeyChar;
            this.Alt = Alt;
            this.Ctrl = Ctrl;
            this.Shift = Shift;
        }

        public static bool operator ==(MyConsoleKeyInfo One, MyConsoleKeyInfo Two) => One.KeyChar == Two.KeyChar &&
                                                                                     One.Alt == Two.Alt &&
                                                                                     One.Ctrl == Two.Ctrl &&
                                                                                     One.Shift == Two.Shift;

        public static bool operator !=(MyConsoleKeyInfo One, MyConsoleKeyInfo Two) => !(One.KeyChar == Two.KeyChar &&
                                                                                        One.Alt == Two.Alt &&
                                                                                        One.Ctrl == Two.Ctrl &&
                                                                                        One.Shift == Two.Shift);

        public override bool Equals(object obj) => (obj is MyConsoleKeyInfo) &&
                                                   (obj as MyConsoleKeyInfo) == this;

        public override int GetHashCode() => this.Alt.GetHashCode() &
                                             this.Shift.GetHashCode() &
                                             this.Ctrl.GetHashCode() &
                                             this.KeyChar.GetHashCode();
    }
}