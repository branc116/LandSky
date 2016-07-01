using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
    public enum Comands
    {
        Left,
        TenStepsLeft,
        Right,
        TenStepsRight,
        Up,
        TenStepsUp,
        Down,
        TenStepsDown,
        ScrollLeft,
        ScrollRight,
        TabLeft,
        TabRight,
        GenerateOneRoom,
        GenerateALotOfRooms,
        GenerateRandomPath,
        DequeMessage
    }
    public enum KindOfMonom
    {
        /// <summary>
        /// a*x^b
        /// </summary>
        Line,
        /// <summary>
        /// a*Sin(b*x)
        /// </summary>
        Sine,
        Constant
    }
    public enum Material
    {
        Path,
        HorisontalWall,
        VerticalWall,
        Trap,
        Player,
        Npc,
        Loot,
        Watter,
        Fire,
        Air,
        Darknes
    }
    enum TextShown
    {
        Help,
        ListRooms,
        ListPaths,
        Message,
        Debug,
        Nothing,
        Max
    }
}
