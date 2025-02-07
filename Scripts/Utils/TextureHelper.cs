using UnityEngine;

namespace MultiResTester.Utility
{
    public class TextureHelper
    {
        public static Texture2D DrawRedRectangle(Texture2D texture, int x, int y, int width, int height,Color fillColor)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null!");
                return null;
            }

            Color[] pixels = texture.GetPixels();

            int texWidth = texture.width;

            // Loop through the pixels and set the red color for the rectangle border
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int pixelIndex = (y + j) * texWidth + (x + i);

                    // Ensure we don't go out of bounds
                    if (pixelIndex >= 0 && pixelIndex < pixels.Length)
                    {
                        pixels[pixelIndex] = fillColor;
                    }
                }
            }

            // Apply changes
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }
    }
}