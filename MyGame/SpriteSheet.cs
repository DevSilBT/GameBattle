using System;
using System.Drawing;
using System.IO;

namespace MyGame
{
    class SpriteSheet
    {
        private Bitmap sheet;
        public int FrameWidth => Config.FrameW;
        public int FrameHeight => Config.FrameH;

        public SpriteSheet(string path)
        {
            try
            {
                // Verificar si el archivo existe
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se encontró el archivo: {path}");
                }

                // Verificar que el archivo no esté vacío
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Length == 0)
                {
                    throw new InvalidOperationException($"El archivo está vacío: {path}");
                }

                // Cargar la imagen con using para manejo seguro
                using (var tempBitmap = new Bitmap(path))
                {
                    sheet = new Bitmap(tempBitmap); // Crear una copia
                }

                // Verificar dimensiones del spritesheet
                if (sheet.Width < FrameWidth || sheet.Height < FrameHeight)
                {
                    throw new InvalidOperationException(
                        $"El spritesheet es muy pequeño. Tamaño: {sheet.Width}x{sheet.Height}, " +
                        $"se esperaba al menos: {FrameWidth}x{FrameHeight}"
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cargando spritesheet desde '{path}': {ex.Message}", ex);
            }
        }

        public Bitmap GetFrame(int col, int row)
        {
            // Validar índices
            if (col < 0 || row < 0 || 
                (col + 1) * FrameWidth > sheet.Width || 
                (row + 1) * FrameHeight > sheet.Height)
            {
                // Crear un frame de fallback
                return CreateFallbackFrame();
            }

            var bmp = new Bitmap(FrameWidth, FrameHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                var src = new Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
                g.DrawImage(sheet, new Rectangle(0, 0, FrameWidth, FrameHeight), src, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        private Bitmap CreateFallbackFrame()
        {
            var bmp = new Bitmap(FrameWidth, FrameHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Magenta); // Color fácil de identificar
                g.DrawRectangle(Pens.Red, 0, 0, FrameWidth - 1, FrameHeight - 1);
                g.DrawString("ERROR", new Font("Arial", 8), Brushes.Black, 2, 2);
            }
            return bmp;
        }

        // Dispose para liberar recursos
        public void Dispose()
        {
            sheet?.Dispose();
        }
    }
}