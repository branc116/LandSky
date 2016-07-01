using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
    class PrintStuff
    {
        protected string OverallString;
        DateTime newestHelp, newestMessage, newestPath, newestRoom, newestDebug;
        TempConsole Help, Message, Path, Room, Debug;
        public PrintStuff() {
            OverallString = string.Empty;
            newestHelp = new DateTime(0);
            newestMessage = new DateTime(0);
            newestPath = new DateTime(0);
            newestRoom = new DateTime(0);
            newestDebug = new DateTime(0);
            Help = new TempConsole();
            Message = new TempConsole();
            Path = new TempConsole();
            Room = new TempConsole();
            Debug = new TempConsole();
        }
        public void PrintHelp()
        {
            Help.PrintLine();
            Help.PrintCenter("This is my version of NetHack, Multyplayer version");
            Help.PrintLine();
            Help.OverallStringAddLine("Comands: " );
            Controls ccc = new Controls();
            bool enter = false;
            foreach(char c in ccc.KeyMap.Keys)
            {
                if (enter)
                    Help.OverallStringLeft = Console.WindowWidth / 2;
                Help.OverallStringAdd(string.Format( "'{0}' -> {1}", c, ccc.KeyMap[c]));
                if(enter)
                    Help.OverallStringAddLine();
                enter = !enter;
            }
            Help.OverallStringAddLine();
            Help.PrintLine();
        }

        public void PrintMessage(int curMessage, List<string> logMessages, Queue<string> message)
        {
            Message.PrintLine();
            Message.PrintCenter("Messages");
            Message.OverallStringAddLine(string.Format( "There are {0} out of {1} new messages", message.Count, logMessages.Count + message.Count));
            Message.PrintLine();
            if (message.Count > 0)
            {
                Message.OverallStringAddLine(string.Format( "Showing unread messages first. Press '{0}' to dissmiss", 'd'));
                Message.PrintLine();
                Message.OverallStringAddLine(message.Peek());
            }else if(logMessages.Count>0)
            {
                Message.OverallStringAddLine(string.Format("Showing {0}. message", curMessage + 1));
                Message.PrintLine();
                Message.OverallStringAddLine(logMessages[curMessage]);
            }
            Message.PrintLine();

        }
        public void ListRoomsinComponent(int n, int start, Component cmp)
        {

            Room.PrintLine();
            Room.PrintCenter("Rooms");
            Room.OverallStringAddLine(string.Format("There are {0} rooms in {1}", cmp.numOfRooms, cmp.name));
            Room.OverallStringAddLine(string.Format( "Showing Rooms {0} to {1}", start, Math.Min(cmp.numOfRooms, start +n)));
            Room.PrintLine();
            if (cmp.numOfRooms == 0)
                return;
            while (n > 0)
            {
                Component comp = cmp.controls[cmp.keys[start]];
                if (comp.GetType() == typeof(Room))
                {
                    n--;
                    Room.OverallStringAddLine(String.Format("{0}: {1} => Location ({2},{3}), Size({4},{5}), zBuffer={6}", new object[] { start, comp.name, comp.x, comp.y, comp.width, comp.height, comp.z }));
                }
                start++;
                if (start >= cmp.keys.Count)
                    start = 0;
            }
            Room.PrintLine();
        }
        public void ListPathsinComponent(int curPath, Component cmp)
        {
            Path.PrintLine();
            Path.PrintCenter("Paths");
            Path.OverallStringAddLine(string.Format("There are {0} paths in {1}", cmp.numOfPaths, cmp.name));
            Path.OverallStringAddLine(string.Format("Showing {0}. Path", curPath));
;           Path.PrintLine();
            if (cmp.numOfPaths == 0)
                return;
            List<KeyValuePair<string, Component>> cvp = cmp.controls.Where(i => i.Value.GetType() == typeof(Path)).ToList();
            Path p = cvp[curPath].Value as Path;
            Path.PrintCenter(p.name);
            Path.OverallStringAddLine(string.Format("Polynom: {0}", p.Poly.LinearRepresentationOfPolinom()));
            Path.OverallStringAddLine(string.Format("Path is connecting {0} components: ", p.ConnectedComponent.Count));
            p.ConnectedComponent.ForEach((i) =>
            {
                Path.OverallStringAdd(string.Format("{0} => dx = {1} ; ", i.name, p.Poly.DerivativeForX(i.x)));
            });
        }
        public void PrintDebug(Engine engine)
        {
            Debug.PrintLine();
            Debug.PrintCenter("Debug");
            Debug.PrintLine();
            Debug.OverallStringAddLine(string.Format("Location({0},{1})", engine.center.x, engine.center.y));
            Debug.OverallStringAddLine(string.Format("On this location is: {0}", engine.root.GetComponentOnLocation(engine.center).name));
            Debug.PrintLine();
        }
        internal void PrintHelp(DateTime helpNewest)
        {
            if (helpNewest > newestHelp)
            {
                newestHelp = helpNewest;
                Help.Clear();
                PrintHelp();
            }
            Help.Flush();
        }
        internal void PrintMessage(int curMessage, List<string> logMessages, Queue<string> message, DateTime messageNewest)
        {
            if (messageNewest > newestMessage)
            {
                newestMessage = messageNewest;
                Message.Clear();
                PrintMessage(curMessage, logMessages, message);
            }
            Message.Flush();
        }
        internal void ListRoomsinComponent(int n, int curRoomList, Component cmp, DateTime RoomsNewest)
        {
            if (RoomsNewest > newestRoom)
            {
                newestRoom = RoomsNewest;
                Room.Clear();
                ListRoomsinComponent(n, curRoomList, cmp);
            }
            Room.Flush();
        }
        internal void ListPathsinComponent(int i, Component cmp, DateTime PathsNewest)
        {
            if (PathsNewest > newestPath)
            {
                newestPath = PathsNewest;
                Path.Clear();
                ListPathsinComponent(i, cmp);
            }
            Path.Flush();
        }
        internal void PrintDebug(Engine engine, DateTime debugNewest)
        {
            if (debugNewest > newestDebug)
            {
                newestDebug = debugNewest;
                Debug.Clear();
                PrintDebug(engine);
            }
            Debug.Flush();
        }
    }
}
