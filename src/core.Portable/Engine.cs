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

        private BaseScreen _activeScreen
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
                return _activeScreen?.Controls?[Name];
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
            return _activeScreen.ParseCommand(_name, Cc, Info);
        }

        public bool InputNextCommand(MyConsoleKeyInfo Info, string NameOfSubject)
        {
            var Cc = CommandControls.KeyMap.ContainsKey(Info) ? CommandControls.KeyMap[Info] : MyEnums.Comands.Any;
            return _activeScreen.ParseCommand(NameOfSubject, Cc, Info);
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
            return _activeScreen.CurrentStateOfTheScreen.StateOfConsole;
        }

        /// <summary>
        /// Get all the components on the screen
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Component> AllComponentsOnScreen()
        {
            foreach (var comp in _activeScreen.Controls)
            {
                yield return comp.Value;
            }
        }

        public IEnumerable<UIComponentBase> AllUIComponentsOnScreen()
        {
            foreach (var ui in _activeScreen.UIComponents)
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
            if (Component?.Name == null || _activeScreen.Controls.Any(i => i.Value.Name == Component.Name))
                throw new Exception($"Component can't have that name... {Component?.Name} sorry bro");
            _activeScreen.Controls.Add(Component.Name, Component);
        }

        public void RemoveComponentFromActiveScreen(Component Component)
        {
            _activeScreen.Delete(Component.Name);
        }

        public void RemoveComponentFromActiveScreen(string ComponentName)
        {
            _activeScreen.Delete(ComponentName);
        }

        public void SetActiveComponent(Component NewActive, int NewWidth, int NewHeight)
        {
            if (_activeScreen is SandboxMap)
            {
                (_activeScreen as SandboxMap).ChangeActiveComponent(NewActive.Name, NewWidth, NewHeight);
            }
            else
            {
                throw new ArgumentOutOfRangeException("NewActive");
            }
        }

        public string RenderAroundComponent(Component Component, int Width, int Height)
        {
            if (_activeScreen is SandboxMap)
            {
                SandboxMap Map = _activeScreen as SandboxMap;
                return Map.ReginToString(Component, Width, Height);
            }
            return "Sorry cant find active game :(";
        }

        public string RenderAroundComponent()
        {
            if (_activeScreen is SandboxMap)
            {
                return (_activeScreen as SandboxMap).ReginToString();
            }
            return "Sorry cant find active game :(";
        }
    }
}