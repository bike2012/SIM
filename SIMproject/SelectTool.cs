using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class SelectTool : BaseTool
    {
        private int selectState = 0;
        private int selectShapeIndex = -1;

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
            foreach (var shape in ImageShow.shapes)
            {
                selectState = shape.onShape(p1);
                if (selectState%10 == 1)
                {
                    selectShapeIndex = ImageShow.shapes.IndexOf(shape);
                    if (DistanceMeasure.pointSelectState > 0)
                        DistanceMeasure.shapeIndex = selectShapeIndex;
                    if (DistanceMeasure.lineSelectState > 0)
                        DistanceMeasure.lineShapeIndex = selectShapeIndex;
                    break;
                }
                if (selectState > 10 && DistanceMeasure.lineSelectState > 0)
                    DistanceMeasure.lineShapeIndex = ImageShow.shapes.IndexOf(shape);
            }
            return 0;
        }
    }
}
