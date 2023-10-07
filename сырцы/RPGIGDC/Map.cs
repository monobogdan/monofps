using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;
using System.Globalization;
using System.IO;
using Tao.Ode;

namespace RPGIGDC
{
    public struct MapMesh
    {
        public Mesh Mesh;
        public Material Material;
    }

    public sealed class Map
    {
        public BoundingBox Bounds
        {
            get;
            private set;
        }

        private Mesh terrainMesh;
        private Material terrainMaterial;

        private IntPtr collision;

        public List<MapMesh> meshes;

        public Map()
        {

        }

        public void Load(string name)
        {
            StreamReader reader = new StreamReader(GameMain.GetResource("maps/" + name + "/map.txt"));

            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;

            meshes = new List<MapMesh>();

            while(!reader.EndOfStream)
            {
                string[] split = reader.ReadLine().Split(' ');

                if(split[0] == "boundMin")
                    min = new Vector3(float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture), float.Parse(split[3], CultureInfo.InvariantCulture));

                if(split[0] == "boundMax")
                    max = new Vector3(float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture), float.Parse(split[3], CultureInfo.InvariantCulture));

                if (split[0] == "mesh")
                {
                    Material mat = new Material();
                    mat.Texture = Game.Current.Data.GetTexture("maps/" + name + "/" + split[2]);

                    

                    if (split.Length >= 4)
                    {
                        if (split[3] == "nodepth")
                            mat.DepthWrite = true;
                    }

                   // if (split.Length >= 4)
                    //    mat.Detail = Game.Current.Data.GetTexture"maps/" + name + "/" + split[3]));

                    meshes.Add(new MapMesh()
                    {
                        Mesh = Mesh.LoadSmd(GameMain.GetResource("maps/" + name + "/" + split[1])),
                        Material = mat
                    });
                }

                if (split[0] == "terrain")
                {
                    terrainMesh = Mesh.LoadSmd(GameMain.GetResource("maps/" + name + "/" + split[1]));
                    terrainMaterial.Texture = Game.Current.Data.GetTexture("maps/" + name + "/splat.jpg");
                    terrainMaterial.Detail = Game.Current.Data.GetTexture("maps/" + name + "/sand.jpg");
                    //terrainMaterial.Detail2 = Game.Current.Data.GetTexture("maps/" + name + "/texture.jpg");
                    terrainMaterial.DetailType = MaterialDetailType.BlendMask;
                    terrainMaterial.NonLit = true;
                    //terrainMaterial.UVScale = new Vector2(15, 15);
                    //terrainMaterial.Detail2 = Game.Current.Data.GetTexture"maps/" + name + "/splat1.bmp"));
                }
            }

            Bounds = new BoundingBox(min, max);
        }

        public void Update()
        {

        }

        public void Draw()
        {
            Game.Current.Graphics.DrawMesh(terrainMesh, 0, terrainMaterial, new Vector3(0, -1, 0), Vector3.Zero);

            foreach (MapMesh mesh in meshes)
            {
                if (!mesh.Material.DepthWrite)
                {
                    Game.Current.Graphics.DrawMesh(mesh.Mesh, 0, mesh.Material, new Vector3(0, -1, 0), Vector3.Zero);
                    
                }
            }
        }

        public void DrawTransparent()
        {
           

            foreach (MapMesh mesh in meshes)
            {
                if (mesh.Material.DepthWrite)
                    Game.Current.Graphics.DrawMesh(mesh.Mesh, 0, mesh.Material, new Vector3(0, -1, 0), Vector3.Zero);
            }
        }
    }
}
