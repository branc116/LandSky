using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

using LandSky.MyEnums;

namespace LandSky.UIComponents
{
    class TextBox : UIComponentBase
    {
        
        private string mAcceptedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTVWXYZ1234567890";
        private int mCursorLeft;
        private int mCursorTop;
        public TextBox(string Name,int TabIndex, int Top, int Left) : base(Name, TabIndex, Top, Left)
        {
        }
        public TextBox(string Name,int TabIndex, int Top, int Left, string InitText) : base (Name, TabIndex, Top, Left, InitText)
        {   
            this.mCursorLeft = Text.Split(new char[] { '\n' }).Last().Length;
            this.mCursorTop = this.Size.Height;
        }
        public TextBox(string Name,int TabIndex, int Top, int Left, string InitText, string AditionalSpecialCharacters) : base(Name, TabIndex, Top, Left, InitText)
        {            
            this.mCursorLeft = InitText.Length;
            mAcceptedCharacters += AditionalSpecialCharacters;
        }

        public override bool NewInput(ConsoleKeyInfo KeyInfo)
        {
            if (mAcceptedCharacters.Any(i => i == KeyInfo.KeyChar)) {
                InsertCharInText(mCursorTop, mCursorLeft, KeyInfo.KeyChar);
                return true;
            }
            else
            {
                switch (KeyInfo.Key)
                {
                    case ConsoleKey.Backspace:
                        RemoveCharFormText(1);
                        return true;
                    case ConsoleKey.Delete:
                        RemoveCharFormText(-1);
                        return true;
                    case ConsoleKey.UpArrow:
                        MoveCursureVertical(1);
                        return true;
                    case ConsoleKey.DownArrow:
                        MoveCursureVertical(-1);
                        return true;
                    case ConsoleKey.RightArrow:
                        MoveCursureHorisontal(1);
                        return true;
                    case ConsoleKey.LeftArrow:
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
            if (mCursorLeft == 0 && v > 0)
            {
                if (mCursorTop != 0) {
                    mCursorLeft = mLinesOfText[mCursorTop - 1].Length;
                    mLinesOfText[mCursorTop - 1] += mLinesOfText[mCursorTop];
                    mLinesOfText.RemoveAt(mCursorTop);
                    mCursorTop--;
                }
            }
            else if (mCursorLeft == mLinesOfText[mCursorTop].Length && v < 0)
            {
                if (mCursorTop != mLinesOfText.Count)
                {
                    mLinesOfText[mCursorTop] += mLinesOfText[mCursorTop + 1];
                    mLinesOfText.RemoveAt(mCursorTop + 1);
                }
            }
            else if (v > 0)
            {
                mLinesOfText[mCursorTop].Remove(Max(0, mCursorLeft - v), v);
            }
            else if (v < 0)
            {
                mLinesOfText[mCursorTop].Remove(mCursorLeft, v);
            }
            LinesUpdated();
        }
        private void InsertCharInText(int top, int left, char c)
        {
            string OutText = string.Empty;
            mLinesOfText[mCursorTop].Insert(mCursorLeft, c.ToString());
            LinesUpdated();

        }
        public override string ToString()
        {
            string[] Lines = new string[mLinesOfText.Count];
            mLinesOfText.CopyTo(Lines);
            Lines[mCursorTop].Insert(mCursorLeft, "|");

            int MaxWitdth = SizeMode == SizeMode.Auto ? Lines.Max(i => i.Length) : this.mSize.Width;
            string OutString = Focus ?  ("-" + new string('+', MaxWitdth - 2) + "-") : ("+" + new string('-', MaxWitdth - 2) + "+");

            foreach (var Line in Lines)
            {
                OutString += $"|{Line}{new string(' ', Max(0, MaxWitdth - Line.Length))}|".Substring(0, SizeMode == SizeMode.Auto ? Line.Length + 2 : Min(Line.Length + 2, mSize.Width)) + '\n';
            }
            if (!Lines.Any())
                OutString += $"|{Hint}{new string(' ', Max(0, MaxWitdth - Hint.Length))}|";

            if (SizeMode == SizeMode.Explicit)
            {
                int currentLine = 0;
                OutString = new string(OutString.Select(i =>
                {
                    if (i == '\n')
                        currentLine++;
                    return currentLine + 2 < this.mSize.Height ? i : '\0';
                }).ToArray());
            }
            OutString += Focus ? ("-" + new string('+', MaxWitdth - 2) + "-") : ("+" + new string('-', MaxWitdth - 2) + "+");
            return OutString;

        }
    }
}
