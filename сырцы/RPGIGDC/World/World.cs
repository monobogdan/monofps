using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;
using Tao.Ode;

namespace RPGIGDC
{
    public sealed class Sky
    {
        private Mesh sky;
        private Material mat;

        private float time;

        public Sky()
        {
            sky = Mesh.LoadSmd(GameMain.GetResource("sphere.smd"));
            mat = new Material();
            mat.Texture = Game.Current.Data.GetTexture("sky.png");
            mat.DepthWrite = true;
            mat.NonLit = true;
        }

        public void Update()
        {
            time += 0.01f;
        }

        public void Draw()
        {
            Game.Current.Graphics.DrawMesh(sky, 0, mat, Game.Current.Graphics.Camera.Position + new Vector3(0, -0.1f, 0), new SharpDX.Vector3(0, time, 0));
        }
    }

    

    


    public sealed class MusicManager
    {
        private string[] trackList;
        private AudioStream stream;
        private int trackNum;

        public MusicManager()
        {
            trackList = System.IO.Directory.GetFiles("Data/music/");
            Next();
        }

        private void Next()
        {
            trackNum++;

            if (trackNum >= trackList.Length)
                trackNum = 0;

            //stream = AudioStream.LoadWav(GameMain.GetResource(trackList[trackNum]));
            //stream.Volume = 0.5f;
            //stream.Play();
        }

        public void Update()
        {
            if (stream != null && !stream.IsPlaying)
                Next();
        }
    }

    public sealed class World
    {
        public List<Entity> Entities
        {
            get;
            private set;
        }

        public int Cash
        {
            get;
            set;
        }

        private Texture uiCrosshair;

        private MusicManager musicManager;

        private Spawner spawner;

        private List<Entity> entToSpawnList;
        private List<Entity> entToRemoveList;

        private Sky sky;
        private GameUI gameUi;

        private Texture uiHealth;
        private Texture uiAmmo;

        private TextRenderer textRenderer;

        public Player Player
        {
            get;
            private set;
        }
        
        public Map Map
        {
            get;
            private set;
        }

        public IntPtr ODEWorld
        {
            get;
            private set;
        }

        private void InitMap()
        {
            Entities = new List<Entity>();

            sky = new Sky();
            Map = new Map();
            Map.Load("island");

            entToSpawnList = new List<Entity>();
            entToRemoveList = new List<Entity>();

            spawner = new Spawner(this);

            Player = new Player(this);
            Spawn(Player);
        }

        private void InitMisc()
        {
            textRenderer = new TextRenderer();
            uiHealth = Game.Current.Data.GetTexture("ui/hp.png");
            uiAmmo = Game.Current.Data.GetTexture("ui/bullet.png");
            uiCrosshair = Game.Current.Data.GetTexture("crosshair.png");

            Cash = 10;

            gameUi = new GameUI(this);
            musicManager = new MusicManager();
        }

        private void InitPhysics()
        {
            Ode.dInitODE();
            ODEWorld = Ode.dWorldCreate();

            Game.Current.Log.Print("Initialized phyiscs");
        }

        public World()
        {
            InitMap();
            InitMisc();
        }

        public void Spawn(Entity entity)
        {
            if (!Entities.Contains(entity))
            {
                entToSpawnList.Add(entity);

                
            }
        }

        public void Destroy(Entity entity)
        {
            if(Entities.Contains(entity))
            {
                entity.OnDeleted();
                entToRemoveList.Add(entity);
            }
        }

        public void Update()
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].OnUpdate();

            spawner.Update();
            sky.Update();
            musicManager.Update();

            foreach (Entity entity in entToSpawnList)
                Entities.Add(entity);

            foreach (Entity entity in entToRemoveList)
                Entities.Remove(entity);

            entToSpawnList.Clear();
            entToRemoveList.Clear();
        }

        public void Draw()
        {
            sky.Draw();
            Map.Draw();

            foreach (Entity entity in Entities)
                entity.OnDraw();

            foreach (Entity entity in Entities)
                entity.OnDrawTransparent();

            Map.DrawTransparent();

            foreach (Entity entity in Entities)
                entity.OnDrawHUD();
        }

        

        public void DrawGUI()
        {
            Game.Current.Graphics.DrawSprite(uiCrosshair, new Vector2(Game.Current.Width / 2 - (uiCrosshair.Width / 2),
                Game.Current.Height / 2 - (uiCrosshair.Height / 2)));

            Vector2 hpRel = GameUI.UIRelativeToAbs(0, 0.9f);

            Game.Current.Graphics.DrawSprite(uiHealth, hpRel, new Color4(1, 1, 1, Player.Health / 100), 0, 0);

            textRenderer.Begin();
            textRenderer.DrawString(null, "Health: " + Player.Health, 0, 20, new Color4(1, 0, 0, 0.7f));
           // textRenderer.DrawString(null, "Ammo: " + Player.Weapons[Player.ActiveWeapon].Ammo, 0, 40, 12, new Color4(1, 0, 0, 0.7f));

            if(Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.B))
                gameUi.BuyMenu = true;

            if (Game.Current.Input.GetKeyState(System.Windows.Forms.Keys.Escape))
                gameUi.BuyMenu = false;

            gameUi.Draw(textRenderer, null);

            textRenderer.End();
        }
    }
}
