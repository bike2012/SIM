using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class LineTool : BaseTool
    {
        public override void mouseDown(PointF p1)
        {
            float x = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            ImageShow.addLine(new PointF(x, y), 100);

            if (DefectDetect.selectLineState)
            {
                DefectDetect.selectLineIndex = ImageShow.shapes.Count - 1;
            }
        }

        public override void mouseUp(PointF p3)
        {
            if (DefectDetect.selectLineState)
            {
                //DefectDetect.selectLineState = false;
                DefectDetect.detectLine();
            }

            if (DistanceMeasure.lineSelectState > 0)
            {
                DistanceMeasure.lineShapeIndex = ImageShow.shapes.Count - 1;
            }
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {
            float x1 = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y1 = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            float x2 = (p3.X - ImageShow.position.X) / ImageShow.scale;
            float y2 = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
            float xr = x2 - x1;
            float yr = y2-y1;

            float r = 0;
            if (xr == 0)
            {
                if (yr >= 0) r = (float)Math.PI / 2;
                else r = (float)Math.PI / 2 * 3;
            }
            else
            {
                r = (float)Math.Atan(yr / xr);
                if (xr < 0)
                {
                    r += (float)Math.PI;
                }
                else
                {
                    if (yr < 0)
                        r += (float)Math.PI * 2;
                }
            }
            ImageShow.editLast(new PointF(x1, y1), new PointF(x2, y2), r);
        }

        public override double mouseClick(PointF p1)
        {
            return 0;
        }
    }
}
