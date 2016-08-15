using System;
using System.Collections.Generic;

using LandSky.Commands;
using LandSky.MyEnums;

namespace LandSky
{
    /// <summary>
    /// Contains keymaps, and commandmaps
    /// </summary>
    class CommandControls
    {
        public static Dictionary<ConsoleKeyInfo, Comands> KeyMap = new Dictionary<ConsoleKeyInfo, Comands>
            {
                {new ConsoleKeyInfo('h',ConsoleKey.H,false,false,false) , Comands.Left},
                {new ConsoleKeyInfo('H',ConsoleKey.H,false,false,false), Comands.TenStepsLeft},
                {new ConsoleKeyInfo('l',ConsoleKey.L,false,false,false), Comands.Right},
                {new ConsoleKeyInfo('L',ConsoleKey.L,false,false,false), Comands.TenStepsRight},
                {new ConsoleKeyInfo('k',ConsoleKey.K,false,false,false), Comands.Up},
                {new ConsoleKeyInfo('K',ConsoleKey.K,false,false,false), Comands.TenStepsUp},
                {new ConsoleKeyInfo('j',ConsoleKey.J,false,false,false), Comands.Down},
                {new ConsoleKeyInfo('J',ConsoleKey.J,false,false,false), Comands.TenStepsDown},
                {new ConsoleKeyInfo('q',ConsoleKey.Q,false,false,false), Comands.ScrollLeft},
                {new ConsoleKeyInfo('e',ConsoleKey.E,false,false,false), Comands.ScrollRight},
                {new ConsoleKeyInfo(',',ConsoleKey.OemComma,false,false,false), Comands.ScrollLeft},
                {new ConsoleKeyInfo('.',ConsoleKey.OemPeriod,false,false,false), Comands.ScrollRight},
                {new ConsoleKeyInfo('r',ConsoleKey.R,false,false,false), Comands.GenerateOneRoom},
                {new ConsoleKeyInfo('R',ConsoleKey.R,false,false,false), Comands.GenerateALotOfRooms},
                {new ConsoleKeyInfo('P',ConsoleKey.P,false,false,false), Comands.GenerateRandomPath},
                {new ConsoleKeyInfo('d',ConsoleKey.D,false,false,false), Comands.DequeMessage},

                {new ConsoleKeyInfo('\0',ConsoleKey.S,false,true,true), Comands.ToJSON },//fucking windows....
                {new ConsoleKeyInfo('s',ConsoleKey.S,false,true,false), Comands.ToJSON },
                {new ConsoleKeyInfo((char)27,ConsoleKey.Escape,false,false,false), Comands.LastSceen},
                {new ConsoleKeyInfo('?',ConsoleKey.Oem2,false,false,false), Comands.ShowHelp},
                {new ConsoleKeyInfo('?',0,false,false,false), Comands.ShowHelp},
                {new ConsoleKeyInfo('*',ConsoleKey.OemPlus,false,false,false), Comands.ShowDebug}, //fucking windows....
                {new ConsoleKeyInfo('*',ConsoleKey.Multiply,false,false,false), Comands.ShowDebug},
                {new ConsoleKeyInfo('1',ConsoleKey.D1,false,false,false), Comands.Option1},
                {new ConsoleKeyInfo('2',ConsoleKey.D2,false,false,false), Comands.Option2},
                {new ConsoleKeyInfo('3',ConsoleKey.D3,false,false,false), Comands.Option3},
                {new ConsoleKeyInfo('4',ConsoleKey.D4,false,false,false), Comands.Option4},
                {new ConsoleKeyInfo('5',ConsoleKey.D5,false,false,false), Comands.Option5},
                {new ConsoleKeyInfo('6',ConsoleKey.D6,false,false,false), Comands.Option6},
                {new ConsoleKeyInfo('7',ConsoleKey.D7,false,false,false), Comands.Option7},
                {new ConsoleKeyInfo('8',ConsoleKey.D8,false,false,false), Comands.Option8},
                {new ConsoleKeyInfo('9',ConsoleKey.D9,false,false,false), Comands.Option9}
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
