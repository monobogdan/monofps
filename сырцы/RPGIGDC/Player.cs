using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    [Flags]
    public enum PlayerFlags
    {
        None,
        Moving
    }

    public sealed class Player : Entity
    {
        public const float FOVKick = 60.0f;

        public const float Speed = 30;
        public const float MaxShake = 0.3f;

        private World world;

        public float Health
        {
            get;
            set;
        }

        public PlayerFlags Flags
        {
            get;
            private set;
        }

        public List<Weapon> Weapons
        {
            get;
            private set;
        }

        public int ActiveWeapon
        {
            get;
            set;
        }

        private float fovBoostTime;

        public Player(World world)
        {
            this.world = world;

            Weapons = new List<Weapon>();

            //GiveWeapon(typeof(Rifle));
            //GiveWeapon(typeof(RocketLauncher));
            GiveWeapon(typeof(Shotgun));

            Health = 100;

            Position = new Vector3(5, 0, 0);
        }

        public bool HasWeapon(Type type)
        {
            foreach (Weapon weapon in Weapons)
            {
                if (weapon.GetType() == type)
                    return true;
            }

            return false;
        }

        public void GiveWeapon(Type type)
        {
            if (HasWeapon(type))
                return;

            Weapon weap = (Weapon)type.GetConstructor(new Type[] { typeof(World), typeof(Player) }).Invoke(new object[] { world, this });
            Weapons.Add(weap);
        }

        private void UpdateInput()
        {
            if(Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.LButton))
            {
                if (ActiveWeapon >= 0 && ActiveWeapon < Weapons.Count)
                    Weapons[ActiveWeapon].OnFire();
            }
            
            for(int i = 0; i < Weapons.Count; i++)
            {
                if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.D1 + i))
                {
                    if (i < Weapons.Count)
                        ActiveWeapon = i;
                }
            }
        }

        private void Move()
        {
            float inX = 0;
            float inY = 0;
            float speedMultipler = 1;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.A))
                inX = -1;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.D))
                inX = 1;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.W))
                inY = 1;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.S))
                inY = -1;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.ShiftKey))
                speedMultipler = 3;

            float dt = Game.Current.DeltaTime;
            Vector3 velocity = GetForward() * inY * (Speed * speedMultipler * Game.Current.DeltaTime);


            if (velocity.LengthSquared() != 0)
                Flags |= PlayerFlags.Moving;
            else
                Flags &= ~PlayerFlags.Moving;

            Rotation += new Vector3(0, inX * 2, 0);
            Position += velocity;

            Position = new Vector3(Mathf.Clamp(Position.X, world.Map.Bounds.Minimum.X, world.Map.Bounds.Maximum.X), 0,
                                   Mathf.Clamp(Position.Z, world.Map.Bounds.Minimum.Z, world.Map.Bounds.Maximum.Z));
            
        }

        private void UpdateFOVBoost()
        {
            if (Flags.HasFlag(PlayerFlags.Moving))
            {
                fovBoostTime = Mathf.Clamp(fovBoostTime + (Game.Current.DeltaTime * 15), 0, 1);
            }
            else
            {
                fovBoostTime = Mathf.Clamp(fovBoostTime - (Game.Current.DeltaTime * 15), 0, 1);
            }

            Game.Current.Graphics.Camera.FOV = 60 + (fovBoostTime * 5);
        }

        public override void OnUpdate()
        {
            UpdateInput();
            Move();

            UpdateFOVBoost();

            Game.Current.Graphics.Camera.Position = Position;
            Game.Current.Graphics.Camera.Rotation = Rotation;
        }

        public override void OnDrawHUD()
        {
            if (ActiveWeapon >= 0 && ActiveWeapon < Weapons.Count)
            {
                Weapons[ActiveWeapon].OnDraw();
            }
        }
    }
}
