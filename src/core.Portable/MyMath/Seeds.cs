using System.Linq;
using static System.Math;

namespace LandSky.MyMath
{
    internal class Seeds
    {
        private const int STICK_FACTOR = 2;
        private const int DEMINISH_FACTOR = 10;

        public string Seed { get; private set; }
        private Polinom3D Poly = new Polinom3D();

        public Seeds(string Seed)
        {
            this.Seed = Seed;
            InitPolinom(Seed);
        }

        public bool IsOver(int x, int y) => Poly.IsBiggerThanZero((x / STICK_FACTOR) * STICK_FACTOR, (y / STICK_FACTOR) * STICK_FACTOR);

        private void InitPolinom(string seed)
        {
            foreach (var c in seed.AsParallel())
            {
                int num = c.GetHashCode() % 100000;

                double ParamaterA = num * ((num % 2) == 0 ? -1 : 1) * Sin(Sqrt(num) * (((num) % 2) == 0 ? -1 : 1)) / DEMINISH_FACTOR;
                double ParamaterB = ((int)Sqrt(num) * ((num % 2) == 1 ? -1 : 1) * Sin(Sqrt(num) * (((num) % 2) == 1 ? -1 : 1))) / DEMINISH_FACTOR;
                Poly.Monoms.Add(new Monom3D(ParamaterA, ParamaterB));
            }
        }
    }
}