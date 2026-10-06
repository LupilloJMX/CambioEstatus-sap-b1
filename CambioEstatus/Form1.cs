using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CambioEstatus
{
    public partial class Form1 : Form
    {
        private HttpClient _client;
        // private readonly HttpClient _client = new HttpClient();
        //private const string ServiceLayerUrl = "https://SERVSAP:50000/b1s/v1/";
        //private const string UserName = "manager";
        //private const string Password = "C2Admin2";
        //private const string CompanyDB = "CLASS_TEST_20260517";

        private SapConfig? _config;

        private bool _isConnected = false;
        public Form1()
        {
            InitializeComponent();

            label1.Text = "Desconectado";
            label1.ForeColor = System.Drawing.Color.Red;

            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }


        private void LimpiarDataGridView()
        {
            if (dataGridView1.DataSource != null)
            {
                dataGridView1.DataSource = null;
            }
            else
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
            }

            //refrescar para que se vea el cambio inmediatamente
            dataGridView1.Refresh();
        }

        private async Task<bool> ConectarASAP()
        {
            try
            {
                // Si no hay config en memoria, intentar cargar del archivo cifrado
                if (_config == null)
                {
                    _config = ConfigManager.Cargar();
                }

                // Si no hay config guardada, abrir el formulario de configuración
                if (_config == null)
                {
                    using var formConfig = new FormConfiguracion();
                    if (formConfig.ShowDialog() != DialogResult.OK)
                        return false;

                    _config = formConfig.ConfiguracionGuardada;
                    if (_config == null) return false;
                }

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    (s, cert, chain, errors) => true;

                _client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(_config.ServiceLayerUrl),
                    Timeout = TimeSpan.FromSeconds(60)
                };

                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var loginData = new
                {
                    UserName = _config.UserName,
                    Password = _config.Password,
                    CompanyDB = _config.CompanyDB
                };

                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var sessionData = JObject.Parse(responseBody);
                    string sessionId = sessionData["SessionId"]?.ToString();

                    if (string.IsNullOrEmpty(sessionId))
                    {
                        MessageBox.Show("No se recibió SessionId de SAP.");
                        return false;
                    }

                    _client.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={sessionId}");

                    string routeId = sessionData["RouteId"]?.ToString();
                    if (!string.IsNullOrEmpty(routeId))
                    {
                        _client.DefaultRequestHeaders.Add("Cookie", $"ROUTEID={routeId}");
                    }

                    _isConnected = true;
                    return true;
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al conectar: {response.StatusCode} - {response.ReasonPhrase}\n{errorBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception al conectar: {ex.Message}");
                return false;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label1.Text = "Conectando...";
            label1.ForeColor = System.Drawing.Color.Orange;

            bool conexionExitosa = await ConectarASAP();

            if (conexionExitosa)
            {
                label1.Text = "Conectado a SAP B1 Service Layer";
                label1.ForeColor = System.Drawing.Color.Green;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;

            }
            else
            {
                label1.Text = "Error de conexión";
                label1.ForeColor = System.Drawing.Color.Red;
            }

            button1.Enabled = true;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (_client == null) return;

                HttpResponseMessage response = await _client.PostAsync("Logout", null);

                if (response.IsSuccessStatusCode)
                {
                    _client.DefaultRequestHeaders.Remove("Cookie");
                    _isConnected = false;

                    LimpiarDataGridView();

                    MessageBox.Show("Sesión cerrada correctamente.");
                    label1.Text = "Desconectado";
                    label1.ForeColor = System.Drawing.Color.Red;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                }
                else
                {
                    MessageBox.Show($"Error al desconectar: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desconectar: {ex.Message}");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            if (!_isConnected || _client == null)
            {
                MessageBox.Show("Primero conéctese a SAP B1 Service Layer.");
                return;
            }

            button3.Enabled = false;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            try
            {
                // Consulta crossjoin: Items + ItemWarehouseInfoCollection
                string query =
                    "$crossjoin(Items,Items/ItemWarehouseInfoCollection)" +
                    "?$expand=Items($select=ItemCode,ItemName,U_Clasificacion_venta)," +
                    "Items/ItemWarehouseInfoCollection($select=InStock,WarehouseCode)" +
                    "&$filter=Items/ItemCode eq Items/ItemWarehouseInfoCollection/ItemCode " +
                    "and Items/U_Clasificacion_venta eq 'NOVEDAD' " +
                    "and Items/ItemWarehouseInfoCollection/WarehouseCode eq '1' " +
                    "and Items/ItemWarehouseInfoCollection/InStock ge 1";

                HttpResponseMessage response = await _client.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al consultar: {response.StatusCode} - {response.ReasonPhrase}\n{errorBody}");
                    return;
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseBody);
                var values = jsonResponse["value"] as JArray;

                // Definir columnas del DataGridView
                dataGridView1.Columns.Add("ItemCode", "Número de Parte");
                dataGridView1.Columns.Add("ItemName", "Descripción");
                dataGridView1.Columns.Add("U_Clasificacion_venta", "Clasificación");
                dataGridView1.Columns.Add("WarehouseCode", "Almacén");
                dataGridView1.Columns.Add("InStock", "Stock");

                if (values == null || values.Count == 0)
                {
                    MessageBox.Show("No se encontraron artículos con esos criterios.");
                    return;
                }

                foreach (var row in values)
                {
                    // En crossjoin, los datos vienen anidados dentro de las propiedades Items y ItemWarehouseInfoCollection
                    var item = row["Items"];
                    var whs = row["Items/ItemWarehouseInfoCollection"];

                    string itemCode = item?["ItemCode"]?.ToString() ?? "";
                    string itemName = item?["ItemName"]?.ToString() ?? "";
                    string clasif = item?["U_Clasificacion_venta"]?.ToString() ?? "";
                    string whsCode = whs?["WarehouseCode"]?.ToString() ?? "";
                    string inStock = whs?["InStock"]?.ToString() ?? "0";

                    dataGridView1.Rows.Add(itemCode, itemName, clasif, whsCode, inStock);
                }

                // Ajustar columnas
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;

                label1.Text = $"Conectado - {dataGridView1.Rows.Count} artículo(s) encontrado(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}");
            }
            finally
            {
                button3.Enabled = true;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            // Validaciones previas
            if (!_isConnected || _client == null)
            {
                MessageBox.Show("Primero conéctese a SAP B1 Service Layer.");
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No hay artículos en la lista para actualizar.");
                return;
            }

            // Confirmación al usuario
            var confirm = MessageBox.Show(
                $"¿Está seguro de cambiar U_Clasificacion_venta a 'INNOVACION' para {dataGridView1.Rows.Count} artículo(s)?",
                "Confirmar actualización",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);


            if (confirm != DialogResult.Yes) return;

            // Preparar UI
            button4.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            progressBar1.Visible = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = dataGridView1.Rows.Count;
            progressBar1.Value = 0;

            int actualizados = 0;
            int errores = 0;
            var erroresDetalle = new StringBuilder();

            // Constante con el nuevo valor
            const string NUEVO_VALOR = "INNOVACION";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string itemCode = row.Cells["ItemCode"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(itemCode)) continue;

                try
                {
                    // PATCH solo al campo U_Clasificacion_venta
                    var patchData = new { U_Clasificacion_venta = NUEVO_VALOR };
                    var json = JsonConvert.SerializeObject(patchData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage(
                        new HttpMethod("PATCH"),
                        $"Items('{itemCode}')")
                    {
                        Content = content
                    };

                    var response = await _client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        actualizados++;
                        // Actualizar visualmente el grid
                        row.Cells["U_Clasificacion_venta"].Value = NUEVO_VALOR;
                        row.DefaultCellStyle.BackColor = Color.FromArgb(235, 255, 235); //verde
                    }
                    else
                    {
                        errores++;
                        string errorBody = await response.Content.ReadAsStringAsync();
                        erroresDetalle.AppendLine($"x {itemCode}: {response.StatusCode} - {errorBody}");
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235); // Rojo suave
                    }
                }
                catch (Exception ex)
                {
                    errores++;
                    erroresDetalle.AppendLine($"x {itemCode}: {ex.Message}");
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                }

                // Avanzar progress bar
                progressBar1.Value++;
            }

            // Restaurar UI
            button4.Enabled = true;
            this.Cursor = Cursors.Default;
            progressBar1.Visible = false;

            // Reporte final
            string mensaje = $"Actualizados: {actualizados}\n Errores: {errores}";

            if (errores > 0)
            {
                MessageBox.Show(mensaje + "\n\nDetalle de errores:\n" + erroresDetalle.ToString(),
                                "Resultado de actualización",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(mensaje, "Actualización completada",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void CerrarSesionSilenciosa()
        {
            try
            {
                if (_client != null && _isConnected)
                {
                    await _client.PostAsync("Logout", null);
                }
            }
            catch
            {
                // Silencioso: no importa si falla, de todos modos vamos a reconectar
            }
            finally
            {
                _client?.DefaultRequestHeaders.Remove("Cookie");
                _isConnected = false;

                LimpiarDataGridView();

                // Restaurar botones
                button2.Enabled = false;   // Desconectar
                button3.Enabled = false;   // Buscar
                button4.Enabled = false;   // Actualizar
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Advertir si hay sesión activa
            if (_isConnected)
            {
                var confirm = MessageBox.Show(
                    "Hay una sesión activa con SAP. Si cambia la configuración, se cerrará la sesión.\n\n¿Desea continuar?",
                    "Cambiar configuración",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                // Cerrar sesión antes de cambiar config
                CerrarSesionSilenciosa();
            }

            using var formConfig = new FormConfiguracion();
            var resultado = formConfig.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                // Actualizar la config en memoria
                _config = formConfig.ConfiguracionGuardada;

                // Actualizar UI
                label1.Text = "Configuración actualizada. Listo para conectar.";
                label1.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185); // Azul

                MessageBox.Show(
                    "Configuración actualizada correctamente.\nPresione 'Conectar' para iniciar una nueva sesión.",
                    "Configuración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
