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
            if (!Alive) return;

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

                animator.SetRow(1, Config.Columns);
            }
            else
            {
                animator.SetRow(0, Config.Columns);
            }

            if (keys.Contains(Keys.Space) && CanAttack())
            {
                attacking = true;
                ResetAttackTimer();
                animator.SetRow(2, Config.Columns);

                attackSource = new PointF(
                    Position.X + width / 2,
                    Position.Y + height / 2
                );
            }
            else
            {
                attacking = false;
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
                    enemy.TakeDamage(attackDamage);
                    return true;
                }
            }

            return false;
        }
    }
}
