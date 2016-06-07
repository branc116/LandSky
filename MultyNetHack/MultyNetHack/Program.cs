using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace MultyNetHack
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine e = new Engine(40,40);
            
            Thread.Sleep(int.MaxValue - 2);
        }
    }
}
