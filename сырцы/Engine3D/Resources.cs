using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine3D
{
    public sealed class Data
    {
        private List<string> searchPath;

        public int PendingResources
        {
            get;
            private set;
        }

        internal Data()
        {
            searchPath = new List<string>();

            string[] split = Game.Current.Config.GetString("engine.resFolder").Split(';');

            foreach(string str in split)
            {
                if (str.EndsWith(".zip"))
                    continue;

                if (Directory.Exists(str))
                    searchPath.Add(str);
                else
                    Game.Current.Log.Print("Data directory not found \"{0}\"", str);
            }
        }

        public Stream GetStream(string fileName)
        {
            foreach(string str in searchPath)
            {
                if (File.Exists(str + fileName))
                    return File.OpenRead(str + fileName);
            }
            
            return null;
        }

        public Mesh GetStaticMesh(string fileName)
        {
            //Game.Current.Log.Print("Loading mesh {0}", fileName);

            return Mesh.LoadSmd(GetStream(fileName));
        }

        public AudioStream GetAudioStream(string fileName)
        {
            //Game.Current.Log.Print("Loading audio stream {0}", fileName);

            return AudioStream.LoadWav(GetStream(fileName));
        }
        
        public Mesh GetMesh(string fileName)
        {
            //Game.Current.Log.Print("Loading mesh {0}", fileName);

            return Mesh.LoadMD2(GetStream(fileName));
        }

        public Texture GetTexture(string fileName)
        {
            //Game.Current.Log.Print("Loading texture {0}", fileName);

            return Texture.FromStream(GetStream(fileName));
        }
    }
}
