using System;

namespace LandSky.Components
{
    public class Cell : IComparable
    {
        public DateTime LastUsed;

        public int Priority { get; set; } = 0;

        public char Value
        {
            get
            {
                LastUsed = DateTime.Now;
                return _value;
            }
            set
            {
                LastUsed = DateTime.Now;
                _value = value;
            }
        }

        private char _value;

        public Cell(char Value)
        {
            this.Value = Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is Cell)
                return (LastUsed).CompareTo((obj as Cell).LastUsed);
            throw new ArgumentException("obj needs to be Cell", "obj");
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static Cell operator +(Cell a, Cell b) => b.Priority > a.Priority ? b : a;
    }
}