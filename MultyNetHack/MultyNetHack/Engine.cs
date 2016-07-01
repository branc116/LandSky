using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace MultyNetHack
{
    class Engine
    {


        List<List<char>> buff1;
        Dictionary<Material, char> texture ;
        List<List<bool>> updated;
        System.Timers.Timer renderTimer, updateTimer;
        Point center;
        Size SceenSize;
        Root root;
        int l
        {
            get
            {
                return center.x - SceenSize.width / 2;
            }
        }
        int r
        {
            get
            {
                return center.x + SceenSize.width / 2;
            }
        }
        int b
        {
            get
            {
                return center.y - SceenSize.height / 2;
            }
        }
        int t
        {
            get
            {
                return center.y + SceenSize.height / 2;
            }
        }
        Rectangle _r;
        Rectangle bounds
        {
            get
            {
                _r.b = b;
                _r.l = l;
                _r.r = r;
                _r.t = t;
                return _r;
            }
        }
        int gh;
        bool showHelp;
        int from;
        double renderInterval;
        Queue<String> Message;
        public Engine(int width, int height)
        {
            
            SceenSize = new Size(width, height);
            center = new Point(0, 0);
            root = new Root();
            Init();
            _r = new Rectangle(0, 0, 0, 0);
            renderInterval = 100;
            Console.CursorVisible = false;
            Message = new Queue<string>();

            renderTimer = new System.Timers.Timer(renderInterval); // update ~60Hz
            updateTimer = new System.Timers.Timer(3); // update ~30HZ
            renderTimer.Enabled = true;
            updateTimer.Enabled = true;
            updateTimer.Elapsed += OnUpdateFrame;
            renderTimer.Elapsed += OnRenderFrame;
            renderTimer.Start();
            updateTimer.Start();
            
        }
        private void GenerateRooms(int n)
        {
#if DEBUG
            long startTicks = DateTime.Now.Ticks;
            long startTime = DateTime.Now.Ticks;
            int startSec = DateTime.Now.Second + DateTime.Now.Minute*60;
#endif
            renderTimer.Interval = 200000;
            Thread.Sleep(Convert.ToInt32(renderInterval * 2));
            Console.Clear();
            Console.WriteLine("GENERATING ROOMS w8 4 it!!!");
            int maxLeft = 50;
            Console.Write("|");
            Console.CursorLeft = maxLeft;
            Console.Write("|");
            int genRooms = n;
#if DEBUG
            long afterDec = DateTime.Now.Ticks- startTime  , consLeft = 0, consWriteHashtag = 0, newRoom = 0, insertRoom = 0, genRoom = 0;
#endif
            for (int i = 0; i < genRooms; i++)
            {
#if DEBUG
                startTime = DateTime.Now.Ticks;
#endif
                Console.CursorLeft = (int)(((float)i / genRooms) * (maxLeft - 1)) + 1;
#if DEBUG
                consLeft += (DateTime.Now.Ticks - startTime);
                startTime = DateTime.Now.Ticks;
#endif
                Console.Write("#");
#if DEBUG
                consWriteHashtag += (DateTime.Now.Ticks - startTime);
                startTime = DateTime.Now.Ticks;
#endif
                Room R = new Room("room" + gh + '.' + i);
#if DEBUG
                newRoom += (DateTime.Now.Ticks - startTime);
                startTime = DateTime.Now.Ticks;
#endif
                root.Insert(R);
#if DEBUG
                insertRoom += (DateTime.Now.Ticks - startTime);
                startTime = DateTime.Now.Ticks;
#endif
                R.GeneratRandom();
#if DEBUG
                genRoom += (DateTime.Now.Ticks - startTime);
#endif
            }
            string s = "Added " + genRooms.ToString() + " new rooms\n";
#if DEBUG
            startTicks = DateTime.Now.Ticks - startTicks;
            string debugL = "----DEBUG---- \n overallTicks: " + startTicks + " \n after declarations: " + afterDec + " \n console Left Comand: " + consLeft + " \n console Write #: " + consWriteHashtag + " \n new Room: " + newRoom + "  \n insert Room: " + insertRoom + " \n generate Room: " + genRoom + "\n";
            s += debugL;
            s += " Time taken: " + (DateTime.Now.Second + DateTime.Now.Minute*60 - startSec ) + "s \n ticks/s: " + startTicks / Math.Max(1, (DateTime.Now.Second + DateTime.Now.Minute*60 - startSec)) +"\n";
#endif
            Message.Enqueue(s);


            renderTimer.Interval = renderInterval;
            gh++;

        }
        private void Init()
        {

            Player p = new Player(center.x, center.y);
            buff1 = new List<List<char>>(SceenSize.height);
            for (int j = 0; j <SceenSize.height; j++)
            {
                buff1.Add(new List<char>(SceenSize.width));
                buff1[j] = new List<char>(SceenSize.width);
                if (j == 0)
                {
                    for (int i = 0; i < SceenSize.width; i++)
                    {
                        buff1[j].Add(new char());
                    }
                }
                else
                    buff1[j].AddRange(buff1[0]);
            }
            loadTexture();
        }
        private void loadTexture()
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
        private void ZBufferUpdate(Component comp, int parentTop, int parentLeft)
        {
            
            Point startEnd = comp.GetStartEndEnter(bounds.l -parentLeft - comp.x, bounds.r  -parentLeft - comp.x);
            for (int i = startEnd.x; i <= startEnd.y; i++)
            {
                if (comp.sweep[i].component + new Point(parentLeft + comp.x, parentTop + comp.y) & bounds)
                    ZBufferUpdate(comp.sweep[i].component, parentTop + comp.y, parentLeft + comp.x);
            }
            FillBuffer(parentLeft + comp.l, parentTop + comp.t, comp.height, comp.width, comp.madeOf);
        }
        private void FlushBuffer()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.Clear();
            
            foreach (List<char> c in buff1)
                Console.WriteLine(c.ToArray());
            if (showHelp)
            {
                if (Message.Count > 0)
                {

                    Console.WriteLine("New Message");
                    Console.WriteLine(new string('-', Console.WindowWidth));
                    Console.WriteLine();
                    try
                    {
                        Console.WriteLine(Message.Peek());
                    }
                    catch
                    {
                        Console.WriteLine("w8");
                    }
                    Console.WriteLine();
                    Console.WriteLine(new string('-', Console.WindowWidth));
                    Console.WriteLine();
                    Console.WriteLine("Press 'd' to dismiss messsage");
                }
                else
                {
                    Console.WriteLine("Move h/j/k/l");
                    Console.WriteLine("Generate room g");
                    Console.WriteLine("Curent Location:({0},{1}) and That location is ocupied by: {2}!", center.x, center.y, root.GetComponentOnLocation(center).name);

                    Console.WriteLine("There are {0} rooms!", root.keys.Count);
                    Console.WriteLine("Shown rooms {0} - {1}.", new object[2] { from, 20 + from, });
                    for (int i = from; i < Math.Min(root.controls.Count, (20 + from)); i++)
                    {
                        string key = root.keys[i];
                        Component c = root.controls[key];

                        Console.WriteLine("{0} is located ({1},{2}) - it's size is {3}x{4} - On The screen? {5} !", new object[] { c.name, c.x, c.y, c.width, c.height, c & bounds });
                    }
                }
            }else
            {
                Console.WriteLine("Press '?' for help");
            }
            renderTimer.Start();
        }
        private void FillBuffer(int x, int y, int h, int w, Material m)
        {
            Point start = bounds.ToTopLeft(x ,y);
            Point end = bounds.ToTopLeft(x + w, y- h);
            for (int i = Math.Min(end.y, start.y); i < Math.Max(start.y, end.y); i++)
            {
                for (int j = Math.Min(end.x, start.x); j < Math.Max(start.x, end.x); j++)
                {

                    if (!updated[j][i])
                    {
                        buff1[j][i] = texture[m];
                        updated[j][i] = true;
                    }
                }
            }
        }
        private void OnUpdateFrame(object sender, ElapsedEventArgs e)
        {
            
            updateTimer.Stop();
            char c = Console.ReadKey().KeyChar;

            switch (c)
            {
                case 'g':
                    GenerateRooms(1);
                    break;
                case 'G':
                    GenerateRooms(1000);
                    break;
                case 'h':
                    center.x -= 1;
                    break;
                case 'j':
                    center.y -= 1;
                    break;
                case 'k':
                    center.y += 1;
                    break;
                case 'l':
                    center.x += 1;
                    break;
                case 'H':
                    center.x -= 10;
                    break;
                case 'J':
                    center.y -= 10;
                    break;
                case 'K':
                    center.y += 10;
                    break;
                case 'L':
                    center.x += 10;
                    break;
                case '?':
                    showHelp = !showHelp;
                    break;
                case 'v':
                    from = Math.Max(from - 1, 0);
                    break;
                case 'V':
                    from++;
                    break;
                case 'd':
                    if (Message.Count>0)
                        Message.Dequeue();
                    break;
            }
                

            updateTimer.Start();
        }
        private void OnRenderFrame(object sender, ElapsedEventArgs e)
        {
            renderTimer.Stop();
            updated = new List<List<bool>>(SceenSize.height);
            for (int i = 0; i < SceenSize.height; i++)
            {

                updated.Add(new List<bool>(SceenSize.width));
                if (i == 0)
                {
                    for (int j = 0; j < SceenSize.width; j++)
                    {
                        updated[i].Add(false);
                    }
                }
                else
                {
                    updated[i].AddRange(updated[0]);
                }
            }
            ZBufferUpdate(root, 0, 0);
            FillBuffer(bounds.l, bounds.t, SceenSize.height, SceenSize.width, root.madeOf);
            FlushBuffer();
        }

    }
}
