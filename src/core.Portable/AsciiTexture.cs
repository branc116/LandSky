using LandSky.MyEnums;
using System.Collections.Generic;

namespace LandSky
{
    internal static class AsciiTexture
    {
        public static Dictionary<Material, char> AsciiTextures = new Dictionary<Material, char>()
        {
            { Material.Air, '.'},
            { Material.Fire, '~'},
            { Material.Loot, '$'},
            { Material.Npc, 'N'},
            { Material.Path, '#'},
            { Material.Player, '@'},
            { Material.Trap, '.'},
            { Material.HorisontalWall, '-'},
            { Material.VerticalWall, '|'},
            { Material.Water, '}'},
            { Material.Darknes, ' '}
        };
    }
}