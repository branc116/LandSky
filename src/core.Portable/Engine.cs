using LandSky.Components;
using LandSky.DotNetExt;
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
    public sealed class Engine
    {
        private string _name;

        private BaseScreen ActiveScreen
        {
            get
            {
                return BaseScreen.Active.Peek();
            }
        }

        public Component this[string Name]
        {
            get
            {
                return ActiveScreen?.Controls?[Name];
            }
        }

        public Engine(string Name)
        {
            _name = Name;
            PushNewScreenOnTop(new SandboxMap(0, 0));
            PushNewComponentOnActiveScreen(new Player(Name));
        }

        public bool InputNextCommand(MyConsoleKeyInfo Info)
        {
            var Cc = CommandControls.KeyMap.ContainsKey(Info) ? CommandControls.KeyMap[Info] : MyEnums.Comands.Any;
            return ActiveScreen.ParseCommand(_name, Cc, Info);
        }
        public bool InputNextCommand(MyConsoleKeyInfo Info, string NameOfSubject)
        {
            var Cc = CommandControls.KeyMap.ContainsKey(Info) ? CommandControls.KeyMap[Info] : MyEnums.Comands.Any;
            return ActiveScreen.ParseCommand(NameOfSubject, Cc, Info);
        }


        /// <summary>
        /// Isn't working..
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Isn't working
        /// </summary>
        /// <param name="StackOfActiveScreens"></param>
        public void LoadFromJSON(string StackOfActiveScreens)
        {
            BaseScreen.Active = JsonConvert.DeserializeObject<Stack<BaseScreen>>(StackOfActiveScreens);
        }

        /// <summary>
        /// Isn't working
        /// </summary>
        /// <returns></returns>
        public string CurrentFrame()
        {
            return ActiveScreen.CurrentStateOfTheScreen.StateOfConsole;
        }

        /// <summary>
        /// Get all the components on the screen
        /// </summary>
        /// <returns></returns>
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