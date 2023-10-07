using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine3D
{
    public sealed class Config
    {
        private Dictionary<string, string> Values
        {
            get;
            set;
        }

        public Config(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            Values = new Dictionary<string, string>();

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line.Length < 1 || line[0] == ';')
                    continue;

                string[] split = line.Split('=');

                if (!Values.ContainsKey(split[0].Trim()))
                    Values.Add(split[0].Trim(), null);

                Values[split[0].Trim()] = split[1].Trim();
            }
        }

        public string GetString(string key)
        {
            if (Values.ContainsKey(key))
                return Values[key];

            return null;
        }

        public int GetInt(string key)
        {
            if (Values.ContainsKey(key))
                return int.Parse(Values[key]);

            return 0;
        }

        public bool GetBool(string key)
        {
            if (Values.ContainsKey(key))
                return bool.Parse(Values[key]);

            return false;
        }
    }
}
