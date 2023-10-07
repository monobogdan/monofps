using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Engine3D
{
    public sealed class UI
    {
        public static void Window(Texture background, Texture frame, string title, Vector2 from, Vector2 to)
        {
            if(background != null && frame != null)
            {
                Game.Current.Graphics.DrawSprite(background, from, new Color4(1, 1, 1, 0.8f), to.X, to.Y + frame.Height);
            }
        }

        private static bool PointInRectangle(float x1, float y1, float x, float y, float w, float h)
        {
            return x1 > x && y1 > y && x1 < x + w && y1 < y + h;
        }

        public static bool Button(Texture tex, Vector2 from, Vector2 to, ref bool state)
        {
            return Button(tex, null, null, Color4.White, from, to, ref state);
        }

        public static bool Button(Texture tex, string text, TextRenderer textRenderer, Color4 col, Vector2 from, Vector2 to, ref bool state)
        {
            Vector2 pos = Game.Current.Input.GetMousePosition();
            Rectangle rect = new Rectangle((int)from.X, (int)from.Y, (int)to.X, (int)to.Y);

            if (tex != null)
                Game.Current.Graphics.DrawSprite(tex, from, Color.White, to.X, to.Y);

            

            bool ret = false;
            bool isHover = false;

            if (PointInRectangle(pos.X, pos.Y, from.X, from.Y, to.X, to.Y))
            {
                isHover = true;

                if (!state && Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.LButton))
                {
                    ret = true;
                    state = true;
                }
            }
            else
            {
                state = false;
            }

            if (textRenderer != null && text.Length > 0)
                textRenderer.DrawString(null, text, (int)from.X, (int)from.Y, !isHover ? col : Color4.White);

            return ret;
        }

        public static bool Checkbox(Texture on, Texture off, string text, TextRenderer textRenderer, Color4 col, Vector2 from, Vector2 to, bool prevCheck, ref bool state)
        {
            Vector2 pos = Game.Current.Input.GetMousePosition();
            Rectangle rect = new Rectangle((int)from.X, (int)from.Y, (int)to.X, (int)to.Y);

            if (prevCheck && on != null)
                Game.Current.Graphics.DrawSprite(on, from, Color.White, on.Width, on.Height);

            if (!prevCheck && off != null)
                Game.Current.Graphics.DrawSprite(off, from, Color.White, on.Width, on.Height);

            bool ret = prevCheck;
            bool isHover = false;

            if (PointInRectangle(pos.X, pos.Y, from.X, from.Y, to.X, to.Y))
            {
                isHover = true;

                if (!state && Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.LButton))
                {
                    ret = !prevCheck;
                    state = true;
                }
                
                if(!Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.LButton))
                {
                    state = false;
                }
            }
            else
            {
                state = false;
            }

            if (textRenderer != null && text.Length > 0)
                textRenderer.DrawString(null, text, (int)from.X + on.Width + 4, (int)from.Y + 8, !isHover ? col : Color4.White);

            return ret;
        }
    }
}
