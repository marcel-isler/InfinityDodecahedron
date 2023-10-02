using ColorMine.ColorSpaces;
using System;

namespace DodecahedronHandler
{
    public class LedBuffer
    {
        public int NumberOfLeds { get; }
        public Rgb[] LedValues { get; private set; }

        public LedBuffer(int numberOfLeds)
        {
            NumberOfLeds = numberOfLeds;
            ClearBuffer();
        }

        public void ClearBuffer()
        {
            LedValues = new Rgb[NumberOfLeds];
            for (int i = 0; i < NumberOfLeds; i++)
            {
                LedValues[i] = new Rgb();
            }
        }

        public void SetPixel(int pixelLocation, Rgb color)
        {
            if (pixelLocation < NumberOfLeds)
            {
                // Use the higher RGB values
                LedValues[pixelLocation].R = Math.Max(LedValues[pixelLocation].R, color.R);
                LedValues[pixelLocation].G = Math.Max(LedValues[pixelLocation].G, color.G);
                LedValues[pixelLocation].B = Math.Max(LedValues[pixelLocation].B, color.B);
            }
        }

        public void ClearPixel(int pixelLocation)
        {
            LedValues[pixelLocation].R = 0.0;
            LedValues[pixelLocation].G = 0.0;
            LedValues[pixelLocation].B = 0.0;
        }

        public int GetNextPixel(int pixelLocation)
        {
            int nextPixel = pixelLocation + 1;
            if (nextPixel >= NumberOfLeds)
            {
                nextPixel = 0;
            }

            return nextPixel;
        }
    }
}
