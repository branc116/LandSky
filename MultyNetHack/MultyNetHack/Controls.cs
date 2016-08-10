using System;
using System.Collections.Generic;

using MultyNetHack.Commands;
using MultyNetHack.MyEnums;

namespace MultyNetHack
{
    /// <summary>
    /// Contains keymaps, and commandmaps
    /// </summary>
    class Controls
    {
        public static Dictionary<char, Comands> KeyMap;
        public static Dictionary<Comands, BaseCommand> InvokedBaseCommand;
        public static void LoadKeyMap()
        {
            KeyMap = new Dictionary<char, Comands>
            {
                {'h', Comands.Left},
                {'H', Comands.TenStepsLeft},
                {'l', Comands.Right},
                {'L', Comands.TenStepsRight},
                {'k', Comands.Up},
                {'K', Comands.TenStepsUp},
                {'j', Comands.Down},
                {'J', Comands.TenStepsDown},
                {'q', Comands.ScrollLeft},
                {'e', Comands.ScrollRight},
                {',', Comands.ScrollLeft},
                {'.', Comands.ScrollRight},
                {'r', Comands.GenerateOneRoom},
                {'R', Comands.GenerateALotOfRooms},
                {'P', Comands.GenerateRandomPath},
                {'d', Comands.DequeMessage},
                {Convert.ToChar(27), Comands.LastSceen},
                {'?', Comands.ShowHelp},
                {'*', Comands.ShowDebug},
                {'1', Comands.Option1},
                {'2', Comands.Option2},
                {'3', Comands.Option3},
                {'4', Comands.Option4},
                {'5', Comands.Option5},
                {'6', Comands.Option6},
                {'7', Comands.Option7},
                {'8', Comands.Option8},
                {'9', Comands.Option9}
            };
        }
        public static void LoadInvokedBaseCommand()
        {
            InvokedBaseCommand = new Dictionary<Comands, BaseCommand>
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
}
