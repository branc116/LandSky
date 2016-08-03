using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace MultyNetHack
{
    class Program
    {
        /// <summary>
        /// Start of the program
        /// </summary>
        /// <param Name="args"></param>
        static void Main(string[] args)
        {
            new Engine(180,20);
        } 
    }

}
