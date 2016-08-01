using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using MultyNetHack.Screen;
using MultyNetHack.DebugItems;
using static System.Convert;
using static System.Math;
namespace MultyNetHack
{
    /// <summary>
    /// This starts everything
    /// </summary>
    class Engine
    {
        Size SceenSize;
        /// <summary>
        /// Plays A major
        /// </summary>
        /// <param Name="tempo">Amount of time spend on each note (in ms)</param>
        public void PlayMajor(int tempo)
        {
            
            int dur = tempo;
            Console.Beep(ToInt32(440 * Pow(2, (double)12 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)24 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, 12 / 12)), dur);
        }

        private void InitConsole(int width, int height)
        {
            Console.WindowWidth = width + 1;
            Console.BufferWidth = width + 1;
            Console.WindowHeight = height * 3;
            try
            {
                Console.CursorVisible = false;
            }
            catch { }
            PlayMajor(2);
            
        }
        private void InitProperties(int width,int height)
        {
            SceenSize = new Size(width, height);
            BaseScreen.Active = new Stack<BaseScreen>();
            
            BaseScreen.AllMessages = new List<DebugMessage>();
            BaseScreen.UnreadMessages = new Queue<DebugMessage>();

            Controls.LoadKeyMap();
            Controls.LoadInvokedBaseCommand();
        }
        /// <summary>
        /// Handels console input
        /// </summary>
        /// <returns></returns>
        private Task Initinput()
        {
            var c = Console.ReadKey(true);
            
            while (true)
            {
                try
                {
                    var cc = Controls.KeyMap[c.KeyChar];
                    var ss = BaseScreen.Active.Peek();
                    Task.Run(() =>
                    {
                        try
                        {
                            ss.Comand[cc](Controls.InvokedBaseCommand[cc]);
                        }
                        catch(Exception ex)
                        {
                            if (ex != null)
                                ex = null;
                        }
                    });
                }
                catch(Exception e) {
                    BaseScreen.EnqueMessage(e);
                    if (e!=null) e = null;
                }
                c = Console.ReadKey(true);
            }

        }
        /// <summary>
        /// Create new engine with main menu as a starting screen
        /// </summary>
        /// <param Name="width">Width of the Screens in the global console</param>
        /// <param Name="height">Height of the Screens in the global console</param>
        public Engine(int width, int height)
        {
            InitConsole(width, height);
            InitProperties(width, height);
            BaseScreen.Active.Push(new MainMenuScreen(0, 0));
            Task t = Initinput();
            
        }
    }
}
