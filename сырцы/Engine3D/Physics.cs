using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Engine3D
{
    public static class Physics
    {
        [System.Runtime.InteropServices.DllImport("d3dx9_25.dll")]
        private static extern bool D3DXIntersectTri(
            ref Vector3 p0,
            ref Vector3 p1,
            ref Vector3 p2,
            ref Vector3 pRayPos,
            ref Vector3 pRayDir,
            out float pU,
            out float pV,
            out float pDist);

        public static bool MeshCast(Mesh mesh, Vector3 origin, Vector3 dir,
            Vector3 meshPos)
        {
            if (mesh != null)
            {
                bool ret = false;

                for (int i = 0; i < mesh.Frames[0].Length / 3; i++)
                {
                    Vertex v0 = mesh.Frames[0][i * 3];
                    Vertex v2 = mesh.Frames[0][i * 3 + 1];
                    Vertex v1 = mesh.Frames[0][i * 3 + 2];

                    float dist = 0;
                    float u, v = 0;

                    //dir =;

                    if (D3DXIntersectTri(ref v0.Position, ref v1.Position, ref v2.Position, ref origin, ref dir, out u, out v, out dist))
                        return true;
                    //if (TriangleCast(origin, dir, v0, v1, v2, ref dist))
                    //    return true;
                }

                return ret;
            }

            return false;
        }

        private static float fabs(float val)
        {
            return val < 0 ? -val : val;
        }

        public static bool TriangleCast(Vector3 origin, Vector3 dir, Vertex v0, Vertex v1, Vertex v2, ref float dist)
        {
            // compute plane's normal
            Vector3 v0v1 = v1.Position - v0.Position;
            Vector3 v0v2 = v2.Position - v0.Position;
            // no need to normalize
            Vector3 N = Vector3.Cross(v0v1, v0v2); // N 
            float area2 = N.Length();

            // Step 1: finding P

            // check if ray and plane are parallel ?
            float NdotRayDirection = Vector3.Dot(N, dir);
            if (fabs(NdotRayDirection) < 0.0001f) // almost 0 
                return false; // they are parallel so they don't intersect ! 

            // compute d parameter using equation 2
            float d = Vector3.Dot(N, v0.Position);

            // compute t (equation 3)
            float t = (Vector3.Dot(N, origin) + d) / NdotRayDirection;
            // check if the triangle is in behind the ray
            if (t < 0) return false; // the triangle is behind 

            // compute the intersection point using equation 1
            Vector3 P = origin + t * dir;

            // Step 2: inside-outside test
            Vector3 C; // vector perpendicular to triangle's plane 

            // edge 0
            Vector3 edge0 = v1.Position - v0.Position;
            Vector3 vp0 = P - v0.Position;
            C = Vector3.Cross(edge0, vp0);
            if (Vector3.Dot(N, C) < 0) return false; // P is on the right side 

            // edge 1
            Vector3 edge1 = v2.Position - v1.Position;
            Vector3 vp1 = P - v1.Position;
            C = Vector3.Cross(edge1, vp1);
            if (Vector3.Dot(N, C) < 0) return false; // P is on the right side 

            // edge 2
            Vector3 edge2 = v0.Position - v2.Position;
            Vector3 vp2 = P - v2.Position;
            C = Vector3.Cross(edge2, vp2);
            if (Vector3.Dot(N, C) < 0) return false; // P is on the right side; 

            return true; // this ray hits the triangle 
        }
    }
}
