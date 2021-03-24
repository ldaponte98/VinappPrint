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
            PrinterSettings ps = new PrinterSettings();
            if (impresora != null && impresora.Trim() != "") ps.PrinterName = impresora;
            string imagen = "C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png";
            if (!File.Exists(imagen))
            {
                SaveImage(ConfigurationManager.AppSettings["url_logo"], "C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png", ImageFormat.Png);
            }
            
            Tools.SaveLog("Imprimiendo desde " + ps.PrinterName);
            printDocument.PrinterSettings = ps;
            if (tipo == "factura") printDocument.PrintPage += ImprimirFactura;
            if (tipo == "comanda") printDocument.PrintPage += ImprimirComanda;
            printDocument.Print();
            if(ConfigurationManager.AppSettings["corte_automatico"] == "true") CutTicket();

        }

        private void ImprimirFactura(object sender, PrintPageEventArgs e)
        {
            
            int width = 300;
            int height = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            string fontFamily = "Arial";
            int fontSize = 9 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]); 
            
            Image img = Image.FromFile("C:\\Users\\" + Environment.UserName + "\\logo_vinapp.png");
            int y = 150 + int.Parse(ConfigurationManager.AppSettings["espaciado_logo"]);
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x"]);
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;

            if(ConfigurationManager.AppSettings["url_logo"].ToString().Trim() != "") e.Graphics.DrawImage(img, new Point(int.Parse(ConfigurationManager.AppSettings["x_logo"]), 0));
            e.Graphics.DrawString(modelPrintFactura.Empresa, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Nit, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Ciudad, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 45, width, height), center);

            y += 45;
            e.Graphics.DrawString(modelPrintFactura.Telefono, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 15, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Direccion, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 30, width, height), center);

            e.Graphics.DrawString(modelPrintFactura.Resolucion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 10, y + 70, width - 20, height));
            y += int.Parse(ConfigurationManager.AppSettings["espaciado_resolucion"]);
            e.Graphics.DrawString("Factura Venta N°: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 70, width, height));
            e.Graphics.DrawString(modelPrintFactura.NumFactura, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 120, y + 70, width, height));

            e.Graphics.DrawString("Fecha: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 90, width, height));
            e.Graphics.DrawString(modelPrintFactura.Fecha, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 55, y + 90, width, height));

            e.Graphics.DrawString("Información del cliente: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 110, width, height));

            e.Graphics.DrawString("Nombre: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 130, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteNombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 63, y + 130, width, height));

            e.Graphics.DrawString("Teléfono: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 150, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteTelefono, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 68, y + 150, width, height));

            e.Graphics.DrawString("Direccion: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 170, width, height));
            e.Graphics.DrawString(modelPrintFactura.ClienteDireccion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 74, y + 170, width, height));

            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 10, y + 200, width, height));
            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 170, y + 200, width, height));
            e.Graphics.DrawString("Valor ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 235, y + 200, width, height));

            y += 20;
            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            foreach (DetalleFactura item in modelPrintFactura.DetallesFactura)
            {
                
                e.Graphics.DrawString(item.Descripcion, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 10, y + 200, 150, height));
                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 180, y + 200, width, height));
                e.Graphics.DrawString(item.Valor, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(-25, y + 200, width, height), sf_right);

                y += this.GetYByHeigth(item.Descripcion);
            }
            e.Graphics.DrawString("Subtotal: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 140, y + 220, width, height));
            e.Graphics.DrawString(modelPrintFactura.Subtotal, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(-25, y + 220, width, height), sf_right);
            y += 20;
            e.Graphics.DrawString("Domicilio: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 135, y + 220, width, height));
            e.Graphics.DrawString(modelPrintFactura.Domicilio, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(-25, y + 220, width, height), sf_right); 
            e.Graphics.DrawString("Descuento: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 127, y + 240, width, height));
            e.Graphics.DrawString(modelPrintFactura.Descuento, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(-25, y + 240, width, height), sf_right);
            e.Graphics.DrawString("Total: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 160, y + 260, width, height));
            e.Graphics.DrawString(modelPrintFactura.Total, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(-25, y + 260, width, height), sf_right);
            e.Graphics.DrawString("Forma de pago: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 100, y + 280, width, height));
            e.Graphics.DrawString(modelPrintFactura.FormaPago, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(-25, y + 280, width, height), sf_right);
            if(modelPrintFactura.PagaCon != null && modelPrintFactura.PagaCon != "")
            {
                e.Graphics.DrawString("Paga con: ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 140, y + 300, width, height));
                e.Graphics.DrawString(modelPrintFactura.PagaCon, new Font(fontFamily, 8.5f), Brushes.Black, new RectangleF(-25, y + 300, width, height), sf_right);
                y += 20;
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(modelPrintFactura.Texto, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 30, y + 330, width / 1.4f, height), sf);
            e.Graphics.DrawString("¡Gracias por su compra!", new Font(fontFamily, 12 + + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]) , FontStyle.Bold), Brushes.Black, new RectangleF(x+40, y + 400, width, height));
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

            int width = 280;
            int height = int.Parse(ConfigurationManager.AppSettings["ancho"]);
            string fontFamily = "Arial";
            int fontSize = 10 + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]);
            int y = 30;
            int x = int.Parse(ConfigurationManager.AppSettings["espaciado_x_comanda"]);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            e.Graphics.DrawString("Comanda #"+modelPrintComanda.NumComanda, new Font(fontFamily, 15, FontStyle.Bold), Brushes.Black, new RectangleF(x - 10, y, width, height), sf);

            e.Graphics.DrawString("Cant ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 5, y + 50, width, height));
            e.Graphics.DrawString("Descripción ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 50, y + 50, width, height));
            e.Graphics.DrawString("Observaciones ", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 160, y + 50, width, height));
            
            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;
            foreach (Producto item in modelPrintComanda.Productos)
            {

                e.Graphics.DrawString(item.Cantidad, new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 15, y + 70, 10, height));
                e.Graphics.DrawString(item.Nombre, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 50, y + 70, 120, height));
                e.Graphics.DrawString(item.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 160, y + 70, 130, height));

                if (item.Nombre.Length > item.Observaciones.Length) y += this.GetYByHeigth(item.Nombre, 17); else y += this.GetYByHeigth(item.Observaciones, 17);
            }

            e.Graphics.DrawString("Observaciones adicionales", new Font(fontFamily, fontSize, FontStyle.Bold), Brushes.Black, new RectangleF(x + 15, y + 100, width, height));
            e.Graphics.DrawString(modelPrintComanda.Observaciones, new Font(fontFamily, fontSize), Brushes.Black, new RectangleF(x + 15, y + 115, width, height));
            e.Graphics.DrawString(ConfigurationManager.AppSettings["nombre_licencia"], new Font(fontFamily, 12 + + int.Parse(ConfigurationManager.AppSettings["tama_fuente"]) , FontStyle.Bold), Brushes.Black, new RectangleF(x, y + 170, width, height), sf);
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
