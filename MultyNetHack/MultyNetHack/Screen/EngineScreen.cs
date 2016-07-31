using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack;
using MultyNetHack.Commands;
using static System.Math;
using static System.Convert;
namespace MultyNetHack.Screen
{
    /// <summary>
    /// Displays the instance of the game. This Extends BaseScreen.
    /// </summary>
    public class EngineSceen : BaseScreen
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
        Dictionary<Material, char> texture;
        public event EventHandler<string> Message;
        /// <summary>
        /// Invoke this when update in the game happends( Played move, npc move...)
        /// </summary>
        public event EventHandler<int> Draw;
        /// <summary>
        /// The bounds in the Cartesian coordinate system
        /// </summary>
        Rectangle bounds
        {
            get
            {
                return new Rectangle(x + TrueHeight / 2, y + TrueWidth / 2, x - TrueHeight / 2, y - TrueWidth / 2);
            }
        }
        
        bool ghost; 
        int speed;
        /// <summary>
        /// Create new Engine Screen
        /// </summary>
        /// <param name="Top">Distance from the top of the global console</param>
        /// <param name="Left">Distance form the left of the global console</param>
        public EngineSceen(int Top, int Left) : base(Top, Left, "Multy Net Hack")
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
            texture = new Dictionary<Material, char>();
            texture.Add(Material.Air, '.');
            texture.Add(Material.Fire, '~');
            texture.Add(Material.Loot, '$');
            texture.Add(Material.Npc, 'N');
            texture.Add(Material.Path, '#');
            texture.Add(Material.Player, '@');
            texture.Add(Material.Trap, '.');
            texture.Add(Material.HorisontalWall, '-');
            texture.Add(Material.VerticalWall, '|');
            texture.Add(Material.Watter, '}');
            texture.Add(Material.Darknes, ' ');
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
            x = 0;
            y = 0;
            WantedWidth = 70;
            WantedHeight = 20;
            buff1 = new List<List<char>>();
            updated = new List<List<int>>();
            mLocalCommands = new Dictionary<Comands, Action<BaseCommand>>();
            ghost = true;
            speed = 50; // steep = 1/speed

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
        /// <param name="bc">This shuld be typeof(GenerateRoomsCommand) </param>
        private async void GenerateRooms(BaseCommand bc)
        {
            GenerateRoomsCommand gRC = bc as GenerateRoomsCommand;
            await Task.Run(async () =>
            {
                DateTime startTime = DateTime.Now;
                await this.GenerateRandomRooms((bc as GenerateRoomsCommand).numberOfRooms);
                
                EnqueMessage(string.Format("Generated {0} rooms in {0}s\n", (bc as GenerateRoomsCommand).numberOfRooms, DateTime.Now.Second-startTime.Second));
                Draw?.Invoke(null, 1);
            }, GenerateRoomsCommand.CancelGenerting);
            Draw?.Invoke(this, 2);
        }
        /// <summary>
        /// Generate new paths in this instance of the game
        /// </summary>
        /// <param name="bc">This shuld be typeof(GeneratePathCommand) </param>
        private async void GeneratePath(BaseCommand bc)
        {
            if (this.numOfRooms < 10)
                await Task.Run(()=> { GenerateRooms(new GenerateRoomsCommand(100)); });
            for (int i = 0; i < (bc as GeneratePathCommand).NumberOfPaths; i++)
            {
                Path p = new Path("Path" + this.numOfPaths);
                p.generatePathThrueRandomChildren(this);
                this.Insert(p);
            }
            Draw?.Invoke(this, 2);
        }
        /// <summary>
        /// Move played in current game instance
        /// </summary>
        /// <param name="bc">this shuld be typeof(MoveCommand) </param>
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
                        if (this.GetComponentOnLocation(this.x, this.y - 1).isPassable || ghost)
                        {
                            this.y--;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Up || move.Direction == MoveDirection.UpLeft || move.Direction == MoveDirection.UpRight)
                    {
                        if (this.GetComponentOnLocation(this.x, this.y + 1).isPassable || ghost)
                        {
                            this.y++;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Left || move.Direction == MoveDirection.UpLeft || move.Direction == MoveDirection.DownLeft)
                    {
                        if (this.GetComponentOnLocation(this.x + 1, this.y).isPassable || ghost)
                        {
                            this.x--;
                            mSteps--;
                        }
                    }
                    if (move.Direction == MoveDirection.Right || move.Direction == MoveDirection.UpRight || move.Direction == MoveDirection.DownRight)
                    {
                        if (this.GetComponentOnLocation(this.x - 1, this.y).isPassable || ghost)
                        {
                            this.x++;
                            mSteps--;
                        }
                    }
                    Draw?.Invoke(this, 2);
                    await Task.Delay(speed);
                }

            }, MoveCommand.CancleMove);
            Draw?.Invoke(this, 2);
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
                
                FillBuffer(this.x, this.y, 1, 1, Material.Player, 15);
                DrawPaths();
                ZBufferUpdate(this, 0, 0);
                FillBuffer(bounds.l, bounds.t, TrueHeight, TrueWidth, this.madeOf, 0);
                FlushBuffer();
                FooterString = string.Format("({0},{1})", this.x, this.y);
                Flush();
                inside = false;
            }
        }
        private void ZBufferUpdate(Component comp, int parentTop, int parentLeft)
        {

            Point startEnd = comp.GetStartEndEnter(l - parentLeft - comp.x, r - parentLeft - comp.x);
            for (int i = startEnd.x; i <= startEnd.y; i++)
            {
                if ((this as Component) & comp.sweep[i].component + new Point(parentLeft + comp.x, parentTop + comp.y))
                    ZBufferUpdate(comp.sweep[i].component, parentTop + comp.y, parentLeft + comp.x);
            }
            FillBuffer(parentLeft + comp.l, parentTop + comp.t, comp.height, comp.width, comp.madeOf, comp.z);

        }
        private void FillBuffer(int x, int y, int h, int w, Material m, int zLevle)
        {
            Point start = bounds.ToTopLeft(x, y);
            Point end = bounds.ToTopLeft(x + w, y - h);
            for (int i = Math.Min(end.y, start.y); i < Math.Max(start.y, end.y); i++)
            {
                for (int j = Math.Min(end.x, start.x); j < Math.Max(start.x, end.x); j++)
                {

                    if (updated[j][i] < zLevle)
                    {
                        buff1[j][i] = texture[m];
                        updated[j][i] = zLevle;
                    }
                }
            }
        }
        private void FlushBuffer()
        {
            Clear();
            Console.Clear();
            foreach (List<char> c in buff1)
                VirtualConsoleAddLine(new string(c.ToArray()));
            Screen_Change(this, EventArgs.Empty);
        }
        private void DrawPaths()
        {
            this.controls.Where(i => i.Value.GetType() == typeof(Path)).ToList().ForEach((i) =>
            {
                LinearInterpolator pol = (i.Value as Path).Poly;
                for (int j = bounds.l; j < bounds.r; j++)
                {
                    FillBuffer(j, ToInt32(pol.ValueForX(j) + Abs(pol.DerivativeForX(j))) + 2, Abs(ToInt32(pol.DerivativeForX(j)) * 2) + 4, 1, i.Value.madeOf, i.Value.z);
                }
            });
        }



    }
}
