using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;
using static System.Convert;

using LandSky.Commands;
using LandSky.Components;
using LandSky.MyEnums;
using LandSky.MyMath;

namespace LandSky.Screen
{
    /// <summary>
    ///     Displays the instance of the game. This Extends BaseScreen.
    /// </summary>
    public class SandboxMap : BaseScreen
    {
        /// <summary>
        ///     Current state of the game instance
        /// </summary>
        private List<List<char>> mBuff1;
        private bool mGhost;
        private readonly object mLockDrawMethode = new object();
        /// <summary>
        ///     The bounds in the Cartesian coordinate system
        /// </summary>
        private Rectangle mMBoundsAroundThisPlayer;
        private int mSpeed;
        /// <summary>
        ///     Textures for the materials
        /// </summary>
        private Dictionary<Material, char> mTexture;
        private Player mThisPlayer;
        /// <summary>
        ///     Matrix for zbuffering
        /// </summary>
        private List<List<int>> mUpdated;
        /// <summary>
        ///     Create new Engine Screen
        /// </summary>
        /// <param Name="Top">Distance from the top of the global console</param>
        /// <param Name="Left">Distance form the left of the global console</param>
        public SandboxMap(int Top, int Left) : base(Top, Left, "Land sky")
        {
            InitProperties();
            InitBuffer();
            InitTexture();
            InitEvents();
            InitControls();
            EngineConsoleDraw();
        }
        public Rectangle BoundsAroundThisPlayer
        {
            get
            {
                if (mMBoundsAroundThisPlayer == null)
                    mMBoundsAroundThisPlayer =
                        new Rectangle(new Point(mThisPlayer.LocalBounds.X, mThisPlayer.LocalBounds.Y), WantedWidth -1,
                            WantedHeight - 1);
                else
                {
                    mMBoundsAroundThisPlayer.X = mThisPlayer.LocalBounds.X;
                    mMBoundsAroundThisPlayer.Y = mThisPlayer.LocalBounds.Y;
                }
                return mMBoundsAroundThisPlayer;
            }
        }
        public event EventHandler<string> Message;

        private void InitBuffer()
        {
            mBuff1 = new List<List<char>>(TrueHeight + 1);
            for (var J = 0; J <= TrueHeight; J++)
            {
                mBuff1.Add(new List<char>(TrueWidth + 1));
                mBuff1[J] = new List<char>(TrueWidth + 1);
                if (J == 0)
                {
                    for (var I = 0; I <= TrueWidth; I++)
                    {
                        mBuff1[J].Add(new char());
                    }
                }
                else
                    mBuff1[J].AddRange(mBuff1[0]);
            }
            mUpdated = new List<List<int>>();

            for (var I = 0; I <= TrueHeight; I++)
            {
                mUpdated.Add(new List<int>(TrueWidth + 1));
                if (I == 0)
                {
                    for (var J = 0; J < TrueWidth + 1; J++)
                    {
                        mUpdated[I].Add(-1);
                    }
                }
                else
                {
                    mUpdated[I].AddRange(mUpdated[0]);
                }
            }
        }

        private void ResetBuffers()
        {
            
            mUpdated = mUpdated.Select(UpLine => UpLine
                .Select(UpChar => -1)
                .ToList())
                .ToList();

            mBuff1 = mBuff1.Select(BuffLine => BuffLine
                .Select(BuffChar => BuffChar ==  ' ' ? BuffChar : ' ')
                .ToList())
                .ToList();
            
        }

