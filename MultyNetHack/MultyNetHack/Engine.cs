using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
namespace MultyNetHack
{
    class Engine : Component
    {

        List<List<char>> buff1;
        Dictionary<Material, char> texture ;
        List<List<bool>> updated;
        Timer renderTimer, updateTimer;
        MyPoint centerLoc;
        int gh;
        bool showHelp;
        public Engine(int width, int height)
        {
            this.width = width;
            this.height = height;
            name = "root";
            top = 0;
            left = 0;
            depht = 0;
            parent = null;
            keys = new List<string>(100);
            controls = new Dictionary<string, Component>(100);
            centerLoc = new MyPoint(0, 0);
            centerLoc.SizeOfScreen(20, 20);

            Init();
            
            renderTimer = new Timer(100); // update ~60Hz
            updateTimer = new Timer(3); // update ~30HZ
            renderTimer.Enabled = true;
            updateTimer.Enabled = true;
            updateTimer.Elapsed += OnUpdateFrame;
            renderTimer.Elapsed += OnRenderFrame;
            renderTimer.Start();
            updateTimer.Start();
            
        }

        private void Init()
        {
            //Console.SetWindowSize(width, 2*height);
            Console.BufferHeight = 2 * height;
            width = Console.BufferWidth - 2;

            madeOf = Material.Darknes;
            buff1 = new List<List<char>>(centerLoc.height);
            for (int j = 0; j <centerLoc.height; j++)
            {
                buff1.Add(new List<char>(centerLoc.width));
                buff1[j] = new List<char>(centerLoc.width);
                if (j == 0)
                {
                    for (int i = 0; i < centerLoc.width; i++)
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
            texture.Add(Material.Wall, '+');
            texture.Add(Material.Watter, '}');
            texture.Add(Material.Darknes, ' ');
        }
        private void ZbufferUpdate(Component comp, int parentTop, int parentLeft)
        {
            
            foreach (string s in comp.keys)
                ZbufferUpdate(comp.controls[s], parentTop + comp.top, parentLeft + comp.left);    
            if (comp & centerLoc)
                FillBuffer(parentTop + comp.top, parentLeft + comp.left, comp.height, comp.width, comp.madeOf);

        }
        private void FlushBuffer()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.Clear();
            
            foreach (List<char> c in buff1)
                Console.WriteLine(c.ToArray());
            //Console.MoveBufferArea(0, top, width, top, 0, 0);
            if (showHelp)
            {
                Console.WriteLine("Move h/j/k/l");
                Console.WriteLine("Generate room g");
                Console.WriteLine("Curent Location:({0},{1})",centerLoc.x,centerLoc.y );
                
                Console.WriteLine("There are {0} rooms!", gh);
                foreach(string key in keys)
                {
                    Component c = controls[key];
                    Console.WriteLine("{0} is located ({1},{2}) - it's size is {3}x{4} - On The screen? {5} !",new object[] { c.name,c.left,c.top,c.width,c.height, c&centerLoc});
                }
            }
            renderTimer.Start();
        }
        private void FillBuffer(int top, int left, int h, int w, Material m)
        {
            MyPoint start = centerLoc.GetStandardCoordinat(left,top);
            MyPoint end = centerLoc.GetStandardCoordinat(left + w, top - h);
            for (int i = start.y; i < end.y; i++)
            {
                for (int j = start.x; j <end.x; j++)
                {

                    if (!updated[i][j])
                    {
                        buff1[i][j] = texture[m];
                        updated[i][j] = true;
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
                    Room r = new Room("room" + gh);
                    this.Insert(r);
                    (this.controls["room" + gh] as Room).generatRandom();
                    gh++;
                    break;
                case 'h':
                    centerLoc.x -= 1;
                    break;
                case 'j':
                    centerLoc.y -= 1;
                    break;
                case 'k':
                    centerLoc.y += 1;
                    break;
                case 'l':
                    centerLoc.x += 1;
                    break;
                case 'H':
                    centerLoc.x -= 10;
                    break;
                case 'J':
                    centerLoc.y -= 10;
                    break;
                case 'K':
                    centerLoc.y += 10;
                    break;
                case 'L':
                    centerLoc.x += 10;
                    break;
                case '?':
                    showHelp = !showHelp;
                    break;
            }
                

            updateTimer.Start();
        }
        private void OnRenderFrame(object sender, ElapsedEventArgs e)
        {
            renderTimer.Stop();
            updated = new List<List<bool>>(centerLoc.height);
            for (int i = 0; i < centerLoc.height; i++)
            {

                updated.Add(new List<bool>(centerLoc.width));
                if (i == 0)
                {
                    for (int j = 0; j < centerLoc.width; j++)
                    {
                        updated[i].Add(false);
                    }
                }
                else
                {
                    updated[i].AddRange(updated[0]);
                }
            }
            ZbufferUpdate(this, 0, 0);
            FillBuffer(centerLoc.topBound, centerLoc.leftBound, centerLoc.height, centerLoc.width, Material.Darknes);
            FlushBuffer();
        }

    }
}
