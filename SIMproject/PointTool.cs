using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class PointTool : BaseTool
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
            float x = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y = (p1.Y - ImageShow.position.Y) / ImageShow.scale;  

            List<int> lines = new List<int>();
            foreach (var shape in ImageShow.shapes)
                if (shape.onShape(p1) > 10)
                    lines.Add(ImageShow.shapes.IndexOf(shape));

            if (lines.Count > 1)
            {
                float x0 = ImageShow.shapes[lines[0]].P1.X;
                float y0 = ImageShow.shapes[lines[0]].P1.Y;
                float x1 = ImageShow.shapes[lines[1]].P1.X;
                float y1 = ImageShow.shapes[lines[1]].P1.Y;
                float angle0 = ImageShow.shapes[lines[0]].R;
                float angle1 = ImageShow.shapes[lines[1]].R;

                float c0 = (float)Math.Cos(angle0), s0 = (float)Math.Sin(angle0);
                float c1 = (float)Math.Cos(angle1), s1 = (float)Math.Sin(angle1);

                if (c0 == 0)
                {
                    if (c1 != 0)
                    {
                        x = x0;
                        y = (x - x1) * s1 / c1 + y1;
                    }
                }
                else if (c1 == 0)
                {
                    x = x1;
                    y = (x - x0) * s0 / c0 + y0;
                }
                else
                {
                    float k0 = s0 / c0;
                    float k1 = s1 / c1;

                    if (k0 != k1)
                    {
                        float b0 = y0 - x0 * k0;
                        float b1 = y1 - x1 * k1;
                        x = (b0 - b1) / (k1 - k0);
                        y = x * k0 + b0;
                    }
                }
                
            }
          
            ImageShow.addPoint(new PointF(x, y));

            if (DistanceMeasure.pointSelectState > 0)
            {
                DistanceMeasure.shapeIndex = ImageShow.shapes.Count - 1;
            }


            if (CameraCalibrate.manualExtractState > 0)
            {
                CameraCalibrate.manualCorner[4 - CameraCalibrate.manualExtractState] = new PointF(x, y);
                CameraCalibrate.manualExtractState--;
                if (CameraCalibrate.manualExtractState == 0)
                {
                    return CameraCalibrate.runManualExtract();
                }
            }

            if (DefectDetect.selectPointState > 0)
            {
                DefectDetect.selectPointIndex.Add(new PointF(x,y));
                //DefectDetect.selectPointState--;
            }
            if (DefectDetect.tempSelectPointState > 0)
            {
                DefectDetect.tempSelectPointIndex.Add(new PointF(x, y));
                //DefectDetect.tempSelectPointState--;
            }
            return 0;
        }
    }
}
