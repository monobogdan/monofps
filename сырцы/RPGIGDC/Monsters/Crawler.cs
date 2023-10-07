using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    public sealed class Crawler : Monster
    {
        const int NumKilledAnims = 1;

        private static Mesh pMesh;
        private static Material pMaterial;
        private static AudioStream[] pHitSound;

        private static void Precache()
        {
            if (pMesh == null)
            {
                pMesh = Game.Current.Data.GetMesh("models/knight/tris.md2");
                pMaterial = new Material();
                pMaterial.Texture = Game.Current.Data.GetTexture("models/knight/skin.jpg");

                pHitSound = new AudioStream[2];
                pHitSound[0] = AudioStream.LoadWav(GameMain.GetResource("sounds/hit2.wav"));
                pHitSound[0] = AudioStream.LoadWav(GameMain.GetResource("sounds/hit1.wav"));
            }
        }

        public Crawler(World world) : base(world)
        {
            Precache();

            mesh = pMesh;
            mat = pMaterial;

            MaxHealth = 20;
            Speed = 50;
            Health = MaxHealth;

            animator.AddSequence(new MeshSequence()
            {
                From = 0,
                To = 30,
                Speed = 0.5f
            }, "idle");

            animator.AddSequence(new MeshSequence()
            {
                From = 80,
                To = 90,
                Speed = 0.2f
            }, "damage");

            animator.AddSequence(new MeshSequence()
            {
                From = 160,
                To = 165,
                Speed = 0.2f
            }, "attack");

            animator.AddSequence(new MeshSequence()
            {
                From = 190,
                To = 197,
                Speed = 0.2f
            }, "kill");

            animator.AddSequence(new MeshSequence()
            {
                From = 180,
                To = 187,
                Speed = 0.2f
            }, "kill1");

            animator.AttachEventToSequence("kill", () =>
            {
                World.Destroy(this);
            });
            animator.AttachEventToSequence("kill1", () =>
            {
                World.Destroy(this);
            });

            animator.AddSequence(new MeshSequence()
            {
                From = 40,
                To = 45,
                Speed = 0.15f
            }, "walk");
        }

        protected override float GetScale()
        {
            return 0.2f;
        }

        public override void OnAttack(Entity ent)
        {
            base.OnAttack(ent);

            if (animator.CurrentSequence != "attack")
            {
                animator.Play("attack");

                Player player = (Player)ent;
                player.Health -= 3;
            }
        }

        public override void OnDamage(float amount)
        {
            base.OnDamage(amount);

            Health -= amount;
            Random rand = new Random();
            pHitSound[rand.Next(0, pHitSound.Length - 1)].Play();

            CreateDecal();

            if (animator.CurrentSequence != "kill")
            {
                if (Health < 0)
                {
                    animator.Play("kill" );

                    return;
                }

                if(animator.CurrentSequence != "damage")
                    animator.Play("damage");
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            animator.Update();

            if (!animator.IsPlaying)
                animator.Play("walk");

            if (animator.CurrentSequence != "damage" && !animator.CurrentSequence.Contains("kill"))
                MoveToPlayer();
        }
    }
}
