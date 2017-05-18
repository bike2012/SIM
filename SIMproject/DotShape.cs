using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class DotShape : BaseShape
    {
        public override void draw(Graphics g)
        {
            float x = P1.X * ImageShow.scale + ImageShow.position.X;
            float y = P1.Y * ImageShow.scale + ImageShow.position.Y;
            g.DrawLine(Config.dotPen, new PointF(x, y), new PointF(x + 1, y + 1));
        }

        public override void setPara(PointF p1, PointF p2, float r, PointF p3)
        {
            P1 = p1; P2 = p2; R = r;
        }

        public override void setPartPara(int type, PointF p1, PointF p2, float r)
        {
        }

        public override void getPara(ref float para1, ref float para2, ref float para3, ref float para4, ref float para5)
        {
        }

        public override int onShape(PointF p1)
        {
            return 0;
        }
    }
}
