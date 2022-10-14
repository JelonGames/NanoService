using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NanoServices.Script
{
    public static class ReadTemperature
    {
        private static readonly JObject json = JObject.Parse(ReadJson.json);
        private static readonly PowerShell ps = PowerShell.Create();

        public static string Read(int device)
        {
            string ipDevice = (string)json["DeviceSettings"][$"Device{device}"]["IP"];
            string msgError = string.Empty;

            ps.AddScript($"Invoke-WebRequest \"{ipDevice}/temp1.txt\" | Select-Object -ExpandProperty Content");

            ps.AddCommand("Out-String");

            PSDataCollection<PSObject> outputCollection = new();
            ps.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
            {
                msgError = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
            };

            IAsyncResult result = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
            ps.EndInvoke(result);

            StringBuilder sb = new();

            foreach(var outputItem in outputCollection)
            {
                sb.AppendLine(outputItem.BaseObject.ToString());
            }

            ps.Commands.Clear();

            if(!string.IsNullOrEmpty(msgError))
                return msgError;

            sb = sb.Replace('.', ',');
            return sb.ToString().Trim();
        }
    }
}
