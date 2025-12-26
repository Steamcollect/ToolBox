using UnityEngine;

namespace MVsToolkit.Utils
{
    public static class TextureUtils
    {
        public static Texture2D MakeColorTex(int width, int height, Color col)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = col;

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}