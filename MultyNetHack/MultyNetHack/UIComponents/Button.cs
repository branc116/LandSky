using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack;
using MultyNetHack.Commands;
/// <summary>
/// Heare are implemented ui elements
/// </summary>
namespace MultyNetHack.UIComponents
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
        public void InvokEvent(BaseCommand bc)
        {
            OnPress?.Invoke(this, DateTime.Now);
        }
        public override string ToString()
        {
            return string.Format("{0}. {1}", InvokeCommand.ToString().Replace("Option", string.Empty), Text);
        }

    }
}
