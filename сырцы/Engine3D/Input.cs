using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;

namespace Engine3D
{
    public sealed class Input
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        public Input()
        {

        }

        public bool GetKeyState(Keys key)
        {
            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }

        public Vector2 GetMousePosition()
        {
            if (!Game.Current.Form.IsDisposed)
            {
                var pt = Game.Current.Form.PointToClient(new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y));

                return new Vector2(pt.X, pt.Y);
            }

            return Vector2.Zero;
        }
    }
}
