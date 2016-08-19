using LandSky;
using LandSky.Commands;
using LandSky.Components;
using LandSky.Screen;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class Players// : IEnumerable<KeyValuePair<string, Player>>
    {
        static private SortedList<string, Player> _Users = new SortedList<string, Player>();
        static private Engine _Engine = new Engine();
        static private Random _Rand = new Random();
        static private SandboxMap _Map = new SandboxMap(0, 0);

        public Players()
        {
            Task t = GeneratePath(3);
            _Engine.PushNewScreenOnTop(_Map);
        }

        public async Task GeneratePath(int N)
        {
            if (_Map.NumOfRooms < 3)
                await _Map.GenerateRandomRooms(200);
            _Map.GeneratePath(new GeneratePathCommand() { NumberOfPaths = N });
        }

        public Player this[string userId]
        {
            get
            {
                return _Users[userId];
            }

            set
            {
                _Users[userId] = value;
            }
        }

        public int Count
        {
            get
            {
                return _Users.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Add(string UserId, Player item)
        {
            try
            {
                _Engine.PushNewComponentOnActiveScreen(new Player(item.Name) { LocalX = _Rand.Next(-10, 10), LocalY = _Rand.Next(-10, 10) });
                _Users.Add(UserId, item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Clear()
        {
            _Users.Clear();
        }

        public bool Contains(Player item)
        {
            return _Users.Any(i => i.Value == item);
        }

        public IEnumerator<KeyValuePair<string, Player>> GetEnumerator()
        {
            foreach (var _Player in _Users)
            {
                yield return _Player;
            }
        }

        public bool Remove(Player item)
        {
            _Engine.RemoveComponentFromActiveScreen(item);
            return _Users.Remove(item.Name);
        }

        public bool Remove(string UserID)
        {
            try
            {
                _Engine.RemoveComponentFromActiveScreen(_Users[UserID]);
                _Users.Remove(UserID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetFrameForPlayer(string PlayerID)
        {
            return _Engine.RenderAroundComponent(_Users[PlayerID], 70, 30);
        }

        //IEnumerator<KeyValuePair<string, Player>> IEnumerable<KeyValuePair<string, Player>>.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return _Users.GetEnumerator();
        //}

        public void ParseControl(string caller, bool ctrl, bool alt, string character)
        {
            Player p = _Users[caller];
            _Engine.RenderAroundComponent(p, 2, 3);
            ConsoleKey ck;

            Enum.TryParse(character.ToUpper(), out ck);
            _Engine.InputNextCommand(new ConsoleKeyInfo(character[0], ck, false, alt, ctrl));
        }
    }

    public class PlayerHub : Hub
    {
        private static Players _players = new Players();

        public override Task OnConnected()
        {
            base.Clients.Caller.Respones();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var a = ((Connection)((Microsoft.AspNetCore.SignalR.Hubs.SignalProxy)((HubConnectionContext)this.Clients).Caller).Connection).Identity;
            Task tt;
            if (_players.Remove(base.Clients.Caller))
                tt = updateForAll();
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void InsertNew(string Name)
        {
            var a = ((Connection)((Microsoft.AspNetCore.SignalR.Hubs.SignalProxy)((HubConnectionContext)this.Clients).Caller).Connection).Identity;
            var Ok = _players.Add(a, new Player(Name));
            if (Ok == true)
            {
                Task t = updateForAll();
            }
            base.Clients.Caller.NameOk(Ok, Name);
        }

        private static object LockUpdateAll = new object();

        private async Task updateForAll()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var player in _players)
                {
                    Task.Factory.StartNew(() =>
                    {
                        base.Clients.Client(player.Key).UpdateFrame(_players.GetFrameForPlayer(player.Key));
                    });
                }
            });
        }

        public void Input(bool ctrl, bool alt, string Character)
        {
            var a = ((Connection)((Microsoft.AspNetCore.SignalR.Hubs.SignalProxy)((HubConnectionContext)this.Clients).Caller).Connection).Identity;
            _players.ParseControl(a, ctrl, alt, Character);
            Task t = updateForAll(a);
        }

        private async Task updateForAll(string a)
        {
            await Task.Factory.StartNew(() => { base.Clients.All.UpdateFrame(_players.GetFrameForPlayer(a)); });
        }
    }
}