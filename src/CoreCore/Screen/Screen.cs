using LandSky.Commands;
using LandSky.Components;
using LandSky.DebugItems;
using LandSky.MyEnums;
using LandSky.MyEventArgs;
using LandSky.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

/// <summary>
/// All of default screens are in this namespace
/// </summary>
namespace LandSky.Screen
{
    /// <summary>
    /// Use this to extend your screen
    /// For eg. look at EngineScreen.cs
    /// </summary>
    public class BaseScreen : Component
    {
        /// <summary>
        /// Hight of the middle part of the screen. Calculated by subtracting Height of header and footer from current Height.
        /// </summary>
        public int BodyHeight
        {
            get
            {
                return TrueHeight - HeadHeight - FooterHeight;
            }
        }

        /// <summary>
        /// Minimum value of wanted Height and maximum Height
        /// </summary>
        public int TrueHeight
        {
            get
            {
                return MScreenHeight;
            }
        }

        /// <summary>
        /// Minimum value of wanted Height and maximum Width
        /// </summary>
        public int TrueWidth
        {
            get
            {
                return MScreenWidth - 1;
            }
        }

        /// <summary>
        /// Wanted Height of the screen
        /// </summary>
        public int WantedHeight
        {
            set
            {
                MScreenHeight = value;
                ScreenChange();
            }
            get
            {
                return MScreenHeight;
            }
        }

        /// <summary>
        /// Wanted Width of the screen
        /// </summary>
        public int WantedWidth
        {
            set
            {
                MScreenWidth = value;
                ScreenChange();
            }
            get
            {
                return MScreenWidth;
            }
        }

        /// <summary>
        /// Calculate current position of the cursure in the virtual console
        /// </summary>
        public int VirtualConsoleLeft
        {
            get
            {
                return mCursorLeft;
            }
            set
            {
                if (value < 0 || value >= TrueWidth)
                    throw new Exception($"value {value} out of range");
                mBodyStringLines[mCursorTop] += new string(' ', Max(0, value - mBodyStringLines[mCursorTop].Length));
                mCursorLeft = value;
            }
        }

        /// <summary>
        /// Calculate number of lines the body part of the screen has
        /// </summary>
        public int VirtualConsoleTop
        {
            get
            {
                return mCursorTop;
            }
            set
            {
                if (value < 0)
                    throw new Exception($"value {value} out of range");
                for (int i = mBodyStringLines.Count; i <= value; i++)
                {
                    mBodyStringLines.Add(string.Empty);
                }

                mCursorTop = value;
            }
        }

        /// <summary>
        /// Current text in the virtual console
        /// </summary>
        public string VirtualConsole
        {
            get
            {
                int mActiveFrom = ActiveFrom;
                mActiveTo = ActiveTo;
                int mTrueWidth = TrueWidth;
                int mTrueHeight = TrueHeight;

                string S = '+' + new string('-', mTrueWidth) + '+' + '\n';
                HeadString.Split('\n').ToList().ForEach((I) =>
                {
                    S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' + '\n';
                });
                S += '|' + new string('-', mTrueWidth) + '|' + '\n';
                for (int i = mActiveFrom; i < mActiveTo; i++)
                {
                    string I = mBodyStringLines[i];
                    S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' + '\n';
                }
                S += '|' + new string('-', mTrueWidth) + '|' + '\n';
                FooterString.Split('\n').ToList().ForEach((I) =>
                {
                    S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' + '\n';
                });
                S += '+' + new string('-', mTrueWidth) + '+';
                mVirtualConsole = S;
                return mVirtualConsole;
            }
        }

        /// <summary>
        /// Position of the body part
        /// </summary>
        public int Scrool { get; set; }

        /// <summary>
        /// Active screen that are displayed. Push on stack new screen to pause the current screen and display new screen
        /// </summary>
        public static Stack<BaseScreen> Active = new Stack<BaseScreen>();

        /// <summary>
        /// Commands that can be used in the current screen
        /// </summary>
        public Dictionary<Comands, Action<BaseCommand>> Comand;

