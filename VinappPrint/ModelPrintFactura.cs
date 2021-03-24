using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinappPrint
{
    public class ModelPrintFactura
    {
        public string Empresa { get; set; }
        public string Punto { get; set; }
        public string Nit { get; set; }
        public string Ciudad { get; set; }
        public string Resolucion { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string NumFactura { get; set; }
        public string Fecha { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteTelefono { get; set; }
        public string ClienteDireccion { get; set; }
        public string Mesa { get; set; }
        public string Domicilio { get; set; }
        public string Servicio { get; set; }
        public List<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string Total { get; set; }
        public string FormaPago { get; set; }
        public string PagaCon { get; set; }
        public string Texto { get; set; } = "Esta factura de venta se asimila en sus efectos a la letra de cambio (ART. 621 Y 774 de C.C)";
    }

    public class DetalleFactura
    {
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string Valor { get; set; }
    }
}
