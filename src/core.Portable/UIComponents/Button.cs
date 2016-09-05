using LandSky.DotNetExt;
using LandSky.MyEnums;

namespace LandSky.UIComponents
{
    /// <summary>
    /// Ui element button
    /// </summary>
    internal class Button : UIComponentBase
    {
        public Comands InvokeCommand;

        public Button(string Name, string Text, int TabIndex, int Top, int Left, Comands Comm) : base(Name, TabIndex, Top, Left, Text)
        {
            InvokeCommand = Comm;
        }

        public Button(string Name, string Text, Comands Comm) : base(Name, 0, 0, 0, Text)
        {
            InvokeCommand = Comm;
        }

        public override string ToString()
        {
            if (Focus == true)
                return $"->{InvokeCommand.ToString().Replace("Option", string.Empty)}. {Text}";
            else
                return $"{InvokeCommand.ToString().Replace("Option", string.Empty)}. {Text}";
        }

        public override bool NewInput(MyConsoleKeyInfo c)
        {
            return base.NewInput(c);
        }
    }
}