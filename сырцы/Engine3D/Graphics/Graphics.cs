using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;

namespace Engine3D
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
    }

    public sealed class Texture
    {
        internal SharpDX.Direct3D9.BaseTexture Handle
        {
            get;
            private set;
        }

        internal string InternalName
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public Texture()
        {

        }

        private Texture(SharpDX.Direct3D9.Texture tex)
        {
            Handle = tex;
            SurfaceDescription desc = tex.GetLevelDescription(0);

            Width = desc.Width;
            Height = desc.Height;
        }

        private Texture(SharpDX.Direct3D9.CubeTexture tex)
        {
            Handle = tex;
            SurfaceDescription desc = tex.GetCubeMapSurface(CubeMapFace.NegativeX, 0).Description;

            Width = desc.Width;
            Height = desc.Height;
        }

        // Create rendertarget
        public Texture(int width, int height)
        {
            Handle = new SharpDX.Direct3D9.Texture(Game.Current.Graphics.Device, width,
                    height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
        }

        ~Texture()
        {
            if (Handle != null)
            {
                Handle.Dispose();

                Game.Current.Log.Print("Freed texture " + InternalName);
            }
        }

        public void Upload(IntPtr pixels, int width, int height, bool dynamic, bool doNotRecreate)
        {
            if(pixels != IntPtr.Zero)
            {
                if (Handle != null && !doNotRecreate)
                    Handle.Dispose();

                if (!doNotRecreate || Handle == null)
                {
                    Handle = new SharpDX.Direct3D9.Texture(Game.Current.Graphics.Device, width,
                        height, 0, Usage.None, Format.A8R8G8B8, Pool.Default);
                }
                
                var tmp = new SharpDX.Direct3D9.Texture(Game.Current.Graphics.Device, width,
                    height, 0, Usage.None, Format.A8R8G8B8, Pool.SystemMemory);

                DataRectangle rect = tmp.LockRectangle(0, LockFlags.None);
                DataStream strm = new DataStream(new DataPointer(rect.DataPointer, width * height * 4));
                strm.WriteRange(pixels, width * height * 4);
                strm.Close();
                tmp.UnlockRectangle(0);

                Game.Current.Graphics.Device.UpdateTexture(tmp, Handle);
                tmp.Dispose();

                Width = width;
                Height = height;
            }
        }

        public static Texture FromStream(Stream strm, bool isCube = false)
        {
            if(!isCube)
                return new Texture(SharpDX.Direct3D9.Texture.FromStream(Game.Current.Graphics.Device, strm, Usage.None, Pool.Default));
            else
                return new Texture(SharpDX.Direct3D9.CubeTexture.FromStream(Game.Current.Graphics.Device, strm, Usage.None, Pool.Default));
        }
    }

    public enum MaterialDetailType
    {
        Default,
        BlendMask
    }

    public enum SpriteDrawMode
    {
        Default,
        Modulate
    }

    [Flags]
    public enum MaterialFlags
    {
        None,
        ShadowMesh = 1
    }

    

    public sealed class Camera
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

        public float FOV
        {
            get;
            set;
        }

        public float Aspect
        {
            get;
            set;
        }

        public Camera()
        {
            FOV = 60;
            Aspect = 4.0f / 3;
        }
    }

    public sealed class Mesh
    {
        internal Vertex[][] Frames
        {
            get;
            private set;
        }

        internal Vertex[] MorphVerts
        {
            get;
            private set;
        }

        public Mesh()
        {

        }

        public void Upload(Vertex[][] frames)
        {
            if (frames != null)
            {
                Frames = (Vertex[][])frames.Clone();

                MorphVerts = new Vertex[frames[0].Length];
            }
        }

        public void UpdateShadowVolume()
        {
            /*float y = float.MinValue;

            for(int i = 0; i < MorphVerts.Length; i++)
            {
                if (MorphVerts[i].Position.Y > y)
                    y = ShadowVolume[i].Position.Y;
            }

            for(int i = 0; i < MorphVerts.Length; i++)
            {
                Vector3 pos = MorphVerts[i].Position;
                float vFactor = Mathf.Abs(pos.Y) / Mathf.Abs(y);

                ShadowVolume[i].Position = new Vector3(pos.X, 0, pos.Z * vFactor);
            }*/
        }

        internal void UpdateMorphTarget(int currFrame, int nextFrame, float time)
        {
            for(int i = 0; i < MorphVerts.Length; i++)
            {
                MorphVerts[i].Position = Vector3.Lerp(Frames[currFrame][i].Position, Frames[nextFrame][i].Position, time);
                MorphVerts[i].Normal = Vector3.Lerp(Frames[currFrame][i].Normal, Frames[nextFrame][i].Normal, time);
                MorphVerts[i].UV = Frames[currFrame][i].UV;
            }
        }

        public static Mesh LoadSmd(Stream stream)
        {
            SmdMesh mesh = new SmdMesh(stream);
            
            Mesh ret = new Mesh();
            Vertex[] verts = new Vertex[mesh.Triangles.Count * 3];

            for (int i = 0; i < mesh.Triangles.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    verts[i * 3 + j].Position = mesh.Triangles[i].Verts[j].Position;
                    verts[i * 3 + j].Normal = mesh.Triangles[i].Verts[j].Normal;
                    verts[i * 3 + j].UV = mesh.Triangles[i].Verts[j].UV;
                }
            }
            
            ret.Upload(new Vertex[][] { verts });

            return ret;
        }

        public static Mesh LoadMD2(Stream stream)
        {
            const float scale = 0.09f;

            // MD2 got serious compression errors, that's why bounding boxes differ from real models.
            MD2Model model = new MD2Model(stream);
            
            Mesh ret = new Mesh();
            Vertex[][] frames = new Vertex[model.Frames.Length][];

            for(int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Vertex[model.Triangles.Length * 3];

                for (int j = 0; j < model.Triangles.Length; j++)
                {
                    frames[i][j * 3].Position = new Vector3(model.Frames[i].verts[model.Triangles[j].verts[0]].x * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[0]].z * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[0]].y * scale);
                    frames[i][j * 3].UV = new Vector2((float)model.Coords[model.Triangles[j].uv[0]].u / model.Header.skinwidth,
                        ((float)model.Coords[model.Triangles[j].uv[0]].v / model.Header.skinheight));
                    frames[i][j * 3].Normal = MD2Normal.Normals[model.Frames[i].verts[model.Triangles[j].verts[0]].normalIndex];

                    frames[i][j * 3 + 2].Position = new Vector3(model.Frames[i].verts[model.Triangles[j].verts[1]].x * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[1]].z * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[1]].y * scale);
                    frames[i][j * 3 + 2].UV = new Vector2((float)model.Coords[model.Triangles[j].uv[1]].u / model.Header.skinwidth,
                            ((float)model.Coords[model.Triangles[j].uv[1]].v / model.Header.skinheight));
                    frames[i][j * 3 + 2].Normal = MD2Normal.Normals[model.Frames[i].verts[model.Triangles[j].verts[0]].normalIndex];

                    frames[i][j * 3 + 1].Position = new Vector3(model.Frames[i].verts[model.Triangles[j].verts[2]].x * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[2]].z * scale,
                        model.Frames[i].verts[model.Triangles[j].verts[2]].y * scale);
                    frames[i][j * 3 + 1].UV = new Vector2((float)model.Coords[model.Triangles[j].uv[2]].u / model.Header.skinwidth,
                        ((float)model.Coords[model.Triangles[j].uv[2]].v / model.Header.skinheight));
                    frames[i][j * 3 + 1].Normal = MD2Normal.Normals[model.Frames[i].verts[model.Triangles[j].verts[0]].normalIndex];
                }
            }

            ret.Upload(frames);

            return ret;
        }
    }

    public struct Ray
    {
        public Vector3 From;
        public Vector3 To;
        public Color4 Color;
    }

    public partial class Graphics
    {
        private Direct3D d3d;
        private Device device;

        internal Device Device
        {
            get
            {
                return device;
            }
        }

        public Camera Camera
        {
            get;
            private set;
        }

        private Surface defaultRt;
        public Texture shadowMap;

        private VertexDeclaration vDecl;

        internal Graphics()
        {
            d3d = new Direct3D();

            CreateDevice();
            Camera = new Camera();
        }

        private void CreateDevice()
        {
            int msaa = PickBestMSAALevel(Game.Current.Config.GetInt("renderer.msaa"));

            bool fullScreen = Game.Current.Config.GetBool("renderer.fullScreen");

            PresentParameters pp = new PresentParameters();
            pp.AutoDepthStencilFormat = Format.D24S8;
            pp.BackBufferCount = 1;
            pp.BackBufferWidth = fullScreen ? Game.Current.Config.GetInt("engine.width") : Game.Current.Width;
            pp.BackBufferHeight = fullScreen ? Game.Current.Config.GetInt("engine.height") : Game.Current.Height;
            pp.BackBufferFormat = !fullScreen ? Format.Unknown : Format.X8R8G8B8;
            pp.DeviceWindowHandle = Game.Current.Form.Handle;
            pp.EnableAutoDepthStencil = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = !fullScreen;
            pp.MultiSampleType = (MultisampleType)msaa;
            pp.FullScreenRefreshRateInHz = 0;
            pp.MultiSampleQuality = 0;
            pp.PresentFlags = PresentFlags.None;
            pp.PresentationInterval = PresentInterval.Default;
            pp.SwapEffect = SwapEffect.Discard;
            device = new Device(d3d, 0, DeviceType.Hardware, Game.Current.Form.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.PureDevice, pp);

            Game.Current.Log.Print("Initialized graphics");
            Game.Current.Log.Print("Total video memory: " + device.AvailableTextureMemory / 1024 / 1024);
        }

        private int PickBestMSAALevel(int level)
        {
            int ret = level;

            for (int i = level; i > 1; i--)
            {
                if (d3d.CheckDeviceMultisampleType(0, DeviceType.Hardware, Format.Unknown,
                    !Game.Current.Config.GetBool("renderer.fullScreen"), (MultisampleType)i))
                {
                    ret = i;

                    continue;
                }
            }

            if(ret != level)
                Game.Current.Log.Print("Picked MSAA x" + ret + " , as your GPU doesn't support desired level");

            return ret;
        }

        internal void CreateResources()
        {
            defaultRt = Device.GetRenderTarget(0);

            /*vDecl = new VertexDeclaration(Device, new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            });

            device.VertexDeclaration = vDecl;*/
            device.VertexFormat = VertexFormat.Position | VertexFormat.Normal
                    | VertexFormat.Texture1;
        }

        private void SetupLighting()
        {
            device.EnableLight(0, true);

            SharpDX.Direct3D9.Material mat = new SharpDX.Direct3D9.Material();
            mat.Ambient = new Color4(0.95f, 0.91f, 0.95f, 1);
            mat.Diffuse = new Color4(1, 1, 1, 1);
            device.Material = mat;

            Light sun = new Light();
            sun.Direction = new Vector3(0.5f, 0.2f, 0.3f);
            sun.Ambient = new Color4(0.95f, 0.91f, 0.95f, 1);
            sun.Diffuse = new Color4(1, 1, 1, 1);
            sun.Type = LightType.Directional;
            sun.Specular = new Color4(1, 1, 1, 1);

            device.SetLight(0, ref sun);
        }

        private void SetupFog()
        {
            device.SetRenderState(RenderState.FogEnable, true);
            device.SetRenderState(RenderState.FogStart, 0.5f);
            device.SetRenderState(RenderState.FogEnd, 0.8f);
            device.SetRenderState(RenderState.FogDensity, 0.02f);
            device.SetRenderState(RenderState.FogColor, new Color(194, 178, 128, 255).ToRgba());
            //device.SetRenderState<FogMode>(RenderState.FogTableMode, FogMode.Linear);
            device.SetRenderState<FogMode>(RenderState.FogVertexMode, FogMode.Exponential);
        }
        

        internal void BeginScene()
        {
            device.BeginScene();
            
            device.SetRenderTarget(0, defaultRt);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new ColorBGRA(0, 0, 255, 255), 1.0f, 0);
                
                
            device.SetRenderState<Blend>(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState<Blend>(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            SetupLighting();
            SetupFog();

            device.SetRenderState<Cull>(RenderState.CullMode, Cull.Clockwise);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);

            Matrix view = Matrix.Translation(-Camera.Position) *
                            Matrix.RotationAxis(Vector3.UnitX, -Camera.Rotation.X * Mathf.DegToRad) *
                            Matrix.RotationAxis(Vector3.UnitZ, -Camera.Rotation.Z * Mathf.DegToRad) *
                            Matrix.RotationAxis(Vector3.UnitY, -Camera.Rotation.Y * Mathf.DegToRad);

            device.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(Camera.FOV * 0.0174533f, Camera.Aspect, 0.1f, 1000));
            device.SetTransform(TransformState.View, view);
        }

        public void SetRenderTarget(Texture rt)
        {
            if (rt != null && rt.Handle is SharpDX.Direct3D9.Texture)
                device.SetRenderTarget(0, ((SharpDX.Direct3D9.Texture)rt.Handle).GetSurfaceLevel(0));
            else
                device.SetRenderTarget(0, defaultRt);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new ColorBGRA(0, 0, 0, 255), 1.0f, 0);
        }

        public void DrawMesh(Mesh mesh, int frame, Material mat, Vector3 position, Vector3 rotation)
        {
            DrawMesh(mesh, frame, mat, position, rotation, Vector3.One, 0, Vector2.Zero);
        }

        public void DrawMesh(Mesh mesh, int frame, Material mat, Vector3 position, Vector3 rotation, Vector3 scale,
            float morphFactor, Vector2 uv)
        {
            if(mesh != null)
            {
                Matrix matrix = Matrix.Identity;

                matrix *= Matrix.Scaling(scale) *
                    Matrix.RotationY(rotation.Y * Mathf.DegToRad) *
                    Matrix.RotationZ(rotation.Z * Mathf.DegToRad) *
                    Matrix.RotationX(rotation.X * Mathf.DegToRad) *
                    Matrix.Translation(position);

                if (mat.Flags.HasFlag(MaterialFlags.ShadowMesh))
                    matrix *= Matrix.Shadow(new Vector4(0, 1, 1, 0), new Plane(new Vector3(0, 1, 0), 0.95f)); // Note: This is hardcoded. We should calculate mesh height by it's bounding box(not implemented yet)

                device.SetTransform(TransformState.World, matrix);

                SetRenderState(mat);
                
                if (mat.Flags.HasFlag(MaterialFlags.ShadowMesh))
                {
                    SetShadowMeshState();
                }
                else
                {
                    device.SetRenderState(RenderState.Lighting, !mat.NonLit);
                    SetTextureState(mat);
                }

                bool lerpEnable = Game.Current.Config.GetBool("renderer.lerp");

                if (morphFactor > 0)
                {
                    if (frame < mesh.Frames.Length && lerpEnable)
                    {
                        mesh.UpdateMorphTarget(frame, frame + 1, morphFactor);
                    }
                }

                if (morphFactor > 0 && lerpEnable)
                    device.DrawUserPrimitives<Vertex>(PrimitiveType.TriangleList, mesh.Frames[frame].Length / 3, mesh.MorphVerts);
                else
                    device.DrawUserPrimitives<Vertex>(PrimitiveType.TriangleList, mesh.Frames[frame].Length / 3, mesh.Frames[frame]);
            }
        }

        public void DrawLines(Ray[] rays)
        {
            if(rays != null)
            {
                Vertex[] verts = new Vertex[rays.Length * 2];
                
                for(int i = 0; i < rays.Length; i++)
                {
                    verts[i * 2].Position = rays[i].From;
                    verts[i * 2 + 1].Position = rays[i].To;
                }

                device.SetTransform(TransformState.World, Matrix.Identity);

                device.SetTexture(0, null);
                device.SetRenderState(RenderState.ZEnable, false);
                device.DrawUserPrimitives<Vertex>(PrimitiveType.LineList, rays.Length, verts);
            }
        }

        public void DrawPointSprite(Texture tex, Vector3 pos, float scale)
        {
            if (tex != null)
            {
                Vertex[] ptSprite = new Vertex[]
                {
                    new Vertex()
                    {
                        Position = pos
                     }
                };

                device.SetTexture(0, tex.Handle);

                device.SetTransform(TransformState.World, Matrix.Identity);

                device.SetRenderState(RenderState.TextureFactor, Color4.White.ToRgba());
                device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Modulate);
                device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Modulate);
                device.SetTextureStageState(1, TextureStage.Constant, Color4.White.ToBgra());
                device.SetTextureStageState(1, TextureStage.ColorArg0, TextureArgument.Current);
                device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Constant);
                device.SetTextureStageState(1, TextureStage.AlphaArg0, TextureArgument.Current);
                device.SetTextureStageState(1, TextureStage.AlphaArg1, TextureArgument.Constant);

                device.SetRenderState(RenderState.PointSize, scale);
                device.SetRenderState(RenderState.PointSpriteEnable, true);
                device.DrawUserPrimitives<Vertex>(PrimitiveType.PointList, 1, ptSprite);
            }
        }

        internal void BeginHUD()
        {
            device.SetTransform(TransformState.Projection, Matrix.OrthoOffCenterLH(0, Game.Current.Width,
                -Game.Current.Height, 0, 0, 1));
            device.SetTransform(TransformState.View, Matrix.Identity);

            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState<Blend>(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState<Blend>(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.ZEnable, false);
            device.SetRenderState(RenderState.Lighting, false);
            device.SetRenderState(RenderState.ScissorTestEnable, true);

            device.EnableLight(0, false);

            device.SetTransform(TransformState.World, Matrix.Identity);

            device.VertexFormat = VertexFormat.Position | VertexFormat.Normal
                    | VertexFormat.Texture1;
        }

        public void DrawSprite(Texture tex, Vector2 pos)
        {
            DrawSprite(tex, pos, Color4.White, 0, 0);
        }

        public void DrawSprite(Texture tex, Vector2 pos, Color4 color, float width, float height, SpriteDrawMode drawMode = SpriteDrawMode.Default)
        {
            if(tex != null)
            {
                device.SetTexture(0, tex.Handle);

                if(drawMode == SpriteDrawMode.Default)
                {
                    device.SetRenderState<Blend>(RenderState.SourceBlend, Blend.SourceAlpha);
                    device.SetRenderState<Blend>(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                }
                else
                {
                    device.SetRenderState<Blend>(RenderState.SourceBlend, Blend.One);
                    device.SetRenderState<Blend>(RenderState.DestinationBlend, Blend.One);
                }

                if (width == 0)
                    width = tex.Width;

                if (height == 0)
                    height = tex.Height;

                device.SetTransform(TransformState.World, Matrix.Translation(pos.X, -pos.Y, 0));

                Vertex[] verts = new Vertex[]
                {
                    new Vertex()
                    {
                        Position = new Vector3(0, 0, 0),
                        Normal = new Vector3(0, 0, 0),
                        UV = new Vector2(0, 0)
                    },
                    new Vertex()
                    {
                        Position = new Vector3(width, -height, 0),
                        Normal = new Vector3(0, 0, 0),
                        UV = new Vector2(1, 1)
                    },
                    new Vertex()
                    {
                        Position = new Vector3(width, 0, 0),
                        Normal = new Vector3(1, 0, 0),
                        UV = new Vector2(1, 0)
                    },
                    
                    new Vertex()
                    {
                        Position = new Vector3(width, -height, 0),
                        Normal = new Vector3(0, 0, 0),
                        UV = new Vector2(1, 1)
                    },
                    new Vertex()
                    {
                        Position = new Vector3(0, 0, 0),
                        Normal = new Vector3(0, 0, 0),
                        UV = new Vector2(0, 0)
                    },
                    new Vertex()
                    {
                        Position = new Vector3(0, -height, 0),
                        Normal = new Vector3(0, 0, 0),
                        UV = new Vector2(0, 1)
                    },
                    
                };

                device.SetRenderState(RenderState.TextureFactor, color.ToRgba());
                device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Modulate);
                device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Modulate);
                device.SetTextureStageState(1, TextureStage.Constant, color.ToBgra());
                device.SetTextureStageState(1, TextureStage.ColorArg0, TextureArgument.Current);
                device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Constant);
                device.SetTextureStageState(1, TextureStage.AlphaArg0, TextureArgument.Current);
                device.SetTextureStageState(1, TextureStage.AlphaArg1, TextureArgument.Constant);
                device.DrawUserPrimitives<Vertex>(PrimitiveType.TriangleList, 2, verts);
            }
        }

        internal void EndScene()
        {
            device.EndScene();
            device.Present();
        }
    }
}
