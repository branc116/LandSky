using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

using LandSky.MyEnums;

namespace LandSky.MyMath
{
    /// <summary>
    /// Used for interpolating polinonm that in on a list of points
    /// </summary>
    public class LinearInterpolator
    {
        private List<Monom> mPolinom;

        public LinearInterpolator()
        {
            mPolinom = new List<Monom>();

        }
        public double ValueForX(double X)
        {
            double Rj = 0;
            foreach (Monom m in mPolinom)
            {
                Rj += m.ValuForX(X);
            }
            return Round(Rj, 2);
        }
        public double ValueForX(int X)
        {
            return ValueForX(Convert.ToDouble(X));
        }
        public double DerivativeForX(double X)
        {
            double Rj = 0;
            foreach (Monom m in mPolinom)
            {
                Rj += m.DerivativeForX(X);
            }
            return Round(Rj, 0);
        }
        public double DerivativeForX(int X)
        {
            return DerivativeForX(Convert.ToDouble(X));
        }
        private void CheckIfOneCanInterpolate(List<Point> Points)
        {
            foreach (Point P in Points)
            {
                int N = 0;
                for (int I = 0; I < Points.Count; I++)
                {
                    if (Points[I].X == P.X)
                        N++;
                }
                if (N != 1)
                    throw new Exception("Can't interpolate if you have two or more points with the same x, sorry... remove duplicates or use non correct interpolation");
            }

        }
        public void Interpolate(List<Point> Points, KindOfMonom Monoms)
        {
            Interpolate(Points, Monoms, false);
        }
        public void Interpolate(List<Point> Points, KindOfMonom Monoms, bool MustBeCorrect)
        {
            if (MustBeCorrect)
                CheckIfOneCanInterpolate(Points);
            if (Monoms == KindOfMonom.Constant)
                throw new Exception("One can't interpolate array of points only with constants, one can but it wont be good... don't select constants, pls, select line it's good, or sine");
            List<KindOfMonom> Monomz = new List<KindOfMonom>(Points.Count);
            Monomz.Add(KindOfMonom.Constant);
            for (int I = 1; I < Points.Count; I++)
            {
                Monomz.Add(Monoms);
            }
            Interpolate(Points, Monomz);
        }
        public void Interpolate(List<Point> Points, List<KindOfMonom> Monoms)
        {
            List<List<double>> Matrix = new List<List<double>>(Points.Count);
            mPolinom = new List<Monom>();
            int LastLine = -1, LastSine = 0;
            for (int I = 0; I < Points.Count; I++)
            {
                Matrix.Add(new List<double>());
                for (int J = 0; J < Points.Count + 2; J++)
                {
                    Matrix[I].Add(0);
                }
            }
            for (int I = 0; I < Points.Count; I++)
            {

                switch (Monoms[I])
                {
                    case KindOfMonom.Constant:
                        mPolinom.Add(new Monom(1));
                        for (int J = 0; J < Points.Count; J++)
                        {
                            Matrix[J][I] = 1;
                        }
                        break;
                    case KindOfMonom.Line:
                        LastLine++;
                        mPolinom.Add(new Monom(KindOfMonom.Line, 1, LastLine));
                        for (int J = 1; J < Points.Count; J++)
                        {
                            Point P = Points[J];
                            Matrix[J][I] = Pow(P.X, LastLine);
                        }
                        break;
                    case KindOfMonom.Sine:
                        LastSine++;
                        mPolinom.Add(new Monom(KindOfMonom.Sine, 1, LastSine));
                        for (int J = 1; J < Points.Count; J++)
                        {
                            Point P = Points[J];
                            Matrix[J][I] = Sin(LastSine * P.X);
                        }
                        break;
                }

                Matrix[I][Points.Count] = (Points[I].Y);
                //start x
                Matrix[I][Points.Count + 1] = I;
            }
            List<double> Aas = SolveMatrix(Matrix);
            for (int I = 0; I < Aas.Count; I++)
            {
                mPolinom[I].InterpolatedValue = Aas[I];
            }
        }
        public void Interpolate(List<Point> Points, List<KindOfMonom> Monoms, bool MustBeCorrect)
        {
            if (MustBeCorrect)
                CheckIfOneCanInterpolate(Points);
            Interpolate(Points, Monoms);
        }
        public override string ToString()
        {
            return mPolinom.Aggregate(string.Empty, (Current, m) => Current + (" " + m.LinearRepresentationOfMonom() + " "));
        }

        public List<double> SolveMatrix(List<List<double>> Matrix)
        {
            if (Matrix[0].Count - 2 != Matrix.Count)
                throw new Exception("One can only interpolate Matrix of size n*n+2\n a*X=BottomBound a:= n*n matrix, BottomBound:= 1*n combination, and 1*n matrix where are saved original indexes of rows");
            #region down
            int N = Matrix.Count;
            if (Matrix[0].Count - 1 == Matrix.Count)
            {
                for (int I = 0; I < Matrix.Count; I++)
                {
                    Matrix[I].Add(I);
                }
            }
            double Miny = 0.0001;
            for (int I = 0; I < N; I++)
            {
                if (Abs(Matrix[I][I]) - Miny <= 0)
                {
                    bool Found = false;
                    for (int J = N - 1; J > I; J--)
                    {
                        if (Abs(Matrix[J][I]) - Miny > 0)
                        {
                            List<double> Temp = Matrix[I];
                            Matrix[I] = Matrix[J];
                            Matrix[J] = Temp;
                            Found = true;
                            J = I;
                        }
                    }
                    if (!Found)
                        throw new Exception("Sorry can't interpolate :(");
                }

                for (int J = I + 1; J < N; J++)
                {
                    double Koeficjent = Matrix[J][I] / Matrix[I][I];

                    for (int K = 0; K < N + 1; K++)
                    {
                        Matrix[J][K] = Matrix[J][K] - (Koeficjent * Matrix[I][K]);
                    }
                }
            }

            #endregion

            #region up
            for (int I = (N - 1); I >= 0; I--)
            {
                for (int J = I - 1; J >= 0; J--)
                {
                    double Koeficjent = new double();
                    Koeficjent = Matrix[J][I] / Matrix[I][I];
                    Matrix[J][I] = 0;
                    Matrix[J][N] = Matrix[J][N] - Koeficjent * Matrix[I][N];
                }
            }
            #endregion
            #region solve
            List<double> Soluton = new List<double>(N);
            for (int I = 0; I < N; I++)
            {
                Soluton.Add(1);
            }
            for (int I = 0; I < N; I++)
            {
                Soluton[Convert.ToInt32(Matrix[I][N + 1])] = Matrix[I][N] / Matrix[I][I];
            }
            #endregion
            return Soluton;
        }
    }
}
