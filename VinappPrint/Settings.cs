using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VinappPrint.Infraestructure;

namespace VinappPrint
{
    public partial class Settings : Form
    {
        List<Models.Point> points;
        public bool firstAccess = false;
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            string str = LocalStorage.Get("puntos");
            points = JsonConvert.DeserializeObject<List<Models.Point>>(str);

            txtCompany.Text = LocalStorage.Get("nombre_licencia");
            txtLogo.Text = LocalStorage.Get("url_logo");

            int cont = 0;
            int pos = 0;
            foreach (var item in points)
            {
                comboPoints.Items.Add(item.name);
                if (LocalStorage.Get("punto") == item.id_point.ToString()) pos = cont;
                cont++;
            }
            comboPoints.SelectedIndex = pos;

            cont = 0;
            pos = 0;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboPrintersComand.Items.Add(printer);
                if (LocalStorage.Get("impresoras_comanda") == printer) pos = cont;
                cont++;
            }
            comboPrintersComand.SelectedIndex = pos;

            cont = 0;
            pos = 0;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboPrintersInvoice.Items.Add(printer);
                if (LocalStorage.Get("impresoras_facturacion_mesa") == printer) pos = cont;
                cont++;
            }
            comboPrintersInvoice.SelectedIndex = pos;

            comboPrintersValids.Items.Add("Facturas y comandas");
            comboPrintersValids.Items.Add("Solo facturas");
            comboPrintersValids.Items.Add("Solo comandas");

            pos = 0;
            if (LocalStorage.Get("permite_facturacion") == "true") pos = 1;
            if (LocalStorage.Get("permite_comanda") == "true") pos = 2;
            if (LocalStorage.Get("permite_facturacion") == "true" && LocalStorage.Get("permite_comanda") == "true") pos = 0;

            comboPrintersValids.SelectedIndex = pos;

            
            comboFirstPrinter.Items.Add("Comanda");
            comboFirstPrinter.Items.Add("Factura");

            pos = 0;
            if (LocalStorage.Get("imprime_primero") == "factura") pos = 1;
            comboFirstPrinter.SelectedIndex = pos;

            comboCutAutomatic.Items.Add("No");
            comboCutAutomatic.Items.Add("Si");

            pos = 0;
            if (LocalStorage.Get("corte_automatico") == "true") pos = 1;
            comboCutAutomatic.SelectedIndex = pos;

            txtSizeFont.Text = (int.Parse(LocalStorage.Get("tama_fuente")) + 12).ToString();
            txtWeightPaper.Text = LocalStorage.Get("ancho");
            comboTypePaper.Items.Add("Normal");
            comboTypePaper.Items.Add("Pequeño");

            pos = 0;
            if (LocalStorage.Get("tipo_papel") == "Pequeño") pos = 1;
            comboTypePaper.SelectedIndex = pos;

            txtSpaceLeftDetails.Text = LocalStorage.Get("espaciado_x");
            txtSpaceTopLogo.Text = LocalStorage.Get("espaciado_logo");
            txtSpaceLeftLogo.Text = LocalStorage.Get("x_logo");
            txtSpaceResolution.Text = LocalStorage.Get("espaciado_resolucion");
            checkPrinterDrivers.Checked = LocalStorage.Get("drivers_originales") == "true" ? true : false;
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
            loading.Start();
            LocalStorage.Save("nombre_licencia", txtCompany.Text);
            LocalStorage.Save("punto", getPointByName(comboPoints.Text));
            LocalStorage.Save("url_logo", txtLogo.Text);
            LocalStorage.Save("impresoras_comanda", comboPrintersComand.Text);
            LocalStorage.Save("impresoras_facturacion_mesa", comboPrintersInvoice.Text);
            LocalStorage.Save("impresoras_facturacion_domicilio", comboPrintersInvoice.Text);

            if (comboPrintersValids.Text == "Facturas y comandas")
            {
                LocalStorage.Save("permite_facturacion", "true");
                LocalStorage.Save("permite_comanda", "true");
            }

            if (comboPrintersValids.Text == "Solo facturas")
            {
                LocalStorage.Save("permite_facturacion", "true");
                LocalStorage.Save("permite_comanda", "false");
            }

            if (comboPrintersValids.Text == "Solo comandas")
            {
                LocalStorage.Save("permite_facturacion", "false");
                LocalStorage.Save("permite_comanda", "true");
            }
            LocalStorage.Save("imprime_primero", comboFirstPrinter.Text.ToLower());
            if (comboFirstPrinter.Text == "Si")
            {
                LocalStorage.Save("corte_automatico", "true");
            }
            else
            {
                LocalStorage.Save("corte_automatico", "false");
            }
            
            LocalStorage.Save("tipo_papel", comboTypePaper.Text);
            LocalStorage.Save("tama_fuente", int.Parse(txtSizeFont.Text) - 12);
            LocalStorage.Save("ancho", txtWeightPaper.Text);
            LocalStorage.Save("espaciado_x", txtSpaceLeftDetails.Text);
            LocalStorage.Save("espaciado_logo", txtSpaceTopLogo.Text);
            LocalStorage.Save("x_logo", txtSpaceLeftLogo.Text);
            LocalStorage.Save("espaciado_resolucion", txtSpaceResolution.Text);
            LocalStorage.Save("drivers_originales", checkPrinterDrivers.Checked ? "true" : "false");
            loading.Stop();
            MessageBox.Show("Cambios guardados exitosamente", "Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (firstAccess)
            {
                Main form = new Main();
                form.Show();
                this.Close();
            }
            else
            {
                this.Close();
            }
            
        }

        public string getPointByName(string name)
        {
            foreach (var item in points)
            {
                if (item.name.ToString() == name) return item.id_point.ToString();
            }
            return "";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TypeDriversPrinter form = new TypeDriversPrinter();
            form.Show();
        }
    }
}
