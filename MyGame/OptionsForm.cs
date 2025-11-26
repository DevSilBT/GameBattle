// OptionsForm.cs
using System;
using System.Windows.Forms;

namespace MyGame
{
    // Agregar 'partial' aquí
    public partial class OptionsForm : Form
    {
        // Propiedades para devolver los valores ajustados
        public int Brightness { get; private set; }
        public int Volume { get; private set; }

        // Constructor
        public OptionsForm()
        {
            // Llamar a InitializeComponent desde el archivo .Designer.cs
            InitializeComponent();
            // Inicializar los TrackBars con valores predeterminados o cargados
            // Por ejemplo:
            trackBrightness.Value = 50; // Valor inicial
            trackVolume.Value = 75;     // Valor inicial
        }

        // Manejador del botón OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TrackBars (que estarán en .Designer.cs)
            Brightness = trackBrightness.Value; // <-- Aquí se usa trackBrightness
            Volume = trackVolume.Value;         // <-- Aquí se usa trackVolume
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Manejador del botón Cancelar
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}