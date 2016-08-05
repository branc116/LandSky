using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;
using static System.Convert;

using MultyNetHack.Commands;
using MultyNetHack.Components;
using MultyNetHack.MyEnums;
using MultyNetHack.MyMath;

namespace MultyNetHack.Screen
{
    /// <summary>
    /// Displays the instance of the game. This Extends BaseScreen.
    /// </summary>
    public class SandboxMap : BaseScreen
    {
        /// <summary>
        /// Matrix for zbuffering
        /// </summary>
        private List<List<int>> updated;
        /// <summary>
        /// Current state of the game instance
        /// </summary>
        private List<List<char>> buff1;
        /// <summary>
        /// Textures for the materials
        /// </summary>
        Dictionary<Material, char> Texture;
        public event EventHandler<string> Message;
        /// <summary>
        /// Invoke this when update in the game happens( Played move, npc move...)
        /// </summary>
        public event EventHandler<int> Draw;
        /// <summary>
        /// The bounds in the Cartesian coordinate system
        /// </summary>

        
        bool ghost; 
        int speed;
        /// <summary>
        /// Create new Engine Screen
        /// </summary>
        /// <param Name="Top">Distance from the top of the global console</param>
        /// <param Name="Left">Distance form the left of the global console</param>
        public SandboxMap(int Top, int Left) : base(Top, Left, "MultyNetHack")
        {
            InitProperties();
            InitBuffer();
            InitTexture();
            InitEvents();
            InitControls();
            Draw?.Invoke(this, 22);
        }
        private void InitBuffer()
        {

            buff1 = new List<List<char>>(TrueHeight);
            for (int j = 0; j < TrueHeight; j++)
            {
                buff1.Add(new List<char>(TrueWidth));
                buff1[j] = new List<char>(TrueWidth);
                if (j == 0)
                {
                    for (int i = 0; i < TrueWidth; i++)
                    {
                        buff1[j].Add(new char());
                    }
                }
                else
                    buff1[j].AddRange(buff1[0]);
            }
        }
        private void InitTexture()
        {
            Texture = new Dictionary<Material, char>();
            Texture.Add(Material.Air, '.');
            Texture.Add(Material.Fire, '~');
            Texture.Add(Material.Loot, '$');
            Texture.Add(Material.Npc, 'N');
            Texture.Add(Material.Path, '#');
            Texture.Add(Material.Player, '@');
            Texture.Add(Material.Trap, '.');
            Texture.Add(Material.HorisontalWall, '-');
            Texture.Add(Material.VerticalWall, '|');
            Texture.Add(Material.Water, '}');
            Texture.Add(Material.Darknes, ' ');
        }
        private void InitControls()
        {
            mLocalCommands.Add(Comands.Left, GeneralMove);
            mLocalCommands.Add(Comands.TenStepsLeft, GeneralMove);
            mLocalCommands.Add(Comands.Right, GeneralMove);
            mLocalCommands.Add(Comands.TenStepsRight, GeneralMove);
            mLocalCommands.Add(Comands.Up, GeneralMove);
            mLocalCommands.Add(Comands.TenStepsUp, GeneralMove);
            mLocalCommands.Add(Comands.Down, GeneralMove);
            mLocalCommands.Add(Comands.TenStepsDown, GeneralMove);
            mLocalCommands.Add(Comands.GenerateALotOfRooms, GenerateRooms);
            mLocalCommands.Add(Comands.GenerateOneRoom, GenerateRooms);
            mLocalCommands.Add(Comands.GenerateRandomPath, GeneratePath);
            foreach(KeyValuePair<Comands,Action<BaseCommand>> mComm in mLocalCommands)
            {
                Comand.Add(mComm.Key, mComm.Value);
            }
        }
        private void InitProperties()
        {
            mBounds = new Rectangle(10, 30, -10, -30);
            LocalX = 0;
            LocalY = 0;
            ZValue = 0;
            
            WantedWidth = 60;
            WantedHeight = 20;
            
            buff1 = new List<List<char>>();
            updated = new List<List<int>>();
            mLocalCommands = new Dictionary<Comands, Action<BaseCommand>>();
            MadeOf = Material.Darknes;
            ghost = true;
            speed = 50; // steep = 1/speed
            IsRoot = true;
        }
        private void InitEvents()
        {
            Draw += EngineConsole_Draw;
        }
        public void MyDispose()
        {
            foreach (KeyValuePair<Comands, Action<BaseCommand>> mComm in mLocalCommands)
            {
                Comand.Remove(mComm.Key);
            }
            Draw -= EngineConsole_Draw;
        }

