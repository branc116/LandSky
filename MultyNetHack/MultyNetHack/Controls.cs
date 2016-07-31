using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack.Commands;

namespace MultyNetHack
{
    /// <summary>
    /// Contains keymaps, and commandmaps
    /// </summary>
    class Controls
    {
        public static Dictionary<char, Comands> KeyMap;
        public static Dictionary<Comands, BaseCommand> InvokedBaseCommand;
        static public void LoadKeyMap()
        {
            KeyMap = new Dictionary<char, Comands>();
            KeyMap.Add('h', Comands.Left);
            KeyMap.Add('H', Comands.TenStepsLeft);
            KeyMap.Add('l', Comands.Right);
            KeyMap.Add('L', Comands.TenStepsRight);
            KeyMap.Add('k', Comands.Up);
            KeyMap.Add('K', Comands.TenStepsUp);
            KeyMap.Add('j', Comands.Down);
            KeyMap.Add('J', Comands.TenStepsDown);
            KeyMap.Add(',', Comands.ScrollLeft);
            KeyMap.Add('.', Comands.ScrollRight);
            KeyMap.Add('r', Comands.GenerateOneRoom);
            KeyMap.Add('R', Comands.GenerateALotOfRooms);
            KeyMap.Add('P', Comands.GenerateRandomPath);
            KeyMap.Add('d', Comands.DequeMessage);
            KeyMap.Add(Convert.ToChar(27), Comands.LastSceen);
            KeyMap.Add('?', Comands.ShowHelp);
            KeyMap.Add('*', Comands.ShowDebug);
            KeyMap.Add('1', Comands.Option1);
            KeyMap.Add('2', Comands.Option2);
            KeyMap.Add('3', Comands.Option3);
            KeyMap.Add('4', Comands.Option4);
            KeyMap.Add('5', Comands.Option5);
            KeyMap.Add('6', Comands.Option6);
            KeyMap.Add('7', Comands.Option7);
            KeyMap.Add('8', Comands.Option8);
            KeyMap.Add('9', Comands.Option9);
        }
        static public void LoadInvokedBaseCommand()
        {
            InvokedBaseCommand = new Dictionary<Comands, BaseCommand>();
            InvokedBaseCommand.Add(Comands.GenerateOneRoom, new GenerateRoomsCommand(1));
            InvokedBaseCommand.Add(Comands.GenerateALotOfRooms, new GenerateRoomsCommand(1000));
            InvokedBaseCommand.Add(Comands.Left, new MoveCommand(MoveDirection.Left, 1));
            InvokedBaseCommand.Add(Comands.TenStepsLeft, new MoveCommand(MoveDirection.Left, 10));
            InvokedBaseCommand.Add(Comands.Right, new MoveCommand(MoveDirection.Right, 1));
            InvokedBaseCommand.Add(Comands.TenStepsRight, new MoveCommand(MoveDirection.Right, 10));
            InvokedBaseCommand.Add(Comands.Down, new MoveCommand(MoveDirection.Down, 1));
            InvokedBaseCommand.Add(Comands.TenStepsDown, new MoveCommand(MoveDirection.Down, 10));
            InvokedBaseCommand.Add(Comands.Up, new MoveCommand(MoveDirection.Up, 1));
            InvokedBaseCommand.Add(Comands.TenStepsUp, new MoveCommand(MoveDirection.Up, 10));
            InvokedBaseCommand.Add(Comands.ScrollRight, new ScrollCommand(1));
            InvokedBaseCommand.Add(Comands.ScrollLeft, new ScrollCommand(-1));
            InvokedBaseCommand.Add(Comands.ShowHelp, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option1, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option2, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option3, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option4, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option5, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option6, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option7, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option8, new BaseCommand());
            InvokedBaseCommand.Add(Comands.Option9, new BaseCommand());
            InvokedBaseCommand.Add(Comands.LastSceen, new BaseCommand());
        }
    }
}
