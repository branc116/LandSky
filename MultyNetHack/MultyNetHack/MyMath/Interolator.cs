using System;
using System.Collections.Generic;
using static System.Math;

using MultyNetHack.MyEnums;

namespace MultyNetHack.MyMath
{
    /// <summary>
    /// Used for interpolating polinonm that in on a list of points
    /// </summary>
    public class LinearInterpolator
    {
        List<Monom> Polinom;
        public LinearInterpolator()
        {
            Polinom = new List<Monom>();

        }
        public double ValueForX(double x)
        {
            double rj = 0;
            foreach (Monom m in Polinom)
            {
                rj += m.ValuForX(x);
            }
            return Round(rj, 2);
        }
        public double ValueForX(int x)
        {
            return ValueForX(Convert.ToDouble(x));
        }
        public double DerivativeForX(double x)
        {
            double rj = 0;
            foreach (Monom m in Polinom)
            {
                rj += m.DerivativeForX(x);
            }
            return Round(rj, 0);
        }
        public double DerivativeForX(int x)
        {
            return DerivativeForX(Convert.ToDouble(x));
        }
        protected void CheckIfOneCanInterpolate(List<Point> Points)
        {
            foreach (Point p in Points)
            {
                int n = 0;
                for (int i = 0; i < Points.Count; i++)
                {
                    if (Points[i].x == p.x)
                        n++;
                }
                if (n != 1)
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
            for (int i = 1; i < Points.Count; i++)
            {
                Monomz.Add(Monoms);
            }
            Interpolate(Points, Monomz);
        }
        public void Interpolate(List<Point> Points, List<KindOfMonom> Monoms)
        {
            List<List<double>> Matrix = new List<List<double>>(Points.Count);
            Polinom = new List<Monom>();
            int lastLine = -1, lastSine = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                Matrix.Add(new List<double>());
                for (int j = 0; j < Points.Count + 2; j++)
                {
                    Matrix[i].Add(0);
                }
            }
            for (int i = 0; i < Points.Count; i++)
            {

                switch (Monoms[i])
                {
                    case KindOfMonom.Constant:
                        Polinom.Add(new Monom(1));
                        for (int j = 0; j < Points.Count; j++)
                        {
                            Matrix[j][i] = 1;
                        }
                        break;
                    case KindOfMonom.Line:
                        lastLine++;
                        Polinom.Add(new Monom(KindOfMonom.Line, 1, lastLine));
                        for (int j = 1; j < Points.Count; j++)
                        {
                            Point p = Points[j];
                            Matrix[j][i] = Pow(p.x, lastLine);
                        }
                        break;
                    case KindOfMonom.Sine:
                        lastSine++;
                        Polinom.Add(new Monom(KindOfMonom.Sine, 1, lastSine));
                        for (int j = 1; j < Points.Count; j++)
                        {
                            Point p = Points[j];
                            Matrix[j][i] = Sin(lastSine * p.x);
                        }
                        break;
                }

                Matrix[i][Points.Count] = (Points[i].y);
                //start x
                Matrix[i][Points.Count + 1] = i;
            }
            List<double> aas = SolveMatrix(Matrix);
            for (int i = 0; i < aas.Count; i++)
            {
                Polinom[i].InterpolatedValue = aas[i];
            }
        }
        public void Interpolate(List<Point> Points, List<KindOfMonom> Monoms, bool MustBeCorrect)
        {
            if (MustBeCorrect)
                CheckIfOneCanInterpolate(Points);
            Interpolate(Points, Monoms);
        }
        public string LinearRepresentationOfPolinom()
        {
            string s = string.Empty;
            foreach (Monom m in Polinom)
            {
                s += " " + m.LinearRepresentationOfMonom() + " ";
            }
            return s;
        }
        public List<double> SolveMatrix(List<List<double>> Matrix)
        {
            if (Matrix[0].Count - 2 != Matrix.Count)
                throw new Exception("One can only interpolate Matrix of size n*n+2\n a*X=BottomBound a:= n*n matrix, BottomBound:= 1*n combination, and 1*n matrix where are saved original indexes of rows");
            #region down
            int n = Matrix.Count;
            if (Matrix[0].Count - 1 == Matrix.Count)
            {
                for (int i = 0; i < Matrix.Count; i++)
                {
                    Matrix[i].Add(i);
                }
            }
            double miny = 0.0001;
            for (int i = 0; i < n; i++)
            {
                if (Abs(Matrix[i][i]) - miny <= 0)
                {
                    bool found = false;
                    for (int j = n - 1; j > i; j--)
                    {
                        if (Abs(Matrix[j][i]) - miny > 0)
                        {
                            List<double> temp = Matrix[i];
                            Matrix[i] = Matrix[j];
                            Matrix[j] = temp;
                            found = true;
                            j = i;
                        }
                    }
                    if (!found)
                        throw new Exception("Sorry can't interpolate :(");
                }

                for (int j = i + 1; j < n; j++)
                {
                    double koeficjent = Matrix[j][i] / Matrix[i][i];

                    for (int k = 0; k < n + 1; k++)
                    {
                        Matrix[j][k] = Matrix[j][k] - (koeficjent * Matrix[i][k]);
                    }
                }
            }

            #endregion

            #region up
            for (int i = (n - 1); i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    double koeficjent = new double();
                    koeficjent = Matrix[j][i] / Matrix[i][i];
                    Matrix[j][i] = 0;
                    Matrix[j][n] = Matrix[j][n] - koeficjent * Matrix[i][n];
                }
            }
            #endregion
            #region solve
            List<double> soluton = new List<double>(n);
            for (int i = 0; i < n; i++)
            {
                soluton.Add(1);
            }
            for (int i = 0; i < n; i++)
            {
                soluton[Convert.ToInt32(Matrix[i][n + 1])] = Matrix[i][n] / Matrix[i][i];
            }
            #endregion
            return soluton;
        }
    }
}
