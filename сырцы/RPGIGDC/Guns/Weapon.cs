using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    [Flags]
    public enum WeaponFlags
    {
        None,
        ShootLastFrame = 1
    }

    public class Weapon
    {
        private static Texture pMuzzleTexture;

        public const float MaxShake = 0.1f;
        public const float ShakeSpeed = 0.2f;

        private WeaponFlags flags;

        protected World world;

        protected Mesh mesh;
        protected Material material;

        private float shakeTime;
        private float shakeFactor;

        public int Ammo
        {
            get;
             set;
        }

        protected Player owner;

        protected MeshAnimator Animator
        {
            get;
            private set;
        }

        public Weapon(World world, Player owner)
        {
            this.world = world;
            this.owner = owner;

            if (pMuzzleTexture == null)
                pMuzzleTexture = Game.Current.Data.GetTexture("muzzle.png");

            Animator = new MeshAnimator();
        }

        public virtual void OnFire()
        {
            flags |= WeaponFlags.ShootLastFrame;
        }

        public virtual void OnFire2()
        {

        }

        protected SpriteEffect CreateMuzzle()
        {
            SpriteEffect effect = new SpriteEffect(world);

            effect.Texture = pMuzzleTexture;
            effect.Speed = 0.5f;
            effect.Size = 105;
            effect.Position = GetViewOffset() + (world.Player.GetForward() * 0.5f) + new Vector3(0, 0, 0);
            world.Spawn(effect);

            return effect;
        }

        private Vector3 GetViewOffset()
        {
            Vector3 dir = world.Player.GetForward();
            Vector3 viewDir = (world.Player.GetRight() * -0.3f) + ((dir) * (0.8f + shakeFactor));
            Vector3 viewPos = Game.Current.Graphics.Camera.Position + viewDir + new Vector3(0, -0.4f, 0);

            return viewPos;
        }

        public virtual void OnDraw()
        {
            Animator.Update();

            if(owner.Flags.HasFlag(PlayerFlags.Moving))
            {
                shakeFactor = (float)Math.Sin(shakeTime) * MaxShake;
                shakeTime += ShakeSpeed;
            }
            else
            {
                shakeFactor = 0;
            }

            Vector3 rot = Game.Current.Graphics.Camera.Rotation + new Vector3(0, -90, 0);

            material.DepthWrite = false;
            material.DepthTest = false;

            Game.Current.Graphics.DrawMesh(mesh, Animator.Frame, material, GetViewOffset(), rot, new Vector3(0.3f, 0.3f, 0.3f), Animator.Time, Vector2.Zero);
        }
    }

    public class Shotgun : Weapon
    {
        private const float ProjectileSpeed = 0.8f;

        public static Mesh pMesh;
        public static Material pMaterial;

        private static Shader pShader;

        private static AudioStream shoot;

        public static void Precache()
        {
            if (shoot == null)
            {
                shoot = AudioStream.LoadWav(GameMain.GetResource("sounds/shotgun.wav"));
                pMesh = Mesh.LoadMD2(GameMain.GetResource("models/shotgun/tris.md2"));

                pMaterial = new Material();
                pMaterial.DepthWrite = true;
                pMaterial.Texture = Game.Current.Data.GetTexture("models/shotgun/skin.jpg");
               
            }
        }

        public Shotgun(World world, Player owner) : base(world, owner)
        {
            Precache();

            shoot.Volume = 0.5f;
            
            Animator.AddSequence(new MeshSequence()
            {
                From = 5,
                To = 25,
                Speed = 0.45f
            }, "deploy");

            Ammo = 15;

            mesh = pMesh;
            material = pMaterial;
        }

        public override void OnFire()
        {
            base.OnFire();

            if(!shoot.IsPlaying && Ammo > 0)
            {
                shoot.Play();

                Projectile proj = new Projectile(world, null, owner.Position + (owner.GetForward()), world.Player.Rotation, material, ProjectileSpeed, 55,
                new BoundingBox(Vector3.Zero, new Vector3(1, 1, 1)), ProjectileType.Normal);
                world.Spawn(proj);

                CreateMuzzle();

                Ammo--;

                Animator.Play("deploy");
            }
        }
    }

    public class Rifle : Weapon
    {
        private const float ProjectileSpeed = 0.8f;

        private static Mesh pMesh;
        private static Material pMaterial;



        private static AudioStream shoot;

        public static void Precache()
        {
            if (shoot == null)
            {
                shoot = AudioStream.LoadWav(GameMain.GetResource("sounds/rifle.wav"));
                pMesh = Mesh.LoadMD2(GameMain.GetResource("models/rifle/tris.md2"));

                pMaterial = new Material();
                pMaterial.DepthWrite = true;
                pMaterial.Texture = Game.Current.Data.GetTexture("models/rifle/skin.jpg");
            }
        }

        public Rifle(World world, Player owner) : base(world, owner)
        {
            Precache();

            shoot.Volume = 0.5f;

            Animator.AddSequence(new MeshSequence()
            {
                From = 5,
                To = 25,
                Speed = 0.45f
            }, "deploy");

            Ammo = 15;

            mesh = pMesh;
            material = pMaterial;
        }

        public override void OnFire()
        {
            base.OnFire();

            if (!shoot.IsPlaying && Ammo > 0)
            {
                shoot.Play();

                Projectile proj = new Projectile(world, null, owner.Position + (owner.GetForward() * 2), world.Player.Rotation, material, ProjectileSpeed, 90,
                new BoundingBox(Vector3.Zero, new Vector3(1.5f, 1.5f, 1.5f)), ProjectileType.Normal);
                world.Spawn(proj);

                CreateMuzzle();

                Ammo--;

                Animator.Play("deploy");
            }
        }
    }

    public class MachineGun : Weapon
    {
        private const float ProjectileSpeed = 0.7f;

        private static Mesh pMesh;
        private static Material pMaterial;

        private float nextAttack;

        private static AudioStream shoot;

        public static void Precache()
        {
            if (shoot == null)
            {
                shoot = AudioStream.LoadWav(GameMain.GetResource("sounds/minigun_shoot.wav"));
                pMesh = Mesh.LoadMD2(GameMain.GetResource("models/chaingun/tris.md2"));

                pMaterial = new Material();
                pMaterial.DepthWrite = true;
                pMaterial.Texture = Game.Current.Data.GetTexture("models/chaingun/skin.jpg");
            }
        }

        public MachineGun(World world, Player owner) : base(world, owner)
        {
            Precache();

            shoot.Volume = 0.8f;

            Animator.AddSequence(new MeshSequence()
            {
                From = 5,
                To = 25,
                Speed = 0.45f
            }, "deploy");

            Animator.AddSequence(new MeshSequence()
            {
                From = 10,
                To = 20,
                Speed = 0.7f
            }, "fire");

            Ammo = 120;

            mesh = pMesh;
            material = pMaterial;
        }

        public override void OnFire()
        {
            base.OnFire();

            nextAttack -= 0.03f;

            if (nextAttack < 0 && Ammo > 0)
            {
                if (!shoot.IsPlaying)
                    shoot.Play();

                Projectile proj = new Projectile(world, null, owner.Position + (owner.GetForward() * 2), world.Player.Rotation, material, ProjectileSpeed, 10,
                new BoundingBox(Vector3.Zero, new Vector3(0.3f, 1, 0.3f)), ProjectileType.Normal);
                world.Spawn(proj);

                CreateMuzzle();

                Ammo--;
                nextAttack = 0.05f;

                if (!Animator.IsPlaying)
                    Animator.Play("fire");
            }
        }
    }

    public class RocketLauncher : Weapon
    {
        private const float ProjectileSpeed = 0.8f;

        private static Mesh pMesh;
        private static Material pMaterial;

        private static Mesh pProj;
        private static Material pProjMaterial;

        private static AudioStream shoot;

        public static void Precache()
        {
            if (shoot == null)
            {
                shoot = AudioStream.LoadWav(GameMain.GetResource("sounds/shotgun.wav"));
                pMesh = Mesh.LoadMD2(GameMain.GetResource("models/rocket/tris.md2"));

                pMaterial = new Material();
                pMaterial.DepthWrite = true;
                pMaterial.Texture = Game.Current.Data.GetTexture("models/rocket/skin.jpg");

                pProj = Mesh.LoadMD2(GameMain.GetResource("models/rocket_proj/tris.md2"));
                pProjMaterial = new Material();
                pProjMaterial.Texture = Game.Current.Data.GetTexture("models/rocket_proj/skin.jpg");
            }
        }

        public RocketLauncher(World world, Player owner) : base(world, owner)
        {
            Precache();

            shoot.Volume = 0.5f;

            Animator.AddSequence(new MeshSequence()
            {
                From = 5,
                To = 25,
                Speed = 0.45f
            }, "deploy");

            Ammo = 5;

            mesh = pMesh;
            material = pMaterial;
        }

        public override void OnFire()
        {
            base.OnFire();

            if (!shoot.IsPlaying && Ammo > 0)
            {
                shoot.Play();

                Projectile proj = new Projectile(world, pProj, owner.Position + (owner.GetForward() * 3), world.Player.Rotation, pProjMaterial, ProjectileSpeed, 55,
                new BoundingBox(Vector3.Zero, new Vector3(1.5f, 1.5f, 1.5f)), ProjectileType.Explosive);
                world.Spawn(proj);

                Ammo--;

                Animator.Play("deploy");
            }
        }
    }
}
