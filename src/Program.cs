using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HistacomLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Uri downloadUri = new Uri("http://histacom.github.io");
            string hcAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Histacom");
            JToken latestVer = null;
            bool? update = null; // null = don't launch HC; false = launch HC; true = update & launch HC;

            // Get the URL to the Histacom download
            try
            {
                WebClient wc = new WebClient();
                string versionJson = wc.DownloadString(new Uri(downloadUri, "versions.json"));
                JObject jo = JObject.Parse(versionJson);
                latestVer = jo.First;
            }
            catch (Exception ex)
            {
                // Exception thrown, probably no internet connection
                if (Directory.Exists(hcAppData))
                {
                    // already installed, launch Histacom
                    update = false;
                }
            }            

            // Check for new install
            if (!Directory.Exists(hcAppData))
            {
                Directory.CreateDirectory(hcAppData);
                update = true;
            }
            else
            {
                string versionJson = File.ReadAllText(Path.Combine(hcAppData, "versions.json"));
                JObject jo = JObject.Parse(versionJson);
                JToken jt = jo.First;
                if(jt != latestVer)
                {
                    update = true;
                }
                else
                {
                    update = false;
                }

            }

            if(!update.HasValue)
            {
                // we don't know whether should we update?!
                return;
            }

            if(update.Value == true)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Update());
            }


        }
    }
}
