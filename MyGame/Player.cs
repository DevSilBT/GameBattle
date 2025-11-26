// ------------------ Player.cs ------------------
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyGame
{
    class Player : Character
    {
        private HashSet<Keys> keys = new HashSet<Keys>();
        private bool attacking = false;
        private PointF attackSource;
        private int currentDirection = 0; // 0 = derecha, 1 = izquierda

        // Propiedad pública para acceder a la dirección desde Character
        public int CurrentDirection => currentDirection;

        // Nueva propiedad para indicar si el juego terminó
        public bool IsGameOver => !Alive;

        public Player(PointF pos, Animator animator) : base(pos, animator)
        {
            Speed = 4.5f;
            HP = 10;
            attackCooldownMs = 300;
            attackDamage = 1;
        }

        public void KeyDown(Keys k) => keys.Add(k);
        public void KeyUp(Keys k) => keys.Remove(k);

        public override void Update(double elapsedMs)
        {
            if (!Alive) return; // Si el jugador murió, no actualizamos su lógica

            var dx = 0f;
            var dy = 0f;

            if (keys.Contains(Keys.Left) || keys.Contains(Keys.A)) dx -= 1;
            if (keys.Contains(Keys.Right) || keys.Contains(Keys.D)) dx += 1;
            if (keys.Contains(Keys.Up) || keys.Contains(Keys.W)) dy -= 1;
            if (keys.Contains(Keys.Down) || keys.Contains(Keys.S)) dy += 1;

            if (dx != 0 || dy != 0)
            {
                var len = (float)System.Math.Sqrt(dx * dx + dy * dy);
                dx /= len;
                dy /= len;

                Position = new PointF(Position.X + dx * Speed,
                                      Position.Y + dy * Speed);

                // Determinar la dirección
                if (dx < 0)
                {
                    currentDirection = 1; // Izquierda
                    animator.SetRow(5, Config.Columns); // Fila 5 para caminar hacia la izquierda
                }
                else
                {
                    currentDirection = 0; // Derecha
                    animator.SetRow(1, Config.Columns); // Fila 1 para caminar hacia la derecha
                }
            }
            else
            {
                // Fila 0 para animación de parado (ajusta según tu spritesheet)
                animator.SetRow(0, Config.Columns);
            }

            if (keys.Contains(Keys.Space) && CanAttack()) // Asumiendo que 'Z' es ataque, cambia a 'Space' o la tecla que uses
            {
                attacking = true;
                ResetAttackTimer();
                // Fila 2 para animación de ataque (ajusta según tu spritesheet)
                animator.SetRow(2, Config.Columns);

                attackSource = new PointF(
                    Position.X + width / 2,
                    Position.Y + height / 2
                );
            }
            else
            {
                // Si no está atacando, mantener la animación de parado o caminar
                // Esto se maneja arriba con SetRow(0) o SetRow(1)
            }

            base.Update(elapsedMs);
        }

        // --------------------------------------------------
        //               MÉTODO DE ATAQUE TryHit()
        // --------------------------------------------------
        public bool TryHit(IList<Enemy> enemies)
        {
            if (!attacking)
                return false;

            float attackRange = 40f;

            foreach (var enemy in enemies)
            {
                if (!enemy.Alive)
                    continue;

                float dx = enemy.Position.X - attackSource.X;
                float dy = enemy.Position.Y - attackSource.Y;

                float dist = (float)System.Math.Sqrt(dx * dx + dy * dy);

                if (dist <= attackRange)
                {
                    // Usamos TakeDamage en lugar de ReceiveDamage
                    bool enemyDied = enemy.TakeDamage(attackDamage);
                    if (enemyDied)
                    {
                        // Aquí puedes incrementar el contador de enemigos matados si lo manejas en MainForm
                        // Por ahora, simplemente devolvemos true si matamos al menos uno
                        return true;
                    }
                }
            }

            return false;
        }
    }
}