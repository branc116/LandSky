using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack;
using MultyNetHack.Commands;
using MultyNetHack.DebugItems;
using static System.Math;

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
        /// Hight of the middle part of the screen. Calculated by subtracting height of header and footer from current height.
        /// </summary>
        public int BodyHeight
        {
            get
            {
                return TrueHeight - HeadHeight - FooterHight;
            }
        }
        /// <summary>
        /// Maximum height of screen 
        /// </summary>
        public int MaxHeight
        {
            get
            {
                return Console.WindowHeight - GlobalTop;
            }
        }
        /// <summary>
        /// Maximum width of screen
        /// </summary>
        public int MaxWidth
        {
            get
            {
                return Console.WindowWidth - GlobalLeft - 2;
            }
        }
        /// <summary>
        /// Minimum value of wanted height and maximum height
        /// </summary>
        public int TrueHeight
        {
            get
            {
                return Min(MaxHeight, Height);
            }
        }
        /// <summary>
        /// Minimum value of wanted height and maximum width
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
                else if (VirtualConsole.Length != 0)
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
        /// List of all debug messages. This is used for debuging.
        /// </summary>
        public static List<DebugMessage> AllMessages;
        /// <summary>
        /// Comands in screens that extend Screen methode.
        /// </summary>
        protected Dictionary<Comands, Action<BaseCommand>> mLocalCommands;
        /// <summary>
        /// Determen what line of virtual console shuld be printed out first 
        /// </summary>
        protected int activeFrom
        {
            get
            {
                return Scrool;
            }
        }
        /// <summary>
        /// Determen what line of virtual console shuld be printed out last
        /// </summary>
        protected int activeTo
        {
            get
            {
                return Min(Scrool + BodyHeight, VirtualConsoleTop);
            }
        }
        protected int mScrool, GlobalLeft, GlobalTop, HeadHeight, FooterHight, Height, Width;
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
        /// Indicates that virtual console shuld be redrawn
        /// </summary>
        private bool shuldUpdate;
        private string _virtualConsole;
        /// <summary>
        /// This is displayed in the middle of the screen
        /// </summary>
        private string BodyString;


        /// <summary>
        /// Create new base screen. One shuldn't realy do this. One shuld extend new methode and then call the constructor for that new methode.
        /// </summary>
        /// <param name="Top">Global position in the global console </param>
        /// <param name="Left">Global position in the global console </param>
        /// <param name="Name">Name of the Screen. It will probably be displayed in the title.</param>
        public BaseScreen(int Top, int Left, string Name) : base(Name)
        {
            InitProperties(Top, Left);
            InitComands();
            GenerateHeader();
            GenerateFooter();
            Change += Screen_Change;
        }
        /// <summary>
        /// Create new base screen. One shuldn't realy do this. One shuld extend new methode and then call the constructor for that new methode. Name will be "UnNamed"
        /// </summary>
        /// <param name="Top">Global position in the global console </param>
        /// <param name="Left">Global position in the global console </param>
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
        /// <param name="Top"></param>
        /// <param name="Left"></param>
        private void InitProperties(int Top, int Left)
        {
            GlobalLeft = Left;
            GlobalTop = Top;
            WantedWidth = MaxWidth;
            WantedHeight = WantedWidth;
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
        /// <param name="bc"></param>
        private void PopSceen(BaseCommand bc)
        {
            this.Pause();
            Active.Pop();
            if (Active.Count == 0)
                Environment.Exit(0);   
        }
        /// <summary>
        /// Show the debug screen on top of current screen
        /// </summary>
        /// <param name="bc"></param>
        private void ShowDebug(BaseCommand bc)
        {
            if (this.GetType() != typeof(HelpScreen) && this.GetType() != typeof(DebugScreen))
            {
                Active.Push(new DebugScreen(GlobalTop, GlobalLeft));
            }
        }
        /// <summary>
        /// Show help creen on top of current screen
        /// </summary>
        /// <param name="bc"></param>
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
        /// <param name="bc">Shuld be typeof(ScrollCommand) </param>
        private void ScrollCommand(BaseCommand bc)
        {
            ScrollCommand scroll = bc as ScrollCommand;
            Scrool += scroll.n;

            Change?.Invoke(null, EventArgs.Empty);
        }
        /// <summary>
        /// Needs to be implemented
        /// </summary>
        /// <param name="bc"></param>
        private void ChangeScene(BaseCommand bc)
        {

        }
        /// <summary>
        /// this is called when the screen stopps beeing on top of the stack
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
        /// This is called when screen starts beeing on top of the stack
        /// </summary>
        virtual protected void Resume()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> mKVP in mLocalCommands)
            {
                Comand.Add(mKVP.Key, mKVP.Value);
            }
            Flush();    
        }
        /// <summary>
        /// Called when Screen changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Screen_Change(object sender, EventArgs e)
        {
            GenerateFooter();
            GenerateHeader();
            shuldUpdate = true;
            Flush();
        }
        
        private void GenerateFooter()
        {
            string mid = string.Format("Shown lines from {0} to {1} out of {2}", activeFrom, activeTo, VirtualConsoleTop);
            FooterString = string.Format("{0}{1}", new string(' ', TrueWidth / 2 - mid.Length / 2), mid);
            FooterHight = 1;
        }
        private void GenerateHeader()
        {
            HeadString = string.Format("{0}{1}", new string(' ', (TrueWidth - name.Length) / 2), name);
        }
        /// <summary>
        /// Use this for debugging. When you enque message, new message will be added in message Q and message list
        /// </summary>
        /// <param name="message">this will be enqued</param>
        public static void EnqueMessage(object message)
        {
            var mDbMsg = new DebugMessage(message.ToString());
            UnreadMessages.Enqueue(mDbMsg);
            AllMessages.Add(mDbMsg);
        }
        /// <summary>
        /// Add text to the virtual console
        /// </summary>
        /// <param name="obj">stuff you want to print in virtual console</param>
        public void VirtualConsoleAdd(object obj)
        {
            BodyString += obj.ToString();
        }
        /// <summary>
        /// Add text to the virtual console and add new line to the console
        /// </summary>
        /// <param name="obj">stuff you want to print in virtual console</param>
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
        /// Print line full of '-' character. Use this to make screen look organised.
        /// </summary>
        public void PrintLine()
        {
            
            VirtualConsoleLeft = 0;
            VirtualConsoleAddLine(new string('-', TrueWidth));
        }
        /// <summary>
        /// Prints text in the middle of the line
        /// </summary>
        /// <param name="obj"></param>
        public void PrintCenter(object obj)
        {
            string s = obj.ToString();
            VirtualConsoleLeft = 0;
            BodyString += new string(' ', TrueWidth / 2 - s.Length / 2) + s + '\n';   
        }
        /// <summary>
        /// Use this when you made changes to the virtual console
        /// </summary>
        public void Flush()
        {
            Console.CursorTop = GlobalTop;
            Console.CursorLeft = GlobalLeft;
            shuldUpdate = true;
            Console.Clear();
            Console.WriteLine(VirtualConsole);
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
