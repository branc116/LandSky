using System;
using System.Threading;

namespace MultyNetHack
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine e = new Engine(50,20);            
            Thread.Sleep(int.MaxValue - 2);
        }
    }
}
