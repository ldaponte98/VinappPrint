using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Configuration;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace VinappPrint
{
    public partial class DesingPos : Form
    {
        public ModelPrintFactura modelPrintFactura;
        public ModelPrintComanda modelPrintComanda;
        public string impresora = null;
        public string tipo = "factura"; //factura - comanda

        public DesingPos(string tipo)
        {
            this.tipo = tipo;
        }

        public void Start()
        {
            Configurations();
        }

        public DesingPos()
        {   
            InitializeComponent();
        }

        public void Configurations()
        {
            printDocument = new PrintDocument();

            StandardPrintController spc = new StandardPrintController();
            printDocument.PrintController = spc; // This hides popup
            var paperSize = new PaperSize("A4", 1000, 10000);
            PrinterSettings ps = new PrinterSettings();
            if (impresora != null && impresora.Trim() != "") ps.PrinterName = impresora;
            string imagen = "C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png";
            if (!File.Exists(imagen))
            {
                if (ConfigurationManager.AppSettings["url_logo"] != "")
                {
                    SaveImage(ConfigurationManager.AppSettings["url_logo"], "C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png", ImageFormat.Png);
                }                
            }
            
            
            
            Tools.SaveLog("Imprimiendo desde " + ps.PrinterName);
            ps.DefaultPageSettings.PaperSize = paperSize;
            printDocument.PrinterSettings = ps;


            printDocument.DefaultPageSettings.PaperSize = paperSize;
            printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;


            if (tipo == "factura")
            {
                if (ConfigurationManager.AppSettings["tipo_papel"] == "Normal") {
                    if (ConfigurationManager.AppSettings["drivers_originales"] == "true")
                    {
                        printDocument.PrintPage += ImprimirFactura;
                    }
                    else
                    {
                        printDocument.PrintPage += ImprimirFacturaSinDrivers;
                    }
                }

                if (ConfigurationManager.AppSettings["tipo_papel"] == "Pequeño")
                {
                    if (ConfigurationManager.AppSettings["drivers_originales"] == "true")
                    {
                        printDocument.PrintPage += ImprimirFacturaSmall;
                    }
                    else
                    {
                        printDocument.PrintPage += ImprimirFacturaSmallSinDrivers;
                    }
                    
                }

            }
            if (tipo == "comanda")
            {
                if (ConfigurationManager.AppSettings["tipo_papel"] == "Normal")
                {
                    if (ConfigurationManager.AppSettings["drivers_originales"] == "true")
                    {
                        printDocument.PrintPage += ImprimirComanda;
                    }
                    else
                    {
                        printDocument.PrintPage += ImprimirComandaSinDrivers;
                    }
                }

                if (ConfigurationManager.AppSettings["tipo_papel"] == "Pequeño")
                {
                    if (ConfigurationManager.AppSettings["drivers_originales"] == "true")
                    {
                        printDocument.PrintPage += ImprimirComandaSmall;
                    }
                    else
                    {
                        printDocument.PrintPage += ImprimirComandaSmallSinDrivers;
                    }
                }
            }
            
            printDocument.Print();
            if(ConfigurationManager.AppSettings["corte_automatico"] == "true") CutTicket();
        }

        private void ImprimirFactura(object sender, PrintPageEventArgs e)
        {
            
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            float height = 0f;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 9 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            int x_detalles = int.Parse(ConfigurationManager.AppSettings["x_menos_detalles"]);

            
            float y = 150 + int.Parse(ConfigurationManager.AppSettings["espaciado_logo"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x"]);
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;
            
            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;

            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;

            string extension = modelPrintFactura.UrlLogo.Split('.')[modelPrintFactura.UrlLogo.Split('.').Count() - 1];
            string nombre_archivo = "imagen_vinapp_" + modelPrintFactura.IdLicencia + "." + extension;

            string imagenLicencia = "C:\\Users\\" + Environment.UserName + "\\"+ nombre_archivo;
            if (!File.Exists(imagenLicencia))
            {
                SaveImage(modelPrintFactura.UrlLogo, imagenLicencia, GetExtension(modelPrintFactura.UrlLogo));
            }

            

            if (ConfigurationManager.AppSettings["url_logo"].ToString().Trim() != "") {
                Image img = Image.FromFile("C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png");
                e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            }
            else
            {
                Image img = Image.FromFile(imagenLicencia);
                e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            }
            if (modelPrintFactura.DetallesFactura.Count >= 8)
            {
                fontSize -= 1;
            }


            e.Graphics.DrawString(modelPrintFactura.Empresa, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Nit, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Ciudad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 45, width, height), center);

            y += 45;
            e.Graphics.DrawString(modelPrintFactura.Telefono, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Direccion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Resolucion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 70, width - 20, height));
            y += int.Parse(ConfigurationManager.AppSettings["espaciado_resolucion"]);
            e.Graphics.DrawString("Factura Venta N°: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width, height));
            e.Graphics.DrawString(modelPrintFactura.NumFactura, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 110, y + 70, width, height));

            e.Graphics.DrawString("Fecha: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 90, width, height));
            e.Graphics.DrawString(modelPrintFactura.Fecha, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 45, y + 90, width, height));

            y -= 20;
    
            e.Graphics.DrawString("Nombre: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 130, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteNombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 53, y + 130, width, height));

            e.Graphics.DrawString("Teléfono: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 150, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteTelefono, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 58, y + 150, width, height));

            e.Graphics.DrawString("Direccion: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteDireccion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 64, y + 170, width, height));

            if (modelPrintFactura.ClienteBarrio != "No definido")
            {
                e.Graphics.DrawString("Barrio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                e.Graphics.DrawString(modelPrintFactura.ClienteBarrio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 40, y + 190, width, height));
                y += 30;
            }
            if (modelPrintFactura.Mesa != "No definida")
            {
                if (modelPrintFactura.Mesa == "Para llevar")
                {
                    e.Graphics.DrawString("Pedido para llevar", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                }
                else if(modelPrintFactura.Mesa == "Para Recogerlo")
                {
                    e.Graphics.DrawString("Pedido para recogerlo", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                }
                else
                {
                    e.Graphics.DrawString("Mesa: " + modelPrintFactura.Mesa, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                }
                y += 30;
            }
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height));
            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 160 - x_detalles, y + 200, width, height));
            e.Graphics.DrawString("Valor ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);
            y += 20;

            foreach (DetalleFactura item in modelPrintFactura.DetallesFactura)
            {
                
                e.Graphics.DrawString(item.Descripcion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x - x_detalles, y + 200, width/2, height));
                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 160 - x_detalles, y + 200, width, height));
                e.Graphics.DrawString(item.Valor, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);

                SizeF textSize = e.Graphics.MeasureString(item.Descripcion, new Font(fontFamily, fontSize)); y += textSize.Height + 12;
            }

            if (modelPrintFactura.Observaciones != "")
            {
                e.Graphics.DrawString("Observaciones", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 220, width, height));
                e.Graphics.DrawString(modelPrintFactura.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 235, width, height));
                SizeF textSize = e.Graphics.MeasureString(modelPrintFactura.Observaciones, new Font(fontFamily, fontSize));
                y += textSize.Height + 40;
            }

            e.Graphics.DrawString("Subtotal: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Subtotal, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            y += 20;
            e.Graphics.DrawString("Domicilio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Domicilio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right); 
            e.Graphics.DrawString("Descuento: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 240, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Descuento, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 240, width, height), sf_right);
            if(modelPrintFactura.Servicio != "$0")
            {
                e.Graphics.DrawString("Servicio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 260, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.Servicio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
                y += 20;
            }
            e.Graphics.DrawString("Total: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 260, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Total, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
            e.Graphics.DrawString("Forma de pago: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 280, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.FormaPago, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 280, width, height), sf_right);
            if(modelPrintFactura.PagaCon != null && modelPrintFactura.PagaCon != "")
            {
                e.Graphics.DrawString("Paga con: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 300, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.PagaCon, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 300, width, height), sf_right);
                y += 20;
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(modelPrintFactura.Texto, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 30, y + 330, width / 1.4f, height), sf);
            e.Graphics.DrawString("¡Gracias por su compra!", new Font(fontFamily, 12 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]), fontStyle), Brushes.Black, new RectangleF(x + 40, y + 400, width, height));
            e.Graphics.DrawString("Impreso por Vinapp", new Font(fontFamily, 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]), fontStyle), Brushes.Black, new RectangleF(x, y + 430, width, height), center);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x, y + 620, width, height));
        }

        private ImageFormat GetExtension(string urlLogo)
        {
            string extension = urlLogo.Split('.')[1].ToUpper();
            switch (extension)
            {
                case "png":
                    return ImageFormat.Png;
                case "jpg":
                    return ImageFormat.Jpeg;

                case "jpeg":
                    return ImageFormat.Jpeg;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        private void ImprimirFacturaSinDrivers(object sender, PrintPageEventArgs e)
        {
            
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            int height = 300;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 9 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            int x_detalles = int.Parse(ConfigurationManager.AppSettings["x_menos_detalles"]);

            Image img = Image.FromFile("C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png");
            int y = 150 + int.Parse(ConfigurationManager.AppSettings["espaciado_logo"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x"]);
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;
            
            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;

            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;

            if (ConfigurationManager.AppSettings["url_logo"].ToString().Trim() != "") e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            e.Graphics.DrawString(modelPrintFactura.Empresa, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Nit, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Ciudad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 45, width, height), center);

            y += 45;
            e.Graphics.DrawString(modelPrintFactura.Telefono, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Direccion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Resolucion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 70, width - 20, height));
            y += int.Parse(ConfigurationManager.AppSettings["espaciado_resolucion"]);
            e.Graphics.DrawString("Factura Venta N°: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width, height));
            e.Graphics.DrawString(modelPrintFactura.NumFactura, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 110, y + 70, width, height));

            e.Graphics.DrawString("Fecha: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 90, width, height));
            e.Graphics.DrawString(modelPrintFactura.Fecha, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 45, y + 90, width, height));

            y -= 20;
    
            e.Graphics.DrawString("Nombre: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 130, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteNombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 53, y + 130, width, height));

            e.Graphics.DrawString("Teléfono: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 150, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteTelefono, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 58, y + 150, width, height));

            e.Graphics.DrawString("Direccion: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteDireccion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 64, y + 170, width, height));
            if (modelPrintFactura.Mesa != "No definida")
            {
                if (modelPrintFactura.Mesa != "Para llevar")
                {
                    e.Graphics.DrawString("Mesa: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                    e.Graphics.DrawString(modelPrintFactura.Mesa, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 40, y + 190, width, height));
                }
                else
                {
                    e.Graphics.DrawString("Pedido para llevar", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 190, width, height));
                }
                
                y += 30;
            }
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height));
            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 160 - x_detalles, y + 200, width, height));
            e.Graphics.DrawString("Valor ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);
            y += 20;

            foreach (DetalleFactura item in modelPrintFactura.DetallesFactura)
            {
                
                e.Graphics.DrawString(item.Descripcion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x - x_detalles, y + 200, width/2, height));
                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 160 - x_detalles, y + 200, width, height));
                e.Graphics.DrawString(item.Valor, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);
                y += this.GetYByHeigth(item.Descripcion);
            }
            e.Graphics.DrawString("Subtotal: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Subtotal, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            y += 20;
            e.Graphics.DrawString("Domicilio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Domicilio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right); 
            e.Graphics.DrawString("Descuento: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 240, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Descuento, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 240, width, height), sf_right);
            if(modelPrintFactura.Servicio != "$0")
            {
                e.Graphics.DrawString("Servicio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 260, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.Servicio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
                y += 20;
            }
            e.Graphics.DrawString("Total: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 260, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Total, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
            e.Graphics.DrawString("Forma de pago: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 280, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.FormaPago, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 280, width, height), sf_right);
            if(modelPrintFactura.PagaCon != null && modelPrintFactura.PagaCon != "")
            {
                e.Graphics.DrawString("Paga con: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 80, y + 300, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.PagaCon, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 300, width, height), sf_right);
                y += 20;
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(modelPrintFactura.Texto, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 30, y + 330, width / 1.4f, height), sf);
            e.Graphics.DrawString("¡Gracias por su compra!", new Font(fontFamily, 12 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]), fontStyle), Brushes.Black, new RectangleF(x + 40, y + 400, width, height));
            e.Graphics.DrawString("Impreso por Vinapp", new Font(fontFamily, 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]), fontStyle), Brushes.Black, new RectangleF(x, y + 430, width, height), center);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x, y + 620, width, height));


        }
        public void CutTicket()
        {
            string corte = "\x1B" + "m";                                    // caracteres de corte
            string avance = "\x1B" + "d" + "\x09";                          // avanza 9 renglones
            RawPrinterHelper.SendStringToPrinter(impresora, avance);        // avanza
            RawPrinterHelper.SendStringToPrinter(impresora, corte);         // corta
        }

        private void ImprimirComanda(object sender, PrintPageEventArgs e)
        {
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            float height = 0F;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            float y = int.Parse(ConfigurationManager.AppSettings["espaciado_y_comanda"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x_comanda"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            if (modelPrintComanda.Productos.Count >= 8)
            {
                fontSize -= 1;
            }
            e.Graphics.DrawString("Comanda #" + modelPrintComanda.NumComanda, new Font(fontFamily, 15, fontStyle), Brushes.Black, new RectangleF(x, y, width, height), sf);
            if (modelPrintComanda.Mesa != "No definida")
            {
                if (modelPrintComanda.Mesa != "Para llevar")
                {
                    e.Graphics.DrawString("Mesa " + modelPrintComanda.Mesa, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }
                else
                {
                    e.Graphics.DrawString("Para llevar", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }
                y += 25;
            }


            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 50, width, height));
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 50, y + 50, width / 3, height));
            e.Graphics.DrawString("Observaciones ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 160, y + 50, width / 2, height));

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            foreach (Producto item in modelPrintComanda.Productos)
            {

                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 10, y + 70, 40, height));
                e.Graphics.DrawString(item.Nombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 50, y + 70, width / 3, height));
                e.Graphics.DrawString(item.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 160, y + 70, width / 2, height));

                string textBig = item.Nombre.Length > item.Observaciones.Length ? item.Nombre : item.Observaciones;
                SizeF textSize = e.Graphics.MeasureString(textBig, new Font(fontFamily, fontSize)); y += textSize.Height + 35;
            }

            e.Graphics.DrawString("Observaciones adicionales", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 100, width, height));
            e.Graphics.DrawString(modelPrintComanda.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 115, width, height));
            e.Graphics.DrawString(ConfigurationManager.AppSettings["nombre_licencia"], new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x+10, y + 310, width, height));
        }

        private void ImprimirComandaSinDrivers(object sender, PrintPageEventArgs e)
        {
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            float height = 0F;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            float y = float.Parse(ConfigurationManager.AppSettings["espaciado_y_comanda"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x_comanda"]);
            FontStyle fontStyle = FontStyle.Regular;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString("Comanda #" + modelPrintComanda.NumComanda, new Font(fontFamily, 15, fontStyle), Brushes.Black, new RectangleF(x + 40, y, width, height), sf);
            if (modelPrintComanda.Mesa != "No definida")
            {
                if (modelPrintComanda.Mesa != "Para llevar")
                {
                    e.Graphics.DrawString("Mesa " + modelPrintComanda.Mesa, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x + 40, y + 25, width, height), sf);
                }
                else
                {
                    e.Graphics.DrawString("Para llevar", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x +40, y + 25, width, height), sf);
                }
                y += 25;
            }


            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 50, width, height));
            e.Graphics.DrawString("Descripcion ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 50, y + 50, width, height));
            e.Graphics.DrawString("Observaciones ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 160, y + 50, width, height));

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            /*foreach (Producto item in modelPrintComanda.Productos)
            {

                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 10, y + 70, 40, height));
                e.Graphics.DrawString(item.Nombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 50, y + 70, width / 3, height));
                e.Graphics.DrawString(item.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 160, y + 70, width / 2, height));
                string textBig = item.Nombre.Length > item.Observaciones.Length ? item.Nombre : item.Observaciones;
                SizeF textSize = e.Graphics.MeasureString(textBig, new Font(fontFamily, fontSize)); y += textSize.Height + 5;
            }*/

            e.Graphics.DrawString("Observaciones adicionales", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 100, width, height));
            e.Graphics.DrawString(modelPrintComanda.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 115, width, height));
            e.Graphics.DrawString(ConfigurationManager.AppSettings["nombre_licencia"], new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x+10, y + 310, width, height));
        }

        public int GetYByHeigth(string text, int caracteres_permitidos = 24)
        {
            double line = text.Length / caracteres_permitidos; // es el total de caracteres por linea permitida en la descripcion del producto
            double lines = Math.Round(line);
            double lines1 = Math.Floor(line);
            double lines2 = Math.Ceiling(line);
            string value = (30 * (lines + 1)).ToString();
            if (caracteres_permitidos == 24)
            {
                value = (20 * (lines + 1)).ToString();
            }
            
            return int.Parse(value);
        }

        private void ImprimirFacturaSmall(object sender, PrintPageEventArgs e)
        {

            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            int height = 300;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 9 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            int x_detalles = int.Parse(ConfigurationManager.AppSettings["x_menos_detalles"]);

            Image img = Image.FromFile("C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png");
            int y = 150 + int.Parse(ConfigurationManager.AppSettings["espaciado_logo"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x"]);
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;

            if (ConfigurationManager.AppSettings["url_logo"].ToString().Trim() != "") e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            e.Graphics.DrawString(modelPrintFactura.Empresa, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Nit, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Ciudad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 45, width, height), center);

            y += 45;
            e.Graphics.DrawString(modelPrintFactura.Telefono, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Direccion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Resolucion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 70, width - 20, height));
            y += int.Parse(ConfigurationManager.AppSettings["espaciado_resolucion"]);
            e.Graphics.DrawString("Factura Venta N°: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width, height));
            e.Graphics.DrawString(modelPrintFactura.NumFactura, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 85, y + 70, width, height));

            e.Graphics.DrawString("Fecha: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 90, width, height));
            e.Graphics.DrawString(modelPrintFactura.Fecha, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 35, y + 90, width, height));

            e.Graphics.DrawString("INFORMACIÓN DEL CLIENTE", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 110, width, height), center);

            e.Graphics.DrawString("Nombre: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 130, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteNombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 45, y + 130, width, height));

            e.Graphics.DrawString("Teléfono: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 150, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteTelefono, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 48, y + 150, width, height));

            e.Graphics.DrawString("Direccion: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteDireccion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 52, y + 170, width, height));

            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width / 1.5f, height));
            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 100, y + 200, width / 4, height));
            e.Graphics.DrawString("Valor ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);

            y += 20;



            foreach (DetalleFactura item in modelPrintFactura.DetallesFactura)
            {
                e.Graphics.DrawString(item.Descripcion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width / 2f, height));
                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 100, y + 200, width / 3f, height));
                e.Graphics.DrawString(item.Valor, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);
                y += this.GetYByHeigthSmall(item.Descripcion);
            }
            e.Graphics.DrawString("Subtotal: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Subtotal, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            y += 20;
            e.Graphics.DrawString("Domicilio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Domicilio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            e.Graphics.DrawString("Descuento: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 240, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Descuento, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 240, width, height), sf_right);
            if (modelPrintFactura.Servicio != "$0")
            {
                e.Graphics.DrawString("Servicio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 260, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.Servicio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
                y += 20;
            }
            e.Graphics.DrawString("Total: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 260, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Total, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
            e.Graphics.DrawString("Forma de pago: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 280, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.FormaPago, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 280, width, height), sf_right);
            if (modelPrintFactura.PagaCon != null && modelPrintFactura.PagaCon != "")
            {
                e.Graphics.DrawString("Paga con: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 50, y + 300, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.PagaCon, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 300, width, height), sf_right);
                y += 20;
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(modelPrintFactura.Texto, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 30, y + 330, width / 1.4f, height), sf);
            e.Graphics.DrawString("¡Gracias por su compra!", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 400, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x, y + 620, width, height));


        }

        private void ImprimirFacturaSmallSinDrivers(object sender, PrintPageEventArgs e)
        {

            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            int height = 300;
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 9 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            int x_detalles = int.Parse(ConfigurationManager.AppSettings["x_menos_detalles"]);

            Image img = Image.FromFile("C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png");
            int y = 150 + int.Parse(ConfigurationManager.AppSettings["espaciado_logo"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x"]);
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;

            if (ConfigurationManager.AppSettings["url_logo"].ToString().Trim() != "") e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            e.Graphics.DrawString(modelPrintFactura.Empresa, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Nit, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Ciudad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 45, width, height), center);

            y += 45;
            e.Graphics.DrawString(modelPrintFactura.Telefono, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Direccion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Resolucion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 70, width - 20, height));
            y += int.Parse(ConfigurationManager.AppSettings["espaciado_resolucion"]);
            e.Graphics.DrawString("Factura Venta N°: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width, height));
            e.Graphics.DrawString(modelPrintFactura.NumFactura, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 85, y + 70, width, height));

            e.Graphics.DrawString("Fecha: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 90, width, height));
            e.Graphics.DrawString(modelPrintFactura.Fecha, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 35, y + 90, width, height));

            e.Graphics.DrawString("INFORMACIÓN DEL CLIENTE", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 110, width, height), center);

            e.Graphics.DrawString("Nombre: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 130, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteNombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 45, y + 130, width, height));

            e.Graphics.DrawString("Teléfono: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 150, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteTelefono, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 48, y + 150, width, height));

            e.Graphics.DrawString("Direccion: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteDireccion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 52, y + 170, width, height));

            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width / 1.5f, height));
            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 100, y + 200, width / 4, height));
            e.Graphics.DrawString("Valor ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);

            y += 20;



            foreach (DetalleFactura item in modelPrintFactura.DetallesFactura)
            {
                e.Graphics.DrawString(item.Descripcion, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width / 2f, height));
                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 100, y + 200, width / 3f, height));
                e.Graphics.DrawString(item.Valor, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 200, width, height), sf_right);
                y += this.GetYByHeigthSmall(item.Descripcion);
            }
            e.Graphics.DrawString("Subtotal: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Subtotal, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            y += 20;
            e.Graphics.DrawString("Domicilio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 220, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Domicilio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 220, width, height), sf_right);
            e.Graphics.DrawString("Descuento: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 240, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Descuento, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 240, width, height), sf_right);
            if (modelPrintFactura.Servicio != "$0")
            {
                e.Graphics.DrawString("Servicio: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 260, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.Servicio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
                y += 20;
            }
            e.Graphics.DrawString("Total: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 260, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.Total, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 260, width, height), sf_right);
            e.Graphics.DrawString("Forma de pago: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 60, y + 280, width, height), sf_right);
            e.Graphics.DrawString(modelPrintFactura.FormaPago, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 280, width, height), sf_right);
            if (modelPrintFactura.PagaCon != null && modelPrintFactura.PagaCon != "")
            {
                e.Graphics.DrawString("Paga con: ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x - 50, y + 300, width, height), sf_right);
                e.Graphics.DrawString(modelPrintFactura.PagaCon, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(x, y + 300, width, height), sf_right);
                y += 20;
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(modelPrintFactura.Texto, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 30, y + 330, width / 1.4f, height), sf);
            e.Graphics.DrawString("¡Gracias por su compra!", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 400, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x, y + 620, width, height));


        }

        private void ImprimirComandaSmall(object sender, PrintPageEventArgs e)
        {

            int height = 280;
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            int y = int.Parse(ConfigurationManager.AppSettings["espaciado_y_comanda"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x_comanda"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString("Comanda #" + modelPrintComanda.NumComanda, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y, width, height), sf);
            if (modelPrintComanda.Mesa != "No definida")
            {
                if (modelPrintComanda.Mesa != "Para llevar")
                {
                    e.Graphics.DrawString("Mesa " + modelPrintComanda.Mesa, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }
                else
                {
                    e.Graphics.DrawString("Para llevar", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }
                
                y += 25;
            }


            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 0, y + 50, width / 4, height));
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 40, y + 50, width / 2.5f, height));
            e.Graphics.DrawString("Observaciones ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 120, y + 50, (width / 2f), height));

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            foreach (Producto item in modelPrintComanda.Productos)
            {

                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width / 4, height), sf);
                e.Graphics.DrawString(item.Nombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 40, y + 70, width / 2.5f, height));
                e.Graphics.DrawString(item.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 120, y + 70, (width / 2f), height));

                if (item.Nombre.Length > item.Observaciones.Length) y += this.GetYByHeigthSmall(item.Nombre, 17); else y += this.GetYByHeigthSmall(item.Observaciones, 17);
            }

            e.Graphics.DrawString("Observaciones adicionales", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 100, width, height));
            e.Graphics.DrawString(modelPrintComanda.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 115, width, height));
            e.Graphics.DrawString(ConfigurationManager.AppSettings["nombre_licencia"], new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x+10, y + 310, width, height));
        }

        private void ImprimirComandaSmallSinDrivers(object sender, PrintPageEventArgs e)
        {

            int height = 280;
            int width = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            string fontFamily = ConfigurationManager.AppSettings["family_letra"];
            int fontSize = 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            int y = int.Parse(ConfigurationManager.AppSettings["espaciado_y_comanda"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x_comanda"]);
            bool negrita = ConfigurationManager.AppSettings["letra_negrita"] == "true" ? true : false;
            FontStyle fontStyle = negrita ? FontStyle.Bold : FontStyle.Regular;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString("Comanda #" + modelPrintComanda.NumComanda, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y, width, height), sf);
            if (modelPrintComanda.Mesa != "No definida")
            {
                if (modelPrintComanda.Mesa != "Para llevar")
                {
                    e.Graphics.DrawString("Mesa " + modelPrintComanda.Mesa, new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }
                else
                {
                    e.Graphics.DrawString("Para llevar", new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x - 0, y + 25, width, height), sf);
                }

                y += 25;
            }


            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 0, y + 50, width / 4, height));
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 40, y + 50, width / 2.5f, height));
            e.Graphics.DrawString("Observaciones ", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x + 120, y + 50, (width / 2f), height));

            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            foreach (Producto item in modelPrintComanda.Productos)
            {

                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 70, width / 4, height), sf);
                e.Graphics.DrawString(item.Nombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 40, y + 70, width / 2.5f, height));
                e.Graphics.DrawString(item.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 120, y + 70, (width / 2f), height));

                if (item.Nombre.Length > item.Observaciones.Length) y += this.GetYByHeigthSmall(item.Nombre, 17); else y += this.GetYByHeigthSmall(item.Observaciones, 17);
            }

            e.Graphics.DrawString("Observaciones adicionales", new Font(fontFamily, fontSize, fontStyle), Brushes.Black, new RectangleF(x, y + 100, width, height));
            e.Graphics.DrawString(modelPrintComanda.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x, y + 115, width, height));
            e.Graphics.DrawString(ConfigurationManager.AppSettings["nombre_licencia"], new Font(fontFamily, fontSize + 3, fontStyle), Brushes.Black, new RectangleF(x, y + 170, width, height), sf);
            //e.Graphics.DrawString("-----------------------------------------------------------", new Font(fontFamily, 10), Brushes.Black, new RectangleF(x+10, y + 310, width, height));
        }

        public int GetYByHeigthSmall(string text, int caracteres_permitidos = 14)
        {
            double line = text.Length / caracteres_permitidos; // es el total de caracteres por linea permitida en la descripcion del producto
            double lines = Math.Round(line);
            string value = (20 * (lines + 1)).ToString();
            return int.Parse(value);
        }

        public static void SaveImage(string imageUrl, string filename, ImageFormat format)
        {

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
                bitmap.Save(filename, format);

            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }

    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "RAW Document";
            // Win7
            di.pDataType = "RAW";

            // Win8+
            // di.pDataType = "XPS_PASS";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            fs.Close();
            fs.Dispose();
            fs = null;
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }
}
