using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack;
namespace MultyNetHack.Screen
{
    /// <summary>
    /// Help screen (need to be implemented)
    /// </summary>
    public class HelpScreen :BaseScreen    
    {
        public HelpScreen(int Top, int Left) : base(Top, Left, "Help")
        {
            //bool enter = false;
            //foreach (var c in Command)
            //{

            //    char val = Controls.KeyMap.Where((i) => { return i.Value == c.Key; }).ToList()[0].Key;
                
            //    if (enter)
            //        Left = TrueWidth / 2;
            //    VirtualConsoleAddLine(string.Format("'{0}' -> {1}", valc.Key, ));
            //    if (enter)
            //        VirtualConsoleAddLine();
            //    enter = !enter;
            //}
        }
    }
} 
