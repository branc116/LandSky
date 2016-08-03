using static System.Math;

using MultyNetHack.MyEnums;

namespace MultyNetHack.MyMath
{

    /// <summary>
    /// This is monom just like in math
    /// </summary>
    public class Monom
    {
        KindOfMonom monom;

        double a, b, c;
        public KindOfMonom SetMonomKind
        {
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
                        if (a == 1)
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
}
