
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

        private SpriteSheet sheet;
        private DateTime lastTick;

        private System.Windows.Forms.Timer gameTimer;

        private Bitmap groundTexture;

        // ============================
        //         CONSTRUCTOR
        // ============================
        public MainForm()
        {
            this.ClientSize = new Size(960, 640);
            this.DoubleBuffered = true;
            this.Text = "MyGame - Battle System";
            this.KeyPreview = true;

            // Cargar spritesheet
            try
            {
                sheet = new SpriteSheet("Assets/sheet.png"); // AJUSTADO A TU REPO
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando spritesheet: " + ex.Message);
                Environment.Exit(1);
            }

            // Textura del piso
            groundTexture = new Bitmap(1, 1);
            groundTexture.SetPixel(0, 0, Color.DarkOliveGreen);

            // Jugador
            var playerAnim = new Animator(sheet);
            playerAnim.FrameTimeMs = 100;
            player = new Player(new PointF(120, 400), playerAnim);

            // Enemigos
            for (int i = 0; i < 3; i++)
            {
                var anim = new Animator(sheet);
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

            DrawScene(g);

            // Draw characters
            player.Draw(g);
            foreach (var en in enemies)
                en.Draw(g);

            DrawHUD(g);
        }

        private void DrawScene(Graphics g)
        {
            g.Clear(Color.SkyBlue);

            // Piso
            g.FillRectangle(
                new SolidBrush(Color.DarkOliveGreen),
                0,
                450,
                this.ClientSize.Width,
                this.ClientSize.Height - 450
            );
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
