using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public sealed class ParticleSheet
    {
        private Texture[] sheet;

        private int frame;
        private float speed;
        private float time;
        private bool isPlaying;

        public ParticleSheet(Texture[] textures)
        {
            sheet = textures;
        }

        public void Update()
        {
            time += 0.1f;

            if(time > 1.0f)
            {

                time = 0;
            }
        }

        public void Draw(Vector3 pos, float size)
        {

        }
    }
}
