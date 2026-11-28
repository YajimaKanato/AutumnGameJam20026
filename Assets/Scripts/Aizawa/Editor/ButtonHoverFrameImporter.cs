using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aizawa
{
    sealed class ButtonHoverFrameImporter : AssetPostprocessor
    {
        const string FrameFolder = "Assets/Ignore/デザイナー/02_UI/ボタン/ホバーアニメーション/";

        void OnPostprocessTexture(Texture2D texture)
        {
            if (!assetPath.StartsWith(FrameFolder, System.StringComparison.Ordinal)) return;

            var pixels = texture.GetPixels32();
            int width = texture.width;
            int height = texture.height;
            var background = new bool[pixels.Length];
            var pending = new Queue<int>();

            void Visit(int x, int y)
            {
                int index = y * width + x;
                var pixel = pixels[index];
                if (background[index] || pixel.r != 0 || pixel.g != 0 || pixel.b != 0) return;

                background[index] = true;
                pending.Enqueue(index);
            }

            for (int x = 0; x < width; x++)
            {
                Visit(x, 0);
                Visit(x, height - 1);
            }

            for (int y = 0; y < height; y++)
            {
                Visit(0, y);
                Visit(width - 1, y);
            }

            while (pending.Count > 0)
            {
                int index = pending.Dequeue();
                int x = index % width;
                int y = index / width;
                if (x > 0) Visit(x - 1, y);
                if (x + 1 < width) Visit(x + 1, y);
                if (y > 0) Visit(x, y - 1);
                if (y + 1 < height) Visit(x, y + 1);
            }

            // Keep a one-pixel black outline at the edge of the artwork.
            for (int index = 0; index < pixels.Length; index++)
            {
                if (!background[index]) continue;
                int x = index % width;
                int y = index / width;
                if ((x > 0 && !background[index - 1]) || (x + 1 < width && !background[index + 1]) ||
                    (y > 0 && !background[index - width]) || (y + 1 < height && !background[index + width]))
                    continue;

                pixels[index].a = 0;
            }

            texture.SetPixels32(pixels);
            texture.Apply(false);
        }
    }
}
