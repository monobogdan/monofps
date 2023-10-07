using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine3D
{
    public static class Mathf
    {
        public const float DegToRad = 0.0174533f;

        public static float Clamp(float val, float min, float max)
        {
            return val < min ? min : (val > max ? max : val);
        }
        public static float Abs(float val)
        {
            return val < 0 ? -val : val;
        }
    }
}
