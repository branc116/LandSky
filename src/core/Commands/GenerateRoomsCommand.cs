using System.Threading;

namespace LandSky.Commands
{
    /// <summary>
    /// Command for generating rooms
    /// </summary>
    class GenerateRoomsCommand : BaseCommand
    {

        public int NumberOfRooms;
        public static CancellationToken CancelGenerting;
        private static CancellationTokenSource mMSource;

        public GenerateRoomsCommand(int N)
        {
            NumberOfRooms = N;
            mMSource?.Cancel();
            mMSource = new CancellationTokenSource();
            CancelGenerting = mMSource.Token;
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
            mMSource?.Cancel();  
        }

    }
}
