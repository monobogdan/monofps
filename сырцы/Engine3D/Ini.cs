using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine3D
{
    public sealed class Ini
    {
        private Dictionary<string, string> values;
        
        public Ini(string iniFile)
        {
            StringReader reader = new StringReader(iniFile);
        }

        public string GetKey(string name)
        {
            if (values.ContainsKey(name))
                return values[name];

            return null;
        }
    }
}
