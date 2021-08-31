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
using static VinappPrint.Infraestructure.API;

namespace VinappPrint
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }


        public delegate void UpdateValidateLogin(Response response);
        public UpdateValidateLogin m_DelegateUpdateValidateLogin;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text.Trim() != "")
            {
                btnLogin.Visible = false;
                loading.Start();
                loading.Visible = true;
                m_DelegateUpdateValidateLogin = CallUpdateValidatelogin;
                var task = new Task(() =>
                {
                    Response response = API.ValidateLogin(txtEmail.Text.ToString().Trim(), txtPassword.Text.ToString());
                    this.Invoke(m_DelegateUpdateValidateLogin, response);
                });
                task.Start();
            }
            else
            {
                MessageBox.Show("El correo electronico es obligatorio", "Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        public void CallUpdateValidatelogin(Response response)
        {
            loading.Stop();
            loading.Visible = false;
            if (!response.Error)
            {
                LocalStorage.Save("socket", response.Data.socket);
                LocalStorage.Save("licencia", response.Data.id);
                LocalStorage.Save("nombre_licencia", response.Data.name);
                LocalStorage.Save("url_logo", response.Data.logo);
                LocalStorage.Save("puntos", response.Data.points);
                LocalStorage.Save("login", "true");

                Settings form = new Settings();
                form.firstAccess = true;
                form.Show();
                this.Visible = false;
            }
            else
            {
                btnLogin.Visible = true;
                MessageBox.Show(response.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
