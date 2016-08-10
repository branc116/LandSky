using System;
using System.Collections.Generic;

using MultyNetHack.MyEnums;
using MultyNetHack.UIComponents;


namespace MultyNetHack.Screen
{
    /// <summary>
    /// Screen showing all the options. Extends BaseScreen
    /// </summary>
    public class MainMenuScreen : BaseScreen
    {
        /// <summary>
        /// List of buttons in use
        /// </summary>
        private List<Button> mMButtons;
        /// <summary>
        /// Create new Main Menu Screen
        /// </summary>
        /// <param name="Top">Distance from the top of the global console</param>
        /// <param name="Left">Distance form the left of the global console</param>
        public MainMenuScreen(int Top,int Left) : base(Top, Left, "Main menu")
        {
            InitGlobals();
            InitButtons();
            InitComands();
            InitText();

            ScreenChange();
        }
        private void InitGlobals()
        {
            mMButtons = new List<Button>();
        }
        private void InitButtons()
        {
            Button StartLocalGame = new Button("StartLocal", Comands.Option1) { Text = "Start new local game" };
            Button Exit = new Button("Exit", Comands.Option2) { Text = "Exit game" };
            StartLocalGame.OnPress += StartLocalOnPress;
            Exit.OnPress += ExitOnPress;
            mMButtons.AddRange(new Button[] { StartLocalGame, Exit });
        }
        private void InitComands()
        {
            foreach (Button B in mMButtons)
            {
                MLocalCommands.Add(B.InvokeCommand, B.InvokEvent);
            }
            this.Resume();
        }
        private void InitText()
        {
            foreach (Button B in mMButtons)
            {
                this.VirtualConsoleAddLine(B.ToString());
            }
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
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        private void StartLocalOnPress(object Sender, DateTime E)
        {
            this.Pause();
            Active.Push(new SandboxMap(GlobalTop, GlobalLeft));
        }

    }
} 
