using System;
using System.Collections.Generic;
using System.Linq;

using MultyNetHack.Commands;
using MultyNetHack.Components;
using MultyNetHack.MyEnums;

namespace MultyNetHack.Screen
{
    /// <summary>
    /// This should show all debug info (not implemented yet)
    /// </summary>
    public class DebugScreen : BaseScreen 
    {
        public DebugScreen(int Top, int Left, BaseScreen ScreenToDebug) : base (Top,
                                                                                Left,
                                                                                $"Debugging {ScreenToDebug.Name} screen, it's type of {ScreenToDebug.GetType().ToString().Split(new char[] {'.'}).Last()}"
                                                                               )
        {
            MInitText(ScreenToDebug);
            ScreenChange();
        }
        private void MInitText(BaseScreen ScreenToDebug)
        {
            VirtualConsoleAddLine($"Contains {ScreenToDebug.NumOfRooms} rooms, {ScreenToDebug.NumOfPaths} paths, {ScreenToDebug.NumOfWalls} walls");
            PrintControls(ScreenToDebug as Component, 1);
            VirtualConsoleAddLine($"Able to input {ScreenToDebug.Comand.Count} different commands");
            MPrintCommands(ScreenToDebug.Comand, 1);
        }
        private void MPrintCommands(Dictionary<Comands, Action<BaseCommand>> MCommands, int Indent)
        {
            var Km = MultyNetHack.Controls.KeyMap;
            foreach(var Command in MCommands)
            {
                VirtualConsoleAddLine(
                    $"{new string(' ', Indent*2)}{Km.Where(I => I.Value == Command.Key).ToList()?[0].Key} -> {Command.Key} -> {Command.Value.Method.Name}");
            }
        }
        private void PrintControls(Component ScreenToDebug, int Indent)
        {
            if (ScreenToDebug.NumOfRooms > 0)
            {
                MPrintRooms(ScreenToDebug.Controls.Where(I => I.Value.GetType() == typeof(Room)).ToList(), Indent);
            }
            if (ScreenToDebug.NumOfPaths > 0)
            {
                MPrintPaths(ScreenToDebug.Controls.Where(I => I.Value.GetType() == typeof(Path)).ToList(), Indent);
            }
            if (ScreenToDebug.NumOfWalls > 0)
            {
                MPrintWalls(ScreenToDebug.Controls.Where(I => I.Value.GetType() == typeof(Wall) ).ToList(), Indent);
            }
        }
        private void MPrintWalls(IEnumerable<KeyValuePair<string, Component>> MWalls, int Indent)
        {
            foreach (var Wall in MWalls)
            {
                VirtualConsoleAddLine(string.Format("{0}Wall {1} is localy located on ({2},{3}) and globaly on ({4},{5}), width = {6}, height = {7}", new object[] { new string(' ', Indent * 2), Wall.Value.Name, Wall.Value.LocalX, Wall.Value.LocalY, Wall.Value.GlobalX, Wall.Value.GlobalY, Wall.Value.LocalBounds.Width, Wall.Value.LocalBounds.Height }));
                if (Wall.Value.Controls.Count>0)
                    VirtualConsoleAddLine(
                        $"{new string(' ', Indent*2)}Wall contains {Wall.Value.NumOfRooms} Rooms, {Wall.Value.NumOfPaths} Paths, {Wall.Value.NumOfWalls} walls");
                PrintControls(Wall.Value, Indent + 1);
            }
        }
        private void MPrintPaths(IEnumerable<KeyValuePair<string, Component>> MPaths, int Indent)
        {
            foreach (var Path in MPaths)
            {
                VirtualConsoleAddLine(
                    $"{new string(' ', Indent*2)}Path {Path.Value.Name} is polynomial of {(Path.Value as Path).ConnectedComponent.Count}th power");
                if (Path.Value.Controls.Count > 0)
                    VirtualConsoleAddLine(
                        $"{new string(' ', Indent*2)}Room contains {Path.Value.NumOfRooms} Rooms, {Path.Value.NumOfPaths} Paths, {Path.Value.NumOfWalls} walls");
                if ((Path.Value as Path).ConnectedComponent.Count > 0)
                {
                    MPrintRooms((Path.Value as Path).ConnectedComponent.Where(I => I.GetType() == typeof(Room)).ToList(), Indent + 1);
                }
                PrintControls(Path.Value, Indent + 1);

            }
        }
        private void MPrintRooms(IEnumerable<Component> MRooms, int Indent)
        {
            foreach (var Room in MRooms)
            {
                VirtualConsoleAddLine(
                    $"{new string(' ', Indent*2)}Room {Room.Name} is located on ({Room.LocalX},{Room.LocalY}), width = {Room.LocalBounds.Width}, height = {Room.LocalBounds.Height}");
                if (Room.Controls.Count > 0)
                    VirtualConsoleAddLine(
                        $"{new string(' ', Indent*2)}Room contains {Room.NumOfRooms} Rooms, {Room.NumOfPaths} Paths, {Room.NumOfWalls} walls");
                if (Room.NumOfRooms > 0)
                {
                    MPrintRooms(Room.Controls.Where(I => I.Value.GetType() == typeof(Room)).ToList(), Indent + 1);
                }
                PrintControls(Room, Indent + 1);

            }
        }
        private void MPrintRooms(IEnumerable<KeyValuePair<string, Component>> MRooms,int Indent)
        {
            foreach(var Room in MRooms)
            {
                VirtualConsoleAddLine(
                    $"{new string(' ', Indent*2)}Room {Room.Value.Name} is located on ({Room.Value.LocalX},{Room.Value.LocalY}), width = {Room.Value.LocalBounds.Width}, height = {Room.Value.LocalBounds.Height}");
                if (Room.Value.Controls.Count > 0)
                    VirtualConsoleAddLine(
                        $"{new string(' ', Indent*2)}Room contains {Room.Value.NumOfRooms} Rooms, {Room.Value.NumOfPaths} Paths, {Room.Value.NumOfWalls} walls");
                PrintControls(Room.Value, Indent + 1);
            }
        }
    }
}
