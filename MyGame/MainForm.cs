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

        // Imagen de fondo (lawntown.png) - MANTENIDA COMO ESTÁ
        private Bitmap background;

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

            // --- CARGA DE IMÁGENES ---
            // Declarar las variables ANTES del try para que estén disponibles fuera de él
            CharacterSpriteSheet? playerSheet = null;
            CharacterSpriteSheet? enemySheet = null;

            try
            {
                string assetsPath = Config.AssetsPath;

                Console.WriteLine("Intentando cargar imágenes...");

                // Cargar el fondo (lawntown.png) - MANTENIDO COMO ESTÁ
                background = Config.LoadImage("lawntown.png");
                Console.WriteLine($"background cargada: {background?.Width}x{background?.Height}");

                // Cargar las spritesheets individuales para el jugador y el enemigo
                playerSheet = new CharacterSpriteSheet("Assets/player.png", 48, 32); // Tamaño de frame para jugador
                enemySheet = new CharacterSpriteSheet("Assets/enemy.png", 16, 16);   // Tamaño de frame para enemigo

                // Verificar si alguna imagen es nula
                if (background == null || playerSheet == null || enemySheet == null)
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

            // --- INICIALIZACIÓN DEL JUEGO ---
            // Jugador: Creamos un Animator con la spritesheet del jugador
            var playerAnim = new Animator(playerSheet!); // El ! indica al compilador que asuma que playerSheet no es nulo aquí, ya que si lo fuera, se habría lanzado una excepción en el try
            playerAnim.FrameTimeMs = 100;
            playerAnim.SetRow(0, 4); // Fila 0, 4 columnas (ajusta según tu spritesheet)
            player = new Player(new PointF(120, 400), playerAnim);

            // Enemigos: Creamos un Animator con la spritesheet del enemigo
            for (int i = 0; i < 3; i++)
            {
                var anim = new Animator(enemySheet!); // El ! indica al compilador que asuma que enemySheet no es nulo aquí
                anim.FrameTimeMs = 120;
                anim.SetRow(0, 4); // Fila 0, 4 columnas (ajusta según tu spritesheet)

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

        // ============================
        //          GAME LOOP - AHORA ACEPTA NULL EN SENDER
        // ============================
        private void GameLoop(object? sender, EventArgs e) // <-- Cambiado object a object?
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