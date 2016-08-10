using System;

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
            return $"({CreateTime})>{Message}";
        }
    }
}
