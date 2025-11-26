// Animator.cs (Modificado)
using System;
using System.Drawing;

namespace MyGame
{
    class Animator
    {
        private CharacterSpriteSheet sheet; // Cambiado de SpriteSheet a CharacterSpriteSheet
        private int currentFrame;
        private double timer;

        public int TotalFrames { get; private set; } = Config.Columns;
        public int Row { get; private set; } = 0;
        public double FrameTimeMs { get; set; } = 100;

        public Animator(CharacterSpriteSheet sheet) // Cambiado el parámetro
        {
            this.sheet = sheet;
        }

        // --- MÉTODO SIMPLE ---
        public void SetRow(int row)
        {
            SetRow(row, Config.Columns);
        }

        // --- MÉTODO COMPLETO ---
        public void SetRow(int row, int totalFrames)
        {
            if (totalFrames <= 0)
                totalFrames = Config.Columns;

            if (row != Row)
            {
                Row = row;
                TotalFrames = totalFrames;
                currentFrame = 0;
                timer = 0;
            }
        }

        public void Update(double elapsedMs)
        {
            timer += elapsedMs;
            if (timer >= FrameTimeMs)
            {
                timer = 0;
                currentFrame++;
                if (currentFrame >= TotalFrames)
                    currentFrame = 0;
            }
        }

        public Bitmap CurrentFrame()
        {
            return sheet.GetFrame(currentFrame, Row);
        }
    }
}