using System;

namespace LandSky.Components
{
    public class Cell : IComparable
    {
        public DateTime LastUsed;

        public bool Value
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

        private bool _value;

        public Cell(bool Value)
        {
            this.Value = Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is Cell)
                return (LastUsed).CompareTo((obj as Cell).LastUsed);
            throw new ArgumentException("obj needs to be Cell", "obj");
        }
    }
}