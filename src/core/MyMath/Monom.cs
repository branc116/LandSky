using System;
using static System.Math;

using LandSky.MyEnums;

namespace LandSky.MyMath
{

    /// <summary>
    /// This is monom just like in math
    /// </summary>
    public class Monom
    {
        
        private double mA, mB, mC;
        private const double mTolerance = 10e-6;

        public KindOfMonom ThisKindOfMonom;
        public KindOfMonom SetThisKindOfMonomKind
        {
            get
            {
                return ThisKindOfMonom;
            }
            set
            {
                mA = mB = mC = 0;
                ThisKindOfMonom = value;
            }
        }
        public double ParamaterA => ThisKindOfMonom != KindOfMonom.Constant ? mA : 0;
        public double ParamaterB => ThisKindOfMonom != KindOfMonom.Constant ? mB : 0;
        public double Constant => ThisKindOfMonom == KindOfMonom.Constant ? mC : 0;
        public double InterpolatedValue
        {
            get {
                return this.ThisKindOfMonom != KindOfMonom.Constant ? mA : mC;
            }
            set
            {
                if (this.ThisKindOfMonom != KindOfMonom.Constant)
                    mA = value;
                else
                    mC = value;
            }
        }

        public Monom(KindOfMonom ThisKindOfMonom, double ParamaterA, double ParamaterB)
        {
            if (ThisKindOfMonom != KindOfMonom.Constant)
            {
                mA = ParamaterA;
                mB = ParamaterB;
            }
            else
            {
                mC = ParamaterA;
            }
            this.ThisKindOfMonom = ThisKindOfMonom;
        }
        public Monom(double Constant)
        {
            ThisKindOfMonom = KindOfMonom.Constant;

        }
        public double ValuForX(double X)
        {
            switch (ThisKindOfMonom)
            {
                case KindOfMonom.Line:
                    return mA * Pow(X, mB);
                case KindOfMonom.Sine:
                    return mA * Sin(mB * X);
                case KindOfMonom.Constant:
                    return mC;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        public double DerivativeForX(double X)
        {
            switch (ThisKindOfMonom)
            {
                case KindOfMonom.Constant:
                    return 0;
                case KindOfMonom.Line:
                    if (Abs(mB - 1) < mTolerance)
                        return mA;
                    if (Abs(mB) < mTolerance)
                        return 0;
                    return mA*mB*Pow(X, mB - 1);
                case KindOfMonom.Sine:
                    if (Abs(mB) < mTolerance)
                        return 0;
                    return mB * mA * Cos(mB * X);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public string LinearRepresentationOfMonom()
        {
            const string plus = "+";
            switch (ThisKindOfMonom)
            {
                case KindOfMonom.Constant:
                    if (mC > 0)
                        return plus + $"{Round(mC, 2)}";
                    return mC < 0 ? $"{Round(mC, 2)}" : string.Empty;
                case KindOfMonom.Line:
                    if (mB > 1)
                    {
                        if (mA > 0)
                            return plus + $"{Round(mA, 2)}X^{Round(mB, 2)}";
                        return mA < 0 ? $"{Round(mA, 2)}X^{Round(mB, 2)}" : string.Empty;
                    }
                    if (Abs(mB - 1) < mTolerance)
                    {
                        if (mA > 0)
                            return plus + $"{Round(mA, 2)}X";
                        return mA < 0 ? $"{Round(mA, 2)}X" : string.Empty;
                    }
                    else
                    {
                        if (mA > 0)
                            return plus + $"{Round(mA, 2)}";
                        return mA < 0 ? $"{Round(mA, 2)}" : string.Empty;
                    }
                case KindOfMonom.Sine:
                    if (Abs(mB) > mTolerance)
                    {
                        if (Abs(mA - 1) < mTolerance)
                            return plus + $"Sin(X{Round(mB, 2)})";
                        if (Abs(mA + 1) < mTolerance)
                            return plus + $"-Sin(X{Round(mB, 2)})";
                        if (mA > 0)
                            return plus + $"{Round(mA, 2)}Sin({Round(mB, 2)}X)";
                        return mA < 0 ? $"{Round(mA, 2)}Sin({Round(mB, 2)}X)" : string.Empty;
                    }
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
