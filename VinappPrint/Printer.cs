using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Linq;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace VinappPrint
{
    public partial class Printer
    {
        public static void Print(dynamic data)
        {
            if (data.type_print == "comanda") PrintComanda(data);
            if (data.type_print == "factura") PrintFactura(data);
            if (data.type_print == "comanda-factura" || data.type_print == null)
            {
                PrintFactura(data);
                PrintComanda(data);
            }
        }

        public static void PrintFactura(dynamic data)
        {
            ModelPrintFactura modelPrint = new ModelPrintFactura();
            modelPrint.Empresa = data.business;
            modelPrint.Punto = data.point;
            modelPrint.Nit = data.nit_business;
            modelPrint.Ciudad = data.city_point;
            modelPrint.Direccion = data.address_point;
            modelPrint.Telefono = data.phone_point;
            modelPrint.Resolucion = data.resolution;
            modelPrint.NumFactura = data.document_number;
            modelPrint.Fecha = data.document_date;
            modelPrint.ClienteNombre = data.client_name;
            modelPrint.ClienteTelefono = data.client_phone;
            modelPrint.ClienteDireccion = data.client_address;
            modelPrint.Mesa = data.table;
            modelPrint.Domicilio = data.shipping;
            modelPrint.PagaCon = data.pay_with;

            foreach (dynamic item in data.details)
            {
                modelPrint.DetallesFactura.Add(new DetalleFactura
                {
                    Descripcion = item.product,
                    Cantidad = item.quantity,
                    Valor = item.value
                });
            }
           
            modelPrint.Subtotal = data.subtotal;
            modelPrint.Descuento = data.total_discount;
            modelPrint.Servicio = data.service;
            modelPrint.Total = data.total;
            modelPrint.FormaPago = data.payment_methods;
            
            string[] impresoras = ConfigurationManager.AppSettings["impresoras_facturacion"].ToString().Split(',');
            foreach (string impresora in impresoras)
            {
                DesingPos desingPos = new DesingPos("factura");
                desingPos.modelPrintFactura = modelPrint;
                desingPos.impresora = impresora;
                desingPos.Start();
                //MessageBox.Show("Imprimiendo Factura");
            }
        }

        public static void PrintComanda(dynamic data)
        {
            ModelPrintComanda modelPrint = new ModelPrintComanda();
            modelPrint.NumComanda = data.comanda;
            modelPrint.Mesa = data.table;
            foreach (dynamic item in data.details)
            {
                modelPrint.Productos.Add(new Producto
                {
                    Nombre = item.product,
                    Cantidad = item.quantity,
                    Observaciones = item.additions
                });
            }
            modelPrint.Observaciones = data.commentary;

            string[] impresoras = ConfigurationManager.AppSettings["impresoras_comanda"].ToString().Split(',');
            foreach (string impresora in impresoras)
            {
                DesingPos desingPos = new DesingPos("comanda");
                desingPos.modelPrintComanda = modelPrint;
                desingPos.impresora = impresora;
                desingPos.Start();  
                
            }
            
        }


    }
}
