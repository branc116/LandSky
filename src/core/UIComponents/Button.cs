using System;

using LandSky.Commands;
using LandSky.MyEnums;

namespace LandSky.UIComponents
{
    /// <summary>
    /// Ui element button
    /// </summary>
    class Button 
    {
        public Comands InvokeCommand;
        public event EventHandler<DateTime> OnPress;
        public string Name;
        public string Text;
        public Button(string Name, Comands Comm)
        {
            InvokeCommand = Comm;
            this.Name = Name;
        }
        public void InvokEvent()
        {
            OnPress?.Invoke(this, DateTime.Now);
        }
        public void InvokEvent(BaseCommand Bc)
        {
            OnPress?.Invoke(this, DateTime.Now);
        }
        public override string ToString()
        {
            return $"{InvokeCommand.ToString().Replace("Option", string.Empty)}. {Text}";
        }

    }
}
