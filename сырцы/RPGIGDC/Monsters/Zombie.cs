using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public sealed class Zombie : Monster
    {


        private static Mesh pMesh;
        private static Material pMaterial;
        private static AudioStream[] pHitSound;

        private static void Precache()
        {
            if (pMesh == null)
            {
                pMesh = Mesh.LoadMD2(GameMain.GetResource("models/slith/tris.md2"));
                pMaterial = new Material();
                pMaterial.Texture = Game.Current.Data.GetTexture("models/slith/skin.jpg");

                pHitSound = new AudioStream[2];
                pHitSound[0] = AudioStream.LoadWav(GameMain.GetResource("sounds/hit2.wav"));
                pHitSound[0] = AudioStream.LoadWav(GameMain.GetResource("sounds/hit1.wav"));
            }
        }

        public Zombie(World world) : base(world)
        {
            Precache();

            mesh = pMesh;
            mat = pMaterial;
            //mat.Flags |= MaterialFlags.ShadowMesh;


            MaxHealth = 50;
            Health = MaxHealth;
            Speed = 35;

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
            }, "attack");

            animator.AddSequence(new MeshSequence()
            {
                From = 80,
                To = 85,
                Speed = 0.2f
            }, "damage");

            animator.AddSequence(new MeshSequence()
            {
                From = 190,
                To = 197,
                Speed = 0.2f
            }, "kill");

            animator.AttachEventToSequence("kill", () =>
            {
                World.Destroy(this);
            });

            animator.AddSequence(new MeshSequence()
            {
                From = 40,
                To = 45,
                Speed = 0.14f
            }, "walk");
        }

        public override void OnDamage(float amount)
        {
            base.OnDamage(amount);

            Health -= amount;
            

            CreateDecal();

            if (animator.CurrentSequence != "kill")
            {
                Random rand = new Random();
                pHitSound[rand.Next(0, pHitSound.Length - 1)].Play();

                if (Health < 0)
                {
                    animator.Play("kill");

                    return;
                }

                if(animator.CurrentSequence != "damage")
                    animator.Play("damage");
            }
        }

        public override void OnAttack(Entity ent)
        {
            base.OnAttack(ent);

            Player player = (Player)ent; 
            player.Health -= 10;
            animator.Play("attack");
        }

        protected override float GetScale()
        {
            return 0.5f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            animator.Update();

            if (!animator.IsPlaying)
                animator.Play("walk");

            if (animator.CurrentSequence != "damage" && animator.CurrentSequence != "kill")
                MoveToPlayer();
        }
    }
}
