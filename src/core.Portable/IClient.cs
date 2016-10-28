using LandSky.MyEnums;
using LandSky.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandSky
{
    public interface IClient
    {
        void InitMap(SandboxMap Map);
        void Update(Comands Command, string ObjectsName);
        void CallBack(string Message);

    }
}
