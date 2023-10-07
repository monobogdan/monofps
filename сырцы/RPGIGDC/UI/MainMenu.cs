using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    internal enum SubMenu
    {
        Main,
        Settings,
        Scoreboard
    }

    public sealed class MainMenu
    {
        const int TextHeight = 15;

        private static Color4 Color = new Color4(0.80f, 0.80f, 0.59f, 1);

        private GameMain gameMain;

        private TextRenderer textRenderer;
        private Texture menuBg;

        private Texture checkOn;
        private Texture checkOff;

        private SubMenu subMenu;

        public bool IsVisible
        {
            get;
            set;
        }

        private bool btnStart;
        private bool stateLerp;
        private bool checkLerp;

        public MainMenu(GameMain gameMain)
        {
            this.gameMain = gameMain;

            menuBg = Game.Current.Data.GetTexture("menuBg.jpg");
            checkOff = Game.Current.Data.GetTexture("checkoff.png");
            checkOn = Game.Current.Data.GetTexture("checkon.png");
            textRenderer = new TextRenderer();

            subMenu = SubMenu.Main;
        }

        public void OnUpdate()
        {

        }

        private void Title()
        {
            Vector2 offs = GameUI.UIRelativeToAbs(0.05f, 0.4f);

            textRenderer.Begin();

            if (UI.Button(null, "Начать", textRenderer, Color, offs, new Vector2(128, TextHeight), ref btnStart))
            {
                gameMain.StartGame();
            }

            /*if (UI.Button(shotgTex, "Настройки", textRenderer, Color, offs + new Vector2(0, TextHeight), new Vector2(128, TextHeight), ref btnStart))
            {
                subMenu = SubMenu.Settings;
            }*/

            if (UI.Button(null, "Выход", textRenderer, Color, offs + new Vector2(0, TextHeight * 2), new Vector2(128, TextHeight), ref btnStart))
            {
                Console.WriteLine("Hi");
            }

            textRenderer.End();
        }

        private void Settings()
        {
            Vector2 offs = GameUI.UIRelativeToAbs(0.05f, 0.4f);

            textRenderer.Begin();

            checkLerp = UI.Checkbox(checkOn, checkOff, "Lerp", textRenderer, Color, offs, new Vector2(128, TextHeight), checkLerp, ref stateLerp) ;

            textRenderer.End();
        }

        public void OnDraw()
        {
            Game.Current.Graphics.DrawSprite(menuBg, Vector2.Zero, Color4.White, Game.Current.Width, Game.Current.Height);

            if (subMenu == SubMenu.Main)
                Title();

            if (subMenu == SubMenu.Settings)
                Settings();
        }
    }
}
