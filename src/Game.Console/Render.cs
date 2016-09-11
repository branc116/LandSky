using LandSky.Components;
using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game.Cns
{
    public class Render
    {
        private new Dictionary<Material, char> mTexture = new Dictionary<Material, char>() {
            { Material.Air, '.' },
            {Material.Fire, '~'},
            {Material.Loot, '$'},
            {Material.Npc, 'N'},
            {Material.Path, '#'},
            {Material.Player, '@'},
            {Material.Trap, '.'},
            {Material.HorisontalWall, '-'},
            {Material.VerticalWall, '|'},
            {Material.Water, '}'},
            {Material.Darknes, ' '}
            };

        public string RenderSandBoxMap(IEnumerable<Component> Map, Component AroundWho, Size Size)
        {
            return RenderSandBoxMap(Map, new Rectangle(new Point(AroundWho.GlobalX, AroundWho.GlobalY), Size.Width, Size.Height));
        }

        public string RenderSandBoxMap(IEnumerable<Component> Map, Rectangle Bounds)
        {
            var watch = new Stopwatch();
            watch.Start();
            List<string> Return = new List<string>(Bounds.Height + 1);
            InfinitePlane plane = Map.FirstOrDefault(i => i.GetType() == typeof(InfinitePlane)) as InfinitePlane;
            if (plane == null)
                return string.Empty;
            string ReturnString = string.Empty;
            for (int i = 0; i < Bounds.Height; i++)
            {
                Return.Add(string.Empty);
                for (int j = 0; j < Bounds.Width; j++)
                {
                    Return[i] += plane.GetPoint(new Point(Bounds.LeftBound + j, Bounds.TopBound - i)) ? mTexture[Material.Air] : mTexture[Material.Darknes];
                }
            }
            foreach (var player in Map.Where(i => i.GetType() == typeof(Player)))
            {
                Bounds.ToTopLeft(new Rectangle(new Point(player.GlobalX, player.GlobalY), 1, 1));
                for (int i = Bounds.TopBound; i < Bounds.BottomBound; i++)
                {
                    for (int j = Bounds.LeftBound; j < Bounds.RightBound; j++)
                    {
                        Return[i] = $"{Return[i].Substring(0, i)}{mTexture[Material.Player]}{Return[i].Substring(i + 1)}";
                    }
                }
            }
            for (int i = 0; i < Return.Count; i++)
            {
                ReturnString += Return[i] + Environment.NewLine;
            }
            watch.Stop();
            ReturnString += $"PointsSaved: {plane._table.Count} Draw time: {watch.ElapsedTicks} ticks, {watch.ElapsedMilliseconds} ms";
            return ReturnString;
        }
    }
}