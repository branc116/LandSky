using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;
using static System.Convert;
using static System.Math;
namespace MultyNetHack
{
    class Engine
    {
        

        List<List<char>> buff1;
        Dictionary<Material, char> texture;

        List<List<int>> updated;
        System.Timers.Timer renderTimer, updateTimer;
        public Point center;
        Size SceenSize;
        public Root root;
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
        int curRoomList, curMessage, curPath;
        double renderInterval;
        Queue<string> Message;
        List<string> LogMessages;
        TextShown currentText;
        PrintStuff PrintStuf;
        Controls Control;
        DateTime HelpNewest, ListRoomsNewest, ListPathsNewest, MessageNewest, DebugNewest;


        public void PlayMajor(int tempo)
        {
            
            int dur = tempo;
            Console.Beep(ToInt32(440 * Pow(2, (double)12 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)24 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)23 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)21 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)19 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)17 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)16 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, (double)14 / 12)), dur);
            Console.Beep(ToInt32(440 * Pow(2, 12 / 12)), dur);
        }
        public Engine(int width, int height)
        {
            SceenSize = new Size(width, height);
            Console.WindowWidth = width +1 ;
            Console.BufferWidth = width +1;
            Console.WindowHeight = height * 3;
            PlayMajor(2);
            center = new Point(0, 0);
            root = new Root();
            Init();
            _r = new Rectangle(0, 0, 0, 0);
            renderInterval = 50;
            if (Console.CursorVisible)
                try
                {
                    Console.CursorVisible = false;
                }
                catch { }
            Message = new Queue<string>();
            LogMessages = new List<string>();
            PrintStuf = new PrintStuff();
            Control = new Controls();
            HelpNewest = ListPathsNewest = ListRoomsNewest = MessageNewest = DebugNewest = DateTime.Now;
            

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
            int startSec = DateTime.Now.Second + DateTime.Now.Minute * 60;
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
            long afterDec = DateTime.Now.Ticks - startTime, consLeft = 0, consWriteHashtag = 0, newRoom = 0, insertRoom = 0, genRoom = 0;
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
            s += " Time taken: " + (DateTime.Now.Second + DateTime.Now.Minute * 60 - startSec) + "s \n ticks/s: " + startTicks / Math.Max(1, (DateTime.Now.Second + DateTime.Now.Minute * 60 - startSec)) + "\n";
#endif
            Message.Enqueue(s);


            renderTimer.Interval = renderInterval;
            gh++;

        }
        private void Init()
        {

            Player p = new Player(center.x, center.y);
            buff1 = new List<List<char>>(SceenSize.height);
            for (int j = 0; j < SceenSize.height; j++)
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

            Point startEnd = comp.GetStartEndEnter(bounds.l - parentLeft - comp.x, bounds.r - parentLeft - comp.x);
            for (int i = startEnd.x; i <= startEnd.y; i++)
            {
                if (comp.sweep[i].component + new Point(parentLeft + comp.x, parentTop + comp.y) & bounds)
                    ZBufferUpdate(comp.sweep[i].component, parentTop + comp.y, parentLeft + comp.x);
            }
            FillBuffer(parentLeft + comp.l, parentTop + comp.t, comp.height, comp.width, comp.madeOf, comp.z);
        }
        private void FlushBuffer()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.Clear();

            foreach (List<char> c in buff1)
                Console.WriteLine(c.ToArray());
            switch (currentText) {
                case TextShown.Help:
                    PrintStuf.PrintHelp(HelpNewest);
                    break;
                case TextShown.Message:
                    PrintStuf.PrintMessage(curMessage, LogMessages, Message, MessageNewest);
                    break;
                case TextShown.ListRooms:
                    PrintStuf.ListRoomsinComponent(Console.WindowHeight - Console.CursorTop - 15, curRoomList, root, ListRoomsNewest);
                    break;
                case TextShown.ListPaths:
                    PrintStuf.ListPathsinComponent(curPath, root, ListPathsNewest);
                    break;
                case TextShown.Debug:
                    PrintStuf.PrintDebug(this, DebugNewest);
                    break;

            }
            renderTimer.Start();
        }
        private void FillBuffer(int x, int y, int h, int w, Material m, int zLevle)
        {
            Point start = bounds.ToTopLeft(x, y);
            Point end = bounds.ToTopLeft(x + w, y - h);
            for (int i = Math.Min(end.y, start.y); i < Math.Max(start.y, end.y); i++)
            {
                for (int j = Math.Min(end.x, start.x); j < Math.Max(start.x, end.x); j++)
                {

                    if (updated[j][i] < zLevle )
                    {
                        buff1[j][i] = texture[m];
                        updated[j][i] = zLevle;
                    }
                }
            }
        }
        private void OnUpdateFrame(object sender, ElapsedEventArgs e)
        {

            updateTimer.Stop();
            char c = Console.ReadKey(true).KeyChar;
            Comands curentComand;
            try
            {
                curentComand = Control.KeyMap[c];
            }
            catch
            {
                updateTimer.Start();
                return;
            }
            switch (curentComand)
            {
                case Comands.GenerateOneRoom:
                    GenerateRooms(1);
                    ListRoomsNewest = DateTime.Now;
                    MessageNewest = DateTime.Now;
                    break;
                case Comands.GenerateALotOfRooms:
                    GenerateRooms(1000);
                    ListRoomsNewest = DateTime.Now;
                    MessageNewest = DateTime.Now;
                    break;
                case Comands.GenerateRandomPath:
                    Path path = new Path("Path" + root.numOfPaths);
                    path.generatePathThrueRandomChildren(root);
                    root.Insert(path);
                    ListPathsNewest = DateTime.Now;
                    break;
                case Comands.Left:
                    center.x -= 1;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.Down:
                    center.y -= 1;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.Up:
                    center.y += 1;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.Right:
                    center.x += 1;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.TenStepsLeft:
                    center.x -= 10;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.TenStepsDown:
                    center.y -= 10;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.TenStepsUp:
                    center.y += 10;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.TenStepsRight:
                    center.x += 10;
                    DebugNewest = DateTime.Now;
                    break;
                case Comands.ScrollLeft:
                    if (currentText == TextShown.ListRooms)
                    {
                        ListRoomsNewest = DateTime.Now;
                        curRoomList--;
                        if (curRoomList < 0)
                            curRoomList = root.numOfRooms - 1;
                    } else if (currentText == TextShown.ListPaths)
                    {
                        ListPathsNewest = DateTime.Now;
                        curPath--;
                        if (curPath < 0)
                            curPath = root.numOfPaths - 1;
                    } else if (currentText == TextShown.Message)
                    {
                        MessageNewest = DateTime.Now;
                        curMessage--;
                        if (curMessage < 0)
                            curMessage = LogMessages.Count - 1;
                    }
                    break;
                case Comands.ScrollRight:
                    if (currentText == TextShown.ListRooms)
                    {
                        ListRoomsNewest = DateTime.Now;
                        curRoomList++;
                        curRoomList %= root.numOfRooms;
                    }
                    else if (currentText == TextShown.ListPaths)
                    {
                        ListPathsNewest = DateTime.Now;
                        curPath++;
                        curPath = curPath % root.numOfPaths;
                    }
                    else if (currentText == TextShown.Message)
                    {
                        MessageNewest = DateTime.Now;
                        if (LogMessages.Count != 0)
                        {
                            curMessage++;
                            curMessage %= LogMessages.Count;
                        }
                    }
                    break;
                case Comands.DequeMessage:
                    if (Message.Count > 0)
                    {
                        LogMessages.Add(Message.Dequeue());
                        MessageNewest = DateTime.Now;
                    }
                    break;
                case Comands.TabRight:
                    currentText++;
                    if (currentText == TextShown.Max)
                        currentText = 0;
                    break;
                case Comands.TabLeft:
                    if (currentText != 0)
                        currentText--;
                    else
                        currentText = TextShown.Max - 1;
                    break;
            }


            updateTimer.Start();
        }
        private void OnRenderFrame(object sender, ElapsedEventArgs e)
        {
            renderTimer.Stop();
            updated = new List<List<int>>(SceenSize.height);
            for (int i = 0; i < SceenSize.height; i++)
            {

                updated.Add(new List<int>(SceenSize.width));
                if (i == 0)
                {
                    for (int j = 0; j < SceenSize.width; j++)
                    {
                        updated[i].Add(-1);
                    }
                }
                else
                {
                    updated[i].AddRange(updated[0]);
                }
            }
            FillBuffer(center.x, center.y, 1, 1, Material.Player, 15);
            DrawPaths();
            ZBufferUpdate(root, 0, 0);
            FillBuffer(bounds.l, bounds.t, SceenSize.height, SceenSize.width, root.madeOf, 0);
            FlushBuffer();
        }
        private void DrawPaths() {
            root.controls.Where(i => i.Value.GetType() == typeof(Path)).ToList().ForEach((i) =>
            {
                LinearInterpolator pol = (i.Value as Path).Poly;
                for (int j = bounds.l; j < bounds.r;j++)
                {
                    FillBuffer(j, Convert.ToInt32( pol.ValueForX(j) + Math.Abs(pol.DerivativeForX(j)))+2,Math.Abs( Convert.ToInt32( pol.DerivativeForX(j) )*2) +4, 1, i.Value.madeOf, i.Value.z);
                }
            });
        }
        
    }
}
