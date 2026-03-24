using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Media.Imaging;

namespace ImagePixel.utils
{
    public class Pixelizer
    {
        public enum PixelMethod
        {
            Average,
            Contrast,
            FirstPixel
        }


        public BitmapImage ToBitmapImage(Image<Rgba32> image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
        }


        public static Image<Rgba32> Pixelate(Image<Rgba32> source, int pixelSize, bool keepSize, PixelMethod method = PixelMethod.Average)
        {
            if (pixelSize <= 1)
            {
                return source.Clone();
            }

            if (keepSize)
            {
                return PixelateKeepSize(source, pixelSize, method);
            }
            else
            {
                return PixelateReduceSize(source, pixelSize, method);
            }
        }


        private static Image<Rgba32> PixelateKeepSize(Image<Rgba32> source, int pixelSize, PixelMethod method)
        {
            Image<Rgba32> result = source.Clone();

            for (int y = 0; y < source.Height; y += pixelSize)
            {
                for (int x = 0; x < source.Width; x += pixelSize)
                {
                    Rgba32 blockColor = GetBlockColor(source, x, y, pixelSize, method);

                    int limitY = y + pixelSize;
                    if (limitY > source.Height) limitY = source.Height;
                    int limitX = x + pixelSize;
                    if (limitX > source.Width) limitX = source.Width;

                    for (int yy = y; yy < limitY; yy++)
                    {
                        for (int xx = x; xx < limitX; xx++)
                        {
                            result[xx, yy] = blockColor;
                        }
                    }
                }
            }

            return result;
        }


        private static Image<Rgba32> PixelateReduceSize(Image<Rgba32> source, int pixelSize, PixelMethod method)
        {
            int newWidth = source.Width / pixelSize;
            if (newWidth < 1) newWidth = 1;
            int newHeight = source.Height / pixelSize;
            if (newHeight < 1) newHeight = 1;

            Image<Rgba32> result = new Image<Rgba32>(newWidth, newHeight);

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int startX = x * pixelSize;
                    int startY = y * pixelSize;

                    Rgba32 blockColor = GetBlockColor(source, startX, startY, pixelSize, method);
                    result[x, y] = blockColor;
                }
            }

            return result;
        }


        private static Rgba32 GetBlockColor(Image<Rgba32> source, int startX, int startY, int pixelSize, PixelMethod method)
        {
            if (method == PixelMethod.Average)
            {
                return GetAverageColor(source, startX, startY, pixelSize);
            }

            else if (method == PixelMethod.Contrast)
            {
                return GetContrastColor(source, startX, startY, pixelSize);
            }


            return GetFirstPixelColor(source, startX, startY);
        }


        private static Rgba32 GetAverageColor(Image<Rgba32> source, int startX, int startY, int pixelSize)
        {
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            int count = 0;

            for (int y = startY; y < startY + pixelSize && y < source.Height; y++)
            {
                for (int x = startX; x < startX + pixelSize && x < source.Width; x++)
                {
                    Rgba32 p = source[x, y];
                    sumR += p.R;
                    sumG += p.G;
                    sumB += p.B;
                    sumA += p.A;
                    count++;
                }
            }

            if (count == 0)
            {
                return new Rgba32(0, 0, 0, 255);
            }

            byte avgR = (byte)(sumR / count);
            byte avgG = (byte)(sumG / count);
            byte avgB = (byte)(sumB / count);
            byte avgA = (byte)(sumA / count);

            return new Rgba32(avgR, avgG, avgB, avgA);
        }


        private static Rgba32 GetContrastColor(Image<Rgba32> source, int startX, int startY, int pixelSize)
        {
            Rgba32[] palette = new Rgba32[]
            {
                new Rgba32(255, 0, 0),
                new Rgba32(0, 255, 0),
                new Rgba32(0, 0, 255),
                new Rgba32(255, 255, 0),
                new Rgba32(255, 0, 255),
                new Rgba32(0, 255, 255),
                new Rgba32(255, 255, 255),
                new Rgba32(0, 0, 0)
            };

            double bestDistance = double.MaxValue;
            Rgba32 bestColor = palette[0];

            for (int y = startY; y < startY + pixelSize && y < source.Height; y++)
            {
                for (int x = startX; x < startX + pixelSize && x < source.Width; x++)
                {
                    Rgba32 p = source[x, y];

                    for (int i = 0; i < palette.Length; i++)
                    {
                        Rgba32 c = palette[i];
                        double dist = ColorDistance(p, c);
                        if (dist < bestDistance)
                        {
                            bestDistance = dist;
                            bestColor = c;
                        }
                    }
                }
            }

            return bestColor;
        }

        private static Rgba32 GetFirstPixelColor(Image<Rgba32> source, int startX, int startY)
        {
            if (startX >= 0 && startX < source.Width && startY >= 0 && startY < source.Height)
            {
                return source[startX, startY];
            }
            return new Rgba32(0, 0, 0, 255);
        }

        private static double ColorDistance(Rgba32 a, Rgba32 b)
        {
            int dr = a.R - b.R;
            int dg = a.G - b.G;
            int db = a.B - b.B;
            return Math.Sqrt(dr * dr + dg * dg + db * db);
        }
    }
}
