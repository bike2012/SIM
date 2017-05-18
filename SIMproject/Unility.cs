using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class Unility
    {
        public static void drawRect(Bitmap pic, Rectangle rect, Color color)
        {
            for (int i = rect.X; i <= rect.X + rect.Width; i++)
            {
                pic.SetPixel(i, rect.Y, color);
                pic.SetPixel(i, rect.Y + rect.Height, color);
            }

            for (int i = rect.Y; i < rect.Y + rect.Height; i++)
            {
                pic.SetPixel(rect.X, i, color);
                pic.SetPixel(rect.X + rect.Width, i, color);
            }

        }
    }
}
