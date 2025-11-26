//MainMenuForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO; // Necesario para Path.Combine y StreamWriter
using System.Windows.Forms;

namespace MyGame
{
    // Agregar 'partial' aquí
    public partial class MainMenuForm : Form
    {
        // Campo para la imagen del título - AHORA ACEPTA NULL
        private Bitmap? titleImage;

        // Lista de jugadores (para el registro) - DENTRO DE ESTE FORMULARIO
        private List<PlayerRecord> playerRecords = new List<PlayerRecord>();

        // Clase interna para almacenar registros de jugadores - DENTRO DE ESTE FORMULARIO
        private class PlayerRecord
        {
            public string Name { get; set; } = string.Empty; // Inicializar
            public int BestTime { get; set; }
            public PlayerRecord(string name, int bestTime)
            {
                Name = name;
                BestTime = bestTime;
            }
        }

        // Constructor
        public MainMenuForm()
        {
            // Llamar a InitializeComponent desde el archivo .Designer.cs
            InitializeComponent();
            LoadTitleImage();
            CreateMenuButtons();
        }

        // Método para cargar la imagen del título
        private void LoadTitleImage()
        {
            try
            {
                titleImage = Config.LoadImage("KNIGHTMARE TITLE.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando el título: {ex.Message}");
                titleImage = null; // Manejar el caso donde no se puede cargar la imagen
            }
        }

        // Método para crear los botones del menú
        private void CreateMenuButtons()
        {
            // Estilo común para los botones
            var buttonStyle = new Font("Arial", 12, FontStyle.Bold);
            var buttonSize = new Size(200, 50);

            // Botón Register
            var btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Font = buttonStyle;
            btnRegister.Size = buttonSize;
            btnRegister.Location = new Point((this.ClientSize.Width - buttonSize.Width) / 2, 200); // Ajusta Y según necesites
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Botón Play
            var btnPlay = new Button();
            btnPlay.Text = "Play";
            btnPlay.Font = buttonStyle;
            btnPlay.Size = buttonSize;
            btnPlay.Location = new Point((this.ClientSize.Width - buttonSize.Width) / 2, 270); // Ajusta Y según necesites
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);

            // Botón Options
            var btnOptions = new Button();
            btnOptions.Text = "Options";
            btnOptions.Font = buttonStyle;
            btnOptions.Size = buttonSize;
            btnOptions.Location = new Point((this.ClientSize.Width - buttonSize.Width) / 2, 340); // Ajusta Y según necesites
            btnOptions.Click += BtnOptions_Click;
            this.Controls.Add(btnOptions);

            // Botón Exit
            var btnExit = new Button();
            btnExit.Text = "Exit";
            btnExit.Font = buttonStyle;
            btnExit.Size = buttonSize;
            btnExit.Location = new Point((this.ClientSize.Width - buttonSize.Width) / 2, 410); // Ajusta Y según necesites
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);
        }

        // --- Manejador del Botón Register - AHORA ACEPTA NULL EN SENDER ---
        private void BtnRegister_Click(object? sender, EventArgs e) // <-- Cambiado object a object?
        {
            using (var registerForm = new RegisterForm())
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    string playerName = registerForm.PlayerName;
                    int bestTime = registerForm.BestTime;

                    // Agregar el registro a la lista LOCAL del menú
                    playerRecords.Add(new PlayerRecord(playerName, bestTime));

                    // Guardar los registros en CSV DESDE EL MENÚ
                    SavePlayerRecordsToCSV(); // <-- Aquí se llama al método correcto
                }
            }
        }

        // --- Manejador del Botón Play - AHORA ACEPTA NULL EN SENDER ---
        private void BtnPlay_Click(object? sender, EventArgs e) // <-- Cambiado object a object?
        {
            this.Hide();
            using (var gameForm = new MainForm())
            {
                gameForm.ShowDialog();
            }
            this.Show();
        }

        // --- Manejador del Botón Options - AHORA ACEPTA NULL EN SENDER ---
        private void BtnOptions_Click(object? sender, EventArgs e) // <-- Cambiado object a object?
        {
            using (var optionsForm = new OptionsForm())
            {
                optionsForm.ShowDialog(); // Puedes manejar el DialogResult aquí si es necesario
            }
        }

        // --- Manejador del Botón Exit - AHORA ACEPTA NULL EN SENDER ---
        private void BtnExit_Click(object? sender, EventArgs e) // <-- Cambiado object a object?
        {
            Environment.Exit(0);
        }

        // --- MÉTODO PARA GUARDAR EN CSV - AQUÍ ---
        private void SavePlayerRecordsToCSV()
        {
            try
            {
                // Definir la ruta del archivo CSV
                string filePath = Path.Combine(Config.AssetsPath, "PlayerRecords.csv");

                // Usar StreamWriter para escribir en el archivo
                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Escribir encabezados
                    writer.WriteLine("Nombre,Mejor Tiempo"); // Separador: coma

                    // Escribir los datos de cada jugador
                    foreach (var record in playerRecords)
                    {
                        // Escribir cada registro en una nueva línea
                        // Asegúrate de escapar comas en los nombres si es posible que las contengan
                        writer.WriteLine($"{record.Name},{record.BestTime}");
                    }
                }

                MessageBox.Show("Registros guardados correctamente en PlayerRecords.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los registros en CSV: {ex.Message}");
            }
        }

        // --- MÉTODO PARA DIBUJAR EL TÍTULO CON ANCHO AJUSTABLE ---
        private void DrawTitle(Graphics g)
        {
            if (titleImage != null)
            {
                // Definir el ancho deseado para el título (ajusta este valor según lo que necesites)
                int desiredWidth = 800; // Por ejemplo, 800 píxeles de ancho

                // Calcular la altura proporcional para mantener la relación de aspecto
                int desiredHeight = (int)((double)titleImage.Height / titleImage.Width * desiredWidth);

                // Crear una nueva imagen redimensionada
                Bitmap resizedTitle = new Bitmap(titleImage, new Size(desiredWidth, desiredHeight));

                // Calcular la posición X para centrar el título horizontalmente
                int titleX = (this.ClientSize.Width - desiredWidth) / 2;
                int titleY = -150; // Posición Y fija (puede ser ajustable también)

                // Dibujar la imagen redimensionada
                g.DrawImage(resizedTitle, titleX, titleY);

                // Liberar recursos de la imagen temporal
                resizedTitle.Dispose();
            }
        }

        // --- Dibujo del Menú ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Dibujar fondo negro
            g.Clear(Color.Black);

            // Dibujar título usando el nuevo método
            DrawTitle(g);

            // Dibujar texto de instrucciones
            using (var font = new Font("Arial", 10))
            using (var brush = new SolidBrush(Color.White))
            {
                string instructions = "Use las teclas de flecha para moverse. Presiona 'Z' para atacar.";
                SizeF textSize = g.MeasureString(instructions, font);
                int textX = (this.ClientSize.Width - (int)textSize.Width) / 2;
                int textY = this.ClientSize.Height - 50;
                g.DrawString(instructions, font, brush, textX, textY);
            }
        }
    }
}