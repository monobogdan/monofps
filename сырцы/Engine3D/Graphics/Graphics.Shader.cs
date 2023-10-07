using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;

namespace Engine3D
{
    public sealed class Shader
    {
        internal Effect Effect
        {
            get;
            private set;
        }

        public Shader(string internalName, Stream strm)
        {
            try
            {
                Effect = Effect.FromString(Game.Current.Graphics.Device, new StreamReader(strm).ReadToEnd(), ShaderFlags.None);

                Effect.Technique = Effect.GetTechnique(0);

                Game.Current.Log.Print("Successfuly compiled shader {0}", internalName);
            } catch(Exception e)
            {
                Game.Current.Log.Print("Failed to compile shader {0}({1}). Using default one.", internalName, e.Message);
            }
        }

        internal void Apply()
        {
            Effect.Begin();
            Effect.BeginPass(0);
        }

        private void ApplyFromMaterial(Material mat)
        {
            if(mat.Texture != null)
            {
                Effect.SetValue<int>(Effect.GetParameter(null, "primary"), 0);
                Effect.SetValue<int>(Effect.GetParameter(null, "detail1"), 1);
                Effect.SetValue<int>(Effect.GetParameter(null, "detail2"), 2);
            }
        }

        internal void Release()
        {
            Effect.EndPass();
            Effect.End();
        }
    }
}
