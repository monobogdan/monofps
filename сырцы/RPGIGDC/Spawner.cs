using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using SharpDX;

namespace RPGIGDC
{
    public sealed class Spawner
    {
        const float MaxSpawnTime = 15;

        public int Round
        {
            get;
            private set;
        }

        public float DifficultyFactor
        {
            get;
            private set;
        }

        private float nextSpawn;
        private float nextSpawnItem;

        private float itemTime;
        private float time;

        private static Type[] monsterTypes =
        {
            typeof(Zombie),
            typeof(Crawler)
        };

        private World world;

        public Spawner(World world)
        {
            this.world = world;

            DifficultyFactor = 1;

            nextSpawnItem = 40;
            nextSpawn = MaxSpawnTime;
        }

        public void Update()
        {
            itemTime += Game.Current.DeltaTime;
            time += 0.1f;

            nextSpawn = DifficultyFactor * MaxSpawnTime;
            //DifficultyFactor = Mathf.Clamp(DifficultyFactor - (Game.Current.DeltaTime * 0.3f), 0.3f, 1);

            if (itemTime > nextSpawnItem)
            {
                Random rand = new Random();

                GroundItem item = new GroundItem(world);
                item.Position = new Vector3(rand.NextFloat(world.Map.Bounds.Minimum.X, world.Map.Bounds.Maximum.X),
                    item.Position.Y, rand.NextFloat(world.Map.Bounds.Minimum.Z, world.Map.Bounds.Maximum.Z));
                item.ItemType = (ItemType)rand.Next(0, 2);
                world.Spawn(item);

                itemTime = 0;
            }

            if (time > nextSpawn)
            {
                Random rand = new Random();

                Monster monster = (Monster)monsterTypes[rand.Next(0, monsterTypes.Length)].GetConstructor(new Type[] { typeof(World) }).Invoke(new object[] { world });
                monster.Position = new Vector3(rand.NextFloat(world.Map.Bounds.Minimum.X, world.Map.Bounds.Maximum.X),
                    monster.Position.Y, rand.NextFloat(world.Map.Bounds.Minimum.X, world.Map.Bounds.Maximum.X));
                world.Spawn(monster);

                time = 0;
            }
        }
    }
}
