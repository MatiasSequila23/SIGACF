using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace UI
{
    public partial class Login : Form
    {
        private int intentosFallidos = 0;
        private const int MAX_INTENTOS = 3; // Número máximo de intentos permitido
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            if (txtUser.Text == "Usuario") 
            {
                txtUser.Text = "";
                txtUser.ForeColor = Color.DarkGreen;
            }
        }

        private void txtUser_Leave(object sender, EventArgs e)
        {
            if (txtUser.Text == "") 
            {
                txtUser.Text = "Usuario";
                txtUser.ForeColor = Color.SeaGreen;
            }

        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "Contraseña")
            {
                txtPass.Text = "";
                txtPass.ForeColor = Color.DarkGreen;
                txtPass.UseSystemPasswordChar = true;
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                txtPass.Text = "Contraseña";
                txtPass.ForeColor = Color.SeaGreen;
                txtPass.UseSystemPasswordChar = false;
            }
        }

        private void btnToAccess_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtUser.Text;
                string password = txtPass.Text;

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Ingrese usuario y contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar si la contraseña cumple con los requisitos
                if (!Seguridad.ValidarContraseña(password))
                {
                    MessageBox.Show("La contraseña debe tener al menos 12 caracteres, una mayúscula, un número y un carácter especial.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Hashear la contraseña ingresada
                string passwordHash = Seguridad.HashPassword(password);

                // Verificar si el usuario está bloqueado antes de validar
                if (UsuarioBloqueado(usuario))
                {
                    MessageBox.Show("Su cuenta está bloqueada por múltiples intentos fallidos. Intente más tarde.", "Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar usuario en la base de datos
                if (ValidarUsuario(usuario, passwordHash))
                {
                    MessageBox.Show("Acceso concedido.", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    barraTitulo frm = new barraTitulo();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    intentosFallidos++;

                    if (intentosFallidos >= MAX_INTENTOS)
                    {
                        BloquearUsuario(usuario);
                        MessageBox.Show("Se ha bloqueado su cuenta por múltiples intentos fallidos.", "Cuenta Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnToAccess.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show($"Usuario o contraseña incorrectos. Intentos restantes: {MAX_INTENTOS - intentosFallidos}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrarLogin_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximizarLogin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaximizarLogin.Visible = false;
            btnRestaurarLogin.Visible = true;
        }

        private void btnRestaurarLogin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnMaximizarLogin.Visible = true;
            btnRestaurarLogin.Visible = false;
        }

        private void btnMinimizarLogin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]

        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);


        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private bool ValidarUsuario(string usuario, string passwordHash)
        {
            string connectionString = "Server=tu_servidor;Database=tu_base_de_datos;User Id=tu_usuario;Password=tu_contraseña;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @usuario AND PasswordHash = @passwordHash AND Bloqueado = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private bool UsuarioBloqueado(string usuario)
        {
            string connectionString = "Server=tu_servidor;Database=tu_base_de_datos;User Id=tu_usuario;Password=tu_contraseña;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Bloqueado FROM Usuarios WHERE NombreUsuario = @usuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null && Convert.ToBoolean(resultado);
                }
            }
        }

        private void BloquearUsuario(string usuario)
        {
            string connectionString = "Server=tu_servidor;Database=tu_base_de_datos;User Id=tu_usuario;Password=tu_contraseña;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Usuarios SET Bloqueado = 1 WHERE NombreUsuario = @usuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
    public class Seguridad
    {
        // Método para hashear una contraseña con SHA256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convierte a hexadecimal
                }
                return builder.ToString();
            }
        }

        // Método para validar la seguridad de la contraseña
        public static bool ValidarContraseña(string password)
        {
            if (password.Length < 12) return false;  // Mínimo 12 caracteres
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;  // Al menos una mayúscula
            if (!Regex.IsMatch(password, @"[a-z]")) return false;  // Al menos una minúscula
            if (!Regex.IsMatch(password, @"[0-9]")) return false;  // Al menos un número
            if (!Regex.IsMatch(password, @"[\W_]")) return false;  // Al menos un carácter especial

            return true;
        }
    }
}
