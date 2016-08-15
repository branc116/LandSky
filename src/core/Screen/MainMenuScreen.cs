using System;
using System.Collections.Generic;
using LandSky.Commands;
using LandSky.MyEnums;
using LandSky.UIComponents;


namespace LandSky.Screen
{
    /// <summary>
    /// Screen showing all the options. Extends BaseScreen
    /// </summary>
    public class MainMenuScreen : BaseScreen
    {
        /// <summary>
        /// Create new Main Menu Screen
        /// </summary>
        /// <param name="Top">Distance from the top of the global console</param>
        /// <param name="Left">Distance form the left of the global console</param>
        public MainMenuScreen(int Top,int Left) : base(Top, Left, "Main menu")
        {
            InitButtons();
            InitComands();
            InputMode = InputMode.ControlFirst;
            ScreenChange();
        }
        
        private void InitButtons()
        {
            Button StartLocalGame = new Button("StartLocal","Start new local game",0,0,0, Comands.Option1);
            Button ConnectToRemoteServer = new Button("ConnectToRemoteServer", "Connect to remote server", 0, 1, 0, Comands.Option2);
            Button Exit = new Button("Exit","Exit game",0,2,0, Comands.Option3);
            StartLocalGame.OnAccept += StartLocalOnPress;
            ConnectToRemoteServer.OnAccept += ConnectToRemoteServerOnPress;
            Exit.OnAccept += ExitOnPress;

            UIComponents.AddRange(new UIComponentBase[] { StartLocalGame, ConnectToRemoteServer, Exit });
        }
        private void InitComands()
        {
            foreach (var B in UIComponents)
            {
                MLocalCommands.Add((B as Button).InvokeCommand, B.InvokeAccept);
            }
            this.Resume();
        }
        
        private void ConnectToRemoteServerOnPress(object sender, DateTime e)
        {
            this.Pause();
            Active.Push(new ConnectToRemoteServerScreen());
        }
        private void ExitOnPress(object Sender, DateTime E)
        {
            this.Pause();
            Active.Pop();
            Environment.Exit(0);
        }
        /// <summary>
        /// Start new instance of the game
        /// </summary>
        private void StartLocalOnPress(object Sender, DateTime E)
        {
            this.Pause();
            Active.Push(new SandboxMap(GlobalTop, GlobalLeft));
        }
    }
} 
