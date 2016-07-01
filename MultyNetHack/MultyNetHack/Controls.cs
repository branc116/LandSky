using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyNetHack
{
    class Controls
    {
        public Dictionary<char, Comands> KeyMap;
        public Controls()
        {
            LoadKeyMap(KeyMap);
        }
        private void LoadKeyMap(Dictionary<char, Comands> keyMap)
        {
            KeyMap = new Dictionary<char, Comands>();
            KeyMap.Add('h', Comands.Left);
            KeyMap.Add('H', Comands.TenStepsLeft);
            KeyMap.Add('l', Comands.Right);
            KeyMap.Add('L', Comands.TenStepsRight);
            KeyMap.Add('k', Comands.Up);
            KeyMap.Add('K', Comands.TenStepsUp);
            KeyMap.Add('j', Comands.Down);
            KeyMap.Add('J', Comands.TenStepsDown);
            KeyMap.Add('q', Comands.ScrollLeft);
            KeyMap.Add('e', Comands.ScrollRight);
            KeyMap.Add('Q', Comands.TabLeft);
            KeyMap.Add('E', Comands.TabRight);
            KeyMap.Add('r', Comands.GenerateOneRoom);
            KeyMap.Add('R', Comands.GenerateALotOfRooms);
            KeyMap.Add('P', Comands.GenerateRandomPath);
            KeyMap.Add('d', Comands.DequeMessage);
        }
    }
}
