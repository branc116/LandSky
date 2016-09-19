using LandSky.MyEnums;
using LandSky.MyMath;
using System.Collections.Generic;
using static LandSky.AsciiTexture;

namespace LandSky.Components
{
    public class InfinitePlane : Component
    {
        private Seeds _seed;
        public Dictionary<Point, Cell> _table = new Dictionary<Point, Cell>();
        public Point CurentLocation { get; set; } = new Point(0, 0);

        public InfinitePlane(string Seed, string Name) : base(Name)
        {
            _seed = new Seeds(Seed);
            IsInfinity = true;
        }

        public Cell GetPoint(Point Location)
        {
            if (_table.ContainsKey(Location))
                return _table[Location];
            var resoult = new Cell(_seed.IsOver(Location.X, Location.Y) ? AsciiTextures[Material.Air] : AsciiTextures[Material.Darknes]);
            resoult.Priority = ZValue;
            _table.Add(Location, resoult);
            return resoult;
        }

        public override Cell[][] GetRegin(Rectangle Rec)
        {
            Cell[][] Area = new Cell[Rec.Height][];
            for (int i = 0; i < Rec.Height; i++)
            {
                Area[i] = new Cell[Rec.Width];
                for (int j = 0; j < Rec.Width; j++)
                {
                    Area[i][j] = GetPoint(new Point(j + Rec.LeftBound, Rec.TopBound - i));
                }
            }
            return Area;
        }
    }

    //class Slice : IList<List<bool>>
    //{
    //    private Point Offset = new Point(0,0);

    //    private Rectangle CurrentSlice { get; set; } = new Rectangle(new Point(0, 0), 100, 100);

    //    private List<List<bool>> _currentSlice;

    //    public Size SliceSize { get; set; } = new Size(100, 100);

    //    public Size ChunkSize { get; set; } = new Size(20, 20);

    //    public string Seed { get; set; } = "Hello World Seed";

    //    public EventHandler<EventArgs> ResetMe;

    //    public List<bool> this[int index]
    //    {
    //        get
    //        {
    //            return _currentSlice[index];
    //        }

    //        set
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public bool this[int X,int Y]
    //    {
    //        get
    //        {
    //            if (_currentSlice?[X - CurrentSlice.LeftBound]?[CurrentSlice.TopBound - Y] == null)
    //                ResetMe?.Invoke(this, EventArgs.Empty);
    //            return _currentSlice?[X - CurrentSlice.LeftBound]?[CurrentSlice.TopBound - Y] ?? false;
    //        }
    //        set
    //        {
    //            if (_currentSlice?[X - CurrentSlice.LeftBound]?[CurrentSlice.TopBound - Y] == null)
    //                ResetMe?.Invoke(this, EventArgs.Empty);
    //            else
    //                _currentSlice[X - CurrentSlice.LeftBound][CurrentSlice.TopBound - Y] = value;
    //        }
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            return _currentSlice.Count;
    //        }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    public void Add(List<bool> item)
    //    {
    //        _currentSlice.Add(item);
    //    }

    //    public void Clear()
    //    {
    //        _currentSlice.Clear();
    //    }

    //    public bool Contains(List<bool> item)
    //    {
    //        return _currentSlice.Contains(item);
    //    }

    //    public void CopyTo(List<bool>[] array, int arrayIndex)
    //    {
    //        _currentSlice.CopyTo(array, arrayIndex);
    //    }

    //    public IEnumerator<List<bool>> GetEnumerator()
    //    {
    //        foreach(var item in _currentSlice)
    //        {
    //            yield return item;
    //        }
    //    }

    //    public int IndexOf(List<bool> item)
    //    {
    //        return _currentSlice.IndexOf(item);
    //    }

    //    public void Insert(int index, List<bool> item)
    //    {
    //        _currentSlice.Insert(index, item);
    //    }

    //    public bool Remove(List<bool> item)
    //    {
    //        return _currentSlice.Remove(item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        _currentSlice.RemoveAt(index);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return _currentSlice.GetEnumerator();
    //    }
    //}
}