        /// <summary>
        /// Q of unread messages. This is used for debugging.
        /// </summary>
        public static Queue<DebugMessage> UnreadMessages = new Queue<DebugMessage>();

        /// <summary>
        /// List of all debug messages. This is used for debugging.
        /// </summary>
        public static List<DebugMessage> AllMessages = new List<DebugMessage>();

        public UIComponentsCollection UIComponents = new UIComponentsCollection();

        public InputMode InputMode { get; set; } = InputMode.ControlOnly;

        public event EventHandler<ScreenChangedArgs> ScreenChanged;

        public ScreenChangedArgs CurrentStateOfTheScreen;

        /// <summary>
        /// Commands in screens that extend Screen method.
        /// </summary>
        protected Dictionary<Comands, Action<BaseCommand>> MLocalCommands;

        /// <summary>
        /// Determent what line of virtual console should be printed out first
        /// </summary>
        protected int ActiveFrom
        {
            get
            {
                return Scrool;
            }
        }

        /// <summary>
        /// Determent what line of virtual console should be printed out last
        /// </summary>
        protected int ActiveTo
        {
            get
            {
                mVirtualConsoleTop = VirtualConsoleTop;
                mActiveTo = Min(mVirtualConsoleTop, Min(Scrool + mVirtualConsoleTop, Scrool + WantedHeight - HeadHeight * 2));
                return mActiveTo;
            }
        }

        protected int MScrool, GlobalLeft, GlobalTop, HeadHeight, FooterHeight, MScreenHeight, MScreenWidth;

        ///// <summary>
        ///// Invoke this when something changes
        ///// </summary>
        //protected event EventHandler Change;
        /// <summary>
        /// String that is displayed on top of everything
        /// </summary>
        protected string HeadString;

        /// <summary>
        /// String that is displayed on the bottom of the screen
        /// </summary>
        protected string FooterString;

        private string mVirtualConsole;

        /// <summary>
        /// This is displayed in the middle of the screen
        /// </summary>
        private string mBodyString;

        private List<string> mBodyStringLines = new List<string>() { string.Empty };
        private int mCursorTop = 0, mCursorLeft = 0;
        private object mObjectToLockFlush;
        private int mVirtualConsoleTop, mActiveTo;

        /// <summary>
        /// Create new base screen. One shouldn't really do this. One should extend new method and then call the constructor for that new method.
        /// </summary>
        /// <param Name="Top">Global position in the global console </param>
        /// <param Name="Left">Global position in the global console </param>
        /// <param Name="Name">Name of the Screen. It will probably be displayed in the title.</param>
        public BaseScreen(int Top, int Left, string Name) : base(Name)
        {
            InitProperties(Top, Left);
            InitComands();
            GenerateHeader();
            GenerateFooter();
        }

        /// <summary>
        /// Create new base screen. One shouldn't really do this. One should extend new method and then call the constructor for that new method. Name will be "UnNamed"
        /// </summary>
        /// <param Name="Top">Global position in the global console </param>
        /// <param Name="Left">Global position in the global console </param>
        public BaseScreen(int Top, int Left) : base("UnNamed")
        {
            InitProperties(Top, Left);
            InitComands();
            GenerateHeader();
            GenerateFooter();
        }

        /// <summary>
        /// This is called when creating new BaseScreen to initiate global properties
        /// </summary>
        /// <param Name="Top"></param>
        /// <param Name="Left"></param>
        private void InitProperties(int Top, int Left)
        {
            mBodyString = string.Empty;
            GlobalLeft = Left;
            GlobalTop = Top;
            WantedWidth = 50;
            WantedHeight = 20;
            Comand = new Dictionary<Comands, Action<BaseCommand>>();
            MLocalCommands = new Dictionary<Comands, Action<BaseCommand>>();
        }

        /// <summary>
        /// This is called when creating new BaseScreen to initiate command mapping
        /// </summary>
        private void InitComands()
        {
            Comand.Add(Comands.ScrollLeft, ScrollCommand);
            Comand.Add(Comands.ScrollRight, ScrollCommand);
            Comand.Add(Comands.ShowHelp, ShowHelp);
            Comand.Add(Comands.ShowDebug, ShowDebug);
            Comand.Add(Comands.LastSceen, PopSceen);
            Comand.Add(Comands.ToJSON, ScreenToJSON);
        }

