using LandSky.MyEnums;
using LandSky.MyMath;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LandSky.Components
{
    [JsonObject]
    /// <summary>
    /// Everything should be extended from this
    /// </summary>
    public abstract class Component : IEnumerable, IDisposable
    {
        public int LocalX
        {
            get
            {
                return LocalBounds == null ? 0 : LocalBounds.X;
            }
            set
            {
                LocalBounds.X = value;
            }
        }

        public int LocalY
        {
            get
            {
                return LocalBounds == null ? 0 : LocalBounds.Y;
            }
            set
            {
                LocalBounds.Y = value;
            }
        }

        public int GlobalX => IsRoot ? 0 : LocalX + (Parent == null ? 0 : Parent.LocalX);
        public int GlobalY => IsRoot ? 0 : LocalY + (Parent == null ? 0 : Parent.LocalY);
        public int Height => LocalBounds == null ? 0 : LocalBounds.Height;
        public int Width => LocalBounds == null ? 0 : LocalBounds.Width;
        public int NumOfRooms => Controls.Count(I => I.Value.GetType() == typeof(Room));
        public int NumOfWalls => Controls.Count(I => I.Value.GetType() == typeof(Wall));
        public int NumOfPaths => Controls.Count(I => I.Value.GetType() == typeof(Path));
        public Rectangle LocalBounds => Bounds;
        public Rectangle GlobalBounds => this.IsRoot ? new Rectangle(0, 0, 0, 0) : (LocalBounds == null ? new Rectangle(0, 0, 0, 0) : (Parent == null ? LocalBounds : LocalBounds + Parent.GlobalBounds));

        public bool IsRoot { get; set; }
        public bool IsPassable { get; set; }
        public int ZValue { get; set; }
        public string Name { get; }
        public Material MadeOf { get; set; }
        public Rectangle Bounds { get; set; }
        public Random Rand { get; set; }
        public Component Parent { get; set; }
        public Dictionary<string, Component> Controls { get; set; }

        public Component(string Name)
        {
            Controls = new Dictionary<string, Component>();
            Rand = new Random();
            this.Name = Name;
            IsRoot = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool Dispose)
        {
            if (Dispose)
            {
                this.Parent.Controls.Remove(this.Name);
                Controls = null;
                Parent = null;
                GC.SuppressFinalize(this);
            }
        }

        public void Insert(Component C)
        {
            if (C.Name == null) throw new Exception("Component must have a Name");
            this.Controls.Add(C.Name, C);
            C.Parent?.Controls.Remove(C.Name);
            C.Parent = this;
        }

        public void Delete(string name)
        {
            using (Component C = Controls[name])
            {
                foreach (var Comp in C.Controls)
                {
                    Comp.Value.Dispose();
                }
            }
            Controls.Remove(name);
        }

        public Component GetComponentOnLocation(int X, int Y)
        {
            Point P = new Point(X, Y);
            return GetComponentOnLocation(P);
        }

        public Component GetComponentOnLocation(Point Point)
        {
            var Intersect = this.Controls.Where(
                I =>
                {
                    if (I.Value.GetType() == typeof(Wall) || I.Value.GetType() == typeof(Room))
                        return Point & I.Value.GlobalBounds;
                    if (I.Value.GetType() == typeof(Path))
                        return ((Path)I.Value).IsOnPath(Point);
                    return false;
                })
                .OrderBy(N => -N.Value.ZValue);
            return Intersect.Any() ? Intersect.First().Value.GetComponentOnLocation(Point) : this;
        }

        public async Task GenerateRandomPaths(int N, IEnumerable<string> Names)
        {
            List<Path> Range = new List<Path>();
            Task[] Tasks = new Task[N];
            int I = 0;
            foreach (string name in Names)
            {
                Tasks[I] = Task.Run(() =>
                {
                    Path P = new Path(name);
                    P.GeneratePathThrueRandomChildren(this);
                    Range.Add(P);
                });
                I++;
            }
            Task.WaitAll(Tasks);
            foreach (Task task in Tasks)
            {
                await task;
            }
            foreach (Path Path in Range)
            {
                this.Insert(Path);
            }
        }

        public async Task GenerateRandomPaths(int N)
        {
            List<string> Names = new List<string>();
            for (int I = 0; I < N; I++)
            {
                Names.Add($"Room{I}-{Guid.NewGuid().ToString().Substring(0, 7)}");
            }
            await GenerateRandomPaths(N, Names);
        }

        public async Task GenerateRandomRooms(int N, IReadOnlyList<string> Names)
        {
            Task[] Tasks = new Task[4];
            List<List<Component>> Quad = new List<List<Component>>();
            Quadrant[] Quads = new Quadrant[4] { Quadrant.First, Quadrant.Second, Quadrant.Third, Quadrant.Fourth };
            int I = 0;
            int Pool = 0;
            int n = N / 4;
            int Made = 0;
            foreach (Quadrant Q in Quads)
            {
                Tasks[I] = Task.Run(() =>
                {
                    int Id = Pool++;
                    if (N % 4 > 0)
                    {
                        n++;
                        N--;
                    }
                    List<Component> Rooms = new List<Component>();
                    for (int J = 0; J < n; J++)
                    {
                        int Breakint = 0;
                        Room R = new Room($"{Names[J]}-{J}-{Quads[Id]}");
                        bool repeat = true;
                        while (repeat)
                        {
                            repeat = false;
                            Breakint++;
                            R.GenerateRandom(Quads[Id], 500);
                            if ((CheckCollision(Rooms, R) || CheckCollision(this.Controls, R)) && Breakint < 100)
                                repeat = true;
                        }
                        Made++;
                        if (Breakint < 100)
                            Rooms.Add(R);
                    }
                    Quad.Add(Rooms);
                });
                I++;
            }
            Task.WaitAll(Tasks);
            I = 0;
            foreach (List<Component> Comps in Quad)
            {
                await Tasks[0];
                foreach (Room Room in Comps)
                {
                    if (Room.LocalBounds.Width > 0 && Room.LocalBounds.Height > 0)
                    {
                        Room.ZValue = this.ZValue + 3;
                        Room.GenerateWall();
                        this.Insert(Room);
                    }
                }
            }
        }

        public async Task GenerateRandomRooms(int N)
        {
            List<string> Names = new List<string>();
            for (int I = 0; I < N; I++)
            {
                Names.Add($"Room-{Guid.NewGuid().ToString().Substring(0, 7)}");
            }
            await GenerateRandomRooms(N, Names);
        }

        public bool CheckCollision(IEnumerable<Component> Components, Component NewComponent)
        {
            return Components.Any(N => N.LocalBounds & NewComponent.LocalBounds && N.ZValue == NewComponent.ZValue);
        }

        public bool CheckCollision(Dictionary<string, Component> Components, Component NewComponent)
        {
            return Components.Any(N => N.Value.LocalBounds & NewComponent.LocalBounds && N.Value.ZValue == NewComponent.ZValue);
        }

        public async Task<string> GimeJSON()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return JsonConvert.SerializeObject(this, this.GetType(), Formatting.Indented, new JsonSerializerSettings()
                    {
                        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                        CheckAdditionalContent = true,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        MaxDepth = 5,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                        StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                    });
                }
                catch
                {
                    throw;
                }
            }
            );
        }

        public async Task SaveStateToDisc(string FileName)
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    File.WriteAllText(FileName, await GimeJSON(), System.Text.Encoding.UTF8);
                }
                catch
                {
                    throw;
                }
            }).ContinueWith((i) =>
            {
                //finished
            });
        }

        public static async Task<Component> GimeComponentFromJSON(string FileName)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Component>(File.ReadAllText(FileName, System.Text.Encoding.UTF8)));
        }

        //check for intersection in two Components
        public static bool operator &(Component One, Component Two)
        {
            return (One.LocalBounds.LeftBound <= Two.LocalBounds.RightBound && One.LocalBounds.RightBound >= Two.LocalBounds.LeftBound &&
                One.LocalBounds.TopBound >= Two.LocalBounds.BottomBound && One.LocalBounds.BottomBound <= Two.LocalBounds.TopBound);
        }

        public static bool operator &(Component One, Player Two)
        {
            return (One.LocalBounds.LeftBound <= Two.LocalBounds.RightBound && One.LocalBounds.RightBound >= Two.LocalBounds.LeftBound &&
                One.LocalBounds.TopBound >= Two.LocalBounds.BottomBound && One.LocalBounds.BottomBound <= Two.LocalBounds.TopBound);
        }

        public static bool operator &(Component One, Point Two)
        {
            return (One.LocalBounds.TopBound >= Two.Y && One.LocalBounds.BottomBound < Two.Y && One.LocalBounds.LeftBound <= Two.X && One.LocalBounds.RightBound >= Two.X);
        }

        public static bool operator &(Component One, Rectangle Two)
        {
            return (One.LocalBounds.LeftBound < Two.RightBound && One.LocalBounds.RightBound > Two.LeftBound &&
                    One.LocalBounds.TopBound > Two.BottomBound && One.LocalBounds.BottomBound < Two.TopBound);
        }

        public static bool operator ==(Component One, Component Two)
        {
            return One?.LocalBounds?.TopBound == Two?.LocalBounds?.TopBound &&
                   One?.LocalBounds?.BottomBound == Two?.LocalBounds?.BottomBound &&
                   One?.LocalBounds?.LeftBound == Two?.LocalBounds?.LeftBound &&
                   One?.LocalBounds?.RightBound == Two?.LocalBounds?.RightBound &&
                   One?.Name == Two?.Name;
        }

        public static bool operator !=(Component One, Component Two)
        {
            try
            {
                return !(One == Two);
            }
            catch
            {
                return false;
            }
        }

        public static Rectangle operator +(Component C, Point P)
        {
            return new Rectangle(C.LocalBounds.TopBound + P.Y, C.LocalBounds.RightBound + P.X, C.LocalBounds.BottomBound + P.Y, C.LocalBounds.LeftBound + P.X);
        }

        public override bool Equals(object Obj) => Obj is Component ? Obj as Component == this : false;

        public bool Equals(Component Other)
        {
            return ZValue == Other.ZValue &&
                IsRoot == Other.IsRoot &&
                Equals(Controls, Other.Controls) &&
                Equals(Parent, Other.Parent) &&
                MadeOf == Other.MadeOf &&
                string.Equals(Name, Other.Name) &&
                IsPassable == Other.IsPassable &&
                Equals(Bounds, Other.Bounds) &&
                Equals(Rand, Other.Rand);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int HashCode = ZValue;
                HashCode = (HashCode * 397) ^ IsRoot.GetHashCode();
                HashCode = (HashCode * 397) ^ (Controls?.GetHashCode() ?? 0);
                HashCode = (HashCode * 397) ^ (Parent?.GetHashCode() ?? 0);
                HashCode = (HashCode * 397) ^ (int)MadeOf;
                HashCode = (HashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                HashCode = (HashCode * 397) ^ IsPassable.GetHashCode();
                HashCode = (HashCode * 397) ^ (Bounds?.GetHashCode() ?? 0);
                HashCode = (HashCode * 397) ^ (Rand?.GetHashCode() ?? 0);
                return HashCode;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Controls).GetEnumerator();
        }
    }
}