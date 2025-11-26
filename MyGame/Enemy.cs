//------Enemy----
using System;
using System.Drawing;

namespace MyGame
{
    class Enemy : Character
    {
        private Character target;
        private float attackRange = Config.FrameW * 1.2f;
        private double aiTimer = 0;

        public Enemy(PointF pos, Animator animator, Character target)
            : base(pos, animator)
        {
            this.target = target;
            Speed = 2.2f;
            HP = 3;
            attackCooldownMs = 1200;
            attackDamage = 1;
        }

        public override void Update(double elapsedMs)
        {
            if (!Alive) return;

            aiTimer += elapsedMs;

            float dx = target.Position.X - Position.X;
            float dy = target.Position.Y - Position.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Movimiento hacia el jugador
            if (dist > attackRange)
            {
                float nx = dx / dist;
                float ny = dy / dist;

                Position = new PointF(
                    Position.X + nx * Speed,
                    Position.Y + ny * Speed
                );

                animator.SetRow(1, Config.Columns);
            }
            else
            {
                // Ataque
                if (CanAttack())
                {
                    ResetAttackTimer();
                    animator.SetRow(3, Config.Columns);

                    target.ReceiveDamage(
                        attackDamage,
                        new PointF(
                            Position.X + width / 2,
                            Position.Y + height / 2
                        )
                    );
                }
            }

            base.Update(elapsedMs);
        }
    }
}
