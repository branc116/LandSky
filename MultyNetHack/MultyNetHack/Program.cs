using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace MultyNetHack
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine e = new Engine(70,20);
            Thread.Sleep(int.MaxValue - 2);
        }
    }

}
