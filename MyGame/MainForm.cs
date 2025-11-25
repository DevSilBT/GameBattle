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
        private SpriteSheet sheet;

        private DateTime lastTick;
        private Timer gameTimer;

        public MainForm()
        {
            // -------------------------
            //   CONFIG VENTANA
            // -------------------------
            this.ClientSize = new Size(960, 640);
            this.DoubleBuffered = true;
            this.Text = "MyGame - OOP";
            this.KeyPreview = true;

            // -------------------------
            //   CARGA SPRITESHEET
            // -------------------------
            try
            {
                sheet = new SpriteSheet(Config.SpriteSheetPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el sprite: " + ex.Message);
                this.Close();
                return;
            }

            // -------------------------
            //   CREAR PLAYER
            // -------------------------
            var playerAnim = new Animator(sheet)
            {
                FrameTimeMs = 100
            };

            player = new Player(new PointF(120, 360), playerAnim);

            // -------------------------
            //   CREAR ENEMIGOS
            // -------------------------
            for (int i = 0; i < 3; i++)
            {
                var anim = new Animator(sheet)
                {
                    FrameTimeMs = 120
                };

                var enemy = new Enemy(
                    new PointF(500 + i * 80, 360 - i * 20),
                    anim,
                    player);

                enemies.Add(enemy);
            }

            // -------------------------
            //   LOOP DEL JUEGO
            // -------------------------
            lastTick = DateTime.Now;

            gameTimer = new Timer();
            gameTimer.Interval = 16; // 60 FPS aprox
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            // -------------------------
            //   INPUT DEL TECLADO
            // -------------------------
            this.KeyDown += (s, e) => player.KeyDown(e.KeyCode);
            this.KeyUp += (s, e) => player.KeyUp(e.KeyCode);
        }

        // ==========================================================
        //   LOOP PRINCIPAL
        // ==========================================================
        private void GameLoop(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var elapsed = (now - lastTick).TotalMilliseconds;
            lastTick = now;

            player.Update(elapsed);

            foreach (var en in enemies)
                en.Update(elapsed);

            // Ataque del jugador
            player.TryHit(enemies);

            // Eliminar enemigos muertos
            enemies.RemoveAll(x => !x.Alive);

            this.Invalidate(); // redibujar
        }

        // ==========================================================
        //   DIBUJO
        // ==========================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            // Fondo
            g.Clear(Color.CornflowerBlue);

            // Suelo
            g.FillRectangle(Brushes.DarkOliveGreen,
                new Rectangle(0, 420, ClientSize.Width, ClientSize.Height - 420));

            // Dibujar entidades
            player.Draw(g);
            foreach (var en in enemies)
                en.Draw(g);

            // HUD
            DrawHUD(g);
        }

        // ==========================================================
        //   HUD
        // ==========================================================
        private void DrawHUD(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 10, 10, 160, 22);
            g.FillRectangle(Brushes.Red, 12, 12, 156, 18);

            float hpPercent = Math.Max(0, player.HP) / 10f;  // para 10 HP máximo

            g.FillRectangle(Brushes.Green, 12, 12, (int)(156 * hpPercent), 18);

            g.DrawString($"HP: {player.HP}", this.Font, Brushes.White, 14, 12);
            g.DrawString($"Enemies: {enemies.Count}", this.Font, Brushes.White, 14, 36);
        }
    }
}