        private void InitTexture()
        {
            mTexture = new Dictionary<Material, char>();
            mTexture.Add(Material.Air, '.');
            mTexture.Add(Material.Fire, '~');
            mTexture.Add(Material.Loot, '$');
            mTexture.Add(Material.Npc, 'N');
            mTexture.Add(Material.Path, '#');
            mTexture.Add(Material.Player, '@');
            mTexture.Add(Material.Trap, '.');
            mTexture.Add(Material.HorisontalWall, '-');
            mTexture.Add(Material.VerticalWall, '|');
            mTexture.Add(Material.Water, '}');
            mTexture.Add(Material.Darknes, ' ');
        }
        private void InitControls()
        {
            MLocalCommands.Add(Comands.Left, GeneralMove);
            MLocalCommands.Add(Comands.TenStepsLeft, GeneralMove);
            MLocalCommands.Add(Comands.Right, GeneralMove);
            MLocalCommands.Add(Comands.TenStepsRight, GeneralMove);
            MLocalCommands.Add(Comands.Up, GeneralMove);
            MLocalCommands.Add(Comands.TenStepsUp, GeneralMove);
            MLocalCommands.Add(Comands.Down, GeneralMove);
            MLocalCommands.Add(Comands.TenStepsDown, GeneralMove);
            MLocalCommands.Add(Comands.GenerateALotOfRooms, GenerateRooms);
            MLocalCommands.Add(Comands.GenerateOneRoom, GenerateRooms);
            MLocalCommands.Add(Comands.GenerateRandomPath, GeneratePath);
            foreach (var Comm in MLocalCommands)
            {
                Comand.Add(Comm.Key, Comm.Value);
            }
        }
        private void InitProperties()
        {
            WantedWidth = MaxWidth;
            WantedHeight = MaxHeight - 10;
            mThisPlayer = new Player("CrazyMOFO");
            Insert(mThisPlayer);
            mBuff1 = new List<List<char>>();
            mUpdated = new List<List<int>>();
            MLocalCommands = new Dictionary<Comands, Action<BaseCommand>>();
            MadeOf = Material.Darknes;
            mGhost = true;
            mSpeed = 50; // steep = 1/speed
            IsRoot = true;
        }
        private void InitEvents()
        {
        }
        public void MyDispose()
        {
            foreach (var Comm in MLocalCommands)
            {
                Comand.Remove(Comm.Key);
            }
        }

