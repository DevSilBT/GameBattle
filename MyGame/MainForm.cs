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

        // --- NUEVAS VARIABLES ---
        private int enemiesKilled = 0;
        private bool gameOver = false;

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
            var playerAnim = new Animator(playerSheet);
            playerAnim.FrameTimeMs = 100;
            playerAnim.SetRow(0, 4); // Fila 0, 4 columnas (ajusta según tu spritesheet)
            player = new Player(new PointF(120, 400), playerAnim);

            // Inicializar la primera oleada de enemigos
            SpawnEnemies();

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

        // --- MÉTODO PARA GENERAR ENEMIGOS ---
        private void SpawnEnemies()
        {
            // Calcular cuántos enemigos generar (1 a 3)
            Random random = new Random();
            int numEnemiesToSpawn = random.Next(1, 4); // Genera 1, 2 o 3

            // Cargar las spritesheets individuales para el jugador y el enemigo
            CharacterSpriteSheet? enemySheet = null;
            try
            {
                enemySheet = new CharacterSpriteSheet("Assets/enemy.png", 16, 16);   // Tamaño de frame para enemigo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar spritesheet del enemigo para spawn:\n{ex.Message}");
                return; // Salir si no se puede cargar la spritesheet
            }

            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                var anim = new Animator(enemySheet!); // El ! indica al compilador que asuma que enemySheet no es nulo aquí
                anim.FrameTimeMs = 120;
                anim.SetRow(0, 4); // Fila 0, 4 columnas (ajusta según tu spritesheet)

                // Posicionar enemigos aleatoriamente en la parte superior derecha (ajusta según necesites)
                var en = new Enemy(
                    new PointF(520 + i * 80, 50 - i * 20), // Cambia la posición inicial
                    anim,
                    player
                );

                enemies.Add(en);
            }

            Console.WriteLine($"Generados {numEnemiesToSpawn} enemigos. Total enemigos: {enemies.Count}");
        }

        // ============================
        //          GAME LOOP - AHORA ACEPTA NULL EN SENDER
        // ============================
        private void GameLoop(object? sender, EventArgs e) // <-- Cambiado object a object?
        {
            if (gameOver) return; // Si el juego terminó, no actualizamos la lógica

            var now = DateTime.Now;
            var elapsed = (now - lastTick).TotalMilliseconds;
            lastTick = now;

            // Update player
            player.Update(elapsed);

            // Verificar si el jugador murió
            if (player.IsGameOver)
            {
                gameOver = true; // Activar la bandera de Game Over
                Console.WriteLine("Game Over!");
                // No necesitas hacer nada más aquí, el OnPaint se encargará de dibujar la pantalla de Game Over
                this.Invalidate(); // Forzar un repaint
                return; // Salir del loop si el juego terminó
            }

            // Update enemies
            foreach (var en in enemies)
                en.Update(elapsed);

            // Player attacking logic
            player.TryHit(enemies);

            // Contar enemigos matados y removerlos
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (!enemies[i].Alive)
                {
                    enemiesKilled++; // Incrementar el contador
                    Console.WriteLine($"Enemigo matado. Total matados: {enemiesKilled}");
                    enemies.RemoveAt(i);
                }
            }

            // Verificar si todos los enemigos están muertos y generar nuevos
            if (enemies.Count == 0)
            {
                Console.WriteLine("Todos los enemigos eliminados. Generando nuevos...");
                SpawnEnemies(); // Generar una nueva oleada
            }

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

            if (gameOver)
            {
                // Dibujar la pantalla de Game Over
                DrawGameOverScreen(g);
            }
            else
            {
                // Dibujar fondo (lawntown.png)
                DrawBackground(g);

                // Draw characters
                player.Draw(g);
                foreach (var en in enemies)
                    en.Draw(g);

                DrawHUD(g);
            }
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
            // Mostrar el contador de enemigos matados
            g.DrawString($"Matados: {enemiesKilled}", this.Font, Brushes.White, 14, 60); // Nueva línea
        }

        // --- MÉTODO PARA DIBUJAR LA PANTALLA DE GAME OVER ---
        private void DrawGameOverScreen(Graphics g)
        {
            // Fondo oscuro o semi-transparente
            using (var brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0))) // Negro semi-transparente
            {
                g.FillRectangle(brush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }

            // Texto de Game Over
            using (var font = new Font("Arial", 48, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                string gameOverText = "GAME OVER";
                SizeF textSize = g.MeasureString(gameOverText, font);
                int textX = (this.ClientSize.Width - (int)textSize.Width) / 2;
                int textY = (this.ClientSize.Height - (int)textSize.Height) / 2 - 50; // Ajusta la posición vertical
                g.DrawString(gameOverText, font, brush, textX, textY);
            }

            // Texto de puntuación final
            using (var font = new Font("Arial", 24))
            using (var brush = new SolidBrush(Color.White))
            {
                string scoreText = $"Enemigos Matados: {enemiesKilled}";
                SizeF textSize = g.MeasureString(scoreText, font);
                int textX = (this.ClientSize.Width - (int)textSize.Width) / 2;
                int textY = (this.ClientSize.Height - (int)textSize.Height) / 2 + 20; // Ajusta la posición vertical
                g.DrawString(scoreText, font, brush, textX, textY);
            }

            // Texto de instrucciones (opcional)
            using (var font = new Font("Arial", 16))
            using (var brush = new SolidBrush(Color.White))
            {
                string instructions = "Presiona ESC para salir.";
                SizeF textSize = g.MeasureString(instructions, font);
                int textX = (this.ClientSize.Width - (int)textSize.Width) / 2;
                int textY = (this.ClientSize.Height - (int)textSize.Height) / 2 + 80; // Ajusta la posición vertical
                g.DrawString(instructions, font, brush, textX, textY);
            }
        }
    }
}