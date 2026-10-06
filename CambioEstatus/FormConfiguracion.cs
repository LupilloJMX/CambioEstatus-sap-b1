using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CambioEstatus
{
    public partial class FormConfiguracion : Form
    {
        public SapConfig? ConfiguracionGuardada { get; private set; }
        public FormConfiguracion()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
            labelEstado.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Cargar config existente al abrir(si la hay)
        private void FormConfiguracion_Load(object sender, EventArgs e)
        {
            var config = ConfigManager.Cargar();
            if (config != null)
            {
                txtUrl.Text = config.ServiceLayerUrl;
                txtUser.Text = config.UserName;
                txtCompany.Text = config.CompanyDB;
                // No mostramos la contraseña por seguridad.
                // El usuario tendrá que reescribirla si quiere cambiarla.
            }
        }

        // ---- Validación básica ----
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Ingrese la URL del Service Layer.");
                txtUrl.Focus();
                return false;
            }
            if (!Uri.TryCreate(txtUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                MessageBox.Show("La URL no tiene un formato válido.");
                txtUrl.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Ingrese el usuario.");
                txtUser.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Ingrese la contraseña.");
                txtPass.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCompany.Text))
            {
                MessageBox.Show("Ingrese la CompanyDB.");
                txtCompany.Focus();
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var config = new SapConfig
            {
                ServiceLayerUrl = txtUrl.Text.Trim(),
                UserName = txtUser.Text.Trim(),
                Password = txtPass.Text,
                CompanyDB = txtCompany.Text.Trim()
            };

            try
            {
                ConfigManager.Guardar(config);
                ConfiguracionGuardada = config;

                MessageBox.Show("Configuración guardada correctamente.",
                                "Éxito",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            button1.Enabled = false;
            labelEstado.Text = "Probando conexión...";
            labelEstado.ForeColor = System.Drawing.Color.Orange;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    (s, cert, chain, errors) => true;

                using var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(txtUrl.Text.Trim()),
                    Timeout = TimeSpan.FromSeconds(30)
                };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var loginData = new
                {
                    UserName = txtUser.Text.Trim(),
                    Password = txtPass.Text,
                    CompanyDB = txtCompany.Text.Trim()
                };

                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("Login", content);

                if (response.IsSuccessStatusCode)
                {
                    labelEstado.Text = " Conexión exitosa";
                    labelEstado.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    labelEstado.Text = $" Error: {response.StatusCode}";
                    labelEstado.ForeColor = System.Drawing.Color.Red;
                    MessageBox.Show($"Error al conectar:\n{errorBody}");
                }
            }
            catch (Exception ex)
            {
                labelEstado.Text = " Error de conexión";
                labelEstado.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Excepción: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
    }
}
