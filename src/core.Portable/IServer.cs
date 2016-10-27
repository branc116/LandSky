using LandSky.MyEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandSky
{
    public interface IServer
    {
        void Register(string Mail, string Password, string Username);
        void Login(string MailOrUsername, string Password);
        void NewCommand(Comands Command, string Token);
        void Logout();

    }
}
