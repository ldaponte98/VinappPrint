using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using VinappPrint.Infraestructure;

namespace VinappPrint
{ 
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (LocalStorage.Get("Login").Equals("true"))
            {
                Application.Run(new Main());
            }
            else{
                Application.Run(new Login());
            }
            
            //SocketConfiguration.Listen();
        }
    }
}
