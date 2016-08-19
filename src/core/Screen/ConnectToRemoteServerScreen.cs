using LandSky.UIComponents;

namespace LandSky.Screen
{
    public class ConnectToRemoteServerScreen : BaseScreen
    {
        public ConnectToRemoteServerScreen() : base(0, 0, "Connect To Remote Server")
        {
            InitTextBoxes();
            InputMode = MyEnums.InputMode.InputFirst;
            ScreenChange();
        }

        private void InitTextBoxes()
        {
            UIComponents.Add(new TextBox("test", 0, 2, 20));
            UIComponents.Add(new TextBox("Ip", 0, 7, 7, "", ".:") { Hint = "Enter IP:PORT" });
        }
    }
}