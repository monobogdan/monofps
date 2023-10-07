using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public enum ItemType
    {
        Health,
        Ammo,
        Quad
    }

    public sealed class GroundItem : Entity
    {
        private static Mesh pAmmo;
        private static Material pAmmoMat;
        private static Mesh pHealth;
        private static Material pHealthMat;
        private static Mesh pQuad;
        private static Material pQuadMat;

        private static AudioStream pSoundTake;
        private static AudioStream pSoundTakeQuad;
        
        public ItemType ItemType
        {
            get;
            set;
        }

        private Mesh mesh;
        private World world;

        private static void Precache()
        {
            if (pAmmo == null)
            {
                pAmmo = Mesh.LoadMD2(GameMain.GetResource("models/ammo/tris.md2"));
                pHealth = Mesh.LoadMD2(GameMain.GetResource("models/health/tris.md2"));
                pQuad = Mesh.LoadMD2(GameMain.GetResource("models/quad/tris.md2"));

                pAmmoMat = new Material();
                pAmmoMat.Texture = Game.Current.Data.GetTexture("models/ammo/skin.jpg");
                pAmmoMat.NonLit = true;
                pHealthMat = new Material();
                pHealthMat.Texture = Game.Current.Data.GetTexture("models/health/skin.jpg");
                pHealthMat.NonLit = true;
                pQuadMat = new Material();
                pQuadMat.Texture = Game.Current.Data.GetTexture("models/ammo/skin.jpg");
                pQuadMat.NonLit = true;

                pSoundTake = AudioStream.LoadWav(GameMain.GetResource("sounds/pickup.wav"));
            }
        }

        public GroundItem(World world)
        {
            this.world = world;

            Precache();

            Position = new SharpDX.Vector3(0, -0.8f, 0);
            Box = new SharpDX.BoundingBox(new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1));
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Rotation += new SharpDX.Vector3(0, 290 * Game.Current.DeltaTime, 0);

            foreach(Entity ent in world.Entities)
            {
                BoundingBox eBox = new BoundingBox(ent.Position + ent.Box.Minimum, ent.Position + ent.Box.Maximum);
                BoundingBox cBox = new BoundingBox(Position + Box.Minimum, Position + Box.Maximum);

                if(ent is Player && eBox.Intersects(cBox))
                {
                    switch(ItemType)
                    {
                        case ItemType.Ammo:
                            world.Player.Weapons[world.Player.ActiveWeapon].Ammo += 20;
                            break;
                        case ItemType.Health:
                            world.Player.Health += 40;
                            break;
                    }

                    pSoundTake.Play();

                    world.Destroy(this);
                }
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();
            Mesh mesh = ItemType == ItemType.Ammo ? pAmmo : (ItemType == ItemType.Health ? pHealth : pQuad);
            Material material = ItemType == ItemType.Ammo ? pAmmoMat : (ItemType == ItemType.Health ? pHealthMat : pQuadMat);

            Game.Current.Graphics.DrawMesh(mesh, 0, material, Position + new Vector3(0, 0, 0), 
                Rotation, new SharpDX.Vector3(0.4f, 0.4f, 0.4f), 0, Vector2.Zero);
        }
    }
}
