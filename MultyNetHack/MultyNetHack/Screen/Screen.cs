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
                return TrueHeight - HeadHeight - FooterHight;
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
                return Min(MaxHeight, Height);
            }
        }
        /// <summary>
        /// Minimum value of wanted Height and maximum Width
        /// </summary>
        public int TrueWidth
        {
            get
            {
                return Min(MaxWidth, Width)-1;
            }
        }
        /// <summary>
        /// Wanted Height of the screen
        /// </summary>
        public int WantedHeight
        {
            set
            {
                Height = value;
                Change?.Invoke(null, EventArgs.Empty);
            }
            get
            {
                return Height;
            }
        } 
        /// <summary>
        /// Wanted Width of the screen
        /// </summary>
        public int WantedWidth
        {
            set
            {
                Width = value;
                Change?.Invoke(null, EventArgs.Empty);
            }
            get
            {
                return Width;
            }
        }
        /// <summary>
        /// Calculate current position of the cursure in the virtual console
        /// </summary>
        public int VirtualConsoleLeft
        {
            get
            {
                for (int i = BodyString.Length - 1; i >= 0; i--)
                {
                    if (BodyString[i] == '\n')
                        return BodyString.Length - i - 1;
                }
                return BodyString.Length;
            }
            set
            {
                int i = BodyString.Length - 1;
                for (; i >= 0; i--)
                {
                    if (BodyString[i] == '\n')
                        break;
                }
                if (i + value <= BodyString.Length && BodyString.Length != 0)
                    BodyString = BodyString.Remove(i + value);
                else if (BodyString.Length != 0)
                    BodyString += new string(' ', i + value - BodyString.Length);
            }
        }
        /// <summary>
        /// Calculate number of lines the body part of the screen has
        /// </summary>
        public int VirtualConsoleTop
        {
            get
            {
                return BodyString.Where(i => i == '\n').ToList().Count;
            }
            set
            {
                if (VirtualConsoleTop < value)
                {
                    BodyString += new string('\n', value - VirtualConsoleTop);
                }
                else if (VirtualConsoleTop > value)
                {
                    int i = 0;
                    for (; i < BodyString.Length; i++)
                    {
                        if (BodyString[i] == '\n')
                        {
                            value--;
                            if (value == 0)
                                break;
                        }

                    }

                    BodyString = BodyString.Remove(i + 1);
                }
                else
                {
                    BodyString = BodyString.Remove(BodyString.LastIndexOf('\n') + 1);
                }
            }

        }
        /// <summary>
        /// Current text in the virtual console
        /// </summary>
        public string VirtualConsole
        {
            get
            {
                if (shuldUpdate)
                {
                    string s = '+' + new string('-', TrueWidth) + '+' + '\n';
                    HeadString.Split('\n').ToList().ForEach((i) =>
                    {
                        s += '|' + i.Substring(0, Min(i.Length, TrueWidth)) + new string(' ', Max(0, TrueWidth - i.Length)) + '|' +'\n';
                    });
                    s += '|' + new string('-', TrueWidth) + '|' + '\n';
                    List<string> ss = BodyString.Split('\n').ToList();
                    for (int j = activeFrom; j < activeTo; j++)
                    {
                        string i = ss[j];
                        s += '|' + i.Substring(0, Min(i.Length, TrueWidth)) + new string(' ', Max(0, TrueWidth - i.Length)) + '|' + '\n';
                    }
                    s += '|' + new string('-', TrueWidth) + '|' + '\n';
                    FooterString.Split('\n').ToList().ForEach((i) =>
                    {
                        s += '|' + i.Substring(0, Min(i.Length, TrueWidth)) + new string(' ', Max(0, TrueWidth - i.Length)) + '|' + '\n';
                    });
                    s += '+' + new string('-', TrueWidth) + '+';
                    _virtualConsole = s;
                    shuldUpdate = false;
                }
                return _virtualConsole;
            }
        }
        /// <summary>
        /// Position of the body part
        /// </summary>
        public int Scrool
        {
            get
            {
                return mScrool;
            }set
            {
                mScrool = value % VirtualConsoleTop;
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
        protected Dictionary<Comands, Action<BaseCommand>> mLocalCommands;
        /// <summary>
        /// Determent what line of virtual console should be printed out first 
        /// </summary>
        protected int activeFrom
        {
            get
            {
                return Scrool;
            }
        }
        /// <summary>
        /// Determent what line of virtual console should be printed out last
        /// </summary>
        protected int activeTo
        {
            get
            {
                return Min(VirtualConsoleTop, Min(Scrool + VirtualConsoleTop,Scrool + WantedHeight - HeadHeight*2));
            }
        }
        protected int mScrool, GlobalLeft, GlobalTop, HeadHeight, FooterHight;
        /// <summary>
        /// Invoke this when something changes
        /// </summary>
        protected event EventHandler Change;
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
        private bool shuldUpdate;
        private string _virtualConsole;
        /// <summary>
        /// This is displayed in the middle of the screen
        /// </summary>
        private string BodyString;


        /// <summary>
        /// Create new base screen. One shouldn't relay do this. One should extend new method and then call the constructor for that new method.
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
            Change += Screen_Change;
        }
        /// <summary>
        /// Create new base screen. One shouldn't relay do this. One should extend new method and then call the constructor for that new method. Name will be "UnNamed"
        /// </summary>
        /// <param Name="Top">Global position in the global console </param>
        /// <param Name="Left">Global position in the global console </param>
        public BaseScreen(int Top, int Left) : base("UnNamed")
        {
            InitProperties(Top, Left);
            InitComands();
            GenerateHeader();
            GenerateFooter();
            Change += Screen_Change;
        }
        /// <summary>
        /// This is called when creating new BaseScreen to initiate global properties
        /// </summary>
        /// <param Name="Top"></param>
        /// <param Name="Left"></param>
        private void InitProperties(int Top, int Left)
        {
            GlobalLeft = Left;
            GlobalTop = Top;
            WantedWidth = MaxWidth;
            WantedHeight = 30;
            BodyString = string.Empty;
            shuldUpdate = true;
            Comand = new Dictionary<Comands, Action<BaseCommand>>();
            

            mLocalCommands = new Dictionary<Comands, Action<BaseCommand>>();
            
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
        private void PopSceen(BaseCommand bc)
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
        private void ShowDebug(BaseCommand bc)
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
        private void ShowHelp(BaseCommand bc)
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
        private void ScrollCommand(BaseCommand bc)
        {
            ScrollCommand scroll = bc as ScrollCommand;
            Scrool += scroll.n;
            if (VirtualConsoleTop >0)
                Scrool %= VirtualConsoleTop;
            if (Scrool < 0)
                Scrool = VirtualConsoleTop;

            Change?.Invoke(null, EventArgs.Empty);
        }
        /// <summary>
        /// Needs to be implemented
        /// </summary>
        /// <param Name="bc"></param>
        private void ChangeScene(BaseCommand bc)
        {

        }
        /// <summary>
        /// this is called when the screen stops being on top of the stack
        /// </summary>
        virtual protected void Pause()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> mKVP in mLocalCommands)
            {
                Comand.Remove(mKVP.Key);
            }
            Console.Clear();
        }
        /// <summary>
        /// This is called when screen starts being on top of the stack
        /// </summary>
        virtual protected void Resume()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> mKVP in mLocalCommands)
            {
                try
                {
                    Comand.Add(mKVP.Key, mKVP.Value);
                }
                catch
                {

                }
            }
            Screen_Change(null, EventArgs.Empty);
        }
        /// <summary>
        /// Called when Screen changes
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        protected void Screen_Change(object sender, EventArgs e)
        {
            GenerateFooter();
            GenerateHeader();
            Flush();
        }
        
        virtual protected void GenerateFooter()
        {
            string mid = string.Format("Shown lines from {0} to {1} out of {2}", activeFrom + 1, activeTo, VirtualConsoleTop);
            GenerateFooter(mid);
            
        }
        protected void GenerateFooter(string Mid)
        {
            FooterString = string.Format("{0}{1}", new string(' ', Max(0, TrueWidth / 2 - Mid.Length / 2)), Mid);
            FooterHight = 1;
        }
        virtual protected void GenerateHeader()
        {
            HeadString = string.Format("{0}{1}", new string(' ', Max(0, (TrueWidth - Name.Length) / 2)), Name);
        }
        /// <summary>
        /// Use this for debugging. When you enqueue message, new message will be added in message Q and message list
        /// </summary>
        /// <param Name="message">this will be enqueued</param>
        public static void EnqueMessage(object message)
        {
            var mDbMsg = new DebugMessage(message.ToString());
            UnreadMessages.Enqueue(mDbMsg);
            AllMessages.Add(mDbMsg);
        }
        /// <summary>
        /// Add text to the virtual console
        /// </summary>
        /// <param Name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAdd(object obj)
        {
            BodyString += obj.ToString();
        }
        /// <summary>
        /// Add text to the virtual console and add new line to the console
        /// </summary>
        /// <param Name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAddLine(object obj)
        {
            VirtualConsoleAdd(obj);
            BodyString += '\n';
        }
        /// <summary>
        /// Only add new line to the virtual console
        /// </summary>
        public void VirtualConsoleAddLine()
        {
            BodyString += '\n';
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
        public void PrintCenter(object obj)
        {
            string s = obj.ToString() + '\n';
            VirtualConsoleLeft = 0;
            BodyString += new string(' ', TrueWidth / 2 - s.Length / 2) + s + '\n';   
        }
        bool inside = false;
        /// <summary>
        /// Use this when you made changes to the virtual console
        /// </summary>
        public void Flush()
        {
            if (!inside)
            {
                inside = true;
                Console.CursorTop = GlobalTop;
                Console.CursorLeft = GlobalLeft;
                shuldUpdate = true;
                string ThreadSafeShit = VirtualConsole;
                Console.Clear();
                Console.WriteLine(ThreadSafeShit);
                inside = false;
            }
        }
        /// <summary>
        /// Delete everything in the virtual consoled
        /// </summary>
        public void Clear()
        {
            BodyString = string.Empty;
        }
    }
}
