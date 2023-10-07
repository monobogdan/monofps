using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public sealed class GameUI
    {
        const int ItemHeight = 59;

        public bool BuyMenu
        {
            get;
             set;
        }

        private Texture windowBg;
        private Texture windowFrame;

        private Texture goldBar;
        private Texture shopShotgun;
        private Texture shopRifle;
        private Texture shopMinigun;
        private Texture shopRocketl;
        private Texture shopHealth;
        private Texture shopBullets;

        private bool btnShotgState;
        private bool btnRifleState;
        private bool btnRocketlState;
        private bool btnMinigunState;
        private bool btnBulletState;
        private bool btnHealthState;

        private World world;

        public GameUI(World world)
        {
            this.world = world;

            windowBg = Game.Current.Data.GetTexture("ui/uik_window.bmp");
            windowFrame = Game.Current.Data.GetTexture("ui/uik_frame.bmp");
            goldBar = Game.Current.Data.GetTexture("ui/gold_bar.png");

            shopShotgun = Game.Current.Data.GetTexture("ui/store_shotg.png");
            shopRifle = Game.Current.Data.GetTexture("ui/store_rifle.png");
            shopMinigun = Game.Current.Data.GetTexture("ui/store_minigun.png");
            shopRocketl = Game.Current.Data.GetTexture("ui/store_rocket.png");

            shopHealth = Game.Current.Data.GetTexture("ui/store_health.png");
            shopBullets = Game.Current.Data.GetTexture("ui/store_bullets.png");
        }

        public void OpenBuyMenu()
        {
            BuyMenu = true;
        }

        public static Vector2 UIRelativeToAbs(float x, float y)
        {
            return new Vector2(x * Game.Current.Width, y * Game.Current.Height);
        }

        private void DrawBuyMenu()
        {
            int w = shopShotgun.Width * 2 + 30;
            int x = Game.Current.Width / 2 - (w / 2);
            int y = Game.Current.Height / 2 - (480 / 2);
            UI.Window(windowBg, windowFrame, "Shop", new Vector2(x, y), new Vector2(w, 480));

            for (int i = 0; i < (int)world.Cash; i++)
                Game.Current.Graphics.DrawSprite(goldBar, new Vector2(x + 10 + (i * goldBar.Width), y + 32), Color4.White, 0, 0);

            // Shop items
            int iGridX = x + 10;
            int iGridY = y + 32 + goldBar.Height;

            if (!world.Player.HasWeapon(typeof(Shotgun)) && UI.Button(shopShotgun,
                new Vector2(iGridX, iGridY), new Vector2(shopShotgun.Width, ItemHeight), ref btnShotgState))
            {
                world.Player.GiveWeapon(typeof(Shotgun));
                world.Cash -= 2;
            }

            if (!world.Player.HasWeapon(typeof(Rifle)) && UI.Button(shopRifle,
                new Vector2(iGridX, iGridY + ItemHeight), new Vector2(shopShotgun.Width, ItemHeight), ref btnRifleState))
            {
                world.Player.GiveWeapon(typeof(Rifle));
                world.Cash -= 3;
            }

            if (!world.Player.HasWeapon(typeof(RocketLauncher)) && UI.Button(shopRocketl,
                new Vector2(iGridX, iGridY + (ItemHeight * 2)), new Vector2(shopShotgun.Width, ItemHeight), ref btnRocketlState))
            {
                world.Player.GiveWeapon(typeof(RocketLauncher));
                world.Cash -= 7;
            }

            if (!world.Player.HasWeapon(typeof(MachineGun)) && UI.Button(shopMinigun,
                new Vector2(iGridX, iGridY + (ItemHeight * 3)), new Vector2(shopMinigun.Width, ItemHeight), ref btnMinigunState))
            {
                world.Player.GiveWeapon(typeof(MachineGun));
                world.Cash -= 5;
            }

            if (UI.Button(shopHealth,
                new Vector2(iGridX + shopShotgun.Width + 8, iGridY), new Vector2(shopShotgun.Width, ItemHeight), ref btnRocketlState))
            {
                world.Player.Health = 100;
            }

            if (UI.Button(shopBullets,
                new Vector2(iGridX + shopShotgun.Width + 8, iGridY + ItemHeight), new Vector2(shopMinigun.Width, ItemHeight), ref btnMinigunState))
            {
                for (int i = 0; i < world.Player.Weapons.Count; i++)
                    world.Player.Weapons[i].Ammo += 25;
            }

            //(windowBg, windowFrame, "Shop", new Vector2(x, y), new Vector2(640, 480));
        }

        public void Draw(TextRenderer textRenderer, TextFont font)
        {
            if(BuyMenu)
            {
                DrawBuyMenu();
            }
        }
    }
}
