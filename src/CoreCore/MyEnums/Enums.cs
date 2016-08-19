namespace LandSky.MyEnums
{
    /// <summary>
    /// All supported commands
    /// </summary>
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
        LeftScene,
        RightScene,
        GenerateOneRoom,
        GenerateALotOfRooms,
        GenerateRandomPath,
        DequeMessage,
        ShowMessages,
        ShowDebug,
        ShowHelp,
        LastSceen,
        ToJSON,
        Option1,
        Option2,
        Option3,
        Option4,
        Option5,
        Option6,
        Option7,
        Option8,
        Option9,
        Any
    }

    /// <summary>
    /// All supported monoms
    /// </summary>
    public enum KindOfMonom
    {
        /// <summary>
        /// a*x^BottomBound
        /// </summary>
        Line,

        /// <summary>
        /// a*Sin(BottomBound*x)
        /// </summary>
        Sine,

        Constant
    }

    /// <summary>
    /// All supported material that can be displayed
    /// </summary>
    public enum Material
    {
        Path,
        HorisontalWall,
        VerticalWall,
        Trap,
        Player,
        Npc,
        Loot,
        Water,
        Fire,
        Air,
        Darknes
    }

    /// <summary>
    /// All of different game screens that can be displayed
    /// </summary>
    public enum GameSceens
    {
        Game
    }

    /// <summary>
    /// Different menu screens that can be displayed
    /// </summary>
    public enum MenuSceens
    {
        MainMenu,
        Help,
        KeyMap,
        ListRooms,
        ListPaths,
        Message,
        Debug
    }

    /// <summary>
    /// Directions the player can move
    /// </summary>
    public enum MoveDirection
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    /// <summary>
    /// Cartesian coordinate system quadrants
    /// </summary>
    public enum Quadrant
    {
        First,
        Second,
        Third,
        Fourth
    }

    /// <summary>
    /// Supported states of the screen
    /// </summary>
    public enum ScreenState
    {
        Active,
        Paused,
        Disposed
    }

    public enum SizeMode
    {
        Auto,
        Explicit
    }

    public enum InputMode
    {
        ControlOnly,
        ControlFirst,
        InputOnly,
        InputFirst
    }
}