using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LandSky.UIComponents;

namespace LandSky.Screen
{
    public class ConnectToRemoteServerScreen : BaseScreen
    {

        public ConnectToRemoteServerScreen() : base(0,0,"Connect To Remote Server")
        {
            InitTextBoxes();
            ScreenChange();
        }

        private void InitTextBoxes()
        {
            UIComponents.Add(new TextBox("Ip", 0, 7, 7));
        }
    }
}
