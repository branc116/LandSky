using System.Collections.Generic;
using System.Linq;

namespace LandSky.MyMath
{
    internal class Polinom3D
    {
        public List<Monom3D> Monoms = new List<Monom3D>();

        public bool IsBiggerThanZero(int x, int y)
        {
            if (Monoms.Count < 100)
                return Monoms.Sum(Mon => Mon.ValueForX(x, y)) > -1;
            return Monoms.AsParallel().Sum(Mon => Mon.ValueForX(x, y)) > -1;
        }

        public double ValueForX(int x, int y)
        {
            if (Monoms.Count < 100)
                return Monoms.Sum(Mon => Mon.ValueForX(x, y));
            return Monoms.AsParallel().Sum(Mon => Mon.ValueForX(x, y));
        }
    }
}