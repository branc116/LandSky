namespace LandSky.DotNetExt
{
    internal interface IMyConsoleKeyInfo
    {
        bool Alt { get; set; }
        bool Ctrl { get; set; }
        char KeyChar { get; set; }
        bool Shift { get; set; }
    }
}