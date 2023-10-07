using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpDX;

namespace Engine3D
{
    public interface IGameApp
    {
        void OnCreate();
        void OnUpdate();
        void OnDraw();
        void OnDrawGUI();
        void OnExit();
    }

    public sealed class Game
    {
        private Version Version = new Version("1.0");

        public static Game Current
        {
            get;
            private set;
        }

        public static void Initialize(IGameApp callbacks)
        {
            Current = new Game(callbacks);
            Current.CreateModules();
        }

        internal Form Form
        {
            get;
            private set;
        }

        public Config Config
        {
            get;
            private set;
        }

        public Graphics Graphics
        {
            get;
            private set;
        }

        public Data Data
        {
            get;
            private set;
        }

        public AudioManager AudioManager
        {
            get;
            private set;
        }

        public float DeltaTime
        {
            get;
            set;
        }

        public Input Input
        {
            get;
            private set;
        }

        public int Width
        {
            get
            {
                return Form.ClientSize.Width;
            }
        }

        public int Height
        {
            get
            {
                return Form.ClientSize.Height;
            }
        }

        public Log Log
        {
            get;
            private set;
        }

        public float Time
        {
            get;
            private set;
        }

        private System.Diagnostics.Stopwatch stopWatch;

        private IGameApp callbacks;

        public Game(IGameApp callbacks)
        {
            Form = new Form();
            Form.Width = 1024;
            Form.Height = 768;
            Form.Show();

            this.callbacks = callbacks;
        }

        private void CreateModules()
        {
            using (System.IO.Stream strm = System.IO.File.OpenRead("Engine.cfg"))
                Config = new Config(strm);

            Log = new Log();

            Form.Width = Config.GetInt("engine.width");
            Form.Height = Config.GetInt("engine.height");

            Log.Print("Engine3D version {0}", Version.ToString());

            Graphics = new Graphics();
            Graphics.CreateResources();
            Input = new Input();
            AudioManager = new AudioManager();
            Data = new Data();
            
            stopWatch = new System.Diagnostics.Stopwatch();

            this.callbacks.OnCreate();
        }

        private void LoadConfig()
        {

        }

        float rot = 0;

        public void Run()
        {
            while(!Form.IsDisposed)
            {
                Application.DoEvents();

                stopWatch.Start();

                callbacks.OnUpdate();
                
                Graphics.BeginScene();
                callbacks.OnDraw();
                Graphics.BeginHUD();
                callbacks.OnDrawGUI();
                Graphics.EndScene();

                stopWatch.Stop();
                DeltaTime = 0.003f;
                Time += (float)stopWatch.ElapsedMilliseconds / 1000;
                stopWatch.Reset();

                System.Threading.Thread.Sleep(1000 / 60);
            }
        }
    }
}
