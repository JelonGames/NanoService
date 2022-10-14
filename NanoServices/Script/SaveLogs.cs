using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoServices.Script
{
    public static class SaveLogs
    {
        public static void Save(string value)
        {
            string path = CheckerPath.GetPath();
            StreamWriter file = new StreamWriter(path, true, System.Text.Encoding.UTF8);
            file.WriteLine(value);
            file.Close();
        }
    }
}
