using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace Engine3D
{
    public struct Material
    {
        public Shader Shader; // Null for default

        public Texture Texture;
        public Texture Detail;
        public Texture Detail2;
        public Texture Reflection;

        public MaterialDetailType DetailType;
        public MaterialFlags Flags;

        public bool NonLit;
        public bool SampleShadowMap;

        public Color4 Color;
        public bool DepthTest;
        public bool DepthWrite; // false - write

        public Vector2 UVScale;

        public static Material CreateDefault()
        {
            return new Material()
            {
                Color = new Color4(1, 1, 1, 1)
            };
        }
    }

    public partial class Graphics
    {

        private void SetRenderState(Material mat)
        {
            device.SetRenderState<ZBufferType>(RenderState.ZEnable, !mat.DepthTest ? ZBufferType.UseZBuffer : ZBufferType.DontUseZBuffer);
            device.SetRenderState(RenderState.ZWriteEnable, !mat.DepthWrite);
        }

        private void SetShadowMeshState()
        {
            // Special case for shadow meshes
            device.SetRenderState(RenderState.ZWriteEnable, false);
            device.SetRenderState(RenderState.Lighting, false);

            device.SetRenderState(RenderState.TextureFactor, new Color4(0, 0, 0, 0.9f).ToBgra());
            device.SetTextureStageState(0, TextureStage.Constant, new Color4(0.1f, 0.1f, 0.1f, 1).ToBgra());
            device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
            device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.TFactor);
            device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.TFactor);

            device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Disable);
            device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Disable);
        }

        private void SetTextureState(Material mat)
        {
            for (int i = 1; i < 4; i++)
            {
                device.SetTextureStageState(i, TextureStage.ColorOperation, TextureOperation.Disable);
                device.SetTextureStageState(i, TextureStage.AlphaOperation, TextureOperation.Disable);
                
                device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);

                device.SetSamplerState(i, SamplerState.MinFilter, TextureFilter.Linear);
                device.SetSamplerState(i, SamplerState.MagFilter, TextureFilter.Anisotropic);
                device.SetSamplerState(i, SamplerState.MipFilter, TextureFilter.Linear);
            }

            device.SetRenderState(RenderState.TextureFactor, mat.Color.ToRgba());

            if (mat.DetailType != MaterialDetailType.BlendMask)
            {
                if (mat.Texture != null)
                {
                    device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                    device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                    device.SetTextureStageState(0, TextureStage.ColorArg0, TextureArgument.Current);
                    device.SetTexture(0, mat.Texture.Handle);

                    if (mat.UVScale == Vector2.Zero)
                        mat.UVScale = new Vector2(1, 1);

                    device.SetTextureStageState(0, TextureStage.TextureTransformFlags, TextureTransform.Count2);
                    device.SetTransform(TransformState.Texture0, Matrix.Scaling(mat.UVScale.X, mat.UVScale.Y, 1));
                    device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Current);

                    device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                    device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
                }
            }
            else
            {
                if (mat.Texture != null)
                {
                    device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                    device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                    device.SetTextureStageState(0, TextureStage.ColorArg0, TextureArgument.Current);
                    device.SetTexture(0, mat.Texture.Handle);

                    if (mat.UVScale == Vector2.Zero)
                        mat.UVScale = new Vector2(1, 1);

                    device.SetTextureStageState(0, TextureStage.TextureTransformFlags, TextureTransform.Count2);
                    device.SetTransform(TransformState.Texture0, Matrix.Scaling(mat.UVScale.X, mat.UVScale.Y, 1));
                    device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Temp);

                    device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                    device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
                }
            }

            if (mat.Detail != null)
            {
                if (mat.DetailType == MaterialDetailType.BlendMask)
                {
                    device.SetTexture(1, mat.Detail.Handle);

                    device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.SelectArg1);

                    device.SetTextureStageState(1, TextureStage.ColorArg0, TextureArgument.Current);
                    device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Texture);
                    device.SetTextureStageState(1, TextureStage.ResultArg, TextureArgument.Temp);
                    device.SetTextureStageState(1, TextureStage.TexCoordIndex, 0);

                    device.SetSamplerState(1, SamplerState.MinFilter, TextureFilter.Linear);
                    device.SetSamplerState(1, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(1, SamplerState.MipFilter, TextureFilter.Linear);
                }
                else
                {
                    device.SetTexture(1, mat.Detail.Handle);

                    device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Modulate);

                    device.SetTextureStageState(1, TextureStage.ColorArg0, TextureArgument.Current);
                    device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Texture);
                    device.SetTextureStageState(1, TextureStage.ResultArg, TextureArgument.Current);
                    device.SetTextureStageState(1, TextureStage.TexCoordIndex, 0);

                    device.SetSamplerState(1, SamplerState.MinFilter, TextureFilter.Linear);
                    device.SetSamplerState(1, SamplerState.MagFilter, TextureFilter.Anisotropic);
                    device.SetSamplerState(1, SamplerState.MipFilter, TextureFilter.Linear);
                }
            }
        }
    }
}
