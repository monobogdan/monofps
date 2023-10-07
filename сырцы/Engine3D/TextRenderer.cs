using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SharpDX;

namespace Engine3D
{
    public sealed class TextFont
    {
        public string Name
        {
            get;
            private set;
        }

        internal Font Native
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }

        public TextFont(string name, int size)
        {
            Native = new Font(name, size, FontStyle.Regular, GraphicsUnit.Pixel);

            Size = size;
            Name = name;
        }
    }

    public sealed class TextRenderer
    {
        internal Bitmap Bitmap
        {
            get;
            private set;
        }

        private TextFont arial;
        private Texture texture;
        private System.Drawing.Graphics graphics;

        private SolidBrush brush;

        public TextRenderer()
        {
            arial = new TextFont("Arial", 16);

            brush = new SolidBrush(System.Drawing.Color.White);
            texture = new Texture();
        }

        public void Begin()
        {
            if(Bitmap == null)
            {
                Bitmap = new Bitmap(1024, 1024);
                graphics = System.Drawing.Graphics.FromImage(Bitmap);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }

            graphics.Clear(System.Drawing.Color.Transparent);
        }

        public void DrawString(TextFont font, string str, int x, int y, Color4 col)
        {
            if (font == null)
                font = arial;

            brush.Color = System.Drawing.Color.FromArgb((int)(col.Alpha * 255), (int)(col.Red * 255), (int)(col.Green * 255), (int)(col.Blue * 255));
            graphics.DrawString(str, font.Native, brush, x, y);
        }

        public void End()
        {
            var bitmapLock = Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            texture.Upload(bitmapLock.Scan0, Bitmap.Width, Bitmap.Height, true, true);
            Bitmap.UnlockBits(bitmapLock);

            Game.Current.Graphics.DrawSprite(texture, Vector2.Zero);
        }
    }
}
