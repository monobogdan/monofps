using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;

namespace RPGIGDC
{
    class Program
    {
        public static bool ServerArg
        {
            get;
            private set;
        }

        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0] == "-server")
                ServerArg = true;

            Game.Initialize(new GameMain());
            Game.Current.Run();
        }
    }
}
