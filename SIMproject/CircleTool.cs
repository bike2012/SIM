using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SIMproject
{
    class CircleTool : BaseTool
    {
        public override void mouseDown(PointF p1)
        {
            float x = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            ImageShow.addCircle(new PointF(x,y), 1);
            if (figureDetect.setCircleParaState)
            {
                figureDetect.shapeIndex = ImageShow.shapes.Count - 1;
            }
        }

        public override void mouseUp(PointF p3)
        {
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {
            float x = (p1.X - ImageShow.position.X) / ImageShow.scale;
            float y = (p1.Y - ImageShow.position.Y) / ImageShow.scale;
            float xr = (p3.X - ImageShow.position.X) / ImageShow.scale;
            float yr = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
            ImageShow.editLast(new PointF(x, y), new PointF(x, y), (float)Math.Sqrt((xr - x) * (xr - x) + (yr - y) * (yr - y)));
        }

        public override double mouseClick(PointF p1)
        {
            return 0;
        }
    }
}
