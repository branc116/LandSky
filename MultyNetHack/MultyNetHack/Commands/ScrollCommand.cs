using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack.Commands
{
    
    class ScrollCommand : BaseCommand
    {
        public int n;
        public ScrollCommand(int n)
        {
            this.n = n;
        }
        public static ScrollCommand Left()
        {
            return new ScrollCommand(-1);
        }
        public static ScrollCommand Right()
        {
            return new ScrollCommand(1);
        }
        public static ScrollCommand LeftTen()
        {
            return new ScrollCommand(-10);
        }
        public static ScrollCommand RightTen()
        {
            return new ScrollCommand(10);
        }
    }
}
