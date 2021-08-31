using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VinappPrint.Infraestructure
{
    public class LocalStorage
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool Save(string key, object value)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                foreach (XmlElement element in xml.DocumentElement)
                {
                    if (element.Name.Equals("appSettings"))
                    {
                        foreach (XmlNode node in element.ChildNodes)
                        {
                            if (node.Attributes[0].Value == key)
                            {
                                node.Attributes[1].Value = value.ToString();
                            }
                        }
                    }
                }
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception e)
            {
                Tools.SaveLog("Error [LocalStorage] => [Save] : " + e.Message);
                return false;
            }
        }       
    }
}
