using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using Quobject.EngineIoClientDotNet.Client;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Authentication;

namespace VinappPrint
{
    public class SocketConfiguration
    {

        public const SslProtocols Tls12 = (SslProtocols)0x00000C00;
        
        public static void Listen()
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)Tls12;
            var socket_ = IO.Socket(ConfigurationManager.AppSettings["socket"]);
            socket_.On(Quobject.SocketIoClientDotNet.Client.Socket.EVENT_CONNECT, () =>
            {
                var connection = new ModelConnection
                {
                    IdCompany = ConfigurationManager.AppSettings["licencia"],
                    Point = ConfigurationManager.AppSettings["punto"]
                };
                string message_connection = JsonConvert.SerializeObject(connection);
                //socket_.Emit("printer:setup", message_connection);
                socket_.Emit("setup point", message_connection);
            });

            /*socket_.On("printer:new-print", (data) =>
            {
                dynamic response = JObject.Parse(data.ToString());
                Printer.Print(response.data);
            });*/

            socket_.On("new print", (data) =>
            {
                dynamic response = JObject.Parse(data.ToString());
                Printer.Print(response.data);
            });
            //Console.ReadLine();
        }

    }

    public class ModelConnection
    {
        public string IdCompany { get; set; }
        public string Point { get; set; }
    }
}
