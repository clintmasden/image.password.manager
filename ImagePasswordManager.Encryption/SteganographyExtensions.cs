using System.Drawing;
using System.Drawing.Imaging;

namespace ImagePasswordManager.Encryption
{
    public static class SteganographyExtensions
    {
        /// <summary>
        ///     Creates a bitmap from an image without indexed pixels
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Bitmap CreateNonIndexedImage(Image src)
        {
            var newBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);

            using (var gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public static Bitmap SaveString(string text, Bitmap bitmap)
        {
            var s = State.HIDING;

            var charIndex = 0;
            var charValue = 0;
            long colorUnitIndex = 0;

            var zeros = 0;

            int R = 0, G = 0, B = 0;

            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);

                    pixel = Color.FromArgb(pixel.R - pixel.R % 2,
                        pixel.G - pixel.G % 2, pixel.B - pixel.B % 2);

                    R = pixel.R;
                    G = pixel.G;
                    B = pixel.B;

                    for (var n = 0; n < 3; n++)
                    {
                        if (colorUnitIndex % 8 == 0)
                        {
                            if (zeros == 8)
                            {
                                if ((colorUnitIndex - 1) % 3 < 2)
                                {
                                    bitmap.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }

                                return bitmap;
                            }

                            if (charIndex >= text.Length)
                            {
                                s = State.FILL_WITH_ZEROS;
                            }
                            else
                            {
                                charValue = text[charIndex++];
                            }
                        }

                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    if (s == State.HIDING)
                                    {
                                        R += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;

                            case 1:
                                {
                                    if (s == State.HIDING)
                                    {
                                        G += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;

                            case 2:
                                {
                                    if (s == State.HIDING)
                                    {
                                        B += charValue % 2;

                                        charValue /= 2;
                                    }

                                    bitmap.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (s == State.FILL_WITH_ZEROS)
                        {
                            zeros++;
                        }
                    }
                }
            }

            return bitmap;
        }

        public static string GetString(Bitmap bitmap)
        {
            var colorUnitIndex = 0;
            var charValue = 0;

            var extractedText = string.Empty;

            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);

                    for (var n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;

                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;

                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (colorUnitIndex % 8 == 0)
                        {
                            charValue = ReverseBits(charValue);

                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            var c = (char)charValue;

                            extractedText += c.ToString();
                        }
                    }
                }
            }

            return extractedText;
        }

        private static int ReverseBits(int n)
        {
            var result = 0;

            for (var i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }

        private enum State
        {
            HIDING,
            FILL_WITH_ZEROS
        }
    }
}