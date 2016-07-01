using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
    class TempConsole
    {
        protected string OverallString;
        public int OverallStringLeft
        {
            get
            {
                for (int i = OverallString.Length - 1; i >= 0; i--)
                {
                    if (OverallString[i] == '\n')
                        return OverallString.Length - i - 1;
                }
                return OverallString.Length;
            }
            set
            {
                int i = OverallString.Length - 1;
                for (; i >= 0; i--)
                {
                    if (OverallString[i] == '\n')
                        break;
                }
                if (i + value <= OverallString.Length && OverallString.Length!=0)
                    OverallString = OverallString.Remove(i + value);
                else if (OverallString.Length!=0)
                    OverallString += new string(' ', i + value - OverallString.Length);
            }
        }
        public TempConsole()
        {
            OverallString = string.Empty;
        }
        public void OverallStringAdd(object obj)
        {
            OverallString += obj.ToString();
        }
        public void OverallStringAddLine(object obj)
        {
            OverallStringAdd(obj);
            OverallString += '\n';
        }
        public void OverallStringAddLine()
        {
            OverallString += '\n';
        }
        public void PrintLine()
        {
            if (OverallStringLeft != 0)
                OverallStringLeft = 0;
            OverallStringAddLine(new string('-', Console.WindowWidth));
        }
        public void PrintCenter(object obj)
        {
            string s = obj.ToString();
            OverallStringLeft = 0;
            OverallString += new string(' ', Console.WindowWidth / 2 - s.Length / 2) + s + '\n';   
        }
        public void Flush()
        {
            Console.WriteLine(OverallString);
        }
        public void Clear()
        {
            OverallString = string.Empty;
        }
    }
}
