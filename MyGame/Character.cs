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
        // Nueva propiedad para verificar si el personaje acaba de morir
        public bool IsAlive => HP > 0;

        protected Animator animator;

        // El tamaño del personaje se calcula basado en Config y Scale
        protected int width = (int)(Config.FrameW * Config.Scale);
        protected int height = (int)(Config.FrameH * Config.Scale);

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
            // Ajusta la fila según tu spritesheet. Suponiendo fila 4 para "herido"
            animator.SetRow(4, Config.Columns);
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

        // ------------------------------
        //      DIBUJAR PERSONAJE
        // ------------------------------
        public virtual void Draw(Graphics g)
        {
            if (!Alive) return;

            var bmp = animator.CurrentFrame();
            if (bmp != null)
            {
                // Redimensionar la imagen al tamaño definido en Config
                int drawWidth = (int)(Config.FrameW * Config.Scale);
                int drawHeight = (int)(Config.FrameH * Config.Scale);

                // Crear una copia invertida horizontalmente si es necesario
                Bitmap drawnBmp = bmp;

                // Verificar si es un jugador y si necesita invertirse
                if (this is Player player)
                {
                    if (player.CurrentDirection == 1) // Izquierda
                    {
                        drawnBmp = new Bitmap(bmp.Width, bmp.Height);
                        using (Graphics gr = Graphics.FromImage(drawnBmp))
                        {
                            // Crear una matriz de transformación
                            System.Drawing.Drawing2D.Matrix transformMatrix = new System.Drawing.Drawing2D.Matrix();
                            // Escalar horizontalmente por -1 (inversión)
                            transformMatrix.Scale(-1, 1);
                            // Trasladar para compensar la inversión
                            transformMatrix.Translate(bmp.Width, 0);
                            // Aplicar la transformación al contexto gráfico
                            gr.Transform = transformMatrix;

                            // Dibujar el sprite original (que ahora se invertirá y trasladará)
                            gr.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);

                            // Liberar la matriz
                            transformMatrix.Dispose();
                        }
                    }
                }
                // Si necesitas que otros personajes (como enemigos) también se inviertan,
                // puedes agregar una propiedad similar en la clase Enemy y verificarla aquí.

                g.DrawImage(
                    drawnBmp,
                    new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        drawWidth,
                        drawHeight
                    )
                );

                // Liberar recursos de la imagen invertida si fue creada
                if (drawnBmp != bmp)
                {
                    drawnBmp.Dispose();
                }
            }
            else
            {
                // Dibujar un rectángulo rojo como fallback si la imagen es nula
                g.FillRectangle(Brushes.Red, (int)Position.X, (int)Position.Y, 50, 50);
            }
        }

        protected bool CanAttack() => attackTimer >= attackCooldownMs;
        protected void ResetAttackTimer() => attackTimer = 0;

        // ------------------------------
        //      MÉTODO TOMAR DAÑO USADO POR Player.TryHit
        // ------------------------------
        // Modificado para devolver true si el personaje muere
        public virtual bool TakeDamage(int amount)
        {
            HP -= amount;

            if (HP <= 0)
            {
                // NO asignamos Alive porque es una propiedad de solo lectura
                // Ajusta la fila según tu spritesheet. Suponiendo fila 3 para "muerte"
                animator.SetRow(3, Config.Columns);
                return true; // Indica que el personaje murió
            }
            else
            {
                // Ajusta la fila según tu spritesheet. Suponiendo fila 4 para "herido"
                animator.SetRow(4, Config.Columns);
                return false; // Indica que el personaje no murió
            }
        }
    }
}