        /// <summary>
        ///     Generate new rooms in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GenerateRoomsCommand) </param>
        private void GenerateRooms(BaseCommand Bc)
        {
            var GRc = Bc as GenerateRoomsCommand;
            Task.Run(async () =>
            {
                var StartTime = DateTime.Now;
                await GenerateRandomRooms((Bc as GenerateRoomsCommand).NumberOfRooms);

                EnqueMessage(string.Format("Generated {0} rooms in {0}s\n", (Bc as GenerateRoomsCommand).NumberOfRooms,
                    DateTime.Now.Second - StartTime.Second));
                EngineConsoleDraw();
            }, GenerateRoomsCommand.CancelGenerting);
        }
        /// <summary>
        ///     Generate new paths in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GeneratePathCommand) </param>
        private async void GeneratePath(BaseCommand Bc)
        {
            if (NumOfRooms < 10)
                await Task.Run(() => { GenerateRooms(new GenerateRoomsCommand(100)); });
            for (var I = 0; I < (Bc as GeneratePathCommand).NumberOfPaths; I++)
            {
                var P = new Path("Path" + NumOfPaths);
                P.GeneratePathThrueRandomChildren(this);
                Insert(P);
            }
            EngineConsoleDraw();
        }
        /// <summary>
        ///     Move played in current game instance
        /// </summary>
        /// <param Name="bc">this should be typeof(MoveCommand) </param>
        private void GeneralMove(BaseCommand Bc)
        {
            var Move = Bc as MoveCommand;
            MoveCommand.InitToken();
            var T = Task.Run(async () =>
            {
                int Steps = Move.Steps;
                while (Steps > 0)
                {
                    if (Move.Direction == MoveDirection.Down || Move.Direction == MoveDirection.DownLeft ||
                        Move.Direction == MoveDirection.DownRight)
                    {
                        if (GetComponentOnLocation(mThisPlayer.LocalX, mThisPlayer.LocalY - 1).IsPassable || mGhost)
                        {
                            mThisPlayer.LocalY--;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Up || Move.Direction == MoveDirection.UpLeft ||
                        Move.Direction == MoveDirection.UpRight)
                    {
                        if (GetComponentOnLocation(mThisPlayer.LocalX, mThisPlayer.LocalY + 1).IsPassable || mGhost)
                        {
                            mThisPlayer.LocalY++;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Left || Move.Direction == MoveDirection.UpLeft ||
                        Move.Direction == MoveDirection.DownLeft)
                    {
                        if (GetComponentOnLocation(mThisPlayer.LocalX + 1, mThisPlayer.LocalY).IsPassable || mGhost)
                        {
                            mThisPlayer.LocalX--;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Right || Move.Direction == MoveDirection.UpRight ||
                        Move.Direction == MoveDirection.DownRight)
                    {
                        if (GetComponentOnLocation(mThisPlayer.LocalX - 1, mThisPlayer.LocalY).IsPassable || mGhost)
                        {
                            mThisPlayer.LocalX++;
                            Steps--;
                        }
                    }
                    EngineConsoleDraw();
                    await Task.Delay(mSpeed);
                }
            }, MoveCommand.CancleMove);
        }
        protected override void GenerateFooter()
        {
            try
            {
                string Mid = $"({mThisPlayer.LocalX},{mThisPlayer.LocalY}) - {GetComponentOnLocation(mThisPlayer.LocalX, mThisPlayer.LocalY).Name}";
                GenerateFooter(Mid);
            }
            catch
            {
                base.GenerateFooter();
            }
        }
        private void EngineConsoleDraw()
        {
            lock (mLockDrawMethode)
            {
                ResetBuffers();
                
                ZBufferUpdate(this);
                FillBuffer(BoundsAroundThisPlayer, MadeOf, 0);
                FlushBuffer();
                ScreenChange();

            }
        }
        private void ZBufferUpdate(Component Comp)
        {
            var GoodComponents = Comp.Controls.Where(I => I.Value.GetType() != typeof(Path) &&
                                                          I.Value.GlobalBounds & BoundsAroundThisPlayer);
            foreach (var GoodComponent in GoodComponents)
            {
                ZBufferUpdate(GoodComponent.Value);
            }
            FillBuffer(Comp.GlobalBounds, Comp.MadeOf, Comp.ZValue);
            DrawPaths(Comp);
        }
        private void FillBuffer(Rectangle TransformdBounds, Material madeOf, int ZLevel)
        {
            var Transformed = BoundsAroundThisPlayer.ToTopLeft(TransformdBounds);
            for (int I = Min( Transformed.TopBound, Transformed.BottomBound); I <= Max(Transformed.TopBound, Transformed.BottomBound); I++)
            {
                for (int J = Transformed.LeftBound; J <= Transformed.RightBound; J++)
                {
                    if (mUpdated[I][J] < ZLevel)
                    {
                        mBuff1[I][J] = mTexture[madeOf];
                        mUpdated[I][J] = ZLevel;
                    }
                }
            }
        }
        
        private void FlushBuffer()
        {
            Clear();
            foreach (var C in mBuff1)
                VirtualConsoleAddLine(new string(C.ToArray()));
        }
        private void DrawPaths(Component ComponentsWithPaths)
        {
            foreach (var path in ComponentsWithPaths.Controls.Where(I => I.Value.GetType() == typeof(Path)))
            {
                var Component = path.Value as Path;
                
                var Pol = Component.Poly;

                for (int J = BoundsAroundThisPlayer.LeftBound; J < BoundsAroundThisPlayer.RightBound; J++)
                {
                    var TransformdBounds = new Rectangle(
                        new Point(J, ToInt32(Pol.ValueForX(J))),
                        1,
                        Abs(ToInt32(Pol.DerivativeForX(J)*2)) + 4);
                    if (TransformdBounds & BoundsAroundThisPlayer)
                        FillBuffer(TransformdBounds , 
                                    path.Value.MadeOf, 
                                    path.Value.ZValue);
                }
            }
        }
    }
}