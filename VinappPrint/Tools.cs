using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinappPrint
{
    public class Tools
    {
        public static void SaveLog(string message)
        {
            string pathLog = "C:\\Users\\" + Environment.UserName;
            if (ConfigurationManager.AppSettings["carpeta_usuario"] != "")
            {
                pathLog = ConfigurationManager.AppSettings["carpeta_usuario"] ;
            }

            if (!Directory.Exists(pathLog))
            {
                Directory.CreateDirectory(pathLog);
            }
            try
            {
                pathLog += "\\logs_vinapp_print.txt";
                StreamWriter streamWriter = File.Exists(pathLog) ? File.AppendText(pathLog) : File.CreateText(pathLog);
                streamWriter.WriteLine(DateTime.Now.ToString() + ": " + message);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo escribir el log -> " + ex.Message);
            }
        }
    }
}
