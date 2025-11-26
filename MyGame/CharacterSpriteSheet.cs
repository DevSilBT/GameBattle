// CharacterSpriteSheet.cs
using System;
using System.Drawing;

namespace MyGame
{
    public class CharacterSpriteSheet
    {
        private Bitmap sheet;
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }

        public CharacterSpriteSheet(string path, int frameWidth, int frameHeight)
        {
            this.sheet = new Bitmap(path);
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
        }

        public Bitmap GetFrame(int col, int row)
        {
            var bmp = new Bitmap(FrameWidth, FrameHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                var src = new Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
                g.DrawImage(sheet, new Rectangle(0, 0, FrameWidth, FrameHeight), src, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}