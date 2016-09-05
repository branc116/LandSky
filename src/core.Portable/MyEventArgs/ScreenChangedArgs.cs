using System;

namespace LandSky.MyEventArgs
{
    public class ScreenChangedArgs : EventArgs
    {
        public string StateOfConsole;
        public string BodyString;
        public DateTime DateOfCreation;

        public ScreenChangedArgs(string StateOfConsole, string BodyString) : base()
        {
            this.StateOfConsole = StateOfConsole;
            this.BodyString = BodyString;
            DateOfCreation = DateTime.Now;
        }
    }
}