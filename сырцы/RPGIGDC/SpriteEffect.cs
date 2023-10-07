using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;

namespace RPGIGDC
{
    public sealed class SpriteEffect : Entity
    {
        private World world;

        public Texture Texture
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }

        public float Size
        {
            get;
            set;
        }

        private float time;

        public SpriteEffect(World world)
        {
            this.world = world;

            Speed = 0.1f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            time += Speed;

            if(time > 1)
            {
                world.Destroy(this);
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            Game.Current.Graphics.DrawPointSprite(Texture, Position, Size);
        }
    }
}
