using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
namespace MultyNetHack
{
    /// <summary>
    /// What Kinde of monom are you working whith
    /// </summary>
   

    public class Monom
    {
        KindOfMonom monom;

        double a, b, c;
        public KindOfMonom SetMonomKind{
            set
            {
                a = b = c = 0;
                monom = value;
            }
        }
        public double ParamaterA
        {
            get
            {
                if (monom != KindOfMonom.Constant)
                    return a;
                else
                    return 0;
            }
        }
        public double ParamaterB
        {
            get
            {
                if (monom != KindOfMonom.Constant)
                    return b;
                else
                    return 0;
            }
        }
        public double Constant
        {
            get
            {
                if (monom == KindOfMonom.Constant)
                    return c;
                else
                    return 0;
            }
        }
        public double InterpolatedValue
        {
            set
            {
                if (this.monom != KindOfMonom.Constant)
                    a = value;
                else
                    c = value;
            }
        }
        public Monom(KindOfMonom Monom, double ParamaterA, double ParamaterB)
        {
            if (Monom != KindOfMonom.Constant)
            {
                a = ParamaterA;
                b = ParamaterB;
            }
            else
            {
                c = ParamaterA;
            }
            monom = Monom;
        }
        public Monom(double Constant)
        {
            monom = KindOfMonom.Constant;

        }
        public double ValuForX(double x)
        {
            if (monom == KindOfMonom.Line)
                return a * Pow(x, b);
            else if (monom == KindOfMonom.Sine)
                return a * Sin(b * x);
            else
                return c;
            
        }
        public double DerivativeForX(double x)
        {
            if (monom == KindOfMonom.Constant)
                return 0;
            else if (monom == KindOfMonom.Line)
            {
                if (b == 1)
                    return a;
                if (b == 0)
                    return 0;
                return a * b * Pow(x, b - 1);
            }
            else
            {
                if (b == 0)
                    return 0;
                return b * a * Cos(b * x);
            }
        }
        public string LinearRepresentationOfMonom()
        {
            string plus = "+";
            switch (monom)
            {
                case KindOfMonom.Constant:
                    if (c > 0)
                        return plus + string.Format("{0}", Round(c, 2));
                    else if (c < 0)
                        return string.Format("{0}", Round(c, 2));
                    else
                        return string.Empty;
                case KindOfMonom.Line:
                    if (b > 1)
                    {

                        if (a > 0)
                            return plus + string.Format("{0}X^{1}", Round(a, 2), Round(b, 2));
                        else if (a < 0)
                            return string.Format("{0}X^{1}", Round(a, 2), Round(b, 2));
                        else
                            return string.Empty;
                    }
                    else if (b == 1)
                    {
                        if (a > 0)
                            return plus + string.Format("{0}X", Round(a, 2));
                        else if (a < 0)
                            return string.Format("{0}X", Round(a, 2));
                        else
                            return string.Empty;
                    }
                    else
                    {
                        if (a > 0)
                            return plus + string.Format("{0}", Round(a, 2));
                        else if (a < 0)
                            return string.Format("{0}", Round(a, 2));
                        else
                            return string.Empty;
                    }
                default:
                    if (b != 0)
                    {
                        if(a==1)
                            return plus + string.Format("Sin(X{1})", Round(b, 2));
                        if (a == -1)
                            return plus + string.Format("-Sin(X{1})", Round(b, 2));
                        if (a > 0)
                            return plus + string.Format("{0}Sin({1}X)", Round(a, 2), Round(b, 2));
                        else if (a < 0)
                            return string.Format("{0}Sin({1}X)", Round(a, 2), Round(b, 2));
                        else
                            return string.Empty;
                    }
                    else
                    {
                        return string.Empty;
                    }
            }
        }
    }
    public class Rectangle
    {

        public int l;
        public int r;
        public int t;
        public int b;
        public int width
        {
            get
            {
                return this.r - this.l;
            }
        }
        public int height
        {
            get
            {
                return this.t - this.b;
            }
        }
        public Rectangle(int t,int r,int b,int l)
        {
            this.l = l;
            this.r = r;
            this.t = t;
            this.b = b;
        }
        /// <summary>
        ///     |
        ///     | l,t___
        ///     |   |x,y|
        ///     |    ---
        /// -------------------
        ///     |
        ///     |
        ///     |
        ///     |
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point ToTopLeft(int x, int y)
        {
            int top = Math.Min(this.height-1, Math.Max(1, this.t - y));
            int left = Math.Min(this.width-1, Math.Max(1, x-this.l));
            Point mP = new Point(top,left);
            return mP;

        }
        public static Rectangle operator -(Rectangle rc, Point p)
        {
            return new Rectangle(rc.t - p.y, rc.r - p.x, rc.b - p.y, rc.l - p.x);
        }
        public static bool operator &(Rectangle one, Rectangle two)
        {
            return (one.l < two.r && one.r > two.l &&
                    one.t > two.b && one.b < two.t);
        }
    }
    public class Point
    {
        public int x, y;
        public bool enter;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Point ToTopLeft(Size s, int x, int y)
        {
            Point mP = new Point(Math.Max(1, Math.Min(s.width - 1, x - this.x)), Math.Max(1, Math.Min(s.height - 2, -y + this.y)));
            return mP;

        }
        public static Point operator -(Point a, Point b)
        {
            return new Point(b.x - a.x, b.y - a.y);
        }
    }
    public class Size
    {
        public int height, width;
        public Size(int width, int height)
        {
            this.height = height;
            this.width = width;
        }
        
    }
    public class LinearInterpolator
    {
        List<Monom> Polinom;
        List<double> Values;
        public LinearInterpolator()
        {
            Polinom = new List<Monom>();

        }
        public double ValueForX(double x)
        {
            double rj = 0;
            foreach(Monom m in Polinom)
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
            foreach(Monom m in Polinom)
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
                throw new Exception("One can't interpolate array of points only with constants, one can but it wount be good... don't select contants, pls, select line it's good, or sine");
            List<KindOfMonom> Monomz = new List<KindOfMonom>(Points.Count);
            Monomz.Add(KindOfMonom.Constant);
            for (int i=1; i<Points.Count;i++)
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
            for(int i = 0; i < Points.Count; i++)
            {
                Matrix.Add(new List<double>());
                for (int j=0;j<Points.Count + 2; j++)
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
                
                Matrix[i][Points.Count] =(Points[i].y);
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
            foreach(Monom m in Polinom)
            {
                s += " " + m.LinearRepresentationOfMonom() + " ";
            }
            return s;
        }
        public List<double> SolveMatrix(List<List<double>> Matrix)
        {
            if (Matrix[0].Count - 2 != Matrix.Count)
                throw new Exception("One can only interpolate Matrix of size n*n+2\n a*X=b a:= n*n matrix, b:= 1*n combination, and 1*n matrix where are saved original inexes of rows");
#region down
            int n = Matrix.Count;
            if (Matrix[0].Count - 1 == Matrix.Count)
            {
                for (int i = 0; i < Matrix.Count;i++)
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
                    //matrica[j][i] = 0;

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
                soluton[Convert.ToInt32(Matrix[i][n+1])] = Matrix[i][n] / Matrix[i][i];
            }
#endregion
            return soluton;
        }
    }
}
