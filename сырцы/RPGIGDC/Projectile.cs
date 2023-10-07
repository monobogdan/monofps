using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    public enum ProjectileType
    {
        Normal,
        Explosive
    }

    public sealed class Projectile : Entity
    {
        private World world;

        private AudioStream pExplosion;

        private Mesh mesh;
        private Material material;
        private float speed;
        private float damage;
        private float lifeTime;
        private ProjectileType type;

        public Projectile(World world, Mesh mesh, Vector3 pos, Vector3 rot,
            Material material, float speed, float damage, BoundingBox box, ProjectileType type)
        {
            this.world = world;

            if (pExplosion == null)
                pExplosion = AudioStream.LoadWav(GameMain.GetResource("sounds/explosion.wav"));

            this.mesh = mesh;
            this.material = material;
            this.speed = speed;
            this.damage = damage;
            this.type = type;

            lifeTime = 0.17f;

            Position = pos;
            Rotation = rot;
            Box = box;
        }

        private void Explode()
        {
            foreach (Entity ent in world.Entities)
            {
                if (Vector3.Distance(Position, ent.Position) < 15)
                    ent.OnDamage(60);
            }

            
            pExplosion.Play();

            world.Destroy(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Position += GetForward() * speed;

            lifeTime -= Game.Current.DeltaTime;

            if(lifeTime < 0)
            {
                world.Destroy(this);

                return;
            }

            if (Position.Y < -1 && type == ProjectileType.Explosive)
                Explode();

            foreach(Entity ent in world.Entities)
            {
                if (ent is Monster)
                {
                    BoundingBox entBox = new BoundingBox(ent.Position + ent.Box.Minimum, ent.Position + ent.Box.Maximum);
                    BoundingBox box = new BoundingBox(Position + Box.Minimum, Position + Box.Maximum);

                    if (entBox.Contains(box) == ContainmentType.Intersects)
                    {
                        if (type == ProjectileType.Normal)
                        {
                            world.Destroy(this);
                            ent.OnDamage(damage);
                        }
                        else
                        {
                            Explode();
                        }

                        break;
                    }
                }
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if(mesh != null)
                Game.Current.Graphics.DrawMesh(mesh, 0, material, Position, Rotation);
        }
    }
}
