using LandSky.DotNetExt;
using LandSky.MyEnums;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace LandSky.UIComponents
{
    internal class TextBox : UIComponentBase
    {
        private List<char> mAcceptedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 ".ToCharArray().ToList();
        private int mCursorLeft;
        private int mCursorTop;

        public TextBox(string Name, int TabIndex, int Top, int Left) : base(Name, TabIndex, Top, Left)
        {
        }

        public TextBox(string Name, int TabIndex, int Top, int Left, string InitText) : base(Name, TabIndex, Top, Left, InitText)
        {
            this.mCursorLeft = Text.Split(new char[] { '\n' }).Last().Length;
            this.mCursorTop = this.Size.Height;
        }

        public TextBox(string Name, int TabIndex, int Top, int Left, string InitText, string AditionalSpecialCharacters) : base(Name, TabIndex, Top, Left, InitText)
        {
            this.mCursorLeft = InitText.Length;
            mAcceptedCharacters.AddRange(AditionalSpecialCharacters.ToCharArray());
        }

        public override bool NewInput(MyConsoleKeyInfo KeyInfo)
        {
            if (mAcceptedCharacters.Any(i => i == KeyInfo.KeyChar))
            {
                InsertCharInText(mCursorTop, mCursorLeft, KeyInfo.KeyChar);

                return true;
            }
            else
            {
                switch (KeyInfo.KeyChar)
                {
                    case (char)SpecialAsciiKeys.Backspace:
                        RemoveCharFormText(1);
                        return true;

                    case (char)SpecialAsciiKeys.Del:
                        RemoveCharFormText(-1);
                        return true;

                    case (char)SpecialAsciiKeys.ArrowUp:
                        MoveCursureVertical(1);
                        return true;

                    case (char)SpecialAsciiKeys.ArrowDown:
                        MoveCursureVertical(-1);
                        return true;

                    case (char)SpecialAsciiKeys.ArrowRight:
                        MoveCursureHorisontal(1);
                        return true;

                    case (char)SpecialAsciiKeys.ArrowLeft:
                        MoveCursureHorisontal(-1);
                        return true;
                }
            }
            return base.NewInput(KeyInfo);
        }

        private void MoveCursureHorisontal(int v)
        {
            mCursorLeft = Max(0, Min(mCursorLeft + v, mLinesOfText[mCursorTop].Length));
        }

        private void MoveCursureVertical(int v)
        {
            mCursorTop = Max(0, Min(mLinesOfText.Count - 1, mCursorTop + v));
            mCursorLeft = Min(mCursorLeft, mLinesOfText[mCursorTop].Length - 1);
        }

        private void RemoveCharFormText(int v)
        {
            string aLine = mLinesOfText[mCursorTop];
            int pointA = Min(aLine.Length, Max(0, mCursorLeft - v));
            int pointB = Min(aLine.Length, Max(0, mCursorLeft));
            mLinesOfText[mCursorTop] = $"{aLine.Substring(0, Min(pointA, pointB))}{aLine.Substring(Max(pointA, pointB))}";
            mCursorLeft -= v;
            LinesUpdated();
        }

        private void InsertCharInText(int top, int left, char c)
        {
            string OutText = string.Empty;
            string before = mLinesOfText[mCursorTop].Substring(0, mCursorLeft);
            string after = mLinesOfText[mCursorTop].Substring(mCursorLeft);
            mLinesOfText[mCursorTop] = $"{before}{c}{after}";
            mCursorLeft++;
            LinesUpdated();
        }

        public override string ToString()
        {
            List<string> Lines = new List<string>();
            if (mLinesOfText.Count > 0 && mLinesOfText[0].Length > 0)
            {
                foreach (var line in mLinesOfText)
                {
                    Lines.Add(line);
                }
            }
            else
            {
                Lines.Add(Hint);
            }
            string aLine = Lines[mCursorTop];
            Lines[mCursorTop] = $"{aLine.Substring(0, mCursorLeft)}|{aLine.Substring(mCursorLeft)}";
            int MaxWitdth = SizeMode == SizeMode.Auto ? Lines.Max(i => i.Length) : this.mSize.Width;
            string OutString = Focus ? ("-" + new string('+', MaxWitdth) + "-\n") : ("+" + new string('-', MaxWitdth) + "+\n");

            foreach (var Line in Lines)
            {
                OutString += $"|{Line}{new string(' ', Max(0, MaxWitdth - Line.Length))}|".Substring(0, SizeMode == SizeMode.Auto ? Line.Length + 2 : Min(Line.Length + 2, mSize.Width)) + '\n';
            }
            if (!Lines.Any())
                OutString += $"|{Hint}{new string(' ', Max(0, MaxWitdth - Hint.Length))}|";

            if (SizeMode == SizeMode.Explicit)
            {
                int currentLine = 0;
                OutString = new string(OutString.ToCharArray().Select(i =>
                {
                    if (i == '\n')
                        currentLine++;
                    return currentLine + 2 < this.mSize.Height ? i : '\0';
                }).ToArray());
            }
            OutString += Focus ? ("-" + new string('+', MaxWitdth) + "-") : ("+" + new string('-', MaxWitdth) + "+");
            return OutString;
        }
    }
}