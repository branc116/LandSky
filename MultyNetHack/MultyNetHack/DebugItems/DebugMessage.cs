using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Debug tools go in here
/// </summary>
namespace MultyNetHack.DebugItems
{
    /// <summary>
    /// This call is used to create debug messages
    /// </summary>
    public class DebugMessage
    {
        public DateTime CreateTime;
        public string Message;
        public DebugMessage(string Message)
        {
            this.Message = Message;
            CreateTime = DateTime.Now;
        }
        public override string ToString()
        {
            return string.Format("({0})>{1}", CreateTime, Message);
        }
    }
}
