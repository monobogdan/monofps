using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Engine3D
{
    public sealed class Log
    {
        private StreamWriter writer;

        internal Log()
        {
            writer = new StreamWriter("engine.log");
            
        }

        public void Print(string fmt, params object[] fmtArgs)
        {
            string str = string.Format(fmt, fmtArgs);

            StackTrace trace = new StackTrace();
            var method = trace.GetFrame(1).GetMethod();
            
            str = method.DeclaringType.Name + "(" + method.Name + "): " + str;

            Console.WriteLine(str);
            writer.WriteLine(str);
            writer.Flush();
        }
    }
}
