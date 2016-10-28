using LandSky.Components;
using LandSky.DotNetExt;
using LandSky.Screen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LandSky.Game.Cns
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter Seed");
            var seed = Console.ReadLine();
            var Map = new SandboxMap(0, 0);
            var Engine = new Engine("MyEngine");
            var Me = new Player("Branimir");
            var InfPlain = new InfinitePlane(seed, "MyPlayn");

            Engine.Connect("localhost:52062");
            Engine.PushNewScreenOnTop(Map);
            Engine.PushNewComponentOnActiveScreen(Me);
            Engine.PushNewComponentOnActiveScreen(InfPlain);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var a = System.Console.ReadKey(true);
                    Engine.InputNextCommand(new MyConsoleKeyInfo(a.KeyChar), "Branimir");
                }
            });

            try
            {
                Console.CursorVisible = false;
            }
            catch { }
            Console.Clear();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Engine.SetActiveComponent(Me, Console.WindowWidth - 10, Console.WindowHeight - 10);

                    var threadSafely = Engine.RenderAroundComponent();
                    System.Console.Clear();
                    System.Console.Write(threadSafely);
                    Thread.Sleep(100);
                }
            });
            Thread.Sleep(int.MaxValue);
        }
    }
}