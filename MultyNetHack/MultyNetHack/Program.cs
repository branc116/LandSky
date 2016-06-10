using System;
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
