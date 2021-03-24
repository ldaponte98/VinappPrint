using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinappPrint
{
    public class Tools
    {
        private static string pathLog = "C:\\Users\\" + Environment.UserName + "\\logs_vinapp_print.txt";
        public static void SaveLog(string message)
        {
            try
            {
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
