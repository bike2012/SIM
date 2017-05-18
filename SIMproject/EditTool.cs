using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class EditTool : BaseTool
    {
        private int editState = 0;
        private int editShapeIndex = 0;

        public override void mouseDown(PointF p1)
        {
            foreach (var shape in ImageShow.shapes)
            {
                editState = shape.onShape(p1);
                if (editState > 0)
                {
                    editShapeIndex = ImageShow.shapes.IndexOf(shape);
                    break;
                }
            }
        }

        public override void mouseUp(PointF p3)
        {
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {
            if (editState%10 == 1)
            {
                float xr = (p3.X - ImageShow.position.X) / ImageShow.scale;
                float yr = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
                ImageShow.shapes[editShapeIndex].setPara(new PointF(xr, yr), new PointF(xr - ImageShow.shapes[editShapeIndex].P1.X + ImageShow.shapes[editShapeIndex].P2.X, yr - ImageShow.shapes[editShapeIndex].P1.Y + ImageShow.shapes[editShapeIndex].P2.Y), ImageShow.shapes[editShapeIndex].R, new PointF(0,0));
            }
            else if (editState%10 == 2)
            {
                float x = ImageShow.shapes[editShapeIndex].P1.X;
                float y = ImageShow.shapes[editShapeIndex].P1.Y;
                float xr = (p3.X - ImageShow.position.X) / ImageShow.scale;
                float yr = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
                ImageShow.shapes[editShapeIndex].setPara(ImageShow.shapes[editShapeIndex].P1, ImageShow.shapes[editShapeIndex].P2, (float)Math.Sqrt((xr - x) * (xr - x) + (yr - y) * (yr - y)), new PointF(0, 0));
            }
            else if (editState%10 == 3)
            {
                float x = (p3.X - ImageShow.position.X) / ImageShow.scale;
                float y = (p3.Y - ImageShow.position.Y) / ImageShow.scale; 
                float xr = x - ImageShow.shapes[editShapeIndex].P1.X;
                float yr = y - ImageShow.shapes[editShapeIndex].P1.Y;

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
                ImageShow.shapes[editShapeIndex].setPartPara(1,ImageShow.shapes[editShapeIndex].P1,new PointF(x,y) , r);
                
            }
            else if (editState%10 == 4)
            {
                float xr = (p3.X - ImageShow.position.X) / ImageShow.scale;
                float yr = (p3.Y - ImageShow.position.Y) / ImageShow.scale;
                ImageShow.shapes[editShapeIndex].setPartPara(2,ImageShow.shapes[editShapeIndex].P1, new PointF(xr, yr), ImageShow.shapes[editShapeIndex].R);
            }
        }

        public override double mouseClick(PointF p1)
        {
            return 0;
        }
    }
}
