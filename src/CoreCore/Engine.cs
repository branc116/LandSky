using LandSky.Components;
using LandSky.Screen;
using LandSky.UIComponents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandSky
{
    /// <summary>
    /// This starts everything
    /// </summary>
    public class Engine
    {
        private BaseScreen ActiveScreen
        {
            get
            {
                return BaseScreen.Active.Peek();
            }
        }

        /// <summary>
        /// Create new engine with main menu as a starting screen
        /// </summary>
        /// <param Name="width">Width of the Screens in the global console</param>
        /// <param Name="height">Height of the Screens in the global console</param>

        public bool InputNextCommand(ConsoleKeyInfo Info)
        {
            Info = new ConsoleKeyInfo(Info.KeyChar, Info.Key, false, (ConsoleModifiers.Alt & Info.Modifiers) != 0, 0 != (ConsoleModifiers.Control & Info.Modifiers));
            var Cc = CommandControls.KeyMap.ContainsKey(Info) ? CommandControls.KeyMap[Info] : MyEnums.Comands.Any;
            return ActiveScreen.ParseCommand(Cc, Info);
        }

        public string StateToJSON()
        {
            return JsonConvert.SerializeObject(BaseScreen.Active, BaseScreen.Active.GetType(), Formatting.Indented, new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                CheckAdditionalContent = true,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                MaxDepth = 5,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });
        }

        public void LoadFromJSON(string StackOfActiveScreens)
        {
            BaseScreen.Active = JsonConvert.DeserializeObject<Stack<BaseScreen>>(StackOfActiveScreens);
        }

        public string CurrentFrame()
        {
            return ActiveScreen.CurrentStateOfTheScreen.StateOfConsole;
        }

        public IEnumerable<Component> AllComponentsOnScreen()
        {
            foreach (var comp in ActiveScreen.Controls)
            {
                yield return comp.Value;
            }
        }

        public IEnumerable<UIComponentBase> AllUIComponentsOnScreen()
        {
            foreach (var ui in ActiveScreen.UIComponents)
            {
                yield return ui;
            }
        }

        public void PushNewScreenOnTop(BaseScreen Screen)
        {
            BaseScreen.Active.Push(Screen);
        }

        public void PushNewComponentOnActiveScreen(Component Component)
        {
            if (Component == null)
                throw new Exception("cant hadel it component is null");
            if (Component?.Name == null || ActiveScreen.Controls.Any(i => i.Value.Name == Component.Name))
                throw new Exception($"Component can't have that name... {Component?.Name} sorry bro");
            ActiveScreen.Controls.Add(Component.Name, Component);
        }

        public void RemoveComponentFromActiveScreen(Component Component)
        {
            ActiveScreen.Delete(Component.Name);
        }

        public void RemoveComponentFromActiveScreen(string ComponentName)
        {
            ActiveScreen.Delete(ComponentName);
        }

        public string RenderAroundComponent(Component Component, int Width, int Height)
        {
            if (ActiveScreen is SandboxMap)
            {
                SandboxMap Map = ActiveScreen as SandboxMap;
                Map.ChangeActiveComponent(Component.Name, Width, Height);
                return CurrentFrame();
            }
            return "Sorry cant find active game :(";
        }
    }
}