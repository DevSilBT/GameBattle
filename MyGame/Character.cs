// ------------------ Character.cs ------------------
using System;
using System.Drawing;

namespace MyGame
{
    abstract class Character : IEntity
    {
        public PointF Position;
        public float Speed;
        public int HP;

        // Alive es solo lectura
        public bool Alive => HP > 0;

        protected Animator animator;
        protected int width = Config.FrameW * Config.Scale;
        protected int height = Config.FrameH * Config.Scale;

        // Combat
        protected double attackCooldownMs = 600;
        protected double attackTimer = 0;
        protected int attackDamage = 1;
        protected int knockback = 12;

        public Character(PointF pos, Animator animator)
        {
            this.Position = pos;
            this.animator = animator;
        }

        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, width, height);

        // ------------------------------
        //      RECIBIR DAÑO (con knockback)
        // ------------------------------
        public virtual void ReceiveDamage(int dmg, PointF source)
        {
            HP -= dmg;

            float dirX = Position.X - source.X;
            if (dirX == 0) dirX = 1;

            float sign = Math.Sign(dirX);

            Position = new PointF(
                Position.X + sign * knockback,
                Position.Y
            );

            OnHurt();
        }

        protected virtual void OnHurt()
        {
            animator.SetRow(4, Config.Columns); // animación de ser golpeado
        }

        // ------------------------------
        //      LÓGICA BASE
        // ------------------------------
        public virtual void Update(double elapsedMs)
        {
            if (!Alive) return;

            attackTimer += elapsedMs;
            animator.Update(elapsedMs);
        }

        public virtual void Draw(Graphics g)
        {
            if (!Alive) return;

            using (var bmp = animator.CurrentFrame())
            {
                g.DrawImage(
                    bmp,
                    new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        width,
                        height
                    )
                );
            }
        }

        protected bool CanAttack() => attackTimer >= attackCooldownMs;
        protected void ResetAttackTimer() => attackTimer = 0;

        // ------------------------------
        //      MÉTODO TOMAR DAÑO USADO POR Player.TryHit
        // ------------------------------
        public virtual void TakeDamage(int amount)
        {
            HP -= amount;

            if (HP <= 0)
            {
                // NO asignamos Alive porque es una propiedad de solo lectura
                animator.SetRow(4, Config.Columns); // animación de muerte
            }
            else
            {
                animator.SetRow(4, Config.Columns); // animación de golpe
            }
        }
    }
}
