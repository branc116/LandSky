﻿using LandSky.MyEnums;
using LandSky.MyMath;

namespace LandSky.Components
{
    /// <summary>
    /// Player with the location and texture
    /// </summary>
    public class Player : Component
    {
        public PermissionsLevel PermissionsLevel { get; set; }

        public Player(string Name) : base($"{Name}")
        {
            this.ZValue = 5;
            this.Bounds = Rectangle.DefineRectangleByWidthAndHeight(0, 0, 0, 0);

            MadeOf = Material.Player;
        }
    }
}