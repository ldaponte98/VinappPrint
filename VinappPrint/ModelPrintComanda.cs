using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinappPrint
{
    public class ModelPrintComanda
    {
        public string NumComanda { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public string Observaciones { get; set; }

    }

    public class Producto
    {
        public string Cantidad { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
    }
}
