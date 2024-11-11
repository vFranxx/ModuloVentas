using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ABM_Productos.MODULO_IMPRESION;

namespace ABM_Productos
{
    public partial class MODULO_VENTAS : Form
    {
        private static string SQLString;

        // Popup de confirmación de una acción
        public static bool MostrarMensajeConfirmacion(string mensaje)
        {
            DialogResult resultado = MessageBox.Show(mensaje, "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return (resultado == DialogResult.Yes);
        }

        // Clase donde se almacenaran temporalmente los datos de cliente
        public class Cliente
        {
            public int CUIT { get; set; }
            public string RS { get; set; }
            public string TIPO_CLIENTE { get; set; }
        }

        // Clase donde se almacenaran temporalmente los datos de producto
        public class Producto
        {
            public int CODIGO { get; set; }
            public string DESCRIPCION { get; set; }
            public int STOCK { get; set; }
            public decimal PRECIO_UNITARIO { get; set; }
            public decimal PRECIO_MAYORISTA { get; set; }
            public decimal PRECIO_DISTRIBUIDOR { get; set; }
            public DateTime FECHA_BAJA { get; set; }
            public string RUBRO { get; set; }
            public string UND_MEDIDA { get; set; }
        }

        public MODULO_VENTAS()
        {
            InitializeComponent();
            SQLString = ConfigurationManager.ConnectionStrings["StringSQL"].ConnectionString;
            CargarProductosEnLaGrilla(dataGridView1);
        }

        // Función para cargar el listado de los productos en la grilla principal (izquierda)
        private void CargarProductosEnLaGrilla(DataGridView dg)
        {
            try
            {
                string query = "SELECT * FROM dbo.PRODUCTOS";

                using (var LocalCNX = new SqlConnection(SQLString))
                {
                    LocalCNX.Open();

                    using (var LocalCMD = new SqlCommand(query, LocalCNX))
                    {
                        var LocalDA = new SqlDataAdapter(LocalCMD);
                        var LocalDT = new DataTable();
                        LocalDA.Fill(LocalDT);
                        dg.DataSource = LocalDT;
                    }

                    LocalCNX.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos en la grilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Función para obtener un producto y añadirlo al ticket
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // Obtengo el codigo del producto
                    string codigo = row.Cells["CODIGO"].Value.ToString();

                    // Recupero sus datos
                    Producto producto = ObtenerProducto(codigo);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el ticket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Funcion para utilidad de busqueda (Actualización de grilla cuando se escribe el codigo a modo parcial y completo)
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string inputCode = textBox1.Text;

            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    label2.Text = "Buscando...";
                }
                else
                {
                    label2.Text = "Busqueda por código";
                }

                string query = "SELECT * FROM dbo.PRODUCTOS WHERE CAST(CODIGO AS VARCHAR) LIKE @inputCode";

                using (var LocalCNX = new SqlConnection(SQLString))
                {
                    LocalCNX.Open();

                    using (var LocalCMD = new SqlCommand(query, LocalCNX))
                    {
                        // Añadir el parámetro con el carácter comodín
                        LocalCMD.Parameters.AddWithValue("@inputCode", "%" + inputCode + "%");
                        var LocalDA = new SqlDataAdapter(LocalCMD);
                        var LocalDT = new DataTable();
                        LocalDA.Fill(LocalDT);
                        dataGridView1.DataSource = LocalDT;
                    }

                    LocalCNX.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Funcion que devuelve una clase con los datos del producto
        private Producto ObtenerProducto(string codigo)
        {
            Producto producto = null;

            try
            {
                using (var LocalCNX = new SqlConnection(SQLString))
                {
                    LocalCNX.Open();

                    using (var LocalCMD = new SqlCommand("SELECT * FROM dbo.PRODUCTOS WHERE CODIGO = @CODIGO", LocalCNX))
                    {
                        LocalCMD.Parameters.AddWithValue("@CODIGO", codigo);

                        using (var reader = LocalCMD.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                producto = new Producto
                                {
                                    CODIGO = reader.GetInt32(reader.GetOrdinal("CODIGO")),
                                    DESCRIPCION = reader.GetString(reader.GetOrdinal("DESCRIPCION")),
                                    STOCK = reader.GetInt32(reader.GetOrdinal("STOCK")),
                                    PRECIO_UNITARIO = reader.GetDecimal(reader.GetOrdinal("PRECIO_UNITARIO")),
                                    PRECIO_MAYORISTA = reader.GetDecimal(reader.GetOrdinal("PRECIO_MAYORISTA")),
                                    PRECIO_DISTRIBUIDOR = reader.GetDecimal(reader.GetOrdinal("PRECIO_DISTRIBUIDOR")),
                                    FECHA_BAJA = reader.GetDateTime(reader.GetOrdinal("FECHA_BAJA")),
                                    RUBRO = reader.GetString(reader.GetOrdinal("RUBRO")),
                                    UND_MEDIDA = reader.GetString(reader.GetOrdinal("UND_MEDIDA"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return producto;
        }

        // Botón donde se toman los datos y se los inserta como detalles en una cabecera
        private void button1_Click(object sender, EventArgs e)
        {
            if (Funciones.MostrarMensajeConfirmacion("¿Desea generar finalizar la compra?"))
            {
                // Codigo acá
            }
        }
    }
}
