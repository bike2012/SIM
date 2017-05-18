using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class ColorTool : BaseTool
    {
        public override void mouseDown(PointF p1)
        {

        }

        public override void mouseUp(PointF p3)
        {
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {

        }

        public override double mouseClick(PointF p1)
        {
            int x = (int)((p1.X - ImageShow.position.X) / ImageShow.scale);
            int y = (int)((p1.Y - ImageShow.position.Y) / ImageShow.scale);
            if (ImageShow.source != null)
                if (x >= 0 && x < ImageShow.source.Width && y >= 0 && y < ImageShow.source.Height)
                {
                    ColorRecog.r = ImageShow.source.GetPixel(x, y).R;
                    ColorRecog.g = ImageShow.source.GetPixel(x, y).G;
                    ColorRecog.b = ImageShow.source.GetPixel(x, y).B;
                }
            return ColorRecog.callColorRecog();
        }
    }
}
