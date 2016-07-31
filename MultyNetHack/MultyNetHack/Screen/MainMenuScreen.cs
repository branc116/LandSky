using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultyNetHack.UIComponents;
using MultyNetHack.Commands;

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
        private List<Button> mButtons;
        
        /// <summary>
        /// Create new Main Menu Screen
        /// </summary>
        /// <param Name="Top">Distance from the top of the global console</param>
        /// <param Name="Left">Distance form the left of the global console</param>
        public MainMenuScreen(int Top,int Left) : base(Top, Left, "Main menu")
        {
            InitGlobals();
            InitButtons();
            InitComands();
            InitText();
            
            Flush();
        }

        private void InitGlobals()
        {
            mButtons = new List<Button>();
        }
        private void InitButtons()
        {
            Button StartLocalGame = new Button("StartLocal", Comands.Option1) { Text = "Start new local game" };
            Button Exit = new Button("Exit", Comands.Option2) { Text = "Exit game" };
            StartLocalGame.OnPress += StartLocal_OnPress;
            Exit.OnPress += Exit_OnPress;
            mButtons.AddRange(new Button[] { StartLocalGame, Exit });
        }
        private void InitComands()
        {
            foreach (Button mB in mButtons)
            {
                mLocalCommands.Add(mB.InvokeCommand, mB.InvokEvent);
            }
            this.Resume();
        }
        private void InitText()
        {
            foreach (Button mB in mButtons)
            {
                this.VirtualConsoleAddLine(mB.ToString());
            }
        }

        private void Exit_OnPress(object sender, DateTime e)
        {
            this.Pause();
            Active.Pop();
            Environment.Exit(0);
        }
        /// <summary>
        /// Start new instance of the game
        /// </summary>
        /// <param Name="sender"></param>
        /// <param Name="e"></param>
        private void StartLocal_OnPress(object sender, DateTime e)
        {
            this.Pause();
            Active.Push(new EngineSceen(GlobalTop, GlobalLeft));
        }

    }
} 
