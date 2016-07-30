using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace MultyNetHack.Commands
{

    class GenerateRoomsCommand : BaseCommand
    {

        public int numberOfRooms;
        static public CancellationToken CancelGenerting;
        static private CancellationTokenSource mSource;
        public GenerateRoomsCommand(int n)
        {
            numberOfRooms = n;
            mSource?.Cancel();
            mSource = new CancellationTokenSource();
            CancelGenerting = mSource.Token;
        }
        public static GenerateRoomsCommand GenerateAlotOfRooms()
        {
            return new GenerateRoomsCommand(1000);
        }
        public static GenerateRoomsCommand GenerateOneRoom()
        {
            return new GenerateRoomsCommand(1);
        }
        public static void CancleGeneratingRooms()
        {
            mSource?.Cancel();  
        }

    }
}
