using LandSky.Commands;
using LandSky.Components;
using LandSky.MyEnums;
using LandSky.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;

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

        private Component _ActiveComponent;

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
            //EngineConsoleDraw();
        }

        public Rectangle BoundsAroundThisPlayer
        {
            get
            {
                if (_ActiveComponent.IsRoot != true)
                    return new Rectangle(new Point(_ActiveComponent.LocalBounds.X, _ActiveComponent.LocalBounds.Y), WantedWidth - 1, WantedHeight - 1);
                return new Rectangle(new Point(0, 0), WantedWidth, WantedHeight);
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
                .Select(BuffChar => BuffChar == ' ' ? BuffChar : ' ')
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
            WantedWidth = 50;
            WantedHeight = 20;
            _ActiveComponent = this;
            //Insert(_ActiveComponent);
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

        private object GenerateRoomsLock = new object();

        /// <summary>
        ///     Generate new rooms in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GenerateRoomsCommand) </param>
        public void GenerateRooms(BaseCommand Bc)
        {
            lock (GenerateRoomsLock)
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
        }

        /// <summary>
        ///     Generate new paths in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GeneratePathCommand) </param>
        public async void GeneratePath(BaseCommand Bc)
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
                        if (GetComponentOnLocation(_ActiveComponent.LocalX, _ActiveComponent.LocalY - 1).IsPassable || mGhost)
                        {
                            _ActiveComponent.LocalY--;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Up || Move.Direction == MoveDirection.UpLeft ||
                        Move.Direction == MoveDirection.UpRight)
                    {
                        if (GetComponentOnLocation(_ActiveComponent.LocalX, _ActiveComponent.LocalY + 1).IsPassable || mGhost)
                        {
                            _ActiveComponent.LocalY++;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Left || Move.Direction == MoveDirection.UpLeft ||
                        Move.Direction == MoveDirection.DownLeft)
                    {
                        if (GetComponentOnLocation(_ActiveComponent.LocalX + 1, _ActiveComponent.LocalY).IsPassable || mGhost)
                        {
                            _ActiveComponent.LocalX--;
                            Steps--;
                        }
                    }
                    if (Move.Direction == MoveDirection.Right || Move.Direction == MoveDirection.UpRight ||
                        Move.Direction == MoveDirection.DownRight)
                    {
                        if (GetComponentOnLocation(_ActiveComponent.LocalX - 1, _ActiveComponent.LocalY).IsPassable || mGhost)
                        {
                            _ActiveComponent.LocalX++;
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
                string Mid = $"({_ActiveComponent.LocalX},{_ActiveComponent.LocalY}) - {GetComponentOnLocation(_ActiveComponent.LocalX, _ActiveComponent.LocalY).Name}";
                GenerateFooter(Mid);
            }
            catch
            {
                base.GenerateFooter();
            }
        }

        private object LockChangeActiveComponent = new object();

        public void ChangeActiveComponent(string NewActiveComponent, int NewWidth, int NewHeight)
        {
            lock (LockChangeActiveComponent)
            {
                _ActiveComponent = this.Controls.First(i => i.Value.Name == NewActiveComponent).Value;
                WantedWidth = NewWidth;
                WantedHeight = NewHeight;
                EngineConsoleDraw();
            }
        }

        private void EngineConsoleDraw()
        {
            lock (mLockDrawMethode)
            {
                //ResetBuffers();
                InitBuffer();
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
            for (int I = Min(Transformed.TopBound, Transformed.BottomBound); I <= Max(Transformed.TopBound, Transformed.BottomBound); I++)
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
                        new Point(J, Pol.IntValueForX(J)),
                        1,
                        Abs(Pol.IntDerivativeForX(J) * 2) + 4);
                    if (TransformdBounds & BoundsAroundThisPlayer)
                        FillBuffer(TransformdBounds,
                                    path.Value.MadeOf,
                                    path.Value.ZValue);
                }
            }
        }
    }
}