        /// <summary>
        /// Destroys current screen and displays the screen below it
        /// </summary>
        /// <param Name="bc"></param>
        private void PopSceen(BaseCommand Bc)
        {
            this.Pause();
            Active.Pop();
            if (Active.Count == 0)
                Environment.Exit(0);
            Active.Peek().Resume();
        }

        /// <summary>
        /// Show the debug screen on top of current screen
        /// </summary>
        /// <param Name="bc"></param>
        private void ShowDebug(BaseCommand Bc)
        {
            if (this.GetType() != typeof(HelpScreen) && this.GetType() != typeof(DebugScreen))
            {
                Active.Push(new DebugScreen(0, 0, this));
            }
        }

        /// <summary>
        /// Show help screen on top of current screen
        /// </summary>
        /// <param Name="bc"></param>
        private void ShowHelp(BaseCommand Bc)
        {
            if (this.GetType() != typeof(HelpScreen) && this.GetType() != typeof(DebugScreen))
            {
                Active.Push(new HelpScreen(GlobalTop, GlobalLeft));
            }
        }

        /// <summary>
        /// Scroll the body part down by some number
        /// </summary>
        /// <param Name="bc">Should be typeof(ScrollCommand) </param>
        private void ScrollCommand(BaseCommand Bc)
        {
            ScrollCommand Scroll = Bc as ScrollCommand;
            Scrool += Scroll.N;
            if (VirtualConsoleTop > 0)
                Scrool %= VirtualConsoleTop;
            if (Scrool < 0)
                Scrool = VirtualConsoleTop;

            ScreenChange();
        }

        /// <summary>
        /// Needs to be implemented
        /// </summary>
        /// <param Name="bc"></param>
        private void ChangeScene(BaseCommand Bc)
        {
        }

        private void DrawUIElements()
        {
            foreach (var element in UIComponents)
            {
                var s = element.ToString();
                VirtualConsoleTop = element.Top;

                foreach (var line in s.Split(new char[] { '\n' }))
                {
                    VirtualConsoleLeft = element.Left;
                    VirtualConsoleAdd(line);
                    VirtualConsoleTop++;
                }
            }
        }

        private async void ScreenToJSON(BaseCommand Bc)
        {
            ScreenToJsonCommand Comm = Bc as ScreenToJsonCommand;
            await SaveStateToDisc($"{Comm.FileName}_{DateTime.Now.ToString().Replace(':', '_').Replace('/', '_')}.json");
        }

