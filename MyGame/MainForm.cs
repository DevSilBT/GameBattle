
//Mainform.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyGame
{
    public class MainForm : Form
    {
        private Player player;
        private List<Enemy> enemies = new List<Enemy>();

        // Cambiamos SpriteSheet por Bitmaps individuales
        private Bitmap playerImage; // Imagen del jugador
        private Bitmap enemyImage;  // Imagen del enemigo
        private Bitmap background;  // Imagen de fondo (lawntown.png)

        private DateTime lastTick;

        private System.Windows.Forms.Timer gameTimer;

        // ============================
        //         CONSTRUCTOR
        // ============================
        public MainForm()
        {
            this.ClientSize = new Size(960, 640);
            this.DoubleBuffered = true;
            this.Text = "MyGame - Battle System";
            this.KeyPreview = true;

// En MainForm.cs, dentro del constructor, modifica el bloque try-catch:
try
{
    string assetsPath = Config.AssetsPath;

    // Cargar imágenes con mensajes de depuración
    Console.WriteLine("Intentando cargar imágenes...");

    playerImage = Config.LoadImage("player.png");
    Console.WriteLine($"playerImage cargada: {playerImage?.Width}x{playerImage?.Height}");

    enemyImage = Config.LoadImage("enemy.png");
    Console.WriteLine($"enemyImage cargada: {enemyImage?.Width}x{enemyImage?.Height}");

    background = Config.LoadImage("lawntown.png");
    Console.WriteLine($"background cargada: {background?.Width}x{background?.Height}");

    // Verificar si alguna imagen es nula
    if (playerImage == null || enemyImage == null || background == null)
    {
        throw new Exception("Una o más imágenes son nulas.");
    }

    Console.WriteLine("¡Imágenes cargadas correctamente!");
}
catch (Exception ex)
{
    MessageBox.Show($"Error crítico al cargar imágenes:\n{ex.Message}\n\nVerifique que los archivos 'player.png', 'enemy.png' y 'lawntown.png' existan en la carpeta 'Assets'.", "Error de Carga");
    Environment.Exit(1);
}



            // Jugador: Creamos un Animator pero lo inicializamos con una sola imagen
            var playerAnim = new Animator(playerImage); // Modificado: pasamos la imagen directamente
            playerAnim.FrameTimeMs = 100;
            player = new Player(new PointF(120, 400), playerAnim);

            // Enemigos: Creamos un Animator con la imagen del enemigo
            for (int i = 0; i < 3; i++)
            {
                var anim = new Animator(enemyImage); // Modificado: pasamos la imagen directamente
                anim.FrameTimeMs = 120;

                var en = new Enemy(
                    new PointF(520 + i * 80, 400 - i * 20),
                    anim,
                    player
                );

                enemies.Add(en);
            }

            lastTick = DateTime.Now;

            // Timer 60 FPS
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            // Controles
            this.KeyDown += (s, e) => player.KeyDown(e.KeyCode);
            this.KeyUp += (s, e) => player.KeyUp(e.KeyCode);
        }

// En Config.cs, reemplaza el método LoadImage existente:
public static Bitmap LoadImage(string fileName)
{
    // Construir la ruta absoluta usando Application.StartupPath
    string fullPath = Path.Combine(Application.StartupPath, "Assets", fileName);

    // Verificar si el archivo existe
    if (!File.Exists(fullPath))
    {
        // Si no existe en Application.StartupPath, intentar con una ruta relativa al proyecto
        string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets"));
        fullPath = Path.Combine(projectPath, fileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"No se encontró el archivo {fileName} en ninguna de las rutas probadas.\nRuta 1: {Path.Combine(Application.StartupPath, "Assets")}\nRuta 2: {projectPath}");
        }
    }

    try
    {
        using (var temp = new Bitmap(fullPath))
        {
            return new Bitmap(temp);
        }
    }
    catch (Exception ex)
    {
        throw new Exception($"Error al cargar la imagen {fileName}: {ex.Message}", ex);
    }
}



        // ============================
        //          GAME LOOP
        // ============================
        private void GameLoop(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var elapsed = (now - lastTick).TotalMilliseconds;
            lastTick = now;

            // Update player
            player.Update(elapsed);

            // Update enemies
            foreach (var en in enemies)
                en.Update(elapsed);

            // Player attacking logic
            player.TryHit(enemies);

            // Remove dead enemies
            enemies.RemoveAll(e => !e.Alive);

            // Redraw screen
            this.Invalidate();
        }

        // ============================
        //            RENDER
        // ============================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Dibujar fondo (lawntown.png)
            DrawBackground(g);

            // Draw characters
            player.Draw(g);
            foreach (var en in enemies)
                en.Draw(g);

            DrawHUD(g);
        }

        private void DrawBackground(Graphics g)
        {
            // Dibujar la imagen de fondo completa
            g.DrawImage(background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }

        private void DrawHUD(Graphics g)
        {
            // Barra de vida
            g.FillRectangle(Brushes.Black, 10, 10, 160, 22);
            g.FillRectangle(Brushes.Red, 12, 12, 156, 18);

            float hpPercent = Math.Max(0, player.HP) / 10f;
            g.FillRectangle(Brushes.Green, 12, 12, (int)(156 * hpPercent), 18);

            g.DrawString($"HP: {player.HP}", this.Font, Brushes.White, 14, 12);
            g.DrawString($"Enemigos: {enemies.Count}", this.Font, Brushes.White, 14, 36);
        }
    }
}