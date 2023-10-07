using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public abstract class Monster : Entity
    {
        protected World World
        {
            get;
            private set;
        }

        public float MaxHealth
        {
            get;
            set;
        }
   
        public float Health
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            protected set;
        }

        private static Texture pBlood;
        private static Texture pShadowTexture;

        private Material shadowMat;

        private Decal shadowDecal;

        protected Mesh mesh;
        protected Material mat;
        protected MeshAnimator animator;

        protected float disappearTime;

        public Monster(World world)
        {
            World = world;

            if (pBlood == null)
            {
                pBlood = Game.Current.Data.GetTexture("textures/blood0.png");

                pShadowTexture = Game.Current.Data.GetTexture("shadow.png");
            }

            Position = new Vector3(25, -0.99f, 25);
            Box = new BoundingBox(Vector3.Zero, new Vector3(1.5f, 1.5f, 1.5f));

            shadowMat = new Material();
            shadowMat.Flags = MaterialFlags.ShadowMesh;

            /*shadowDecal = new Decal(World);
            shadowDecal.Material = new Material()
            {
                NonLit = true,
                Texture = pShadowTexture
            };
            shadowDecal.Scale = 1;
            World.Spawn(shadowDecal);*/

            MaxHealth = 100;
            Health = MaxHealth;

            animator = new MeshAnimator();
        }

        protected virtual float GetScale()
        {
            return 1;
        }

        protected float AngleBetween(Vector3 point)
        {
            Vector3 fw = Position;
            return (float)Math.Atan2(point.X - fw.X, point.Z - fw.Z) * 57.0f;
        }

        protected void MoveToPlayer()
        {
            float angle = AngleBetween(World.Player.Position);
            float side = Mathf.Clamp(angle, -1, 1);
            float y = 0;

            if (Rotation.Y < angle)
                y += 1;

            if (Rotation.Y > angle)
                y -= 1;

            if(Vector3.Distance(Position, World.Player.Position) > 1.5f && animator.CurrentSequence != "attack")
                Position += GetForward() * Speed * Game.Current.DeltaTime;
            else
            {
                if(animator.CurrentSequence != "attack")
                    OnAttack(World.Player);
            }
            

            Rotation = new Vector3(0, angle, 0);
        }

        protected void CreateDecal()
        {
            Random rand = new Random();

            Decal decal = new Decal(World);
            decal.Position = Position + new Vector3(0, rand.NextFloat(0.01f, 0.1f), 0);
            decal.Scale = 2;
            decal.FadeTime = 15;

            Material mat = new Material();
            mat.NonLit = true;
            mat.Texture = pBlood;
            decal.Material = mat;

            World.Spawn(decal);
        }

        public override void OnDeleted()
        {
            base.OnDeleted();

            World.Destroy(shadowDecal);
        }

        public virtual void OnAttack(Entity ent)
        {

        }

        public override void OnUpdate()
        {
            //shadowDecal.Position = Position + new Vector3(0, 0.1f, 0);

            if(disappearTime < 0)
            {
                World.Destroy(this);
            }

            if (disappearTime != 0)
                disappearTime -= Game.Current.DeltaTime;
        }

        public override void OnDraw()
        {
            base.OnDraw();

            Game.Current.Graphics.DrawMesh(mesh, animator.Frame, mat, Position + new Vector3(0.5f, 0.5f, 0.5f),
                Rotation + new Vector3(0, -90, 0), new Vector3(GetScale(), GetScale(), GetScale()), animator.Time, Vector2.Zero);
            Game.Current.Graphics.DrawMesh(mesh, animator.Frame, shadowMat, Position + new Vector3(0.5f, 0.5f, 0.5f),
                Rotation + new Vector3(0, -90, 0), new Vector3(GetScale(), GetScale(), GetScale()), animator.Time, Vector2.Zero);

        }

    }
}
