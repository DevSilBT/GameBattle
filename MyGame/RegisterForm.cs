// RegisterForm.cs
using System;
using System.Windows.Forms;

namespace MyGame
{
    // Agregar 'partial' aquí
    public partial class RegisterForm : Form
    {
        // Propiedades para devolver los datos
        public string PlayerName { get; private set; } = string.Empty; // Inicializar
        public int BestTime { get; private set; }

        // Constructor
        public RegisterForm()
        {
            // Llamar a InitializeComponent desde el archivo .Designer.cs
            InitializeComponent();
        }

        // Manejador del botón OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Obtener el nombre del TextBox txtName (que estará en .Designer.cs)
            PlayerName = txtName.Text; // <-- Aquí se usa txtName

            // Intentar convertir el tiempo del TextBox txtBestTime (que estará en .Designer.cs)
            if (int.TryParse(txtBestTime.Text, out int time)) // <-- Aquí se usa txtBestTime
            {
                BestTime = time;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un número válido para el mejor tiempo.");
            }
        }

        // Manejador del botón Cancelar
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}