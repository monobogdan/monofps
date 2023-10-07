using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Engine3D
{
    internal struct MD2Header
    {
        public int ident;                  /* magic number: "IDP2" */
        public int version;                /* version: must be 8 */

        public int skinwidth;              /* texture width */
        public int skinheight;             /* texture height */

        public int framesize;              /* size in bytes of a frame */

        public int num_skins;              /* number of skins */
        public int num_vertices;           /* number of vertices per frame */
        public int num_st;                 /* number of texture coordinates */
        public int num_tris;               /* number of triangles */
        public int num_glcmds;             /* number of opengl commands */
        public int num_frames;             /* number of frames */

        public int offset_skins;           /* offset skin data */
        public int offset_st;              /* offset texture coordinate data */
        public int offset_tris;            /* offset triangle data */
        public int offset_frames;          /* offset frame data */
        public int offset_glcmds;          /* offset OpenGL command data */
        public int offset_end;             /* offset end of file */
    }

    internal struct MD2TexCoord
    {
        public ushort u, v;
    }

    internal struct MD2Triangle
    {
        public int[] verts;
        public int[] uv;
    }

    internal struct MD2Vertex
    {
        public float x, y, z;
        public byte normalIndex;
    }

    internal struct MD2Frame
    {
        public Vector3 scale;
        public Vector3 translate;

        public string name;
        public MD2Vertex[] verts;
    }

    internal sealed class MD2Model
    {
        public MD2Header Header
        {
            get;
            private set;
        }

        public MD2Triangle[] Triangles
        {
            get;
            private set;
        }

        public MD2TexCoord[] Coords
        {
            get;
            private set;
        }

        public MD2Frame[] Frames
        {
            get;
            private set;
        }

        public MD2Model(Stream strm)
        {
            BinaryReader reader = new BinaryReader(strm, Encoding.ASCII);

            Header = new MD2Header()
            {
                ident = reader.ReadInt32(),
                version = reader.ReadInt32(),
                skinwidth = reader.ReadInt32(),
                skinheight = reader.ReadInt32(),
                framesize = reader.ReadInt32(),
                num_skins = reader.ReadInt32(),
                num_vertices = reader.ReadInt32(),
                num_st = reader.ReadInt32(),
                num_tris = reader.ReadInt32(),
                num_glcmds = reader.ReadInt32(),
                num_frames = reader.ReadInt32(),
                offset_skins = reader.ReadInt32(),
                offset_st = reader.ReadInt32(),
                offset_tris = reader.ReadInt32(),
                offset_frames = reader.ReadInt32(),
                offset_glcmds = reader.ReadInt32(),
                offset_end = reader.ReadInt32()
            };

            Triangles = new MD2Triangle[Header.num_tris];
            Coords = new MD2TexCoord[Header.num_st];
            Frames = new MD2Frame[Header.num_frames];

            reader.BaseStream.Seek(Header.offset_tris, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_tris; i++)
            {
                Triangles[i] = new MD2Triangle()
                {
                    verts = new int[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() },
                    uv = new int[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() }
                };
            }

            reader.BaseStream.Seek(Header.offset_st, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_st; i++)
            {
                Coords[i] = new MD2TexCoord()
                {
                    u = reader.ReadUInt16(),
                    v = reader.ReadUInt16()
                };
            }

            reader.BaseStream.Seek(Header.offset_frames, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_frames; i++)
            {
                Frames[i] = new MD2Frame()
                {
                    scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    translate = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    name = new string(reader.ReadChars(16)),
                    verts = new MD2Vertex[Header.num_vertices]
                };

                for(int j = 0; j < Header.num_vertices; j++)
                {
                    Frames[i].verts[j].x = ((float)reader.ReadByte() * Frames[i].scale.X) + Frames[i].translate.X;
                    Frames[i].verts[j].y = ((float)reader.ReadByte() * Frames[i].scale.Y) + Frames[i].translate.Y;
                    Frames[i].verts[j].z = ((float)reader.ReadByte() * Frames[i].scale.Z) + Frames[i].translate.Z;
                    Frames[i].verts[j].normalIndex = reader.ReadByte();
                }
            }
        }
    }
}
