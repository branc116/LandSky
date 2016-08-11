using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

using MultyNetHack.Commands;
using MultyNetHack.Components;
using MultyNetHack.DebugItems;
using MultyNetHack.MyEnums;

/// <summary>
/// All of default screens are in this namespace
/// </summary>
namespace MultyNetHack.Screen
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
        /// Maximum Height of screen 
        /// </summary>
        public int MaxHeight
        {
            get
            {
                return Console.WindowHeight - GlobalTop;
            }
        }
        /// <summary>
        /// Maximum Width of screen
        /// </summary>
        public int MaxWidth
        {
            get
            {
                return Console.WindowWidth - GlobalLeft - 2;
            }
        }
        /// <summary>
        /// Minimum value of wanted Height and maximum Height
        /// </summary>
        public int TrueHeight
        {
            get
            {
                return Min(MaxHeight, MScreenHeight);
            }
        }
        /// <summary>
        /// Minimum value of wanted Height and maximum Width
        /// </summary>
        public int TrueWidth
        {
            get
            {
                return Min(MaxWidth, MScreenWidth)-1;
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
                for (int I = mBodyString.Length - 1; I >= 0; I--)
                {
                    if (mBodyString[I] == '\n')
                        return mBodyString.Length - I - 1;
                }
                return mBodyString.Length;
            }
            set
            {
                int I = mBodyString.Length - 1;
                for (; I >= 0; I--)
                {
                    if (mBodyString[I] == '\n')
                        break;
                }
                if (I + value <= mBodyString.Length && mBodyString.Length != 0)
                    mBodyString = mBodyString.Remove(I + value);
                else if (mBodyString.Length != 0)
                    mBodyString += new string(' ', I + value - mBodyString.Length);
                mIsBodyUpdated = true;
            }
        }
        /// <summary>
        /// Calculate number of lines the body part of the screen has
        /// </summary>
        public int VirtualConsoleTop
        {
            get
            {
                if (mIsBodyUpdated) {
                    mVirtualConsoleTop = mBodyString.Count(mI => mI == '\n');
                    mIsBodyUpdated = false;
                }
                return mVirtualConsoleTop;
            }
            set
            {
                if (VirtualConsoleTop < value)
                {
                    mBodyString += new string('\n', value - VirtualConsoleTop);
                }
                else if (VirtualConsoleTop > value)
                {
                    int I = 0;
                    for (; I < mBodyString.Length; I++)
                    {
                        if (mBodyString[I] == '\n')
                        {
                            value--;
                            if (value == 0)
                                break;
                        }

                    }

                    mBodyString = mBodyString.Remove(I + 1);
                }
                else
                {
                    mBodyString = mBodyString.Remove(mBodyString.LastIndexOf('\n') + 1);
                }
                mIsBodyUpdated = true;
            }

        }
        /// <summary>
        /// Current text in the virtual console
        /// </summary>
        public string VirtualConsole
        {
            get
            {
                if (mShuldUpdate)
                {
                    int mActiveFrom = ActiveFrom;
                    mActiveTo = ActiveTo;
                    int mTrueWidth = TrueWidth;
                    int mTrueHeight = TrueHeight;

                    string S = '+' + new string('-', mTrueWidth) + '+' + '\n';
                    HeadString.Split('\n').ToList().ForEach((I) =>
                    {
                        S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' +'\n';
                    });
                    S += '|' + new string('-', mTrueWidth) + '|' + '\n';
                    int CountingEn = -1;
                    var ActiveBodyLines = mBodyString.Split('\n').Where(i =>
                    {
                        CountingEn++;
                        return CountingEn >= mActiveFrom && CountingEn < mActiveTo;
                    });
                    foreach (var ActiveBodyLine in ActiveBodyLines) {
                        string I = ActiveBodyLine;
                        S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' + '\n';
                        if (CountingEn > mActiveTo)
                            break;
                    }
                    S += '|' + new string('-', mTrueWidth) + '|' + '\n';
                    FooterString.Split('\n').ToList().ForEach((I) =>
                    {
                        S += '|' + I.Substring(0, Min(I.Length, mTrueWidth)) + new string(' ', Max(0, mTrueWidth - I.Length)) + '|' + '\n';
                    });
                    S += '+' + new string('-', mTrueWidth) + '+';
                    mVirtualConsole = S;
                    mShuldUpdate = false;
                }
                return mVirtualConsole;
            }
        }
        /// <summary>
        /// Position of the body part
        /// </summary>
        public int Scrool
        {
            get
            {
                return MScrool;
            }set
            {
                MScrool = value % VirtualConsoleTop;
            }
        }
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
        public static Queue<DebugMessage> UnreadMessages;
        /// <summary>
        /// List of all debug messages. This is used for debugging.
        /// </summary>
        public static List<DebugMessage> AllMessages;

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

        /// <summary>
        /// Indicates that virtual console should be redrawn
        /// </summary>
        private bool mShuldUpdate, mIsBodyUpdated;
        private string mVirtualConsole;
        /// <summary>
        /// This is displayed in the middle of the screen
        /// </summary>
        private string mBodyString;
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
            mIsBodyUpdated = true;
            GlobalLeft = Left;
            GlobalTop = Top;
            WantedWidth = MaxWidth;
            WantedHeight = 30;
            mShuldUpdate = true;
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
            if (VirtualConsoleTop >0)
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
        /// <summary>
        /// this is called when the screen stops being on top of the stack
        /// </summary>
        protected virtual void Pause()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> Kvp in MLocalCommands)
            {
                Comand.Remove(Kvp.Key);
            }
            Console.Clear();
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
            FooterString = $"{new string(' ', Max(0, TrueWidth/2 - Mid.Length/2))}{Mid}";
            FooterHeight = 1;
        }
        protected virtual void GenerateHeader()
        {
            HeadString = $"{new string(' ', Max(0, (TrueWidth - Name.Length)/2))}{Name}";
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
            mBodyString += Obj.ToString();
            mIsBodyUpdated = true;
        }
        /// <summary>
        /// Add text to the virtual console and add new line to the console
        /// </summary>
        /// <param Name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAddLine(object Obj)
        {
            VirtualConsoleAdd(Obj);
            mBodyString += '\n';
            mIsBodyUpdated = true;
        }
        /// <summary>
        /// Only add new line to the virtual console
        /// </summary>
        public void VirtualConsoleAddLine()
        {
            mBodyString += '\n';
            mIsBodyUpdated = true;
        }
        /// <summary>
        /// Print line full of '-' character. Use this to make screen look nicer.
        /// </summary>
        public void PrintLine()
        {
            
            VirtualConsoleLeft = 0;
            VirtualConsoleAddLine(new string('-', TrueWidth)  + '\n');
        }
        /// <summary>
        /// Prints text in the middle of the line
        /// </summary>
        /// <param Name="obj"></param>
        public void PrintCenter(object Obj)
        {
            string S = Obj.ToString() + '\n';
            VirtualConsoleLeft = 0;
            mBodyString += new string(' ', TrueWidth / 2 - S.Length / 2) + S + '\n';
            mIsBodyUpdated = true;
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
                Console.CursorTop = GlobalTop;
                Console.CursorLeft = GlobalLeft;
                mShuldUpdate = true;
                string ThreadSafeShit = VirtualConsole;
                Console.Clear();
                Console.WriteLine(ThreadSafeShit);
            }
        }
        /// <summary>
        /// Delete everything in the virtual consoled
        /// </summary>
        public void Clear()
        {
            mBodyString = string.Empty;
            mIsBodyUpdated = true;
        }
    }
}
