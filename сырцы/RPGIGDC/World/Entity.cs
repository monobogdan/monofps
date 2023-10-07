using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Engine3D;

namespace RPGIGDC
{
    public abstract class Entity
    {
        public Vector3 Position
        {
            get;
            set;
        }

        public Vector3 Rotation
        {
            get;
            set;
        }

        public BoundingBox Box
        {
            get;
            protected set;
        }

        private Engine3D.Ray[] bboxRays;

        public Vector3 GetForward()
        {
            return new Vector3((float)Math.Sin(Rotation.Y * Mathf.DegToRad), 0, (float)Math.Cos(Rotation.Y * Mathf.DegToRad));
        }

        public Vector3 GetRight()
        {
            Vector3 v = GetForward();

            return Vector3.Cross(v, Vector3.Up);
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnDraw()
        {
            /*BoundingBox box = new BoundingBox(Position + Box.Minimum, Position + Box.Maximum);

            if (bboxRays == null)
                bboxRays = new Engine3D.Ray[4];

            bboxRays[0] = new Engine3D.Ray()
            {
                From = new Vector3(box.Minimum.X, box.Minimum.Y, box.Minimum.Z),
                To = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z)
            };
            bboxRays[1] = new Engine3D.Ray()
            {
                From = new Vector3(box.Minimum.X, box.Minimum.Y, box.Minimum.Z),
                To = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z)
            };
            bboxRays[2] = new Engine3D.Ray()
            {
                From = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z),
                To = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z)
            };
            bboxRays[3] = new Engine3D.Ray()
            {
                From = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z),
                To = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z)
            };

            Game.Current.Graphics.DrawLines(bboxRays);*/
        }

        public virtual void OnDrawTransparent()
        {

        }

        public virtual void OnDrawHUD()
        {

        }

        public virtual void OnDeleted()
        {

        }

        public virtual void OnDamage(float amount)
        {

        }
    }
}
