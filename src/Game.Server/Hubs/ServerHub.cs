using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using LandSky;
using LandSky.MyEnums;
using Microsoft.AspNet.SignalR.Hubs;

namespace Game.Server.Hubs
{
    [HubName("ServerHub")]
    public class ServerHub : Hub, IServer
    {
        private static Engine _engine;

        public void Login(string MailOrUsername, string Password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void NewCommand(Comands Command, string Token)
        {
            throw new NotImplementedException();
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public void Register(string Mail, string Password, string Username)
        {
            throw new NotImplementedException();
        }
    }
}