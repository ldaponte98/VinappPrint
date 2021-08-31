using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace VinappPrint.Infraestructure
{
    public class API
    {
        public const SslProtocols Tls12 = (SslProtocols)0x00000C00;

        public static Response ValidateLogin(string email, string password)
        {
            try
            {
                if (email != null && email.Trim() != "")
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)Tls12;
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("email", email);
                        reqparm.Add("password", password);
                        string url = LocalStorage.Get("url_validate_login");
                        byte[] responsebytes = client.UploadValues(url, "POST", reqparm);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);
                        ResponseLogin response = JsonConvert.DeserializeObject<ResponseLogin>(responsebody);
                        return new Response(response.message, !response.success, response.info);
                    }
                }
                return new Response("Correo electronico no suministrado.", true);
            }
            catch (Exception)
            {
                return new Response("Ocurrio un error inesperado, favor comunicarse con el proveedor de software.", true);
            }
        }

        public class Response
        {
            public string Message { get; set; }
            public bool Error { get; set; }
            public dynamic Data { get; set; }

            public Response(string Message, bool Error = true, dynamic Data = null)
            {
                this.Message = Message;
                this.Error = Error;
                this.Data = Data;
            }
        }

        public class ResponseLogin
        {
            public bool success { get; set; }
            public string message { get; set; }
            public dynamic info { get; set; }
        }
    }
}
