using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class RectTool : BaseTool
    {
        public override void mouseDown(PointF p1)
        {
            float x1 = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y1 = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            ImageShow.addRect(new PointF(x1, y1), new PointF(x1, y1), 0, true);
            if (figureDetect.setRectParaState)
            {
                figureDetect.shapeIndex = ImageShow.shapes.Count - 1;
            }
            if (CodeRecog.setRectParaState > 0)
            {
                CodeRecog.shapeIndex = ImageShow.shapes.Count - 1;
            }
        }

        public override void mouseUp(PointF p3)
        {
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {
            float x1 = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y1 = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            float x2 = (p3.X - ImageShow.position.X) / ImageShow.scale;
            float y2 = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
            ImageShow.editLast(new PointF(x1, y1),new PointF(x2, y2), 0);
        }

        public override double mouseClick(PointF p1)
        {
            return 0;
        }
    }
}