        /// <summary>
        /// this is called when the screen stops being on top of the stack
        /// </summary>
        protected virtual void Pause()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> Kvp in MLocalCommands)
            {
                Comand.Remove(Kvp.Key);
            }
        }

        /// <summary>
        /// This is called when screen starts being on top of the stack
        /// </summary>
        protected virtual void Resume()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> Kvp in MLocalCommands)
            {
                try
                {
                    Comand.Add(Kvp.Key, Kvp.Value);
                }
                catch
                {
                }
            }
            ScreenChange();
        }

        /// <summary>
        /// Called when Screen changes
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        protected void ScreenChange()
        {
            GenerateFooter();
            GenerateHeader();
            Flush();
        }

        protected virtual void GenerateFooter()
        {
            string Mid = $"Shown lines from {ActiveFrom + 1} to {ActiveTo} out of {VirtualConsoleTop}";
            GenerateFooter(Mid);
        }

        protected void GenerateFooter(string Mid)
        {
            FooterString = $"{new string(' ', Max(0, TrueWidth / 2 - Mid.Length / 2))}{Mid}";
            FooterHeight = 1;
        }

        protected virtual void GenerateHeader()
        {
            HeadString = $"{new string(' ', Max(0, (TrueWidth - Name.Length) / 2))}{Name}";
        }

        public bool ParseCommand(Comands Command, ConsoleKeyInfo KeyInfo)
        {
            switch (InputMode)
            {
                case InputMode.ControlOnly:
                    try
                    {
                        Comand[Command](CommandControls.InvokedBaseCommand[Command]);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        EnqueMessage(ex);
                        return false;
                    }

                case InputMode.ControlFirst:
                    try
                    {
                        Comand[Command](CommandControls.InvokedBaseCommand[Command]);
                        return true;
                    }
                    catch
                    {
                        if (UIComponents.ParseCommand(KeyInfo))
                        {
                            if (this.Name == Active.Peek().Name)
                                ScreenChange();
                            return true;
                        }
                        return false;
                    }

                case InputMode.InputOnly:
                    if (UIComponents.ParseCommand(KeyInfo))
                    {
                        if (this.Name == Active.Peek().Name)
                            ScreenChange();
                        return true;
                    }
                    return false;

                case InputMode.InputFirst:
                    if (!UIComponents.ParseCommand(KeyInfo))
                        try
                        {
                            Comand[Command](CommandControls.InvokedBaseCommand[Command]);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            EnqueMessage(ex);
                            return false;
                        }
                    else if (this.Name == Active.Peek().Name)
                        ScreenChange();
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Use this for debugging. When you enqueue message, new message will be added in message Q and message list
        /// </summary>
        /// <param Name="message">this will be enqueued</param>
        public static void EnqueMessage(object Message)
        {
            var DbMsg = new DebugMessage(Message.ToString());
            UnreadMessages.Enqueue(DbMsg);
            AllMessages.Add(DbMsg);
        }

        /// <summary>
        /// Add text to the virtual console
        /// </summary>
        /// <param Name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAdd(object Obj)
        {
            string s = Obj.ToString();
            var CurLine = mBodyStringLines[mCursorTop];
            var EndOfObj = Min(s.Length + mCursorLeft, TrueWidth - mCursorLeft);
            var EndPart = ((CurLine.Length < EndOfObj) ? (string.Empty) : (CurLine.Substring(EndOfObj)));
            var MidPart = s;
            var BeginPart = CurLine.Substring(0, mCursorLeft);

            mBodyStringLines[mCursorTop] = $"{BeginPart}{MidPart}{EndPart}";
            mCursorLeft = 0;
        }

        /// <summary>
        /// Add text to the virtual console and add new line to the console
        /// </summary>
        /// <param Name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAddLine(object Obj)
        {
            VirtualConsoleAdd(Obj);
            VirtualConsoleAddLine();
        }

        /// <summary>
        /// Only add new line to the virtual console
        /// </summary>
        public void VirtualConsoleAddLine()
        {
            if (mCursorTop + 1 == mBodyStringLines.Count)
                mBodyStringLines.Add(string.Empty);
            mCursorLeft = 0;
            mCursorTop++;
        }

        /// <summary>
        /// Print line full of '-' character. Use this to make screen look nicer.
        /// </summary>
        public void PrintLine()
        {
            mCursorLeft = 0;
            VirtualConsoleAddLine(new string('-', TrueWidth));
        }

        /// <summary>
        /// Prints text in the middle of the line
        /// </summary>
        /// <param Name="obj"></param>
        public void PrintCenter(object Obj)
        {
            var s = Obj.ToString();
            mCursorLeft = (TrueWidth - s.Length) / 2;
            VirtualConsoleAddLine(s);
        }

        /// <summary>
        /// Use this when you made changes to the virtual console
        /// </summary>
        public void Flush()
        {
            if (mObjectToLockFlush == null)
                mObjectToLockFlush = new object();
            lock (mObjectToLockFlush)
            {
                DrawUIElements();

                string ThreadSafeShit = VirtualConsole;
                CurrentStateOfTheScreen = new ScreenChangedArgs(ThreadSafeShit, mBodyString);
                ScreenChanged?.Invoke(this, CurrentStateOfTheScreen);
                Clear();
            }
        }

        /// <summary>
        /// Delete everything in the virtual consoled
        /// </summary>
        public void Clear()
        {
            mBodyString = string.Empty;
            mBodyStringLines = new List<string>() { string.Empty };
            mCursorLeft = 0;
            mCursorTop = 0;
        }
    }
}