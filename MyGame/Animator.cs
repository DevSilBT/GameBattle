///animator.cs

using System;
using System.Drawing;

namespace MyGame
{
    class Animator
    {
        private Bitmap currentImage; // La imagen actual que se va a dibujar
        private int currentFrame;
        private double timer;

        public int TotalFrames { get; private set; } = 1;
        public int Row { get; private set; } = 0;
        public double FrameTimeMs { get; set; } = 100;

        public Animator(Bitmap image)
        {
            this.currentImage = image ?? throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
            this.TotalFrames = 1;
        }

        public void SetRow(int row)
        {
            // Ignoramos el row, ya que solo tenemos una imagen
        }

        public void SetRow(int row, int totalFrames)
        {
            // Ignoramos el row y totalFrames, ya que solo tenemos una imagen
        }

        public void Update(double elapsedMs)
        {
            // Para una sola imagen, no hacemos nada en Update
        }

        public Bitmap CurrentFrame()
        {
            return currentImage;
        }
    }
}