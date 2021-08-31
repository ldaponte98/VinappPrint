using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VinappPrint
{
    public partial class TypeDriversPrinter : Form
    {
        public TypeDriversPrinter()
        {
            InitializeComponent();
        }

        private void TypeDriversPrinter_Load(object sender, EventArgs e)
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboPrinters.Items.Add(printer);                
            }
            if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count > 0)
            {
                comboPrinters.SelectedIndex = 0;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string impresora = comboPrinters.Text;
            if (impresora != "")
            {
                printDocument = new PrintDocument();
                printDocument.PrintPage += new PrintPageEventHandler(ImprimirTest); // Filling in the stuff
                // Print Controller
                StandardPrintController spc = new StandardPrintController();
                printDocument.PrintController = spc; // This hides popup
                

                // Printer Settings
                PrinterSettings ps = new PrinterSettings();
                ps.PrinterName = impresora;
                printDocument.PrinterSettings = ps;
                var paperSize = new PaperSize("A4", 1000, 5000);
                printDocument.DefaultPageSettings.PaperSize = paperSize;
                printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                printDocument.Print();

                string corte = "\x1B" + "m";                                    // caracteres de corte
                string avance = "\x1B" + "d" + "\x09";                          // avanza 9 renglones
                RawPrinterHelper.SendStringToPrinter(impresora, avance);        // avanza
                RawPrinterHelper.SendStringToPrinter(impresora, corte);         // corta
            }
            else
            {
                MessageBox.Show("Debe escoger una impresora valida", "Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImprimirTest(object sender, PrintPageEventArgs e)
        {
            StringFormat center = new StringFormat();
            center.Alignment = StringAlignment.Center;
            FontStyle fontStyle = FontStyle.Regular;
            FontStyle fontStyleBold = FontStyle.Bold;
            string fontFamily = "Arial";
            int fontSize = 12; 
            string text1 = "Este es un texto de prueba para verificacion del tipo de controlador que la impresora actualmente maneja.";
            int x = 5;
            float y = 0f;
            float width = 300F;
            float height = 0F;
            Font font = new Font(fontFamily, fontSize, fontStyleBold);
            e.Graphics.DrawString("Vinapp Test", font, Brushes.Black, new RectangleF(x, y, width, height), center);
            SizeF textSize = e.Graphics.MeasureString("Vinapp Test", font); y += textSize.Height + 50;
            font = new Font(fontFamily, fontSize, fontStyle);
            Image img = Properties.Resources.LogoVinapp;
            e.Graphics.DrawString(text1, font, Brushes.Black, new RectangleF(x, y, width, height));
            textSize = e.Graphics.MeasureString(text1, font); y += textSize.Height + 50;
            e.Graphics.DrawString(".", font, Brushes.Black, new RectangleF(x, y+100, width, height));



            //SizeF textSize = e.Graphics.MeasureString(text, font); y += textSize.Height + 50;
            //e.Graphics.DrawString("Fin Test", font, Brushes.Black, new RectangleF(x, y, width, height), center);
        }
    }
}
