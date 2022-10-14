using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoServices.Script
{
    public static class ReadJson
    {
        private static string _json()
        {
            try
            {
                return File.ReadAllText(@"./json/UserSettings.json");
            }
            catch
            {
                return File.ReadAllText(@"./NanoSettings/json/UserSettings.json");
            }
        }

        public static string json
        {
            get { return _json(); }
        }
    }
}
