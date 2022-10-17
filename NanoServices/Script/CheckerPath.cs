using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NanoServices.Script
{
    public static class CheckerPath
    {
        private static readonly JObject json = JObject.Parse(ReadJson.json);

        public static string GetPath()
        {
            string path = (string)json["AppSettings"]["Logs"]["Path"];
            
            CheckDir(path);

            DateTime date = DateTime.Now;
            path = path.Replace("%%", $"{date.Year.ToString()}{date.Month.ToString()}{date.Day.ToString()}");

            CheckFile(path);

            return path;
        }

        private static void CheckFile(string path)
        {
            if (!File.Exists(path))
            {
                FileStream file = File.Create(path);
                file.Close();
                StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8);
                sw.WriteLine("DATE     PRIOROTY     DEVICE     VALUE");
                sw.Close();
            }
        }

        private static void CheckDir(string path)
        {
            int lengthPath = path.Length;
            string[] temp = path.Split('\\');
            int lengthFile = temp[temp.Length - 1].Length;
            string pathDir = path.Remove(lengthPath - lengthFile);

            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }
        }
    }
}
