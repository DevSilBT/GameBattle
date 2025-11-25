using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public static class Config
{
    // ==== Rutas ====
    public static readonly string AssetsPath =
        Path.Combine(Application.StartupPath, "Assets");

    // ==== Tamaños de frame ====
    // Ajusta estos valores a tu Player y Enemy
    public static int FrameW = 64;      // ancho de cada frame
    public static int FrameH = 64;      // alto de cada frame
    public static float Scale = 2.0f;   // escalado del personaje

    // ==== Spritesheet ====
    public static int Columns = 4;  // número de columnas del sheet principal

    // ==== Cargar imagen simple ====
    public static Bitmap LoadImage(string fileName)
    {
        string fullPath = Path.Combine(AssetsPath, fileName);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                $"No se encontró el archivo {fileName} en {AssetsPath}"
            );

        using (var temp = new Bitmap(fullPath))
            return new Bitmap(temp);
    }

    // ==== Cortar un spritesheet en frames ====
    public static Bitmap[] LoadSpriteSheet(string fileName, int frameWidth, int frameHeight)
    {
        Bitmap sheet = LoadImage(fileName);

        int framesX = sheet.Width / frameWidth;
        int framesY = sheet.Height / frameHeight;
        int totalFrames = framesX * framesY;

        Bitmap[] frames = new Bitmap[totalFrames];
        int index = 0;

        for (int y = 0; y < framesY; y++)
        {
            for (int x = 0; x < framesX; x++)
            {
                Rectangle src = new Rectangle(
                    x * frameWidth,
                    y * frameHeight,
                    frameWidth,
                    frameHeight
                );

                if (src.Right > sheet.Width || src.Bottom > sheet.Height)
                    throw new Exception("Dimensiones de spritesheet incorrectas.");

                frames[index] = sheet.Clone(src, sheet.PixelFormat);
                index++;
            }
        }

        return frames;
    }
}
