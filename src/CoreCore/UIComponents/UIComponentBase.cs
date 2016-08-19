using LandSky.Commands;
using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandSky.UIComponents
{
    public abstract class UIComponentBase
    {
        public int Top
        {
            get
            {
                return mTop;
            }
            set
            {
                mTop = value;
                OnMove?.Invoke(this, DateTime.Now);
            }
        }

        public int Left
        {
            get
            {
                return mLeft;
            }
            set
            {
                mLeft = value;
                OnMove?.Invoke(this, DateTime.Now);
            }
        }

        public Size Size
        {
            get
            {
                int MaxLenght = mLinesOfText.Max(i => i.Length);
                return SizeMode == SizeMode.Auto ? new Size(MaxLenght, mLinesOfText.Count) : mSize;
            }
            set
            {
                SizeMode = SizeMode.Explicit;
                mSize = value;
                OnSizeChanged?.Invoke(this, DateTime.Now);
            }
        }

        public string Name { get; set; }

        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
                OnTextChanged?.Invoke(this, DateTime.Now);
            }
        }

        public int TabIndex { get; set; } = 0;
        public bool Focus { get; set; } = false;
        public string Hint { get; set; } = "Hint not set";
        public SizeMode SizeMode { get; set; } = SizeMode.Auto;
        public Rectangle Bounds => new Rectangle(Top, Left + Size.Width, Top + Size.Height, Left);

        public event EventHandler<DateTime> OnFocusGained;

        public event EventHandler<DateTime> OnFocusLost;

        public event EventHandler<DateTime> OnAccept;

        public event EventHandler<DateTime> OnMove;

        public event EventHandler<DateTime> OnTextChanged;

        public event EventHandler<DateTime> OnStart;

        public event EventHandler<DateTime> OnSizeChanged;

        public event EventHandler<DateTime> OnTab;

        private int mTop = 0;
        private int mLeft = 0;
        private bool mForcus = false;

        protected Size mSize;
        protected List<string> mLinesOfText = new List<string>() { string.Empty };
        protected string mText = "";

        protected UIComponentBase(string Name)
        {
            this.Name = Name;
            OnStart?.Invoke(this, DateTime.Now);
        }

        protected UIComponentBase(string Name, int TabIndex)
        {
            this.Name = Name;
            this.TabIndex = TabIndex;
            OnStart?.Invoke(this, DateTime.Now);
        }

        protected UIComponentBase(string Name, int TabIndex, int Top, int Left)
        {
            this.Name = Name;
            this.Top = Top;
            this.Left = Left;
            this.TabIndex = TabIndex;
            OnStart?.Invoke(this, DateTime.Now);
        }

        protected UIComponentBase(string Name, int TabIndex, int Top, int Left, string InitText)
        {
            this.Name = Name;
            this.Top = Top;
            this.Left = Left;
            this.Text = InitText;
            this.TabIndex = TabIndex;
            OnStart?.Invoke(this, DateTime.Now);
        }

        protected void LinesUpdated()
        {
            mText = string.Empty;
            foreach (var Line in mLinesOfText)
            {
                mText += Line + '\n';
            }
        }

        public virtual bool NewInput(ConsoleKeyInfo KeyInfo)
        {
            switch (KeyInfo.Key)
            {
                case ConsoleKey.Tab:
                    OnTab?.Invoke(this, DateTime.Now);
                    return true;

                case ConsoleKey.Enter:
                    OnAccept?.Invoke(this, DateTime.Now);
                    return true;
            }
            return false;
        }

        public void InvokeAccept(BaseCommand BaseCommand)
        {
            OnAccept?.Invoke(this, DateTime.Now);
        }
    }
}