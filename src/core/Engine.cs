using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Convert;
using static System.Math;

using LandSky.Screen;
using LandSky.DebugItems;
using LandSky.MyMath;

namespace LandSky
{
    /// <summary>
    /// This starts everything
    /// </summary>
    class Engine
    {
        private Size mSceenSize;
        /// <summary>
        /// Plays A major
        /// </summary>
        /// <param Name="tempo">Amount of time spend on each note (in ms)</param>
        public void PlayMajor(int Tempo)
        {
            int Dur = Tempo;
            Console.Beep(ToInt32(440 * Pow(2, (double)12 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)24 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), Dur);
            Console.Beep(ToInt32(440 * Pow(2, 12 / 12)), Dur);
        }

        private void InitConsole(int Width, int Height)
        {
            Console.SetWindowSize(Width, Height);
            try
            {
                Console.CursorVisible = false;
            }
            catch
            {
                // ignored
            }
            PlayMajor(2);
            
        }
        private void InitProperties(int Width,int Height)
        {
            mSceenSize = new Size(Width, Height);
            BaseScreen.Active = new Stack<BaseScreen>();
            
            BaseScreen.AllMessages = new List<DebugMessage>();
            BaseScreen.UnreadMessages = new Queue<DebugMessage>();
        }
        /// <summary>
        /// Handels console input
        /// </summary>
        /// <returns></returns>
        private Task Initinput()
        {
            var C = Console.ReadKey(true);
            C = new ConsoleKeyInfo(C.KeyChar, C.Key, false, (ConsoleModifiers.Alt & C.Modifiers) != 0, 0 != (ConsoleModifiers.Control & C.Modifiers));
            while (true)
            {
                try
                {

                    var Cc = Controls.KeyMap[C];
                    var Ss = BaseScreen.Active.Peek();

                    try
                    {
                        Ss.Comand[Cc](Controls.InvokedBaseCommand[Cc]);
                    }
                    catch(Exception Ex)
                    {
                        throw;
                        if (Ex != null)
                            Ex = null;
                    }
                    

                }
                catch(Exception e) {
                    BaseScreen.EnqueMessage(e);
                    Console.WriteLine(e);
                    if (e!=null) e = null;
                }
#if DEBUG
                Console.WriteLine($"{(string)(((C.Modifiers & ConsoleModifiers.Control) != 0) ? "ctrl +" : string.Empty )} {(string)(((C.Modifiers & ConsoleModifiers.Alt) != 0) ? "alt +" : string.Empty)} {C.KeyChar.ToString()} - { C.Key.ToString()} ");
#endif
                C = Console.ReadKey(true);
                C = new ConsoleKeyInfo(C.KeyChar, C.Key, false, (ConsoleModifiers.Alt & C.Modifiers) != 0, 0 != (ConsoleModifiers.Control & C.Modifiers));
            }

        }
        /// <summary>
        /// Create new engine with main menu as a starting screen
        /// </summary>
        /// <param Name="width">Width of the Screens in the global console</param>
        /// <param Name="height">Height of the Screens in the global console</param>
        public Engine(int Width, int Height)
        {
            InitConsole(Width, Height);
            InitProperties(Width, Height);
            BaseScreen.Active.Push(new MainMenuScreen(0, 0));
            Task T = Initinput();
            
        }
    }
}
