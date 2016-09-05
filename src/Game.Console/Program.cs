using System;
using System.Threading.Tasks;

namespace Game.Cns
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter Seed");
            var seed = Console.ReadLine();
            var Map = new SandboxMap(0, 0);
            var Engine = new Engine("MyEngine");
            Engine.PushNewScreenOnTop(Map);
            var Me = new Player("Branimir");
            Engine.PushNewComponentOnActiveScreen(Me);
            var InfPlain = new InfinitePlane(seed, "MyPlayn");
            Engine.PushNewComponentOnActiveScreen(InfPlain);
            Engine.RenderAroundComponent(Me, 2, 2);
            Task.Factory.StartNew(async () =>
            {
                var Render = new Render();
                while (true)
                {
                    try
                    {
                        Console.CursorVisible = false;
                    }
                    catch { }
                    Console.Clear();
                    try
                    {
                        var threadSafely = Render.RenderSandBoxMap(Engine.AllComponentsOnScreen(), Me, new Size(Console.WindowWidth - 10, Console.WindowHeight - 10));
                        System.Console.Clear();
                        System.Console.WriteLine(threadSafely);
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
            while (true)
            {
                var a = System.Console.ReadKey(true);
                Engine.InputNextCommand(new MyConsoleKeyInfo(a.KeyChar));
            }
        }
    }
}