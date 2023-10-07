using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine3D;

namespace RPGIGDC
{
    public sealed class GameMain : IGameApp
    {

        public Map Map
        {
            get;
            private set;
        }

        private MainMenu menu;
        private World world;

        public Player Player
        {
            get;
            private set;
        }

        public static Stream GetResource(string fileName)
        {
            return Game.Current.Data.GetStream(fileName);
        }

        public void OnCreate()
        {
            menu = new MainMenu(this);
            menu.IsVisible = true;
        }

        public void StartGame()
        {
            menu.IsVisible = false;

            world = new World();
            GC.Collect();
        }

        public void OnDraw()
        {
            if(world != null)
                world.Draw();
        }

        public void OnExit()
        {

        }

        public void OnDrawGUI()
        {
            if(world != null)
                world.DrawGUI();

            if (menu.IsVisible)
                menu.OnDraw();
        }

        public void OnUpdate()
        {
            if(world != null)
                world.Update();
        }
    }
}