        /// <summary>
        /// Generate new rooms in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GenerateRoomsCommand) </param>
        private void GenerateRooms(BaseCommand bc)
        {
            GenerateRoomsCommand gRC = bc as GenerateRoomsCommand;
            Task.Run(async () =>
            {
                DateTime startTime = DateTime.Now;
                await this.GenerateRandomRooms((bc as GenerateRoomsCommand).numberOfRooms);
                
                EnqueMessage(string.Format("Generated {0} rooms in {0}s\n", (bc as GenerateRoomsCommand).numberOfRooms, DateTime.Now.Second-startTime.Second));
                Draw?.Invoke(null, 1);
            }, GenerateRoomsCommand.CancelGenerting);
            
        }
        /// <summary>
        /// Generate new paths in this instance of the game
        /// </summary>
        /// <param Name="bc">This should be typeof(GeneratePathCommand) </param>
        private async void GeneratePath(BaseCommand bc)
        {
            if (this.NumOfRooms < 10)
                await Task.Run(()=> { GenerateRooms(new GenerateRoomsCommand(100)); });
            for (int i = 0; i < (bc as GeneratePathCommand).NumberOfPaths; i++)
            {
                Path p = new Path("Path" + this.NumOfPaths);
                p.generatePathThrueRandomChildren(this);
                this.Insert(p);
            }
            Draw?.Invoke(this, 2);
        }
        /// <summary>
        /// Move played in current game instance
        /// </summary>
        /// <param Name="bc">this should be typeof(MoveCommand) </param>
        private void GeneralMove(BaseCommand bc)
        {
            MoveCommand move = bc as MoveCommand;
            MoveCommand.InitToken();
            Task t = Task.Run(async () =>
            {
                int mSteps = move.Steps;
                while (mSteps > 0)
                {
                    if (move.Direction == MoveDirection.Down || move.Direction == MoveDirection.DownLeft || move.Direction == MoveDirection.DownRight)
                    {
                        if (this.GetComponentOnLocation(this.LocalX, this.LocalY - 1).IsPassable || ghost)
                        {
                            this.LocalY--;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Up || move.Direction == MoveDirection.UpLeft || move.Direction == MoveDirection.UpRight)
                    {
                        if (this.GetComponentOnLocation(this.LocalX, this.LocalY + 1).IsPassable || ghost)
                        {
                            this.LocalY++;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Left || move.Direction == MoveDirection.UpLeft || move.Direction == MoveDirection.DownLeft)
                    {
                        if (this.GetComponentOnLocation(this.LocalX + 1, this.LocalY).IsPassable || ghost)
                        {
                            this.LocalX--;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Right || move.Direction == MoveDirection.UpRight || move.Direction == MoveDirection.DownRight)
                    {
                        if (this.GetComponentOnLocation(this.LocalX - 1, this.LocalY).IsPassable || ghost)
                        {
                            this.LocalX++;
                            mSteps--;
                        }
                    }
                    Draw?.Invoke(this, 2);
                    await Task.Delay(speed);
                }

            }, MoveCommand.CancleMove);
        }

        protected override void GenerateFooter()
        {
            try
            {
                string mid = string.Format("({0},{1}) - {2}", this.LocalX, this.LocalY, this.GetComponentOnLocation(this.LocalX, this.LocalY).Name);
                GenerateFooter(mid);
            }
            catch
            {
                base.GenerateFooter();
            }
        }
        bool inside = false;
        private void EngineConsole_Draw(object sender, int e)
        {
            if (!inside)
            {
                inside = true;
                updated = new List<List<int>>(TrueHeight);
                for (int i = 0; i < TrueHeight; i++)
                {

                    updated.Add(new List<int>(TrueWidth));
                    if (i == 0)
                    {
                        for (int j = 0; j < TrueWidth; j++)
                        {
                            updated[i].Add(-1);
                        }
                    }
                    else
                    {
                        updated[i].AddRange(updated[0]);
                    }
                }
                
                FillBuffer(new Rectangle(this.LocalY,this.LocalX ,this.LocalY - 1,this.LocalX - 1), Material.Player, 15);
                DrawPaths();
                ZBufferUpdate(this);
                FillBuffer(this.LocalBounds, this.MadeOf, 0);
                FlushBuffer();
                ScreenChange();
                inside = false;
            }
        }
        private void ZBufferUpdate(Component comp, int parentTop, int parentLeft)
        {

            Point startEnd = comp.GetStartEndEnter( comp.LocalX - parentLeft - comp.LocalBounds.Width, comp.LocalX - parentLeft + comp.LocalBounds.Width);
            for (int i = startEnd.x; i <= startEnd.y; i++)
            {
                if (this.LocalBounds & (comp.sweep[i].component.LocalBounds))
                {
                    //if (comp.GetType() != typeof(EngineSceen))
                        ZBufferUpdate(comp.sweep[i].component, parentTop + comp.LocalY, parentLeft + comp.LocalX);
                    //else
                    //    ZBufferUpdate(comp.sweep[i].component, 0, 0);

                }
            }
            FillBuffer(comp.LocalBounds.LeftBound - parentLeft , comp.LocalBounds.TopBound - parentTop, comp.LocalBounds.height, comp.LocalBounds.Width, comp.MadeOf, comp.ZValue);

        }
        private void ZBufferUpdate(Component comp, Point Translatin)
        {
            Rectangle TransformdBounds = comp.LocalBounds + Translatin;
            Point startEnd = comp.GetStartEndEnter(TransformdBounds.LeftBound, TransformdBounds.RightBound);
            for (int i = startEnd.x; i <= startEnd.y; i++)
            {
                if (comp.GetType() != typeof(SandboxMap))
                {
                    if (this.LocalBounds & (comp.sweep[i].component.LocalBounds + Translatin + new Point(comp.LocalX, comp.LocalY)))
                    {
                    
                        var NewTranslatin = new Point(comp.LocalX, comp.LocalY) + Translatin;
                        ZBufferUpdate(comp.sweep[i].component, NewTranslatin);
                    }
                    
                }
                else
                {
                    if (this.LocalBounds & (comp.sweep[i].component.LocalBounds + Translatin))
                        ZBufferUpdate(comp.sweep[i].component, Point.Origin());
                }
            }
            FillBuffer(TransformdBounds, comp.MadeOf, comp.ZValue);

        }
        private void ZBufferUpdate(Component comp)
        {
            var GoodComponents = comp.Controls.Where(i => ((i.Value.GetType() == typeof(Room) ||
                                                            i.Value.GetType() == typeof(Wall)) &&
                                                            i.Value.GlobalBounds & this.LocalBounds

            )).ToList();
            foreach (var GoodComponent in GoodComponents)
            {
                ZBufferUpdate(GoodComponent.Value);
            }
            FillBuffer(comp.GlobalBounds, comp.MadeOf, comp.ZValue);
        }
        private void FillBuffer(Rectangle transformdBounds, Material madeOf, int zLevel)
        {
            Rectangle Transformed = LocalBounds.ToTopLeft(transformdBounds);
            for (int i=Transformed.TopBound; i < Transformed.BottomBound; i++)
            {
                for (int j = Transformed.LeftBound; j < Transformed.RightBound; j++)
                {
                    if (updated[i][j] < zLevel)
                    {
                        buff1[i][j] = Texture[madeOf];
                        updated[i][j] = zLevel;
                    }
                }
            }
        }

        private void FillBuffer(int x, int y, int h, int w, Material m, int zLevle)
        {
            Point start = LocalBounds.ToTopLeft(x, y);
            Point end = LocalBounds.ToTopLeft(x + w, y - h);
            for (int i = Min(end.y, start.y); i < Max(start.y, end.y); i++)
            {
                for (int j = Min(end.x, start.x); j < Max(start.x, end.x); j++)
                {

                    if (updated[j][i] < zLevle)
                    {
                        buff1[j][i] = Texture[m];
                        updated[j][i] = zLevle;
                    }
                }
            }
        }
        private void FlushBuffer()
        {
            Clear();
            foreach (List<char> c in buff1)
                VirtualConsoleAddLine(new string(c.ToArray()));
            ScreenChange();
        }
        private void DrawPaths()
        {
            this.Controls.Where(i => i.Value.GetType() == typeof(Path)).ToList().ForEach((i) =>
            {
                LinearInterpolator pol = (i.Value as Path).Poly;
                for (int j = LocalBounds.LeftBound; j < LocalBounds.RightBound; j++)
                {
                    FillBuffer(j, ToInt32(pol.ValueForX(j) + Abs(pol.DerivativeForX(j))) + 2, Abs(ToInt32(pol.DerivativeForX(j)) * 2) + 4, 1, i.Value.MadeOf, i.Value.ZValue);
                }
            });
        }

    }
}
