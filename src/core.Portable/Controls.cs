using LandSky.Commands;
using LandSky.DotNetExt;
using LandSky.MyEnums;
using System.Collections.Generic;

namespace LandSky
{
    /// <summary>
    /// Contains keymaps, and commandmaps
    /// </summary>
    internal class CommandControls
    {
        public static Dictionary<MyConsoleKeyInfo, Comands> KeyMap = new Dictionary<MyConsoleKeyInfo, Comands>
            {
                {new MyConsoleKeyInfo('h') , Comands.Left},
                {new MyConsoleKeyInfo('H'), Comands.TenStepsLeft},
                {new MyConsoleKeyInfo('l'), Comands.Right},
                {new MyConsoleKeyInfo('L'), Comands.TenStepsRight},
                {new MyConsoleKeyInfo('k'), Comands.Up},
                {new MyConsoleKeyInfo('K'), Comands.TenStepsUp},
                {new MyConsoleKeyInfo('j'), Comands.Down},
                {new MyConsoleKeyInfo('J'), Comands.TenStepsDown},
                {new MyConsoleKeyInfo('q'), Comands.ScrollLeft},
                {new MyConsoleKeyInfo('e'), Comands.ScrollRight},
                {new MyConsoleKeyInfo(','), Comands.ScrollLeft},
                {new MyConsoleKeyInfo('.'), Comands.ScrollRight},
                {new MyConsoleKeyInfo('r'), Comands.GenerateOneRoom},
                {new MyConsoleKeyInfo('R'), Comands.GenerateALotOfRooms},
                {new MyConsoleKeyInfo('P'), Comands.GenerateRandomPath},
                {new MyConsoleKeyInfo('d'), Comands.DequeMessage},

                {new MyConsoleKeyInfo('s',true, false), Comands.ToJSON },
                {new MyConsoleKeyInfo((char)27), Comands.LastSceen},
                {new MyConsoleKeyInfo('?'), Comands.ShowHelp},
                {new MyConsoleKeyInfo('*'), Comands.ShowDebug},
                {new MyConsoleKeyInfo('1'), Comands.Option1},
                {new MyConsoleKeyInfo('2'), Comands.Option2},
                {new MyConsoleKeyInfo('3'), Comands.Option3},
                {new MyConsoleKeyInfo('4'), Comands.Option4},
                {new MyConsoleKeyInfo('5'), Comands.Option5},
                {new MyConsoleKeyInfo('6'), Comands.Option6},
                {new MyConsoleKeyInfo('7'), Comands.Option7},
                {new MyConsoleKeyInfo('8'), Comands.Option8},
                {new MyConsoleKeyInfo('9'), Comands.Option9}
            };

        public static Dictionary<Comands, BaseCommand> InvokedBaseCommand = new Dictionary<Comands, BaseCommand>
            {
                {Comands.GenerateOneRoom,     new GenerateRoomsCommand(1)},
                {Comands.GenerateALotOfRooms, new GenerateRoomsCommand(100)},
                {Comands.GenerateRandomPath,  new GeneratePathCommand() {NumberOfPaths = 1 } },
                {Comands.Left,                new MoveCommand(MoveDirection.Left, 1)},
                {Comands.TenStepsLeft,        new MoveCommand(MoveDirection.Left, 10)},
                {Comands.Right,               new MoveCommand(MoveDirection.Right, 1)},
                {Comands.TenStepsRight,       new MoveCommand(MoveDirection.Right, 10)},
                {Comands.Down,                new MoveCommand(MoveDirection.Down, 1)},
                {Comands.TenStepsDown,        new MoveCommand(MoveDirection.Down, 10)},
                {Comands.Up,                  new MoveCommand(MoveDirection.Up, 1)},
                {Comands.TenStepsUp,          new MoveCommand(MoveDirection.Up, 10)},
                {Comands.ScrollRight,         new ScrollCommand(1)},
                {Comands.ScrollLeft,          new ScrollCommand(-1)},
                {Comands.ShowHelp,            new BaseCommand()},
                {Comands.ShowDebug,           new BaseCommand()},
                {Comands.ToJSON,              new ScreenToJsonCommand("ScreenState")},
                {Comands.Option1,             new BaseCommand()},
                {Comands.Option2,             new BaseCommand()},
                {Comands.Option3,             new BaseCommand()},
                {Comands.Option4,             new BaseCommand()},
                {Comands.Option5,             new BaseCommand()},
                {Comands.Option6,             new BaseCommand()},
                {Comands.Option7,             new BaseCommand()},
                {Comands.Option8,             new BaseCommand()},
                {Comands.Option9,             new BaseCommand()},
                {Comands.LastSceen,           new BaseCommand()}
            };
    }
}