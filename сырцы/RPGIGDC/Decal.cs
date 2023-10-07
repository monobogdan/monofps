using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;

namespace RPGIGDC
{
    public sealed class Decal : Entity
    {
        private World world;

        public Material Material
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public float FadeTime
        {
            get;
            set;
        }

        private float alpha;
        private Mesh plane;

        public Decal(World world)
        {
            this.world = world;

            Vertex[] verts = new Vertex[]
            {
                new Vertex()
                {
                    Position = new SharpDX.Vector3(0, 0, 0),
                    UV = new SharpDX.Vector2(0, 0)
                },
                new Vertex()
                {
                    Position = new SharpDX.Vector3(1, 0, 0),
                    UV = new SharpDX.Vector2(1, 0)
                },
                new Vertex()
                {
                    Position = new SharpDX.Vector3(1, 0, 1),
                    UV = new SharpDX.Vector2(1, 1)
                },
                new Vertex()
                {
                    Position = new SharpDX.Vector3(1, 0, 1),
                    UV = new SharpDX.Vector2(1, 1)
                },
                new Vertex()
                {
                    Position = new SharpDX.Vector3(0, 0, 1),
                    UV = new SharpDX.Vector2(0, 1)
                },
                new Vertex()
                {
                    Position = new SharpDX.Vector3(0, 0, 0),
                    UV = new SharpDX.Vector2(0, 0)
                }
            };

            alpha = 1;

            plane = new Mesh();
            plane.Upload(new Vertex[][] { verts });
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(FadeTime != 0)
            {
                FadeTime -= Game.Current.DeltaTime;

                if(FadeTime < 0)
                {
                    world.Destroy(this);
                }
            }
        }

        public override void OnDrawTransparent()
        {
            base.OnDrawTransparent();

            Game.Current.Graphics.DrawMesh(plane, 0, Material, Position, Rotation, new SharpDX.Vector3(Scale, Scale, Scale), 0, SharpDX.Vector2.Zero);
        }
    }
}
